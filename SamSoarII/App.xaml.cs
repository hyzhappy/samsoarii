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
using Xceed.Wpf.AvalonDock.Global;

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
        public static SplashScreen splashScreen;

        public App()
        {
            splashScreen = new SplashScreen(@"Resources\Image\SplashScreen.png");
            splashScreen.Show(false, true);
            InitializeComponent();
            SettingManager.Load();
            AllResourceManager.Initialize();
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
            ((MainWindow)Current.MainWindow).IFParent.SaveAsProject(FileHelper.AppRootPath + string.Format(@"\Temp\__backupfile({0}{1}{2}_{3}.{4}.{5}).ssr",
                DateTime.Now.Year, DateTime.Now.Month < 10 ? string.Format("0{0}", DateTime.Now.Month) : DateTime.Now.Month.ToString(),
                DateTime.Now.Day < 10 ? string.Format("0{0}", DateTime.Now.Day) : DateTime.Now.Day.ToString(),
                DateTime.Now.Hour < 10 ? string.Format("0{0}", DateTime.Now.Hour) : DateTime.Now.Hour.ToString(),
                DateTime.Now.Minute < 10 ? string.Format("0{0}", DateTime.Now.Minute) : DateTime.Now.Minute.ToString(),
                DateTime.Now.Second < 10 ? string.Format("0{0}", DateTime.Now.Second) : DateTime.Now.Second.ToString()), 
                true);
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
            MainWindow mwnd = new MainWindow();
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
            LayoutSetting.Save();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static bool CultureIsZH_CH()
        {
            return Thread.CurrentThread.CurrentUICulture.Name.Equals("zh-Hans");
        }
    }
}
