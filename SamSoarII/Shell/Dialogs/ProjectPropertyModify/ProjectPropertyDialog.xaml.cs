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
using System.Windows.Shapes;
using SamSoarII.Core.Models;
using SamSoarII.HelpDocument;
using System.Windows.Controls.Primitives;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// SettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectPropertyDialog : Window, IDisposable
    {
        public event RoutedEventHandler EnsureButtonClick = delegate { };
        
        public ProjectPropertyDialog(ProjectModel _core)
        {
            InitializeComponent();
            core = _core;
            oldParams = core.PARAProj;
            newParams = oldParams.Clone(null);
            wgDevice = new DeviceSelectionWidget(_core);
            wgCommunication = new CommunicationSettingWidget(newParams.PARACom232, newParams.PARACom485);
            wgPassword = new PasswordSettingWidget(newParams.PARAPassword);
            wgAnalog = new AnalogQuantitySettingWidget(newParams.PARAAnalog);
            wgFilter = new FilterSettingWidget(newParams.PARAFilter);
            wgHolding = new HoldingSectionSettingWidget(newParams.PARAHolding);
            wgExpansion = new ExpansionModuleSettingWidget(newParams.PARAExpansion);
            _widget.Add(wgDevice);
            _widget.Add(wgCommunication);
            _widget.Add(wgPassword);
            _widget.Add(wgAnalog);
            _widget.Add(wgFilter);
            _widget.Add(wgHolding);
            _widget.Add(wgExpansion);
            ShowWidget(0);
        }

        public void Dispose()
        {
            wgDevice.Dispose();
            wgCommunication.Dispose();
            wgPassword.Dispose();
            wgAnalog.Dispose();
            wgFilter.Dispose();
            wgHolding.Dispose();
            wgExpansion.Dispose();
            wgDevice = null;
            wgCommunication = null;
            wgPassword = null;
            wgAnalog = null;
            wgFilter = null;
            wgHolding = null;
            wgExpansion = null;
            _widget.Clear();
            _widget = null;
            core = null;
        }

        #region Number

        private ProjectModel core;
        public ProjectModel Core { get { return this.core; } }

        private ProjectPropertyParams oldParams;
        private ProjectPropertyParams newParams;

        private List<UserControl> _widget = new List<UserControl>();
        private DeviceSelectionWidget wgDevice;
        private CommunicationSettingWidget wgCommunication;
        private PasswordSettingWidget wgPassword;
        private AnalogQuantitySettingWidget wgAnalog;
        private FilterSettingWidget wgFilter;
        private HoldingSectionSettingWidget wgHolding;
        private ExpansionModuleSettingWidget wgExpansion;

        #endregion
        
        public void Save()
        {
            wgDevice.Save();
            wgCommunication.Save();
            wgPassword.Save();
            wgHolding.Save();
            oldParams.Load(newParams);
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
            core.Parent.ShowHelpDocument();
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
    public class ProjectPropertyException : Exception
    {
        private string _message;
        public ProjectPropertyException(string message)
        {
            _message = message;
        }
        public override string Message
        {
            get { return _message; }
        }
    }
}
