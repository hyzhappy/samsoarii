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

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages.Function_Features_Block.C_Programming_Preliminary
{
    /// <summary>
    /// Page7200.xaml 的交互逻辑
    /// </summary>
    public partial class Page7200 : Page,IPageItem
    {
        public Page7200()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 7200;
            }
        }
        public string TabHeader
        {
            get
            {
                return string.Format("C语言程序设计初步");
            }
        }
    }
}
