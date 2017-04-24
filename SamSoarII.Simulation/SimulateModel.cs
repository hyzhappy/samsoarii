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
using SamSoarII.Simulation.UI.Chart;
using SamSoarII.Simulation.UI.Monitor;
using SamSoarII.Simulation.UI.PLCTop;
using SamSoarII.Extend.Utility;
using System.Diagnostics;
using System.IO;
using SamSoarII.Simulation.Core.Event;
using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.UI.Base;

/// <summary>
/// Namespace : SamSoarII.Simulation
/// ClassName : SimulateModel
/// Version   : 1.0
/// Date      : 2017/3/31
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 管理整个PLC仿真过程的模型类
/// 里面包含仿真的工具类和界面类的成员
/// 以及这些成员的事件驱动接口
/// </remarks>

namespace SamSoarII.Simulation
{
    public class SimulateModel : IDisposable
    {
        #region Numbers
        /// <summary>
        /// 仿真管理器
        /// </summary>
        private SimulateManager smanager;
        /// <summary>
        /// 所有元件单元的显示模型，已弃用
        /// </summary>
        //private List<SimuViewBaseModel> svmodels;
        /// <summary>
        /// 所有注册的监视变量的有序集，用于查重，已弃用
        /// </summary>
        //private SortedSet<SimulateVariableUnit> suvars;
        /// <summary>
        /// 所有注册的监视变量的列表
        /// </summary>
        private List<SimulateVariableUnit> smvars;
        /// <summary>
        /// 仿真信息返回报告的文本窗口
        /// </summary>
        private TextBox report;
        /// <summary>
        /// 仿真信息返回报告的文本窗口
        /// </summary>
        public TextBox ReportTextBox
        {
            private get { return this.report; }
            set
            {
                this.report = value;
                report.Clear();
            }
        }
        /// <summary>
        /// 定时更新界面的线程
        /// </summary>
        private Thread UpdateThread;
        /// <summary>
        /// 显示所有元件程序的界面
        /// </summary>
        public SimuViewAllDiaModel AllRoutine;
        /// <summary>
        /// 显示元件主程序的界面
        /// </summary>
        public SimuViewDiagramModel MainRoutine;
        /// <summary>
        /// 显示元件子程序的界面的列表
        /// </summary>
        public List<SimuViewDiagramModel> SubRoutines;
        /// <summary>
        /// 显示所有用户实现函数的界面
        /// </summary>
        public SimuViewFuncBlockModel AllFuncs;
        /// <summary>
        /// 显示用户实现的函数的界面的列表
        /// </summary>
        public List<SimuViewFuncBlockModel> FuncBlocks;
        /// <summary>
        /// 显示主图表的界面
        /// </summary>
        public SimuViewChartModel MainChart;
        /// <summary>
        /// 显示所有子图表的界面的列表
        /// </summary>
        public List<SimuViewTabModel> SubCharts;
        /// <summary>
        /// 仿真的主窗口，由于通过AvalonDock和软件窗口合并，故已弃用
        /// </summary>
        //public SimulateWindow MainWindow;
        /// <summary>
        /// 仿真的工程资源树界面
        /// </summary>
        public ProjectTreeView PTView;
        /// <summary>
        /// 仿真的控制台界面
        /// </summary>
        public PLCTopPhoto PLCTopPhoto;
        /// <summary>
        /// 监视变量的表格的界面
        /// </summary>
        public MonitorTable MTable;
        /// <summary>
        /// LED灯显示的变量组模型
        /// </summary>
        public SimulateBitModel LEDBit;
        #endregion

        /// <summary>
        /// 给定名称和类型获得变量单元
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="type">变量类型（BIT/WORD/DWORD/FLOAT/DOUBLE）</param>
        /// <returns></returns>
        public SimulateVariableUnit GetVariableUnit(string name, string type)
        {
            // 如果是常量的头
            switch (name[0])
            {
                case 'K':
                case 'F':
                case 'H':
                    return GetConstantUnit(name, type);
                default:
                    break;
            }
            // 剩下的是变量的头
            // 在管理器中查找
            SimulateVariableUnit unit = null;
            unit = smanager.Find(name, type);
            if (unit != null)
            {
                return unit;
            }
            // 不存在则新建
            unit = SimulateVariableUnit.Create(name, type);
            // 注册到管理器中
            smanager.Add(unit);
            return unit;
        }

        /// <summary>
        /// 给定名称和类型获得常量单元
        /// </summary>
        /// <param name="name">常量名称</param>
        /// <param name="type">常量类型（BIT/WORD/DWORD/FLOAT/DOUBLE）</param>
        /// <returns></returns>
        public SimulateVariableUnit GetConstantUnit(string name, string type)
        {
            SimulateVariableUnit unit = null;
            switch (type)
            {
                case "BIT":
                    unit = new SimulateBitUnit();
                    unit.Value = Int32.Parse(name.Substring(1));
                    break;
                case "WORD":
                    unit = new SimulateWordUnit();
                    unit.Value = Int32.Parse(name.Substring(1));
                    break;
                case "DWORD":
                    unit = new SimulateDWordUnit();
                    unit.Value = Int64.Parse(name.Substring(1));
                    break;
                case "FLOAT":
                    unit = new SimulateFloatUnit();
                    unit.Value = double.Parse(name.Substring(1));
                    break;
                default:
                    return unit;
            }
            unit.Name = name;
            return unit;
        }
        
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public SimulateModel()
        {
            smanager = new SimulateManager();
            //svmodels = new List<SimuViewBaseModel>();
            //suvars = new SortedSet<SimulateVariableUnit>(new SimulateVariableUintComparer());
            smvars = new List<SimulateVariableUnit>();
            AllRoutine = null;
            MainRoutine = null;
            SubRoutines = new List<SimuViewDiagramModel>();
            AllFuncs = null;
            FuncBlocks = new List<SimuViewFuncBlockModel>();
            SubCharts = new List<SimuViewTabModel>();
            //XYCharts = new List<SimuViewXYModel>();
            LEDBit = new SimulateBitModel();
            LEDBit.Base = "Y0";
            LEDBit.Size = 8;
            smanager.Add(LEDBit);
        }

        /// <summary>
        /// 终止析构函数
        /// </summary>
        public void Dispose()
        {
            // 仿真停止
            smanager.Stop();
            // 释放外部的dll资源
            SimulateDllModel.FreeDll();
        }

        #region Event handler
        /// <summary>
        /// 关闭主窗口时发生（已弃用）
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnMainWindowClosed(object sender, EventArgs e)
        {

            if (PLCTopPhoto.RunTrigger.Status == UI.PLCTop.Trigger.STATUS_RUN)
            {
                smanager.Stop();
            }
            UpdateThread.Abort();
        }
        /// <summary>
        /// 将要打开tab子界面时触发的事件代理
        /// </summary>
        public event ShowTabItemEventHandler TabOpened;
        /// <summary>
        /// 将要打开tab子界面时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnTabOpened(object sender, ShowTabItemEventArgs e)
        {
            if (TabOpened != null)
            {
                TabOpened(sender, e);
            }
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
            if (e.TabName.Equals("图表"))
            {
                ShowItem(MainChart, e.TabName);
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
            foreach (SimuViewTabModel tabmodel in SubCharts)
            {
                if (e.TabName.Equals(tabmodel.Name))
                {
                    ShowItem(tabmodel, e.TabName);
                    return;
                }
            }
        }
        /// <summary>
        /// 当前活动的tab子界面更改时触发的事件代理
        /// </summary>
        public event SelectionChangedEventHandler TabItemChanged;
        /// <summary>
        /// 当前活动的tab子界面更改时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItemChanged != null)
            {
                TabItemChanged(sender, e);
            }
        }
        /// <summary>
        /// 工程树内双击菜单项发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnProjectTreeDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                TreeViewItem tvi = sender as TreeViewItem;
                if (tvi == PTView.TVI_AllRoutine)
                {
                    OnTabOpened(sender, new ShowTabItemEventArgs("所有程序"));
                }
                else
                if (tvi == PTView.TVI_MainRoutine)
                {
                    OnTabOpened(sender, new ShowTabItemEventArgs("主程序"));
                }
                else
                {
                    OnTabOpened(sender, new ShowTabItemEventArgs(tvi.Header.ToString()));
                }
            }
        }
        /// <summary>
        /// PLC仿真控制器停止时发生（已弃用）
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void PLCTopPhotoTriggerStop(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        /// <summary>
        /// PLC仿真控制器运行时发生（已弃用）
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void PLCTopPhotoTriggerRun(object sender, RoutedEventArgs e)
        {
            Start();
        }
        /// <summary>
        /// 监视变量改变时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitChanged(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Replace(e.Old, e.New);
        }
        /// <summary>
        /// 监视变量锁定时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitLocked(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Lock(e.Old);
        }
        /// <summary>
        /// 监视变量解锁时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitUnlocked(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Unlock(e.Old);
        }
        /// <summary>
        /// 变量数据模型锁定时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateDataModelLock(object sender, SimulateDataModelEventArgs e)
        {
            SimulateDataModel sdmodel = e.SDModel_new;
            smanager.Lock(sdmodel);
        }
        /// <summary>
        /// 变量数据模型监视时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateDataModelView(object sender, SimulateDataModelEventArgs e)
        {
            SimulateDataModel sdmodel = e.SDModel_new;
            smanager.View(sdmodel);
        }
        /// <summary>
        /// 变量数据模型解除锁定时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateDataModelUnlock(object sender, SimulateDataModelEventArgs e)
        {
            SimulateDataModel sdmodel = e.SDModel_new;
            smanager.Unlock(sdmodel);
        }
        /// <summary>
        /// 变量数据模型解除监视时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateDataModelUnview(object sender, SimulateDataModelEventArgs e)
        {
            SimulateDataModel sdmodel = e.SDModel_new;
            smanager.Unview(sdmodel);
        }
        /// <summary>
        /// 进行获得数据的一次运行时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateDataModelRun(object sender, SimulateDataModelEventArgs e)
        {
            double timestart = e.TimeStart;
            double timeend = e.TimeEnd;
            smanager.RunData(timestart, timeend);
        }
        /// <summary>
        /// 进行获得图表的一次运行时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateDataModelDraw(object sender, SimulateDataModelEventArgs e)
        {
            double timestart = e.TimeStart;
            double timeend = e.TimeEnd;
            smanager.RunDraw(timestart, timeend);
        }
        /// <summary>
        /// 当仿真dll的RunDraw方法完成时发生，用于绘制图像
        /// </summary>
        public event SimulateDataModelEventHandler RunDrawFinished;
        /// <summary>
        /// 当仿真dll的RunDraw方法完成时发生，用于绘制图像
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnRunDrawFinished(object sender, SimulateDataModelEventArgs e)
        {
            if (RunDrawFinished != null)
            {
                RunDrawFinished(this, e);
            }
        }
        /// <summary>
        /// 当仿真dll的RunData方法完成时发生，用于绘制波形
        /// </summary>
        public event SimulateDataModelEventHandler RunDataFinished;
        /// <summary>
        /// 当仿真dll的RunData方法完成时发生，用于绘制波形
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnRunDataFinished(object sender, SimulateDataModelEventArgs e)
        {
            if (RunDataFinished != null)
            {
                RunDataFinished(this, e);
            }
        }
        /// <summary>
        /// 当创建新的XY坐标图表时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSimuViewXYModelCreate(object sender, SimulateDataModelEventArgs e)
        {
            SimuViewXYModel xychart = new SimuViewXYModel(e.SDModels, (int)(e.TimeStart), (int)(e.TimeEnd));
            xychart.Name = String.Format("坐标图{0:d}", SubCharts.Count());
            SubCharts.Add(xychart);
            TreeViewItem tvitem = new TreeViewItem();
            tvitem.Header = xychart.Name;
            tvitem.MouseDoubleClick += OnProjectTreeDoubleClicked;
            TreeViewItem TVI_Chart = PTView.TVI_Chart;
            TVI_Chart.Items.Add(tvitem);
        }
        #endregion

        #region User Interface
        /// <summary>
        /// 建立与成员的事件连接
        /// </summary>
        private void BuildRouted()
        {
            // 主窗口
            //MainWindow.OpenChart += OnTabOpened;
            //MainWindow.Closed += OnMainWindowClosed;
            // 项目树
            PTView = new ProjectTreeView();
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                PTView.AddTreeViewItem(svdmodel.Name, ProjectTreeView.ADDTVI_TYPE_SUBROUTINES);
            }
            foreach (SimuViewFuncBlockModel svfmodel in FuncBlocks)
            {
                PTView.AddTreeViewItem(svfmodel.Name, ProjectTreeView.ADDTVI_TYPE_FUNCBLOCKS);
            }
            TreeViewItem tvi_arou = PTView.TVI_AllRoutine;
            TreeViewItem tvi_mrou = PTView.TVI_MainRoutine;
            TreeViewItem tvi_srou = PTView.TVI_SubRoutines;
            TreeViewItem tvi_fblo = PTView.TVI_FuncBlocks;
            TreeViewItem tvi_char = PTView.TVI_Chart;
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
            tvi_char.MouseDoubleClick += OnProjectTreeDoubleClicked;
            // 仿真模拟PLC控制器
            PLCTopPhoto = new PLCTopPhoto();
            PLCTopPhoto.RunTrigger.Run += PLCTopPhotoTriggerRun;
            PLCTopPhoto.RunTrigger.Stop += PLCTopPhotoTriggerStop;
            // 监视列表的第一个空表单
            SimulateVInputUnit sviunit = new SimulateVInputUnit();
            smvars.Add(sviunit);
            // 监视列表
            MTable = new MonitorTable();
            MTable.VariableUnitChanged += OnVariableUnitChanged;
            MTable.VariableUnitLocked += OnVariableUnitLocked;
            MTable.VariableUnitUnlocked += OnVariableUnitUnlocked;
            MTable.SVUnits = smvars;
            MTable.Update();
            // 主图表
            MainChart = new SimuViewChartModel();
            MainChart.SDModelView += OnSimulateDataModelView;
            MainChart.SDModelLock += OnSimulateDataModelLock;
            MainChart.SDModelUnlock += OnSimulateDataModelUnlock;
            MainChart.SDModelUnview += OnSimulateDataModelUnview;
            MainChart.SDModelRun += OnSimulateDataModelRun;
            MainChart.SDModelDraw += OnSimulateDataModelDraw;
            MainChart.XYModelCreate += OnSimuViewXYModelCreate;
            MainChart.BuildRouted(this);
            // 仿真管理器
            smanager.RunDataFinished += OnRunDataFinished;
            smanager.RunDrawFinished += OnRunDrawFinished;
        }
        /// <summary>
        /// 建立与元件显示模型的事件连接
        /// </summary>
        /// <param name="svbmodel"></param>
        public void BuildRouted(SimuViewBaseModel svbmodel)
        {
            svbmodel.VariableUnitLocked += OnVariableUnitLocked;
            svbmodel.VariableUnitUnlocked += OnVariableUnitUnlocked;
        }
        /// <summary>
        /// 显示主窗口，主窗口已经弃用，主要用来建立Route
        /// </summary>
        public void ShowWindow()
        {
            //MainWindow = new SimulateWindow();
            BuildRouted();
            //MainWindow.Show();
        }
        /// <summary>
        /// 显示tab子窗口
        /// </summary>
        /// <param name="tabmodel"></param>
        /// <param name="name"></param>
        public void ShowItem(SimuViewTabModel tabmodel, string name)
        {
            //MainWindow.MainTab.ShowItem(tabmodel, name);
        }
        #endregion

        #region Simulate Control
        /// <summary>
        /// PLC打开运行
        /// </summary>
        public void Start()
        {
            smanager.Start();
            //UpdateThread = new Thread(_Update_Thread);
            //UpdateThread.Start();
            //PLCTopPhoto.RunLight.Status = StatusLight.STATUS_LIGHT;
            //PLCTopPhoto.StopLight.Status = StatusLight.STATUS_DARK;
        }
        /// <summary>
        /// PLC终止运行
        /// </summary>
        public void Stop()
        {
            smanager.Stop();
            //PLCTopPhoto.RunLight.Status = StatusLight.STATUS_DARK;
            //PLCTopPhoto.StopLight.Status = StatusLight.STATUS_LIGHT;
        }
        /// <summary>
        /// PLC暂停运行
        /// </summary>
        public void Pause()
        {
            smanager.Pause();
        }
        #endregion

        #region Check
        /// <summary>
        /// 检查工程是否合法
        /// 检查完毕后，仿真的初始化也基本完成了
        /// </summary>
        /// <returns>检查结果</returns>
        public int Check()
        {
            // 错误个数
            int ret = 0;
            // 开始检查线路合法
            report.Dispatcher.Invoke(() => { report.Text = "-----> 开始检查线路合法... <-----\r\n"; });
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
            // 开始生成PLC指令
            report.Dispatcher.Invoke(() => { report.Text += "-----> 开始生成PLC指令... <-----\r\n"; });
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
            // 生成仿真支持文件
            report.Dispatcher.Invoke(() => { report.Text += "-----> 生成仿真支持文件... <-----\r\n"; });
            List<InstHelper.PLCInstNetwork> nets = new List<InstHelper.PLCInstNetwork>();
            ret += MergeAll_Diagram(nets, MainRoutine);
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                ret += MergeAll_Diagram(nets, svdmodel);
            }
            // 建立仿真的c环境的路径
            string currentPath = Environment.CurrentDirectory;
            string ladderHFile = String.Format(@"{0:s}\simug\simuc.h", currentPath);
            string ladderCFile = String.Format(@"{0:s}\simug\simuc.c", currentPath);
            string funcBlockHFile = String.Format(@"{0:s}\simug\simuf.h", currentPath);
            string funcBlockCFile = String.Format(@"{0:s}\simug\simuf.c", currentPath);
            string simulibHFile = String.Format(@"{0:s}\simug\simulib.h", currentPath);
            string simulibCFile = String.Format(@"{0:s}\simug\simulib.c", currentPath);
            string outputDllFile = String.Format(@"{0:s}\simuc.dll", currentPath);
            string outputAFile = String.Format(@"{0:s}\simuc.a", currentPath);
            // 生成梯形图的c语言
            StreamWriter sw = new StreamWriter(ladderCFile);
            InstHelper.InstToSimuCode(sw, nets.ToArray());
            sw.Close();
            // 生成用户函数的c语言头
            sw = new StreamWriter(funcBlockHFile);
            AllFuncs.GenerateCHeader(sw);
            sw.Close();
            // 生成用户函数的c语言
            sw = new StreamWriter(funcBlockCFile);
            AllFuncs.GenerateCCode(sw);
            sw.Close();
            // 生成仿真dll
            SimulateDllModel.CreateDll(ladderCFile, funcBlockCFile, outputDllFile, outputAFile);
            ret = SimulateDllModel.LoadDll(outputDllFile);
            // 各种情况
            report.Dispatcher.Invoke(() =>
            {
                switch (ret)
                {
                    // 没问题
                    case SimulateDllModel.LOADDLL_OK:
                        // 初始化所有的数据点
                        SimulateDllModel.InitDataPoint();
                        break;
                    // 各种错误
                    // 包括找不到dll，以及缺少函数入口
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_DLLFILE:
                        report.Text += "Error : 找不到生成的dll文件simuc.dll\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETBIT:
                        report.Text += "Error : 找不到入口GetBit\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETWORD:
                        report.Text += "Error : 找不到入口GetWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDWORD:
                        report.Text += "Error : 找不到入口GetDWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETFLOAT:
                        report.Text += "Error : 找不到入口GetFloat\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDOUBLE:
                        report.Text += "Error : 找不到入口GetDouble\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETBIT:
                        report.Text += "Error : 找不到入口SetBit\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETWORD:
                        report.Text += "Error : 找不到入口SetWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDWORD:
                        report.Text += "Error : 找不到入口SetDWord\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETFLOAT:
                        report.Text += "Error : 找不到入口SetFloat\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDOUBLE:
                        report.Text += "Error : 找不到入口SetDouble\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDBITDATAPOINT:
                        report.Text += "Error : 找不到入口AddBitDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDWORDDATAPOINT:
                        report.Text += "Error : 找不到入口AddWordDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDDWORDDATAPOINT:
                        report.Text += "Error : 找不到入口AddDWordDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDFLOATDATAPOINT:
                        report.Text += "Error : 找不到入口AddFloatDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDDOUBLEATAPOINT:
                        report.Text += "Error : 找不到入口AddDoubleDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEBITDATAPOINT:
                        report.Text += "Error : 找不到入口RemoveBitDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEWORDDATAPOINT:
                        report.Text += "Error : 找不到入口RemoveWordDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEDWORDDATAPOINT:
                        report.Text += "Error : 找不到入口RemoveDWordDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEFLOATDATAPOINT:
                        report.Text += "Error : 找不到入口RemoveFloatDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEDOUBLEATAPOINT:
                        report.Text += "Error : 找不到入口RemoveDoubleDataPoint\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_BEFORERUNLADDER:
                        report.Text += "Error : 找不到入口BeforeRunLadder\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_AFTERRUNLADDER:
                        report.Text += "Error : 找不到入口AfterRunLadder\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDVIEWINPUT:
                        report.Text += "Error : 找不到入口AddViewInput\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDVIEWOUTPUT:
                        report.Text += "Error : 找不到入口AddViewOutput\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEVIEWINPUT:
                        report.Text += "Error : 找不到入口RemoveViewInput\r\n";
                        break;
                    case SimulateDllModel.LOADDLL_CANNOT_FOUNd_REMOVEVIEWOUTPUT:
                        report.Text += "Error : 找不到入口RemoveViewOutput\r\n";
                        break;
                    default:
                        report.Text += "error : 发生未知错误\r\n";
                        break;
                }
            });
            return ret;
        }
        /// <summary>
        /// 检查程序的连线情况
        /// </summary>
        /// <param name="svdmodel">SimuView程序模型</param>
        /// <returns></returns>
        private int CheckCircuit_Diagram(SimuViewDiagramModel svdmodel)
        {
            int ret = 0;
            // 检查所有网络
            foreach (SimuViewNetworkModel svnmodel in svdmodel.GetNetworks())
            {
                ret += CheckCircuit_Network(svnmodel);
            }
            return ret;
        }
        /// <summary>
        /// 检查网络的连线情况
        /// </summary>
        /// <param name="svnmodel">SimuView网络模型</param>
        /// <returns></returns>
        private int CheckCircuit_Network(SimuViewNetworkModel svnmodel)
        {
            int ret = 0;
            ret += svnmodel.CheckCircuit(report);
            return ret;
        }
        /// <summary>
        /// 生成程序的PLC代码
        /// </summary>
        /// <param name="svdmodel">SimuView程序模型</param>
        /// <returns></returns>
        private int GenPLCInst_Diagram(SimuViewDiagramModel svdmodel)
        {
            int ret = 0;
            // 检查所有网络
            foreach (SimuViewNetworkModel svnmodel in svdmodel.GetNetworks())
            {
                ret += GenPLCInst_Network(svnmodel);
            }
            return ret;
        }

        /// <summary>
        /// 生成网络的PLC代码
        /// </summary>
        /// <param name="svdmodel">SimuView网络模型</param>
        /// <returns></returns>
        private int GenPLCInst_Network(SimuViewNetworkModel svnmodel)
        {
            int ret = 0;
            ret += svnmodel.GenPLCInst(report);
            return ret;
        }
        /// <summary>
        /// 将程序的PLC代码合并到代码表中
        /// </summary>
        /// <param name="nets">PLC代码表</param>
        /// <param name="svdmodel">SimuView程序模型</param>
        /// <returns></returns>
        private int MergeAll_Diagram(List<InstHelper.PLCInstNetwork> nets, SimuViewDiagramModel svdmodel)
        {
            int ret = 0;
            // 检查所有网络
            foreach (SimuViewNetworkModel svnmodel in svdmodel.GetNetworks())
            {
                ret += MergeAll_Network(nets, svnmodel, svdmodel.Name);
            }
            return ret;
        }
        /// <summary>
        /// 将网络的PLC代码合并到代码表中
        /// </summary>
        /// <param name="nets">PLC代码表</param>
        /// <param name="svdmodel">SimuView网络模型</param>
        /// <returns></returns>
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
