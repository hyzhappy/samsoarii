using SamSoarII.AppMain.Project;
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
using Xceed.Wpf.AvalonDock;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class MainTabDiagramItem : DockingManager, ITabItem
    {
        public const int VIEWMODE_LADDER = 0x01;
        public const int VIEWMODE_INST = 0x02;

        private int viewmode;
        public int ViewMode
        {
            get { return this.viewmode; }
            set
            {
                this.viewmode = value;
                if ((viewmode & VIEWMODE_LADDER) != 0)
                {
                    ShowLadder();
                }
                else
                {
                    HideLadder();
                }
                if ((viewmode & VIEWMODE_INST) != 0)
                {
                    ShowInst();
                }
                else
                {
                    HideInst();
                }
            }
        }

        public string TabHeader
        {
            get; set;
        }

        protected double actualwidth;
        double ITabItem.ActualWidth
        {
            get
            {
                return this.actualwidth;
            }

            set
            {
                this.actualwidth = value;
            }
        }

        protected double actualheight;
        double ITabItem.ActualHeight
        {
            get
            {
                return this.actualheight;
            }

            set
            {
                this.actualheight = value;       
            }
        }

        public MainTabDiagramItem()
        {
            InitializeComponent();
        }

        public MainTabDiagramItem(IProgram ITI_ladder, int _viewmode)
        {
            InitializeComponent();

            LadderDiagramViewModel LDVM_ladder = (LadderDiagramViewModel)(ITI_ladder);
            LA_Ladder.Content = ITI_ladder;
            LA_Inst.Content = LDVM_ladder.IDVModel;
            
            ViewMode = _viewmode;
        }

        public void ShowLadder()
        {
            if (!LAPGroup.Children.Contains(LAP_Ladder))
            {
                LAPGroup.Children.Add(LAP_Ladder);
            }
        }

        public void HideLadder()
        {
            if (LAPGroup.Children.Contains(LAP_Ladder))
            {
                LAPGroup.Children.Remove(LAP_Ladder);
            }
        }

        public void ShowInst()
        {
            if (!LAPGroup.Children.Contains(LAP_Inst))
            {
                LAPGroup.Children.Add(LAP_Inst);
            }
        }

        public void HideInst()
        {
            if (LAPGroup.Children.Contains(LAP_Inst))
            {
                LAPGroup.Children.Remove(LAP_Inst);
            }
        }
        
    }
}
