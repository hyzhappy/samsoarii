using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    public class CortexMBehavior : BaseCompileBehavior
    {
        public CortexMBehavior(BaseCompiler compiler) : base(compiler)
        {

        }

        public override bool Execute(string ladderFile, string funcBlockFile, string binaryFile)
        {
            // step 1 : compile ladder file, result a ladderObjectFile
            // and compile function block file, result a funcBlockObjectFile
            Process cmd1 = new Process();
            string ladderObjectFile = SamSoarII.Utility.FileHelper.ChangeExtension(ladderFile, ".o");
            cmd1.StartInfo.CreateNoWindow = true;
            cmd1.StartInfo.UseShellExecute = false;
            cmd1.StartInfo.FileName = string.Format(@"{0}\{1}", CompilerUtility.CrossARM_Dir, CompilerUtility.CrossARM_CC);
            cmd1.StartInfo.Arguments = string.Format(@"{0} {1} -o {2}", _compiler.CC_FLAGS, ladderFile, ladderObjectFile);
            cmd1.StartInfo.RedirectStandardError = true;
            cmd1.Start();

            // it's necessary catch stdout and stderr when compiling funcBlockFile 

            Process cmd2 = new Process();
            string funcBlockObjectFile = SamSoarII.Utility.FileHelper.ChangeExtension(funcBlockFile, ".o");
            cmd2.StartInfo.CreateNoWindow = true;
            cmd2.StartInfo.UseShellExecute = false;
            cmd2.StartInfo.FileName = string.Format(@"{0}\{1}", CompilerUtility.CrossARM_Dir, CompilerUtility.CrossARM_CXX);
            cmd2.StartInfo.Arguments = string.Format(@"{0} {1} -o {2}",_compiler.CXX_FLAGS, funcBlockFile, funcBlockObjectFile);
            cmd2.StartInfo.RedirectStandardError = true;
            cmd2.Start();
            // start compile two file together
            cmd1.WaitForExit();
            cmd2.WaitForExit();
            if (cmd1.ExitCode != 0)
            {
                Console.WriteLine("Error on compiling Ladder Diagram !");
                Console.WriteLine(cmd1.StandardError.ReadToEnd());
                return false;
            }
            if (cmd2.ExitCode != 0)
            {
                Console.WriteLine("Error on compiling Function Block: ");
                Console.WriteLine(cmd2.StandardError.ReadToEnd());
                return false;
            }
            // step 2 : link, result a elf file
            Process cmd3 = new Process();
            string elfFile = SamSoarII.Utility.FileHelper.ChangeExtension(ladderFile, ".elf");
            cmd3.StartInfo.CreateNoWindow = true;
            cmd3.StartInfo.UseShellExecute = false;
            cmd3.StartInfo.FileName = string.Format(@"{0}\{1}", CompilerUtility.CrossARM_Dir, CompilerUtility.CrossARM_CXX);
            cmd3.StartInfo.Arguments = string.Format(@"{0} {1} {2} {3} -o {4}", _compiler.LD_FLAGS, ladderObjectFile, funcBlockObjectFile, _compiler.STATIC_LIB_FLAGS, elfFile);
            cmd3.StartInfo.RedirectStandardError = true;
            cmd3.Start();
            cmd3.WaitForExit();
            if (cmd3.ExitCode != 0)
            {
                Console.WriteLine("Error on linking: ");
                Console.WriteLine(cmd3.StandardError.ReadToEnd());
                return false;
            }
            // step 3 : objcopy, result a binary file
            Process cmd4 = new Process();     
            cmd4.StartInfo.CreateNoWindow = true;
            cmd4.StartInfo.UseShellExecute = false;
            cmd4.StartInfo.FileName = string.Format(@"{0}\{1}", CompilerUtility.CrossARM_Dir, CompilerUtility.CrossARM_Objcopy);
            cmd4.StartInfo.Arguments = string.Format(@"{0} {1} {2}", _compiler.OBJCOPY_FLAGS, elfFile, binaryFile);
            cmd4.StartInfo.RedirectStandardError = true;
            cmd4.Start();
            cmd4.WaitForExit();
            if (cmd4.ExitCode != 0)
            {
                Console.WriteLine("Error on creating binary file: ");
                Console.WriteLine(cmd4.StandardError.ReadToEnd());
                return false;
            }
            return true;
        }
    }
}
