using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class MainTabDiagramItem : UserControl, ITabItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private static event PropertyChangedEventHandler StaticPropertyChanged = delegate { };

        private void OnStaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }

        #region Floating

        private bool isfloat;
        public bool IsFloat {
            get { return this.isfloat; }
            set
            {
                this.isfloat = value;
                if (LDVM_ladder != null)
                    LDVM_ladder.IsFloat = value;
            }
        }
        private LayoutFloatingWindow floatwindow;
        public LayoutFloatingWindow FloatWindow
        {
            get { return this.floatwindow; }
            set
            {
                this.floatwindow = value;
                if (LDVM_ladder != null)
                    LDVM_ladder.FloatWindow = value;
            }
        }
        private LayoutFloatingWindowControl floatcontrol;
        public LayoutFloatingWindowControl FloatControl
        {
            get { return this.floatcontrol; }
            set
            {
                this.floatcontrol = value;
                floatcontrol.Closed += OnFloatClosed;
                if (LDVM_ladder != null)
                    LDVM_ladder.FloatControl = value;
            }
        }
        public event RoutedEventHandler FloatClosed = delegate { };
        private void OnFloatClosed(object sender, EventArgs e)
        {
            FloatClosed(this, new RoutedEventArgs());
        }

        #endregion

        #region Numbers

        static private double gsheight;

        public double GSHeight
        {
            get { return gsheight; }
            set
            {
                gsheight = value;
                StaticPropertyChanged(this, new PropertyChangedEventArgs("GSHeight"));
            }
        }

        #region View Mode
        public const int VIEWMODE_LADDER = 0x01;
        public const int VIEWMODE_INST = 0x02;

        private int viewmode;
        public int ViewMode
        {
            get { return this.viewmode; }
            set
            {
                this.viewmode = value;
                if ((viewmode & VIEWMODE_LADDER) == 0
                 && (viewmode & VIEWMODE_INST) == 0)
                {
                    Content = null;
                }
                else if ((viewmode & VIEWMODE_LADDER) != 0
                      && (viewmode & VIEWMODE_INST) == 0)
                {
                    GB_Ladder.Content = null;
                    Content = LDVM_ladder;
                }
                else if ((viewmode & VIEWMODE_LADDER) == 0
                      && (viewmode & VIEWMODE_INST) != 0)
                {
                    GB_Inst.Content = null;
                    Content = IDVM_inst;
                }
                else
                {
                    Content = G_Main;
                    GB_Ladder.Content = LDVM_ladder;
                    GB_Inst.Content = IDVM_inst;
                }
            }
        }
        #endregion

        #region ITabItem Interfaces
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
        #endregion

        public LadderDiagramViewModel LDVM_ladder { get; private set; }

        public InstructionDiagramViewModel IDVM_inst { get; private set; }
        
        #endregion

        public MainTabDiagramItem()
        {
            InitializeComponent();
        }

        public MainTabDiagramItem(IProgram ITI_ladder, int _viewmode)
        {
            InitializeComponent();

            DataContext = this;
            StaticPropertyChanged += OnStaticPropertyChanged;

            LDVM_ladder = (LadderDiagramViewModel)(ITI_ladder);
            GB_Ladder.Content = LDVM_ladder;
            IDVM_inst = (InstructionDiagramViewModel)(LDVM_ladder.IDVModel);
            GB_Inst.Content = IDVM_inst;
            ViewMode = _viewmode;
        }
    }
}

