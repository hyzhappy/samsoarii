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
using System.Windows.Media.Animation;
using Xceed.Wpf.AvalonDock.Themes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Global;
using Xceed.Wpf.AvalonDock.Controls;
using System.Xml;
using SamSoarII.HelpDocument.HelpDocComponet;
using SamSoarII.HelpDocument;
using SamSoarII.AppMain.UI.Monitor;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private InteractionFacade _interactionFacade;
        private CanAnimationScroll MainScroll;
        public LayoutAnchorControl LACProj { get { return LAProj?.AnchorControl; } }
        public LayoutAnchorControl LACFind { get { return LAFind?.AnchorControl; } }
        public LayoutAnchorControl LACReplace { get { return LAReplace?.AnchorControl; } }
        public LayoutAnchorControl LACMonitor { get { return LAMonitor?.AnchorControl; } }
        public LayoutAnchorControl LACErrorList { get { return LAErrorList?.AnchorControl; } }
        public event RoutedEventHandler InstShortCutOpen = delegate { };
        public MainWindow()
        {
            InitializeComponent();
            InitializeAvalonDock();
            _interactionFacade = new InteractionFacade(this);
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            RecentFileMenu.DataContext = ProjectFileManager.projectShowMessage;
            FindWindow findwindow = new FindWindow(_interactionFacade);
            LAFind.Content = findwindow;
            ReplaceWindow replacewindow = new ReplaceWindow(_interactionFacade);
            LAReplace.Content = replacewindow;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is ElementList)
                {
                    window.Closing -= ((ElementList)window).OnClosing;
                    window.Close();
                }
                if (window is ElementInitializeWindow)
                {
                    window.Closing -= ((ElementInitializeWindow)window).OnClosing;
                    window.Close();
                }
            }
        }

        private void InitializeAvalonDock()
        {
            //DockManager.Theme = new VS2010Theme();

            LayoutSetting.Load();
            
            InitializeAvalonDock(LAProj);
            InitializeAvalonDock(LAFind);
            InitializeAvalonDock(LAReplace);
            InitializeAvalonDock(LAMonitor);
            InitializeAvalonDock(LAErrorList);
        }
        private void InitializeAvalonDock(LayoutAnchorable LAnch)
        {
            AnchorSide side;
            side = LayoutSetting.GetDefaultSideAnchorable(LAnch.Title);
            LAnch.ReplaceSide(side);

            double[] autohidesize;
            autohidesize = LayoutSetting.GetDefaultAutoHideSizeAnchorable(LAnch.Title);
            LAnch.AutoHideWidth = autohidesize[0];
            LAnch.AutoHideHeight = autohidesize[1];

            double[] floatsize;
            floatsize = LayoutSetting.GetDefaultFloatSizeAnchorable(LAnch.Title);
            LAnch.FloatingWidth = floatsize[0];
            LAnch.FloatingHeight = floatsize[1];

            LAnch.Hide();
        }
	    protected override void OnClosing(CancelEventArgs e)
        {
            if (_interactionFacade.ProjectModel != null
             && _interactionFacade.ProjectModel.IsModify)
            {
                MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case MessageBoxResult.Yes:
                        OnSaveProjectExecute(this, new RoutedEventArgs());
                        _interactionFacade.ProjectModel.IsModify = false;
                        break;
                    case MessageBoxResult.No:
                        _interactionFacade.ProjectModel.IsModify = false;
                        break;
                    case MessageBoxResult.Cancel:
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            base.OnClosing(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            LayoutSetting.Save();
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
        public void SetProjectMonitor(MainMonitor MMonitor)
        {
            MonitorViewGrid.Children.Clear();
            MonitorViewGrid.Children.Add(MMonitor);
        }
        public void SetProjectTreeView(ProjectTreeView treeview)
        {
            TreeViewGrid.Children.Clear();
            TreeViewGrid.Children.Add(treeview);
        }
        public void ClearProjectMonitor()
        {
            MonitorViewGrid.Children.Clear();
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
        private void OnClick(object sender, RoutedEventArgs e)
        {
            InstShortCutOpen.Invoke(sender, e);
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
                MessageBox.Show(string.Format("file has been moved or deleted"));
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
                    ProjectFileManager.Update(projectMessage.Value.Item1, projectMessage.Value.Item2);
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

        #region Project
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
        #endregion

        #region Command can Execute
        private void OnEleListOpenCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
        private void OnEleInitializeCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
	private void AddNewModbusCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
        private void ShowCommunicationSettingDialogCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
        
        private void ShowMainMonitorCanExecute(object sender, CanExecuteRoutedEventArgs e)
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

        private void MonitorCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
                e.CanExecute = (e.CanExecute && _interactionFacade.ProjectModel.LadderMode != LadderMode.Simulate);
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void EditCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
                e.CanExecute = (e.CanExecute && _interactionFacade.ProjectModel.LadderMode != LadderMode.Edit);
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
        private void OnInstShortCutOpenCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.CurrentLadder != null && _interactionFacade.CurrentLadder.SelectionStatus != SelectStatus.Idle;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void ShowOptionDialogCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
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

	    private void ShowErrorListCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
                e.CanExecute = (e.CanExecute && _interactionFacade.ProjectModel.LadderMode != LadderMode.Monitor);
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
        #endregion

        #region Command Execute
        private void OnEleInitializeCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.PTView.OpenEleInitialize();
        }
        private void OnEleListOpenCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.PTView.OpenElementList();
        }
        private void OnCloseProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.CloseCurrentProject();
            LAProj.Hide();
            LAMonitor.Hide();
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
        
        private void OnShowMainMonitorCommand(object sender, RoutedEventArgs e)
        {
            LACMonitor.Show();
        }
        private void OnShowErrorListExecute(object sender, RoutedEventArgs e)
        {
            LACErrorList.Show();
        }

        private void OnAddNewSubRoutineCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LACProj.Show();
            _interactionFacade.CreateRoutine();
        }

        private void OnAddNewFuncBlockCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LACProj.Show();
            _interactionFacade.CreateFuncBlock();
        }

        private void OnAddNewModbusCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.CreateModbus();
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ssp文件|*.ssp";
            if (saveFileDialog.ShowDialog() == true)
            {
                _interactionFacade.SaveAsProject(saveFileDialog.FileName);
            }
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

        private void OnMonitorCommandExecute(object sender, RoutedEventArgs e)
        {
            if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Edit)
            {
                CommunicationSettingDialog dialog = new CommunicationSettingDialog((CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]);
                BaseSetting baseSetting = dialog.GetBaseSetting();
                baseSetting.SettingButtonClick += (sender1, e1) =>
                {
                    CommunicationsettingParamsDialog dialog1 = new CommunicationsettingParamsDialog((CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]);
                    dialog1.ShowDialog();
                };
                dialog.Ensure += (sender1, e1) =>
                {
                    if (_interactionFacade.MonitorProject())
                    {
                        MonitorModeButton.IsChecked = true;
                        LACMonitor.Show();
                        dialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("通信失败！请检查参数设置。");
                    }
                };
                dialog.ShowDialog();
            }
            else if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Monitor)
            {
                _interactionFacade.EditProject();
                MonitorModeButton.IsChecked = false;
            }
        }

        private void OnEditCommandExecute(object sender, RoutedEventArgs e)
        { 
            if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Simulate)
            {
                OnSimulateCommandExecute(sender, e);
            }
            _interactionFacade.EditProject();
        }
        
        private void OnSimulateCommandExecute(object sender, RoutedEventArgs e)
        {
            if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Edit)
            {
                int ret = _interactionFacade.SimulateProject();
                if (ret == SimulateHelper.SIMULATE_OK)
                {
                    SimuToolBarTray.Visibility = Visibility.Visible;
                    SimulateHelper.SModel.SimulateStart += OnSimulateStart;
                    SimulateHelper.SModel.SimulatePause += OnSimulatePause;
                    SimulateHelper.SModel.SimulateAbort += OnSimulateAbort;
                    SimulateModeButton.IsChecked = true;
                }
                else
                {
                    SimulateModeButton.IsChecked = false;
                }
            }
            else if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Simulate)
            {
                if (SimulateHelper.Close() == SimulateHelper.CLOSE_OK)
                {
                    SimuToolBarTray.Visibility = Visibility.Collapsed;
                }
                else
                {
                    SimulateModeButton.IsChecked = true;
                }
            }
        }
        private void OnSimulateStart(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SimuStartButton.IsChecked = true;
                SimuPauseButton.IsChecked = false;
                SimuStopButton.IsChecked = false;
                SimuStartButton.IsEnabled = false;
                SimuPauseButton.IsEnabled = true;
                SimuStopButton.IsEnabled = true;
            });
        }

        private void OnSimuStartCommandExecute(object sender, RoutedEventArgs e)
        {
            SimulateHelper.SModel.Start();
        }
        
        private void OnSimulatePause(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SimuStartButton.IsChecked = false;
                SimuPauseButton.IsChecked = true;
                SimuStopButton.IsChecked = false;
                SimuStartButton.IsEnabled = true;
                SimuPauseButton.IsEnabled = false;
                SimuStopButton.IsEnabled = true;
            });
        }

        private void OnSimuPauseCommandExecute(object sender, RoutedEventArgs e)
        {
            SimulateHelper.SModel.Pause();
        }

        private void OnSimulateAbort(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SimuStartButton.IsChecked = false;
                SimuPauseButton.IsChecked = false;
                SimuStopButton.IsChecked = true;
                SimuStartButton.IsEnabled = true;
                SimuPauseButton.IsEnabled = false;
                SimuStopButton.IsEnabled = false;
            });
        }

        private void OnSimuStopCommandExecute(object sender, RoutedEventArgs e)
        {
            SimulateHelper.SModel.Stop();
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
            OptionDialog dialog = new OptionDialog(_interactionFacade);
            dialog.ShowDialog();
        }

        private void OnProcessExitExecute(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnDownloadCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {

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
        private void OnShowCommunicationSettingDialogExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CommunicationSettingDialog dialog = new CommunicationSettingDialog((CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]);
            BaseSetting baseSetting = dialog.GetBaseSetting();
            baseSetting.SettingButtonClick += (sender1, e1) =>
            {
                CommunicationsettingParamsDialog dialog1 = new CommunicationsettingParamsDialog((CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]);
                dialog1.ShowDialog();
            };
            dialog.Ensure += (sender2, e2) =>
            {
                dialog.Close();
            };
            dialog.ShowDialog();
        }
    }
}

