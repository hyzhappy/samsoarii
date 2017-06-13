using SamSoarII.AppMain;
using SamSoarII.AppMain.Project;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Simulation.Core;
using SamSoarII.Simulation.Core.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SamSoarII.Simulation.UI.Breakpoint
{
    /// <summary>
    /// BackTraceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BackTraceWindow : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public BackTraceWindow(InteractionFacade _ifacade)
        {
            InitializeComponent();
            DataContext = this;
            ifacade = _ifacade;
        }

        private InteractionFacade ifacade;
        private SimulateManager smanager;
        public SimulateManager SManager
        {
            get { return this.smanager; }
            set
            {
                if (smanager != null)
                {
                    smanager.BreakpointPause -= OnBreakpointPause;
                    smanager.BreakpointResume -= OnBreakpointResume;
                }
                this.smanager = value;
                if (smanager != null)
                {
                    smanager.BreakpointPause += OnBreakpointPause;
                    smanager.BreakpointResume += OnBreakpointResume;
                }
            }
        }
        private ObservableCollection<BackTraceElement> items
            = new ObservableCollection<BackTraceElement>();
        public IEnumerable<BackTraceElement> Items
        {
            get { return this.items; }
        }
        private int[] data = new int[1024];
        
        private BackTraceElement GetElement(int bpaddr)
        {
            BaseViewModel bvmodel = BreakPointManager.GetBVModel(bpaddr);
            if (bvmodel != null)
            {
                LadderNetworkViewModel lnvmodel = ifacade.ProjectModel.GetNetwork(bvmodel);
                if (lnvmodel == null) return null;
                LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
                SimuBrpoElement brpo = new SimuBrpoElement();
                brpo.LDVModel = ldvmodel;
                brpo.LNVModel = lnvmodel;
                brpo.BVModel = bvmodel;
                return new BackTraceElement(BackTraceType.Diagram, brpo);
            }
            FuncBlock fblock = BreakPointManager.GetFBlock(bpaddr);
            if (fblock != null)
            {
                FuncBlockViewModel fbvmodel = ifacade.ProjectModel.GetFuncBlock(fblock);
                if (fbvmodel == null) return null;
                FuncBrpoElement brpo = new FuncBrpoElement();
                brpo.FBVModel = fbvmodel;
                brpo.FBlock = fblock;
                return new BackTraceElement(BackTraceType.FuncBlock, brpo);
            }
            return new BackTraceElement(BackTraceType.External);
        }

        private void OnBreakpointResume(object sender, BreakpointPauseEventArgs e)
        {
            items.Clear();
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }
        private void OnBreakpointPause(object sender, BreakpointPauseEventArgs e)
        {
            BackTraceElement btele = null;
            int size = SimulateDllModel.GetBackTrace(data);
            items.Clear();
            for (int i = size / 2 - 1; i >= 0 ; i--)
            {
                btele = GetElement(data[i * 2]);
                if (btele != null) items.Add(btele);
            }
            int bpaddr = SimulateDllModel.GetBPAddr();
            btele = GetElement(bpaddr);
            if (btele != null) items.Add(btele);
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }
    }

}
