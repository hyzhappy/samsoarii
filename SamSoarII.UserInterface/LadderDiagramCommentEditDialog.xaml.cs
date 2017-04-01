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
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// LadderDiagramCommentEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LadderDiagramCommentEditDialog : Window
    {
        public string LadderName { get; set; }

        public string LadderComment { get; set; }

        public event RoutedEventHandler EnsureButtonClick = delegate { };
        public event RoutedEventHandler CancelButtonClick = delegate { };
        public LadderDiagramCommentEditDialog()
        {
            InitializeComponent();
            LadderNameTextBlock.DataContext = this;
            LadderCommentTextBox.DataContext = this;
        }

        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick.Invoke(sender, e);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            CancelButtonClick.Invoke(sender, e);
        }
    }
}
