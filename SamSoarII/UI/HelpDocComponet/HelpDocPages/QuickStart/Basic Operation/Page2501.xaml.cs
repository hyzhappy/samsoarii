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
    /// Page2501.xaml 的交互逻辑
    /// </summary>
    public partial class Page2501 : Page,IPageItem
    {
        public Page2501()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 2501;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("鼠标用法和快捷键");
            }
        }
    }
}
