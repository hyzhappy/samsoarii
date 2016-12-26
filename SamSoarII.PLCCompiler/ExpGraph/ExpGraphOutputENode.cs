using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;

namespace SamSoarII.PLCCompiler
{
    public class ExpGraphOutputENode : ExpGraphNode
    {
        private BaseModel _model;
        public override ElementType Type
        {
            get
            {
                return ElementType.Output;
            }
        }
        public override int MaxNextNodeCount
        {
            get
            {
                return 1;
            }
        }
        public ExpGraphOutputENode(BaseModel model)
        {
            _model = model;
        }

        public override bool GetPublicParentCondition()
        {
            return false;
        }

        public override ExpTreeNode CreateTreeNode()
        {
            return new ExpTreeOutputENode(_model);
        }
    }
}
