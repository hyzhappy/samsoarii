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

namespace SamSoarII.AppMain
{
    public class InteractionFacade
    {
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
        private ProjectModel _projectModel;
        public ProjectModel ProjectModel
        {
            get
            {
                return _projectModel;
            }
        }
        private ProjectTreeView _projectTreeView;
        private MainTabControl _mainTabControl;
        public ProjectTreeView PTView
        {
            get
            {
                return _projectTreeView;
            }
        }
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
        public ErrorReportWindow ERWindow
        {
            get { return _erwindow; }
        }
        public bool IsLadderMode
        {
            get { return (_mainTabControl.ViewMode & MainTabControl.VIEWMODE_LADDER) != 0; }
            set
            {
                if (value == true)
                {
                    _mainTabControl.ViewMode |= MainTabControl.VIEWMODE_LADDER;
                }
                if (value == false)
                {
                    _mainTabControl.ViewMode &= ~MainTabControl.VIEWMODE_LADDER;
                }
            }
        }

        public bool IsInstMode
        {
            get { return (_mainTabControl.ViewMode & MainTabControl.VIEWMODE_INST) != 0; }
            set
            {
                if (value == true)
                {
                    _mainTabControl.ViewMode |= MainTabControl.VIEWMODE_INST;
                }
                if (value == false)
                {
                    _mainTabControl.ViewMode &= ~MainTabControl.VIEWMODE_INST;
                }
            }
        }

        public bool ProjectLoaded
        {
            get
            {
                return _projectModel != null;
            }
        }
        public string ProjectFullFileName { get; set; }

        public LadderDiagramViewModel CurrentLadder
        {
            get;
            private set;
        }
        public InteractionFacade(MainWindow mainwindow)
        {
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
            ElementList.NavigateToNetwork += ElementList_NavigateToNetwork;
            SimulateHelper.TabOpen += OnTabOpened;
            _erwindow = new ErrorReportWindow(this);
            mainwindow.LAErrorList.Content = _erwindow;
        }

        private void CheckLadderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckLadder(true);
        }
        public bool CheckLadder(bool showreport = false)
        {
            List<ErrorReportElement> weinsts = new List<ErrorReportElement>();
            IEnumerable<ErrorReportElement> _weinsts = null;
            _weinsts = _projectModel.MainRoutine.IDVModel.Check();
            weinsts.AddRange(_weinsts);
            foreach (LadderDiagramViewModel ldvmodel in _projectModel.SubRoutines)
            {
                _weinsts = ldvmodel.IDVModel.Check();
                weinsts.AddRange(_weinsts);
            }
            int ecount = 0;
            int wcount = 0;
            bool result = false;
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
            ErrorMessage errorMessage = LadderGraphCheckModule.Execute(CurrentLadder);
            if (errorMessage.Error == ErrorType.None
             || errorMessage.Error == ErrorType.InstPair)
            {
                if (weinsts.Count() > 0)
                {
                    result = (ecount == 0);
                    if (showreport || !result)
                        MessageBox.Show(
                            String.Format("程序存在{0:d}处错误，{1:d}处警告。",
                                ecount, wcount));
                }
                else
                {
                    if (showreport)
                        MessageBox.Show("程序正确!");
                    result = true;
                }
                if (!result)
                {
                    _erwindow.Mode = ErrorReportWindow.MODE_LADDER;
                    _erwindow.Update(weinsts);
                    _mainWindow.LACErrorList.Show();
                }
            }
            else if (errorMessage.Error == ErrorType.Empty)
            {
                MessageBox.Show(string.Format("网络{0}元素为空!", errorMessage.RefNetworks.First().NetworkNumber));
                result = false;
            }
            else
            {
                errorMessage.RefNetworks.First().AcquireSelectRect();
                CurrentLadder.SelectionRect.X = errorMessage.RefNetworks.Last().ErrorModels.First().X;
                CurrentLadder.SelectionRect.Y = errorMessage.RefNetworks.Last().ErrorModels.First().Y;
                CurrentLadder.HScrollToRect(CurrentLadder.SelectionRect.X);
                CurrentLadder.VScrollToRect(errorMessage.RefNetworks.First().NetworkNumber, CurrentLadder.SelectionRect.Y);
                switch (errorMessage.Error)
                {
                    case ErrorType.Open:
                        MessageBox.Show("光标处开路错误!");
                        break;
                    case ErrorType.Short:
                        MessageBox.Show("光标处短路错误!");
                        break;
                    case ErrorType.SelfLoop:
                        MessageBox.Show("光标处自环错误!");
                        break;
                    case ErrorType.HybridLink:
                        MessageBox.Show("光标处混联错误!");
                        break;
                    case ErrorType.Special:
                        MessageBox.Show("光标处特殊指令错误!");
                        break;
                    default:
                        break;
                }
                result = false;
            }
            return result;
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

        public bool CheckFuncBlock(bool showreport = false)
        {
            bool result = true;
            if (_projectModel.FuncBlocks.Count() == 0)
            {
                return result;
            }
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
                cmd.StartInfo.FileName = "arm-none-eabi-gcc";
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
            cmd.StartInfo.FileName = "arm-none-eabi-gcc";
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
            m1 = Regex.Match(stderr, @"\s(.+):(.+):\((.+)\): (.+)\r\n");
            while (m1 != null && m1.Success)
            {
                message = m1.Groups[4].Value;
                line = column = 0;
                string _ofile = m1.Groups[1].Value;
                int _ofile_id = ofiles.IndexOf(_ofile);
                if (_ofile_id >= 0)
                {
                    FuncBlockViewModel _fbvmodel = fbvmodels[_ofile_id];
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
            if (showreport || !result)
            {
                if (ecount == 0 && wcount == 0)
                {
                    MessageBox.Show("函数块全部正确！");
                }
                else
                {
                    MessageBox.Show(String.Format("函数块发生{0:d}处错误，{1:d}处警告。", ecount, wcount));
                    _erwindow.Mode = ErrorReportWindow.MODE_FUNC;
                    _erwindow.Update(eweles);
                    _mainWindow.LACErrorList.Show();
                }
            }
            return result;
        }
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
            {
                e.RelativeObject = ldvmodel;
            }
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
            LadderNetworkViewModel lnvmodel,
            ProjectTreeViewEventArgs e = null
        )
        {
            if (e == null)
            {
                e = new ProjectTreeViewEventArgs
                (
                    ProjectTreeViewEventArgs.TYPE_NETWORK
                  | ProjectTreeViewEventArgs.FLAG_REPLACE,
                    lnvmodel, null);
            }
            else
            {
                e.RelativeObject = lnvmodel;
            }
            PTVEvent(this, e);
        }

        #region FuncBlock
        public void CreateFuncBlock
        (
            ProjectTreeViewEventArgs e = null
        )
        {
            FuncBlockViewModel fbvmodel = new FuncBlockViewModel(String.Empty);
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
            //_projectModel.MTVModel.RemoveModel(mtmodel);
        }
        #endregion

        #endregion
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
            if (CurrentLadder != null && CurrentLadder.SelectionStatus != SelectStatus.Idle && CurrentLadder.CrossNetState == CrossNetworkState.NoCross)
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
            if (CurrentLadder != null && CurrentLadder.SelectionStatus == SelectStatus.SingleSelected)
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
                        if (catalogId == 10)
                        {
                            RemoveNetworkHLines(CurrentLadder.SelectStartNetwork);
                            foreach (var network in CurrentLadder.SelectAllNetworks)
                            {
                                RemoveNetworkHLines(network);
                            }
                        }
                        else
                        {
                            RemoveNetworkVLines(CurrentLadder.SelectStartNetwork);
                            foreach (var network in CurrentLadder.SelectAllNetworks)
                            {
                                RemoveNetworkVLines(network);
                            }
                        }
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
                if (catalogId != 209 && catalogId != 210)
                {
                    SelectionRectRight();
                }
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
                    if (CurrentLadder.SelectRectOwner.GetElementByPosition(CurrentLadder.SelectionRect.X, CurrentLadder.SelectionRect.Y) is HorizontalLineViewModel)
                    {
                        CurrentLadder.SelectRectOwner.RemoveElement(CurrentLadder.SelectionRect.X, CurrentLadder.SelectionRect.Y);
                    }
                    SelectionRectRight();
                }
                else
                {
                    if (CurrentLadder.SelectionRect.X > 0)
                    {
                        if (CurrentLadder.SelectRectOwner.RemoveVerticalLine(CurrentLadder.SelectionRect.X - 1, CurrentLadder.SelectionRect.Y))
                        {
                            SelectionRectDown();
                        }
                    }
                }
            }
        }
        public void RemoveNetworkHLines(LadderNetworkViewModel network)
        {
            foreach (var hline in network.GetSelectedHLines())
            {
                network.RemoveElement(hline.X,hline.Y);
            }
        }
        public void RemoveNetworkVLines(LadderNetworkViewModel network)
        {
            foreach (var vline in network.GetSelectedVerticalLines())
            {
                network.RemoveVerticalLine(vline.X,vline.Y);
            }
        }
        public MessageBoxResult ShowSaveYesNoCancelDialog()
        {
            string title = "确认保存";
            string text = String.Format("{0:s}已经更改，是否保存？", _projectModel.ProjectName);
            return MessageBox.Show(text, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        private void ElementList_NavigateToNetwork(NavigateToNetworkEventArgs e)
        {
            NavigateToNetwork(e);
        }
        public void NavigateToNetwork(NavigateToNetworkEventArgs e)
        {
            LadderDiagramViewModel tempItem;
            if (ProjectModel.MainRoutine.ProgramName == e.RefLadderName)
            {
                tempItem = ProjectModel.MainRoutine;
            }
            else
            {
                tempItem = ProjectModel.SubRoutines.Where(x => { return x.ProgramName == e.RefLadderName; }).First();
            }
            var network = tempItem.GetNetworkByNumber(e.NetworkNum);
            network.AcquireSelectRect();
            tempItem.SelectionRect.X = e.X;
            tempItem.SelectionRect.Y = e.Y;
            tempItem.NavigateToNetworkByNum(e.NetworkNum);
            _mainTabControl.ShowItem(tempItem);
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
        public void CreateProject(string name, string fullFileName)
        {
            if (_projectModel != null && _projectModel.IsModify)
            {
                MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case MessageBoxResult.Yes:
                        SaveProject();
                        _projectModel.IsModify = false;
                        CreateProject(name, fullFileName);
                        return;
                    case MessageBoxResult.No:
                        _projectModel.IsModify = false;
                        CreateProject(name, fullFileName);
                        return;
                    case MessageBoxResult.Cancel:
                    default:
                        return;
                }
            }
            else
            {
                _projectModel = new ProjectModel(name, _mainWindow.OutputModel);
                _projectModel.IFacade = this;
                ProjectFileManager.Update(name,fullFileName);
                ValueAliasManager.Clear();
                ValueCommentManager.Clear();
                InstructionCommentManager.Clear();
                _projectTreeView = new ProjectTreeView(_projectModel);
                _projectTreeView.TabItemOpened += OnTabOpened;
                _projectTreeView.PTVHandle += OnGotPTVHandle;
                _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
                _mainTabControl.SelectionChanged += OnTabItemChanged;
                _mainTabControl.ShowEditItem += OnTabOpened;
                _mainTabControl.Reset();
                _mainTabControl.ShowItem(_projectModel.MainRoutine);
                CurrentLadder = _projectModel.MainRoutine;
                _mainWindow.SetProjectTreeView(_projectTreeView);
                _mainWindow.SetProjectMonitor(_projectModel.MMonitorManager.MMWindow);
                ProjectFullFileName = fullFileName;
                _projectModel.MainRoutine.PropertyChanged += _projectModel.MainRoutine_PropertyChanged;
                UpdateRefNetworksBrief(_projectModel);
            }
        }
        public void CloseCurrentProject()
        {
            if (_projectModel != null && _projectModel.IsModify)
            {
                MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case MessageBoxResult.Yes:
                        SaveProject();
                        _projectModel.IsModify = false;
                        CloseCurrentProject();
                        return;
                    case MessageBoxResult.No:
                        _projectModel.IsModify = false;
                        CloseCurrentProject();
                        return;
                    case MessageBoxResult.Cancel:
                    default:
                        return;
                }
            }
            else
            {
                _projectTreeView.TabItemOpened -= OnTabOpened;
                _projectTreeView.PTVHandle += OnGotPTVHandle;
                _projectTreeView.NavigatedToNetwork -= ElementList_NavigateToNetwork;
                _projectModel.MainRoutine.PropertyChanged -= _projectModel.MainRoutine_PropertyChanged;
                ProjectFullFileName = string.Empty;
                _mainTabControl.SelectionChanged -= OnTabItemChanged;
                _mainWindow.ClearProjectTreeView();
                _mainWindow.ClearProjectMonitor();
                _mainTabControl.Reset();
                _projectTreeView = null;
                _projectModel = null;
            }
        }
        public void SaveProject()
        {
            SaveAsProject(ProjectFullFileName);
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
        public bool LoadProject(string fileName)
        {
            if (_projectModel != null && _projectModel.IsModify)
            {
                MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case MessageBoxResult.Yes:
                        SaveProject();
                        _projectModel.IsModify = false;
                        return LoadProject(fileName);
                    case MessageBoxResult.No:
                        _projectModel.IsModify = false;
                        return LoadProject(fileName);
                    case MessageBoxResult.Cancel:
                    default:
                        return false;
                }
            }
            else
            {
                _projectModel = ProjectHelper.LoadProject(fileName, new ProjectModel(String.Empty, _mainWindow.OutputModel));
                _projectModel.IFacade = this;
                XDocument xdoc = XDocument.Load(fileName);
                XElement xele_r = xdoc.Element("Root");
                XElement xele_rtv = xele_r.Element("ProjectTreeView");
                InstructionCommentManager.UpdateAllComment();
                _mainTabControl.SelectionChanged -= OnTabItemChanged;
                _mainTabControl.ShowEditItem -= OnTabOpened;
                _projectModel.EleInitializeData = null;
                _projectTreeView = new ProjectTreeView(_projectModel, xele_rtv);
                _projectTreeView.TabItemOpened += OnTabOpened;
                _projectTreeView.PTVHandle += OnGotPTVHandle;
                _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
                _mainTabControl.Reset();
                _mainTabControl.SelectionChanged += OnTabItemChanged;
                _mainTabControl.ShowEditItem += OnTabOpened;
                _mainTabControl.ShowItem(_projectModel.MainRoutine);
                CurrentLadder = _projectModel.MainRoutine;
                _mainWindow.SetProjectTreeView(_projectTreeView);
                _mainWindow.SetProjectMonitor(_projectModel.MMonitorManager.MMWindow);
                ProjectFullFileName = fileName;
                _projectModel.MainRoutine.PropertyChanged += _projectModel.MainRoutine_PropertyChanged;
                UpdateRefNetworksBrief(_projectModel);
                return true;
            }
        }
        private void UpdateRefNetworksBrief(ProjectModel projectModel)
        {
            projectModel.UpdateNetworkBriefs(projectModel.MainRoutine,ChangeType.Add);
            foreach (var item in projectModel.SubRoutines)
            {
                projectModel.UpdateNetworkBriefs(item,ChangeType.Add);
            }
        }
        public void CompileProject()
        {
            _projectModel.Compile();
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
            int ret = SimulateHelper.Simulate(this, _mainWindow.OutputModel);
            switch (ret)
            {
                case SimulateHelper.SIMULATE_OK:
                    break;
                default:
                    return ret;
            }
            _mainWindow.LASimuProj.Content = SimulateHelper.SModel.PTView;
            _mainWindow.LASimuMonitor.Content = SimulateHelper.SModel.MTable;
            _mainTabControl.ReplaceAllTabsToSimulate();
            return ret;
        }

        public void CloseTabItem(ITabItem tabItem)
        {
            _mainTabControl.CloseItem(tabItem);
        }

        #region Event handler
        private void OnEditTabOpened(object sender, ShowTabItemEventArgs e)
        {
            
        }

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
                case TabType.Simulate:
                    ITabItem tab = sender as ITabItem;
                    _mainTabControl.ShowItem(tab);
                    break;
                case TabType.SimuToEdit:
                    string header = e.Header;
                    if (header.Equals("主程序") || header.Equals("main"))
                    {
                        _mainTabControl.ShowItem(_projectModel.MainRoutine);
                        break;
                    }
                    foreach (LadderDiagramViewModel ldvmodel in _projectModel.SubRoutines)
                    {
                        if (header.Equals(ldvmodel.ProgramName))
                        {
                            _mainTabControl.ShowItem(ldvmodel);
                            break;
                        }
                    }
                    foreach (FuncBlockViewModel fbvmodel in _projectModel.FuncBlocks)
                    {
                        if (header.Equals(fbvmodel.ProgramName))
                        {
                            _mainTabControl.ShowItem(fbvmodel);
                            break;
                        }
                    }
                    break;
            }
        }

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
                            ReplaceNetwork(lnvmodel, e);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            RemoveNetwork(lnvmodel, e);
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
