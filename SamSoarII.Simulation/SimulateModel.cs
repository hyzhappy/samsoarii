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
using SamSoarII.UserInterface;

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
        /// 仿真管理器
        /// </summary>
        public SimulateManager SManager
        {
            get { return this.smanager; }
        }
        /// <summary>
        /// dll模型
        /// </summary>
        private SimulateDllModel dllmodel;
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
        /// 图表的时间尺
        /// </summary>
        public TimeRuler TRuler;
        /// <summary>
        /// 显示所有子图表的界面的列表
        /// </summary>
        public List<SimuViewTabModel> SubCharts;
        /// <summary>
        /// 仿真的工程资源树界面
        /// </summary>
        public ProjectTreeView PTView;
        /// <summary>
        /// 监视变量的表格的界面
        /// </summary>
        public MonitorTable MTable;
        /// <summary>
        /// 显示图表绘制进度的窗口
        /// </summary>
        private ProgressBarWindow pbwin;
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
            dllmodel = smanager.DllModel;
            smvars = new List<SimulateVariableUnit>();
            AllRoutine = null;
            MainRoutine = null;
            SubRoutines = new List<SimuViewDiagramModel>();
            AllFuncs = null;
            FuncBlocks = new List<SimuViewFuncBlockModel>();
            SubCharts = new List<SimuViewTabModel>();
        }
        /// <summary>
        /// 是否已经释放
        /// </summary>
        private bool isdisposed = false;
        /// <summary>
        /// 是否已经释放
        /// </summary>
        public bool IsDisposed { get { return this.isdisposed; } }
        /// <summary>
        /// 终止析构函数
        /// </summary>
        public void Dispose()
        {
            isdisposed = true;
            // 仿真停止
            if (dllmodel.SimulateStatus != SimulateDllModel.SIMULATE_STOP)
            {
                smanager.Stop();
                smanager.Dispose();
            }
            else
            {
                OnSimulateAbort(this, new RoutedEventArgs());
            }
        }

        #region Event handler

        #region PointStart & PointEnd

        public bool PointStartEnable { get; private set; }

        public bool PointEndEnable { get; private set; }

        private void OnPointStartEnable(object sender, RoutedEventArgs e)
        {
            PointStartEnable = true;
            dllmodel.TimeStart = (int)TRuler.PointStart;
            if (PointStartEnable && PointEndEnable)
            {
                dllmodel.SimuMode = SimulateDllModel.SIMUMODE_CHART;
            }
        }
        
        private void OnPointEndEnable(object sender, RoutedEventArgs e)
        {
            PointEndEnable = true;
            dllmodel.TimeEnd = (int)TRuler.PointEnd;
            if (PointStartEnable && PointEndEnable)
            {
                dllmodel.SimuMode = SimulateDllModel.SIMUMODE_CHART;
            }
        }

        private void OnPointStartDisable(object sender, RoutedEventArgs e)
        {
            PointStartEnable = false;
            dllmodel.SimuMode = SimulateDllModel.SIMUMODE_NORMAL;
        }

        private void OnPointEndDisable(object sender, RoutedEventArgs e)
        {
            PointEndEnable = false;
            dllmodel.SimuMode = SimulateDllModel.SIMUMODE_NORMAL;
        }

        #endregion

        #region Simulate Control

        public event RoutedEventHandler SimulateStart = delegate { };

        public event RoutedEventHandler SimulatePause = delegate { };

        public event RoutedEventHandler SimulateAbort = delegate { };

        private void OnSimulateStart(object sender, RoutedEventArgs e)
        {
            SimulateStart(this, e);
            if (dllmodel.SimuMode == SimulateDllModel.SIMUMODE_CHART)
            {
                pbwin = new ProgressBarWindow("当前进度", "正在选定时间范围内运行，请稍后。。。");
                pbwin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                pbwin.ShowDialog();
            }
        }

        private void OnSimulatePause(object sender, RoutedEventArgs e)
        {
            SimulatePause(this, e);
        }

        private void OnSimulateAbort(object sender, RoutedEventArgs e)
        {
            SimulateAbort(this, e);
            if (isdisposed)
            {
                dllmodel.SimulateAbort -= OnSimulateAbort;
                dllmodel.SimulateStart -= OnSimulateStart;
                dllmodel.SimulatePause -= OnSimulatePause;
                dllmodel.SimulateProgress -= OnSimulateProgress;
                dllmodel = null;
                SimulateDllModel.FreeDll();
                return;
            }
            if (dllmodel.SimuMode == SimulateDllModel.SIMUMODE_CHART)
            {
                if (pbwin != null)
                {
                    pbwin.Dispatcher.Invoke(new Utility.Delegates.Execute(
                        () => { pbwin.Close(); }));
                    pbwin = null;
                }
                MainChart.Dispatcher.Invoke(new Utility.Delegates.Execute(
                    MainChart.Update));
            }
        }

        private void OnSimulateProgress(object sender, RoutedEventArgs e)
        {
            if (sender is double && pbwin != null)
            {
                double pvalue = (double)sender;
                pbwin.Dispatcher.Invoke(new Utility.Delegates.Execute(
                    () => { pbwin.Value = pvalue; }));
            }
        }
        
        #endregion

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
        /// 监视变量单次修改时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitValueChanged(object sender, VariableUnitChangeEventArgs e)
        {
            smanager.Change(e.Old);
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
            // 项目树
            PTView = new ProjectTreeView();
            foreach (SimuViewDiagramModel svdmodel in SubRoutines)
            {
                PTView.AddTreeViewItem(svdmodel.Name, ProjectTreeView.ADDTVI_TYPE_SUBROUTINES);
            }
            PTView.AddTreeViewItem("所有函数", ProjectTreeView.ADDTVI_TYPE_FUNCBLOCKS);
            foreach (SimuViewFuncBlockModel svfmodel in FuncBlocks)
            {
                PTView.AddTreeViewItem(svfmodel.Name, ProjectTreeView.ADDTVI_TYPE_FUNCBLOCKS);
            }
            TreeViewItem tvi_mrou = PTView.TVI_MainRoutine;
            TreeViewItem tvi_srou = PTView.TVI_SubRoutines;
            TreeViewItem tvi_fblo = PTView.TVI_FuncBlocks;
            TreeViewItem tvi_char = PTView.TVI_Chart;
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
            // 时间尺
            TRuler = MainChart.TRuler;
            TRuler.PointStartEnable += OnPointStartEnable;
            TRuler.PointStartDisable += OnPointStartDisable;
            TRuler.PointEndEnable += OnPointEndEnable;
            TRuler.PointEndDisable += OnPointEndDisable;
            // 仿真管理器
            //smanager.RunDataFinished += OnRunDataFinished;
            //smanager.RunDrawFinished += OnRunDrawFinished;
            // DLL模型
            dllmodel.SimulateStart += OnSimulateStart;
            dllmodel.SimulatePause += OnSimulatePause;
            dllmodel.SimulateAbort += OnSimulateAbort;
            dllmodel.SimulateProgress += OnSimulateProgress;
        }
        /// <summary>
        /// 建立与元件显示模型的事件连接
        /// </summary>
        /// <param name="svbmodel"></param>
        public void BuildRouted(SimuViewBaseModel svbmodel)
        {
            svbmodel.VariableUnitLocked += OnVariableUnitLocked;
            svbmodel.VariableUnitUnlocked += OnVariableUnitUnlocked;
            svbmodel.VariableUnitValueChanged += OnVariableUnitValueChanged;
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
        
    }
}
