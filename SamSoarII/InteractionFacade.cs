using Microsoft.Win32;
using SamSoarII.Global;
using SamSoarII.Core.Models;
using SamSoarII.Core.Generate;
using SamSoarII.Core.Simulate;
using SamSoarII.Core.Helpers;
using SamSoarII.Core.Communication;
using SamSoarII.Core.Update;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Managers;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Shell.Windows.Update;
using SamSoarII.Threads;
using SamSoarII.HelpDocument;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Specialized;
using SamSoarII.Utility.DXF;

namespace SamSoarII
{
    public class InteractionFacade : IDisposable, IWindow,INotifyPropertyChanged
    {
        #region IWindow

        InteractionFacade IWindow.IFParent { get { return this; } }

        event IWindowEventHandler IWindow.Post
        {
            add { PostIWindowEvent += value; }
            remove { PostIWindowEvent -= value; }
        }

        #endregion

        public InteractionFacade(MainWindow _wndMain)
        {
            wndMain = _wndMain;
            mngValue = new ValueManager(this);
            vmngValue = new ValueViewManager(mngValue);
            mngSimu = new SimulateManager(this);
            mngComu = new CommunicationManager(this);
            barStatus = new UnderBar(this);
            tvProj = new ProjectTreeView(this);
            tcMain = new MainTabControl(this);
            wndError = new ErrorReportWindow(this);
            wndFind = new FindWindow(this);
            wndReplace = new ReplaceWindow(this);
            wndTFind = new TextFindWindow(this);
            wndTReplace = new TextReplaceWindow(this);
            wndEList = new ElementListWindow(this);
            wndEInit = new ElementInitWindow(this);
            wndMoni = new MonitorWindow(this);
            wndBrpo = new BreakpointWindow(this);
            wndVBrpo = new ValueBrpoWindow(this);
            udManager = new UpdateManager(this);
            wndInform = new UpdateWindow(this);
            thmngCore = new CoreThreadManager(this);
            thmngView = new ViewThreadManager(this);
            mngSimu.Started += OnSimulateStarted;
            mngSimu.Paused += OnSimulatePaused;
            mngSimu.Aborted += OnSimulateAborted;
            mngSimu.BreakpointPaused += OnBreakpointPaused;
            mngSimu.BreakpointResumed += OnBreakpointResumed;
            barStatus.Post += OnReceiveIWindowEvent;
            tvProj.Post += OnReceiveIWindowEvent;
            tcMain.Post += OnReceiveIWindowEvent;
            wndError.Post += OnReceiveIWindowEvent;
            wndFind.Post += OnReceiveIWindowEvent;
            wndReplace.Post += OnReceiveIWindowEvent;
            wndTFind.Post += OnReceiveIWindowEvent;
            wndTReplace.Post += OnReceiveIWindowEvent;
            wndEList.Post += OnReceiveIWindowEvent;
            wndEInit.Post += OnReceiveIWindowEvent;
            wndMoni.Post += OnReceiveIWindowEvent;
            wndBrpo.Post += OnReceiveIWindowEvent;
            wndVBrpo.Post += OnReceiveIWindowEvent;
            wndInform.Post += OnReceiveIWindowEvent;
        }
        
        public void Dispose()
        {
            //if (mdProj != null) CloseProject();
            WaitForThreadAbort();
            PostIWindowEvent = delegate { };
            tvProj.Post -= OnReceiveIWindowEvent;
            tcMain.Post -= OnReceiveIWindowEvent;
            wndError.Post -= OnReceiveIWindowEvent;
            wndFind.Post -= OnReceiveIWindowEvent;
            wndReplace.Post -= OnReceiveIWindowEvent;
            wndTFind.Post -= OnReceiveIWindowEvent;
            wndTReplace.Post -= OnReceiveIWindowEvent;
            wndEList.Post -= OnReceiveIWindowEvent;
            wndEInit.Post -= OnReceiveIWindowEvent;
            wndMoni.Post -= OnReceiveIWindowEvent;
            wndBrpo.Post -= OnReceiveIWindowEvent;
            wndVBrpo.Post -= OnReceiveIWindowEvent;
            wndInform.Post -= OnReceiveIWindowEvent;
            tvProj = null;
            tcMain = null;
            wndError = null;
            wndFind = null;
            wndReplace = null;
            wndTFind = null;
            wndTReplace = null;
            wndMain = null;
            wndEList = null;
            wndEInit = null;
            wndMoni = null;
            wndBrpo = null;
            wndVBrpo = null;
            wndInform = null;
            udManager = null;
        }

        #region Numbers

        #region UI

        private MainWindow wndMain;
        public MainWindow WNDMain { get { return this.wndMain; } }

        private UnderBar barStatus;
        public UnderBar BARStatus { get { return this.barStatus; } }
 
        private ProjectTreeView tvProj;
        public ProjectTreeView TVProj { get { return this.tvProj; } }

        private MainTabControl tcMain;
        public MainTabControl TCMain { get { return this.tcMain; } }
        public BaseTabItem CurrentTabItem { get { return (BaseTabItem)(tcMain.SelectedItem); } }
        public LadderDiagramViewModel CurrentLadder { get { return tcMain.SelectedItem is MainTabDiagramItem
            ? ((MainTabDiagramItem)(tcMain.SelectedItem)).LDVModel : null; } }
        public FuncBlockViewModel CurrentFuncBlock { get { return tcMain.SelectedItem is FuncBlockViewModel
            ? (FuncBlockViewModel)(tcMain.SelectedItem) : null; } }

        private ErrorReportWindow wndError;
        public ErrorReportWindow WNDError { get { return this.wndError; } }

        private FindWindow wndFind;
        public FindWindow WNDFind { get { return this.wndFind; } }

        private ReplaceWindow wndReplace;
        public ReplaceWindow WNDReplace { get { return this.wndReplace; } }

        private TextFindWindow wndTFind;
        public TextFindWindow WNDTFind { get { return this.wndTFind; } }

        private TextReplaceWindow wndTReplace;
        public TextReplaceWindow WNDTReplace { get { return this.wndTReplace; } }

        private ElementListWindow wndEList;
        public ElementListWindow WNDEList { get { return this.wndEList; } }

        private ElementInitWindow wndEInit;
        public ElementInitWindow WNDEInit { get { return this.wndEInit; } }

        private MonitorWindow wndMoni;
        public MonitorWindow WNDMoni { get { return this.wndMoni; } }

        private BreakpointWindow wndBrpo;
        public BreakpointWindow WNDBrpo { get { return this.wndBrpo; } }

        private ValueBrpoWindow wndVBrpo;
        public ValueBrpoWindow WNDVBrpo { get { return this.wndVBrpo; } }

        private UpdateWindow wndInform;
        public UpdateWindow WNDInform { get { return wndInform; } }

        #endregion

        #region Project
        public PLCDevice.Device Device { get { return mdProj?.Device; } }
        public string ProjectName { get { return mdProj?.ProjName; } }
        private ProjectModel mdProj;
        public ProjectModel MDProj { get { return this.mdProj; } }
        
        private ProjectViewModel vmdProj;
        public ProjectViewModel VMDProj { get { return this.vmdProj; } }

        private SimulateManager mngSimu;
        public SimulateManager MNGSimu { get { return this.mngSimu; } }

        private CommunicationManager mngComu;
        public CommunicationManager MNGComu { get { return this.mngComu; } }
        
        #endregion

        #region Values

        private ValueManager mngValue;
        public ValueManager MNGValue { get { return this.mngValue; } }

        private ValueViewManager vmngValue;
        public ValueViewManager VMNGValue { get { return this.vmngValue; } }

        #endregion

        #region Threads

        private CoreThreadManager thmngCore;
        public CoreThreadManager ThMNGCore { get { return this.thmngCore; } }

        private ViewThreadManager thmngView;
        public ViewThreadManager ThMNGView { get { return this.thmngView; } }
        
        private void WaitForThreadAbort()
        {
            thmngCore.Abort();
            thmngView.Abort();
            while (thmngCore.IsAlive || thmngView.IsAlive) Thread.Sleep(10);
        }

        private void AllThreadStart()
        {
            thmngCore.Start();
            thmngView.Start();
        }

        #region Tools
        private UpdateManager udManager;
        public UpdateManager UDManager
        {
            get { return udManager; }
        }

        #endregion

        #endregion

        #endregion

        #region Project

        private void InitializeProject()
        {
            mdProj.PropertyChanged += OnProjectPropertyChanged;
            mdProj.ViewPropertyChanged += OnProjectPropertyChanged;
            mdProj.Modified += OnProjectModified;
            vmdProj = new ProjectViewModel(mdProj);
            vmdProj.PropertyChanged += OnViewPropertyChanged;
            AllThreadStart();
            tcMain.ViewMode = MainTabControl.VIEWMODE_LADDER;
            tcMain.ShowItem(mdProj.MainDiagram.Tab);
            tvProj.ReinitializeComponent();
            barStatus.Project = mdProj;
            wndEInit.UpdateElements();
            wndMoni.Core = mdProj.Monitor;
            wndVBrpo.Core = mdProj.ValueBrpo;
            wndMain.LACProj.Show();
            mdProj.IsLoaded = true;
            PropertyChanged(this, new PropertyChangedEventArgs("ProjectName"));
            PropertyChanged(this, new PropertyChangedEventArgs("Device"));
        }
        
        public void CreateProject(string name, string filename = null)
        {
            CloseProject();
            _CreateProject(name, filename);
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Normal, Properties.Resources.Ready));
        }

        private void _CreateProject(string name, string filename)
        {
            mdProj = new ProjectModel(this, name);
            InitializeProject();
            if (filename != null) SaveAsProject(filename);
        }
        
        public void LoadProject(string filename,bool _isUpload = false)
        {
            CloseProject();
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Loading, Properties.Resources.Project_Preparing));
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Project_Load);
            handle.Start();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                _LoadProject(filename, handle,_isUpload);
                handle.Completed = true;
                handle.Abort();
            });
            while (!handle.Completed) Thread.Sleep(10);
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Normal, Properties.Resources.Ready));
        }

        private void _LoadProject(string filename, LoadingWindowHandle handle, bool _isUpload = false)
        {
#if RELEASE
            try
            {
#endif
                mdProj = new ProjectModel(this, FileHelper.GetFileName(filename), filename);
                if (_isUpload)
                    mdProj.ClearFileName();
                else ProjectFileManager.Update(filename, filename);
                InitializeProject();
#if RELEASE
            }
            catch (Exception e)
            {
               handle.Completed = true;
               handle.Abort();
               mdProj = null;
               LocalizedMessageBox.Show(Properties.Resources.Message_Project_Error, LocalizedMessageIcon.Information);
            }
#endif
        }

        public void SaveProject()
        {
            if (FileHelper.InvalidFileName(mdProj.FileName))
                ShowSaveProjectDialog();
            else
                mdProj.Save();
        }

        public void SaveAsProject(string filename, bool isexception = false)
        {
            if (isexception)
            {
                try
                {
                    vmdProj.Dispose();
                    if (mdProj.RedoDiagram != null && mdProj.RedoDiagram.CanUndo)
                        mdProj.RedoDiagram.Undo();
                    if (mdProj.UndoDiagram != null && mdProj.UndoDiagram.CanRedo)
                        mdProj.UndoDiagram.Redo();
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (mdProj.FileName != string.Empty) mdProj.Save();
                    mdProj.Save(filename);
                }
                return;
            }
            mdProj.Save(filename);
            ProjectFileManager.Update(filename, filename);
        }
        
        public void CloseProject()
        {
            if (vmdProj != null)
            {
                EditProject();
                LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.MainWindow_Close_Proj);
                handle.Start();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    _CloseProject();
                    handle.Completed = true;
                    handle.Abort();
                });
                while (!handle.Completed)
                {
                    Dispatcher.Run();
                    Thread.Sleep(10);
                }
            }
        }

        private void _CloseProject()
        {
            if (mdProj == null) return;
            WaitForThreadAbort();
            mdProj.PropertyChanged -= OnProjectPropertyChanged;
            mdProj.ViewPropertyChanged -= OnProjectPropertyChanged;
            mdProj.Modified -= OnProjectModified;
            wndMain.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { wndMain.HideAllDock(); });
            tcMain.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { tcMain.Reset(); });
            tvProj.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { tvProj.Reset(); });
            barStatus.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { barStatus.Reset(); });
            wndMoni.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { wndMoni.Core = null; });
            wndVBrpo.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { wndVBrpo.Core = null; });
            vmdProj.PropertyChanged -= OnViewPropertyChanged;
            vmdProj.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { vmdProj.Dispose(); });
            mdProj.Dispose();
            vmdProj = null;
            mdProj = null;
            mngValue.Initialize();
            GC.Collect(2);
        }
        /// <summary>
        /// 记录每次下载后的选项，判断是否需重新生成Bin,初始化为-1表示第一次打开对话框
        /// </summary>
        /// <returns></returns>
        private int _option = -1;
        public bool DownloadProject()
        {
            if (vmdProj.LadderMode == LadderModes.Simulate) _CloseSimulate();
            if (vmdProj.LadderMode == LadderModes.Monitor) _CloseMonitor();
            if (!CheckLadder(false)) return false;
            if (!CheckFuncBlock(false)) return false;
#if DEBUG
            //GenerateHelper.GenerateFinal(mdProj);
            //DownloadHelper.InitializeData(mdProj);
#endif
            _option = -1;
            mngComu.IsEnable = true;
            CommunicationParams paraCom = mdProj.PARAProj.PARACom;
            using (CommunicationSettingDialog dialog = new CommunicationSettingDialog(paraCom,
                CommunicationSettingDialogMode.DOWNLOAD))
            {
                dialog.Owner = wndMain;
                ComBaseSetting basesetting = dialog.GetBaseSetting();
                //basesetting.DataLen = mngComu.ExecLen;
                basesetting.SettingButtonClick += (sender1, e1) =>
                {
                    using (CommunicationParamsDialog dialog1 = new CommunicationParamsDialog(paraCom))
                    {
                        dialog1.ShowDialog();
                    }
                };
                dialog.CommunicationTest += (sender2, e2) =>
                {
                    if (mngComu.CheckLink())
                        LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Success, LocalizedMessageIcon.Information);
                    else if (mngComu.MNGCurrent.Ex != null && mngComu.MNGCurrent.Ex is UnauthorizedAccessException)
                    {
                        LocalizedMessageBox.Show(Properties.Resources.SerialPort_Occupied, LocalizedMessageIcon.Information);
                    }else LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                };
                basesetting.ModifyButtonClick += (sender2, e2) =>
                {
                    using (ProjectPropertyDialog dialog2 = new ProjectPropertyDialog(mdProj))
                    {
                        dialog2.EnsureButtonClick += (sender1, e1) =>
                        {
                            bool ret = true;
                            if(mdProj.LadderMode != LadderModes.Edit)
                            {
                                LocalizedMessageBox.Show(Properties.Resources.Modify_Project, LocalizedMessageIcon.Warning);
                            }
                            else
                            {
                                ret = dialog2.Save();
                            }
                            if(ret)
                                dialog2.Close();
                        };
                        dialog2.ShowDialog();
                    }
                };
                dialog.Ensure += (sender3, e3) =>
                {
                    if (DownloadHelper.DownloadOption == 0)
                    {
                        LocalizedMessageBox.Show(Properties.Resources.Please_Select_DownData, LocalizedMessageIcon.Information);
                        return;
                    }

                    if (mngComu.CheckLink())
                    {
                        if (!mngComu.PasswordHandle(CommunicationType.Download))
                        {
                            LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                            return;
                        }

                        LoadingWindowHandle handle;

                        if (DownloadHelper.IsDownloadProgram && (_option < 0 || !DownloadHelper.CheckOption(_option, DownloadHelper.DownloadOption)))
                        {
                            //按下下载键时再生成Bin,判断是否要包括软元件初始化
                            handle = new LoadingWindowHandle(Properties.Resources.Generating_Final);
                            handle.Start();
                            Thread genthread = new Thread(() =>
                            {
                                mngComu.LoadExecute(GenerateHelper.GenerateFinal(mdProj));
                                handle.Completed = true;
                                handle.Abort();
                            });
                            genthread.Start();
                            while (!handle.Completed) Thread.Sleep(10);
                        }

                        _option = DownloadHelper.DownloadOption;

                        ProgressBarHandle phandle = new ProgressBarHandle(Properties.Resources.Project_Download, 0, 100,  mngComu.DownloadExecute, new BackgroundWorker(), vmdProj.Dispatcher);
                        phandle.StartWork();
                    }
                    else if (mngComu.MNGCurrent.Ex != null && mngComu.MNGCurrent.Ex is UnauthorizedAccessException)
                    {
                        LocalizedMessageBox.Show(Properties.Resources.SerialPort_Occupied, LocalizedMessageIcon.Information);
                    }
                    else LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                };
                dialog.ShowDialog();
            }
            mngComu.IsEnable = false;
            return true;
        }
        
        public bool UploadProject()
        {
            if (UploadHelper.HasConfig)
            {
                if (mdProj != null && mdProj.IsLoaded)
                {
                    var ret = LocalizedMessageBox.Show(Properties.Resources.Config_Applied, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information);
                    if (ret == LocalizedMessageResult.Yes)
                        mdProj.PARAProj.Load(UploadHelper.ProjectParams);
                }
                else if (mdProj == null)
                {
                    var ret = LocalizedMessageBox.Show(Properties.Resources.Config_Applied_NewProj, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information);
                    if (ret == LocalizedMessageResult.Yes)
                    {
                        CreateProject("New Project");
                        mdProj.PARAProj.Load(UploadHelper.ProjectParams);
                    }
                }
            }
            if (!UploadHelper.HasConfig || LocalizedMessageBox.Show(Properties.Resources.Continue_Upload, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information) == LocalizedMessageResult.Yes)
            {
                if (mdProj != null)
                {
                    if (vmdProj.LadderMode == LadderModes.Simulate) _CloseSimulate();
                    if (vmdProj.LadderMode == LadderModes.Monitor) _CloseMonitor();
                }
                mngComu.IsEnable = true;
                CommunicationParams paraCom = mngComu.PARACom;
                using (CommunicationSettingDialog dialog = new CommunicationSettingDialog(paraCom,
                    CommunicationSettingDialogMode.UPLOAD))
                {
                    dialog.Owner = wndMain;
                    ComBaseSetting basesetting = dialog.GetBaseSetting();
                    basesetting.SettingButtonClick += (sender1, e1) =>
                    {
                        using (CommunicationParamsDialog dialog1 = new CommunicationParamsDialog(paraCom))
                        {
                            dialog1.ShowDialog();
                        }
                    };
                    dialog.CommunicationTest += (sender2, e2) =>
                    {
                        if (mngComu.CheckLink())
                            LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Success, LocalizedMessageIcon.Information);
                        else if (mngComu.MNGCurrent.Ex != null && mngComu.MNGCurrent.Ex is UnauthorizedAccessException)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.SerialPort_Occupied, LocalizedMessageIcon.Information);
                        }
                        else LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                    };
                    dialog.Ensure += (sender3, e3) =>
                    {
                        if (UploadHelper.UploadOption == 0)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Please_Select_UpData, LocalizedMessageIcon.Information);
                            return;
                        }
                        if (mngComu.CheckLink())
                        {
                            if (!mngComu.PasswordHandle(CommunicationType.Upload))
                            {
                                LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                                return;
                            }
                            
                            LocalizedMessageResult ret = LocalizedMessageResult.Yes;
                            if (UploadHelper.HasConfig && UploadHelper.IsUploadSetting)
                                ret = LocalizedMessageBox.Show(Properties.Resources.Config_Override, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information);
                            if (ret == LocalizedMessageResult.Yes)
                            {
                                ProgressBarHandle handle = new ProgressBarHandle(Properties.Resources.Project_Upload, 0,100, mngComu.UploadExecute ,new BackgroundWorker(), Application.Current.Dispatcher);
                                handle.StartWork();
                            }
                        }
                        else if (mngComu.MNGCurrent.Ex != null && mngComu.MNGCurrent.Ex is UnauthorizedAccessException)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.SerialPort_Occupied, LocalizedMessageIcon.Information);
                        }
                        else LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                    };
                    dialog.ShowDialog();
                }
                mngComu.IsEnable = false;
            }
            return true;
        }

        public bool SimulateProject()
        {
            if (vmdProj.LadderMode == LadderModes.Monitor)
                _CloseMonitor();
            if (vmdProj.LadderMode == LadderModes.Simulate)
            {
                _CloseSimulate();
                return false;
            }
            if (!CheckLadder(false)) return false;
            if (!CheckFuncBlock(false)) return false;
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, 
                UnderBarStatus.Loading, Properties.Resources.Simulate_Initing));
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Simulate_Initing);
            handle.Start();
            int ret = 0;
            vmdProj.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                mngSimu.MNGBrpo.Initialize();
                ret = GenerateHelper.GenerateSimu(mdProj);
                handle.Completed = true;
                handle.Abort();
            });
            while (!handle.Completed) Thread.Sleep(10);
            switch (ret)
            {
                case SimulateDllModel.LOADDLL_OK:
                    vmdProj.LadderMode = LadderModes.Simulate;
                    mngSimu.IsEnable = true;
                    return true;
                default:
                    PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                        MainWindowEventArgs.TYPE_TOGGLE_UP
                      | MainWindowEventArgs.FLAG_SIMULATE));
                    PostIWindowEvent(this, new UnderBarEventArgs(barStatus,
                        UnderBarStatus.Error, Properties.Resources.Simulate_Error));
                    LocalizedMessageBox.Show(Properties.Resources.Simulate_Error, LocalizedMessageIcon.Error);
                    return false;
            }
        }

        public bool MonitorProject()
        {
            if (vmdProj.LadderMode == LadderModes.Simulate)
                _CloseSimulate();
            if (vmdProj.LadderMode == LadderModes.Monitor)
            {
                _CloseMonitor();
                return false;
            }
            if (!CheckLadder(false)) return false;
            //if (!CheckFuncBlock(false)) return false;
            
            if (!mngComu.CheckLink())
            {
                PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                    MainWindowEventArgs.TYPE_TOGGLE_UP
                  | MainWindowEventArgs.FLAG_MONITOR));
                PostIWindowEvent(this, new UnderBarEventArgs(barStatus,
                    UnderBarStatus.Error, Properties.Resources.MessageBox_Communication_Failed));
                if (mngComu.MNGCurrent.Ex != null && mngComu.MNGCurrent.Ex is UnauthorizedAccessException)
                {
                    LocalizedMessageBox.Show(Properties.Resources.SerialPort_Occupied, LocalizedMessageIcon.Information);
                }
                else LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                mngComu.IsEnable = false;
                return false;
            }

            if (!mngComu.PasswordHandle(CommunicationType.Monitor))
            {
                LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                return false;
            }
            
            mngComu.IsEnable = true;
            vmdProj.LadderMode = LadderModes.Monitor;
            return true;
        }
        
        public void EditProject()
        {
            if (vmdProj == null) return;
            switch (vmdProj.LadderMode)
            {
                case LadderModes.Simulate:
                    _CloseSimulate();
                    break;
                case LadderModes.Monitor:
                    _CloseMonitor();
                    break;
            }
        }
        public bool HandleCurrentProj()
        {
            if (mdProj != null && (mdProj.IsModified == true || FileHelper.InvalidFileName(mdProj.FileName)))
            {
                LocalizedMessageResult mbret;
                if (mdProj.FileName == string.Empty)
                    mbret = ShowSaveYesNoCancelDialog(Properties.Resources.Project_UnSaved);
                else
                    mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case LocalizedMessageResult.Yes:
                        SaveProject();
                        break;
                    case LocalizedMessageResult.No:
                        break;
                    case LocalizedMessageResult.Cancel:
                    default:
                        return false;
                }
            }
            return true;
        }

        private void _CloseSimulate()
        {
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                UnderBarStatus.Loading, Properties.Resources.Simulate_Closing));
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Simulate_Closing);
            handle.Start();
            vmdProj.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                mngSimu.Abort();
                mngSimu.IsEnable = false;
                while (mngSimu.IsAlive) Thread.Sleep(10);
                mngSimu.MNGBrpo.Initialize();
                SimulateDllModel.FreeDll();
                vmdProj.LadderMode = LadderModes.Edit;
                handle.Completed = true;
                handle.Abort();
            });
            while (!handle.Completed) Thread.Sleep(10);
        }

        private void _CloseMonitor()
        {
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                UnderBarStatus.Loading, Properties.Resources.Monitor_Closing));
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Monitor_Closing);
            handle.Start();
            vmdProj.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                mngComu.AbortAll();
                mngComu.IsEnable = false;
                while (mngComu.IsAlive) Thread.Sleep(10);
                vmdProj.LadderMode = LadderModes.Edit;
                handle.Completed = true;
                handle.Abort();
            });
            while (!handle.Completed) Thread.Sleep(10);
        }
        
        #endregion

        #region Check

        public bool CheckLadder(bool showreport = false)
        {
            bool result = false;
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Loading, Properties.Resources.LadderDiagram_check));
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.LadderDiagram_check);
            wndMain.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                handle.Start();
                thmngCore.MNGInst.Pause();
                while (thmngCore.MNGInst.IsActive)
                    Thread.Sleep(10);
                result = _CheckLadder(handle, showreport);
                thmngCore.MNGInst.Start();
                handle.Completed = true;
                handle.Abort();
            });
            while (!handle.Completed) Thread.Sleep(10);
            if (!showreport)
            {
                if (result)
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Normal, Properties.Resources.Ladder_Correct));
                else
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Error, Properties.Resources.Ladder_Error));
            }
            return result;
        }
        
        private bool _CheckLadder(LoadingWindowHandle handle, bool showreport)
        {
            bool result = false;
            foreach (ModbusModel modbus in mdProj.Modbus.Children)
                if (!modbus.IsValid)
                {
                    ShowMessage(
                        String.Format("{0:s}{1:s}",
                            modbus.Name, Properties.Resources.Message_Modbus_Table_Error)
                        , handle, true, true);
                    return false;
                }
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            foreach (LadderDiagramModel diagram in mdProj.Diagrams)
            {
                errorMessages.Add(LadderGraphCheckModule.Execute(diagram));
                if (diagram.View != null)
                    diagram.View.IsViewModified = true;
            }
            for (int i = 0; i < errorMessages.Count; i++)
            {
                if (errorMessages[i].Error == ErrorType.None
                 || errorMessages[i].Error == ErrorType.InstPair)
                {
                    IList<ErrorReportElement> weinsts = null;
                    int ecount = 0;
                    int wcount = 0;
                    weinsts = _CheckInsts(ref ecount, ref wcount);
                    if (weinsts.Count() > 0)
                    {
                        result = (ecount == 0);
                        if ((showreport || !result) && i == errorMessages.Count - 1)
                        {
                            if (App.CultureIsZH_CH())
                                ShowMessage(string.Format("程序存在{0:d}处错误，{1:d}处警告。",
                                        ecount, wcount), handle, true, true);
                            else
                            {
                                ShowMessage(string.Format("There are {0} errors and {1} warnings in the program.",
                                        ecount, wcount), handle, true, true);
                            }
                        }
                    }
                    else
                    {
                        if (i == errorMessages.Count - 1)
                        {
                            if (showreport)
                                ShowMessage(Properties.Resources.Program_Correct, handle, false, true);
                            else
                                handle?.Abort();
                            result = true;
                        }
                    }
                    if (i == errorMessages.Count - 1)
                    {
                        wndError.Mode = ErrorReportWindow.MODE_LADDER;
                        wndError.Update(weinsts);
                        if (!result) wndMain.LACErrorList.Show();
                    }
                    //else
                    //    _projectModel.IsModify = false;
                }
                else if (errorMessages[i].Error == ErrorType.Empty)
                {
                    LadderNetworkModel network = errorMessages[i].RefNetworks.First();
                    int num = network.ID;
                    string name = network.Parent.Name;
                    Navigate(network);
                    if (App.CultureIsZH_CH())
                        ShowMessage(string.Format("程序{0}的网络{1}元素为空!", name, num), handle, true, true);
                    else
                        ShowMessage(string.Format("Network {0} in {1} is empty!", num, name), handle, true, true);
                    result = false;
                    break;
                }
                else
                {
                    Navigate(errorMessages[i].RefNetworks.First(), errorMessages[i].RefNetworks.Last().ErrorModels.First().X, errorMessages[i].RefNetworks.Last().ErrorModels.First().Y);
                    result = false;
                    switch (errorMessages[i].Error)
                    {
                        case ErrorType.Open:
                            ShowMessage(Properties.Resources.Open_Error, handle, true, true);
                            break;
                        case ErrorType.Short:
                            ShowMessage(Properties.Resources.Short_Error, handle, true, true);
                            break;
                        case ErrorType.SelfLoop:
                            ShowMessage(Properties.Resources.Selfloop_Error, handle, true, true);
                            break;
                        case ErrorType.HybridLink:
                            ShowMessage(Properties.Resources.HybridLink_Error, handle, true, true);
                            break;
                        case ErrorType.Special:
                            ShowMessage(Properties.Resources.Special_Instruction_Error, handle, true, true);
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }
            return result;
        }

        private IList<ErrorReportElement> _CheckInsts(ref int ecount, ref int wcount)
        {
            foreach (LadderDiagramModel ldmodel in mdProj.Diagrams)
            {
                ldmodel.IsInterruptLadder = false;
                foreach (InstructionNetworkModel inmodel in ldmodel.Inst.Children) 
                    foreach (PLCOriginInst inst in inmodel.Insts)
                    {
                        inst.Status = PLCOriginInst.STATUS_ACCEPT;
                        inst.Message = String.Empty;
                    } 
            }
            mngValue.Check();
            foreach (LadderDiagramModel ldmodel in mdProj.Diagrams) 
                ldmodel.Inst.Check(); 
            foreach (LadderDiagramModel ldmodel in mdProj.Diagrams) 
                ldmodel.Inst.CheckForInterrrupt(); 
            ecount = 0;
            wcount = 0;
            List<ErrorReportElement> weinsts = new List<ErrorReportElement>();
            foreach (LadderDiagramModel ldmodel in mdProj.Diagrams)
                foreach (InstructionNetworkModel inmodel in ldmodel.Inst.Children)
                    foreach (PLCOriginInst inst in inmodel.Insts) 
                        if (inst.Status == PLCOriginInst.STATUS_ERROR)
                        {
                            ecount++;
                            weinsts.Add(new ErrorReportElement(inst));
                        } 
            foreach (LadderDiagramModel ldmodel in mdProj.Diagrams)
                foreach (InstructionNetworkModel inmodel in ldmodel.Inst.Children)
                    foreach (PLCOriginInst inst in inmodel.Insts) 
                        if (inst.Status == PLCOriginInst.STATUS_WARNING)
                        {
                            wcount++;
                            weinsts.Add(new ErrorReportElement(inst));
                        } 
            return weinsts;
        }

        public bool CheckFuncBlock(bool showreport = false)
        {
            bool result = false;
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Loading, Properties.Resources.Funcblock_Check));
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Funcblock_Check);
            wndMain.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                handle.Start();
                result = _CheckFuncBlock(handle, showreport);
            });
            while (!handle.Completed) Thread.Sleep(10);
            if (!showreport)
            {
                if (result)
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Normal, Properties.Resources.FuncBlock_Correct));
                else
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Error, Properties.Resources.FuncBlock_Error));
            }
            return result;
        }

        public bool _CheckFuncBlock(LoadingWindowHandle handle, bool showreport)
        {
            List<string> cfiles = new List<string>();
            List<string> ofiles = new List<string>();
            Process cmd = null;
            //StringBuilder stdout = null;
            StringBuilder stderr = null;
            Match m1 = null;
            Match m2 = null;
            string message = null;
            int sline = 0;
            int line = 0;
            int column = 0;
            ErrorReportElement_FB ewele = null;
            List<ErrorReportElement_FB> eweles = new List<ErrorReportElement_FB>();
            foreach (FuncBlockModel fbmodel in mdProj.FuncBlocks)
            {
                //string hfile = SamSoarII.Utility.FileHelper.GetTempFile(".h");
                string cfile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
                string ofile = SamSoarII.Utility.FileHelper.GetTempFile(".o");
                cfiles.Add(cfile);
                ofiles.Add(ofile);
                StreamWriter cw = new StreamWriter(cfile);
                cw.Write("#include <stdint.h>\n");
                cw.Write("typedef uint32_t* BIT;\r\n");
                cw.Write("typedef int16_t* WORD;\r\n");
                cw.Write("typedef uint16_t* UWORD;\r\n");
                cw.Write("typedef int32_t* DWORD;\r\n");
                cw.Write("typedef uint32_t* UDWORD;\r\n");
                cw.Write("typedef float* FLOAT;\r\n");
                cw.Write("#define DW ((DWORD)W)\r\n");
                cw.Write("#define FW ((FLOAT)W)\r\n");
                sline = 9 + fbmodel.Funcs.Count();
                if (fbmodel.IsLibrary)
                {
                    cw.Write("double asin(double a) {}\r\n");
                    cw.Write("double acos(double a) {}\r\n");
                    cw.Write("double atan(double a) {}\r\n");
                    cw.Write("double log(double a) {}\r\n");
                    cw.Write("double log10(double a) {}\r\n");
                    cw.Write("double sqrt(double a) {}\r\n");
                    sline += 6;
                }
                foreach (FuncModel fmodel in fbmodel.Funcs)
                {
                    cw.Write(String.Format("{0:s} {1:s}(",
                        fmodel.ReturnType, fmodel.Name));
                    if (fmodel.ArgCount > 0)
                    {
                        cw.Write(fmodel.GetArgType(0));
                        for (int i = 1; i < fmodel.ArgCount; i++)
                        {
                            cw.Write("," + fmodel.GetArgType(i));
                        }
                    }
                    cw.Write(");\n");
                }
                cw.Write(fbmodel.View != null ? fbmodel.View.Code : fbmodel.Code);
                cw.Close();
                cmd = new Process();
                cmd.StartInfo.FileName =
                    String.Format(@"{0:s}\Compiler\arm\bin\arm-none-eabi-gcc",
                        FileHelper.AppRootPath);
                cmd.StartInfo.Arguments = string.Format("-c {0} -o {1}", cfile, ofile);
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                //cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.Start();
                //stdout = new StringBuilder();
                stderr = new StringBuilder();
                while (!cmd.HasExited)
                {
                    //stdout.Append(cmd.StandardOutput.ReadToEnd());
                    stderr.Append(cmd.StandardError.ReadToEnd());
                    Thread.Sleep(50);
                }
                m1 = Regex.Match(stderr.ToString(), @"[^\s](.+):(.+):(.+): error: (.+)\r\n");
                m2 = Regex.Match(stderr.ToString(), @"[^\s](.+):(.+):(.+): warning: (.+)\r\n");
                while (m1 != null && m1.Success)
                {
                    message = m1.Groups[4].Value;
                    line = int.Parse(m1.Groups[2].Value) - sline;
                    if (line > 0)
                    {
                        column = int.Parse(m1.Groups[3].Value);
                        ewele = new ErrorReportElement_FB
                        (
                            ErrorReportElement_FB.STATUS_ERROR,
                            message,
                            fbmodel,
                            line,
                            column
                        );
                        eweles.Add(ewele);
                    }
                    m1 = m1.NextMatch();
                }
                while (m2 != null && m2.Success)
                {
                    message = m2.Groups[4].Value;
                    line = int.Parse(m2.Groups[2].Value) - sline;
                    column = int.Parse(m2.Groups[3].Value);
                    ewele = new ErrorReportElement_FB
                    (
                        ErrorReportElement_FB.STATUS_WARNING,
                        message,
                        fbmodel,
                        line,
                        column
                    );
                    eweles.Add(ewele);
                    m2 = m2.NextMatch();
                }
            }
            string bfile = SamSoarII.Utility.FileHelper.GetTempFile(".o");
            cmd = new Process();
            cmd.StartInfo.FileName =
                cmd.StartInfo.FileName =
                    String.Format(@"{0:s}\Compiler\arm\bin\arm-none-eabi-gcc",
                        FileHelper.AppRootPath);
            cmd.StartInfo.Arguments = String.Format("-o {0:s}", bfile);
            foreach (string ofile in ofiles)
            {
                cmd.StartInfo.Arguments += " " + ofile;
            }
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            //cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            //stdout = new StringBuilder();
            stderr = new StringBuilder();
            while (!cmd.HasExited)
            {
                //stdout.Append(cmd.StandardOutput.ReadToEnd());
                stderr.Append(cmd.StandardError.ReadToEnd());
                Thread.Sleep(50);
            }
            m1 = Regex.Match(stderr.ToString(), @"\s(.+):\((.+)\): (.+)\r\n");
            while (m1 != null && m1.Success)
            {
                message = m1.Groups[3].Value;
                line = column = 0;
                string _cfile = m1.Groups[1].Value;
                IEnumerable<string> _cfiles_fit = cfiles.Where(
                    (file) => { return file.EndsWith(_cfile); }
                );
                if (_cfiles_fit.Count() > 0)
                {
                    _cfile = _cfiles_fit.First();
                    int _cfile_id = cfiles.IndexOf(_cfile);
                    if (_cfile_id >= 0)
                    {
                        FuncBlockModel _fbmodel = mdProj.FuncBlocks[_cfile_id];
                        ewele = new ErrorReportElement_FB
                        (
                            ErrorReportElement_FB.STATUS_ERROR,
                            message,
                            _fbmodel,
                            line,
                            column
                        );
                        eweles.Add(ewele);
                    }
                }
                m1 = m1.NextMatch();
            }
            int ecount = 0;
            int wcount = 0;
            foreach (ErrorReportElement_FB _ewele in eweles)
            {
                switch (_ewele.Status)
                {
                    case ErrorReportElement_FB.STATUS_ERROR:
                        ecount++; break;
                    case ErrorReportElement_FB.STATUS_WARNING:
                        wcount++; break;
                }
            }
            bool result = (ecount == 0);
            wndError.Mode = ErrorReportWindow.MODE_FUNC;
            wndError.Update(eweles);
            if (showreport || !result)
            {
                if (ecount == 0 && wcount == 0)
                    ShowMessage(Properties.Resources.Function_Block_Correct, handle, false, false);
                else
                {
                    if (App.CultureIsZH_CH())
                        ShowMessage(String.Format("函数块发生{0:d}处错误，{1:d}处警告。", ecount, wcount), handle, true, false);
                    else
                        ShowMessage(String.Format("There are {0} errors and {1} warnings in the funcblock.", ecount, wcount), handle, true, false);
                    wndMain.LACErrorList.Show();
                }
            }
            else
                handle?.Abort();
            if(handle != null)
                handle.Completed = true;
            return result;
        }
        
        #endregion

        #region View Mode

        public void SetLadderMode(bool istrue)
        {
            if (istrue)
                tcMain.ViewMode |= MainTabControl.VIEWMODE_LADDER;
            else
                tcMain.ViewMode &= ~MainTabControl.VIEWMODE_LADDER;
        }

        public void SetInstMode(bool istrue)
        {
            if (istrue)
                tcMain.ViewMode |= MainTabControl.VIEWMODE_INST;
            else
                tcMain.ViewMode &= ~MainTabControl.VIEWMODE_INST;
        }

        public void SetCommentMode(bool istrue)
        {
            vmdProj.IsCommentMode = istrue;
        }

        #endregion

        #region Navigate & Select
        
        private LadderNetworkModel _lastnet;
        private int _lastx1;
        private int _lasty1;
        private int _lastx2;
        private int _lasty2;
        private LadderDiagramModel _lastdia;
        private int _laststart;
        private int _lastend;
        private FuncBlockModel _lastfunc;
        private int _lastoffset;
        private int _lastrow;
        private int _lastcolumn;

        public void Navigate(string text)
        {
            Match m = Regex.Match(text, @"\(Diagram=([^)]*)\)\(Network=([^)]*)\)\(X=([^,]*),Y=([^)]*)\)");
            if (m.Success)
            {
                string diagramname = m.Groups[1].Value;
                int networkid = int.Parse(m.Groups[2].Value);
                int x = int.Parse(m.Groups[3].Value);
                int y = int.Parse(m.Groups[4].Value);
                LadderDiagramModel diagram = mdProj.Diagrams.Where(d => d.Name.Equals(diagramname)).FirstOrDefault();
                LadderNetworkModel network = diagram?.Children[networkid];
                Navigate(network, x, y);
            }
        }

        public void Navigate(LadderUnitModel unit)
        {
            if (unit == null) return;
            Navigate(unit.Parent, unit.X, unit.Y);
        }

        public void Navigate(LadderNetworkModel network)
        {
            Navigate(network, 0, 0);
        }
        
        public void Navigate(LadderNetworkModel network, int x, int y)
        {
            if (network == null || network.IsMasked) return;
            LadderDiagramModel diagram = network.Parent;
            if (diagram == null) return;
            if (diagram.Tab == null)
            {
                _lastnet = network;
                _lastx1 = x;
                _lasty1 = y;
                diagram.Tab = new MainTabDiagramItem(tcMain, diagram, diagram.Inst);
                diagram.Tab.Loaded += OnViewLoadedToNavigate;
                return;
            }
            tcMain.ShowItem(diagram.Tab);
            diagram.View.ReleaseSelect();
            SelectRectCore rect = diagram.View.SelectionRect.Core;
            rect.Parent = network;
            rect.X = x;
            rect.Y = y;
            diagram.View.SelectionStatus = SelectStatus.SingleSelected;
            diagram.View.NavigateToSelectRect();
        }
        
        private void OnViewLoadedToNavigate(object sender, RoutedEventArgs e)
        {
            Navigate(_lastnet, _lastx1, _lasty1);
            _lastnet.Parent.Tab.Loaded -= OnViewLoadedToNavigate;
        }
        
        public void Navigate(FuncBlock fblock)
        {
            Navigate(fblock.Model, fblock.IndexStart);
        }

        public void Navigate(FuncBlockModel fbmodel, int offset)
        {
            if (fbmodel.View == null)
            {
                _lastfunc = fbmodel;
                _lastoffset = offset;
                fbmodel.View = new FuncBlockViewModel(fbmodel, tcMain);
                fbmodel.View.Loaded += OnViewLoadedToNavigateOffset;
            }
            tcMain.ShowItem(fbmodel.View);
            fbmodel.View.SetOffset(offset);
        }

        private void OnViewLoadedToNavigateOffset(object sender, RoutedEventArgs e)
        {
            Navigate(_lastfunc, _lastoffset);
            _lastfunc.View.Loaded -= OnViewLoadedToNavigateOffset;
        }
        
        public void Navigate(FuncBlockModel fbmodel, int row, int column)
        {
            if (fbmodel.View == null)
            {
                _lastfunc = fbmodel;
                _lastrow = row;
                _lastcolumn = column;
                fbmodel.View = new FuncBlockViewModel(fbmodel, tcMain);
                fbmodel.View.Loaded += OnViewLoadedToNavigatePosition;
            }
            tcMain.ShowItem(fbmodel.View);
            fbmodel.View.SetPosition(row, column);
        }

        private void OnViewLoadedToNavigatePosition(object sender, RoutedEventArgs e)
        {
            Navigate(_lastfunc, _lastrow, _lastcolumn);
            _lastfunc.View.Loaded -= OnViewLoadedToNavigatePosition;
        }

        public void Select(LadderNetworkModel network, int x1, int x2, int y1, int y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                Navigate(network, x1, y1);
                return;
            }
            if (network.IsMasked) return;
            LadderDiagramModel diagram = network.Parent;
            if (diagram.Tab == null)
            {
                _lastnet = network;
                _lastx1 = x1;
                _lastx2 = x2;
                _lasty1 = y1;
                _lasty2 = y2;
                diagram.Tab = new MainTabDiagramItem(tcMain, diagram, diagram.Inst);
                diagram.Tab.Loaded += OnViewLoadedToSelectRange;
                return;
            }
            tcMain.ShowItem(diagram.Tab);
            diagram.View.Select(network, x1, x2, y1, y2);
        }

        private void OnViewLoadedToSelectRange(object sender, RoutedEventArgs e)
        {
            Select(_lastnet, _lastx1, _lastx2, _lasty1, _lasty2);
            _lastnet.Parent.Tab.Loaded -= OnViewLoadedToSelectRange;
        }
        
        public void Select(LadderDiagramModel diagram, int start, int end)
        {
            if (diagram.Tab == null)
            {
                _lastdia = diagram;
                _laststart = start;
                _lastend = end;
                diagram.Tab = new MainTabDiagramItem(tcMain, diagram, diagram.Inst);
                diagram.Tab.Loaded += OnViewLoadedToSelectNetworks;
            }
            tcMain.ShowItem(diagram.Tab);
            diagram.View.Select(start, end);
        }
        
        private void OnViewLoadedToSelectNetworks(object sender, RoutedEventArgs e)
        {
            _lastdia.Tab.Loaded -= OnViewLoadedToSelectNetworks;
            Select(_lastdia, _laststart, _lastend);
        }

        public void NavigateToBreakpointCursor()
        {
            if (!mngSimu.IsBPPause) return;
            IBreakpoint ibrpo = mngSimu.Viewer.Cursor.Current;
            if (ibrpo is LadderBrpoModel)
            {
                vmdProj.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    LadderUnitModel unit = ((LadderBrpoModel)ibrpo).Parent;
                    Navigate(unit);
                    if (unit.Breakpoint?.IsEnable == true)
                        SelectToBrpoTable(unit);
                });
            }
            if (ibrpo is FuncBrpoModel)
            {
                vmdProj.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    Navigate(((FuncBrpoModel)ibrpo).Parent);
                });
            }
        }

        public void SelectToBrpoTable(LadderUnitModel unit)
        {
            wndMain.LACBreakpoint.Show();
            wndBrpo.Select(unit);
        }

        #endregion

        #region Dialogs

        private void ShowMessage(string message, LoadingWindowHandle handle, bool isError, bool isLadder)
        {
            if (isLadder)
            {
                if (isError)
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Error, Properties.Resources.Ladder_Error));
                else
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Normal, Properties.Resources.Ladder_Correct));
            }
            else
            {
                if (isError)
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Error, Properties.Resources.FuncBlock_Error));
                else
                    PostIWindowEvent(null, new UnderBarEventArgs(barStatus, UnderBarStatus.Normal, Properties.Resources.FuncBlock_Correct));
            }
            handle?.Abort();
            LocalizedMessageBox.Show(message, LocalizedMessageIcon.Information);
        }

        public LocalizedMessageResult ShowSaveYesNoCancelDialog(string message = "")
        {
            string title = Properties.Resources.Message_Confirm_Save;
            string text = String.Format("{0:s} {1}", mdProj.ProjName, message != string.Empty ? message : Properties.Resources.Message_Changed);
            return LocalizedMessageBox.Show(text, title, LocalizedMessageButton.YesNoCancel, LocalizedMessageIcon.Question);
        }

        public void ShowCreateProjectDialog()
        {
            NewProjectDialog newProjectDialog;
            using (newProjectDialog = new NewProjectDialog())
            {
                newProjectDialog.EnsureButtonClick += (sender1, e1) =>
                {
                    PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(newProjectDialog.Type);
                    if (newProjectDialog.IsSettingChecked)
                    {
                        string name = newProjectDialog.NameContent;
                        string dir = newProjectDialog.PathContent;
                        if (!Directory.Exists(dir))
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_Path, LocalizedMessageIcon.Information);
                            return;
                        }
                        if (name == string.Empty)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Name, LocalizedMessageIcon.Information);
                            return;
                        }
                        string fullFileName = string.Format(@"{0}\{1}.{2}", dir, name, FileHelper.NewFileExtension, LocalizedMessageIcon.Information);
                        if (File.Exists(fullFileName))
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Exist, LocalizedMessageIcon.Information);
                            return;
                        }
                        CreateProject(name, fullFileName);
                    }
                    else
                    {
                        CreateProject(Properties.Resources.New_Project_Name);
                    }
                    newProjectDialog.Close();
                };
                newProjectDialog.ShowDialog();
            }
        }
        
        public void ShowOpenProjectDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = string.Format("{0}|*.{1};*.{2}",Properties.Resources.Project_File, FileHelper.NewFileExtension, FileHelper.OldFileExtension);
            if (openFileDialog.ShowDialog() == true)
            {
                /*
                if (mdProj != null && openFileDialog.FileName.Equals(mdProj.FileName))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Message_Project_Loaded, LocalizedMessageIcon.Information);
                    return;
                }
                */
                if (openFileDialog.FileName.EndsWith(FileHelper.OldFileExtension))
                {
                    ShowFileConvertDialog(openFileDialog.FileName);
                }
                else LoadProject(openFileDialog.FileName);
            }
        }

        public void ShowSaveProjectDialog()
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = string.Format("{0}文件|*.{0}", FileHelper.NewFileExtension);
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveAsProject(saveFileDialog.FileName);
            }
        }

        //private SelectRectCore instinputrect = null;
        public void ShowInstructionInputDialog(string initialString, SelectRectCore core)
        {
            if (core.Parent == null) return;
            using (InstructionInputDialog dialog = new InstructionInputDialog(mdProj, initialString))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.EnsureButtonClick += (sender, e) =>
                {
                    try
                    {
                        core.Parent.Parent.AddSingleUnit(dialog.InstructionInput, core, core.Parent);
                        if (core.X < GlobalSetting.LadderXCapacity - 1) core.X++;
                        core.Parent.Parent.View.NavigateByInstructionInputDialog();
                        dialog.Close();
                    }
                    catch (Exception exce2)
                    {
                        LocalizedMessageBox.Show(string.Format(exce2.Message), LocalizedMessageIcon.Error);
                    }
                };
                dialog.ShowDialog();
            }
        }

        public void ShowElementPropertyDialog(LadderUnitModel current)
        {
            if (current is POLYLINEModel)
            {
                using (PolylinePropertyDialog dialog = new PolylinePropertyDialog(((POLYLINEModel)current).Clone()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        current.Parent.Parent.AddSingleUnit(dialog.Core, current.Parent);
                        dialog.Close();
                    };
                    dialog.ShowDialog();
                }
                return;
            }
            if (current is TBLModel)
            {
                using (TBLDialog dialog = new TBLDialog(((TBLModel)current).Clone()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        try
                        {
                            dialog.SaveToCore();
                            current.Parent.Parent.AddSingleUnit(dialog.Core, current.Parent);
                            dialog.Close();
                        }
                        catch (ValueParseException exce)
                        {
                            LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                        }
                    };
                    dialog.Cancel += (sender, e) => { dialog.Close(); };
                    dialog.ShowDialog();
                }
                return;
            }
            if (current is CAMModel)
            {
                using (CAMDialog dialog = new CAMDialog(((CAMModel)current).Clone()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        try
                        {
                            dialog.Save();
                            current.Parent.Parent.AddSingleUnit(dialog.Core, current.Parent);
                            dialog.Close();
                        }
                        catch (ValueParseException exce)
                        {
                            LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                        }
                    };
                    dialog.Cancel += (sender, e) => { dialog.Close(); };
                    dialog.Help += (sender, e) => { ShowHelpDocument(); };
                    dialog.ShowDialog();
                }
                return;
            }
            if (current.Type == LadderUnitModel.Types.PID)
            {
                using (PIDDialog dialog = new PIDDialog(this, current))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        try
                        {
                            current.Parent.Parent.UpdateUC(current, dialog.PropertyStrings);
                            current.Parent.Parent.View.NavigateByInstructionInputDialog();
                            dialog.Close();
                        }
                        catch (Exception exce)
                        {
                            LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                        }
                    };
                    dialog.Cancel += (sender, e) =>
                    {
                        dialog.Close();
                    };
                    dialog.ShowDialog();
                }
                return;
            }
            using (ElementPropertyDialog dialog = new ElementPropertyDialog(current))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.Ensure += (sender, e) =>
                {
                    try
                    {
                        current.Parent.Parent.UpdateUC(current, dialog.PropertyStrings);
                        current.Parent.Parent.View.NavigateByInstructionInputDialog();
                        dialog.Close();
                    }
                    catch (Exception exce2)
                    {
                        LocalizedMessageBox.Show(string.Format(exce2.Message), LocalizedMessageIcon.Error);
                    }
                };
                dialog.ShowDialog();
            }
        }

        public bool ShowElementPropertyDialog(LadderUnitModel.Types type, SelectRectCore core, bool cover = true)
        {
            bool ret;
            LadderUnitModel current = LadderUnitModel.Create(core.Parent, type);
            current.X = core.X;
            current.Y = core.Y;
            ret = ShowImmediateElementPropertyDialog(current, cover);
            return ret;
        }
        
        public bool ShowElementPropertyDialog(FuncModel func, SelectRectCore core)
        {
            LadderUnitModel current = new LadderUnitModel(core.Parent, func) { X = core.X, Y = core.Y };
            return ShowImmediateElementPropertyDialog(current, false);
        }

        public bool ShowElementPropertyDialog(ModbusModel modbus, SelectRectCore core)
        {
            LadderUnitModel current = new LadderUnitModel(core.Parent, modbus) { X = core.X, Y = core.Y };
            return ShowImmediateElementPropertyDialog(current, false);
        }

        private bool ShowImmediateElementPropertyDialog(LadderUnitModel current, bool cover = true)
        {
            bool ret = false;
            if (current is POLYLINEModel)
            {
                using (PolylinePropertyDialog dialog = new PolylinePropertyDialog(((POLYLINEModel)current).Clone()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        current.Parent.Parent.AddSingleUnit(dialog.Core, current.Parent);
                        ret = true;
                        dialog.Close();
                    };
                    dialog.ShowDialog();
                }
                return ret;
            }
            if (current is TBLModel)
            {
                using (TBLDialog dialog = new TBLDialog(((TBLModel)current).Clone()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        try
                        {
                            dialog.SaveToCore();
                            current.Parent.Parent.AddSingleUnit(dialog.Core, current.Parent);
                            ret = true;
                            dialog.Close();
                        }
                        catch (ValueParseException exce)
                        {
                            LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                        }
                    };
                    dialog.Cancel += (sender, e) => { dialog.Close(); };
                    dialog.ShowDialog();
                }
                return ret;
            }
            if (current is CAMModel)
            {
                using (CAMDialog dialog = new CAMDialog(((CAMModel)current).Clone()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        try
                        {
                            dialog.Save();
                            current.Parent.Parent.AddSingleUnit(dialog.Core, current.Parent);
                            ret = true;
                            dialog.Close();
                        }
                        catch (ValueParseException exce)
                        {
                            LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                        }
                    };
                    dialog.Cancel += (sender, e) => { dialog.Close(); };
                    dialog.Help += (sender, e) => { ShowHelpDocument(); };
                    dialog.ShowDialog();
                }
                return ret;
            }
            if (current.Type == LadderUnitModel.Types.PID)
            {
                using (PIDDialog dialog = new PIDDialog(this, current))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Ensure += (sender, e) =>
                    {
                        try
                        {
                            IList<string> properties = dialog.PropertyStrings;
                            List<string> instargs = new List<string>();
                            for (int i = 0; i < properties.Count / 2; i++)
                            {
                                string value = properties[i * 2];
                                instargs.Add(value);
                                try
                                {
                                    mngValue[value].Comment = properties[i * 2 + 1];
                                }
                                catch (ValueParseException)
                                {
                                }
                            }
                            current.InstArgs = instargs.ToArray();
                            current.Parent.Parent.AddSingleUnit(current, current.Parent, cover);
                            ret = true;
                            dialog.Close();
                        }
                        catch (Exception exce)
                        {
                            LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                        }
                    };
                    dialog.Cancel += (sender, e) =>
                    {
                        current.Dispose();
                        dialog.Close();
                    };
                    dialog.ShowDialog();
                }
                return ret;
            }
            using (ElementPropertyDialog dialog = new ElementPropertyDialog(current))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.Ensure += (sender, e) =>
                {
                    try
                    {
                        IList<string> properties = dialog.PropertyStrings;
                        List<string> instargs = new List<string>();
                        for (int i = 0; i < properties.Count / 2; i++)
                        {
                            string value = properties[i * 2];
                            instargs.Add(value);
                            try
                            {
                                mngValue[value].Comment = properties[i * 2 + 1];
                            }
                            catch (ValueParseException)
                            {
                            }
                        }
                        current.InstArgs = instargs.ToArray();
                        current.Parent.Parent.AddSingleUnit(current, current.Parent, cover);
                        ret = true;
                        dialog.Close();
                    }
                    catch (Exception exce2)
                    {
                        LocalizedMessageBox.Show(string.Format(exce2.Message), LocalizedMessageIcon.Error);
                    }
                };
                dialog.Cancel += (sender, e) => { current.Dispose(); };
                dialog.ShowDialog();
            }
            return ret;
        }

        public void ShowProjectPropertyDialog()
        {
            using (ProjectPropertyDialog dialog = new ProjectPropertyDialog(mdProj))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.EnsureButtonClick += (sender, e) =>
                {
                    try
                    {
                        bool ret = true;
                        if (mdProj.LadderMode != LadderModes.Edit)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Modify_Project, LocalizedMessageIcon.Warning);
                        }
                        else
                        {
                            ret = dialog.Save();
                        }
                        if (ret) dialog.Close();
                    }
                    catch (Exception exce2)
                    {
                        LocalizedMessageBox.Show(string.Format(exce2.Message), LocalizedMessageIcon.Error);
                    }
                };
                dialog.ShowDialog();
            }
        }
        
        public void ShowValueModifyDialog(IEnumerable<object> args, int id = 0)
        {
            if (vmdProj.LadderMode == LadderModes.Simulate && !mngSimu.IsAlive) return;
            if (vmdProj.LadderMode == LadderModes.Monitor && !mngComu.IsAlive) return;
            if (args.Count() == 1)
            {
                using (ValueModifyDialog dialog = new ValueModifyDialog(args.First()))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.ShowDialog();
                }
            }
            else if (args.Count() > 1)
            {
                using (ValueMultiplyModifyDialog dialog = new ValueMultiplyModifyDialog(args))
                {
                    dialog.Owner = wndMain;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.SelectedIndex = id;
                    dialog.ShowDialog();
                }
            }
        }

        public void ShowEditDiagramCommentDialog(LadderDiagramModel core)
        {
            using (LadderDiagramCommentEditDialog dialog = new LadderDiagramCommentEditDialog(core))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    core.Name = dialog.LadderName;
                    core.Brief = dialog.LadderComment;
                    dialog.Close();
                };
                dialog.CancelButtonClick += (sender2, e2) =>
                {
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }
        
        public void ShowEditNetworkCommentDialog(LadderNetworkModel core)
        {
            using (LadderNetworkCommentEditDialog dialog = new LadderNetworkCommentEditDialog(core))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    core.Brief = dialog.NetworkBrief;
                    core.Description = dialog.NetworkDescription;
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }

        private OptionDialog dlgOption;
        public void ShowSystemOptionDialog()
        {
            if (dlgOption == null)
            {
                dlgOption = new OptionDialog(this);
                dlgOption.Owner = wndMain;
                dlgOption.EnsureButtonClick += (sender, e) =>
                {
                    if (VMDProj != null) VMDProj.UpdateUnit();
                };
            }
            dlgOption.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dlgOption.ShowDialog();
        }

        public void ShowCommunicationSettingDialog()
        {
            CommunicationParams paraCom = mdProj.PARAProj.PARACom;
            using (CommunicationSettingDialog dialog = new CommunicationSettingDialog(paraCom))
            {
                dialog.Owner = wndMain;
                ComBaseSetting baseSetting = dialog.GetBaseSetting();
                baseSetting.SettingButtonClick += (sender1, e1) =>
                {
                    using (CommunicationParamsDialog dialog1 = new CommunicationParamsDialog(paraCom))
                    {
                        dialog1.ShowDialog();
                    }
                };
                dialog.Ensure += OnCommunicationTest;
                dialog.CommunicationTest += OnCommunicationTest;
                dialog.ShowDialog();
            }
        }

        private void OnCommunicationTest(object sender, RoutedEventArgs e)
        {
            mngComu.IsEnable = true;
            if (mngComu.CheckLink())
                LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Success, LocalizedMessageIcon.Information);
            else if (mngComu.MNGCurrent.Ex != null && mngComu.MNGCurrent.Ex is UnauthorizedAccessException)
            {
                LocalizedMessageBox.Show(Properties.Resources.SerialPort_Occupied, LocalizedMessageIcon.Information);
            }
            else LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
            mngComu.IsEnable = false;
        }

        public void ShowLanaEnsureDialog(bool ischinese)
        {
            using (LanaEnsureDialog dialog = new LanaEnsureDialog())
            {
                dialog.Owner = wndMain;
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    GlobalSetting.IsOpenLSetting = true;
                    GlobalSetting.LanagArea = ischinese ? "zh-Hans" : "en";
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }

        HelpDocWindow helpDocWindow = null;
        public void ShowHelpDocument()
        {
            if (helpDocWindow == null)
            {
                helpDocWindow = new HelpDocWindow();
                helpDocWindow.Closed += (sender, e) => { helpDocWindow = null; };
                helpDocWindow.Show();
            }
        }

        public void ShowFileConvertDialog()
        {
            FileConvertDialog dialog = new FileConvertDialog(this);
            dialog.Owner = wndMain;
            dialog.ShowDialog();
        }
        public void ShowFileConvertDialog(string filename)
        {
            FileConvertDialog dialog = new FileConvertDialog(this,filename);
            dialog.Owner = wndMain;
            dialog.ShowDialog();
        }

        public void ShowPolylineSystemSettingDialog()
        {
            using (PolylineSystemSettingDialog dialog = new PolylineSystemSettingDialog(mdProj))
            {
                dialog.Owner = wndMain;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.Ensure += (sender, e) =>
                {
                    try
                    {
                        dialog.Save();
                        dialog.Close();
                    }
                    catch (ValueParseException exce)
                    {
                        LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                    }
                };
                dialog.Help += (sender, e) => { ShowHelpDocument(); };
                dialog.ShowDialog();
            }
        }

        public void ShowImageImportDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = string.Format("{0}|*.{1}", Properties.Resources.DXF_File, "dxf");
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                DXFModel dxfmodel = new DXFModel();
                dxfmodel.Convert(openFileDialog.FileName);
                Window window = new Window();
                DXFImage image = new DXFImage(dxfmodel);
                DXFImage.BaseP = new Point(0, 550);
                image.DrawImage();
                DrawingPanel panel = new DrawingPanel();
                window.Content = panel;
                panel.Add(image);
                window.Show();
            }
        }
        #endregion

        #region HotKey System

        /// <summary>
        /// 表示是否在等待命令的输入
        /// </summary>
        public bool IsWaitForKey
        {
            get
            {
                return ThreeHotKeyManager.IsWaitForSecondKey || ThreeHotKeyManager.IsWaitForSecondModifier;
            }
        }
        
        public void SetUnderBarMessage(string message)
        {
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus, barStatus.Status, message));
        }

        #endregion

        #region Modify
        public void ReturnToEdit()
        {
            switch (mdProj.LadderMode)
            {
                case LadderModes.Edit:
                    LocalizedMessageBox.Show(Properties.Resources.In_Edit_Mode,LocalizedMessageIcon.Information);
                    break;
                case LadderModes.Simulate:
                case LadderModes.Monitor:
                    mdProj.LadderMode = LadderModes.Edit;
                    break;
                default:
                    break;
            }
        }

        public void ZoomChanged(bool isZoomIn)
        {
            if (isZoomIn)
            {
                GlobalSetting.LadderScaleX *= 1.1;
                GlobalSetting.LadderScaleY *= 1.1;
            }
            else
            {
                GlobalSetting.LadderScaleX /= 1.1;
                GlobalSetting.LadderScaleY /= 1.1;
            }
        }
        public void AddNewSubRoutine()
        {
            wndMain.LACProj.Show();
            tvProj.AddNewSubRoutines();
        }

        public void AddNewFuncBlock()
        {
            wndMain.LACProj.Show();
            tvProj.AddNewFuncBlock();
        }
        
        public void AddNewModbus()
        {
            tvProj.AddNewModbus();
        }

        public void QuickInsertElement(LadderUnitModel.Types type)
        {
            if (CurrentLadder == null) return;
            CurrentLadder.QuickInsertElement(type);
        }

        public void QuickRemoveElement(LadderUnitModel.Types type)
        {
            if (CurrentLadder == null) return;
            CurrentLadder.QuickRemoveElement(type);
        }

        public void InsertRow()
        {
            if (CurrentLadder == null) return;
            CurrentLadder.InsertRow();
        }

        public void RemoveRow()
        {
            if (CurrentLadder == null) return;
            CurrentLadder.RemoveRow();
        }

        public void InsertNet()
        {
            if (CurrentLadder == null) return;
            CurrentLadder.InsertNet();
        }

        public void RemoveNet()
        {
            if (CurrentLadder == null) return;
            CurrentLadder.RemoveNet();
        }

        public void ProjectCompile()
        {
            if (vmdProj.LadderMode != LadderModes.Edit)
            {
                PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                UnderBarStatus.Normal, Properties.Resources.Not_Compiled));
                return;
            }
            PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                UnderBarStatus.Normal, Properties.Resources.Compiling));
            LoadingWindowHandle handle = new LoadingWindowHandle(string.Format("{0}...\n{1}", Properties.Resources.Compiling,Properties.Resources.LadderDiagram_check));
            handle.Start();
            bool ret = false;
            vmdProj.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                thmngCore.MNGInst.Pause();
                while (thmngCore.MNGInst.IsActive)
                    Thread.Sleep(10);
                ret = _CheckLadder(null, false);
                thmngCore.MNGInst.Start();
                if(ret)
                {
                    handle.UpdateMessage(string.Format("{0}...\n{1}", Properties.Resources.Compiling, Properties.Resources.Funcblock_Check));
                    ret &= _CheckFuncBlock(null, false);
                }
                handle.Completed = true;
                handle.Abort();
            });
            while (!handle.Completed) Thread.Sleep(10);
            if (ret)
            {
                PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                UnderBarStatus.Normal, Properties.Resources.Compiled_Success));
                LocalizedMessageBox.Show(Properties.Resources.Compiled_Success, LocalizedMessageIcon.Information);
            }
            else
            {
                PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                UnderBarStatus.Normal, Properties.Resources.Compile_Failed));
                LocalizedMessageBox.Show(Properties.Resources.Compile_Failed, LocalizedMessageIcon.Error);
            }
        }

        #endregion

        #region Event Handler
        
        public event IWindowEventHandler PostIWindowEvent = delegate { };
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            if (sender is MainTabControl && e is MainTabControlEventArgs)
            {
                MainTabControlEventArgs e1 = (MainTabControlEventArgs)e;
                switch (e1.Action)
                {
                    case TabAction.SELECT:
                    case TabAction.ACTIVE:
                        PostIWindowEvent(sender, e);
                        break;
                    case TabAction.VIEWMODE:
                        PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                            ((tcMain.ViewMode & MainTabControl.VIEWMODE_LADDER) != 0
                                ? MainWindowEventArgs.TYPE_TOGGLE_DOWN
                                : MainWindowEventArgs.TYPE_TOGGLE_UP)
                            | MainWindowEventArgs.FLAG_LADDER));
                        PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                            ((tcMain.ViewMode & MainTabControl.VIEWMODE_INST) != 0
                                ? MainWindowEventArgs.TYPE_TOGGLE_DOWN
                                : MainWindowEventArgs.TYPE_TOGGLE_UP)
                            | MainWindowEventArgs.FLAG_INST));
                        break;
                }
            }
            if (sender is ProjectTreeView && e is ProjectTreeViewEventArgs)
            {
                ProjectTreeViewEventArgs e2 = (ProjectTreeViewEventArgs)e;
                switch (e2.Flags & ~0xf)
                {
                    case ProjectTreeViewEventArgs.FLAG_DOUBLECLICK:
                        if (e2.RelativeObject is LadderDiagramModel)
                        {
                            LadderDiagramModel ldmodel = (LadderDiagramModel)(e2.RelativeObject);
                            if (ldmodel.Tab == null) ldmodel.Tab = new MainTabDiagramItem(tcMain, ldmodel, ldmodel.Inst);
                            tcMain.ShowItem(ldmodel.Tab);
                        }
                        if (e2.RelativeObject is LadderNetworkModel)
                        {
                            LadderNetworkModel lnmodel = (LadderNetworkModel)(e2.RelativeObject);
                            Navigate(lnmodel);
                        }
                        if (e2.RelativeObject is FuncBlockModel)
                        {
                            FuncBlockModel fbmodel = (FuncBlockModel)(e2.RelativeObject);
                            if (fbmodel.View == null)
                                fbmodel.View = new FuncBlockViewModel(fbmodel, tcMain);
                            tcMain.ShowItem(fbmodel.View);
                        }
                        if (e2.RelativeObject is FuncModel)
                        {
                            FuncModel fmodel = (FuncModel)(e2.RelativeObject);
                            Navigate(fmodel.Parent, fmodel.Offset);
                        }
                        if (e2.RelativeObject is ModbusTableModel)
                        {
                            ModbusTableModel mtmodel = (ModbusTableModel)(e2.RelativeObject);
                            tcMain.ShowItem(mtmodel.View);
                        }
                        if (e2.RelativeObject is ModbusModel)
                        {
                            ModbusModel mmodel = (ModbusModel)(e2.RelativeObject);
                            ModbusTableModel mtmodel = mmodel.Parent;
                            tcMain.ShowItem(mtmodel.View);
                            mtmodel.View.Current = mmodel;
                        }
                        if (e2.RelativeObject is LadderUnitModel.Types)
                        {
                            LadderUnitModel.Types type = (LadderUnitModel.Types)(e2.RelativeObject);
                            if (CurrentLadder != null)
                                CurrentLadder.QuickInsertElement(type);
                        }
                        if (e2.TargetedObject is ProjectTreeViewItem)
                        {
                            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)(e2.TargetedObject);
                            switch (ptvitem.Flags & 0xf)
                            {
                                case ProjectTreeViewItem.TYPE_ELEMENTLIST:
                                    wndMain.LACElemList.Show();
                                    break;
                                case ProjectTreeViewItem.TYPE_ELEMENTINITIALIZE:
                                    wndMain.LACElemInit.Show();
                                    break;
                            }
                        }
                        break;
                    case ProjectTreeViewEventArgs.FLAG_CONFIG:
                        if (e2.RelativeObject is LadderDiagramModel)
                        {
                            ShowEditDiagramCommentDialog((LadderDiagramModel)(e2.RelativeObject));
                        }
                        if (e2.RelativeObject is LadderNetworkModel)
                        {
                            ShowEditNetworkCommentDialog((LadderNetworkModel)(e2.RelativeObject));
                        }
                        break;
                }
            }
        }

        public bool CanStart
        {
            get
            {
                if (vmdProj == null) return false;
                switch (vmdProj.LadderMode)
                {
                    case LadderModes.Simulate:
                        return !mngSimu.IsActive || mngSimu.IsBPPause;
                    case LadderModes.Monitor:
                        return !mngComu.IsAlive;
                    default:
                        return false;
                }
            }
        }

        public bool CanPause
        {
            get
            {
                if (vmdProj == null) return false;
                switch (vmdProj.LadderMode)
                {
                    case LadderModes.Simulate:
                        return mngSimu.IsActive && !mngSimu.IsBPPause;
                    default:
                        return false;
                }
            }
        }

        public bool CanStop
        {
            get
            {
                if (vmdProj == null) return false;
                switch (vmdProj.LadderMode)
                {
                    case LadderModes.Simulate:
                        return mngSimu.IsAlive;
                    case LadderModes.Monitor:
                        return mngComu.IsAlive;
                    default:
                        return false;
                }
            }
        }
        
        public bool CanExecute(ICommand cmd)
        {
            bool ret = true;
            if (cmd == ApplicationCommands.New
             || cmd == ApplicationCommands.Open
             || cmd == ApplicationCommands.Save
             || cmd == ApplicationCommands.Close)
            {
                ret &= !IsWaitForKey;
            }
            if (cmd == ApplicationCommands.Close) return ret;
            if (cmd != ApplicationCommands.New
             && cmd != ApplicationCommands.Open
             && cmd != GlobalCommand.UploadCommand
             && cmd != GlobalCommand.ShowOptionDialogCommand
             && cmd != GlobalCommand.ChangeToChineseCommand
             && cmd != GlobalCommand.ChangeToEnglishCommand
             && cmd != GlobalCommand.ShowHelpDocumentCommand
             && cmd != GlobalCommand.OnlineHelpCommand
             && cmd != GlobalCommand.ShowAboutCommand
             && cmd != GlobalCommand.FileConvertCommand)
            {
                ret &= mdProj != null && vmdProj != null;
                if (!ret) return ret;
                if (cmd == ApplicationCommands.Save)
                    ret &= mdProj.IsModified;
                if (cmd == GlobalCommand.InstShortCutOpenCommand
                 || cmd == GlobalCommand.InsertRowCommand
                 || cmd == GlobalCommand.DeleteRowCommand
                 || cmd == GlobalCommand.InsertNetCommand
                 || cmd == GlobalCommand.DeleteNetCommand)
                {
                    ret &= CurrentLadder != null && CurrentLadder.SelectionStatus != SelectStatus.Idle;
                }
                if (cmd == MonitorCommand.StartCommand
                 || cmd == GlobalCommand.SimuStartCommand)
                {
                    ret &= CanStart;
                }
                if (cmd == MonitorCommand.StopCommand
                 || cmd == GlobalCommand.SimuStopCommand)
                {
                    ret &= CanStop;
                }
                if (cmd == GlobalCommand.SimuPauseCommand)
                {
                    ret &= CanPause;
                }
                if (cmd == GlobalCommand.BrpoNowCommand
                 || cmd == GlobalCommand.BrpoOutCommand)
                {
                    ret &= mdProj.LadderMode == LadderModes.Simulate && mngSimu.IsBPPause;
                }
                if (cmd == GlobalCommand.BrpoCallCommand
                 || cmd == GlobalCommand.BrpoStepCommand)
                {
                    ret &= mdProj.LadderMode == LadderModes.Simulate && (mngSimu.IsBPPause || !mngSimu.IsActive);
                }
                if (cmd == GlobalCommand.AddNewFuncBlockCommand
                 || cmd == GlobalCommand.AddNewModbusCommand
                 || cmd == GlobalCommand.AddNewSubRoutineCommand
                 || cmd == ApplicationCommands.Replace)
                 //|| cmd == GlobalCommand.CompileCommand
                {
                    ret &= vmdProj.LadderMode == LadderModes.Edit;
                }
            }
            if (cmd == ApplicationCommands.New
             || cmd == ApplicationCommands.Open
             || cmd == ApplicationCommands.Save
             || cmd == ApplicationCommands.SaveAs
             || cmd == GlobalCommand.CloseProjectCommand
             || cmd == GlobalCommand.SimulateCommand
             || cmd == GlobalCommand.DownloadCommand
             || cmd == GlobalCommand.MonitorCommand
             || cmd == GlobalCommand.AddNewFuncBlockCommand
             || cmd == GlobalCommand.AddNewModbusCommand
             || cmd == GlobalCommand.AddNewSubRoutineCommand
             || cmd == GlobalCommand.CompileCommand)
            {
                ret &= !ProjectTreeViewItem.HasRenaming;
            }
            return ret;
        }
        
        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsCommentMode":
                    PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                        (vmdProj.IsCommentMode
                            ? MainWindowEventArgs.TYPE_TOGGLE_DOWN
                            : MainWindowEventArgs.TYPE_TOGGLE_UP)
                        | MainWindowEventArgs.FLAG_COMMENT));
                    break;
            }
        }

        private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Device":
                    PostIWindowEvent(this, new InteractionFacadeEventArgs(InteractionFacadeEventArgs.Types.DeviceModified, sender, sender));
                    PropertyChanged(this, new PropertyChangedEventArgs("Device"));
                    break;
                case "LadderMode":
                    switch (mdProj.LadderMode)
                    {
                        case LadderModes.Edit:
                            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_SHOW
                              | MainWindowEventArgs.FLAG_EDIT));
                            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_HIDE
                              | MainWindowEventArgs.FLAG_SIMULATE));
                            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_TOGGLE_UP
                              | MainWindowEventArgs.FLAG_SIMULATE
                              | MainWindowEventArgs.FLAG_MONITOR));
                            PostIWindowEvent(null, new UnderBarEventArgs(barStatus,
                                UnderBarStatus.Normal, Properties.Resources.Ready));
                            wndMain.LAMonitor.Hide();
                            break;
                        case LadderModes.Simulate:
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_HIDE
                              | MainWindowEventArgs.FLAG_EDIT));
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_SHOW
                              | MainWindowEventArgs.FLAG_SIMULATE));
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_TOGGLE_UP
                              | MainWindowEventArgs.FLAG_MONITOR));
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_TOGGLE_DOWN
                              | MainWindowEventArgs.FLAG_SIMULATE));
                            OnSimulateAborted(this, new RoutedEventArgs());
                            PostIWindowEvent(this, new UnderBarEventArgs(barStatus,
                                UnderBarStatus.Simulate, Properties.Resources.MainWindow_Simulation));
                            wndMain.LAReplace.Hide();
                            wndMain.LACMonitor.Show();
                            break;
                        case LadderModes.Monitor:
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_HIDE
                              | MainWindowEventArgs.FLAG_EDIT));
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_TOGGLE_UP
                              | MainWindowEventArgs.FLAG_SIMULATE));
                            PostIWindowEvent(this, new MainWindowEventArgs(wndMain,
                                MainWindowEventArgs.TYPE_TOGGLE_DOWN
                              | MainWindowEventArgs.FLAG_MONITOR));
                            PostIWindowEvent(this, new UnderBarEventArgs(barStatus,
                                UnderBarStatus.Monitor, Properties.Resources.MainWindow_Monitor));
                            wndMain.LAReplace.Hide();
                            wndMain.LACMonitor.Show();
                            break;
                    }
                    break;
            }
        }

        private void OnProjectModified(object sender, RoutedEventArgs e)
        {
            if (sender is LadderDiagramModel || sender is LadderNetworkModel || sender is LadderUnitModel)
                PostIWindowEvent(this, new InteractionFacadeEventArgs(InteractionFacadeEventArgs.Types.DiagramModified, sender, sender));
            if (sender is FuncBlockModel)
                PostIWindowEvent(this, new InteractionFacadeEventArgs(InteractionFacadeEventArgs.Types.FuncBlockModified, sender, sender));
        }

        #region Simulate

        private void OnSimulateStarted(object sender, RoutedEventArgs e)
        {
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_TOGGLE_DOWN
              | MainWindowEventArgs.FLAG_START));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_START));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_TOGGLE_UP
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_ENABLE
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_BRPO));
        }

        private void OnSimulatePaused(object sender, RoutedEventArgs e)
        {
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_TOGGLE_DOWN
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_TOGGLE_UP
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_START));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_ENABLE
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_START));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_BRPO));
        }

        private void OnSimulateAborted(object sender, RoutedEventArgs e)
        {
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_TOGGLE_DOWN
              | MainWindowEventArgs.FLAG_STOP));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_TOGGLE_UP
              | MainWindowEventArgs.FLAG_START
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_ENABLE
              | MainWindowEventArgs.FLAG_START));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_BRPO));
        }

        private void OnBreakpointPaused(object sender, BreakpointPauseEventArgs e)
        {
            NavigateToBreakpointCursor();
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_PAUSE));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_ENABLE
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_START
              | MainWindowEventArgs.FLAG_BRPO));
        }

        private void OnBreakpointResumed(object sender, BreakpointPauseEventArgs e)
        {
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_DISABLE
              | MainWindowEventArgs.FLAG_START
              | MainWindowEventArgs.FLAG_BRPO));
            PostIWindowEvent(null, new MainWindowEventArgs(wndMain,
                MainWindowEventArgs.TYPE_ENABLE
              | MainWindowEventArgs.FLAG_STOP
              | MainWindowEventArgs.FLAG_PAUSE));
        }
        
        #endregion

        #endregion

    }

    public class InteractionFacadeEventArgs : IWindowEventArgs
    {
        public enum Types { DiagramModified, FuncBlockModified, DeviceModified }
        private Types flags;
        public Types Flags { get { return this.flags; } }
        int IWindowEventArgs.Flags { get { return (int)Flags; } }

        private object targetedobject;
        object IWindowEventArgs.TargetedObject { get { return this.targetedobject; } }

        private object relativeobject;
        object IWindowEventArgs.RelativeObject { get { return this.relativeobject; } }

        public InteractionFacadeEventArgs(Types _flags, object _targetedobject, object _relativeobject)
        {
            flags = _flags;
            targetedobject = _targetedobject;
            relativeobject = _relativeobject;
        }
    }
}
