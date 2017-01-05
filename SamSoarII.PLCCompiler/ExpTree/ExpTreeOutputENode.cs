using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
namespace SamSoarII.PLCCompiler
{
    public class ExpTreeOutputENode : ExpTreeNode
    {
        private BaseModel _model;
        public ExpTreeOutputENode(BaseModel model)
        {
            _model = model;
        }
        public override ElementType Type
        {
            get
            {
                return ElementType.Output;
            }
        }
        public override string _generateCode()
        {
            string result = string.Empty;
            if(LeftChild != null)
            {
                result += LeftChild.GenerateCode();
            }
            _model.ImportVaribleName = LeftChild.GetExportVaribleName();
            for (int i = 0; i < _model.TotalVaribleCount; i++)
                _model.AddVaribleName(Tree.AssignVaribleName());
            result += _model.GenerateCode();
            return result;
        }

        public override string GetExportVaribleName()
        {
            throw new NotImplementedException();
        }
    }
}
