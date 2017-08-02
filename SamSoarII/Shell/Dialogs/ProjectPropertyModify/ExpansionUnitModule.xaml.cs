using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ExpansionUnitModule.xaml 的交互逻辑
    /// </summary>
    public partial class ExpansionUnitModule : UserControl,IDisposable
    {
        
        public ExpansionUnitModule(ExpansionUnitModuleParams _core)
        {
            InitializeComponent();
            core = _core;
            DataContext = core;
        }
        private ExpansionUnitModuleParams core;
        public ExpansionUnitModuleParams Core { get { return core; } }

        public void Dispose()
        {
            core = null;
            DataContext = null;
            ModuleTypeChangedHandle = null;
        }
        #region event
        public event RoutedEventHandler ModuleTypeChangedHandle = delegate { };
        private void ModuleTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (core.ModuleType)
            {
                case ModuleType.FGs_E4AI:
                case ModuleType.FGs_E4TC:
                    GP_Input.Visibility = Visibility.Visible;
                    GP_Output.Visibility = Visibility.Collapsed;
                    GP_Time.Visibility = Visibility.Collapsed;
                    break;
                case ModuleType.FGs_E8R:
                case ModuleType.FGs_E8T:
                case ModuleType.FGs_E16R:
                case ModuleType.FGs_E16T:
                    GP_Input.Visibility = Visibility.Collapsed;
                    GP_Output.Visibility = Visibility.Collapsed;
                    GP_Time.Visibility = Visibility.Collapsed;
                    break;
                case ModuleType.FGs_E8X:
                case ModuleType.FGs_E8X8T:
                case ModuleType.FGs_E8X8R:
                case ModuleType.FGs_E16X:
                case ModuleType.FGs_E16X16T:
                case ModuleType.FGs_E16X16R:
                    GP_Input.Visibility = Visibility.Collapsed;
                    GP_Output.Visibility = Visibility.Collapsed;
                    GP_Time.Visibility = Visibility.Visible;
                    break;
                case ModuleType.FGs_E2AO:
                    GP_Input.Visibility = Visibility.Collapsed;
                    GP_Output.Visibility = Visibility.Visible;
                    GP_Time.Visibility = Visibility.Collapsed;
                    break;
                case ModuleType.FGs_E4AI2AO:
                    GP_Input.Visibility = Visibility.Visible;
                    GP_Output.Visibility = Visibility.Visible;
                    GP_Time.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
            ModuleTypeChangedHandle(this,new RoutedEventArgs());
        }
        #endregion
    }
}
