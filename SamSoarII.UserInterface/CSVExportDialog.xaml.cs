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

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// CSVExportDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CSVExportDialog : Window, IDisposable
    {
        public event RoutedEventHandler ExportButtonClick;
        public string FileName { get { return NameTextBox.Text; } }
        public string Path { get { return PathTextBox.Text; } }
        public string Separator { get { return SeparatorComboBox.Text; } }
        public CSVExportDialog()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            KeyDown += CSVExportDialog_KeyDown;
            ExportButton.Click += ExportButton_Click;
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExportButtonClick != null)
            {
                ExportButtonClick.Invoke(this,new RoutedEventArgs());
            }
        }

        private void CSVExportDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExportButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
