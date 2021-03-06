﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SamSoarII.ValueModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.AppMain.LadderGraphModule;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using SamSoarII.Extend.FuncBlockModel;
using System.Xml.Linq;
using SamSoarII.Extend.Utility;
using SamSoarII.UserInterface;
using SamSoarII.Simulation.UI.Breakpoint;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using SamSoarII.Communication;
using SamSoarII.AppMain.Project.Helper;
using SamSoarII.AppMain.LadderCommand;
using SamSoarII.Utility;

namespace SamSoarII.AppMain
{
    public enum StatusBarItem
    {
        Empty,
        Program,
        Routine,
        Modbus,
        Func
    }
    public enum CheckRet
    {
        None,
        LadderError,
        FuncblockError,
        CommunicationError
    }
    public class InteractionFacade:INotifyPropertyChanged
    {
        private StatusBarItem _statusBarItem;
        public StatusBarItem StatusBarItem
        {
            get { return _statusBarItem; }
            set
            {
                _statusBarItem = value;
                SetStatusBar(value);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public bool ProjectLoaded
        {
            get { return _projectModel != null; }
        }
        public string ProjectFullFileName { get; set; }
        private ProjectModel _projectModel;
        public ProjectModel ProjectModel
        {
            get { return _projectModel; }
            private set
            {
                _projectModel = value;
                PropertyChanged(this,new PropertyChangedEventArgs("ProjectModel"));
            }
        }
        private bool _isCommentMode;
        public bool IsCommentMode
        {
            get { return _isCommentMode; }
            set
            {
                _isCommentMode = value;
                _projectModel.IsCommentMode = _isCommentMode;
            }
        }
        public bool IsLadderMode
        {
            get
            {
                return (_mainTabControl.ViewMode & MainTabControl.VIEWMODE_LADDER) != 0;
            }
            set
            {
                if (value == true)
                    _mainTabControl.ViewMode |= MainTabControl.VIEWMODE_LADDER;
                if (value == false)
                    _mainTabControl.ViewMode &= ~MainTabControl.VIEWMODE_LADDER;
            }
        }
        public bool IsInstMode
        {
            get
            {
                return (_mainTabControl.ViewMode & MainTabControl.VIEWMODE_INST) != 0;
            }
            set
            {
                if (value == true)
                    _mainTabControl.ViewMode |= MainTabControl.VIEWMODE_INST;
                if (value == false)
                    _mainTabControl.ViewMode &= ~MainTabControl.VIEWMODE_INST;
            }
        }

        private ProjectTreeView _projectTreeView;
        public ProjectTreeView PTView
        {
            get { return _projectTreeView; }
        }
        private MainTabControl _mainTabControl;
        public MainTabControl MainTabControl
        {
            get { return this._mainTabControl; }
        }
        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get { return _mainWindow; }
        }
        private ErrorReportWindow _erwindow;
        public LadderDiagramViewModel CurrentLadder
        {
            get;
            private set;
        }
        private FindWindow _fwindow;
        private ReplaceWindow _rwindow;
        private TextFindWindow _tfwindow;
        private TextReplaceWindow _trwindow;
        private SimuBrpoWindow _bpwindow;
        public SimuBrpoWindow BPWindow
        {
            get { return _bpwindow; }
        }

        public InteractionFacade(MainWindow mainwindow)
        {
            StatusBarHepler.IFacade = this;
            _statusBarItem = StatusBarItem.Empty;
            this._mainWindow = mainwindow;
            mainwindow.InstShortCutOpen += Mainwindow_InstShortCutOpen;
            mainwindow.InsertRowCommand.CanExecute += InsertRowCommand_CanExecute;
            mainwindow.InsertRowCommand.Executed += InsertRowCommand_Executed;
            mainwindow.DeleteRowCommand.CanExecute += DeleteRowCommand_CanExecute;
            mainwindow.DeleteRowCommand.Executed += DeleteRowCommand_Executed;
            mainwindow.CheckLadderCommand.CanExecute += CheckLadderCommand_CanExecute;
            mainwindow.CheckLadderCommand.Executed += CheckLadderCommand_Executed;
            mainwindow.CheckFuncBlockCommand.CanExecute += CheckFuncBlockCommand_CanExecute;
            mainwindow.CheckFuncBlockCommand.Executed += CheckFuncBlock_Executed;
            _mainTabControl = _mainWindow.MainTab;
            _mainTabControl.SelectionChanged += OnTabItemChanged;
            _mainTabControl.CloseTabItem += _mainTabControl_CloseTabItem;
            _mainTabControl.FloatingWinClosed += _mainTabControl_FloatingWinClosed;
            ElementList.NavigateToNetwork += ElementList_NavigateToNetwork;
            _erwindow = new ErrorReportWindow(this);
            _fwindow = new FindWindow(this);
            _rwindow = new ReplaceWindow(this);
            _tfwindow = new TextFindWindow(this);
            _trwindow = new TextReplaceWindow(this);
            _bpwindow = new SimuBrpoWindow(this);
            mainwindow.GD_Find.Children.Add(_fwindow);
            mainwindow.GD_Find.Children.Add(_tfwindow);
            mainwindow.GD_Replace.Children.Add(_rwindow);
            mainwindow.GD_Replace.Children.Add(_trwindow);
            mainwindow.LAErrorList.Content = _erwindow;
            mainwindow.LABreakpoint.Content = _bpwindow;
            CurrentTabChanged += InteractionFacade_CurrentTabChanged;
            PropertyChanged += InteractionFacade_PropertyChanged;
        }
        
        #region StatusBar
        private void _mainTabControl_FloatingWinClosed(object sender, RoutedEventArgs e)
        {
            if (_mainTabControl.TabItemCollection.Count == 0)
                StatusBarItem = StatusBarItem.Program;
        }

        private void _mainTabControl_CloseTabItem(object sender, RoutedEventArgs e)
        {
            if (_mainTabControl.ChildrenCount == 0 && _mainTabControl.TabItemCollection.Count == 0)
                StatusBarItem = StatusBarItem.Program;
            else
            {
                if (_mainTabControl.SelectedItem is MainTabDiagramItem)
                {
                    StatusBarItem = StatusBarItem.Routine;
                    ((MainTabDiagramItem)_mainTabControl.SelectedItem).LDVM_ladder.RetStatusBar();
                }
                if (_mainTabControl.SelectedItem is FuncBlockViewModel)
                {
                    MainWindow.SB_Func.Text = ((FuncBlockViewModel)_mainTabControl.SelectedItem).ProgramName;
                    StatusBarItem = StatusBarItem.Func;
                }
                if (_mainTabControl.SelectedItem is ModbusTableViewModel)
                {
                    StatusBarItem = StatusBarItem.Modbus;
                    ProjectModel.MTVModel.RetStatusBar();
                }
            }
        }
        private void InteractionFacade_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProjectModel")
            {
                if (ProjectModel == null)
                {
                    StatusBarItem = StatusBarItem.Empty;
                    MainWindow.SB_Message.Text = string.Empty;
                    MainWindow.SB_Program.Text = string.Empty;
                    MainWindow.SB_SP_Program.ToolTip = string.Empty;
                }
                else
                {
                    StatusBarItem = StatusBarItem.Program;
                    MainWindow.SB_Program.Text = ProjectModel.ProjectName;
                    if (ProjectFullFileName == string.Empty)
                        MainWindow.SB_Program.ToolTip = string.Empty;
                    else
                    {
                        if (App.CultureIsZH_CN())
                            MainWindow.SB_SP_Program.ToolTip = "路径：" + ProjectFullFileName;
                        else
                            MainWindow.SB_SP_Program.ToolTip = "Path:" + ProjectFullFileName;
                    }
                }
            }
        }
        private void InteractionFacade_CurrentTabChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStatusBar();
        }
        private void SetStatusBar(StatusBarItem statusBarItem)
        {
            switch (statusBarItem)
            {
                case StatusBarItem.Empty:
                    MainWindow.SB_SP_Program.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Routine.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Network.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Modbus.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Func.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Row.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Column.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_X.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Y.Visibility = Visibility.Hidden;
                    break;
                case StatusBarItem.Program:
                case StatusBarItem.Modbus:
                    MainWindow.SB_SP_Program.Visibility = Visibility.Visible;
                    MainWindow.SB_SP_Routine.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Network.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Modbus.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Func.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_X.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Y.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Row.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Column.Visibility = Visibility.Hidden;
                    break;
                case StatusBarItem.Routine:
                    MainWindow.SB_SP_Modbus.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Func.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Program.Visibility = Visibility.Visible;
                    MainWindow.SB_SP_Routine.Visibility = Visibility.Visible;
                    MainWindow.SB_SP_Network.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_X.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Y.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Row.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Column.Visibility = Visibility.Hidden;
                    break;
                case StatusBarItem.Func:
                    MainWindow.SB_SP_Modbus.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Func.Visibility = Visibility.Visible;
                    MainWindow.SB_SP_Program.Visibility = Visibility.Visible;
                    MainWindow.SB_SP_Routine.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Network.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_X.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Y.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Row.Visibility = Visibility.Hidden;
                    MainWindow.SB_SP_Column.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
        private void OnPTVRenamed(object sender, RoutedEventArgs e)
        {
            UpdateStatusBar();
            if(sender is string)
            {
                string programName = (string)sender;
                foreach (var subRoutine in ProjectModel.SubRoutines)
                {
                    if(subRoutine.ProgramName == programName)
                    {
                        subRoutine.UpdateModelMessageByNetwork();
                        break;
                    }
                }
            }
        }
        private void UpdateStatusBar()
        {
            if (_mainTabControl.ChildrenCount == 0 && _mainTabControl.TabItemCollection.Count == 0)
            {
                StatusBarItem = StatusBarItem.Program;
                return;
            }
            if (_mainTabControl.SelectedItem == null)
                StatusBarItem = StatusBarItem.Program;
            if (_mainTabControl.SelectedItem is MainTabDiagramItem || _mainTabControl.SelectedItem is LadderDiagramViewModel)
            {
                MainWindow.SB_Routine.Text = _mainTabControl.SelectedItem is LadderDiagramViewModel ? ((LadderDiagramViewModel)_mainTabControl.SelectedItem).ProgramName : ((MainTabDiagramItem)_mainTabControl.SelectedItem).LDVM_ladder.ProgramName;
                StatusBarItem = StatusBarItem.Routine;
                if (_mainTabControl.SelectedItem is LadderDiagramViewModel)
                    ((LadderDiagramViewModel)_mainTabControl.SelectedItem).RetStatusBar();
                else
                    ((MainTabDiagramItem)_mainTabControl.SelectedItem).LDVM_ladder.RetStatusBar();
            }
            if (_mainTabControl.SelectedItem is FuncBlockViewModel)
            {
                MainWindow.SB_Func.Text = ((FuncBlockViewModel)_mainTabControl.SelectedItem).ProgramName;
                StatusBarItem = StatusBarItem.Func;
            }
            if (_mainTabControl.SelectedItem is ModbusTableViewModel)
            {
                StatusBarItem = StatusBarItem.Modbus;
                ProjectModel.MTVModel.RetStatusBar();
            }
            SetMessage(Properties.Resources.Ready);
        }
        public void SetMessage(string message)
        {
            MainWindow.SB_Message.Text = message;
        }
        private void _projectModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Ladder")
            {
                SetMessage(Properties.Resources.Ladder_Changed);
                return;
            }
            if (e.PropertyName == "FuncBlock")
            {
                SetMessage(Properties.Resources.Func_Changed);
                return;
            }
            if (e.PropertyName == "ProjectProperty")
            {
                SetMessage(Properties.Resources.Project_Config_Changed);
                return;
            }
            if (e.PropertyName == "ProjectModel")
            {
                SetMessage(Properties.Resources.Project_Changed);
                return;
            }
            if (e.PropertyName == "ModBus")
            {
                SetMessage(Properties.Resources.ModBus_Changed);
                return;
            }
        }
        #endregion
        
        #region Check

        private void CheckLadderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckLadder(true);
        }
        public void CheckLadderWork(LoadingWindowHandle handle, bool showreport,ref bool result)
        {
            List<ErrorReportElement> weinsts = new List<ErrorReportElement>();
            IEnumerable<ErrorReportElement> _weinsts = null;
            InstructionDiagramViewModel.CheckInitialize();
            _projectModel.MainRoutine.IsInterruptCalled = false;
            foreach (LadderDiagramViewModel ldvmodel in _projectModel.SubRoutines)
            {
                ldvmodel.IsInterruptCalled = false;
            }
            _weinsts = _projectModel.MainRoutine.IDVModel.Check();
            weinsts.AddRange(_weinsts);
            foreach (LadderDiagramViewModel ldvmodel in _projectModel.SubRoutines)
            {
                _weinsts = ldvmodel.IDVModel.Check();
                weinsts.AddRange(_weinsts);
            }
            _weinsts = _projectModel.MainRoutine.IDVModel.CheckForInterrrupt();
            weinsts.AddRange(_weinsts);
            foreach (LadderDiagramViewModel ldvmodel in _projectModel.SubRoutines)
            {
                _weinsts = ldvmodel.IDVModel.CheckForInterrrupt();
                weinsts.AddRange(_weinsts);
            }
            int ecount = 0;
            int wcount = 0;
            
            foreach (ErrorReportElement inst in weinsts)
            {
                switch (inst.Status)
                {
                    case PLCOriginInst.STATUS_WARNING:
                        wcount++;
                        break;
                    case PLCOriginInst.STATUS_ERROR:
                        ecount++;
                        break;
                }
            }
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            errorMessages.Add(LadderGraphCheckModule.Execute(_projectModel.MainRoutine));
            foreach (var routine in _projectModel.SubRoutines)
            {
                errorMessages.Add(LadderGraphCheckModule.Execute(routine));
            }
            for (int i = 0 ; i < errorMessages.Count;i++ )
            {
                if (errorMessages[i].Error == ErrorType.None
             || errorMessages[i].Error == ErrorType.InstPair)
                {
                    if (weinsts.Count() > 0)
                    {
                        result = (ecount == 0);
                        if ((showreport || !result) && i == errorMessages.Count - 1)
                        {
                            if (App.CultureIsZH_CN())
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
                        if(i == errorMessages.Count - 1)
                        {
                            if (showreport)
                                ShowMessage(Properties.Resources.Program_Correct, handle, false, true);
                            else
                                handle.Abort();
                            result = true;
                        }
                    }
                    if(i == errorMessages.Count - 1)
                    {
                        _erwindow.Mode = ErrorReportWindow.MODE_LADDER;
                        _erwindow.Update(weinsts);
                        if (!result)
                            _mainWindow.LACErrorList.Show();
                    }
                    //else
                    //    _projectModel.IsModify = false;
                }
                else if (errorMessages[i].Error == ErrorType.Empty)
                {
                    var num = errorMessages[i].RefNetworks.First().NetworkNumber;
                    var name = errorMessages[i].RefNetworks.First().LDVModel.ProgramName;
                    NavigateToNetwork(new NavigateToNetworkEventArgs(num,name,0,0));
                    if (App.CultureIsZH_CN())
                        ShowMessage(string.Format("程序{0}的网络{1}元素为空!", name, num), handle, true, true);
                    else
                        ShowMessage(string.Format("Network {0} in {1} is empty!", num, name), handle, true, true);
                    result = false;
                    break;
                }
                else
                {
                    errorMessages[i].RefNetworks.First().AcquireSelectRect();
                    CurrentLadder.SelectionRect.X = errorMessages[i].RefNetworks.Last().ErrorModels.First().X;
                    CurrentLadder.SelectionRect.Y = errorMessages[i].RefNetworks.Last().ErrorModels.First().Y;
                    CurrentLadder.HScrollToRect(CurrentLadder.SelectionRect.X);
                    CurrentLadder.VScrollToRect(errorMessages[i].RefNetworks.First().NetworkNumber, CurrentLadder.SelectionRect.Y);
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
            handle.Completed = true;
        }
        public bool CheckLadder(bool showreport = false)
        {
            bool result = false;
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.LadderDiagram_check);
            StatusBarHepler.UpdateMessageAsync(Properties.Resources.LadderDiagram_check);
            MainWindow.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                handle.Start();
                _projectModel.AutoInstManager.Pause();
                while (_projectModel.AutoInstManager.IsActive)
                    Thread.Sleep(10);
                CheckLadderWork(handle, showreport,ref result);
                _projectModel.AutoInstManager.Start();
            });
            while (!handle.Completed)
            {
                Thread.Sleep(10);
            }
            if (!showreport)
            {
                if (result)
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.Ladder_Correct);
                else
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.Ladder_Error);
            }
            return result;
        }

        private void ShowMessage(string message,LoadingWindowHandle handle,bool isError,bool isLadder)
        {
            if (isLadder)
            {
                if (isError)
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.Ladder_Error);
                else
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.Ladder_Correct);
            }
            else
            {
                if (!isError)
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.FuncBlock_Correct);
                else
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.FuncBlock_Error);
            }
            handle.Abort();
            LocalizedMessageBox.Show(message,LocalizedMessageIcon.Information);
        }
        private void CheckLadderCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentLadder != null;
        }
        private void CheckFuncBlockCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentLadder != null;
        }
        private void CheckFuncBlock_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckFuncBlock(true);
        }
        private void CheckFuncBlockWork(LoadingWindowHandle handle,bool showreport,ref bool result)
        {
            List<string> cfiles = new List<string>();
            List<string> ofiles = new List<string>();
            List<FuncBlockViewModel> fbvmodels = new List<FuncBlockViewModel>();
            Process cmd = null;
            string stdout = null;
            string stderr = null;
            Match m1 = null;
            Match m2 = null;
            string message = null;
            int sline = 0;
            int line = 0;
            int column = 0;
            ErrorReportElement_FB ewele = null;
            List<ErrorReportElement_FB> eweles = new List<ErrorReportElement_FB>();
            foreach (FuncBlockViewModel fbvmodel in _projectModel.FuncBlocks)
            {
                //string hfile = SamSoarII.Utility.FileHelper.GetTempFile(".h");
                string cfile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
                string ofile = SamSoarII.Utility.FileHelper.GetTempFile(".o");
                fbvmodels.Add(fbvmodel);
                cfiles.Add(cfile);
                ofiles.Add(ofile);
                StreamWriter cw = new StreamWriter(cfile);
                cw.Write("typedef int BIT;\n");
                cw.Write("typedef int WORD;\n");
                cw.Write("typedef long DWORD;\n");
                cw.Write("typedef double FLOAT;\n");
                foreach (FuncModel fmodel in fbvmodel.Funcs)
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

                cw.Write(fbvmodel.Code);
                cw.Close();
                sline = 4 + fbvmodel.Funcs.Count();
                cmd = new Process();
                cmd.StartInfo.FileName =
                    String.Format(@"{0:s}\Compiler\arm\bin\arm-none-eabi-gcc",
                        FileHelper.AppRootPath);
                cmd.StartInfo.Arguments = string.Format("-c {0} -o {1}", cfile, ofile);
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.Start();
                cmd.WaitForExit();
                stdout = cmd.StandardOutput.ReadToEnd();
                stderr = cmd.StandardError.ReadToEnd();
                m1 = Regex.Match(stderr, @"[^\s](.+):(.+):(.+): error: (.+)\r\n");
                m2 = Regex.Match(stderr, @"[^\s](.+):(.+):(.+): warning: (.+)\r\n");
                while (m1 != null && m1.Success)
                {
                    message = m1.Groups[4].Value;
                    line = int.Parse(m1.Groups[2].Value) - sline;
                    column = int.Parse(m1.Groups[3].Value);
                    ewele = new ErrorReportElement_FB
                    (
                        ErrorReportElement_FB.STATUS_ERROR,
                        message,
                        fbvmodel,
                        line,
                        column
                    );
                    eweles.Add(ewele);
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
                        fbvmodel,
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
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
            stdout = cmd.StandardOutput.ReadToEnd();
            stderr = cmd.StandardError.ReadToEnd();
            m1 = Regex.Match(stderr, @"\s(.+):\((.+)\): (.+)\r\n");
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
                        FuncBlockViewModel _fbvmodel = fbvmodels[_cfile_id];
                        ewele = new ErrorReportElement_FB
                        (
                            ErrorReportElement_FB.STATUS_ERROR,
                            message,
                            _fbvmodel,
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
            result = (ecount == 0);
            _erwindow.Mode = ErrorReportWindow.MODE_FUNC;
            _erwindow.Update(eweles);
            if (showreport || !result)
            {
                if (ecount == 0 && wcount == 0)
                    ShowMessage(Properties.Resources.Function_Block_Correct, handle, false, false);
                else
                {
                    if (App.CultureIsZH_CN())
                        ShowMessage(String.Format("函数块发生{0:d}处错误，{1:d}处警告。", ecount, wcount), handle, true, false);
                    else
                        ShowMessage(String.Format("There are {0} errors and {1} warnings in the funcblock.", ecount, wcount), handle, true, false);
                    _mainWindow.LACErrorList.Show();
                }
            }
            else
                handle.Abort();
            handle.Completed = true;
        }
        public bool CheckFuncBlock(bool showreport = false)
        {
            bool result = true;
            if (_projectModel.FuncBlocks.Count() == 0)
            {
                return result;
            }
            StatusBarHepler.UpdateMessageAsync(Properties.Resources.Funcblock_Check);
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Funcblock_Check);
            MainWindow.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                handle.Start();
                CheckFuncBlockWork(handle, showreport, ref result);
            });
            while (!handle.Completed)
            {
                Thread.Sleep(10);
            }
            if (!showreport)
            {
                if (result)
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.FuncBlock_Correct);
                else
                    StatusBarHepler.UpdateMessageAsync(Properties.Resources.FuncBlock_Error);
            }
            return result;
        }

        #endregion

        #region Routine
        public void CreateRoutine
        (
            ProjectTreeViewEventArgs e = null
        )
        {
            LadderDiagramViewModel ldvmodel = new LadderDiagramViewModel(String.Empty, _projectModel);
            _projectModel.Add(ldvmodel);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_ROUTINE
                  | ProjectTreeViewEventArgs.FLAG_CREATE,
                    ldvmodel, null);
            }
            else
                e.RelativeObject = ldvmodel;
            _mainWindow.LACProj.Show();
            PTVEvent(this, e);
        }
        public void RemoveRoutine
        (
            LadderDiagramViewModel ldvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            _projectModel.Remove(ldvmodel);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_ROUTINE
                  | ProjectTreeViewEventArgs.FLAG_REMOVE,
                    ldvmodel, null);
            }
            else
            {
                e.RelativeObject = ldvmodel;
            }
            _mainTabControl.CloseItem(ldvmodel);
            PTVEvent(this, e);
        }
        public void ReplaceRoutine
        (
            LadderDiagramViewModel ldvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_ROUTINE
                  | ProjectTreeViewEventArgs.FLAG_REPLACE,
                    ldvmodel, null);
            }
            else
            {
                e.RelativeObject = ldvmodel;
            }
            _mainTabControl.RenameItem(ldvmodel);
            PTVEvent(this, e);
        }
        #endregion

        #region Network

        public void CreateNetwork
        (
            LadderDiagramViewModel ldvmodel,
            int networknumber,
            ProjectTreeViewEventArgs e = null
        )
        {
            LadderNetworkViewModel lnvmodel = null;
            lnvmodel = new LadderNetworkViewModel(ldvmodel, networknumber);
            InsertNetwork(ldvmodel, lnvmodel, e);
        }

        public void InsertNetwork
        (
            LadderDiagramViewModel ldvmodel,
            LadderNetworkViewModel lnvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            ldvmodel.IFAddNetwork(lnvmodel);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_NETWORK
                  | ProjectTreeViewEventArgs.FLAG_CREATE,
                    lnvmodel, null);
            }
            else
            {
                e.RelativeObject = lnvmodel;
            }
            PTVEvent(this, e);
        }

        public void RemoveNetwork
        (
            LadderNetworkViewModel lnvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            ldvmodel.IFRemoveNetwork(lnvmodel);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_NETWORK
                  | ProjectTreeViewEventArgs.FLAG_REMOVE,
                    lnvmodel, null);
            }
            else
            {
                e.RelativeObject = lnvmodel;
            }
            PTVEvent(this, e);
        }

        public void ReplaceNetwork
        (
            LadderNetworkViewModel lnvmodel_old,
            LadderNetworkViewModel lnvmodel_new,
            ProjectTreeViewEventArgs e = null
        )
        {
            LadderDiagramViewModel ldvmodel = lnvmodel_old.LDVModel;
            ldvmodel.IFReplaceNetwork(lnvmodel_old, lnvmodel_new);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_NETWORK
                  | ProjectTreeViewEventArgs.FLAG_REPLACE,
                    lnvmodel_new, lnvmodel_old);
            }
            else
            {
                e.RelativeObject = lnvmodel_new;
                e.TargetedObject = lnvmodel_old;
            }
            PTVEvent(this, e);
        }

        public void ReplaceNetwork
        (
            LadderNetworkViewModel lnvmodel_old,
            int number_new,
            ProjectTreeViewEventArgs e = null
        )
        {
            LadderDiagramViewModel ldvmodel = lnvmodel_old.LDVModel;
            ldvmodel.IFReplaceNetwork(lnvmodel_old, number_new);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_NETWORK
                  | ProjectTreeViewEventArgs.FLAG_REPLACE,
                    number_new, lnvmodel_old);
            }
            else
            {
                e.RelativeObject = number_new;
                e.TargetedObject = lnvmodel_old;
            }
            PTVEvent(this, e);
        }

        public void ReplaceNetwork
        (
            LadderDiagramViewModel ldvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_NETWORK
                  | ProjectTreeViewEventArgs.FLAG_REPLACE,
                    ldvmodel, null);
            }
            else
            {
                e.RelativeObject = ldvmodel;
            }
            PTVEvent(this, e);
        }

        #region FuncBlock
        public void CreateFuncBlock
        (
            ProjectTreeViewEventArgs e = null
        )
        {
            FuncBlockViewModel fbvmodel = new FuncBlockViewModel(String.Empty, ProjectModel);
            _projectModel.Add(fbvmodel);
            _mainWindow.LACProj.Show();
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_FUNCBLOCK
                  | ProjectTreeViewEventArgs.FLAG_CREATE,
                    fbvmodel, null);
            }
            else
            {
                e.RelativeObject = fbvmodel;
            }
            PTVEvent(this, e);
        }
        public void RemoveFuncBlock
        (
            FuncBlockViewModel fbvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            _projectModel.Remove(fbvmodel);
            _mainTabControl.CloseItem(fbvmodel);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_FUNCBLOCK
                  | ProjectTreeViewEventArgs.FLAG_REMOVE,
                    fbvmodel, null);
            }
            else
            {
                e.RelativeObject = fbvmodel;
            }
            PTVEvent(this, e);
        }
        public void ReplaceFuncBlock
        (
            FuncBlockViewModel fbvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            _mainTabControl.RenameItem(fbvmodel);
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_FUNCBLOCK
                  | ProjectTreeViewEventArgs.FLAG_REPLACE,
                    fbvmodel, null);
            }
            else
            {
                e.RelativeObject = fbvmodel;
            }
            PTVEvent(this, e);
        }
        #endregion

        #region Modbus
        public void CreateModbus
        (
            ProjectTreeViewEventArgs e = null
        )
        {
            _mainTabControl.ShowItem(_projectModel.MTVModel);
            _projectModel.MTVModel.AddModel();
        }

        public void RemoveModbus
        (
            ModbusTableModel mtmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            _mainTabControl.ShowItem(_projectModel.MTVModel);
            _projectModel.MTVModel.RemoveModel(mtmodel);
        }

        public void ReplaceModbus
        (
            ModbusTableModel mtmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            _mainTabControl.ShowItem(_projectModel.MTVModel);
            _projectModel.MTVModel.UpdateList();
        }
        #endregion

        #endregion

        #region Modification

        private void DeleteRowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentLadder.SelectionStatus == SelectStatus.SingleSelected)
            {
                CurrentLadder.NetworkRemoveRow(CurrentLadder.SelectRectOwner,CurrentLadder.SelectionRect.Y);
            }
            else
            {
                if (CurrentLadder.CrossNetState == CrossNetworkState.NoCross)
                {
                    if (CurrentLadder.SelectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit)
                    {
                        CurrentLadder.NetworkRemoveRow(CurrentLadder.SelectStartNetwork, CurrentLadder.SelectStartNetwork.SelectAreaFirstY);
                    }
                    else
                    {
                        CurrentLadder.NetworkRemoveRows(CurrentLadder.SelectStartNetwork,Math.Min(CurrentLadder.SelectStartNetwork.SelectAreaFirstY,CurrentLadder.SelectStartNetwork.SelectAreaSecondY),Math.Abs(CurrentLadder.SelectStartNetwork.SelectAreaFirstY - CurrentLadder.SelectStartNetwork.SelectAreaSecondY) + 1);
                    }
                }
            }
        }
        private void DeleteRowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ProjectLoaded && CurrentLadder != null && CurrentLadder.SelectionStatus != SelectStatus.Idle && CurrentLadder.CrossNetState == CrossNetworkState.NoCross)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void InsertRowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentLadder.NetworkAddRow(CurrentLadder.SelectRectOwner,CurrentLadder.SelectionRect.Y + 1);
        }
        private void InsertRowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ProjectLoaded && CurrentLadder != null && CurrentLadder.SelectionStatus == SelectStatus.SingleSelected)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void Mainwindow_InstShortCutOpen(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int catalogId = int.Parse(button.Tag as string);
            switch (CurrentLadder.SelectionStatus)
            {
                case SelectStatus.Idle:
                    break;
                case SelectStatus.SingleSelected:
                    ReplaceElementsExecute(catalogId);
                    break;
                case SelectStatus.MultiSelecting:
                    break;
                case SelectStatus.MultiSelected:
                    if (catalogId == 10 || catalogId == 11)
                    {
                        LadderDiagramRemoveElementsCommand command = new LadderDiagramRemoveElementsCommand();
                        if (catalogId == 10)
                        {
                            command.AddCommand(new NetworkRemoveElementsCommand(CurrentLadder.SelectStartNetwork, CurrentLadder.SelectStartNetwork.GetSelectedHLines(), new List<VerticalLineViewModel>()));
                            foreach (var network in CurrentLadder.SelectAllNetworks)
                                command.AddCommand(new NetworkRemoveElementsCommand(network, network.GetSelectedHLines(), new List<VerticalLineViewModel>()));
                        }
                        else
                        {
                            command.AddCommand(new NetworkRemoveElementsCommand(CurrentLadder.SelectStartNetwork, new List<BaseViewModel>(), CurrentLadder.SelectStartNetwork.GetSelectedVerticalLines()));
                            foreach (var network in CurrentLadder.SelectAllNetworks)
                                command.AddCommand(new NetworkRemoveElementsCommand(network, new List<BaseViewModel>(), network.GetSelectedVerticalLines()));
                        }
                        CurrentLadder.CommandExecute(command);
                    }
                    else
                    {
                        CurrentLadder.SelectionRect.X = CurrentLadder.SelectStartNetwork.SelectAreaFirstX;
                        CurrentLadder.SelectionRect.Y = CurrentLadder.SelectStartNetwork.SelectAreaFirstY;
                        CurrentLadder.SelectStartNetwork.AcquireSelectRect();
                        ReplaceElementsExecute(catalogId);
                    }
                    break;
                default:
                    break;
            }
        }
        private void SelectionRectRight()
        {
            if (CurrentLadder.SelectionRect.X < GlobalSetting.LadderXCapacity - 1)
            {
                CurrentLadder.SelectionRect.X++;
            }
        }
        private void SelectionRectDown()
        {
            if (CurrentLadder.SelectionRect.Y + 1 > CurrentLadder.SelectRectOwner.RowCount - 1)
            {
                CurrentLadder.SelectRectOwner.RowCount++;
            }
            CurrentLadder.SelectionRect.Y++;
        }
        private void ReplaceElementsExecute(int catalogId)
        {
            if (catalogId != 10 && catalogId != 11 && catalogId != 101)
            {
                CurrentLadder.ReplaceSingleElement(catalogId);
                if (catalogId == 206 || catalogId == 207 || catalogId == 208 || catalogId == 100)
                    SelectionRectRight();
            }
            else
            {
                if (catalogId == 101 && CurrentLadder.SelectionRect.X > 0)
                {
                    CurrentLadder.ReplaceSingleVerticalLine(CurrentLadder.SelectRectOwner, new VerticalLineViewModel() { X = CurrentLadder.SelectionRect.X - 1, Y = CurrentLadder.SelectionRect.Y });
                    SelectionRectDown();
                }
                else if (catalogId == 10)
                {
                    if (CurrentLadder.SelectionRect.CurrentElement is HorizontalLineViewModel)
                    {
                        CurrentLadder.RemoveSingleElement(
                            CurrentLadder.SelectRectOwner, 
                            CurrentLadder.SelectionRect.CurrentElement);
                    }
                    SelectionRectRight();
                }
                else
                {
                    VerticalLineViewModel vlvmodel =
                            CurrentLadder.SelectRectOwner.GetVerticalLineByPosition(
                                CurrentLadder.SelectionRect.X - 1,
                                CurrentLadder.SelectionRect.Y);
                    if (vlvmodel != null)
                    {
                        CurrentLadder.RemoveSingleVerticalLine(
                            CurrentLadder.SelectRectOwner, vlvmodel);
                        SelectionRectDown();
                    }
                }
            }
        }
        #endregion

        #region Navigate

        private void ElementList_NavigateToNetwork(NavigateToNetworkEventArgs e)
        {
            NavigateToNetwork(e);
        }
        public void NavigateToNetwork(NavigateToNetworkEventArgs e)
        {
            LadderDiagramViewModel tempItem;
            if (ProjectModel.MainRoutine.ProgramName == e.RefLadderName)
                tempItem = ProjectModel.MainRoutine;
            else
                tempItem = ProjectModel.SubRoutines.Where(x => { return x.ProgramName == e.RefLadderName; }).First();
            var network = tempItem.GetNetworkByNumber(e.NetworkNum);
            if (network.IsMasked) return;
            if (!network.ladderExpander.IsExpand)
                network.ladderExpander.IsExpand = true;
            network.AcquireSelectRect();
            tempItem.SelectionRect.X = e.X;
            tempItem.SelectionRect.Y = e.Y;
            _mainTabControl.ShowItem(tempItem);
            tempItem.NavigateToNetworkByNum(e.NetworkNum);
        }
        public bool NavigateToNetwork(BaseViewModel bvmodel)
        {
            LadderNetworkViewModel lnvmodel = _projectModel.GetNetwork(bvmodel);
            if (lnvmodel == null|| lnvmodel.IsMasked) return false;
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            NavigateToNetwork(new NavigateToNetworkEventArgs(
                lnvmodel.NetworkNumber, ldvmodel.ProgramName, bvmodel.X, bvmodel.Y));
            return true;
        }
        public void NavigateToFuncBlock(FuncBlockViewModel fbvmodel, int line, int column)
        {
            _mainTabControl.ShowItem(fbvmodel);
            fbvmodel.SetPosition(line, column);
        }
        public void NavigateToFuncBlock(FuncBlockViewModel fbvmodel, int offset)
        {
            _mainTabControl.ShowItem(fbvmodel);
            fbvmodel.SetOffset(offset);
        }

        #endregion

        #region Project
        
        public void CreateProject(string name, string fullFileName)
        {
            CloseCurrentProject();
            ProjectModel = new ProjectModel(name);
            _projectModel.IFacade = this;
            _projectModel.autoSavedManager = new AutoSavedManager(this);
            _projectModel.AutoInstManager = new AutoInstManager(this);
            if (fullFileName != string.Empty)
                ProjectFileManager.Update(name, fullFileName);
            ValueAliasManager.Clear();
            ValueCommentManager.Clear();
            InstructionCommentManager.Clear();
            _projectTreeView = new ProjectTreeView(_projectModel);
            _projectTreeView.TabItemOpened += OnTabOpened;
            _projectTreeView.PTVHandle += OnGotPTVHandle;
            _projectTreeView.PTVRenamed += OnPTVRenamed;
            _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
            _mainTabControl.Reset();
            _mainTabControl.ShowItem(_projectModel.MainRoutine);
            MainWindow.ResetDock();
            MainWindow.ResetToolBar(false, _mainTabControl.ViewMode);
            CurrentLadder = _projectModel.MainRoutine;
            _mainWindow.SetProjectTreeView(_projectTreeView);
            _mainWindow.SetProjectMonitor(_projectModel.MMonitorManager.MMWindow);
            ProjectFullFileName = fullFileName;
            _projectModel.PropertyChanged += _projectModel_PropertyChanged;
            if (GlobalSetting.IsSavedByTime)
                _projectModel.autoSavedManager.Start();
            _projectModel.AutoInstManager.Start();
            ProjectModel.IsModify = false;
        }
        
        public void CloseCurrentProject()
        {
            if (_projectModel != null)
            {
                _projectModel.PropertyChanged -= _projectModel_PropertyChanged;
                _projectModel.Dispose();
                ProjectModel = null;
            }
            if (_projectTreeView != null)
            {
                _projectTreeView.TabItemOpened -= OnTabOpened;
                _projectTreeView.PTVHandle -= OnGotPTVHandle;
                _projectTreeView.PTVRenamed -= OnPTVRenamed;
                _projectTreeView.NavigatedToNetwork -= ElementList_NavigateToNetwork;
                _projectTreeView.Dispose();
                _projectTreeView = null;
            }
            ProjectFullFileName = string.Empty;
            _mainWindow.ResetDock();
            _mainWindow.ResetToolBar(true);
            _mainWindow.ClearProjectTreeView();
            _mainWindow.ClearProjectMonitor();
            _mainTabControl.Reset();
            MainWindow.LACProj.Hide();
            _erwindow.Initialize();
            _fwindow.Initialize();
            _rwindow.Initialize();
            _tfwindow.Initialize();
            _trwindow.Initialize();
            GC.Collect();
        }
        public void SaveProject()
        {
            if (ProjectFullFileName != string.Empty)
            {
                SaveAsProject(ProjectFullFileName);
                ProjectFileManager.Update(FileHelper.GetFileName(ProjectFullFileName), ProjectFullFileName);
            }
        }
        public void SaveAsProject(string fileName)
        {
            XDocument xdoc = new XDocument();
            XElement xele_r = new XElement("Root");
            xdoc.Add(xele_r);
            XElement xele_p = new XElement("Project");
            _projectModel.Save(xele_p);
            xele_r.Add(xele_p);
            XElement xele_ptv = new XElement("ProjectTreeView");
            _projectTreeView.Save(xele_ptv);
            xele_r.Add(xele_ptv);
            xdoc.Save(fileName);
            _projectModel.IsModify = false;
        }
        private void LoadProjectWork(LoadingWindowHandle handle)
        {
            string _pname = ProjectFullFileName;
            CloseCurrentProject();
            ProjectFullFileName = _pname;
            InstructionCommentManager.UpdateAllComment();
            ProjectModel = ProjectHelper.LoadProject(ProjectFullFileName, new ProjectModel(String.Empty));
            XDocument xdoc = XDocument.Load(ProjectFullFileName);
            XElement xele_r = xdoc.Element("Root");
            XElement xele_rtv = xele_r.Element("ProjectTreeView");
            _projectModel.IFacade = this;
            //在这里更新用户可能自己更改的文件名称
            _pname = FileHelper.GetFileName(ProjectFullFileName);
            if (_pname != _projectModel.ProjectName) _projectModel.ProjectName = _pname;
            _projectModel.autoSavedManager = new AutoSavedManager(this);
            _projectModel.AutoInstManager = new AutoInstManager(this);
            _projectModel.PropertyChanged += _projectModel_PropertyChanged;
            //_projectModel.EleInitializeData = null;
            _projectTreeView = new ProjectTreeView(_projectModel, xele_rtv);
            _projectTreeView.TabItemOpened += OnTabOpened;
            _projectTreeView.PTVHandle += OnGotPTVHandle;
            _projectTreeView.PTVRenamed += OnPTVRenamed;
            _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
            _mainWindow.ResetDock();
            _mainWindow.ResetToolBar(false, _mainTabControl.ViewMode);
            _mainWindow.SetProjectTreeView(_projectTreeView);
            _mainWindow.SetProjectMonitor(_projectModel.MMonitorManager.MMWindow);
            _mainWindow.ElemInitWind.LoadElementsByXElement(_projectModel.EleInitializeData);
            _mainTabControl.Reset();
            _mainTabControl.ShowItem(_projectModel.MainRoutine);
            CurrentLadder = _projectModel.MainRoutine;
            ProjectFileManager.Update(_projectModel.ProjectName, ProjectFullFileName);
            if (GlobalSetting.IsSavedByTime)
                _projectModel.autoSavedManager.Start();
            _projectModel.AutoInstManager.Start();
            _projectModel.LadderMode = LadderMode.Edit;
            handle.Abort();
            handle.Completed = true;
            ProjectModel.IsModify = false;
        }
        public bool LoadProject(string fileName)
        {
            ProjectFullFileName = fileName;
            LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Project_Load);
            StatusBarHepler.IsLoading = true;
            StatusBarHepler.UpdateMessageAsync(Properties.Resources.Project_Preparing);
            MainWindow.Dispatcher.Invoke(DispatcherPriority.Background,(ThreadStart)delegate()
            {
                handle.Start();
                LoadProjectWork(handle);
            });
            while (!handle.Completed)
            {
                Thread.Sleep(10);
            }
            StatusBarHepler.IsLoading = false;
            return true;
        }
        public int DownloadProject()
        {
            if (!CheckFuncBlock(false))
            {
                return DownloadHelper.DOWNLOAD_FUNCBLOCK_ERROR;
            }
            if (!CheckLadder(false))
            {
                return DownloadHelper.DOWNLOAD_LADDER_ERROR;
            }
            CommunicationParams cparams =
                (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
            GenerateHelper.GenerateFinal(_projectModel, "libF103PLC.a");
            DownloadHelper.Write(_projectModel, cparams.DownloadOption);
            using (CommunicationSettingDialog dialog = new CommunicationSettingDialog(
                cparams, CommunicationSettingDialogMode.DOWNLOAD))
            {
                BaseSetting baseSetting = dialog.GetBaseSetting();
                baseSetting.DataLen = DownloadHelper.DataLen;
                baseSetting.SettingButtonClick += (sender1, e1) =>
                {
                    CommunicationsettingParamsDialog dialog1 = new CommunicationsettingParamsDialog(
                        (CommunicationParams)(ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]));
                    dialog1.ShowDialog();
                };
                dialog.CommunicationTest += (sender, e) => 
                {
                    var _ret = CommunicationTest();
                    switch (_ret)
                    {
                        case CheckRet.None:
                            LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Success,LocalizedMessageIcon.Information);
                            break;
                        case CheckRet.LadderError:
                            LocalizedMessageBox.Show(Properties.Resources.Ladder_Error, LocalizedMessageIcon.Error);
                            break;
                        case CheckRet.FuncblockError:
                            LocalizedMessageBox.Show(Properties.Resources.FuncBlock_Error, LocalizedMessageIcon.Error);
                            break;
                        case CheckRet.CommunicationError:
                            LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                            break;
                        default:
                            break;
                    }
                };
                baseSetting.ModifyButtonClick += (sender2, e2) =>
                {
                    using (ProjectPropertyDialog dialog2 = new ProjectPropertyDialog(_projectModel))
                    {
                        dialog2.EnsureButtonClick += (sender1, e1) =>
                        {
                            dialog2.Save();
                            dialog2.Close();
                        };
                        dialog2.ShowDialog();
                    }
                };
                dialog.Ensure += (sender3, e3) =>
                {
                    var _ret = CommunicationTest();
                    if (_ret == CheckRet.None)
                    {
                        CommunicationParams paras = (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
                        LoadingWindowHandle handle = new LoadingWindowHandle(Properties.Resources.Project_Download);
                        StatusBarHepler.UpdateMessageAsync(Properties.Resources.Downloading);
                        MainWindow.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            handle.Start();
                            bool ret = DownloadHelper.Download(paras.IsCOMLinked
                                ? (ICommunicationManager)(_projectModel.PManager)
                                : (ICommunicationManager)(_projectModel.UManager));
                            handle.Abort();
                            handle.Completed = true;
                            if (!ret)
                            {
                                StatusBarHepler.UpdateMessageAsync(Properties.Resources.Download_Fail);
                                LocalizedMessageBox.Show(Properties.Resources.Download_Fail, LocalizedMessageIcon.Information);
                            }
                            else
                            {
                                StatusBarHepler.UpdateMessageAsync(Properties.Resources.MessageBox_Download_Successd);
                                LocalizedMessageBox.Show(Properties.Resources.MessageBox_Download_Successd, LocalizedMessageIcon.Information);
                            }
                        });
                        while (!handle.Completed)
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else
                    {
                        LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                    }
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
            return 0;
        }
        public int UploadProject()
        {
            UploadHelper.Read(ref _projectModel);
            LoadProject(String.Format(@"{0:s}\upload.xml", FileHelper.AppRootPath));
            return 0;
        }
        public int SimulateProject()
        {
            if (!CheckFuncBlock(false))
            {
                return SimulateHelper.SIMULATE_FUNCBLOCK_ERROR;
            }
            if (!CheckLadder(false))
            {
                return SimulateHelper.SIMULATE_LADDER_ERROR;
            }
            int ret = SimulateHelper.Simulate(_projectModel);
            return ret;
        }
        public CheckRet CommunicationTest()
        {
            if (!CheckLadder(false))
            {
                return CheckRet.LadderError;
            }
            CommunicationParams paras = (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
            if (paras.IsCOMLinked)
            {
                _projectModel.MMonitorManager.CManager = _projectModel.PManager;
                if (paras.IsAutoCheck
                 && !_projectModel.PManager.AutoCheck())
                {
                    return CheckRet.CommunicationError;
                }
                if (_projectModel.PManager.Start() != 0)
                {
                    return CheckRet.CommunicationError;
                }
            }
            else
            {
                _projectModel.MMonitorManager.CManager = _projectModel.UManager;
                if (_projectModel.UManager.Start() != 0)
                {
                    return CheckRet.CommunicationError;
                }
            }
            return CheckRet.None;
        }
        private void MonitorAbort()
        {
            _projectModel.MMonitorManager.MMWindow.OnStopCommandExecute(null,null);
            Thread.Sleep(10);
            _projectModel.UManager.Abort();
            _projectModel.PManager.Abort();
        }
        public void EditProject()
        {
            MonitorAbort();
            _projectModel.LadderMode = LadderMode.Edit;
        }

        #endregion

        #region Event handler

        private void OnTabOpened(object sender, ShowTabItemEventArgs e)
        {
            switch (e.Type)
            {
                case TabType.Program:
                    IProgram prog = sender as IProgram;
                    _mainTabControl.ShowItem(prog);
                    break;
                case TabType.Modbus:
                    ITabItem itab = _projectModel.MTVModel;
                    _mainTabControl.ShowItem(itab);
                    break;
            }
        }

        public event SelectionChangedEventHandler CurrentTabChanged = delegate { };
        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainTabControl.SelectedItem is LadderDiagramViewModel)
            {
                var ldmodel = _mainTabControl.SelectedItem as LadderDiagramViewModel;
                if (ldmodel != null)
                {
                    CurrentLadder = ldmodel;
                }
            }
            CurrentTabChanged(_mainTabControl, e);
        }

        public event ProjectTreeViewEventHandler PTVEvent = delegate { };

        private void OnGotPTVHandle(object sender, ProjectTreeViewEventArgs e)
        {
            LadderDiagramViewModel ldvmodel = null;
            LadderNetworkViewModel lnvmodel = null;
            FuncBlockViewModel fbvmodel = null;
            ModbusTableModel mtmodel = null;
            switch (e.Flags & 0xf)
            {
                case ProjectTreeViewEventArgs.TYPE_ROUTINE:
                    if (e.RelativeObject is LadderDiagramViewModel)
                    {
                        ldvmodel = (LadderDiagramViewModel)(e.RelativeObject);
                    }
                    switch (e.Flags & ~0xf)
                    {
                        case ProjectTreeViewEventArgs.FLAG_CREATE:
                            CreateRoutine(e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REPLACE:
                            ReplaceRoutine(ldvmodel, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            RemoveRoutine(ldvmodel, e);
                            break;
                    }
                    break;
                case ProjectTreeViewEventArgs.TYPE_FUNCBLOCK:
                    if (e.RelativeObject is FuncBlockViewModel)
                    {
                        fbvmodel = (FuncBlockViewModel)(e.RelativeObject);
                    }
                    switch (e.Flags & ~0xf)
                    {
                        case ProjectTreeViewEventArgs.FLAG_CREATE:
                            CreateFuncBlock(e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REPLACE:
                            ReplaceFuncBlock(fbvmodel, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            RemoveFuncBlock(fbvmodel, e);
                            break;
                    }
                    break;
                case ProjectTreeViewEventArgs.TYPE_NETWORK:
                    if (e.RelativeObject is LadderNetworkViewModel)
                    {
                        lnvmodel = (LadderNetworkViewModel)(e.RelativeObject);
                        ldvmodel = lnvmodel.LDVModel;
                    }
                    if (e.RelativeObject is LadderDiagramViewModel)
                    {
                        ldvmodel = (LadderDiagramViewModel)(e.RelativeObject);
                    }
                    if (e.RelativeObject is int)
                    {
                        int number = (int)(e.RelativeObject);
                        if ((e.Flags & ~0xf) == ProjectTreeViewEventArgs.FLAG_REPLACE)
                        {
                            LadderNetworkViewModel lnvmodel_old = (LadderNetworkViewModel)(e.TargetedObject);
                            ReplaceNetwork(lnvmodel_old, number, e);
                        }
                        break;
                    }
                    switch (e.Flags & ~0xf)
                    {
                        case ProjectTreeViewEventArgs.FLAG_CREATE:
                            CreateNetwork(ldvmodel, ldvmodel.NetworkCount, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_CREATEBEFORE:
                            CreateNetwork(ldvmodel, lnvmodel.NetworkNumber, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_CREATEAFTER:
                            CreateNetwork(ldvmodel, lnvmodel.NetworkNumber + 1, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_CONFIG:
                            lnvmodel.EditComment();
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REPLACE:
                            LadderNetworkViewModel lnvmodel_old = (LadderNetworkViewModel)(e.TargetedObject);
                            ReplaceNetwork(lnvmodel_old, lnvmodel, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            RemoveNetwork(lnvmodel, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_INSERT:
                            ldvmodel = (LadderDiagramViewModel)(e.TargetedObject);
                            InsertNetwork(ldvmodel, lnvmodel, e);
                            break;
                    }
                    break;
                case ProjectTreeViewEventArgs.TYPE_MODBUS:
                    if (e.RelativeObject is ModbusTableModel)
                    {
                        mtmodel = (ModbusTableModel)(e.RelativeObject);
                    }
                    switch (e.Flags & ~0xf)
                    {
                        case ProjectTreeViewEventArgs.FLAG_CREATE:
                            CreateModbus(e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REPLACE:
                            ReplaceModbus(mtmodel, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            RemoveModbus(mtmodel, e);
                            break;
                    }
                    break;
                case ProjectTreeViewEventArgs.TYPE_INSTRUCTION:
                    if (e.RelativeObject is BaseViewModel && CurrentLadder != null)
                    {
                        CurrentLadder.ReplaceSingleElement(((BaseViewModel)(e.RelativeObject)).GetCatalogID());
                    }
                    break;
                default:
                    break;
            }
        }
        
        #endregion

    }
}
