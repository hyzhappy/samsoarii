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

namespace SamSoarII.AppMain
{
    public class InteractionFacade
    {
        private ProjectModel _projectModel;
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
        }

        public void CreateProject(string name, string fullFileName)
        {         
            _projectModel = new ProjectModel(name);
            _projectTreeView = new ProjectTreeView(name);
            _projectTreeView.LoadProject(_projectModel);
            _projectTreeView.TabItemOpened += OnTabOpened;
            _projectTreeView.RoutineRemoved += OnRemoveRoutine;
            _projectTreeView.RoutineRenamed += OnRenameRoutine;
            _mainTabControl.SelectionChanged += OnTabItemChanged;
            _mainTabControl.ShowItem(_projectModel.MainRoutine);        
            _mainWindow.SetProjectTreeView(_projectTreeView);
            ProjectFullFileName = fullFileName;
            _projectTreeView.InstructionTreeItemDoubleClick += OnInstructionTreeItemDoubleClick;
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
                _mainTabControl.SelectionChanged -= OnTabItemChanged;
                _projectTreeView = new ProjectTreeView(_projectModel.ProjectName);
                _projectTreeView.LoadProject(_projectModel);
                _projectTreeView.TabItemOpened += OnTabOpened;
                _projectTreeView.RoutineRemoved += OnRemoveRoutine;
                _projectTreeView.RoutineRenamed += OnRenameRoutine;
                _mainTabControl.Reset();
                _mainTabControl.SelectionChanged += OnTabItemChanged;
                _mainTabControl.ShowItem(_projectModel.MainRoutine);
                _mainTabControl.UpdateVariableCollection();        
                _mainWindow.SetProjectTreeView(_projectTreeView);
                ProjectFullFileName = fullFileName;
                _projectTreeView.InstructionTreeItemDoubleClick += OnInstructionTreeItemDoubleClick;
                return true;
            }
            return false;

        }

        public bool AddNewSubRoutine(string name)
        {
            if(_projectModel.ContainSubRoutine(name) || _projectModel.ContainFuncBlock(name))
            {
                return false;
            }
            else
            {
                LadderDiagramViewModel ldmodel = new LadderDiagramViewModel(name);
                _projectModel.AddSubRoutine(ldmodel);
                _projectTreeView.AddSubRoutine(ldmodel.LadderName);
                _mainTabControl.ShowItem(ldmodel);
                return true;
            }
        }

        public bool AddNewFuncBlock(string name)
        {
            if (_projectModel.ContainSubRoutine(name) || _projectModel.ContainFuncBlock(name))
            {
                return false;
            }
            else
            {
                FuncBlockViewModel fbmodel = new FuncBlockViewModel(name);
                _projectModel.AddFuncBlock(fbmodel);
                _projectTreeView.AddFuncBlock(fbmodel.FuncBlockName);
                _mainTabControl.ShowItem(fbmodel);
                return true;
            }
        }

        public void CompileProject()
        {
            _projectModel.Compile();
        }

        public void DeleteSubRoutine()
        {

        }

        public void CloseTabItem(TabItem tabItem)
        {
            if(_mainTabControl.Items.Contains(tabItem))
            {
                _mainTabControl.Items.Remove(tabItem);
            }
        }

        #region Event handler
        private void OnTabOpened(object sender, ShowTabItemEventArgs e)
        {
            switch(e.Type)
            {
                case TabType.Program:
                    var ldmodel = _projectModel.GetRoutineByName(e.TabName);
                    if (ldmodel != null)
                    {
                        _mainTabControl.ShowItem(ldmodel);
                        return;
                    }
                    var fbmodel = _projectModel.GetFuncBlockByName(e.TabName);
                    if (fbmodel != null)
                    {
                        _mainTabControl.ShowItem(fbmodel);
                        return;
                    }
                    break;
                case TabType.VariableList:
                    _mainTabControl.ShowVariableList();
                    break;
            }

        }

        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_mainTabControl.CurrentTab != null)
            {
                CurrentLadder = _mainTabControl.CurrentTab.Content as LadderDiagramViewModel;     
            }
            else
            {
                CurrentLadder = null;
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
            if(_projectModel.ContainFuncBlock(e.NewName) || _projectModel.ContainSubRoutine(e.NewName))
            {
                MessageBox.Show("已存在同名子程序");
                var treeitem = sender as TreeViewItem;
                if (treeitem != null)
                {
                    treeitem.Header = e.OldName;
                }
                return;
            }
            else
            {
                var ldmodel = _projectModel.GetRoutineByName(e.OldName);
                if(ldmodel != null)
                {
                    ldmodel.LadderName = e.NewName;
                }
                else
                {
                    var fbmodel = _projectModel.GetFuncBlockByName(e.OldName);
                    if(fbmodel != null)
                    {
                        fbmodel.FuncBlockName = e.NewName;
                    }
                }
                var tab = _mainTabControl.GetTabByName(e.OldName);
                if (tab != null)
                {
                    tab.Header = e.NewName;
                }
                var treeitem = sender as TreeViewItem;
                if (treeitem != null)
                {
                    treeitem.Header = e.NewName;
                }
            }
        }

        private void OnRemoveRoutine(object sender, RoutineChangedEventArgs e)
        {
            _projectModel.RemoveRoutineByName(e.RoutineName);
            _projectTreeView.RemoveRoutine(e.RoutineName);
            _mainTabControl.CloseItem(e.RoutineName);
        }
        
        #endregion

    }
}
