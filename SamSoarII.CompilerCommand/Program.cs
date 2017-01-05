using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace SamSoarII.CompilerCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            Process cmd = new Process();
            cmd.StartInfo.WorkingDirectory = "";
            cmd.StartInfo.FileName = "gcc";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.Arguments = " -o2 -c main.c";
            cmd.Start();
            cmd.WaitForExit();

            
        }
    }
}
