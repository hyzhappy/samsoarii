using SamSoarII.Global;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SamSoarII
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 用于双击文件启动程序时保存的路径
        /// </summary>
        public static string AutoOpenFileFullPath;

        public App()
        {
            InitializeComponent();
            AllResourceManager.Initialize();
            SettingManager.Load();
            Startup += App_Startup;
            Exit += App_Exit;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        /// <summary>
        /// 程序发生未捕捉的异常而崩溃时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //保存用户正在编辑的文件
            ((MainWindow)Current.MainWindow).IFParent.SaveAsProject(FileHelper.AppRootPath + @"\Temp\__last.ssr", true);
            AppFinalize();
            //写入Debug信息
            TempDebugger.WriteLine(DateTime.Now);
            TempDebugger.WriteLine(e.Exception.Message);
            TempDebugger.WriteLine();
            TempDebugger.WriteLine(e.Exception.StackTrace);
            TempDebugger.WriteLine();
            TempDebugger.Dispose();

            LocalizedMessageBox.Show(SamSoarII.Properties.Resources.Unknowed_Exception, SamSoarII.Properties.Resources.Error, LocalizedMessageIcon.Error);
            Current.Shutdown();
            e.Handled = true;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mwnd = new SamSoarII.MainWindow();
            MainWindow = mwnd;
            mwnd.Show();
        }
        
        private void App_Exit(object sender, ExitEventArgs e)
        {
            AppFinalize();
        }
        
        public void AppFinalize()
        {
            SettingManager.Save();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static bool CultureIsZH_CH()
        {
            return Thread.CurrentThread.CurrentUICulture.Name.Equals("zh-Hans");
        }
    }
}
