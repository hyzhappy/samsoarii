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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// DemoLadderViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class DemoLadderViewModel : UserControl
    {
        public DemoLadderViewModel()
        {
            InitializeComponent();
            mainStackPanel.LayoutTransform = new ScaleTransform(0.45,0.45);
            CommentAreaExpander.IsEnabled = false;
            CommentAreaExpander2.IsEnabled = false;
            ladderExpander.IsEnabled = false;
            ladderExpander2.IsEnabled = false;
            ladderExpander.line.Visibility = Visibility.Hidden;
            ladderExpander.line1.Visibility = Visibility.Hidden;
        }
    }
}
