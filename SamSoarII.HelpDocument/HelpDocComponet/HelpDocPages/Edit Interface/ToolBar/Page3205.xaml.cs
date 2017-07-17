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
    /// Page3205.xaml 的交互逻辑
    /// </summary>
    public partial class Page3205 : Page, IPageItem
    {
        public Page3205()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 3205;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("注释编辑模式");
            }
        }
    }
}
