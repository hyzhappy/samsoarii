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

using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.UI.Base;

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// SimuViewChartModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewChartModel : SimuViewTabModel
    {
        private LinkedList<SimulateDataViewModel> sdvmodels;
        private LinkedList<SimulateDataChartModel> sdcmodels;

        public SimuViewChartModel()
        {
            InitializeComponent();
            sdvmodels = new LinkedList<SimulateDataViewModel>();
            sdcmodels = new LinkedList<SimulateDataChartModel>();
            ReinitializeComponent();
        }

        public SimuViewChartModel(IEnumerable<SimulateDataModel> sdmodels)
        {
            InitializeComponent();
            sdvmodels = new LinkedList<SimulateDataViewModel>();
            sdcmodels = new LinkedList<SimulateDataChartModel>();
            foreach (SimulateDataModel sdmodel in sdmodels)
            {
                SimulateDataViewModel sdvmodel = new SimulateDataViewModel(sdmodel);
                SimulateDataChartModel sdcmodel = new SimulateDataChartModel(sdmodel, TRuler);
                sdvmodels.AddLast(sdvmodel);
                sdcmodels.AddLast(sdcmodel);
            }
            ReinitializeComponent();
        }

        public void ReinitializeComponent()
        {
            VList.SDVModels = sdvmodels;
            VChart.SDCModels = sdcmodels;
            
            BuildRouted();
        }

        #region Actual Size
        protected override void OnActualWidthChanged()
        {
            base.OnActualWidthChanged();
            
            TRuler.ActualWidth = ActualWidth - 200;
            VChart.ActualWidth = ActualWidth - 200;
        }

        protected override void OnActualHeightChanged()
        {
            base.OnActualHeightChanged();
        }
        #endregion

        #region Event Handler

        public void BuildRouted()
        {
            VList.SDModelSetup += OnSDModelSetup;
            VList.SDModelClose += OnSDModelClose;
            VList.SDModelLock += OnSDModelLock;
            VList.SDModelView += OnSDModelView;
            VList.SDModelUnlock += OnSDModelUnlock;
            VList.SDModelUnview += OnSDModelUnview;
            VList.BuildRouted(this);

            VChart.SDModelSetup += OnSDModelSetup;
            VChart.SDModelClose += OnSDModelClose;
            VChart.BuildRouted(this);
        }
        
        public event SimulateDataModelEventHandler SDModelSetup;
        private void OnSDModelSetup(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelSetup != null)
            {
                SDModelSetup(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelClose;
        private void OnSDModelClose(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelClose != null)
            {
                SDModelClose(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelLock;
        private void OnSDModelLock(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelLock != null)
            {
                SDModelLock(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelView;
        private void OnSDModelView(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelView != null)
            {
                SDModelView(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelUnlock;
        private void OnSDModelUnlock(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelUnlock != null)
            {
                SDModelUnlock(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelUnview;
        private void OnSDModelUnview(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelUnview != null)
            {
                SDModelUnview(this, e);
            }
        }

        #endregion

    }
}
