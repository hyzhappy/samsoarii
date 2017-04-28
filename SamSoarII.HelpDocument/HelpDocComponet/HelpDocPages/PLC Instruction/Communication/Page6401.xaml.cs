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

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Communication
{
    /// <summary>
    /// Page6401.xaml 的交互逻辑
    /// </summary>
    public partial class Page6401 : Page,IPageItem
    {
        public Page6401()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 6401;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("MBUS()");
            }
        }
    }
}
