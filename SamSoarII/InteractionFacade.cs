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
            _projectTreeView = new ProjectTreeView(_projectModel);
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
                _projectTreeView = new ProjectTreeView(_projectModel);
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
                case TabType.VariableList:
                    _mainTabControl.ShowVariableList();
                    break;
            }

        }

        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (_mainTabControl.CurrentTab != null)
            //{
            //    CurrentLadder = _mainTabControl.CurrentTab.Content as LadderDiagramViewModel;
            //}
            //else
            //{
            //    CurrentLadder = null;
            //}
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
