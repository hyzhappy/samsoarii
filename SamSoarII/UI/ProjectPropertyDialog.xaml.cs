using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI.ProjectPropertyWidget;
using SamSoarII.HelpDocument;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// SettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectPropertyDialog : Window
    {
        public event RoutedEventHandler EnsureButtonClick = delegate { };

        private ProjectModel _projectModel;

        private List<ISaveDialog> _widget = new List<ISaveDialog>();

        public ProjectPropertyDialog(ProjectModel projectModel)
        {
            InitializeComponent();
            _widget.Add(new DeviceSelectionWidget());
            _widget.Add(new CommunicationSettingWidget());
            _widget.Add(new PasswordSettingWidget());
            _widget.Add(new AnalogQuantitySettingWidget());
            _widget.Add(new FilterSettingWidget());
            _widget.Add(new HoldingSectionSettingWidget());
            _widget.Add(new ExpansionModuleSettingWidget());
            _projectModel = projectModel;
            ShowWidget(0);
        }
        public void Save()
        {
            foreach (var item in _widget)
            {
                item.Save();
            }
        }
        private void ShowWidget(int index)
        {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add((UserControl)_widget[index]);
        }

        #region Event handler
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick.Invoke(sender, e);
        }
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void OnHelpButtonClick(object sender, RoutedEventArgs e)
        {
            HelpDocWindow helpDocWindow = new HelpDocWindow();
            helpDocWindow.Show();
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                this.ShowWidget(listBox.SelectedIndex);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        #endregion
    }
}
