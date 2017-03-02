using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.PLCCompiler
{
    public abstract class ExpGraphNode
    {
        public LinkedList<ExpGraphNode> NextNodes = new LinkedList<ExpGraphNode>();
        public LinkedList<ExpGraphNode> ParentNodes = new LinkedList<ExpGraphNode>();
        public HashSet<ExpGraphNode> SubNodesSet = new HashSet<ExpGraphNode>();
        public virtual ElementType Type { get; }
        public virtual int MaxNextNodeCount { get; }
        public bool IsVisited { get; set; }

        public ExpGraphNode()
        {

        }

        public void AddNextNode(ExpGraphNode node)
        {
            NextNodes.AddLast(node);
            node.ParentNodes.AddLast(this);
        }

        public abstract ExpTreeNode CreateTreeNode();
        /// <summary>
        /// 调用前需要清除IsVisited标志
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public HashSet<ExpGraphNode> FindSubNodes(LinkedList<ExpGraphNode> nodes)
        {
            if(IsVisited)
            {
                return SubNodesSet;
            }
            IsVisited = true;
            SubNodesSet.Clear();
            if(nodes.Contains(this))
            {
                SubNodesSet.Add(this);
                return SubNodesSet;
            }
            foreach(var pnode in NextNodes)
            {
                SubNodesSet.UnionWith(pnode.FindSubNodes(nodes));
            }
            return SubNodesSet;
        }

        public int GetSubNodesCount()
        {
            return SubNodesSet.Count;
        }

        public virtual bool GetPublicParentCondition()
        {
            if (SubNodesSet.Contains(this))
            {
                return true;
            }
            if (SubNodesSet.Count == 0)
            {
                return false;
            }
            int c = NextNodes.Aggregate(1, (current, node) => current * node.GetSubNodesCount());
            return c > 0;
        }

        public bool GetExchangeCondition()
        {
            int c = SubNodesSet.Aggregate(1, (current, node) => current * node.SubNodesSet.Count);
            return c == 0;
        }


    }
}
