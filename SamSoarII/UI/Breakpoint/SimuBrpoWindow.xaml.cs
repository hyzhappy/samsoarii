using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SamSoarII.AppMain;
using SamSoarII.Simulation.Core;
using SamSoarII.AppMain.UI;
using System.ComponentModel;
using SamSoarII.Simulation.Core.Event;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstViewModel;

namespace SamSoarII.Simulation.UI.Breakpoint
{
    /// <summary>
    /// SimuBrpoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimuBrpoWindow : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        static public RoutedUICommand JumpCommand { get; private set; }
            = new RoutedUICommand();
        static public RoutedUICommand ActiveSwapCommand { get; private set; }
            = new RoutedUICommand();
        static public RoutedUICommand ReleaseCommand { get; private set; }
            = new RoutedUICommand();
        static public RoutedUICommand ReleaseAllCommand { get; private set; }
            = new RoutedUICommand();

        public SimuBrpoWindow(InteractionFacade _ifacade)
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
        
        private ObservableCollection<SimuBrpoElement> items
            = new ObservableCollection<SimuBrpoElement> ();
        public IEnumerable<SimuBrpoElement> Items
        {
            get { return this.items; }
        }
        private BaseViewModel breakpoint;
        public BaseViewModel Breakpoint
        {
            get
            {
                return this.breakpoint;
            }
            private set
            {
                Dispatcher.Invoke(() =>
                {
                    if (breakpoint != null)
                        breakpoint.Background = Brushes.Transparent;
                    this.breakpoint = value;
                    if (breakpoint != null)
                        breakpoint.Background = Brushes.Yellow;
                });
            }
        }

        public void Route(ProjectModel pmodel)
        {
            Route(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                Route(ldvmodel);
            }
        }
        public void Route(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                Route(lnvmodel);
            }
        }
        public void Route(LadderNetworkViewModel lnvmodel)
        {
            lnvmodel.BreakpointChanged += OnBreakpointChanged;
        }

        public void Unroute(ProjectModel pmodel)
        {
            Unroute(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                Unroute(ldvmodel);
            }
        }
        public void Unroute(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                Unroute(lnvmodel);
            }
        }
        public void Unroute(LadderNetworkViewModel lnvmodel)
        {
            lnvmodel.BreakpointChanged -= OnBreakpointChanged;
        }

        public void Active(SimuBrpoElement ele)
        { 
            int bpaddr = ele.BVModel.BPAddress;
            int bpmsg = 0;
            int cpmsg = 0;
            switch (ele.BPRect.Label)
            {
                case "": ele.Condition = "无"; bpmsg = 1; break;
                case "0": ele.Condition = "0"; cpmsg = 1; break;
                case "1": ele.Condition = "1"; cpmsg = 2; break;
                case "↑": ele.Condition = "上升沿"; cpmsg = 4; break;
                case "↓": ele.Condition = "下降沿"; cpmsg = 8; break;
                default: ele.Condition = "无"; break;
            }
            SimulateDllModel.SetBPAddr(bpaddr, bpmsg);
            SimulateDllModel.SetCPAddr(bpaddr, cpmsg);
        }
        public void Unactive(SimuBrpoElement ele)
        {
            int bpaddr = ele.BVModel.BPAddress;
            SimulateDllModel.SetBPAddr(bpaddr, 0);
            SimulateDllModel.SetCPAddr(bpaddr, 0);
        }
        public void Remove(int id)
        {
            SimuBrpoElement ele = items[id];
            Unactive(ele);
            items.Remove(ele);
            ele.LDVModel.RemoveBreakpoint(ele.LNVModel, ele.BPRect);
        }

        private void OnJumpCommandCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (smanager != null);
        }
        private void OnActiveSwapCommandCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (smanager != null);
        }
        private void OnReleaseCommandCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (smanager != null);
        }
        private void OnReleaseAllCommandCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (smanager != null);
        }

        private void OnJumpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SimuBrpoElement ele = (SimuBrpoElement)(DG_Main.SelectedItem);
            if (ele != null)
                smanager.JumpTo(ele.BVModel.BPAddress);
        }
        private void OnActiveSwapCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SimuBrpoElement ele = (SimuBrpoElement)(DG_Main.SelectedItem);
            if (ele != null)
            {
                ele.IsActive ^= true;
                if (ele.IsActive)
                    Active(ele);
                else
                    Unactive(ele);
            }
        }
        private void OnReleaseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (DG_Main.SelectedIndex >= 0)
            {
                Remove(DG_Main.SelectedIndex);
            }
        }
        private void OnReleaseAllCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            while (items.Count() > 0)
            {
                Remove(0);
            }
        }

        private void OnBreakpointChanged(object sender, BreakpointChangedEventArgs e)
        {
            LadderNetworkViewModel lnvmodel = (LadderNetworkViewModel)sender;
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            IEnumerable<SimuBrpoElement> fit = null;
            SimuBrpoElement ele = null;
            if (e.BPRect_old != null)
            {
                fit = items.Where((_ele) => { return _ele.BPRect == e.BPRect_old; });
                if (fit.Count() > 0)
                {
                    ele = fit.First();
                    Unactive(ele);
                    items.Remove(ele);
                }
            }
            if (e.BPRect_new != null)
            {
                ele = new SimuBrpoElement();
                ele.IsActive = true;
                ele.LDVModel = ldvmodel;
                ele.LNVModel = lnvmodel;
                ele.BVModel = e.BPRect_new.BVModel;
                ele.BPRect = e.BPRect_new;
                ele.BreakTime = "1";
                items.Add(ele);
                Active(ele);
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            ifacade.MainWindow.LACBreakpoint.Show();
        }
        private void OnBreakpointResume(object sender, BreakpointPauseEventArgs e)
        {
            Breakpoint = null;
        }
        private void OnBreakpointPause(object sender, BreakpointPauseEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                BaseViewModel bvmodel = BreakPointManager.GetBVModel(e.Address);
                if (bvmodel != null)
                {
                    Breakpoint = bvmodel;
                    ifacade.NavigateToNetwork(bvmodel);
                    IEnumerable<SimuBrpoElement> fit = items.Where(
                        (_ele) => { return _ele.BVModel == bvmodel; });
                    if (fit.Count() > 0)
                    {
                        SimuBrpoElement ele = fit.First();
                        int id = items.IndexOf(ele);
                        DG_Main.SelectedIndex = id;
                    }
                }
            });
        }

        private void DG_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SimuBrpoElement ele = (SimuBrpoElement)(DG_Main.SelectedItem);
                if (ele != null)
                {
                    if (e.OriginalSource is ComboBox
                     && e.AddedItems.Count == 1)
                    {
                        ele.Condition = e.AddedItems[0].ToString();
                        switch (ele.Condition)
                        {
                            case "无": ele.BPRect.Label = String.Empty; break;
                            case "0": ele.BPRect.Label = "0"; break;
                            case "1": ele.BPRect.Label = "1"; break;
                            case "上升沿": ele.BPRect.Label = "↑"; break;
                            case "下降沿": ele.BPRect.Label = "↓"; break;
                        }
                        Active(ele);
                    }
                    ifacade.NavigateToNetwork(
                        new NavigateToNetworkEventArgs(
                            ele.LNVModel.NetworkNumber,
                            ele.LDVModel.ProgramName,
                            ele.BVModel.X,
                            ele.BVModel.Y));
                }
            });
        }
    }

    public class SimuBrpoConditionProvider
    {
        static private string[] selectedconditions
            = { "无", "0", "1", "上升沿", "下降沿" };

        public IEnumerable<string> SelectedConditions()
        {
            return selectedconditions;
        }
    }
}
