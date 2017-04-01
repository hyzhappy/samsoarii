using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    public class PCCompiler : BaseCompiler
    {
        public PCCompiler()
        {
            _compileBehavior = new PCBehavior(this);
        }
    }
}
