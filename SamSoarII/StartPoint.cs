using SamSoarII.AppMain.UI;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            //if (CheckPrincipal())
            //{
                string filePath = "";
                if ((args != null) && (args.Length > 0))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        // 对于路径中间带空格的会自动分割成多个参数传入
                        filePath += " " + args[i];
                    }
                    filePath = filePath.Trim();
                }
                //FilePath为Main程序的数据成员属性
                App.AutoOpenFileFullPath = filePath;
                new App().Run();
            //}
            /*
            else
            {
                //创建启动对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                //设置运行文件
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                //设置启动参数
                startInfo.Arguments = string.Join(" ", args);
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                //如果不是管理员，则启动UAC
                Process.Start(startInfo);
                //退出
                return;
            }
            */
        }
        //检查是否以管理员权限运行
        static bool CheckPrincipal()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}