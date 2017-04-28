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

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Auxiliary_Function
{
    /// <summary>
    /// Page6719.xaml 的交互逻辑
    /// </summary>
    public partial class Page6719 : Page, IPageItem
    {
        public Page6719()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 6719;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("FMOVD指令");
            }
        }
    }
}
