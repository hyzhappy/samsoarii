using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    class FGs16MTCompiler : BaseCompiler
    {
        public FGs16MTCompiler()
        {
            _compileBehavior = new CortexMBehavior(this);
        }
    }
}
