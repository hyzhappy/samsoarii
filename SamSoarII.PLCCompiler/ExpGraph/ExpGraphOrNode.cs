using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;

namespace SamSoarII.PLCCompiler
{
    public class ExpGraphOrNode : ExpGraphNode
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.Or;
            }
        }
        public override int MaxNextNodeCount
        {
            get
            {
                return 2;
            }
        }
        public ExpGraphOrNode()
        {
        }
        public override ExpTreeNode CreateTreeNode()
        {
            return new ExpTreeORNode();
        }
    }
}
