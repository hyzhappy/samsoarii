using Microsoft.Win32;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// FileConvertDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FileConvertDialog : Window
    {
        private InteractionFacade ifParent;
        public FileConvertDialog(InteractionFacade ifParent)
        {
            InitializeComponent();
            this.ifParent = ifParent;
            FileSelect.Click += FileSelect_Click;
            ConvertButton.Click += ConvertButton_Click;
            CancelButton.Click += CancelButton_Click;
            BrowseButton.Click += BrowseButton_Click;
            KeyDown += FileConvertDialog_KeyDown;
        }
        public FileConvertDialog(InteractionFacade ifParent,string filename):this(ifParent)
        {
            LB_Old.Items.Add(filename);
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
                LB_New.Items.Clear();
                ProgressBarHandle handle = new ProgressBarHandle(Properties.Resources.File_Convert, 0, 100,_Handle, new BackgroundWorker(),Dispatcher);
                handle.StartWork();
            }
        }
        private void _Handle(ProgressBarHandle handle)
        {
            int cnt = 0, covered = 0;
            long currentFileLen;
            string currentPath = FileHelper.AppRootPath;
            string outPath = TB_Path.Text;
            outPath = StringHelper.RemoveSystemSeparator(outPath);
            foreach (string item in LB_Old.Items)
            {
                currentFileLen = FileHelper.GetFileLength(item);
                string filename = FileHelper.GetFileName(item);
                string outFilename = string.Format("{0}{1}{2}.{3}", outPath, System.IO.Path.DirectorySeparatorChar, filename, FileHelper.NewFileExtension);
                if (File.Exists(outFilename))
                {
                    var ret = LocalizedMessageBox.Show(string.Format("{0}\n{1}", Properties.Resources.File_Override, outFilename), LocalizedMessageButton.YesNo, LocalizedMessageIcon.Warning);
                    if (ret != LocalizedMessageResult.Yes)
                    {
                        currentFileLen = 0;//不覆盖，因此将文件长度设置为0
                        handle.ReportProgress((int)(((double)(cnt + 1) / LB_Old.Items.Count) * 100), currentFileLen / (160.0 * 1024));
                        cnt++;
                        continue;
                    }
                    else covered++;
                }
                Process cmd = new Process();
                cmd.StartInfo.WorkingDirectory = string.Format(@"{0}\Converter\.", currentPath);
                cmd.StartInfo.FileName = string.Format(@"{0}\Converter\Converter.exe", currentPath);
                cmd.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\"", 1, outPath, item);
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                try
                {
                    handle.ReportProgress((int)(((double)(cnt + 1) / LB_Old.Items.Count) * 100), currentFileLen / (160.0 * 1024));
                    cmd.Start();
                    cmd.WaitForExit();
                    Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        LB_New.Items.Add(outFilename);
                        Thread.Sleep(10);
                    });
                    cnt++;
                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                    foreach (var process in Process.GetProcessesByName("Converter.exe"))
                    {
                        process.Kill();
                    }
                }
            }
            handle.Abort();
            if (App.CultureIsZH_CH())
                LocalizedMessageBox.Show(string.Format("成功{0}{1}，失败{2}！", LB_New.Items.Count, covered == 0 ? string.Empty : string.Format("(覆盖{0})", covered), LB_Old.Items.Count - cnt), LocalizedMessageIcon.Information);
            else
                LocalizedMessageBox.Show(string.Format("{0} successful{1}, {2} failed!", LB_New.Items.Count, covered == 0 ? string.Empty : string.Format("({0} is covered)", covered), LB_Old.Items.Count - cnt), LocalizedMessageIcon.Information);
        }

        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = string.Format("{0}|*.{1}", Properties.Resources.Project_File, FileHelper.OldFileExtension);
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filename in openFileDialog.FileNames)
                {
                    if(!LB_Old.Items.Contains(filename))
                        LB_Old.Items.Add(filename);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileOpen(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            new Thread(() => 
            {
                ifParent.WNDMain.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    ifParent.LoadProject((string)item.Content);
                });
            }).Start();
            Close();
        }
    }
}