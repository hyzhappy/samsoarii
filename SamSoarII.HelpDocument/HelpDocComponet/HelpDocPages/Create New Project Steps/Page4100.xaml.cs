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

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Create_New_Project_Steps
{
    /// <summary>
    /// Page4100.xaml 的交互逻辑
    /// </summary>
    public partial class Page4100 : Page, IPageItem
    {
        public Page4100()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 4100;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("软件安装及使用事例");
            }
        }
    }
}
