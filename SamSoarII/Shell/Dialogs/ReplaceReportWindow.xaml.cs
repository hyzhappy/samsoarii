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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ReplaceReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceReportWindow : Window
    {
        public ReplaceReportWindow()
        {
            InitializeComponent();
        }

        private void B_Continue_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Enter)
            {
                Close();
            }
        }
    }
}
