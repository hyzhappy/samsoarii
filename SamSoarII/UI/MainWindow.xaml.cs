﻿using System;
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
using SamSoarII.Utility.FileRegister;
using SamSoarII.PLCDevice;
using SamSoarII.AppMain.Global;

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
            get { return _SB_FontColor; }
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
            InitializeAvalonDock();
            InitializeHotKey();
            _interactionFacade = new InteractionFacade(this);
            DataContext = this;
            TBCB_Device.DataContext = PLCDeviceManager.GetPLCDeviceManager();
            TBCB_ProjectName.DataContext = _interactionFacade;
            Closing += MainWindow_Closing;
            RecentFileMenu.DataContext = ProjectFileManager.projectShowMessage;
            SysSettingDialog = new OptionDialog(_interactionFacade);
            Loaded += MainWindow_Loaded;
            PreviewKeyDown += MainWindow_KeyDown;
        }

        #region GlobalHotKey
        private List<GlobalThreeHotKey> _threeHotKeys = new List<GlobalThreeHotKey>();
        private string inputMessage = string.Empty;
        /// <summary>
        /// 表示是否在等待命令的输入
        /// </summary>
        public bool IsWaitForKey
        {
            get
            {
                return ThreeHotKeyManager.IsWaitForSecondKey || ThreeHotKeyManager.IsWaitForSecondModifier;
            }
        }
        private void InitializeHotKey()
        {
            KeyPartTwo keyPart;
            GlobalThreeHotKey hotKey;
            foreach (var command in GlobalCommand.commands)
            {
                if (command == GlobalCommand.AddNewFuncBlockCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.W, Key.F);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+W,F)\t{0}", Properties.Resources.MainWindow_Add_FuncBlock);
                    continue;
                }
                if (command == GlobalCommand.AddNewModbusCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.W, Key.M);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+W,M)\t{0}", Properties.Resources.MainWindow_Add_Modbus_Table);
                    continue;
                }
                if (command == GlobalCommand.AddNewSubRoutineCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.W, Key.R);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+W,R)\t{0}", Properties.Resources.MainWindow_Add_SubRoutine);
                    continue;
                }
                if (command == GlobalCommand.CheckFuncBlockCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.F);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,F)\t{0}", Properties.Resources.MainWindow_Funcblock_Check);
                    continue;
                }
                if (command == GlobalCommand.CheckNetworkErrorCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.N);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,N)\t{0}", Properties.Resources.MainWindow_Ladder_Check);
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
                    hotKey.ShowMessage = string.Format("(Ctrl+T,D)\t{0}", Properties.Resources.Download);
                    continue;
                }
                if (command == GlobalCommand.MonitorCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.M);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,M)\t{0}", Properties.Resources.Monitor);
                    continue;
                }
                if (command == GlobalCommand.UploadCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.U);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,U)\t{0}", Properties.Resources.Upload);
                    continue;
                }
                if (command == GlobalCommand.EditCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.E);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,E)\t{0}", Properties.Resources.MainWindow_Ret_Edit_Mode);
                    continue;
                }
                if (command == GlobalCommand.ZoomInCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.I);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,I)\t{0}", Properties.Resources.Zoom_In);
                    continue;
                }
                if (command == GlobalCommand.ZoomOutCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.T, Key.O);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+T,O)\t{0}", Properties.Resources.Zoom_Out);
                    continue;
                }
                if (command == GlobalCommand.SimulateCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.S);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,S)\t{0}", Properties.Resources.Simulate);
                    continue;
                }
                if (command == GlobalCommand.MonitorCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.E, Key.M);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+E,M)\t{0}", Properties.Resources.Monitor);
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
                    hotKey.ShowMessage = string.Format("(Ctrl+F4,O)\t{0}", Properties.Resources.MainWindow_Monitor_List);
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
                    hotKey.ShowMessage = string.Format("(Ctrl+F7,O)\t{0}", Properties.Resources.MainWindow_Property_Proj);
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
                if (command == GlobalCommand.CloseProjectCommand)
                {
                    keyPart = new KeyPartTwo(ModifierKeys.Control, Key.Q, Key.E);
                    hotKey = new GlobalThreeHotKey(this, command, keyPart);
                    ThreeHotKeyManager.AddHotKey(keyPart, hotKey);
                    hotKey.ShowMessage = string.Format("(Ctrl+Q,E)\t{0}", Properties.Resources.Close_Proj);
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
            if (_threeHotKeys.Count == 0)
            {
                if (KeyInputHelper.IsModifier(e.Key))
                {
                    return;
                }
                if (((e.KeyboardDevice.Modifiers ^ ModifierKeys.Control) == ModifierKeys.None))
                {
                    _threeHotKeys = ThreeHotKeyManager.GetHotKeys(ModifierKeys.Control, e.Key);
                    if (_threeHotKeys.Count > 0)
                    {
                        _interactionFacade.SetMessage(string.Format("(Ctrl+{0}){1}", e.Key, Properties.Resources.Key_Pressed));
                        ThreeHotKeyManager.IsWaitForSecondModifier = true;
                        ThreeHotKeyManager.IsWaitForSecondKey = true;
                        SB_Message.ToolTip = GenToolTipByHotKeys();
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
                                    _interactionFacade.SetMessage(Properties.Resources.Ready);
                                    hotKey.Execute();
                                }
                                else
                                {
                                    _interactionFacade.SetMessage(Properties.Resources.Command_Not_Execute);
                                }
                                break;
                            }
                        }
                        if (_threeHotKeys.Count != 0)
                        {
                            _interactionFacade.SetMessage(string.Format("{0}{1}{2}{3}", Properties.Resources.Key_Combination, _threeHotKeys.First(), inputMessage, Properties.Resources.Not_Command));
                            _threeHotKeys.Clear();
                        }
                    }
                    else
                    {
                        _interactionFacade.SetMessage(string.Format("{0}{1}{2}{3}", Properties.Resources.Key_Combination, _threeHotKeys.First(), inputMessage, Properties.Resources.Not_Command));
                        _threeHotKeys.Clear();
                    }
                    ThreeHotKeyManager.IsWaitForSecondModifier = false;
                    ThreeHotKeyManager.IsWaitForSecondKey = false;
                    inputMessage = string.Empty;
                    SB_Message.ToolTip = null;
                }
                e.Handled = true;
            }
        }
        #endregion
        
        private void FileRegister()
        {
            FileTypeRegInfo fileTypeRegInfo = new FileTypeRegInfo(string.Format(".{0}", FileHelper.ExtensionName));
            fileTypeRegInfo.Description = "SamSoarII文件类型";
            fileTypeRegInfo.ExePath = System.Windows.Forms.Application.ExecutablePath;
            fileTypeRegInfo.ExtendName = string.Format(".{0}", FileHelper.ExtensionName);
            fileTypeRegInfo.IconPath = System.Windows.Forms.Application.ExecutablePath;
            if (!FileTypeRegister.FileTypeRegistered(string.Format(".{0}", FileHelper.ExtensionName)))
            {
                // 注册
                FileTypeRegister fileTypeRegister = new FileTypeRegister();
                FileTypeRegister.RegisterFileType(fileTypeRegInfo);
            }
            else
                FileTypeRegister.UpdateFileTypeRegInfo(fileTypeRegInfo);
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
            LayoutSetting.Load();
            InitializeAvalonDock(LAProj);
            InitializeAvalonDock(LAFind);
            InitializeAvalonDock(LAReplace);
            InitializeAvalonDock(LAMonitor);
            InitializeAvalonDock(LAErrorList);
            InitializeAvalonDock(LAElemList);
            InitializeAvalonDock(LAElemInit);
            InitializeAvalonDock(LABreakpoint);
            //DockManager.Theme = new VS2010Theme();
            
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
        private void OnClick(object sender, RoutedEventArgs e)
        {
            InstShortCutOpen.Invoke(sender, e);
        }
        private void OnShowAboutDialog(object sender, RoutedEventArgs e)
        {
            LocalizedMessageBox.Show("Version Number:1.0.8", Properties.Resources.About,LocalizedMessageIcon.Information);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!GlobalSetting.LoadLadderScaleSuccess())
            {
                ILayoutPositionableElementWithActualSize _maintab = MainTab;
                GlobalSetting.LadderOriginScaleX = _maintab.ActualWidth / 4340;
                GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / 4340;
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
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                ElementList.InitializeElementCollection();
                InstructionCommentManager.MappedMessageChanged += ElementList.InstructionCommentManager_MappedMessageChanged;
                ValueCommentManager.ValueCommentChanged += ElementList.ValueCommentManager_ValueCommentChanged;
                ValueAliasManager.ValueAliasChanged += ElementList.ValueAliasManager_ValueAliasChanged;
                if (App.AutoOpenFileFullPath != string.Empty)
                {
                    _interactionFacade.LoadProject(App.AutoOpenFileFullPath);
                    LACProj.Show();
                }
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
                LocalizedMessageBox.Show(string.Format("{0}",Properties.Resources.Message_File_Moved),LocalizedMessageIcon.Information);
                ProjectFileManager.Delete(index);
            }
            else
            {
                if (_interactionFacade.ProjectLoaded && projectMessage.Value.Item2 == _interactionFacade.ProjectFullFileName)
                {
                    LocalizedMessageBox.Show(string.Format("{0}", Properties.Resources.Message_Project_Loaded), LocalizedMessageIcon.Information);
                }
                else
                {
                    if (CurrentProjectHandle(false, false))
                    {
                        _interactionFacade.LoadProject(projectMessage.Value.Item2);
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
        public LocalizedMessageResult ShowSaveYesNoCancelDialog()
        {
            string title = Properties.Resources.Message_Confirm_Save;
            string text = String.Format("{0:s} {1}", _interactionFacade.ProjectModel.ProjectName, Properties.Resources.Message_Changed);
            return LocalizedMessageBox.Show(text, title, LocalizedMessageButton.YesNoCancel, LocalizedMessageIcon.Question);
        }
        #endregion

        #region Command can Execute
        private void OnNewProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsWaitForKey;
        }

        private void OnOpenProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsWaitForKey;
        }

        private void OnProcessExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsWaitForKey;
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
                e.CanExecute = _interactionFacade.ProjectLoaded && !IsWaitForKey;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void AddNewSubRoutineCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded && _interactionFacade.ProjectModel.LadderMode == LadderMode.Edit;
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
                e.CanExecute = _interactionFacade.ProjectLoaded && _interactionFacade.ProjectModel.LadderMode == LadderMode.Edit;
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
                e.CanExecute = _interactionFacade.ProjectLoaded && _interactionFacade.ProjectModel.LadderMode == LadderMode.Edit;
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
        /// <summary>
        /// 进行关闭，打开，创建等影响当前工程的操作时调用
        /// </summary>
        /// <param name="CreateNewProject">结束操作后是否创建新工程</param>
        /// <param name="OpenProject">结束操作后是否加载工程</param>
        /// <returns>是否对当前工程进行了处理</returns>
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
                    LocalizedMessageResult mbret = ShowSaveYesNoCancelDialog();
                    switch (mbret)
                    {
                        case LocalizedMessageResult.Yes:
                            OnSaveProjectExecute(this, new RoutedEventArgs());
                            _interactionFacade.ProjectModel.IsModify = false;
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case LocalizedMessageResult.No:
                            _interactionFacade.ProjectModel.IsModify = false;
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case LocalizedMessageResult.Cancel:
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
                    LocalizedMessageResult mbret = ShowSaveYesNoCancelDialog();
                    switch (mbret)
                    {
                        case LocalizedMessageResult.Yes:
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = string.Format("{0}文件|*.{0}", FileHelper.ExtensionName);
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                _interactionFacade.ProjectFullFileName = saveFileDialog.FileName;
                                _interactionFacade.ProjectModel.ProjectName = FileHelper.GetFileName(saveFileDialog.FileName);
                                _interactionFacade.SaveProject();
                            }
                            else return false;
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case LocalizedMessageResult.No:
                            if (CreateNewProject)
                                NewProjectCreated();
                            if (OpenProject)
                                ProjectOpen();
                            return true;
                        case LocalizedMessageResult.Cancel:
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
                            LocalizedMessageBox.Show(Properties.Resources.Message_Path,LocalizedMessageIcon.Information);
                            return;
                        }
                        if (name == string.Empty)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Name, LocalizedMessageIcon.Information);
                            return;
                        }
                        string fullFileName = string.Format(@"{0}\{1}.{2}", dir, name,FileHelper.ExtensionName, LocalizedMessageIcon.Information);
                        if (File.Exists(fullFileName))
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Exist, LocalizedMessageIcon.Information);
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
            openFileDialog.Filter = string.Format("{0}文件|*.{0}", FileHelper.ExtensionName);
            if (openFileDialog.ShowDialog() == true)
            {
                if (_interactionFacade.ProjectFullFileName == openFileDialog.FileName)
                {
                    LocalizedMessageBox.Show(Properties.Resources.Message_Project_Loaded, LocalizedMessageIcon.Information);
                    return;
                }
                if (!OpenProject(openFileDialog.FileName))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Message_Project_Error, LocalizedMessageIcon.Information);
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
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
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
            _interactionFacade.SetMessage(Properties.Resources.Project_Saved);
            //SB_Message.Text = Properties.Resources.Project_Saved;
        }
        public void SaveProject()
        {
            if (ProjectTreeViewItem.HasRenaming)
                return;
            CurrentProjectHandle(false,false);
        }
        /// <summary>
        /// only called by app throw exceptions
        /// </summary>
        internal void SaveProjectByException()
        {
            if (_interactionFacade.ProjectModel == null) return;
            if (!_interactionFacade.ProjectModel.IsModify && _interactionFacade.ProjectFullFileName != string.Empty) return;
            if (_interactionFacade.ProjectFullFileName == string.Empty)
            {
                if (!Directory.Exists(FileHelper.AppRootPath + System.IO.Path.DirectorySeparatorChar + "Temp"))
                    Directory.CreateDirectory(FileHelper.AppRootPath + System.IO.Path.DirectorySeparatorChar + "Temp");
                int id = 1;
                _interactionFacade.ProjectFullFileName = string.Format("{0}{1}{2}.{3}", FileHelper.AppRootPath + System.IO.Path.DirectorySeparatorChar + "Temp", System.IO.Path.DirectorySeparatorChar, Properties.Resources.Project + id , FileHelper.ExtensionName);
                while (File.Exists(_interactionFacade.ProjectFullFileName))
                {
                    id++;
                    _interactionFacade.ProjectFullFileName = string.Format("{0}{1}{2}.{3}", FileHelper.AppRootPath + System.IO.Path.DirectorySeparatorChar + "Temp", System.IO.Path.DirectorySeparatorChar, Properties.Resources.Project + id, FileHelper.ExtensionName);
                }
                _interactionFacade.ProjectModel.ProjectName = FileHelper.GetFileName(_interactionFacade.ProjectFullFileName);
            }
            _interactionFacade.SaveProject();
        }

        private void OnSaveAsProjectExecute(object sender, RoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = string.Format("{0}文件|*.{0}", FileHelper.ExtensionName);
            if (saveFileDialog.ShowDialog() == true)
            {
                _interactionFacade.SaveAsProject(saveFileDialog.FileName);
            }
        }
        
        private void OnMonitorCommandExecute(object sender, RoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
                return;
            }
            if (_interactionFacade.ProjectModel.LadderMode == LadderMode.Edit)
            {
                var _ret = _interactionFacade.CommunicationTest();
                switch (_ret)
                {
                    case CheckRet.None:
                        _interactionFacade.ProjectModel.MMonitorManager.Initialize(_interactionFacade.ProjectModel);
                        _interactionFacade.ProjectModel.LadderMode = LadderMode.Monitor;
                        MonitorModeButton.IsChecked = true;
                        LACMonitor.Show();
                        break;
                    case CheckRet.CommunicationError:
                        MonitorModeButton.IsChecked = false;
                        LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                        break;
                    default:
                        MonitorModeButton.IsChecked = false;
                        break;
                }
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
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
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
            Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
            {
                SimuStartButton.IsChecked = true;
                SimuPauseButton.IsChecked = false;
                SimuStopButton.IsChecked = false;
                SimuStartButton.IsEnabled = false;
                SimuPauseButton.IsEnabled = true;
                SimuStopButton.IsEnabled = true;
            }));
        }

        private void OnSimuStartCommandExecute(object sender, RoutedEventArgs e)
        {
            SimulateHelper.SModel.Start();
        }
        
        private void OnSimulatePause(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
            {
                SimuStartButton.IsChecked = false;
                SimuPauseButton.IsChecked = true;
                SimuStopButton.IsChecked = false;
                SimuStartButton.IsEnabled = true;
                SimuPauseButton.IsEnabled = false;
                SimuStopButton.IsEnabled = true;
            }));
        }

        private void OnSimuPauseCommandExecute(object sender, RoutedEventArgs e)
        {
            SimulateHelper.SModel.Pause();
        }

        private void OnSimulateAbort(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
            {
                SimuStartButton.IsChecked = false;
                SimuPauseButton.IsChecked = false;
                SimuStopButton.IsChecked = true;
                SimuStartButton.IsEnabled = true;
                SimuPauseButton.IsEnabled = false;
                SimuStopButton.IsEnabled = false;
            }));
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
                        LocalizedMessageBox.Show(ex.Message, LocalizedMessageIcon.Error);
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
            //_interactionFacade.UploadProject();
        }

        private void OnDownloadCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (ProjectTreeViewItem.HasRenaming)
            {
                LocalizedMessageBox.Show(Properties.Resources.Item_Rename, LocalizedMessageIcon.Warning);
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
                var ret = _interactionFacade.CommunicationTest();
                if (ret == CheckRet.CommunicationError)
                {
                    LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                }
                else if(ret == CheckRet.None)
                {
                    LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Success, LocalizedMessageIcon.Information);
                }
                dialog.Close();
            };
            dialog.CommunicationTest += (sender1, e1) =>
            {
                var ret = _interactionFacade.CommunicationTest();
                if (ret == CheckRet.CommunicationError)
                {
                    LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Failed, LocalizedMessageIcon.Information);
                }
                else if(ret == CheckRet.None)
                {
                    LocalizedMessageBox.Show(Properties.Resources.MessageBox_Communication_Success, LocalizedMessageIcon.Information);
                }
            };
            dialog.ShowDialog();
        }
        private void OnUpdateClick(object sender, RoutedEventArgs e)
        {
            //XmlGen.GenUpdateXML(Directory.GetCurrentDirectory());
            Process process = new Process();
            process.StartInfo.Verb = "runas";
            process.StartInfo.FileName = FileHelper.AppRootPath + @"\Update\SamSoarII.Update.exe";
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
                    LocalizedMessageBox.Show(Properties.Resources.New_Version, LocalizedMessageIcon.Information);
                }
                else if (message == string.Format("Y"))
                {
                    message = reader.ReadLine();
                    if (message == string.Format("WebException"))
                    {
                        LocalizedMessageBox.Show(Properties.Resources.Connect_Failed, LocalizedMessageIcon.Information);
                        return;
                    }
                    else if (message == string.Format("Exception"))
                    {
                        LocalizedMessageBox.Show(Properties.Resources.Download_Failed, LocalizedMessageIcon.Information);
                        return;
                    }else if (message == string.Empty)
                    {
                        LocalizedMessageBox.Show(Properties.Resources.Connect_Failed, LocalizedMessageIcon.Information);
                        return;
                    }
                    long filesize = long.Parse(message);
                    LocalizedMessageResult ret = LocalizedMessageBox.Show(string.Format(Properties.Resources.Update_Or_Not + "(" + Properties.Resources.Update_Process + ")\n" + Properties.Resources.Update_Size + "{0:f3}MB", filesize / (1024 * 1024 * 1.0)), Properties.Resources.Update_Whether, LocalizedMessageButton.OKCancel, LocalizedMessageIcon.Information);
                    StreamWriter writer = new StreamWriter(clientPipe);
                    if (ret == LocalizedMessageResult.Yes)
                        writer.WriteLine("Update");
                    else
                        writer.WriteLine("Cancel");
                    writer.Flush();
                    clientPipe.WaitForPipeDrain();
                }
                else if (message == string.Format("WebException"))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Connect_Failed, LocalizedMessageIcon.Information);
                }
                else if (message == string.Format("Exception"))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Download_Failed, LocalizedMessageIcon.Information);
                }
            }
            else
            {
                LocalizedMessageBox.Show(Properties.Resources.Connect_Failed, LocalizedMessageIcon.Information);
            }
        }
    }
}

