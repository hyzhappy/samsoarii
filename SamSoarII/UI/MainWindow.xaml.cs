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
using System.Threading;
using System.Windows.Threading;
using SamSoarII.ValueModel;
using SamSoarII.Simulation.Core.Event;
using System.Globalization;
using SamSoarII.Utility;
using System.IO.Pipes;
using System.Windows.Interop;
using Xceed.Wpf.AvalonDock;
using SamSoarII.AppMain.Project.Helper;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private OptionDialog SysSettingDialog;
        private InteractionFacade _interactionFacade;
        private CanAnimationScroll MainScroll;
        public LayoutAnchorControl LACProj { get { return LAProj?.AnchorControl; } }
        public LayoutAnchorControl LACFind { get { return LAFind?.AnchorControl; } }
        public LayoutAnchorControl LACReplace { get { return LAReplace?.AnchorControl; } }
        public LayoutAnchorControl LACMonitor { get { return LAMonitor?.AnchorControl; } }
        public LayoutAnchorControl LACErrorList { get { return LAErrorList?.AnchorControl; } }
        public LayoutAnchorControl LACElemList { get { return LAElemList?.AnchorControl; } }
        public LayoutAnchorControl LACElemInit { get { return LAElemInit?.AnchorControl; } }
        public LayoutAnchorControl LACBreakpoint { get { return LABreakpoint?.AnchorControl; } }
        public event RoutedEventHandler InstShortCutOpen = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private NamedPipeClientStream clientPipe;
        private SolidColorBrush _SB_FontColor = Brushes.Black;
        public SolidColorBrush SB_FontColor
        {
            get => _SB_FontColor;
            set
            {
                _SB_FontColor = value;
                PropertyChanged(this,new PropertyChangedEventArgs("SB_FontColor"));
            }
        }
        public MainWindow()
        {
            App.splashScreen.Close(TimeSpan.FromMilliseconds(0));
            InitializeComponent();
            DataContext = this;
            InitializeAvalonDock();
            _interactionFacade = new InteractionFacade(this);
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            RecentFileMenu.DataContext = ProjectFileManager.projectShowMessage;
            SysSettingDialog = new OptionDialog(_interactionFacade);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is OptionDialog)
                {
                    window.Closing -= ((OptionDialog)window).OnClosing;
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
            InitializeAvalonDock(LAElemList);
            InitializeAvalonDock(LAElemInit);
            InitializeAvalonDock(LABreakpoint);
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
            LAnch.FloatingLeft = floatsize[0];
            LAnch.FloatingTop = floatsize[1];
            LAnch.FloatingWidth = floatsize[2];
            LAnch.FloatingHeight = floatsize[3];

            LAnch.Hide();
        }
	    protected override void OnClosing(CancelEventArgs e)
        {
            if (!CurrentProjectHandle(false, false))
                e.Cancel = true;
            else base.OnClosing(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            if (_interactionFacade.ProjectModel != null)
            {
                if (_interactionFacade.ProjectModel.AutoInstManager.IsAlive)
                {
                    _interactionFacade.ProjectModel.AutoInstManager.Aborted += (sender1, e1) =>
                    {
                        OnClosed(e);
                    };
                    _interactionFacade.ProjectModel.AutoInstManager.Abort();
                    return;
                }
            }
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
        //Called when close or load current project
        public void ResetToolBar(bool isClose, int viewmode = MainTabControl.VIEWMODE_LADDER)
        {
            if (isClose)
            {
                LadderModeButton.IsChecked = false;
                InstModeButton.IsChecked = false;
                CommentModeToggleButton.IsChecked = false;
            }
            else
            {
                if((viewmode & MainTabControl.VIEWMODE_LADDER) == 0)
                    LadderModeButton.IsChecked = false;
                else
                    LadderModeButton.IsChecked = true;
                if ((viewmode & MainTabControl.VIEWMODE_INST) == 0)
                    InstModeButton.IsChecked = false;
                else
                    InstModeButton.IsChecked = true;
            }
        }
        #region Event handler
        private void Click_zh_Hans(object sender, RoutedEventArgs e)
        {
            using (LanaEnsureDialog dialog = new LanaEnsureDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    GlobalSetting.IsOpenLSetting = true;
                    GlobalSetting.LanagArea = string.Format("zh-Hans");
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }
        private void Click_en_US(object sender, RoutedEventArgs e)
        {
            using (LanaEnsureDialog dialog = new LanaEnsureDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    GlobalSetting.IsOpenLSetting = true;
                    GlobalSetting.LanagArea = string.Format("en");
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }
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
                GlobalSetting.LadderOriginScaleX = _maintab.ActualWidth / 4400;
                GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / 4400;
            }
            MainScroll = GetMainScroll();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += backgroundWorker_DoWork;
            worker.RunWorkerAsync();
            LACProj.Show();
            LAProj.Hide();
            Loaded -= MainWindow_Loaded;
        }
        private void Initialize()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ElementList.InitializeElementCollection();
                InstructionCommentManager.MappedMessageChanged += ElementList.InstructionCommentManager_MappedMessageChanged;
                ValueCommentManager.ValueCommentChanged += ElementList.ValueCommentManager_ValueCommentChanged;
                ValueAliasManager.ValueAliasChanged += ElementList.ValueAliasManager_ValueAliasChanged;
            });
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Initialize();
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
                MessageBox.Show(string.Format("{0}",Properties.Resources.Message_File_Moved));
                ProjectFileManager.Delete(index);
            }
            else
            {
                if (_interactionFacade.ProjectLoaded && projectMessage.Value.Item1 == _interactionFacade.ProjectModel.ProjectName)
                {
                    MessageBox.Show(string.Format("{0}", Properties.Resources.Message_Project_Loaded));
                }
                else
                {
                    if (CurrentProjectHandle(false, false))
                    {
                        _interactionFacade.LoadProject(projectMessage.Value.Item2);
                        ProjectFileManager.Update(projectMessage.Value.Item1, projectMessage.Value.Item2);
                        LACProj.Show();
                    }
                }
            }
            e.Handled = true;
        }
        private CanAnimationScroll GetMainScroll()
        {
            return MainTab.InnerScroll;
        }
        #endregion

        #region Project
        private void CreateProject(string name, string fullFileName)
        {
            _interactionFacade.CreateProject(name, fullFileName);
            _interactionFacade.SaveProject();
            LadderModeButton.IsChecked = true;
            InstModeButton.IsChecked = false;
            LACProj.Show();
        }
        private void CreateProject()
        {
            _interactionFacade.CreateProject(string.Format("{0}",Properties.Resources.Project), string.Empty);
            LACProj.Show();
        }
        private bool OpenProject(string fullFileName)
        {
            bool ret = _interactionFacade.LoadProject(fullFileName);
            LACProj.Show();
            return ret;
        }
        public void ResetDock()
        {
            foreach (var window in new List<Window>(DockManager.FloatingWindows))
            {
                window.Close();
            }
        }
        public MessageBoxResult ShowSaveYesNoCancelDialog()
        {
            string title = Properties.Resources.Message_Confirm_Save;
            string text = String.Format("{0:s} {1}", _interactionFacade.ProjectModel.ProjectName, Properties.Resources.Message_Changed);
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
                e.CanExecute = _interactionFacade.ProjectLoaded && _interactionFacade.CurrentLadder != null && _interactionFacade.CurrentLadder.SelectionStatus != SelectStatus.Idle;
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

        private void ShowElemListCanExecute(object sender, CanExecuteRoutedEventArgs e)
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


        private void ShowElemInitCanExecute(object sender, CanExecuteRoutedEventArgs e)
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

        private void ShowBreakpointCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null
             && _interactionFacade.ProjectLoaded
             && _interactionFacade.ProjectModel.LadderMode == LadderMode.Simulate)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void DownloadCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null
             && _interactionFacade.ProjectLoaded)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void UploadCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
        
        private void BPNowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = SimulateHelper.SModel.SManager.ISBPPause;
        }

        private void BPStepCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = SimulateHelper.SModel.SManager.ISBPPause;
        }

        private void BPCallCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = SimulateHelper.SModel.SManager.ISBPPause;
        }

        private void BPOutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SimulateHelper.SModel == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = SimulateHelper.SModel.SManager.ISBPPause;
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
            LACElemInit.Show();
        }
        private void OnEleListOpenCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LACElemList.Show();
        }
        private void OnCloseProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_interactionFacade.ProjectModel != null)
            {
                if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Simulate)
                {
                    OnSimulateCommandExecute(sender, e);
                }
                if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Monitor)
                {
                    OnMonitorCommandExecute(sender, e);
                }
            }
            if(CurrentProjectHandle(false, false))
            {
                _interactionFacade.CloseCurrentProject();
                LAProj.Hide();
                LAMonitor.Hide();
            }
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

        private void OnShowElemListExecute(object sender, RoutedEventArgs e)
        {
            LACElemList.Show();
        }

        private void OnShowElemInitExecute(object sender, RoutedEventArgs e)
        {
            LACElemInit.Show();
        }

        private void OnShowBreakpointExecute(object sender, ExecutedRoutedEventArgs e)
        {
            LACBreakpoint.Show();
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
        private bool CurrentProjectHandle(bool CreateNewProject,bool OpenProject)
        {
            if (_interactionFacade.ProjectModel == null)
            {
                if (CreateNewProject) NewProjectCreated();
                if (OpenProject) ProjectOpen();
            }
            else
            {
                if (_interactionFacade.ProjectModel.IsModify && _interactionFacade.ProjectFullFileName != string.Empty)
                {
                    MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                    switch (mbret)
                    {
                        case MessageBoxResult.Yes:
                            OnSaveProjectExecute(this, new RoutedEventArgs());
                            _interactionFacade.ProjectModel.IsModify = false;
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case MessageBoxResult.No:
                            _interactionFacade.ProjectModel.IsModify = false;
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case MessageBoxResult.Cancel:
                        default:
                            return false;
                    }
                }
                else if(_interactionFacade.ProjectFullFileName != string.Empty)
                {
                    if (CreateNewProject) NewProjectCreated();
                    if (OpenProject) ProjectOpen();
                }
                else
                {
                    MessageBoxResult mbret = ShowSaveYesNoCancelDialog();
                    switch (mbret)
                    {
                        case MessageBoxResult.Yes:
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = "ssp文件|*.ssp";
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                _interactionFacade.ProjectFullFileName = saveFileDialog.FileName;
                                _interactionFacade.SaveProject();
                            }
                            else return false;
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case MessageBoxResult.No:
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case MessageBoxResult.Cancel:
                        default:
                            return false;
                    }
                }
            }
            return true;
        }
        private void NewProjectCreated()
        {
            NewProjectDialog newProjectDialog;
            using (newProjectDialog = new NewProjectDialog())
            {
                newProjectDialog.EnsureButtonClick += (sender1, e1) =>
                {
                    PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(newProjectDialog.Type);
                    if (newProjectDialog.IsSettingChecked)
                    {
                        string name = newProjectDialog.NameContent;
                        string dir = newProjectDialog.PathContent;
                        if (!Directory.Exists(dir))
                        {
                            MessageBox.Show(Properties.Resources.Message_Path);
                            return;
                        }
                        if (name == string.Empty)
                        {
                            MessageBox.Show(Properties.Resources.Message_File_Name);
                            return;
                        }
                        string fullFileName = string.Format(@"{0}\{1}.ssp", dir, name);
                        if (File.Exists(fullFileName))
                        {
                            MessageBox.Show(Properties.Resources.Message_File_Exist);
                            return;
                        }
                        CreateProject(name, fullFileName);
                    }
                    else
                    {
                        CreateProject();
                    }
                    newProjectDialog.Close();
                };
                newProjectDialog.ShowDialog();
            }
        }
        private void OnNewProjectExecute(object sender, RoutedEventArgs e)
        {
            if (_interactionFacade.ProjectModel != null)
            {
                if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Simulate)
                {
                    OnSimulateCommandExecute(sender, e);
                }
                if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Monitor)
                {
                    OnMonitorCommandExecute(sender, e);
                }
            }
            CurrentProjectHandle(true, false);
        }
        private void ProjectOpen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ssp文件|*.ssp";
            if (openFileDialog.ShowDialog() == true)
            {
                if (_interactionFacade.ProjectFullFileName == openFileDialog.FileName)
                {
                    MessageBox.Show(Properties.Resources.Message_Project_Loaded);
                    return;
                }
                if (!OpenProject(openFileDialog.FileName))
                {
                    MessageBox.Show(Properties.Resources.Message_Project_Error);
                }
            }
        }
        private void OnOpenProjectExecute(object sender, RoutedEventArgs e)
        {
            if (_interactionFacade.ProjectModel != null)
            {
                if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Simulate)
                {
                    OnSimulateCommandExecute(sender, e);
                }
                if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Monitor)
                {
                    OnMonitorCommandExecute(sender, e);
                }
            }
            CurrentProjectHandle(false, true);
        }

        private void OnSaveProjectExecute(object sender, RoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                MessageBox.Show(Properties.Resources.Item_Rename);
                return;
            }
            if (_interactionFacade.ProjectFullFileName == string.Empty)
            {
                SaveProject();
            }
            else
            {
                _interactionFacade.SaveProject();
            }
            SB_Message.Text = Properties.Resources.Project_Saved;
        }
        public void SaveProject()
        {
            if (ProjectTreeViewItem.HasRenaming)
                return;
            CurrentProjectHandle(false, false);
        }
        private void OnSaveAsProjectExecute(object sender, RoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                MessageBox.Show(Properties.Resources.Item_Rename);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ssp文件|*.ssp";
            if (saveFileDialog.ShowDialog() == true)
            {
                _interactionFacade.SaveAsProject(saveFileDialog.FileName);
            }
        }
        
        private void OnMonitorCommandExecute(object sender, RoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                MessageBox.Show(Properties.Resources.Item_Rename);
                return;
            }
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
                        MessageBox.Show(Properties.Resources.MessageBox_Communication_Failed);
                };
                dialog.Closed += (sender2, e2) => 
                {
                    MonitorModeButton.IsChecked = false;
                };
                dialog.CommunicationTest += (sender1, e1) => 
                {
                    if (!_interactionFacade.CommunicationTest())
                        MessageBox.Show(Properties.Resources.MessageBox_Communication_Failed);
                    else
                        MessageBox.Show(Properties.Resources.MessageBox_Communication_Success);
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
            if (ProjectTreeViewItem.HasRenaming)
            {
                MessageBox.Show(Properties.Resources.Item_Rename);
                return;
            }
            if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Edit)
            {
                int ret = _interactionFacade.SimulateProject();
                if (ret == SimulateHelper.SIMULATE_OK)
                {
                    EditToolBarTray.Visibility = Visibility.Collapsed;
                    SimuToolBarTray.Visibility = Visibility.Visible;
                    SimulateHelper.SModel.SimulateStart += OnSimulateStart;
                    SimulateHelper.SModel.SimulatePause += OnSimulatePause;
                    SimulateHelper.SModel.SimulateAbort += OnSimulateAbort;
                    SimulateHelper.SModel.SManager.BreakpointPause += OnBreakpointPause;
                    SimulateHelper.SModel.SManager.BreakpointResume += OnBreakpointResume;
                    SimulateModeButton.IsChecked = true;
                    LACMonitor.Show();
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
                    EditToolBarTray.Visibility = Visibility.Visible;
                    SimuToolBarTray.Visibility = Visibility.Collapsed;
                    SimulateModeButton.IsChecked = false;
                }
                else
                {
                    SimulateModeButton.IsChecked = true;
                }
            }
        }

        private void OnBreakpointResume(object sender, BreakpointPauseEventArgs e)
        {
            if (SimuStopButton.IsChecked != true)
                OnSimulateStart(sender, new RoutedEventArgs());
        }

        private void OnBreakpointPause(object sender, BreakpointPauseEventArgs e)
        {
            OnSimulatePause(sender, new RoutedEventArgs());
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

        private void OnBPNowCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void OnBPStepCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (SimulateHelper.SModel != null)
            {
                SimulateHelper.SModel.SManager.StepMove();
            }
        }

        private void OnBPCallCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (SimulateHelper.SModel != null)
            {
                SimulateHelper.SModel.SManager.CallMove();
            }
        }

        private void OnBPOutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (SimulateHelper.SModel != null)
            {
                SimulateHelper.SModel.SManager.JumpOut();
            }
        }
        
        private void OnShowPropertyDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            using (ProjectPropertyDialog dialog = new ProjectPropertyDialog(_interactionFacade.ProjectModel))
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    try
                    {
                        dialog.Save();
                        dialog.Close();
                    }
                    catch (ProjectPropertyException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                dialog.ShowDialog();
            }
        }
        private void OnShowHelpDocWindow(object sender, RoutedEventArgs e)
        {
            HelpDocWindow helpDocWindow = new HelpDocWindow();
            helpDocWindow.Show();
        }

        private void OnShowOptionDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            SysSettingDialog.ShowDialog();
        }

        private void OnProcessExitExecute(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void OnUploadCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.UploadProject();
        }

        private void OnDownloadCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                MessageBox.Show(Properties.Resources.Item_Rename);
                return;
            }
            _interactionFacade.DownloadProject();
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
            dialog.Ensure += (sender1, e1) =>
            {
                if (!_interactionFacade.CommunicationTest())
                {
                    MessageBox.Show(Properties.Resources.MessageBox_Communication_Failed);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MessageBox_Communication_Success);
                }
                dialog.Close();
            };
            dialog.CommunicationTest += (sender1, e1) =>
            {
                if (!_interactionFacade.CommunicationTest())
                {
                    MessageBox.Show(Properties.Resources.MessageBox_Communication_Failed);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MessageBox_Communication_Success);
                }
            };
            dialog.ShowDialog();
        }
        private void OnUpdateClick(object sender, RoutedEventArgs e)
        {
            //XmlGen.GenUpdateXML(Directory.GetCurrentDirectory());
            Process process = new Process();
            process.StartInfo.FileName = Directory.GetCurrentDirectory() + @"\Update\SamSoarII.Update.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            clientPipe = new NamedPipeClientStream(".", "SamSoarII.Update", PipeDirection.InOut);
            clientPipe.Connect(6000);
            if (clientPipe.IsConnected)
            {
                StreamReader reader = new StreamReader(clientPipe);
                string message = reader.ReadLine();
                if (message == string.Format("N"))
                {
                    MessageBox.Show(Properties.Resources.New_Version);
                }
                else if (message == string.Format("Y"))
                {
                    message = reader.ReadLine();
                    if (message == string.Format("WebException"))
                    {
                        MessageBox.Show(Properties.Resources.Connect_Failed);
                        return;
                    }
                    else if (message == string.Format("Exception"))
                    {
                        MessageBox.Show(Properties.Resources.Download_Failed);
                        return;
                    }else if (message == string.Empty)
                    {
                        MessageBox.Show(Properties.Resources.Connect_Failed);
                        return;
                    }
                    long filesize = long.Parse(message);
                    MessageBoxResult ret = MessageBox.Show(string.Format(Properties.Resources.Update_Or_Not + "(" + Properties.Resources.Update_Process + ")\n" + Properties.Resources.Update_Size + "{0:f3}MB", filesize / (1024 * 1024 * 1.0)), Properties.Resources.Update_Whether, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    StreamWriter writer = new StreamWriter(clientPipe);
                    if (ret == MessageBoxResult.OK)
                        writer.WriteLine("Update");
                    else
                        writer.WriteLine("Cancel");
                    writer.Flush();
                    clientPipe.WaitForPipeDrain();
                }
                else if (message == string.Format("WebException"))
                {
                    MessageBox.Show(Properties.Resources.Connect_Failed);
                }
                else if (message == string.Format("Exception"))
                {
                    MessageBox.Show(Properties.Resources.Download_Failed);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.Connect_Failed);
            }
        }
    }
}

