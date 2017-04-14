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
        private MainWindow _mainWindow;
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
            _mainTabControl = _mainWindow.MainTab;
            ElementList.NavigateToNetwork += ElementList_NavigateToNetwork;
        }
        private void ElementList_NavigateToNetwork(NavigateToNetworkEventArgs e)
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
            tempItem.AcquireSelectRect(network);
            tempItem.SelectionRect.X = e.X;
            tempItem.SelectionRect.Y = e.Y;
            if (!network.LadderCanvas.Children.Contains(tempItem.SelectionRect))
            {
                network.LadderCanvas.Children.Add(tempItem.SelectionRect);
            }
            tempItem.NavigateToNetworkByNum(e.NetworkNum);
            _mainTabControl.ShowItem(tempItem);
        }

        public void CreateProject(string name, string fullFileName)
        {         
            _projectModel = new ProjectModel(name);
            _projectTreeView = new ProjectTreeView(_projectModel);
            _projectTreeView.TabItemOpened += OnTabOpened;
            _projectTreeView.RoutineRemoved += OnRemoveRoutine;
            _projectTreeView.RoutineRenamed += OnRenameRoutine;
            _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
            _mainTabControl.SelectionChanged += OnTabItemChanged;
            _mainTabControl.ShowItem(_projectModel.MainRoutine);
            _mainWindow.SetProjectTreeView(_projectTreeView);
            ProjectFullFileName = fullFileName;
            _projectTreeView.InstructionTreeItemDoubleClick += OnInstructionTreeItemDoubleClick;
            _projectModel.MainRoutine.PropertyChanged += _projectModel.MainRoutine_PropertyChanged;
            UpdateRefNetworksBrief(_projectModel);
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
            _projectModel = ProjectHelper.LoadProject(fullFileName);
            if (_projectModel != null)
            {
                SamSoarII.LadderInstViewModel.InstructionCommentManager.UpdateAllComment();
                _mainTabControl.SelectionChanged -= OnTabItemChanged;
                _projectTreeView = new ProjectTreeView(_projectModel);
                _projectTreeView.TabItemOpened += OnTabOpened;
                _projectTreeView.RoutineRemoved += OnRemoveRoutine;
                _projectTreeView.RoutineRenamed += OnRenameRoutine;
                _projectTreeView.NavigatedToNetwork += ElementList_NavigateToNetwork;
                _mainTabControl.Reset();
                _mainTabControl.SelectionChanged += OnTabItemChanged;
                _mainTabControl.ShowItem(_projectModel.MainRoutine);
                _mainWindow.SetProjectTreeView(_projectTreeView);
                ProjectFullFileName = fullFileName;
                _projectTreeView.InstructionTreeItemDoubleClick += OnInstructionTreeItemDoubleClick;
                _projectModel.MainRoutine.PropertyChanged += _projectModel.MainRoutine_PropertyChanged;
                _projectTreeView.SubRoutineTreeItems.ItemsSource = _projectModel.SubRoutineTreeViewItems;
                UpdateRefNetworksBrief(_projectModel);
                return true;
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
                LadderDiagramViewModel ldmodel = new LadderDiagramViewModel(name);
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

        public void SimulateProject()
        {
            SimulateHelper.Simulate(_projectModel);
        }

        public void CloseTabItem(ITabItem tabItem)
        {
            _mainTabControl.CloseItem(tabItem);
        }

        #region Event handler
        private void OnTabOpened(object sender, ShowTabItemEventArgs e)
        {
            switch (e.Type)
            {
                case TabType.Program:
                    IProgram prog = sender as IProgram;
                    _mainTabControl.ShowItem(prog);
                    break;
            }
        }

        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var ldmodel = _mainTabControl.SelectedItem as LadderDiagramViewModel;
            if (ldmodel != null)
            {
                CurrentLadder = ldmodel;
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
        #endregion

    }
}
