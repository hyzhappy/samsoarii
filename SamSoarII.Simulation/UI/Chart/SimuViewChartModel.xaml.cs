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
        private SimulateModel parent;

        private LinkedList<SimulateDataViewModel> sdvmodels;
        private LinkedList<SimulateDataChartModel> sdcmodels;

        private List<SimulateDataModel> copysdmodels;
        private double copystart;
        private double copyend;

        public SimuViewChartModel()
        {
            InitializeComponent();
            sdvmodels = new LinkedList<SimulateDataViewModel>();
            sdcmodels = new LinkedList<SimulateDataChartModel>();
            copysdmodels = new List<SimulateDataModel>();
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

            VChart.ActualHeight = ActualHeight - 32;
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

            VChart.SDModelRun += OnSDModelRun;
            VChart.SDModelCopy += OnSDModelCopy;
            VChart.SDModelCut += OnSDModelCut;
            VChart.SDModelPaste += OnSDModelPaste;
            VChart.SDModelSelect += OnSDModelSelect;
            VChart.SDModelDelete += OnSDModelDelete;
            VChart.SDModelDraw += OnSDModelDraw;
            VChart.XYModelCreate += OnXYModelCreate;
            VChart.BuildRouted(this);
        }

        public void BuildRouted(SimulateModel _parent)
        {
            parent = _parent;
            parent.RunDataFinished += OnRunDataFinished;
            parent.RunDrawFinished += OnRunDrawFinished;
        }

        public event SimulateDataModelEventHandler RunDrawFinished;
        private void OnRunDrawFinished(object sender, SimulateDataModelEventArgs e)
        {
            if (RunDrawFinished != null)
            {
                RunDrawFinished(this, e);
            }
        }

        public event SimulateDataModelEventHandler RunDataFinished;
        private void OnRunDataFinished(object sender, SimulateDataModelEventArgs e)
        {
            if (RunDataFinished != null)
            {
                RunDataFinished(this, e);
            }
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
            if (e.SDModel_old.IsView)
            {
                e.SDModel_new.IsView = false;
                if (SDModelUnview != null)
                {
                    SDModelUnview(this, e);
                }
            }
            e.SDModel_new.IsLock = true;
            if (SDModelLock != null)
            {
                SDModelLock(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelView;
        private void OnSDModelView(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDModel_old.IsLock)
            {
                e.SDModel_new.IsLock = false;
                if (SDModelUnlock != null)
                {
                    SDModelUnlock(this, e);
                }
            }
            e.SDModel_new.IsView = true;
            if (SDModelView != null)
            {
                SDModelView(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelUnlock;
        private void OnSDModelUnlock(object sender, SimulateDataModelEventArgs e)
        {
            e.SDModel_new.IsLock = false;
            if (SDModelUnlock != null)
            {
                SDModelUnlock(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelUnview;
        private void OnSDModelUnview(object sender, SimulateDataModelEventArgs e)
        {
            e.SDModel_new.IsView = false;
            if (SDModelUnview != null)
            {
                SDModelUnview(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelRun;
        private void OnSDModelRun(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelRun != null)
            {
                SDModelRun(this, e);
            }
        }
        
        private void OnSDModelCut(object sender, SimulateDataModelEventArgs e)
        {
            copysdmodels.Clear();
            copystart = e.TimeStart;
            copyend = e.TimeEnd;
            foreach (SimulateDataChartModel sdcmodel in VChart.CursorCollection())
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel.Clone();
                copysdmodels.Add(sdmodel);
                VChart.CursorRemoveValue(sdcmodel.SDModel);
            }
        }

        private void OnSDModelCopy(object sender, SimulateDataModelEventArgs e)
        {
            copysdmodels.Clear();
            copystart = e.TimeStart;
            copyend = e.TimeEnd;
            foreach (SimulateDataChartModel sdcmodel in VChart.CursorCollection())
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel.Clone();
                copysdmodels.Add(sdmodel);
            }
        }

        private void OnSDModelPaste(object sender, SimulateDataModelEventArgs e)
        {
            int id = e.ID;
            foreach (SimulateDataModel sdmodel in copysdmodels)
            {
                SimulateDataChartModel sdcmodel = VChart.SDCModels.ElementAt(id);
                VChart.SetValue(sdmodel, sdcmodel.SDModel, (int)(copystart), (int)(copyend), (int)(e.TimeStart), (int)(e.TimeEnd));
            }
        }

        private void OnSDModelSelect(object sender, SimulateDataModelEventArgs e)
        {

        }

        private void OnSDModelDelete(object sender, SimulateDataModelEventArgs e)
        {
            foreach (SimulateDataChartModel sdcmodel in VChart.CursorCollection())
            {
                VChart.CursorRemoveValue(sdcmodel.SDModel);
            }
        }

        public event SimulateDataModelEventHandler SDModelDraw;
        private void OnSDModelDraw(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelDraw != null)
            {
                SDModelDraw(this, e);
            }
        }

        public event SimulateDataModelEventHandler XYModelCreate;
        private void OnXYModelCreate(object sender, SimulateDataModelEventArgs e)
        {
            if (XYModelCreate != null)
            {
                XYModelCreate(this, e);
            }
        }

        #endregion

    }
}
