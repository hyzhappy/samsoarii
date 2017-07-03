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
using System.Xml.Linq;
using Microsoft.Win32;
using SamSoarII.UserInterface;

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

        public void Update()
        {
            VList.Update();
            VChart.Update();
            VChart.UpdateChart();
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
            VChart.SDModelSave += OnSDModelSave;
            VChart.SDModelLoad += OnSDModelLoad;
            VChart.SDModelSaveAll += OnSDModelSaveAll;
            VChart.SDModelLoadAll += OnSDModelLoadAll;
            VChart.BuildRouted(this);
        }

        public void BuildRouted(SimulateModel _parent)
        {
            parent = _parent;
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
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.SDCModel = e.SDCModel;
            _e.SDVModel = e.SDVModel;
            if (SDModelUnlock != null && e.SDModel_old.IsLock)
            {
                _e.SDModel_old = _e.SDModel_new = e.SDModel_old; 
                SDModelUnlock(this, _e);
            }
            if (SDModelUnview != null && e.SDModel_old.IsView)
            {
                _e.SDModel_old = _e.SDModel_new = e.SDModel_old;
                SDModelUnview(this, _e);
            }
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

        private void OnSDModelSave(object sender, SimulateDataModelEventArgs e)
        {
            SaveFileDialog sfdialog = new SaveFileDialog();
            sfdialog.Filter = "SimuWave文件|*.siw";
            sfdialog.Title = "保存仿真波形文件";
            if (sfdialog.ShowDialog() == true)
            {
                try
                {
                    string filename = sfdialog.FileName;
                    SaveXml(filename, (int)(e.TimeStart), (int)(e.TimeEnd));
                }
                catch (Exception)
                {
                    LocalizedMessageBox.Show("保存波形文件失败!",LocalizedMessageIcon.Error);
                }
            }
        }

        private void OnSDModelLoad(object sender, SimulateDataModelEventArgs e)
        {
            OpenFileDialog ofdialog = new OpenFileDialog();
            ofdialog.Filter = "SimuWave文件|*.siw";
            ofdialog.Title = "读取仿真波形文件";
            if (ofdialog.ShowDialog() == true)
            {
                try
                {
                    string filename = ofdialog.FileName;
                    LoadXml(filename, (int)(e.TimeStart), (int)(e.TimeEnd));
                }
                catch (Exception)
                {
                    LocalizedMessageBox.Show("读取波形文件失败!", LocalizedMessageIcon.Error);
                }
            }
        }

        private void OnSDModelSaveAll(object sender, SimulateDataModelEventArgs e)
        {
            SaveFileDialog sfdialog = new SaveFileDialog();
            sfdialog.Filter = "SimuWave文件|*.siw";
            sfdialog.Title = "保存仿真波形文件";
            if (sfdialog.ShowDialog() == true)
            {
                try
                {
                    string filename = sfdialog.FileName;
                    SaveXml(filename);
                }
                catch (Exception)
                {
                    LocalizedMessageBox.Show("保存波形文件失败!", LocalizedMessageIcon.Error);
                }
            }
        }

        private void OnSDModelLoadAll(object sender, SimulateDataModelEventArgs e)
        {
            OpenFileDialog ofdialog = new OpenFileDialog();
            ofdialog.Filter = "SimuWave文件|*.siw";
            ofdialog.Title = "读取仿真波形文件";
            if (ofdialog.ShowDialog() == true)
            {
                try
                {
                    string filename = ofdialog.FileName;
                    LoadXml(filename);
                }
                catch (Exception)
                {
                    LocalizedMessageBox.Show("读取波形文件失败!", LocalizedMessageIcon.Error);
                }
            }
        }

        #endregion

        #region Save & Load
        private void SaveXml(string filename, int timestart, int timeend)
        {
            XDocument xdoc = new XDocument();
            XElement node_Root = new XElement("SimuDatas");
            XElement node_SDModel = null;
            foreach (SimulateDataChartModel sdcmodel in VChart.CursorCollection())
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel;
                node_SDModel = new XElement("SimuDataNode");
                sdmodel.SaveXml(node_SDModel, timestart, timeend);
                node_Root.Add(node_SDModel);
            }
            xdoc.Add(node_Root);
            xdoc.Save(filename);
        }

        private void LoadXml(string filename, int timestart, int timeend)
        {
            XDocument xdoc = XDocument.Load(filename);
            XElement node_Root = xdoc.Element("SimuDatas");
            IEnumerable<XElement> node_SDModels = node_Root.Elements("SimuDataNode");
            foreach (XElement node_SDModel in node_SDModels)
            {
                string name = (string)(node_SDModel.Element("Name"));
                foreach (SimulateDataChartModel sdcmodel in VChart.SDCModels)
                {
                    SimulateDataModel sdmodel = sdcmodel.SDModel;
                    if (sdmodel.Name.Equals(name))
                    {
                        VChart.RemoveValue(sdmodel, timestart, timeend);
                        sdmodel.LoadXml(node_SDModel, timestart, timeend);
                        break;
                    }
                }
            }
            VChart.UpdateChart();
        }

        private void SaveXml(string filename)
        {
            XDocument xdoc = new XDocument();
            XElement node_Root = new XElement("SimuDatas");
            XElement node_SDModel = null;
            foreach (SimulateDataChartModel sdcmodel in VChart.SDCModels)
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel;
                node_SDModel = new XElement("SimuDataNode");
                sdmodel.SaveXml(node_SDModel);
                node_Root.Add(node_SDModel);
            }
            xdoc.Add(node_Root);
            xdoc.Save(filename);
        }

        private void LoadXml(string filename)
        {
            XDocument xdoc = XDocument.Load(filename);
            XElement node_Root = xdoc.Element("SimuDatas");
            IEnumerable<XElement> node_SDModels = node_Root.Elements("SimuDataNode");
            SimulateDataModelEventArgs e;
            foreach (SimulateDataChartModel sdcmodel in VChart.SDCModels)
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel;
                e = new SimulateDataModelEventArgs();
                e.SDModel_old = sdmodel;
                e.SDModel_new = null;
                OnSDModelClose(this, e);
            }
            //VList.Clear();
            //VChart.Clear();
            int id = 0;
            foreach (XElement node_SDModel in node_SDModels)
            {
                SimulateDataModel sdmodel = new SimulateDataModel();
                sdmodel.LoadXml(node_SDModel);
                e = new SimulateDataModelEventArgs();
                e.SDModel_new = sdmodel;
                e.ID = id++;
                if (SDModelSetup != null)
                {
                    SDModelSetup(this, e);
                }
            }
            foreach (SimulateDataChartModel sdcmodel in VChart.SDCModels)
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel;
                e = new SimulateDataModelEventArgs();
                e.SDModel_new = sdmodel;
                if (sdmodel.IsLock && SDModelLock != null)
                {
                    SDModelLock(this, e);
                }
                if (sdmodel.IsView && SDModelView != null)
                {
                    SDModelView(this, e);
                }
            }
        }
        #endregion

    }
}
