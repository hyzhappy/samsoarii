using Microsoft.Win32;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// FileConvertDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FileConvertDialog : Window
    {
        public FileConvertDialog()
        {
            InitializeComponent();
            FileSelect.Click += FileSelect_Click;
            ConvertButton.Click += ConvertButton_Click;
            CancelButton.Click += CancelButton_Click;
            BrowseButton.Click += BrowseButton_Click;
            KeyDown += FileConvertDialog_KeyDown;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TB_Path.Text = dialog.SelectedPath;
            }
        }

        private void FileConvertDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LB_Old.HasItems)
            {
                LocalizedMessageBox.Show(Properties.Resources.Message_File_Requried,LocalizedMessageIcon.Warning);
            }
            else if(LB_Old.Items.Count > 10)
            {
                LocalizedMessageBox.Show(string.Format("{0} 10", Properties.Resources.Max_Number_File), LocalizedMessageIcon.Warning);
            }
            else if (TB_Path.Text == string.Empty)
            {
                LocalizedMessageBox.Show(Properties.Resources.Required_File_Path, LocalizedMessageIcon.Warning);
            }
            else if(!Directory.Exists(TB_Path.Text))
            {
                LocalizedMessageBox.Show(Properties.Resources.Message_Path, LocalizedMessageIcon.Warning);
            }
            else
            {

            }
        }

        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = string.Format("{0}|*.{1}", Properties.Resources.Project_File, FileHelper.OldFileExtension);
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var item in openFileDialog.FileNames)
                {
                    LB_Old.Items.Add(item);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
