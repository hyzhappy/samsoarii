using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages;
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

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : NavigatePage, IPageItem
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public int PageIndex
        {
            get
            {
                return 1000;
            }
        }
        public string TabHeader
        {
            get
            {
                return string.Format("MainPage");
            }
        }
        public override event NavigateToPageEventHandler NavigateToPage = delegate { };

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            NavigateToPage.Invoke(new NavigateToPageEventArgs(int.Parse((string)link.Tag)));
        }
    }
}
