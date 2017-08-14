using SamSoarII.Core.Models;
using SamSoarII.Global;
using SamSoarII.PLCDevice;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Windows;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            RecentFileMenu.DataContext = ProjectFileManager.projectShowMessage;
            TBCB_Device.DataContext = ifParent;
            TBCB_ProjectName.DataContext = ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            InitializeAvalonDock();
            InitializeHotKey();
            App.splashScreen.Close(TimeSpan.FromMilliseconds(0));
            Loaded += OnMainWindowLoaded;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!GlobalSetting.LoadLadderScaleSuccess())
            {
                ILayoutPositionableElementWithActualSize _maintab = ifParent.TCMain;
                GlobalSetting.LadderOriginScaleX = _maintab.ActualWidth / 4400;
                GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / 4400;
            }
            Loaded -= OnMainWindowLoaded;
            LACProj.Show();
            LAProj.Hide();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += backgroundWorker_DoWork;
            worker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                if (App.AutoOpenFileFullPath != string.Empty)
                {
                    if (App.AutoOpenFileFullPath.EndsWith(FileHelper.OldFileExtension))
                    {
                        //LocalizedMessageBox.Show(Properties.Resources.File_Type_Not_Supported, LocalizedMessageIcon.Information);
                        ifParent.ShowFileConvertDialog(App.AutoOpenFileFullPath);
                    }
                    else ifParent.LoadProject(App.AutoOpenFileFullPath);
                }
            });
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

        private List<LayoutAnchorable> anchors;
        public LayoutAnchorable LAProj { get { return LA_Proj; } }
        public LayoutAnchorable LAFind { get { return LA_Find; } }
        public LayoutAnchorable LAReplace { get { return LA_Replace; } }
        public LayoutAnchorable LAMonitor { get { return LA_Monitor; } }
        public LayoutAnchorable LAInform { get { return LA_Inform; } }
        public LayoutAnchorable LAErrorList { get { return LA_ErrorList; } }
        public LayoutAnchorable LAElemList { get { return LA_ElemList; } }
        public LayoutAnchorable LAElemInit { get { return LA_ElemInit; } }
        public LayoutAnchorable LABreakpoint { get { return LA_Breakpoint; } }
        public LayoutAnchorControl LACProj { get { return LA_Proj?.AnchorControl; } }
        public LayoutAnchorControl LACFind { get { return LA_Find?.AnchorControl; } }
        public LayoutAnchorControl LACReplace { get { return LA_Replace?.AnchorControl; } }
        public LayoutAnchorControl LACMonitor { get { return LA_Monitor?.AnchorControl; } }
        public LayoutAnchorControl LACInform { get { return LA_Inform?.AnchorControl; } }
        public LayoutAnchorControl LACErrorList { get { return LA_ErrorList?.AnchorControl; } }
        public LayoutAnchorControl LACElemList { get { return LA_ElemList?.AnchorControl; } }
        public LayoutAnchorControl LACElemInit { get { return LA_ElemInit?.AnchorControl; } }
        public LayoutAnchorControl LACBreakpoint { get { return LA_Breakpoint?.AnchorControl; } }

        private void InitializeAvalonDock()
        {
            anchors = new List<LayoutAnchorable>();
            LayoutSetting.Load();
            InitializeAvalonDock(LAProj);
            InitializeAvalonDock(LAFind);
            InitializeAvalonDock(LAReplace);
            InitializeAvalonDock(LAMonitor);
            InitializeAvalonDock(LAInform);
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
            LA_Monitor.Content = ifParent.WNDMoni;
            LA_Inform.Content = ifParent.WNDInform;
            LA_Breakpoint.Content = ifParent.WNDBrpo;
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

            anchors.Add(LAnch);
            LAnch.Hide();
        }

        public void HideAllDock()
        {
            foreach (LayoutAnchorable anchor in anchors)
                anchor.Hide();
        }

        #endregion

        #region HotKey

        private List<GlobalThreeHotKey> _threeHotKeys;
        private string inputMessage = string.Empty;
        
        private void InitializeHotKey()
        {
            _threeHotKeys = new List<GlobalThreeHotKey>();
            KeyPartTwo keyPart;
            GlobalThreeHotKey hotKey;
            foreach (var command in GlobalCommand.commands)
            {
                if (command == GlobalCommand.AddNewFuncBlockCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.W, Key.F);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+W,F)\t{0}", Properties.Resources.Add_FuncBlock);
                    continue;
                }
                if (command == GlobalCommand.AddNewModbusCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.W, Key.M);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+W,M)\t{0}", Properties.Resources.Add_Modbus_Table);
                    continue;
                }
                if (command == GlobalCommand.AddNewSubRoutineCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.W, Key.R);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+W,R)\t{0}", Properties.Resources.Add_SubRoutine);
                    continue;
                }
                if (command == GlobalCommand.CheckFuncBlockCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.F);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,F)\t{0}", Properties.Resources.Funcblock_Checked);
                    continue;
                }
                if (command == GlobalCommand.CheckNetworkErrorCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.N);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,N)\t{0}", Properties.Resources.Ladder_Check);
                    continue;
                }
                if (command == GlobalCommand.CompileCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.C);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,C)\t{0}", Properties.Resources.MainWindow_Compile);
                    continue;
                }
                if (command == GlobalCommand.DownloadCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.D);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,D)\t{0}", Properties.Resources.MainWindow_Download);
                    continue;
                }
                if (command == GlobalCommand.UploadCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.U);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,U)\t{0}", Properties.Resources.MainWindow_Upload);
                    continue;
                }
                if (command == GlobalCommand.EditCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.E);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,E)\t{0}", Properties.Resources.Ret_Edit_Mode);
                    continue;
                }
                if (command == GlobalCommand.ZoomInCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.I);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,I)\t{0}", Properties.Resources.MainWindow_Zoom_In);
                    continue;
                }
                if (command == GlobalCommand.ZoomOutCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,O)\t{0}", Properties.Resources.MainWindow_Zoom_Out);
                    continue;
                }
                if (command == GlobalCommand.ChangeToChineseCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.U, Key.C);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+U,C)\t{0}", Properties.Resources.Chinese);
                    continue;
                }
                if (command == GlobalCommand.ChangeToEnglishCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.U, Key.E);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+U,E)\t{0}", Properties.Resources.English);
                    continue;
                }
                if (command == GlobalCommand.SimulateCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.S);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,S)\t{0}", Properties.Resources.MainWindow_Simulation);
                    continue;
                }
                if (command == GlobalCommand.MonitorCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.M);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,M)\t{0}", Properties.Resources.MainWindow_Monitor);
                    continue;
                }
                if (command == GlobalCommand.InstModeToggleCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.I);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,I)\t{0}", Properties.Resources.MainWindow_Inst_Mode);
                    continue;
                }
                if (command == GlobalCommand.LadderModeToggleCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.L);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,L)\t{0}", Properties.Resources.MainWindow_Ladder_Mode);
                    continue;
                }
                if (command == GlobalCommand.CommentModeToggleCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.C);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,C)\t{0}", Properties.Resources.MainWindow_Comment_Mode);
                    continue;
                }
                if (command == GlobalCommand.ShowProjectTreeViewCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F1, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F1,O)\t{0}", Properties.Resources.MainWindow_Project_Explorer);
                    continue;
                }
                if (command == GlobalCommand.ShowElemListCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F2, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F2,O)\t{0}", Properties.Resources.MainWindow_Element_List);
                    continue;
                }
                if (command == GlobalCommand.ShowElemInitCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F3, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F3,O)\t{0}", Properties.Resources.MainWindow_Soft_Element_Init);
                    continue;
                }
                if (command == GlobalCommand.ShowMainMonitorCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F4, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F4,O)\t{0}", Properties.Resources.Monitor_List);
                    continue;
                }
                if (command == GlobalCommand.ShowErrorListCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F5, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F5,O)\t{0}", Properties.Resources.MainWindow_Error_List);
                    continue;
                }
                if (command == GlobalCommand.ShowOptionDialogCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F6, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F6,O)\t{0}", Properties.Resources.Option);
                    continue;
                }
                if (command == GlobalCommand.ShowPropertyDialogCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F7, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F7,O)\t{0}", Properties.Resources.Property_Proj);
                    continue;
                }
                if (command == GlobalCommand.ShowBreakpointCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F8, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F8,O)\t{0}", Properties.Resources.MainWindow_Breakpoint_List);
                    continue;
                }
                if (command == GlobalCommand.ShowHelpDocumentCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.F9, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+F9,O)\t{0}", Properties.Resources.View_Help);
                    continue;
                }
                if (command == GlobalCommand.CloseProjectCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.Q, Key.E);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+Q,E)\t{0}", Properties.Resources.Close_Proj);
                    continue;
                }
                if (command == GlobalCommand.FileConvertCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.H);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,H)\t{0}", Properties.Resources.File_Converter);
                    continue;
                }
            }
        }
        private ToolTip GenToolTipByHotKeys()
        {
            ToolTip toolTip = new ToolTip();
            TextBlock textblock = new TextBlock();
            textblock.TextAlignment = TextAlignment.Left;
            textblock.Foreground = Brushes.Black;
            for (int i = 0; i < _threeHotKeys.Count; i++)
            {
                textblock.Text += _threeHotKeys[i].ShowMessage;
                if (i < _threeHotKeys.Count - 1) textblock.Text += "\n";
            }
            toolTip.Content = textblock;
            return toolTip;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Insert || e.Key == Key.Delete || e.Key == Key.Return) return;
            if (_threeHotKeys.Count == 0)
            {
                if (KeyInputHelper.IsModifier(e.Key))
                {
                    return;
                }
                if (((e.KeyboardDevice.Modifiers ^ ModifierKeys.Control) == ModifierKeys.None))
                {
                    _threeHotKeys = ThreeHotKeyManager.GetHotKeys(ModifierKeys.Control, e.Key);
                    if (_threeHotKeys != null && _threeHotKeys.Count() > 0)
                    {
                        ifParent.SetUnderBarMessage(string.Format("(Ctrl+{0}){1}", e.Key, Properties.Resources.Key_Pressed));
                        ThreeHotKeyManager.IsWaitForSecondModifier = true;
                        ThreeHotKeyManager.IsWaitForSecondKey = true;
                        ifParent.BARStatus.TB_Header.ToolTip = GenToolTipByHotKeys();
                    }
                }
            }
            else
            {
                Key key = e.Key;
                if (key == Key.System) key = e.SystemKey;
                if (KeyInputHelper.IsModifier(key))
                {
                    ThreeHotKeyManager.IsWaitForSecondModifier = false;
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && !inputMessage.Contains("Ctrl"))
                        inputMessage += inputMessage == string.Empty ? "Ctrl" : "+Ctrl";
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && !inputMessage.Contains("Alt"))
                        inputMessage += inputMessage == string.Empty ? "Alt" : "+Alt";
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && !inputMessage.Contains("Shift"))
                        inputMessage += inputMessage == string.Empty ? "Shift" : "+Shift";
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows && !inputMessage.Contains("Windows"))
                        inputMessage += inputMessage == string.Empty ? "Windows" : "+Windows";
                }
                else
                {
                    inputMessage += inputMessage == string.Empty ? inputMessage += string.Format("{0})", key) : string.Format("+{0})", key);
                    if (ThreeHotKeyManager.IsWaitForSecondModifier)
                    {
                        foreach (var hotKey in _threeHotKeys)
                        {
                            if (hotKey.Assert(key))
                            {
                                _threeHotKeys.Clear();
                                if (hotKey.CanExecute)
                                {
                                    ifParent.SetUnderBarMessage(Properties.Resources.Ready);
                                    hotKey.Execute();
                                }
                                else
                                {
                                    ifParent.SetUnderBarMessage(Properties.Resources.Command_Not_Execute);
                                }
                                break;
                            }
                        }
                        if (_threeHotKeys.Count != 0)
                        {
                            ifParent.SetUnderBarMessage(string.Format("{0}{1}{2}{3}", Properties.Resources.Key_Combination, _threeHotKeys.First(), inputMessage, Properties.Resources.Not_Command));
                            _threeHotKeys.Clear();
                        }
                    }
                    else
                    {
                        ifParent.SetUnderBarMessage(string.Format("{0}{1}{2}{3}", Properties.Resources.Key_Combination, _threeHotKeys.First(), inputMessage, Properties.Resources.Not_Command));
                        _threeHotKeys.Clear();
                    }
                    ThreeHotKeyManager.IsWaitForSecondModifier = false;
                    ThreeHotKeyManager.IsWaitForSecondKey = false;
                    inputMessage = string.Empty;
                    ifParent.BARStatus.ToolTip = null;
                }
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region Event Handler

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
                LocalizedMessageBox.Show(string.Format("{0}", Properties.Resources.Message_File_Moved), LocalizedMessageIcon.Information);
                ProjectFileManager.Delete(index);
            }
            else
            {
                if (ifParent.MDProj != null && projectMessage.Value.Item2 == ifParent.MDProj.FileName)
                    LocalizedMessageBox.Show(string.Format("{0}", Properties.Resources.Message_Project_Loaded), LocalizedMessageIcon.Information);
                else
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
                    ifParent.LoadProject(projectMessage.Value.Item2);
                    LACProj.Show();
                }
            }
            e.Handled = true;
        }

        private void About(object sender, RoutedEventArgs e)
        {
            LocalizedMessageBox.Show(string.Format("Version : 2.0.6"), LocalizedMessageIcon.Information);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
                e.Cancel = true;
            }
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
                        e.Cancel = true;
                        return;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IFParent.CloseProject();
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
                                TB_Simu.Visibility = Visibility.Visible;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_EDIT) != 0)                            
                                TB_Edit.Visibility = Visibility.Visible;                            
                            break;
                        case MainWindowEventArgs.TYPE_HIDE:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_SIMULATE) != 0)                            
                                TB_Simu.Visibility = Visibility.Collapsed;                            
                            if ((e1.Flags & MainWindowEventArgs.FLAG_EDIT) != 0)                            
                                TB_Edit.Visibility = Visibility.Collapsed;                            
                            break;
                        case MainWindowEventArgs.TYPE_ENABLE:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_START) != 0)
                                TB_Start.IsEnabled = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_PAUSE) != 0)
                                TB_Pause.IsEnabled = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_STOP) != 0)
                                TB_Stop.IsEnabled = true;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_BRPO) != 0)
                            {
                                BrpoNowButton.IsEnabled = true;
                                BrpoCallButton.IsEnabled = true;
                                BrpoStepButton.IsEnabled = true;
                                BropOutButton.IsEnabled = true;
                            }
                            break;
                        case MainWindowEventArgs.TYPE_DISABLE:
                            if ((e1.Flags & MainWindowEventArgs.FLAG_START) != 0)
                                TB_Start.IsEnabled = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_PAUSE) != 0)
                                TB_Pause.IsEnabled = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_STOP) != 0)
                                TB_Stop.IsEnabled = false;
                            if ((e1.Flags & MainWindowEventArgs.FLAG_BRPO) != 0)
                            {
                                BrpoNowButton.IsEnabled = false;
                                BrpoCallButton.IsEnabled = false;
                                BrpoStepButton.IsEnabled = false;
                                BropOutButton.IsEnabled = false;
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
             || e.Command == GlobalCommand.UploadCommand
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
            if (e.Command == GlobalCommand.ShowMainMonitorCommand)
                LACMonitor.Show();
            if (e.Command == GlobalCommand.ShowBreakpointCommand)
                LACBreakpoint.Show();
            if (e.Command == GlobalCommand.ShowPropertyDialogCommand)
                ifParent.ShowProjectPropertyDialog();
            if (e.Command == GlobalCommand.ShowOptionDialogCommand)
                ifParent.ShowSystemOptionDialog();
            if (e.Command == GlobalCommand.ShowCommunicationSettingDialogCommand)
                ifParent.ShowCommunicationSettingDialog();
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
            if (e.Command == GlobalCommand.ChangeToChineseCommand)
                ifParent.ShowLanaEnsureDialog(true);
            if (e.Command == GlobalCommand.ChangeToEnglishCommand)
                ifParent.ShowLanaEnsureDialog(false);
            if (e.Command == GlobalCommand.ShowHelpDocumentCommand)
                ifParent.ShowHelpDocument();
            if (e.Command == GlobalCommand.InsertRowCommand)
                ifParent.InsertRow();
            if (e.Command == GlobalCommand.DeleteRowCommand)
                ifParent.RemoveRow();
            if (e.Command == GlobalCommand.AddNewSubRoutineCommand)
                ifParent.AddNewSubRoutine();
            if (e.Command == GlobalCommand.AddNewFuncBlockCommand)
                ifParent.AddNewFuncBlock();
            if (e.Command == GlobalCommand.AddNewModbusCommand)
                ifParent.AddNewModbus();
            if (e.Command == GlobalCommand.ZoomInCommand)
                ifParent.ZoomChanged(true);
            if (e.Command == GlobalCommand.ZoomOutCommand)
                ifParent.ZoomChanged(false);
            if (e.Command == GlobalCommand.BrpoStepCommand)
                ifParent.MNGSimu.MoveStep();
            if (e.Command == GlobalCommand.BrpoCallCommand)
                ifParent.MNGSimu.CallStep();
            if (e.Command == GlobalCommand.BrpoOutCommand)
                ifParent.MNGSimu.JumpOut();
            if (e.Command == GlobalCommand.BrpoNowCommand)
                ifParent.NavigateToBreakpointCursor();
            if (e.Command == GlobalCommand.EditCommand)
                ifParent.ReturnToEdit();
            if (e.Command == GlobalCommand.FileConvertCommand)
                ifParent.ShowFileConvertDialog();
        }

        private void CommandBinding_Executed_SaveHint(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New
                || e.Command == ApplicationCommands.Open
                || e.Command == ApplicationCommands.Close
                || e.Command == GlobalCommand.CloseProjectCommand)
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
                CommandBinding_Executed_ReturnEditMode(sender, e);
                return;
            }
            bool ret;
            if (Project?.IsModified == true)
                ifParent.SaveProject();
            if (e.Command == GlobalCommand.SimulateCommand)
            {
                ret = ifParent.SimulateProject();
                TB_Simulate.IsChecked = ret;
            }
            if (e.Command == GlobalCommand.DownloadCommand)
                ifParent.DownloadProject();
            if (e.Command == GlobalCommand.UploadCommand)
                ifParent.UploadProject();
            if (e.Command == GlobalCommand.MonitorCommand)
            {
                ret = ifParent.MonitorProject();
                TB_Monitor.IsChecked = ret;
            }
        }

        private void CommandBinding_Executed_ReturnEditMode(object sender, ExecutedRoutedEventArgs e)
        {
            ifParent.EditProject();
            if (e.Command == ApplicationCommands.New)
                ifParent.ShowCreateProjectDialog();
            if (e.Command == ApplicationCommands.Open)
                ifParent.ShowOpenProjectDialog();
            if (e.Command == GlobalCommand.CloseProjectCommand)
                ifParent.CloseProject();
            if (e.Command == ApplicationCommands.Close)
                Application.Current.Shutdown();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ifParent != null ? ifParent.CanExecute(e) : false;
        }

        private void OnInstShortCutClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button button = (Button)(sender);
                switch (button.Tag.ToString())
                {
                    case "200": ifParent.QuickInsertElement(LadderUnitModel.Types.LD); break;
                    case "201": ifParent.QuickInsertElement(LadderUnitModel.Types.LDIM); break;
                    case "202": ifParent.QuickInsertElement(LadderUnitModel.Types.LDI); break;
                    case "203": ifParent.QuickInsertElement(LadderUnitModel.Types.LDIIM); break;
                    case "204": ifParent.QuickInsertElement(LadderUnitModel.Types.LDP); break;
                    case "205": ifParent.QuickInsertElement(LadderUnitModel.Types.LDF); break;
                    case "206": ifParent.QuickInsertElement(LadderUnitModel.Types.MEP); break;
                    case "207": ifParent.QuickInsertElement(LadderUnitModel.Types.MEF); break;
                    case "208": ifParent.QuickInsertElement(LadderUnitModel.Types.INV); break;
                    case "209": ifParent.QuickInsertElement(LadderUnitModel.Types.OUT); break;
                    case "210": ifParent.QuickInsertElement(LadderUnitModel.Types.OUTIM); break;
                    case "100": ifParent.QuickInsertElement(LadderUnitModel.Types.HLINE); break;
                    case "101": ifParent.QuickInsertElement(LadderUnitModel.Types.VLINE); break;
                    case "10":  ifParent.QuickRemoveElement(LadderUnitModel.Types.HLINE); break;
                    case "11":  ifParent.QuickRemoveElement(LadderUnitModel.Types.VLINE); break;
                }
            }
        }

        #endregion

    }

    public class MainWindowEventArgs : IWindowEventArgs
    {
        public const int TYPE_TOGGLE_DOWN = 0x0001;
        public const int TYPE_TOGGLE_UP = 0x0002;
        public const int TYPE_SHOW = 0x0003;
        public const int TYPE_HIDE = 0x0004;
        public const int TYPE_ENABLE = 0x0005;
        public const int TYPE_DISABLE = 0x0006;

        public const int FLAG_LADDER = 0x0010;
        public const int FLAG_INST = 0x0020;
        public const int FLAG_COMMENT = 0x0040;
        public const int FLAG_SIMULATE = 0x0080;
        public const int FLAG_START = 0x0100;
        public const int FLAG_PAUSE = 0x0200;
        public const int FLAG_STOP = 0x0400;
        public const int FLAG_MONITOR = 0x0800;
        public const int FLAG_EDIT = 0x1000;
        public const int FLAG_BRPO = 0x2000;

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
