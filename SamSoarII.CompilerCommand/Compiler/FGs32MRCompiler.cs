using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    class FGs32MRCompiler : BaseCompiler
    {
        public FGs32MRCompiler()
        {
            _compileBehavior = new CortexMBehavior(this);
        }
    }
}
