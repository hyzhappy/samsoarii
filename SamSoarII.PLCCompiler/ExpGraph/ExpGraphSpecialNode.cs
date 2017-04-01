using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;

namespace SamSoarII.PLCCompiler
{
    public class ExpGraphSpecialNode : ExpGraphNode
    {
        private BaseModel _model;
        public override ElementType Type
        {
            get
            {
                return ElementType.Special;
            }
        }
        public override int MaxNextNodeCount
        {
            get
            {
                return 1;
            }
        }
        public ExpGraphSpecialNode(BaseModel model)
        {
            _model = model;
        }

        public override ExpTreeNode CreateTreeNode()
        {
            return new ExpTreeSpecialNode(_model);
        }
    }
}
