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

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages.PLC_Instruction.Move
{
    /// <summary>
    /// Page5505.xaml 的交互逻辑
    /// </summary>
    public partial class Page5505 : Page,IPageItem
    {
        public Page5505()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 5505;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("MVDBLK(成块移动双字)");
            }
        }
    }
}
