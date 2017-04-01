using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    public class ApplicationMain
    {
        private static Dictionary<string, BaseCompiler> _compilerPrototype = new Dictionary<string, BaseCompiler>();
        private static void Init()
        {
            _compilerPrototype.Add("FGs16MR", new FGs16MRCompiler());
            _compilerPrototype.Add("FGs16MT", new FGs16MTCompiler());
            _compilerPrototype.Add("FGs32MR", new FGs32MRCompiler());
            _compilerPrototype.Add("FGs32MT", new FGs32MTCompiler());
            _compilerPrototype.Add("FGs64MR", new FGs64MRCompiler());
            _compilerPrototype.Add("FGs64MT", new FGs64MTCompiler());
            _compilerPrototype.Add("PC", new PCCompiler());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// First argument is device specification 
        /// Second argument is ladder file
        /// Third argument is function block file
        /// </param>
        public static int Main(string[] args)
        {
            if (args.Count() != 4)
            {
                Console.WriteLine("Error!Too few or too much arguments!");
                return -1;
            }
            Init();
            try
            {
                BaseCompiler compiler = _compilerPrototype[args[0]];
                if (compiler.Execute(args[1], args[2], args[3]))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine("Error!Unknown PLC Device");
                return -1;
            }      
        }
    }
}
