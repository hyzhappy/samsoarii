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
            _projectTreeView = new ProjectTreeView();
            _projectTreeView.LoadProject(_projectModel);
            _projectTreeView.TabItemOpened += OnTabOpened;
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
            _projectModel = ProjectModel.Load(fullFileName);
            if (_projectModel != null)
            {
                _mainTabControl.SelectionChanged -= OnTabItemChanged;
                _projectTreeView = new ProjectTreeView();
                _projectTreeView.LoadProject(_projectModel);
                _projectTreeView.TabItemOpened += OnTabOpened;
                _mainTabControl.Reset();
                _mainTabControl.SelectionChanged += OnTabItemChanged;
                _mainTabControl.ShowItem(_projectModel.MainRoutine);               
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
            var ldmodel = _projectModel.GetRoutineByName(e.TabName);
            if (ldmodel != null)
            {
                _mainTabControl.ShowItem(ldmodel);
                return;
            }
            var fbmodel = _projectModel.GetFuncBlockByName(e.TabName);
            if(fbmodel != null)
            {
                _mainTabControl.ShowItem(fbmodel);
                return;
            }
        }

        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_mainTabControl.CurrentTab != null)
            {
                CurrentLadder = _projectModel.GetRoutineByName(_mainTabControl.CurrentTab.Header.ToString());
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
                    CurrentLadder.ReplaceElement(instTreeItem.InstructionIndex);
                }
            }
        }
        #endregion

    }
}
