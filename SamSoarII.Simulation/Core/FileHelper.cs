using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace SamSoarII.Simulation.Core
{
    public static class FileHelper
    {
        /// <summary>
        /// Create a file in temporary path with specified postfix
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string GetTempFile(string postfix)
        {
            var newfile = Path.GetTempFileName();
            var result = Path.ChangeExtension(newfile, postfix);
            File.Move(newfile, result);
            return result;
        }
        /// <summary>
        /// Change file extension
        /// </summary>
        /// <param name="file"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string ChangeExtension(string file, string extension)
        {
            return Path.ChangeExtension(file, extension);
        }
         
    }
}
