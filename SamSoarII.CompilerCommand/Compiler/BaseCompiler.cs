using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    public abstract class BaseCompiler
    {

        public string CC_FLAGS { get; set; }

        public string CXX_FLAGS { get; set; }

        public string LD_FLAGS { get; set; }

        public string OBJCOPY_FLAGS { get; set; }

        public string STATIC_LIB_FLAGS { get; set; }

        protected BaseCompileBehavior _compileBehavior;

        public bool Execute(string ladderFile, string funcBlockFile, string binaryFile)
        {
            return _compileBehavior.Execute(ladderFile, funcBlockFile, binaryFile);
        }
    }
}
