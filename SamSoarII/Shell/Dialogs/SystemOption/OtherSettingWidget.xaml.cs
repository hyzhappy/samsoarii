using SamSoarII.Global;
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
    /// OtherSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class OtherSettingWidget : UserControl
    {
        public OtherSettingWidget()
        {
            InitializeComponent();
            timespantextbox.GetTextBox().Text = GlobalSetting.SaveTimeSpan.ToString();
            TB_Inst.GetTextBox().Text = GlobalSetting.InstTimeSpan.ToString();
            checkbox.IsChecked = GlobalSetting.IsSavedByTime;
            CB_Inst.IsChecked = GlobalSetting.IsInstByTime;
            CB_Coil.IsChecked = GlobalSetting.IsCheckCoil;
        }
        
        public void Save()
        {
            GlobalSetting.SaveTimeSpan = int.Parse(timespantextbox.GetTextBox().Text);
            GlobalSetting.InstTimeSpan = int.Parse(TB_Inst.GetTextBox().Text);
            GlobalSetting.IsSavedByTime = (bool)checkbox.IsChecked;
            GlobalSetting.IsInstByTime = (bool)CB_Inst.IsChecked;
            GlobalSetting.IsCheckCoil = (bool)CB_Coil.IsChecked;
        }
    }

}
