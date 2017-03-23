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

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl
    {
        public ProjectTreeView()
        {
            InitializeComponent();
        }

        public const int ADDTVI_TYPE_SUBROUTINES = 0x01;
        public const int ADDTVI_TYPE_FUNCBLOCKS = 0x02;
        public void AddTreeViewItem(string name, int type)
        {
            TreeViewItem tvitem = new TreeViewItem();
            tvitem.Header = name;
            switch (type)
            {
                case ADDTVI_TYPE_SUBROUTINES:
                    TVI_SubRoutines.Items.Add(tvitem);
                    break;
                case ADDTVI_TYPE_FUNCBLOCKS:
                    TVI_FuncBlocks.Items.Add(tvitem);
                    break;
            }
        }
    }
}
