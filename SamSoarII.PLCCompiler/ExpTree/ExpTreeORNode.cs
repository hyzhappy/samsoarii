using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.PLCCompiler
{
    public class ExpTreeORNode : ExpTreeNode
    {
        public string ExportVaribleName { get; set; }
        public override ElementType Type
        {
            get
            {
                return ElementType.Or;
            }
        }
        public ExpTreeORNode()
        {
        } 
        public override string _generateCode()
        {
            string result = string.Empty;
            if (LeftChild != null)
            {
                result += LeftChild.GenerateCode();
            }
            if (RightChild != null)
            {
                result += RightChild.GenerateCode();
            }
            ExportVaribleName = Tree.AssignVaribleName();
            result += string.Format("plc_bool {0} = {1} | {2}; \r\n", ExportVaribleName, LeftChild.GetExportVaribleName(), RightChild.GetExportVaribleName());
            return result;
        }
        public override string GetExportVaribleName()
        {
            return ExportVaribleName;
        }
    }
}
