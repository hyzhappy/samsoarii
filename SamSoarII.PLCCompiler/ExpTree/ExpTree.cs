using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCCompiler
{
    public class ExpTree
    {
        private List<ExpTreeNode> _rootNodes = new List<ExpTreeNode>();
        private List<ExpTreeNode> _genNodes = new List<ExpTreeNode>();
        public string TreeName { get; set; }
        public int VaribleNameIndex = 0;
        public ExpTree()
        {

        }
        public ExpTree(string name)
        {
            TreeName = name;
        }
        public void ClearSearchedFlag()
        {
            foreach(var node in _rootNodes)
            {
                node.IsSearched = false;
            }
            foreach(var node in _genNodes)
            {
                node.IsSearched = false;
            }
        }
        public string GenerateCode()
        {
            string result = string.Empty;
            ResetVaribleAssignment();
            foreach(var node in _rootNodes)
            {
                result += node.GenerateCode();
            }
            return result;
        } 
        public void AddRootNode(ExpTreeNode node)
        {
            _rootNodes.Add(node);
            node.Tree = this;
        }
        public void AddGenNode(ExpTreeNode node)
        {
            _genNodes.Add(node);
            node.Tree = this;
        }
        public void ResetVaribleAssignment()
        {
            VaribleNameIndex = 0;
        }
        public string AssignVaribleName()
        {
            var result = string.Format("{0}_{1}", TreeName, VaribleNameIndex);
            VaribleNameIndex++;
            return result;
        }
    }
}
