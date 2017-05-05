using System;
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
        public MainTabControl MainTabControl
        {
            get { return this._mainTabControl; }
        }

        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get { return _mainWindow; }
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
            _mainTabControl = _mainWindow.MainTab;
            ElementList.NavigateToNetwork += ElementList_NavigateToNetwork;
            SimulateHelper.TabOpen += OnTabOpened;
        }

        private void CheckLadderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<PLCOriginInst> weinsts = new List<PLCOriginInst>();
            IEnumerable<PLCOriginInst> _weinsts = null;
            _weinsts = _projectModel.MainRoutine.IDVModel.Check();
            weinsts.AddRange(_weinsts);
            foreach (LadderDiagramViewModel ldvmodel in _projectModel.SubRoutines)
            {
                _weinsts = ldvmodel.IDVModel.Check();
                weinsts.AddRange(_weinsts);
            }
            int ecount = 0;
            int wcount = 0;
            foreach (PLCOriginInst inst in weinsts)
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
                    MessageBox.Show(
                        String.Format("程序存在{0:d}处错误，{1:d}处警告。",
                            ecount, wcount));
                }
                else
                {
                    MessageBox.Show("程序正确!");
                }
            }
            else if (errorMessage.Error == ErrorType.Empty)
            {
                MessageBox.Show(string.Format("网络{0}元素为空!", errorMessage.RefNetworks.First().NetworkNumber));
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
            }
        }
        private void CheckLadderCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentLadder != null;
        }

        private void DeleteRowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentLadder.SelectionStatus == SelectStatus.SingleSelected)
            {
                CurrentLadder.NetworkRemoveRow(CurrentLadder.SelectRectOwner, CurrentLadder.SelectionRect.Y);
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
                        CurrentLadder.NetworkRemoveRows(CurrentLadder.SelectStartNetwork, Math.Min(CurrentLadder.SelectStartNetwork.SelectAreaFirstY, CurrentLadder.SelectStartNetwork.SelectAreaSecondY), Math.Abs(CurrentLadder.SelectStartNetwork.SelectAreaFirstY - CurrentLadder.SelectStartNetwork.SelectAreaSecondY) + 1);
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
            CurrentLadder.NetworkAddRow(CurrentLadder.SelectRectOwner, CurrentLadder.SelectionRect.Y + 1);
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
                network.RemoveElement(hline.X, hline.Y);
            }
        }
        public void RemoveNetworkVLines(LadderNetworkViewModel network)
        {
            foreach (var vline in network.GetSelectedVerticalLines())
            {
                network.RemoveVerticalLine(vline.X, vline.Y);
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
                _projectTreeView.RoutineRemoved += OnRemoveRoutine;
                _projectTreeView.RoutineRenamed += OnRenameRoutine;
                _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
                _projectTreeView.RoutineCompile += OnCompileRoutine;
                _mainTabControl.SelectionChanged += OnTabItemChanged;
                _mainTabControl.ShowEditItem += OnTabOpened;
                _mainTabControl.Reset();
                _mainTabControl.ShowItem(_projectModel.MainRoutine);
                CurrentLadder = _projectModel.MainRoutine;
                _mainWindow.SetProjectTreeView(_projectTreeView);
                ProjectFullFileName = fullFileName;
                _projectTreeView.InstructionTreeItemDoubleClick += OnInstructionTreeItemDoubleClick;
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
                _projectTreeView.CloseElementList();
                _projectTreeView.TabItemOpened -= OnTabOpened;
                _projectTreeView.RoutineRemoved -= OnRemoveRoutine;
                _projectTreeView.RoutineRenamed -= OnRenameRoutine;
                _projectTreeView.NavigatedToNetwork -= ElementList_NavigateToNetwork;
                _projectTreeView.InstructionTreeItemDoubleClick -= OnInstructionTreeItemDoubleClick;
                _projectModel.MainRoutine.PropertyChanged -= _projectModel.MainRoutine_PropertyChanged;
                ProjectFullFileName = string.Empty;
                _mainTabControl.SelectionChanged -= OnTabItemChanged;
                _mainWindow.ClearProjectTreeView();
                _mainTabControl.Reset();
                _projectTreeView = null;
                _projectModel = null;
            }
        }
        public void SaveProject()
        {
            _projectModel.Save(ProjectFullFileName);
        }

        public void SaveAsProject(string fullFileName)
        {
            _projectModel.Save(fullFileName);
        }

        public bool LoadProject(string fullFileName)
        {
            _projectModel = ProjectHelper.LoadProject(fullFileName, new ProjectModel(String.Empty, _mainWindow.OutputModel));
            _projectModel.IFacade = this;
            if (_projectModel != null)
            {
                if (_projectModel.IsModify)
                {
                    MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                    switch (mbret)
                    {
                        case MessageBoxResult.Yes:
                            SaveProject();
                            _projectModel.IsModify = false;
                            return LoadProject(fullFileName);
                        case MessageBoxResult.No:
                            _projectModel.IsModify = false;
                            return LoadProject(fullFileName);
                        case MessageBoxResult.Cancel:
                        default:
                            return false;
                    }
                }
                else
                {
                    SamSoarII.LadderInstViewModel.InstructionCommentManager.UpdateAllComment();
                    _mainTabControl.SelectionChanged -= OnTabItemChanged;
                    _mainTabControl.ShowEditItem -= OnTabOpened;
                    if (_projectTreeView != null)
                    {
                        _projectTreeView.CloseElementList();
                    }
                    _projectTreeView = new ProjectTreeView(_projectModel);
                    _projectTreeView.TabItemOpened += OnTabOpened;
                    _projectTreeView.RoutineRemoved += OnRemoveRoutine;
                    _projectTreeView.RoutineRenamed += OnRenameRoutine;
                    _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
                    _projectTreeView.RoutineCompile += OnCompileRoutine;
                    _mainTabControl.Reset();
                    _mainTabControl.SelectionChanged += OnTabItemChanged;
                    _mainTabControl.ShowEditItem += OnTabOpened;
                    _mainTabControl.ShowItem(_projectModel.MainRoutine);
                    CurrentLadder = _projectModel.MainRoutine;
                    _mainWindow.SetProjectTreeView(_projectTreeView);
                    ProjectFullFileName = fullFileName;
                    _projectTreeView.InstructionTreeItemDoubleClick += OnInstructionTreeItemDoubleClick;
                    _projectModel.MainRoutine.PropertyChanged += _projectModel.MainRoutine_PropertyChanged;
                    _projectTreeView.SubRoutineTreeItems.ItemsSource = _projectModel.SubRoutineTreeViewItems;
                    UpdateRefNetworksBrief(_projectModel);
                    return true;
                } 
            }
            return false;
        }
        private void UpdateRefNetworksBrief(ProjectModel projectModel)
        {
            projectModel.UpdateNetworkBriefs(projectModel.MainRoutine,ChangeType.Add);
            foreach (var item in projectModel.SubRoutines)
            {
                projectModel.UpdateNetworkBriefs(item,ChangeType.Add);
            }
        }
        
        public bool AddNewSubRoutine(string name)
        {
            if(_projectModel.ContainProgram(name))
            {
                return false;
            }
            else
            {
                LadderDiagramViewModel ldmodel = new LadderDiagramViewModel(name, _projectModel);
                _projectModel.AddSubRoutine(ldmodel);
                _mainTabControl.ShowItem(ldmodel);
                return true;
            }
        }

        public bool AddNewFuncBlock(string name)
        {
            if (_projectModel.ContainProgram(name))
            {
                return false;
            }
            else
            {
                FuncBlockViewModel fbmodel = new FuncBlockViewModel(name);
                _projectModel.AddFuncBlock(fbmodel);
                _mainTabControl.ShowItem(fbmodel);
                return true;
            }
        }

        public void CompileProject()
        {
            _projectModel.Compile();
        }

        public int SimulateProject()
        {
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

        private void OnInstructionTreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var instTreeItem = sender as InstrucionTreeItem;
            if(instTreeItem != null)
            {
                if(CurrentLadder != null)
                {
                    CurrentLadder.ReplaceSingleElement(instTreeItem.InstructionIndex);
                }
            }
        }
       
        private void OnRenameRoutine(object sender, RoutineRenamedEventArgs e)
        {
            var progmodel = sender as IProgram;
            if(progmodel != null)
            {
                if(progmodel.ProgramName != e.NewName)
                {
                    if(_projectModel.ContainProgram(e.NewName))
                    {
                        MessageBox.Show("已存在同名子程序");
                        // 刷新TreeView中的Item
                        progmodel.ProgramName = progmodel.ProgramName;
                        return;
                    }
                    else
                    {
                        progmodel.ProgramName = e.NewName;   
                    }
                }
            }
        }

        private void OnRemoveRoutine(object sender, RoutedEventArgs e)
        {
            LadderDiagramViewModel ldmodel = sender as LadderDiagramViewModel;
            if (ldmodel != null)
            {
                _projectModel.RemoveSubRoutine(ldmodel);
                _mainTabControl.CloseItem(ldmodel);
            }
            else
            {
                var fbmodel = sender as FuncBlockViewModel;
                if (fbmodel != null)
                {
                    _projectModel.RemoveFuncBlock(fbmodel);
                    _mainTabControl.CloseItem(fbmodel);
                }
            }
        }

        private void OnCompileRoutine(object sender, RoutedEventArgs e)
        {
            var progmodel = sender as IProgram;
            _projectModel.CompileFuncBlock(progmodel.ProgramName);
            _mainWindow.LACOutput.Show();
        }
        
        #endregion

    }
}
