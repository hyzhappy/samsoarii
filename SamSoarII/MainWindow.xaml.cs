using SamSoarII.Core.Models;
using SamSoarII.Global;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Global;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IWindow, INotifyPropertyChanged, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
            ifParent = new InteractionFacade(this);
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            InitializeAvalonDock();
        }

        public void Dispose()
        {
            ifParent.PostIWindowEvent -= OnReceiveIWindowEvent;
            ifParent.Dispose();
            ifParent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { }; 

        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        public ProjectModel Project { get { return ifParent?.MDProj; } }

        #region StatusBar

        private SolidColorBrush _SB_FontColor = Brushes.Black;
        public SolidColorBrush SB_FontColor
        {
            get { return _SB_FontColor; }
            set
            {
                _SB_FontColor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SB_FontColor"));
            }
        }

        #endregion

        #region AvalonDock 

        public LayoutAnchorable LAProj { get { return LA_Proj; } }
        public LayoutAnchorable LAFind { get { return LA_Find; } }
        public LayoutAnchorable LAReplace { get { return LA_Replace; } }
        public LayoutAnchorable LAMonitor { get { return LA_Monitor; } }
        public LayoutAnchorable LAErrorList { get { return LA_ErrorList; } }
        public LayoutAnchorable LAElemList { get { return LA_ElemList; } }
        public LayoutAnchorable LAElemInit { get { return LA_ElemInit; } }
        public LayoutAnchorable LABreakpoint { get { return LA_Breakpoint; } }
        public LayoutAnchorControl LACProj { get { return LA_Proj?.AnchorControl; } }
        public LayoutAnchorControl LACFind { get { return LA_Find?.AnchorControl; } }
        public LayoutAnchorControl LACReplace { get { return LA_Replace?.AnchorControl; } }
        public LayoutAnchorControl LACMonitor { get { return LA_Monitor?.AnchorControl; } }
        public LayoutAnchorControl LACErrorList { get { return LA_ErrorList?.AnchorControl; } }
        public LayoutAnchorControl LACElemList { get { return LA_ElemList?.AnchorControl; } }
        public LayoutAnchorControl LACElemInit { get { return LA_ElemInit?.AnchorControl; } }
        public LayoutAnchorControl LACBreakpoint { get { return LA_Breakpoint?.AnchorControl; } }
        
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
            LP_Dock.Children.Add(ifParent.TCMain);
            LA_Proj.Content = ifParent.TVProj;
            GD_Find.Children.Add(ifParent.WNDFind);
            GD_Find.Children.Add(ifParent.WNDTFind);
            GD_Replace.Children.Add(ifParent.WNDReplace);
            GD_Replace.Children.Add(ifParent.WNDTReplace);
            GD_Bar.Children.Add(ifParent.BARStatus);
            LA_ErrorList.Content = ifParent.WNDError;
            LA_ElemList.Content = ifParent.WNDEList;
            LA_ElemInit.Content = ifParent.WNDEInit;
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

        #endregion

        #endregion

        #region Event Handler

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Dispose();
            Application.Current.Shutdown();
        }

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            if (e is MainWindowEventArgs)
            {
                MainWindowEventArgs e1 = (MainWindowEventArgs)e;
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    switch (e1.Flags & 0xf)
                    {
                        case MainWindowEventArgs.TYPE_TOGGLE_DOWN:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_LADDER) != 0)
                                TB_Ladder.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_INST) != 0)
                                TB_Inst.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_COMMENT) != 0)
                                TB_Comment.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_SIMULATE) != 0)
                                TB_Simulate.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_MONITOR) != 0)
                                TB_Monitor.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_START) != 0)
                                TB_Start.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_PAUSE) != 0)
                                TB_Pause.IsChecked = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_STOP) != 0)
                                TB_Stop.IsChecked = true;
                            break;
                        case MainWindowEventArgs.TYPE_TOGGLE_UP:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_LADDER) != 0)
                                TB_Ladder.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_INST) != 0)
                                TB_Inst.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_COMMENT) != 0)
                                TB_Comment.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_SIMULATE) != 0)
                                TB_Simulate.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_MONITOR) != 0)
                                TB_Monitor.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_START) != 0)
                                TB_Start.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_PAUSE) != 0)
                                TB_Pause.IsChecked = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_STOP) != 0)
                                TB_Stop.IsChecked = false;
                            break;
                        case MainWindowEventArgs.TYPE_SHOW:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_SIMULATE) != 0)
                            {
                                TB_Simu.Visibility = Visibility.Visible;
                            }
                            if ((e1.Flags & MainWindowEventArgs.FLAG_EDIT) != 0)
                            {
                                TB_Edit.Visibility = Visibility.Visible;
                            }
                            break;
                        case MainWindowEventArgs.TYPE_HIDE:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_SIMULATE) != 0)
                            {
                                TB_Simu.Visibility = Visibility.Collapsed;
                            }
                            if ((e1.Flags & MainWindowEventArgs.FLAG_EDIT) != 0)
                            {
                                TB_Edit.Visibility = Visibility.Collapsed;
                            }
                            break;
                    }
                });
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == GlobalCommand.SimulateCommand)
                TB_Simulate.IsChecked = !TB_Simulate.IsChecked;
            if (e.Command == GlobalCommand.MonitorCommand)
                TB_Monitor.IsChecked = !TB_Monitor.IsChecked;
            if (e.Command == GlobalCommand.SimuStartCommand)
                TB_Start.IsChecked = !TB_Start.IsChecked;
            if (e.Command == GlobalCommand.SimuPauseCommand)
                TB_Pause.IsChecked = !TB_Pause.IsChecked;
            if (e.Command == GlobalCommand.SimuStopCommand)
                TB_Stop.IsChecked = !TB_Stop.IsChecked;

            if (e.Command == ApplicationCommands.New
             || e.Command == ApplicationCommands.Open
             || e.Command == ApplicationCommands.Close
             || e.Command == GlobalCommand.CloseProjectCommand
             || e.Command == GlobalCommand.SimulateCommand
             || e.Command == GlobalCommand.DownloadCommand
             || e.Command == GlobalCommand.MonitorCommand)
            {
                CommandBinding_Executed_SaveHint(sender, e);
                return;
            }

            if (e.Command == ApplicationCommands.Save)
                ifParent.SaveProject();
            if (e.Command == ApplicationCommands.SaveAs)
                ifParent.ShowSaveProjectDialog();
            if (e.Command == GlobalCommand.LadderModeToggleCommand)
                ifParent.SetLadderMode(TB_Ladder.IsChecked == true);
            if (e.Command == GlobalCommand.InstModeToggleCommand)
                ifParent.SetInstMode(TB_Inst.IsChecked == true);
            if (e.Command == GlobalCommand.CommentModeToggleCommand)
                ifParent.SetCommentMode(TB_Comment.IsChecked == true);
            if (e.Command == GlobalCommand.ShowElemListCommand)
                LACElemList.Show();
            if (e.Command == GlobalCommand.ShowElemInitCommand)
                LACElemInit.Show();
            if (e.Command == GlobalCommand.ShowProjectTreeViewCommand)
                LACProj.Show();
            if (e.Command == GlobalCommand.ShowErrorListCommand)
                LACErrorList.Show();
            if (e.Command == GlobalCommand.ShowPropertyDialogCommand)
                ifParent.ShowProjectPropertyDialog();
            if (e.Command == GlobalCommand.ShowOptionDialogCommand)
                ifParent.ShowSystemOptionDialog();
            if (e.Command == GlobalCommand.CheckNetworkErrorCommand)
                ifParent.CheckLadder(true);
            if (e.Command == GlobalCommand.CheckFuncBlockCommand)
                ifParent.CheckFuncBlock(true);
            if (e.Command == GlobalCommand.SimuStartCommand)
                ifParent.MNGSimu.Start();
            if (e.Command == GlobalCommand.SimuPauseCommand)
                ifParent.MNGSimu.Pause();
            if (e.Command == GlobalCommand.SimuStopCommand)
                ifParent.MNGSimu.Abort();

        }

        private void CommandBinding_Executed_SaveHint(object sender, ExecutedRoutedEventArgs e)
        {
            if (Project?.IsModified == true)
            {
                LocalizedMessageResult mbret = ifParent.ShowSaveYesNoCancelDialog();
                switch (mbret)
                {
                    case LocalizedMessageResult.Yes:
                        ifParent.SaveProject();
                        break;
                    case LocalizedMessageResult.No:
                        break;
                    case LocalizedMessageResult.Cancel:
                    default:
                        return;
                }
            }
            if (e.Command == ApplicationCommands.New
             || e.Command == ApplicationCommands.Open
             || e.Command == ApplicationCommands.Close
             || e.Command == GlobalCommand.CloseProjectCommand)
            {
                CommandBinding_Executed_ReturnEditMode(sender, e);
                return;
            }
            if (e.Command == GlobalCommand.SimulateCommand)
            {
                ifParent.SimulateProject();
            }
            if (e.Command == GlobalCommand.DownloadCommand)
            {
                ifParent.DownloadProject();
            }
        }

        private void CommandBinding_Executed_ReturnEditMode(object sender, ExecutedRoutedEventArgs e)
        {
            ifParent.EditProject();
            if (e.Command == ApplicationCommands.New)
                ifParent.ShowCreateProjectDialog();
            if (e.Command == ApplicationCommands.Open)
                ifParent.ShowOpenProjectDialog();
            if (e.Command == ApplicationCommands.Close
             || e.Command == GlobalCommand.CloseProjectCommand)
                ifParent.CloseProject();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ifParent != null ? ifParent.CanExecute(e) : false;
        }

        #endregion

    }

    public class MainWindowEventArgs : IWindowEventArgs
    {
        public const int TYPE_TOGGLE_DOWN = 0x0001;
        public const int TYPE_TOGGLE_UP = 0x0002;
        public const int TYPE_SHOW = 0x0003;
        public const int TYPE_HIDE = 0x0004;

        public const int FLAG_LADDER = 0x0010;
        public const int FLAG_INST = 0x0020;
        public const int FLAG_COMMENT = 0x0040;
        public const int FLAG_SIMULATE = 0x0080;
        public const int FLAG_START = 0x0100;
        public const int FLAG_PAUSE = 0x0200;
        public const int FLAG_STOP = 0x0400;
        public const int FLAG_MONITOR = 0x0800;
        public const int FLAG_EDIT = 0x1000;

        public MainWindowEventArgs(MainWindow _wndMain, int _flags)
        {
            wndMain = _wndMain;
            flags = _flags;
        }

        private int flags;
        public int Flags { get { return this.flags; } }

        private MainWindow wndMain;
        public object TargetedObject { get { return wndMain; } }
        object IWindowEventArgs.TargetedObject { get { return TargetedObject; } }

        public object RelativeObject { get { return null; } }
    }

}
