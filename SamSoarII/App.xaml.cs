using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.AppMain
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

            SamSoarII.ValueModel.ValueCommentManager.Initialize();
            SamSoarII.ValueModel.VariableManager.Initialize();
            GlobalSetting.Load();
            this.Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            GlobalSetting.Save();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
