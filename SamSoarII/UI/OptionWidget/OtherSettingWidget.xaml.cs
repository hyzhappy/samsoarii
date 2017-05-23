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
            checkbox.IsChecked = GlobalSetting.IsSavedByTime;
            timespantextbox.Text = GlobalSetting.SaveTimeSpan.ToString();
            OptionDialog.EnsureButtonClick += OptionDialog_EnsureButtonClick;
        }
        private void OptionDialog_EnsureButtonClick(object sender, RoutedEventArgs e)
        {
            GlobalSetting.IsSavedByTime = (bool)checkbox.IsChecked;
            GlobalSetting.SaveTimeSpan = int.Parse(timespantextbox.Text);
            if (_interactionFacade.ProjectLoaded && GlobalSetting.IsSavedByTime)
            {
                _interactionFacade.ProjectModel.autoSavedManager.Start();
            }
            else
            {
                if (_interactionFacade.ProjectLoaded && _interactionFacade.ProjectModel.autoSavedManager.IsRunning)
                {
                    _interactionFacade.ProjectModel.autoSavedManager.Abort();
                }
            }
        }
    }
}
