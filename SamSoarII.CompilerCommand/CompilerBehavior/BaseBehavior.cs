using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    public abstract class BaseCompileBehavior
    {
        protected BaseCompiler _compiler;

        public BaseCompileBehavior(BaseCompiler compiler)
        {
            _compiler = compiler;
        }

        /// <summary>
        /// Execute compile command and get a result file
        /// </summary>
        /// <param name="ladderFile">
        /// Full path of ladder file
        /// </param>
        /// <param name="funcBlockFile">
        /// Full path of function block file
        /// </param>
        /// <returns>
        /// Full path of result file if compile successfully, string.Empty otherwise
        /// </returns>
        public abstract bool Execute(string ladderFile, string funcBlockFile, string binaryFile);
    }
}
