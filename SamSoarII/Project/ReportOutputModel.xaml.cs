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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// ReportOutputModel.xaml 的交互逻辑
    /// </summary>
    public partial class ReportOutputModel : UserControl
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public ReportOutputModel()
        {
            InitializeComponent();
            Report_All.TextChanged += OnReportTextChanged;
            ReportScrollViewer.Content = Report_All;
            //Move(Report_All);
        }
        
        #region Numbers

        public TextBox Report_All { get; } = new TextBox();
        public TextBox Report_Simulate { get; } = new TextBox();
        public TextBox Report_Generate { get; } = new TextBox();
        public TextBox Report_Complie { get; } = new TextBox();
        public TextBox Report_Debug { get; } = new TextBox();

        #endregion
        
        #region Features
        
        public void Clear(TextBox report)
        {
            report.Text = String.Empty;
        }
        
        public void Move(TextBox report)
        {
            if (report == Report_All)
            {
                CB_Source.SelectedIndex = 0;
            }
            if (report == Report_Simulate)
            {
                CB_Source.SelectedIndex = 1;
            }
            if (report == Report_Generate)
            {
                CB_Source.SelectedIndex = 2;
            }
            if (report == Report_Complie)
            {
                CB_Source.SelectedIndex = 3;
            }
            if (report == Report_Debug)
            {
                CB_Source.SelectedIndex = 4;
            }
        }

        public void Write(TextBox report, string text)
        {
            report.Text = text;
            if (report != Report_All)
            {
                Report_All.Text += text;
            }
            Move(report);
        }

        public void Append(TextBox report, string text)
        {
            report.Text += text;
            if (report != Report_All)
            {
                Report_All.Text += text;
            }
            Move(report);
        }

        public void AppendLine(TextBox report, string text)
        {
            Append(report, String.Format("{0:s}\r\n", text));
        }

        #endregion

        #region Event Handler

        private const int MAXLINE = 100;
        private bool _selftextchanged = false;
        private void OnReportTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selftextchanged)
            {
                return;
            }
            if (sender is TextBox)
            {
                TextBox report = (TextBox)(sender);
                string[] lines = report.Text.Split('\n');
                if (lines.Length > MAXLINE)
                {
                    _selftextchanged = true;
                    report.Text = String.Empty;
                    for (int i = Math.Max(0, lines.Length - MAXLINE); i < lines.Length; i++)
                    {
                        report.Text += lines[i] + "\n";
                    }
                    _selftextchanged = false;
                }
            }
        }

        private void CB_Source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = String.Empty;
            if (e.AddedItems[0] is TextBlock)
            {
                text = ((TextBlock)(e.AddedItems[0])).Text;
            }
            if (ReportScrollViewer == null)
            {
                return;
            }
            switch (text)
            {
                case "所有来源":
                    ReportScrollViewer.Content = Report_All;
                    break;
                case "仿真":
                    ReportScrollViewer.Content = Report_Simulate;
                    break;
                case "生成":
                    ReportScrollViewer.Content = Report_Generate;
                    break;
                case "函数块编译":
                    ReportScrollViewer.Content = Report_Complie;
                    break;
                case "#DEBUG":
                    ReportScrollViewer.Content = Report_Debug;
                    break;
            }
        }
        
        private void B_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_Source.Text)
            {
                case "所有来源":
                    Report_All.Text = String.Empty;
                    break;
                case "仿真":
                    Report_Simulate.Text = String.Empty;
                    break;
                case "生成":
                    Report_Generate.Text = String.Empty;
                    break;
                case "函数块编译":
                    Report_Complie.Text = String.Empty;
                    break;
                case "#DEBUG":
                    Report_Debug.Text = String.Empty;
                    break;
            }
        }

        #endregion

    }
}
