using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading;

using SamSoarII.Simulation.Core;
using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.Shell;
using SamSoarII.Simulation.Shell.ViewModel;
using SamSoarII.Simulation.Shell.Event;
using SamSoarII.Simulation.UI;
using SamSoarII.Simulation.UI.Monitor;
using SamSoarII.Simulation.UI.PLCTop;

using SamSoarII.Extend.Utility;
using System.Diagnostics;
using System.IO;
using SamSoarII.Simulation.Core.Event;

namespace SamSoarII.Simulation
{
    public class SimulateModel
    {
        private SimulateManager smanager;

        //private List<SimuViewBaseModel> svmodels;
        
        private SortedSet<SimulateVariableUnit> suvars;

        private List<SimulateVariableUnit> smvars;

        private TextBox report;
        public TextBox ReportTextBox
        {
            set { this.report = value; }
        }

        private Thread UpdateThread;

        public SimuViewAllDiaModel AllRoutine;

        public SimuViewDiagramModel MainRoutine;

        public List<SimuViewDiagramModel> SubRoutines;

        public SimuViewFuncBlockModel AllFuncs;

        public List<SimuViewFuncBlockModel> FuncBlocks;

        public SimulateWindow MainWindow;

        public PLCTopPhoto plcTopPhoto;

        public MonitorTable mtable;

        public SimulateBitModel LEDBit;
        
        public SimulateModel()
        {
            smanager = new SimulateManager();
            //svmodels = new List<SimuViewBaseModel>();
            suvars = new SortedSet<SimulateVariableUnit>(new SimulateVariableUintComparer());
            smvars = new List<SimulateVariableUnit>();
            AllRoutine = null;
            MainRoutine = null;
            SubRoutines = new List<SimuViewDiagramModel>();
            AllFuncs = null;
            FuncBlocks = new List<SimuViewFuncBlockModel>();
            LEDBit = new SimulateBitModel();
            LEDBit.Base = "Y0";
            LEDBit.Size = 8;
            smanager.Add(LEDBit);
        }
        
        private void _Update_Thread()
        {
            while (true)
            {
                smanager.Update();
                MainRoutine.Update();
                foreach (SimuViewDiagramModel svdmodel in SubRoutines)
                {
                    svdmodel.Update();
                }
                plcTopPhoto.LEDLight_Y0.Status = Math.Min(2, ((int)(LEDBit.Values[0].Value)));
                plcTopPhoto.LEDLight_Y1.Status = Math.Min(2, ((int)(LEDBit.Values[1].Value)));
                plcTopPhoto.LEDLight_Y2.Status = Math.Min(2, ((int)(LEDBit.Values[2].Value)));
                plcTopPhoto.LEDLight_Y3.Status = Math.Min(2, ((int)(LEDBit.Values[3].Value)));
                plcTopPhoto.LEDLight_Y4.Status = Math.Min(2, ((int)(LEDBit.Values[4].Value)));
                plcTopPhoto.LEDLight_Y5.Status = Math.Min(2, ((int)(LEDBit.Values[5].Value)));
                plcTopPhoto.LEDLight_Y6.Status = Math.Min(2, ((int)(LEDBit.Values[6].Value)));
                plcTopPhoto.LEDLight_Y7.Status = Math.Min(2, ((int)(LEDBit.Values[7].Value)));
                mtable.UpdateValue();
                Thread.Sleep(200);
            }
        }

        public SimulateVariableUnit GetVariableUnit(string name, string type)
        {
            switch (name[0])
            {
                case 'K':
                case 'F':
                case 'H':
                    return GetConstantUnit(name, type);
                default:
                    break;
            }
            SimulateVariableUnit unit = null;
            switch (type)
            {
                case "BIT":
                    unit = new SimulateBitUnit();
                    break;
                case "WORD":
                    unit = new SimulateWordUnit();
                    break;
                case "DWORD":
                    unit = new SimulateDWordUnit();
                    break;
                case "FLOAT":
                    unit = new SimulateFloatUnit();
                    break;
                case "DOUBLE":
                    unit = new SimulateDoubleUnit();
                    break;
                default:
                    return unit;
            }
            unit.Name = name;
            return smanager.GetVariableUnit(unit);
        }

        public SimulateVariableUnit GetConstantUnit(string name, string type)
        {
            SimulateVariableUnit unit = null;
            switch (type)
            {
                case "BIT":
                    unit = new SimulateBitUnit();
                    unit.Value = int.Parse(name.Substring(1));
                    break;
                case "WORD":
                    unit = new SimulateWordUnit();
                    unit.Value = int.Parse(name.Substring(1));
                    break;
                case "DWORD":
                    unit = new SimulateDWordUnit();
                    unit.Value = int.Parse(name.Substring(1));
                    break;
                case "FLOAT":
                    unit = new SimulateFloatUnit();
                    unit.Value = float.Parse(name.Substring(1));
                    break;
                case "DOUBLE":
                    unit = new SimulateDoubleUnit();
                    unit.Value = double.Parse(name.Substring(1));
                    break;
                default:
                    return unit;
            }
            unit.Name = name;
            return unit;
        }

        #region Event handler
        private void OnMainWindowClosed(object sender, EventArgs e)
        {

            if (plcTopPhoto.RunTrigger.Status == UI.PLCTop.Trigger.STATUS_RUN)
            {
                smanager.Stop();
            }
            UpdateThread.Abort();
        }

        private void OnTabOpened(object sender, ShowTabItemEventArgs e)
        {
            if (e.TabName.Equals("所有程序"))
            {
                ShowItem(AllRoutine, e.TabName);
                return;
            }
            if (e.TabName.Equals("主程序"))
            {
                ShowItem(MainRoutine, e.TabName);
                return;
            }
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                if (e.TabName.Equals(svdmodel.Name))
                {
                    ShowItem(svdmodel, e.TabName);
                    return;
                }
            }
            foreach (SimuViewFuncBlockModel svfmodel in FuncBlocks)
            {
                if (e.TabName.Equals(svfmodel.Name))
                {
                    ShowItem(svfmodel, e.TabName);
                    return;
                }
            }
        }
        
        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        
        private void OnProjectTreeDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                TreeViewItem tvi = sender as TreeViewItem;
                if (tvi == MainWindow.PTView.TVI_AllRoutine)
                {
                    OnTabOpened(sender, new ShowTabItemEventArgs("所有程序"));
                }
                else
                if (tvi == MainWindow.PTView.TVI_MainRoutine)
                {
                    OnTabOpened(sender, new ShowTabItemEventArgs("主程序"));
                }
                else
                {
                    OnTabOpened(sender, new ShowTabItemEventArgs(tvi.Header.ToString()));
                }
            }
        }
        
        private void PLCTopPhotoTriggerStop(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void PLCTopPhotoTriggerRun(object sender, RoutedEventArgs e)
        {
            Start();
        }
        
        private void OnVariableUnitChanged(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Replace(e.Old, e.New);
        }

        private void OnVariableUnitLocked(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Lock(e.Old);
        }

        private void OnVariableUnitUnlocked(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Unlock(e.Old);
        }

        #endregion

        #region User Interface
        private void BuildRouted()
        {
            MainWindow.Closed += OnMainWindowClosed;
            
            ProjectTreeView ptview = MainWindow.PTView;
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                ptview.AddTreeViewItem(svdmodel.Name, ProjectTreeView.ADDTVI_TYPE_SUBROUTINES);
            }
            foreach (SimuViewFuncBlockModel svfmodel in FuncBlocks)
            {
                ptview.AddTreeViewItem(svfmodel.Name, ProjectTreeView.ADDTVI_TYPE_FUNCBLOCKS);
            }

            TreeViewItem tvi_arou = ptview.TVI_AllRoutine;
            TreeViewItem tvi_mrou = ptview.TVI_MainRoutine;
            TreeViewItem tvi_srou = ptview.TVI_SubRoutines;
            TreeViewItem tvi_fblo = ptview.TVI_FuncBlocks;
            tvi_arou.MouseDoubleClick += OnProjectTreeDoubleClicked;
            tvi_mrou.MouseDoubleClick += OnProjectTreeDoubleClicked;
            foreach (TreeViewItem tvi in tvi_srou.Items)
            {
                tvi.MouseDoubleClick += OnProjectTreeDoubleClicked;
            }
            foreach (TreeViewItem tvi in tvi_fblo.Items)
            {
                tvi.MouseDoubleClick += OnProjectTreeDoubleClicked;
            }

            plcTopPhoto = MainWindow.PLCTopView;
            plcTopPhoto.RunTrigger.Run += PLCTopPhotoTriggerRun;
            plcTopPhoto.RunTrigger.Stop += PLCTopPhotoTriggerStop;
            
            SimulateVInputUnit sviunit = new SimulateVInputUnit();
            smvars.Add(sviunit);

            mtable = MainWindow.MTable;
            mtable.VariableUnitChanged += OnVariableUnitChanged;
            mtable.VariableUnitClosed += OnVariableUnitChanged;
            mtable.VariableUnitLocked += OnVariableUnitLocked;
            mtable.VariableUnitUnlocked += OnVariableUnitUnlocked;
            mtable.SVUnits = smvars;
            mtable.Update();
            
            UpdateThread = new Thread(_Update_Thread);
            UpdateThread.Start();
        }
        
        public void ShowWindow()
        {
            MainWindow = new SimulateWindow();
            BuildRouted();
            MainWindow.Show();
        }

        public void ShowItem(SimuViewAllDiaModel svdmodel, string name)
        {
            MainWindow.MainTab.ShowItem(svdmodel, name);
        }

        public void ShowItem(SimuViewDiagramModel svdmodel, string name)
        {
            MainWindow.MainTab.ShowItem(svdmodel, name);
        }

        public void ShowItem(SimuViewFuncBlockModel svdmodel, string name)
        {

            MainWindow.MainTab.ShowItem(svdmodel, name);
        }
        #endregion

        #region Simulate Control

        public void Start()
        {
            smanager.Start();
            //UpdateThread = new Thread(_Update_Thread);
            //UpdateThread.Start();
            plcTopPhoto.RunLight.Status = StatusLight.STATUS_LIGHT;
            plcTopPhoto.StopLight.Status = StatusLight.STATUS_DARK;
        }
        
        public void Stop()
        {
            smanager.Stop();
            plcTopPhoto.RunLight.Status = StatusLight.STATUS_DARK;
            plcTopPhoto.StopLight.Status = StatusLight.STATUS_LIGHT;
        }

        #endregion

        #region Check
        public int Check()
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += "开始检查线路合法...\r\n"; });
            ret += CheckCircuit_Diagram(MainRoutine);
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                ret += CheckCircuit_Diagram(svdmodel);
            }
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共 {0:d} 处错误，仿真初始化失败！", ret); });
                return ret;
            }
            report.Dispatcher.Invoke(() => { report.Text += "开始生成PLC指令...\r\n"; });
            ret += GenPLCInst_Diagram(MainRoutine);
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                ret += GenPLCInst_Diagram(svdmodel);
            }
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共 {0:d} 处错误，仿真初始化失败！", ret); });
                return ret;
            }
            report.Dispatcher.Invoke(() => { report.Text += "生成仿真支持文件...\r\n"; });
            List<InstHelper.PLCInstNetwork> nets = new List<InstHelper.PLCInstNetwork>();
            ret += MergeAll_Diagram(nets, MainRoutine);
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                ret += MergeAll_Diagram(nets, svdmodel);
            }

            string currentPath = Environment.CurrentDirectory;
            string ladderHFile = String.Format(@"{0:s}\simug\simuc.h", currentPath);
            string ladderCFile = String.Format(@"{0:s}\simug\simuc.c", currentPath);
            string funcBlockHFile = String.Format(@"{0:s}\simug\simuf.h", currentPath);
            string funcBlockCFile = String.Format(@"{0:s}\simug\simuf.c", currentPath);
            string simulibHFile = String.Format(@"{0:s}\simug\simulib.h", currentPath);
            string simulibCFile = String.Format(@"{0:s}\simug\simulib.c", currentPath);
            string outputDllFile = String.Format(@"{0:s}\simuc.dll", currentPath);
            string outputAFile = String.Format(@"{0:s}\simuc.a", currentPath);

            StreamWriter sw = new StreamWriter(ladderCFile);
            InstHelper.InstToSimuCode(sw, nets.ToArray());
            sw.Close();

            sw = new StreamWriter(funcBlockHFile);
            AllFuncs.GenerateCHeader(sw);
            sw.Close();

            sw = new StreamWriter(funcBlockCFile);
            AllFuncs.GenerateCCode(sw);
            sw.Close();

            SimulateDllModel.CreateDll(ladderCFile, funcBlockCFile, outputDllFile, outputAFile);
            ret = SimulateDllModel.LoadDll(outputDllFile);

            report.Dispatcher.Invoke(() =>
            {
                switch (ret)
                {
                    case SimulateDllModel.LOADDLL_OK:
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_DLLFILE:
                        report.Text += "error : 找不到生成的dll文件simuc.dll\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETBIT:
                        report.Text += "error : 找不到入口GetBit\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETWORD:
                        report.Text += "error : 找不到入口GetWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDWORD:
                        report.Text += "error : 找不到入口GetDWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETFLOAT:
                        report.Text += "error : 找不到入口GetFloat\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDOUBLE:
                        report.Text += "error : 找不到入口GetDouble\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETBIT:
                        report.Text += "error : 找不到入口SetBit\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETWORD:
                        report.Text += "error : 找不到入口SetWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDWORD:
                        report.Text += "error : 找不到入口SetDWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETFLOAT:
                        report.Text += "error : 找不到入口SetFloat\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDOUBLE:
                        report.Text += "error : 找不到入口SetDouble\r\n";
                        break;
                    default:
                        report.Text += "error : 发生未知错误\r\n";
                        break;
                }
            });

            return ret;
        }
        
        private int CheckCircuit_Diagram(SimuViewDiagramModel svdmodel)
        {
            int ret = 0;
            foreach (SimuViewNetworkModel svnmodel in svdmodel.GetNetworks())
            {
                ret += CheckCircuit_Network(svnmodel);
            }
            return ret;
        }

        private int CheckCircuit_Network(SimuViewNetworkModel svnmodel)
        {
            int ret = 0;
            ret += svnmodel.CheckCircuit(report);
            return ret;
        }
        
        private int GenPLCInst_Diagram(SimuViewDiagramModel svdmodel)
        {
            int ret = 0;
            foreach (SimuViewNetworkModel svnmodel in svdmodel.GetNetworks())
            {
                ret += GenPLCInst_Network(svnmodel);
            }
            return ret;
        }

        private int GenPLCInst_Network(SimuViewNetworkModel svnmodel)
        {
            int ret = 0;
            ret += svnmodel.GenPLCInst(report);
            return ret;
        }
        
        private int MergeAll_Diagram(List<InstHelper.PLCInstNetwork> nets, SimuViewDiagramModel svdmodel)
        {
            int ret = 0;
            foreach (SimuViewNetworkModel svnmodel in svdmodel.GetNetworks())
            {
                ret += MergeAll_Network(nets, svnmodel, svdmodel.Name);
            }
            return ret;
        }

        private int MergeAll_Network(List<InstHelper.PLCInstNetwork> nets, SimuViewNetworkModel svnmodel, string name)
        {
            int ret = 0;
            InstHelper.PLCInstNetwork net = new InstHelper.PLCInstNetwork(
                name, svnmodel.PLCInsts.ToArray());
            nets.Add(net);
            return ret;
        }
        #endregion


    }
}
