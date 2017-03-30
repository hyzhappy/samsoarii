using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.AppMain.Project;
using SamSoarII.UserInterface;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration;
using SamSoarII.LadderInstViewModel;
using Xceed.Wpf.AvalonDock.Themes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Global;
using Xceed.Wpf.AvalonDock.Controls;
using System.Xml;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private InteractionFacade _interactionFacade;

        public LayoutAnchorControl LACProj;
        public LayoutAnchorControl LACSimuProj;
        public LayoutAnchorControl LACMonitor;

        public MainWindow()
        {
            InitializeComponent();

            InitializeAvalonDock();

            _interactionFacade = new InteractionFacade(this);
            this.Loaded += MainWindow_Loaded;
        }

        private void InitializeAvalonDock()
        {
            LayoutSetting.Load();

            LACProj = LAProj.AnchorControl;
            LACSimuProj = LASimuProj.AnchorControl;
            LACMonitor = LAMonitor.AnchorControl;

            AnchorSide side;
            side = LayoutSetting.GetDefaultSideAnchorable(LAProj.Title);
            LAProj.ReplaceSide(side);
            side = LayoutSetting.GetDefaultSideAnchorable(LASimuProj.Title);
            LASimuProj.ReplaceSide(side);
            side = LayoutSetting.GetDefaultSideAnchorable(LAMonitor.Title);
            LAMonitor.ReplaceSide(side);
            LAProj.Hide();
            LASimuProj.Hide();
            LAMonitor.Hide();

            double[] autohidesize;
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LAProj.Title);
            LAProj.AutoHideWidth = autohidesize[0];
            LAProj.AutoHideHeight = autohidesize[1];
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LASimuProj.Title);
            LASimuProj.AutoHideWidth = autohidesize[0];
            LASimuProj.AutoHideHeight = autohidesize[1];
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LAMonitor.Title);
            LAMonitor.AutoHideWidth = autohidesize[0];
            LAMonitor.AutoHideHeight = autohidesize[1];

            double[] floatsize;
            floatsize = LayoutSetting.GetDefaultFloatSizeAnchorable(LAProj.Title);
            LAProj.FloatingWidth = floatsize[0];
            LAProj.FloatingHeight = floatsize[1];
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LASimuProj.Title);
            LASimuProj.FloatingWidth = floatsize[0];
            LASimuProj.FloatingHeight = floatsize[1];
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LAMonitor.Title);
            LAMonitor.FloatingWidth = floatsize[0];
            LAMonitor.FloatingHeight = floatsize[1];
        }

        protected override void OnClosed(EventArgs e)
        {
            LayoutSetting.Save();

            base.OnClosed(e);
        }

        public void SetProjectTreeView(ProjectTreeView treeview)
        {
            TreeViewGrid.Children.Clear();
            TreeViewGrid.Children.Add(treeview);
        }

        #region Event handler
        private void OnCommentModeToggle(object sender, RoutedEventArgs e)
        {
            _interactionFacade.IsCommentMode = !_interactionFacade.IsCommentMode;
        }

        private void OnShowAboutDialog(object sender, RoutedEventArgs e)
        {

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(!GlobalSetting.LoadLadderScaleSuccess())
            {
                ILayoutPositionableElementWithActualSize _maintab = (ILayoutPositionableElementWithActualSize)(MainTab);
                GlobalSetting.LadderOriginScaleX = _maintab.ActualWidth / 3100;
                GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / 3100;
            }
        }
        
        private void OnTabItemHeaderCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button != null)
            {
                TabItem tabitem = button.TemplatedParent as TabItem;
                if(tabitem != null)
                {
                    ITabItem tab = tabitem.Content as ITabItem;
                    if (tab != null)
                    {
                        _interactionFacade.CloseTabItem(tab);
                    }
                }
            }
        }
        private void OnTabItemHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    TabItem tabitem = grid.TemplatedParent as TabItem;
                    if (tabitem != null)
                    {
                        ITabItem tab = tabitem.Content as ITabItem;
                        if (tab != null)
                        {
                            _interactionFacade.CloseTabItem(tab);
                        }
                    }
                }
            }
        }

        #endregion

        private void CreateMainRoutine(string name)
        {

        }

        private void CreateProject(string name, string fullFileName)
        {
            _interactionFacade.CreateProject(name, fullFileName);
            _interactionFacade.SaveProject();
            LACProj.Show();
        }

        private bool OpenProject(string fullFileName)
        {
            bool ret = _interactionFacade.LoadProject(fullFileName);
            LACProj.Show();
            return ret;
        }


        #region Command can Execute
        private void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void AddNewSubRouteinCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void AddNewFuncBlockCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ZoomInCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ZoomOutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowProjectTreeViewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }


        private void ShowSimulateTreeViewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }


        private void ShowMonitorCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void CompileCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowPropertyDialogCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowOptionDialogCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DownloadCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
        }

        private void UploadCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }
        
        private void SimulateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion


        #region Command Execute

        private void OnZoomInCommandExecute(object sender, RoutedEventArgs e)
        {
            GlobalSetting.LadderScaleX += 0.1;
            GlobalSetting.LadderScaleY += 0.1;
        }

        private void OnZoomOutCommandExecute(object sender, RoutedEventArgs e)
        {
            GlobalSetting.LadderScaleX -= 0.1;
            GlobalSetting.LadderScaleY -= 0.1;
        }

        private void OnShowProjectTreeViewCommand(object sender, RoutedEventArgs e)
        {
            LACProj.Show();
        }

        private void OnShowSimulateTreeViewCommand(object sender, RoutedEventArgs e)
        {
            LACSimuProj.Show();
        }

        private void OnShowMonitorCommand(object sender, RoutedEventArgs e)
        {
            LACMonitor.Show();
        }

        private void OnAddNewSubRoutineCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewRoutineWindow window = new AddNewRoutineWindow();
            window.EnsureButtonClick += (sender1, e1) =>
            {
                if(!_interactionFacade.AddNewSubRoutine(window.NameContent))
                {
                    MessageBox.Show("已存在同名的子程序或函数功能块");
                }
                window.Close();
            };
            window.ShowDialog();
        }

        private void OnAddNewFuncBlockCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewRoutineWindow window = new AddNewRoutineWindow();
            window.EnsureButtonClick += (sender1, e1) =>
            {
                if (!_interactionFacade.AddNewFuncBlock(window.NameContent))
                {
                    MessageBox.Show("已存在同名的子程序或函数功能块");
                }
                window.Close();
            };
            window.ShowDialog();
        }

        private void OnNewProjectExecute(object sender, RoutedEventArgs e)
        {
            NewProjectDialog newProjectDialog;
            using (newProjectDialog = new NewProjectDialog())
            {
                newProjectDialog.EnsureButtonClick += (sender1, e1) =>
                {
                    string name = newProjectDialog.NameContent;
                    string dir = newProjectDialog.PathContent;
                    if (!Directory.Exists(dir))
                    {
                        MessageBox.Show("指定路径不存在");
                        return;
                    }
                    string fullFileName = string.Format(@"{0}\{1}.ssp", dir, name);
                    if (File.Exists(fullFileName))
                    {
                        MessageBox.Show("指定路径已存在同名文件");
                        return;
                    }
                    CreateProject(name, fullFileName);
                    newProjectDialog.Close();
                };
                newProjectDialog.ShowDialog();
            }
        }

        private void OnOpenProjectExecute(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ssp文件|*.ssp";
            if (openFileDialog.ShowDialog() == true)
            {            
                if(!OpenProject(openFileDialog.FileName))
                {
                    MessageBox.Show("不正确的工程文件，工程文件已损坏!");
                }
            }
        }

        private void OnSaveProjectExecute(object sender, RoutedEventArgs e)
        {
            _interactionFacade.SaveProject();
        }

        private void OnSaveAsProjectExecute(object sender, RoutedEventArgs e)
        {

        }

        private void OnCompileCommandExecute(object sender, RoutedEventArgs e)
        {
            _interactionFacade.CompileProject();
        }

        private void OnDownloadCommandExecute(object sender, RoutedEventArgs e)
        {

        }

        private void OnUploadCommandExecute(object sender, RoutedEventArgs e)
        {

        }

        private void OnSimulateCommandExecute(object sender, RoutedEventArgs e)
        {
            _interactionFacade.SimulateProject();
        }

        private void OnShowPropertyDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            //ProjectPropertyDialog dialog = new ProjectPropertyDialog(_interactionFacade.pro);
           // dialog.ShowDialog();
        }

        private void OnShowOptionDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            OptionDialog dialog = new OptionDialog();
            dialog.ShowDialog();
        }

        private void OnProcessExitExecute(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        private void OnCommentModeToggle(object sender, ExecutedRoutedEventArgs e)
        {
            if(CommentModeToggleButton.IsChecked.HasValue)
            {
                _interactionFacade.IsCommentMode = CommentModeToggleButton.IsChecked.Value;
            }   
        }

        private void CommentModeCanToggle(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void OnCheckNetworkErrorCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void OnCheckNetworkErrorCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_interactionFacade.CurrentLadder.CheckProgramControlInstructions())
            {
                MessageBox.Show(string.Format("程序控制指令配对失败！"));
            }
            else
            {
                var tempQueue = new Queue<LadderNetworkViewModel>(_interactionFacade.CurrentLadder.GetNetworks());
                while (tempQueue.Count > 0)
                {
                    var ladderNetworkViewModel = tempQueue.Dequeue();
                    if (ladderNetworkViewModel.IsNetworkError())
                    {
                        MessageBox.Show(string.Format("网络{0}错误", ladderNetworkViewModel.NetworkNumber));
                    }
                    else
                    {
                        MessageBox.Show(string.Format("网络{0}正常，可以编译!", ladderNetworkViewModel.NetworkNumber));
                    }
                }
            }
        }
    }
}
