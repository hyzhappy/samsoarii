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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// OtherSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class OtherSettingWidget : UserControl
    {
        private InteractionFacade _interactionFacade;
        public OtherSettingWidget(InteractionFacade _interactionFacade)
        {
            InitializeComponent();
            this._interactionFacade = _interactionFacade;
            timespantextbox.GetTextBox().Text = GlobalSetting.SaveTimeSpan.ToString();
            TB_Inst.GetTextBox().Text = GlobalSetting.InstTimeSpan.ToString();
            checkbox.IsChecked = GlobalSetting.IsSavedByTime;
            CB_Inst.IsChecked = GlobalSetting.IsInstByTime;
            CB_Coil.IsChecked = GlobalSetting.IsCheckCoil;
            OptionDialog.EnsureButtonClick += OptionDialog_EnsureButtonClick;
        }
        private void OptionDialog_EnsureButtonClick(object sender, RoutedEventArgs e)
        {
            GlobalSetting.SaveTimeSpan = int.Parse(timespantextbox.GetTextBox().Text);
            GlobalSetting.InstTimeSpan = int.Parse(TB_Inst.GetTextBox().Text);
            GlobalSetting.IsSavedByTime = (bool)checkbox.IsChecked;
            GlobalSetting.IsInstByTime = (bool)CB_Inst.IsChecked;
            GlobalSetting.IsCheckCoil = (bool)CB_Coil.IsChecked;
            if (_interactionFacade.ProjectLoaded && GlobalSetting.IsSavedByTime)
            {
                _interactionFacade.ProjectModel.autoSavedManager.Start();
            }
            else
            {
                if (_interactionFacade.ProjectLoaded && !_interactionFacade.ProjectModel.autoSavedManager.notRunning)
                {
                    _interactionFacade.ProjectModel.autoSavedManager.Abort();
                }
            }
        }
    }
}
