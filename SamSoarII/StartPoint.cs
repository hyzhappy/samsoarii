using SamSoarII.AppMain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.AppMain
{
    public static class StartPoint
    {
        [STAThread]
        static void Main(string[] args)
        {
            string filePath = "";
            if ((args != null) && (args.Length > 0))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    // 对于路径中间带空格的会自动分割成多个参数传入  
                    filePath += " " + args[i];
                }
                filePath.Trim();
            }
            //FilePath为Main程序的数据成员属性
            App.AutoOpenFileFullPath = filePath;
            App app = new App();
            app.Run();
        }
    }
}
