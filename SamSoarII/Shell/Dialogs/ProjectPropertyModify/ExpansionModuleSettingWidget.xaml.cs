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
            ContentGrid.Children.Add(_widget[0]);
        }

        public void Dispose()
        {
            core = null;
            DataContext = null;
            for (int i = 0; i < Modules.Items.Count; i++)
            {
                ((ListBoxItem)Modules.Items[i]).DataContext = null;
            }
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
            if(TB_Message != null)
                TB_Message.Text = string.Format("AI{0} - AI{1}",8 + Modules.SelectedIndex * 4, 11 + Modules.SelectedIndex * 4);
        }
    }
}
