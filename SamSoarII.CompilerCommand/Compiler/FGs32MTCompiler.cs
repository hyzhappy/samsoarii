using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    class FGs32MTCompiler : BaseCompiler
    {
        public FGs32MTCompiler()
        {
            _compileBehavior = new CortexMBehavior(this);
        }
    }
}
