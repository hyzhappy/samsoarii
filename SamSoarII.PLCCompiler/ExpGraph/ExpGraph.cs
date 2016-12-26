using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
namespace SamSoarII.PLCCompiler
{
    public class ExpGraph
    {
        private LinkedList<ExpGraphNode> _rootNodes = new LinkedList<ExpGraphNode>();
        private LinkedList<ExpGraphNode> _genNodes = new LinkedList<ExpGraphNode>();
        public ExpGraph()
        {

        }

        public void AddRootNode(ExpGraphNode node)
        {
            _rootNodes.AddLast(node);
        }

        public void AddGenNode(ExpGraphNode node)
        {
            _genNodes.AddLast(node);
        }

        public LinkedList<ExpGraphNode> GetRootNodes()
        {
            return _rootNodes;
        }

        public void ClearVisitFlags()
        {
            foreach(var node in _rootNodes)
            {
                node.IsVisited = false;
            }
            foreach(var node in _genNodes)
            {
                node.IsVisited = false;
            }
        }

        public void FindSubNodes(LinkedList<ExpGraphNode> nodes)
        {
            ClearVisitFlags();
            foreach(var rnode in _rootNodes)
            {
                rnode.FindSubNodes(nodes);
            }
        }

        public ExpGraphNode SearchPublicParentNode(ExpGraphNode startNode)
        {
            ClearVisitFlags();
            Queue<ExpGraphNode> tempQueue = new Queue<ExpGraphNode>();
            tempQueue.Enqueue(startNode);
            while (tempQueue.Count > 0)
            {
                var node = tempQueue.Dequeue();
                if (node.GetPublicParentCondition())
                {
                    return node;
                }
                foreach (var nextNode in node.NextNodes)
                {
                    if (!nextNode.IsVisited)
                    {
                        nextNode.IsVisited = true;
                        tempQueue.Enqueue(nextNode);
                    }
                }
            }
            return null;
        }
        public Queue<ExpGraphNode> GetBFSSequence()
        {
            ClearVisitFlags();
            Queue<ExpGraphNode> tempQueue = new Queue<ExpGraphNode>();
            Queue<ExpGraphNode> result = new Queue<ExpGraphNode>();
            foreach (var rnode in _rootNodes)
            {
                tempQueue.Enqueue(rnode);
                while (tempQueue.Count > 0)
                {
                    var node = tempQueue.Dequeue();

                    result.Enqueue(node);

                    foreach (var nextnode in node.NextNodes)
                    {
                        if (!nextnode.IsVisited)
                        {
                            tempQueue.Enqueue(nextnode);
                            nextnode.IsVisited = true;
                        }
                    }
                }
            }

            return result;
        }
        public void Convert()
        {
            var tempQueue = GetBFSSequence();
            ExpGraphNode rnode = _rootNodes.First.Value;
            ClearVisitFlags();
            while (tempQueue.Count > 0)
            {
                var node = tempQueue.Dequeue();
                if (_rootNodes.Contains(node))
                {
                    rnode = node;
                }
                while (node.NextNodes.Count > node.MaxNextNodeCount)
                {
                    //多于一个子节点， 生成OR节点
                    if (node.NextNodes.Count > 1)
                    {
                        //提取前两个节点，用于生成OR节点
                        var fnode1 = node.NextNodes.First.Value;
                        node.NextNodes.RemoveFirst();
                        var fnode2 = node.NextNodes.First.Value;
                        node.NextNodes.RemoveFirst();
                        //生成OR节点
                        var ornode = new ExpGraphOrNode();
                        _genNodes.AddLast(ornode);
                        //找到两个节点的所有公共父节点, 根据某定理，这两个节点的父节点集合是相同的
                        List<ExpGraphNode> pparnodes = fnode1.ParentNodes.ToList();
                        foreach (var pnode in pparnodes)
                        {
                            //父子节点断开
                            GraphHelper.DisconnectGraphNode(pnode, fnode1);
                            GraphHelper.DisconnectGraphNode(pnode, fnode2);
                            //父节点链表前插OR节点
                            pnode.NextNodes.AddFirst(ornode);
                            ornode.ParentNodes.AddLast(pnode);
                        }
                        //OR将两个节点加入子节点
                        ornode.AddNextNode(fnode1);
                        ornode.AddNextNode(fnode2);
                    }
                    else
                    {
                        //当前节点类型为Operand，且只有一个子节点，需要生成若干And节点
                        if (node.Type == ElementType.Input)
                        {
                            //AND的左节点
                            var andLeftNode = node.NextNodes.First.Value;
                            FindSubNodes(andLeftNode.ParentNodes);
                            //foreach (var rnode in _rootNodes)
                            //{

                            if (rnode.GetSubNodesCount() > 0)
                            {
                                //AND的右节点
                                var andRightNode = SearchPublicParentNode(rnode);
                                //判断是否交换OR节点位置
                                if (andRightNode.Type == ElementType.Or)
                                {
                                    var exchangeNode = andRightNode;
                                    //向左边寻找OR节点，直到不是OR节点为止
                                    while (exchangeNode.NextNodes.First.Value.Type == ElementType.Or)
                                    {
                                        exchangeNode = exchangeNode.NextNodes.First.Value;
                                    }
                                    while (exchangeNode != andRightNode)
                                    {
                                        //需要进行节点旋转
                                        if (!exchangeNode.GetPublicParentCondition())
                                        {
                                            
                                            var exchangeParentNode = exchangeNode.ParentNodes.First.Value;
                                            var exchangeSubRightNode = exchangeNode.NextNodes.Last.Value;
                                            GraphHelper.ReplaceGraphNode(exchangeParentNode, exchangeNode, exchangeSubRightNode);
                                            GraphHelper.DisconnectGraphNode(exchangeNode, exchangeSubRightNode);
                                            foreach (var pnode in exchangeParentNode.ParentNodes.ToList())
                                            {
                                                GraphHelper.ReplaceGraphNode(pnode, exchangeParentNode, exchangeNode);
                                            }
                                            exchangeNode.AddNextNode(exchangeParentNode);
                                            if (exchangeNode.NextNodes.Contains(andRightNode))
                                            {
                                                break;
                                            }
                                            continue;
                                        }
                                        exchangeNode = exchangeNode.ParentNodes.First.Value;
                                    }
                                }

                                //生成AND节点
                                var andNode = new ExpGraphAndNode();
                                foreach (var pnode in andRightNode.ParentNodes.ToList())
                                {
                                    GraphHelper.ReplaceGraphNode(pnode, andRightNode, andNode);
                                }
                                //不能断开所有父节点，仅断开与rnode相关的父节点
                                foreach (var pnode in rnode.SubNodesSet.ToList())
                                {
                                    GraphHelper.DisconnectGraphNode(pnode, andLeftNode);
                                }
                                //AND节点将两个节点加入子节点
                                andNode.AddNextNode(andLeftNode);
                                andNode.AddNextNode(andRightNode);
                                _genNodes.AddLast(andNode);
                            }
                            //}
                        }
                    }
                }
            }
        }
        public ExpTree ConvertToTree()
        {
            return GraphHelper.ConvertTreeByGraph(this);
        }
    }
}
