using SamSoarII.PLCDevice;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// NewProjectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewProjectDialog : Window, IDisposable
    {
        public event RoutedEventHandler EnsureButtonClick;
        public PLCDeviceType Type
        {
            get
            {
                switch (PLCTypeComboBox.SelectedIndex)
                {
                    case 0:
                        return PLCDeviceType.FGs_16MR_A;
                    case 1:
                        return PLCDeviceType.FGs_16MR_D;
                    case 2:
                        return PLCDeviceType.FGs_16MT_A;
                    case 3:
                        return PLCDeviceType.FGs_16MT_D;
                    case 4:
                        return PLCDeviceType.FGs_32MR_A;
                    case 5:
                        return PLCDeviceType.FGs_32MR_D;
                    case 6:
                        return PLCDeviceType.FGs_32MT_A;
                    case 7:
                        return PLCDeviceType.FGs_32MT_D;
                    case 8:
                        return PLCDeviceType.FGs_64MR_A;
                    case 9:
                        return PLCDeviceType.FGs_64MR_D;
                    case 10:
                        return PLCDeviceType.FGs_64MT_A;
                    case 11:
                        return PLCDeviceType.FGs_64MT_D;
                    case 12:
                        return PLCDeviceType.FGs_32MR_YTJ;
                    case 13:
                        return PLCDeviceType.FGs_32MT_YTJ;
                    case 14:
                        return PLCDeviceType.FGs_20MR_BYK;
                    default:
                        return PLCDeviceType.FGs_16MR_A;
                }
            }
        }
        public bool IsSettingChecked
        {
            get
            {
                return (bool)setting.IsChecked;
            }
        }
        public string NameContent
        {
            get { return NameTextBox.Text; }
        }
        public string PathContent
        {
            get { return PathTextBox.Text; }
        }

        public NewProjectDialog()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            KeyDown += NewProjectDialog_KeyDown;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
            BrowseButton.Click += BrowseButton_Click;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSettingChecked && File.Exists(PathContent + @"\" + NameContent + "." + FileHelper.ExtensionName))
            {
                string title = Properties.Resources.Message_Is_Override;
                string text = String.Format("{2:s} {1},{0}?",Properties.Resources.Message_Is_Override, Properties.Resources.Message_Already_Exist, NameContent);
                LocalizedMessageResult mbret = LocalizedMessageBox.Show(text, title, LocalizedMessageButton.YesNoCancel, LocalizedMessageIcon.Warning);
                switch (mbret)
                {
                    case LocalizedMessageResult.Yes:
                        break;
                    case LocalizedMessageResult.No:
                    case LocalizedMessageResult.Cancel:
                    default:
                        return;
                }
            }
            if(EnsureButtonClick != null)
            {
                EnsureButtonClick.Invoke(this, new RoutedEventArgs());
            }
        }

        private void NewProjectDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if(e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
        public void Dispose()
        {
            this.Close();
        }
    }
}
