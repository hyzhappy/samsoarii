using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    class PCBehavior : BaseCompileBehavior
    {
        public PCBehavior(BaseCompiler compiler) : base(compiler)
        {

        }

        public override bool Execute(string ladderFile, string funcBlockFile, string binaryFile)
        {
            throw new NotImplementedException();
        }
    }
}
