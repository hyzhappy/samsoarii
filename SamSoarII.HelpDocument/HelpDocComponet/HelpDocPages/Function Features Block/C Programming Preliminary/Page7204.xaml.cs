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

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Function_Features_Block.C_Programming_Preliminary
{
    /// <summary>
    /// Page7204.xaml 的交互逻辑
    /// </summary>
    public partial class Page7204 : Page,IPageItem
    {
        public Page7204()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 7204;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("循环结构程序");
            }
        }
    }
}
