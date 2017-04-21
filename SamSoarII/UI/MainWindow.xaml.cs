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
using SamSoarII.AppMain.UI.HelpDocComponet;
using System.Windows.Media.Animation;
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
        private CanAnimationScroll MainScroll;
        public LayoutAnchorControl LACProj;
        public LayoutAnchorControl LACSimuProj;
        public LayoutAnchorControl LACMonitor;
        public LayoutAnchorControl LACOutput;

        public MainWindow()
        {
            InitializeComponent();

            InitializeAvalonDock();

            _interactionFacade = new InteractionFacade(this);
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            RecentFileMenu.DataContext = ProjectFileManager.projectShowMessage;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Name == "elementList")
                {
                    window.Closing -= ((ElementList)window).OnClosing;
                    window.Close();
                    break;
                }
            }
        }

        private void InitializeAvalonDock()
        {
            //DockManager.Theme = new VS2010Theme();

            LayoutSetting.Load();

            LACProj = LAProj.AnchorControl;
            LACSimuProj = LASimuProj.AnchorControl;
            LACMonitor = LAMonitor.AnchorControl;
            LACOutput = LAOutput.AnchorControl;

            AnchorSide side;
            side = LayoutSetting.GetDefaultSideAnchorable(LAProj.Title);
            LAProj.ReplaceSide(side);
            side = LayoutSetting.GetDefaultSideAnchorable(LASimuProj.Title);
            LASimuProj.ReplaceSide(side);
            side = LayoutSetting.GetDefaultSideAnchorable(LAMonitor.Title);
            LAMonitor.ReplaceSide(side);
            side = LayoutSetting.GetDefaultSideAnchorable(LAOutput.Title);
            LAOutput.ReplaceSide(side);
            LAProj.Hide();
            LASimuProj.Hide();
            LAMonitor.Hide();
            LAOutput.Hide();

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
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LAOutput.Title);
            LAOutput.AutoHideWidth = autohidesize[0];
            LAOutput.AutoHideHeight = autohidesize[1];

            double[] floatsize;
            floatsize = LayoutSetting.GetDefaultFloatSizeAnchorable(LAProj.Title);
            LAProj.FloatingWidth = floatsize[0];
            LAProj.FloatingHeight = floatsize[1];
            floatsize = LayoutSetting.GetDefaultFloatSizeAnchorable(LASimuProj.Title);
            LASimuProj.FloatingWidth = floatsize[0];
            LASimuProj.FloatingHeight = floatsize[1];
            floatsize = LayoutSetting.GetDefaultFloatSizeAnchorable(LAMonitor.Title);
            LAMonitor.FloatingWidth = floatsize[0];
            LAMonitor.FloatingHeight = floatsize[1];
            floatsize = LayoutSetting.GetDefaultFloatSizeAnchorable(LAOutput.Title);
            LAOutput.FloatingWidth = floatsize[0];
            LAOutput.FloatingHeight = floatsize[1];
        }

        protected override void OnClosed(EventArgs e)
        {
            LayoutSetting.Save();

            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        public void SetProjectTreeView(ProjectTreeView treeview)
        {
            TreeViewGrid.Children.Clear();
            TreeViewGrid.Children.Add(treeview);
        }
        public void ClearProjectTreeView()
        {
            TreeViewGrid.Children.Clear();
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
            if (!GlobalSetting.LoadLadderScaleSuccess())
            {
                ILayoutPositionableElementWithActualSize _maintab = (ILayoutPositionableElementWithActualSize)(MainTab);
                GlobalSetting.LadderOriginScaleX = _maintab.ActualWidth / 3700;
                GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / 3700;
            }
            MainScroll = GetMainScroll();
        }
        private int GetMenuItemIndex(string item)
        {
            return RecentFileMenu.Items.IndexOf(item);
        }
        private void OnRecentProjectOpen(object sender, RoutedEventArgs e)
        {
            int index = GetMenuItemIndex((sender as MenuItem).Header as string);
            var projectMessage = ProjectFileManager.RecentUsedProjectMessages.ElementAt(index);
            if (!File.Exists(projectMessage.Value.Item2))
            {
                MessageBox.Show(string.Format("file has been removed or deleted"));
                ProjectFileManager.Delete(index);
            }
            else
            {
                if (_interactionFacade.ProjectLoaded && projectMessage.Value.Item1 == _interactionFacade.ProjectModel.ProjectName)
                {
                    MessageBox.Show(string.Format("the opening project is current project"));
                }
                else
                {
                    _interactionFacade.LoadProject(projectMessage.Value.Item2);
                    LadderModeButton.IsChecked = true;
                    InstModeButton.IsChecked = false;
                    LACProj.Show();
                }
            }
            e.Handled = true;
        }
        private CanAnimationScroll GetMainScroll()
        {
            return MainTab.InnerScroll;
        }

        private void OnTabItemHeaderCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                TabItem tabitem = button.TemplatedParent as TabItem;
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
        private void OnTabItemHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
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
            LadderModeButton.IsChecked = true;
            InstModeButton.IsChecked = false;
            LACProj.Show();
        }

        private bool OpenProject(string fullFileName)
        {
            bool ret = _interactionFacade.LoadProject(fullFileName);
            LadderModeButton.IsChecked = true;
            InstModeButton.IsChecked = false;
            LACProj.Show();
            return ret;
        }

        public MessageBoxResult ShowSaveYesNoCancelDialog()
        {
            string title = "确认保存";
            string text = String.Format("{0:s}已经更改，是否保存？", _interactionFacade.ProjectModel.ProjectName);
            return MessageBox.Show(text, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        #region Command can Execute
        private void ClosePageCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainTab.SelectedItem != null;
        }
        private void ScrollToRightCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainScroll == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = MainScroll.ScrollableWidth != 0;
            }
        }
        private void ScrollToLeftCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainScroll == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = MainScroll.HorizontalOffset != 0;
            }
        }
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
            if (_interactionFacade != null && SimulateHelper.SModel == null)
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
            if (_interactionFacade != null && SimulateHelper.SModel != null)
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
            if (_interactionFacade != null && SimulateHelper.SModel != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowOutputCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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

        private void SimuStartCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void SimuPauseCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
            }
            else
            {
                if (SimuStartButton.IsChecked == true ||
                    SimuPauseButton.IsChecked == true)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        private void SimuStopCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        #endregion

        #region Command Execute
        private void OnCloseProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.CloseCurrentProject();
            //LACProj.Hide();
        }
        private void ClosePageExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MainTab.CloseItem(MainTab.SelectedItem as ITabItem);
        }
        private void ScrollToLeftCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollExecute(ScrollDirection.Left);
        }
        private void ScrollToRightCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollExecute(ScrollDirection.Right);
        }
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

        private void OnShowOutputCommand(object sender, RoutedEventArgs e)
        {
            LACOutput.Show();
        }

        private void OnAddNewSubRoutineCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewRoutineWindow window = new AddNewRoutineWindow();
            window.EnsureButtonClick += (sender1, e1) =>
            {
                if (!_interactionFacade.AddNewSubRoutine(window.NameContent))
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
            if (_interactionFacade.ProjectModel != null
             && _interactionFacade.ProjectModel.IsModify)
            {
                MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case MessageBoxResult.Yes:
                        OnSaveProjectExecute(sender, e);
                        _interactionFacade.ProjectModel.IsModify = false;
                        OnNewProjectExecute(sender, e);
                        return;
                    case MessageBoxResult.No:
                        _interactionFacade.ProjectModel.IsModify = false;
                        OnNewProjectExecute(sender, e);
                        return;
                    case MessageBoxResult.Cancel:
                    default:
                        return;
                }
            }
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
                    if (name == string.Empty)
                    {
                        MessageBox.Show("文件名不能为空");
                        return;
                    }
                    string fullFileName = string.Format(@"{0}\{1}.ssp", dir, name);
                    if (File.Exists(fullFileName))
                    {
                        MessageBox.Show("指定路径已存在同名文件");
                        return;
                    }
                    PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(newProjectDialog.Type);
                    CreateProject(name, fullFileName);
                    newProjectDialog.Close();
                };
                newProjectDialog.ShowDialog();
            }
        }

        private void OnOpenProjectExecute(object sender, RoutedEventArgs e)
        {
            if (_interactionFacade.ProjectModel != null
             && _interactionFacade.ProjectModel.IsModify)
            {
                MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case MessageBoxResult.Yes:
                        OnSaveProjectExecute(sender, e);
                        _interactionFacade.ProjectModel.IsModify = false;
                        OnOpenProjectExecute(sender, e);
                        return;
                    case MessageBoxResult.No:
                        _interactionFacade.ProjectModel.IsModify = false;
                        OnOpenProjectExecute(sender, e);
                        return;
                    case MessageBoxResult.Cancel:
                    default:
                        return;
                }
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ssp文件|*.ssp";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!OpenProject(openFileDialog.FileName))
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
        private void OnCloseProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
        private void OnSimulateCommandExecute(object sender, RoutedEventArgs e)
        {
            if (SimulateModeButton.IsChecked == true)
            {
                LACOutput.Show();
                int ret = _interactionFacade.SimulateProject();
                if (ret == SimulateHelper.SIMULATE_OK)
                {
                    LAOutput.Hide();
                    LAProj.Hide();
                    LACSimuProj.Show();
                    SimuToolBarTray.Visibility = Visibility.Visible;
                }
                else
                {
                    SimulateModeButton.IsChecked = false;
                }
            }
            else if (SimulateModeButton.IsChecked == false)
            {
                if (SimulateHelper.Close(_interactionFacade) == SimulateHelper.CLOSE_OK)
                {
                    LASimuProj.Hide();
                    LAMonitor.Hide();
                    LACProj.Show();
                    SimuToolBarTray.Visibility = Visibility.Collapsed;
                }
                else
                {
                    SimulateModeButton.IsChecked = true;
                }
            }
        }

        private void OnSimuStartCommandExecute(object sender, RoutedEventArgs e)
        {
            if (SimuStartButton.IsChecked == true)
            {
                if (SimuPauseButton.IsChecked == true)
                {
                    SimuPauseButton.IsChecked = false;
                }
                if (SimuStopButton.IsChecked == true)
                {
                    SimuStopButton.IsChecked = false;
                }
                SimulateHelper.SModel.Start();
            }
            else
            {
                SimuStartButton.IsChecked = true;
            }
        }

        private void OnSimuPauseCommandExecute(object sender, RoutedEventArgs e)
        {
            if (SimuPauseButton.IsChecked == true)
            {
                if (SimuStartButton.IsChecked == true)
                {
                    SimuStartButton.IsChecked = false;
                }
                if (SimuStopButton.IsChecked == true)
                {
                    SimuStopButton.IsChecked = false;
                }
                SimulateHelper.SModel.Pause();
            }
            else
            {
                SimuPauseButton.IsChecked = true;
            }
        }

        private void OnSimuStopCommandExecute(object sender, RoutedEventArgs e)
        {

            if (SimuStopButton.IsChecked == true)
            {
                if (SimuStartButton.IsChecked == true)
                {
                    SimuStartButton.IsChecked = false;
                }
                if (SimuPauseButton.IsChecked == true)
                {
                    SimuPauseButton.IsChecked = false;
                }
                SimulateHelper.SModel.Stop();
            }
            else
            {
                SimuStopButton.IsChecked = true;
            }
        }

        private void OnShowPropertyDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            ProjectPropertyDialog dialog = new ProjectPropertyDialog(_interactionFacade.ProjectModel);
            dialog.EnsureButtonClick += (sender1, e1) =>
            {
                dialog.Save();
                dialog.Close();
            };
            dialog.ShowDialog();
        }
        private void OnShowHelpDocWindow(object sender, RoutedEventArgs e)
        {
            HelpDocWindow helpDocWindow = new HelpDocWindow();
            helpDocWindow.Show();
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
            if (CommentModeToggleButton.IsChecked.HasValue)
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

        private void OnLadderModeToggle(object sender, ExecutedRoutedEventArgs e)
        {
            if (LadderModeButton.IsChecked.HasValue)
            {
                _interactionFacade.IsLadderMode = LadderModeButton.IsChecked.Value;
            }
        }

        private void LadderModeCanToggle(object sender, CanExecuteRoutedEventArgs e)
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

        private void OnInstModeToggle(object sender, ExecutedRoutedEventArgs e)
        {
            if (InstModeButton.IsChecked.HasValue)
            {
                _interactionFacade.IsInstMode = InstModeButton.IsChecked.Value;
            }
        }

        private void InstModeCanToggle(object sender, CanExecuteRoutedEventArgs e)
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
        private void ScrollToLeftAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = MainScroll.CanChangeHorizontalOffset;
            animation.To = Math.Max(0, MainScroll.CanChangeHorizontalOffset - 70);
            animation.Duration = new Duration(new TimeSpan(1500000));
            MainScroll.BeginAnimation(CanAnimationScroll.CanChangeHorizontalOffsetProperty, animation);
        }
        private void ScrollToRightAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = MainScroll.CanChangeHorizontalOffset;
            animation.To = MainScroll.CanChangeHorizontalOffset + Math.Min(70, MainScroll.ScrollableWidth);
            animation.Duration = new Duration(new TimeSpan(1500000));
            MainScroll.BeginAnimation(CanAnimationScroll.CanChangeHorizontalOffsetProperty, animation);
        }
        private void ScrollExecute(ScrollDirection direction)
        {
            switch (direction)
            {
                case ScrollDirection.Left:
                    ScrollToLeftAnimation();
                    break;
                case ScrollDirection.Right:
                    ScrollToRightAnimation();
                    break;
                default:
                    break;
            }
        }
    }
}