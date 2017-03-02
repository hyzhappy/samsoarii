using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.PLCCompiler
{
    public class ExpGraphInputJNode : ExpGraphNode
    {
        private BaseModel _model;
        public override ElementType Type
        {
            get
            {
                return ElementType.Input;
            }
        }
        public override int MaxNextNodeCount
        {
            get
            {
                return 0;
            }
        }
        public ExpGraphInputJNode(BaseModel model)
        {
            _model = model;
        }
        public override ExpTreeNode CreateTreeNode()
        {
            return new ExpTreeInputJNode(_model);
        }
    }
}
