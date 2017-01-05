using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
namespace SamSoarII.PLCCompiler
{
    public abstract class ExpTreeNode
    {
        public ExpTree Tree { get; set; }
        public bool IsSearched { get; set; }
        public ExpTreeNode LeftChild { get; set; }
        public ExpTreeNode RightChild { get; set; }
        public abstract string _generateCode();
        public abstract string GetExportVaribleName();
        public virtual ElementType Type { get; }
        public string GenerateCode()
        {
            if (IsSearched)
            {
                return string.Empty;
            }
            else
            {
                IsSearched = true;
                return _generateCode();
            }
        }
    }
}
