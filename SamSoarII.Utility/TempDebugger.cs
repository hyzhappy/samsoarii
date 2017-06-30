using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility
{
    public class TempDebugger
    {
        public static string DebugPath = FileHelper.AppRootPath + "Debug.txt";
        private static StreamWriter writer;
        static TempDebugger()
        {
            writer = new StreamWriter(DebugPath);
        }
        public static void WriteLine(string message)
        {
            writer.WriteLine(message);
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
