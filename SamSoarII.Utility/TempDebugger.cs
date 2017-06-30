using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility
{
    public class TempDebugger
    {
        public static string DebugPath = FileHelper.AppRootPath + @"\Debug.txt";
        private static StreamWriter writer;
        static TempDebugger()
        {
            if (!File.Exists(DebugPath))
            {
                File.Create(DebugPath);
            }
            writer = new StreamWriter(DebugPath);
        }
        public static void WriteLine(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
            //writer.Close();
        }
        public static void WriteLine(object obj)
        {
            writer.WriteLine(obj.ToString());
            writer.Flush();
            //writer.Close();
        }
        public static void Close()
        {
            writer.Close();
        }
        public static void Dispose()
        {
            writer.Dispose();
        }
    }
}
