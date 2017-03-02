using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.PLCCompiler
{
    public class ExpTreeInputJNode : ExpTreeNode
    {
        private BaseModel _model;
        public override ElementType Type
        {
            get
            {
                return ElementType.Input; 
            }
        }
        public ExpTreeInputJNode(BaseModel model)
        {
            _model = model;
        }

        public override string _generateCode()
        {
            _model.ExportVaribleName = Tree.AssignVaribleName();
            return _model.GenerateCode();
        }
        public override string GetExportVaribleName()
        {
            return _model.ExportVaribleName;
        }
    }
}
