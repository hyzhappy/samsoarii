using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.PLCCompiler
{
    public class ExpTreeSpecialNode : ExpTreeNode
    {
        private BaseModel _model;
        public override ElementType Type
        {
            get
            {
                return ElementType.Special;
            }
        }
        public ExpTreeSpecialNode(BaseModel model)
        {
            _model = model;
        }
        public override string _generateCode()
        {
            string result = string.Empty;
            if(LeftChild != null)
            {
                result += LeftChild.GenerateCode();
            }
            _model.ImportVaribleName = LeftChild.GetExportVaribleName();
            _model.ExportVaribleName = Tree.AssignVaribleName();
            result += _model.GenerateCode();
            return result;
        }
        public override string GetExportVaribleName()
        {
            return _model.ExportVaribleName;
        }
    }
}
