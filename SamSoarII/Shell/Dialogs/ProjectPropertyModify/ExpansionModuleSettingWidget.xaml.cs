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
using SamSoarII.Core.Models;
using System.Threading;
using System.Windows.Threading;

namespace SamSoarII.Shell.Dialogs
{
    
    /// <summary>
    /// ExpansionModuleSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class ExpansionModuleSettingWidget : UserControl, IDisposable
    {
        public ExpansionModuleSettingWidget(ExpansionModuleParams _core)
        {
            InitializeComponent();
            core = _core;
            DataContext = core;
            for (int i = 0; i < 8; i++)
            {
                _widget.Add(new ExpansionUnitModule(core.ExpansionUnitParams[i]));
            }
            for (int i = 0; i < Modules.Items.Count; i++)
            {
                ((ListBoxItem)Modules.Items[i]).DataContext = core.ExpansionUnitParams[i];
            }
            foreach (var item in _widget)
            {
                ((ExpansionUnitModule)item).ModuleTypeChangedHandle += ExpansionModuleSettingWidget_ModuleTypeChangedHandle;
            }
            ContentGrid.Children.Add(_widget[0]);
        }

        private void ExpansionModuleSettingWidget_ModuleTypeChangedHandle(object sender, RoutedEventArgs e)
        {
            UpdateTB_Message((ExpansionUnitModule)sender);
        }

        public void Dispose()
        {
            core = null;
            DataContext = null;
            foreach (ExpansionUnitModule item in _widget)
            {
                item.ModuleTypeChangedHandle -= ExpansionModuleSettingWidget_ModuleTypeChangedHandle;
                item.Dispose();
            }
            for (int i = 0; i < Modules.Items.Count; i++)
            {
                ((ListBoxItem)Modules.Items[i]).DataContext = null;
            }
            _widget.Clear();
            _widget = null;
        }

        #region Number

        private ExpansionModuleParams core;
        public ExpansionModuleParams Core { get { return this.core; } }

        private List<UserControl> _widget = new List<UserControl>();
        #endregion

        private void ShowModule(object sender, SelectionChangedEventArgs e)
        {
            ContentGrid?.Children.Clear();
            ContentGrid?.Children.Add(_widget[Modules.SelectedIndex]);
            if (_widget.Count == 0) return;
            UpdateTB_Message((ExpansionUnitModule)_widget[Modules.SelectedIndex]);
        }

        public void ShowWidget(int index)
        {
            Modules.SelectedIndex = index;
        }
        private void UpdateTB_Message(ExpansionUnitModule unitModule)
        {
            var message = string.Empty;
            if (TB_Message != null)
            {
                switch (unitModule.Core.ModuleType)
                {
                    case ModuleType.FGs_E4AI:
                        message = string.Format("AI{0} - AI{1}",4 + 4 * unitModule.Core.ID,7 + 4 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E8R:
                    case ModuleType.FGs_E8T:
                        message = string.Format("Y{0} - Y{1}", 900 + 100 * unitModule.Core.ID, 907 + 100 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E8X:
                        message = string.Format("X{0} - X{1}", 900 + 100 * unitModule.Core.ID, 907 + 100 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E8X8T:
                    case ModuleType.FGs_E8X8R:
                        message = string.Format("X{0} - X{1},Y{0} - Y{1}", 900 + 100 * unitModule.Core.ID, 907 + 100 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E16R:
                    case ModuleType.FGs_E16T:
                        message = string.Format("Y{0} - Y{1}", 900 + 100 * unitModule.Core.ID, 915 + 100 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E2AO:
                        message = string.Format("AO{0} - AO{1}", 4 * unitModule.Core.ID, 1 + 4 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E4AI2AO:
                        message = string.Format("AI{0} - AI{1},AO{2} - AO{3}", 4 + 4 * unitModule.Core.ID, 7 + 4 * unitModule.Core.ID, 4 + 4 * unitModule.Core.ID, 5 + 4 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E4TC:
                        message = string.Format("AI{0} - AI{1}", 4 + 4 * unitModule.Core.ID, 7 + 4 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E16X:
                        message = string.Format("X{0} - X{1}", 900 + 100 * unitModule.Core.ID, 915 + 100 * unitModule.Core.ID);
                        break;
                    case ModuleType.FGs_E16X16T:
                    case ModuleType.FGs_E16X16R:
                        message = string.Format("X{0} - X{1},Y{0} - Y{1}", 900 + 100 * unitModule.Core.ID, 915 + 100 * unitModule.Core.ID);
                        break;
                    default:
                        break;
                }
            }
            TB_Message.Text = message;
        }
    }
}
