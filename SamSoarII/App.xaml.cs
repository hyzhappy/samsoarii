using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using SamSoarII.AppMain.UI.HelpDocComponet;
using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using SamSoarII.LadderInstViewModel;
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
//using Lierda.WPFHelper;

namespace SamSoarII.AppMain
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static SplashScreen splashScreen;
        //public LierdaCracker lierdaCracker;
        public App()
        {
            SpecialValueManager.Initialize();
            ValueCommentManager.Initialize();
            ValueAliasManager.Initialize();
            SettingManager.Load();
            if (GlobalSetting.IsOpenLSetting && GlobalSetting.LanagArea != string.Empty)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GlobalSetting.LanagArea);
            }
            Exit += App_Exit;
            this.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            splashScreen = new SplashScreen(@"Resources\Image\SplashScreen.png");
            splashScreen.Show(false, true);
            //lierdaCracker = new LierdaCracker();
            //lierdaCracker.Cracker();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            SettingManager.Save();
            if (SimulateHelper.SModel != null)
                SimulateHelper.SModel.Dispose();

            //SamSoarII.Simulation.Core.SimulateDllModel.FreeDll();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public static bool CultureIsZH_CH()
        {
            return Thread.CurrentThread.CurrentUICulture.Name.Equals("zh-Hans");
        }
    }
}
