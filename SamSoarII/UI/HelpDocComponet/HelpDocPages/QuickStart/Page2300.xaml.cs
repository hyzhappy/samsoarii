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

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages
{
    /// <summary>
    /// Page2300.xaml 的交互逻辑
    /// </summary>
    public partial class Page2300 : Page,IPageItem
    {
        public Page2300()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 2300;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("SamSoar Developer安装");
            }
        }
    }
}
