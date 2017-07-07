using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using SamSoarII.AppMain.UI.HelpDocComponet;
using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using SamSoarII.LadderInstViewModel;
using SamSoarII.UserInterface;
using SamSoarII.Utility;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SamSoarII.AppMain
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
            SpecialValueManager.Initialize(); 
            ValueCommentManager.Initialize();
            ValueAliasManager.Initialize();
            SettingManager.Load();
            if (GlobalSetting.IsOpenLSetting && GlobalSetting.LanagArea != string.Empty)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GlobalSetting.LanagArea);
            }
            Exit += App_Exit;
            Startup += App_Startup;
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
            ((MainWindow)Current.MainWindow).SaveProjectByException();
            AppFinalize();
            //写入Debug信息
            TempDebugger.WriteLine(DateTime.Now);
            TempDebugger.WriteLine(e.Exception.Message);
            TempDebugger.WriteLine();
            TempDebugger.WriteLine(e.Exception.StackTrace);
            TempDebugger.WriteLine();
            TempDebugger.Dispose();

            LocalizedMessageBox.Show(AppMain.Properties.Resources.Unknowed_Exception, AppMain.Properties.Resources.Error,LocalizedMessageIcon.Error);
            Current.Shutdown();
            e.Handled = true;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            MainWindow = mainwindow;
            MainWindow.Show();
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            AppFinalize();
        }

        public void AppFinalize()
        {
            SettingManager.Save();
            if (SimulateHelper.SModel != null)
                SimulateHelper.SModel.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        /// <summary>
        /// 检查应用程序的文化设置
        /// </summary>
        /// <returns></returns>
        public static bool CultureIsZH_CN()
        {
            return Thread.CurrentThread.CurrentUICulture.Name.Contains("zh");
        }
    }
}
