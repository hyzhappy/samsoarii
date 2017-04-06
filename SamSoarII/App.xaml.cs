using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using SamSoarII.ValueModel;
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
            ValueCommentManager.Initialize();
            VariableManager.Initialize();
            ValueAliasManager.Initialize();
            ElementList.InitializeElementCollection();
            InstructionCommentManager.MappedMessageChanged += ElementList.InstructionCommentManager_MappedMessageChanged;
            ValueCommentManager.ValueCommentChanged += ElementList.ValueCommentManager_ValueCommentChanged;
            ValueAliasManager.ValueAliasChanged += ElementList.ValueAliasManager_ValueAliasChanged;
            GlobalSetting.Load();
            SimulateHelper.LoadGlobalSetting();
            this.Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            GlobalSetting.Save();
            SimulateHelper.SaveGlobalSetting();
            if (SimulateHelper.SModel != null)
            {
                SimulateHelper.SModel.Dispose();
            }
            SamSoarII.Simulation.Core.SimulateDllModel.FreeDll();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
