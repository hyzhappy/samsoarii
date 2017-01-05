using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;

namespace SamSoarII.PLCCompiler
{
    public class ExpGraphAndNode : ExpGraphNode
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.And;
            }
        }
        public override int MaxNextNodeCount
        {
            get
            {
                return 2; 
            }
        }
        public ExpGraphAndNode()
        {
        }
        public override bool GetPublicParentCondition()
        {
            return false;
        }

        public override ExpTreeNode CreateTreeNode()
        {
            return new ExpTreeANDNode();
        }
    }
}
