using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget
{
    /// <summary>
    /// PasswordSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordSettingWidget : UserControl, ISaveDialog
    {
        private PasswordParams PasswordParams;
        public PasswordSettingWidget()
        {
            PasswordParams = (PasswordParams)ProjectPropertyManager.ProjectPropertyDic["PasswordParams"];
            DataContext = PasswordParams;
            Initialized += PasswordSettingWidget_Initialized;
            InitializeComponent();
        }

        private void PasswordSettingWidget_Initialized(object sender, EventArgs e)
        {
            UP_Box.Password = PasswordParams.UPassword;
            DP_Box.Password = PasswordParams.DPassword;
            MP_Box.Password = PasswordParams.MPassword;
        }

        public void Save()
        {
            if (((bool)UP_CB.IsChecked && UP_Box.Password == string.Empty) || ((bool)DP_CB.IsChecked && DP_Box.Password == string.Empty) || ((bool)MP_CB.IsChecked && MP_Box.Password == string.Empty))
                throw new ProjectPropertyException(Properties.Resources.Password_Empty);
            PasswordParams.UPIsChecked = (bool)UP_CB.IsChecked;
            PasswordParams.DPIsChecked = (bool)DP_CB.IsChecked;
            PasswordParams.MPIsChecked = (bool)MP_CB.IsChecked;
            PasswordParams.UPassword = UP_Box.Password;
            PasswordParams.DPassword = DP_Box.Password;
            PasswordParams.MPassword = MP_Box.Password;
        }
    }
}
