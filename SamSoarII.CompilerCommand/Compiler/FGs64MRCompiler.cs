using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    class FGs64MRCompiler : BaseCompiler
    {
        public FGs64MRCompiler()
        {
            _compileBehavior = new CortexMBehavior(this);
        }
    }
}
