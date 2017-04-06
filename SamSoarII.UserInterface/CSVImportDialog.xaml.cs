using Microsoft.Win32;
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
    /// CSVImportDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CSVImportDialog : Window, IDisposable
    {
        public event RoutedEventHandler ImportButtonClick;
        public string FileName { get { return FileTextBox.Text; } }
        public string Separator { get { return SeparatorComboBox.Text; } }
        public CSVImportDialog()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            KeyDown += CSVImportDialog_KeyDown;
            ImportButton.Click += ImportButton_Click;
            CancelButton.Click += CancelButton_Click;
            BrowseButton.Click += BrowseButton_Click;
        }

        private void CSVImportDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ImportButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv文件|*.csv";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                FileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImportButtonClick != null)
            {
                ImportButtonClick.Invoke(this,new RoutedEventArgs());
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
