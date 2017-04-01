using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    class FGs64MTCompiler : BaseCompiler
    {
        public FGs64MTCompiler()
        {
            _compileBehavior = new CortexMBehavior(this);
        }
    }
}
