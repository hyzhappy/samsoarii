using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.PLCCompiler
{
    public class GraphHelper
    {
        public static void DisconnectGraphNode(ExpGraphNode parNode, ExpGraphNode childNode)
        {
            parNode.NextNodes.Remove(childNode);
            childNode.ParentNodes.Remove(parNode);
        }
        public static void ReplaceGraphNode(ExpGraphNode parNode, ExpGraphNode childNode, ExpGraphNode newNode)
        {
            childNode.ParentNodes.Remove(parNode);
            var subIter = parNode.NextNodes.Find(childNode);
            if (subIter != null)
                parNode.NextNodes.AddAfter(subIter, newNode);
            parNode.NextNodes.Remove(childNode);
            newNode.ParentNodes.AddLast(parNode);
        }

        public static ExpTree ConvertTreeByGraph(ExpGraph graph)
        {
            ExpTree result = new ExpTree();
            graph.Convert();
            Dictionary<ExpGraphNode, ExpTreeNode> _gnode_tnode_map = new Dictionary<ExpGraphNode, ExpTreeNode>();
            foreach(var rnode in graph.GetRootNodes())
            {
                var tnode = ConvertGraphNode(rnode, _gnode_tnode_map);
                result.AddRootNode(tnode);
            }
            foreach(var tnode in _gnode_tnode_map.Values)
            {
                result.AddGenNode(tnode);
            }
            return result;
        }

        private static ExpTreeNode ConvertGraphNode(ExpGraphNode gnode, Dictionary<ExpGraphNode, ExpTreeNode> _gnode_tnode_map)
        {
            ExpTreeNode tnode;
            if(_gnode_tnode_map.TryGetValue(gnode, out tnode))
            {
                return tnode;
            }
            else
            {
                tnode = gnode.CreateTreeNode();
                if (tnode.Type != ElementType.Output)
                    _gnode_tnode_map.Add(gnode, tnode);
                if(gnode.NextNodes.Count == 0)
                {
                    return tnode;
                }
                else
                {
                    foreach(var _next_gnode in gnode.NextNodes)
                    {
                        var _n_tnode = ConvertGraphNode(_next_gnode, _gnode_tnode_map);
                        if (tnode.LeftChild == null)
                        {
                            tnode.LeftChild = _n_tnode;
                        }
                        else
                        {
                            if (tnode.RightChild == null)
                            {
                                tnode.RightChild = _n_tnode;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                    return tnode;
                }
            }
        }
    }
}
