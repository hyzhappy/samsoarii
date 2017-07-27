﻿using SamSoarII.Core.Models;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Global;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// ReplaceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceWindow : UserControl, IWindow
    {
        public ReplaceWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            DataContext = this;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            undos = new Stack<ReplaceCommandGroup>();
            redos = new Stack<ReplaceCommandGroup>();
        }

        /// <summary> 属性更改时触发 </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Numbers

        /// <summary> 主交互接口 </summary>
        private InteractionFacade ifParent;
        /// <summary> 主交互接口 </summary>
        public InteractionFacade IFParent { get { return this.ifParent; } }
        /// <summary> 工程 </summary>
        public ProjectModel Project { get { return ifParent.MDProj; } }
        /// <summary> 当前程序 </summary>
        private LadderDiagramModel current;
        /// <summary> 当前程序 </summary>
        public LadderDiagramModel Current
        {
            get
            {
                return this.current;
            }
            private set
            {
                if (current == value) return;
                if (current?.View != null) current.View.SelectionChanged -= OnCurrentSelectionChanged;
                this.current = value;
                if (current?.View != null) current.View.SelectionChanged += OnCurrentSelectionChanged;
            }
        }
        
        /// <summary> 当前查找到的元素 </summary>
        private ObservableCollection<ReplaceElement> items
            = new ObservableCollection<ReplaceElement>();
        /// <summary> 当前查找到的元素 </summary>
        public IEnumerable<ReplaceElement> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                Initialize(false);
                foreach (ReplaceElement item in value)
                    items.Add(item);
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }
        /// <summary> 模式：查找当前程序 </summary>
        public const int MODE_CURRENT = 0x00;
        /// <summary> 模式：查找所有程序 </summary>
        public const int MODE_ALL = 0x01;
        /// <summary> 模式：查找选择范围 </summary>
        public const int MODE_SELECT = 0x02;
        /// <summary> 模式 </summary>
        private int mode;
        /// <summary> 模式 </summary>
        public int Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
                Find();
            }
        }
        /// <summary> 用户的输入信息，经过整理后得到的格式类 </summary>
        private ReplaceFormat RF_Input { get; set; } = new ReplaceFormat();
        /// <summary> 用户要替换的信息，经过整理后得到的格式类 </summary>
        private ReplaceFormat RF_Change { get; set; } = new ReplaceFormat();

        #endregion

        #region Commands

        private Stack<ReplaceCommandGroup> undos;

        private Stack<ReplaceCommandGroup> redos;
        
        public bool CanUndo { get { return undos != null && undos.Count() > 0; } }

        public bool CanRedo { get { return redos != null && redos.Count() > 0; } }

        public void Undo()
        {
            if (!CanUndo) return;
            ReplaceCommandGroup cmd = undos.Pop();
            cmd.Undo();
            redos.Push(cmd);
            IFParent.MDProj.InvokeModify(this);
            Find();
        }

        public void Redo()
        {
            if (!CanRedo) return;
            ReplaceCommandGroup cmd = redos.Pop();
            cmd.Redo();
            undos.Push(cmd);
            IFParent.MDProj.InvokeModify(this);
            Find();
        }

        public void ClearCommands()
        {
            foreach (ReplaceCommandGroup cmd in undos) cmd.Dispose();
            foreach (ReplaceCommandGroup cmd in redos) cmd.Dispose();
            undos.Clear();
            redos.Clear();
        }
        
        #endregion

        /// <summary> 初始化 </summary>
        /// <param name="initcmd">是否初始化撤销操作板</param>
        public void Initialize(bool initcmd = true)
        {
            foreach (ReplaceElement item in items) item.Dispose();
            items.Clear();
            if (initcmd) ClearCommands();
        }
        /// <summary> 查找指令 </summary>
        private void Find()
        {
            Initialize(false);
            // 输入信息非法则不执行
            if (RF_Input.Type == ReplaceFormat.TYPE_INVALID) return;
            switch (Mode)
            {
                // 查找当前程序或者选择范围
                case MODE_CURRENT:
                case MODE_SELECT:
                    Find(current);
                    break;
                // 查找所有程序
                case MODE_ALL:
                    Find(Project.MainDiagram);
                    foreach (LadderDiagramModel diagram in Project.Diagrams)
                        if (!diagram.IsMainLadder) Find(diagram);
                    break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }
        /// <summary>
        /// 在给定程序内查找指令
        /// </summary>
        /// <param name="ldvmodel"></param>
        private void Find(LadderDiagramModel diagram)
        {
            // 选择范围内查找
            if (Mode == MODE_SELECT)
            {
                if (current?.View == null || current.View.SelectionStatus != Models.SelectStatus.MultiSelected) return;
                if (current.View.CrossNetState == Models.CrossNetworkState.NoCross)
                    Find(current.View.SelectStartNetwork.GetSelectedElements());
                else
                {
                    if (current.View.SelectStartNetwork != null)
                        Find(current.View.SelectStartNetwork.Core.Children);
                    foreach (LadderNetworkViewModel netview in current.View.SelectAllNetworks)
                        Find(netview.Core.Children);
                }
            }
            // 遍历所有网络
            else
            {
                foreach (LadderNetworkModel network in diagram.Children)
                    Find(network.Children);
            }
        }

        private void Find(IEnumerable<LadderUnitModel> units)
        {
            // 遍历所有元件
            foreach (LadderUnitModel unit in units)
            {
                if (!unit.IsUsed) continue;
                switch (unit.Shape)
                {
                    // 忽略连线
                    case LadderUnitModel.Shapes.HLine:
                    case LadderUnitModel.Shapes.VLine:
                        break;
                    // 检查元件
                    default:
                        if (RF_Input.Match(unit.ToInstString()))
                            items.Add(new ReplaceElement(this, unit));
                        break;
                }
            }
        }
        
        /// <summary>
        /// 将选定的指令替换
        /// </summary>
        /// <param name="showdialog">是否显示提示窗口</param>
        private void Replace(bool showdialog = true)
        {
            // 替换成功和失败的个数，以及详细的错误信息
            int success = 0;
            int error = 0;
            string errormsg = String.Empty;
            // 建立替换的命令集
            ReplaceCommandGroup group = new ReplaceCommandGroup();
            // 处理所有在表中选择的元素
            foreach (ReplaceElement rele in DG_List.SelectedItems)
            {
                // 获得相关信息
                LadderUnitModel unit = rele.Unit;
                LadderNetworkModel network = unit.Parent;
                LadderDiagramModel diagram = network.Parent;
                int x = unit.X;
                int y = unit.Y;
                // 建立对应的替换命令并运行
                ReplaceCommand command = null;
                try
                {
                    command = RF_Change.Replace(RF_Input, unit);
                    command.Redo();
                    group.Add(command);
                    success++;
                }
                catch (Exception exce2)
                {
                    error++;
                    if (Thread.CurrentThread.CurrentUICulture.Name.Contains("zh-Hans"))
                        errormsg += String.Format("在{0:s}的网络{1:d}的坐标({2:d},{3:d})处发生错误：{4:s}\r\n",
                            diagram.Name, network.ID, x, y, exce2.Message);
                    else
                        errormsg += String.Format("An error occurred at the coordinates({2:d},{3:d})of the network {1:d} of {0:s}: {4:s}\r\n",
                            diagram.Name, network.ID, x, y, exce2.Message);
                }
            }
            // 将运行成功的命令加入撤销栈中
            if (success > 0)
            {
                redos.Clear();
                undos.Push(group);
                IFParent.MDProj.InvokeModify(this);
                Find();
            }
            // 当需要显示结果，或者出现错误替换时显示
            if (showdialog || error > 0)
            {
                ReplaceReportWindow report = new ReplaceReportWindow();
                if (Thread.CurrentThread.CurrentUICulture.Name.Contains("zh-Hans"))
                    report.TB_Subtitle.Text = String.Format("总共进行了{0:d}次替换，{1:d}次成功，{2:d}次错误。"
                        , success + error, success, error);
                else
                    report.TB_Subtitle.Text = string.Format("A total of {0:d} times, {1:d} success, {2:d} Fail."
                    , success + error, success, error);
                report.TB_Message.Text = errormsg;
                report.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                report.ShowDialog();
            }
        }

        #region Event Handler
        
        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            // TAB界面发过来的事件
            if (sender is MainTabControl && e is MainTabControlEventArgs)
            {
                MainTabControlEventArgs e1 = (MainTabControlEventArgs)e;
                if (e1.Action == TabAction.SELECT)
                {
                    // 当前界面是梯形图程序时进行重查
                    if (e1.Tab is MainTabDiagramItem)
                    {
                        Visibility = Visibility.Visible;
                        Current = ((MainTabDiagramItem)(e1.Tab)).LDVModel.Core;
                        if (Mode == MODE_CURRENT) Find();
                    }
                    // 否则隐藏窗口
                    else
                    {
                        Visibility = Visibility.Hidden;
                        Current = null;
                    }
                }
            }
            // 主交互发过来的事件
            if (sender == ifParent && e is InteractionFacadeEventArgs)
            {
                InteractionFacadeEventArgs e2 = (InteractionFacadeEventArgs)e;
                switch (e2.Flags)
                {
                    case InteractionFacadeEventArgs.Types.DiagramModified:
                        Initialize();
                        break;
                }
            }
        }

        /// <summary>
        /// 当前梯形图的选择状态更改时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCurrentSelectionChanged(object sender, RoutedEventArgs e)
        {
            Mode = current?.View?.SelectionStatus == Models.SelectStatus.MultiSelected
                ? MODE_SELECT : MODE_CURRENT;
        }

        /// <summary>
        /// 当输入文本更改时触发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件信息</param>
        private void TB_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 检查输入格式
            RF_Input.Text = TB_Input.Text;
            switch (RF_Input.Type)
            {
                case ReplaceFormat.TYPE_INVALID:
                    // 输入为空时涂白色
                    if (RF_Input.Text.Length == 0)
                        TB_Input.Background = Brushes.White;
                    // 非法时，将输入框涂红
                    else
                        TB_Input.Background = Brushes.Red;
                    break;
                default:
                    // 合法时，将输入框涂绿，表示待操作
                    TB_Input.Background = Brushes.LightGreen;
                    break;
            }
        }

        /// <summary>
        /// 当在输入框内按键时触发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件信息</param>
        private void TB_Input_KeyDown(object sender, KeyEventArgs e)
        {
            // 输入框为红禁止操作
            if (TB_Input.Background == Brushes.Red) return;
            // 输入Enter才有效，进行查找操作
            if (e.Key != Key.Enter) return;
            Find();
            // 输入框涂白，表示已操作
            TB_Input.Background = Brushes.White;
        }

        /// <summary>
        /// 当替换文本更改时触发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件信息</param>
        private void TB_Change_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 检查输入格式
            RF_Change.Text = TB_Change.Text;
            switch (RF_Change.Type)
            {
                case ReplaceFormat.TYPE_REGISTER:
                    // 输入和替换的格式均为相同格式才合法
                    if (RF_Input.Type == RF_Change.Type)
                        TB_Change.Background = Brushes.LightGreen;
                    // 输入为空时涂白色
                    else if (RF_Change.Text.Length == 0)
                        TB_Change.Background = Brushes.White;
                    // 非法时，将输入框涂红
                    else
                        TB_Change.Background = Brushes.Red;
                    break;
                case ReplaceFormat.TYPE_LADDER:
                    // 梯形图格式合法
                    TB_Change.Background = Brushes.LightGreen;
                    break;
                default:
                    // 其他格式非法
                    TB_Change.Background = Brushes.Red;
                    break;
            }
        }

        /// <summary>
        /// 当在替换输入框内按键时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Change_KeyDown(object sender, KeyEventArgs e)
        {
            // 在表内按键也会触发这个事件，所以要覆盖掉
            if (called_DataGridCell_KeyDown)
            {
                called_DataGridCell_KeyDown = false;
                return;
            }
            // 确认已经查找过，并且替换格式合法
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            // 按下Enter进行替换
            if (e.Key != Key.Enter) return;
            TB_Change.IsEnabled = false;
            Replace();
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        /// <summary>
        /// 在表内按键会设为true
        /// </summary>
        private bool called_DataGridCell_KeyDown = false;

        /// <summary>
        /// 当在表内按键时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCell_KeyDown(object sender, KeyEventArgs e)
        {
            // 设置标志
            called_DataGridCell_KeyDown = true;
            // 确认满足条件，并执行替换
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            if (e.Key != Key.Enter) return;
            TB_Change.IsEnabled = false;
            Replace();
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        /// <summary>
        /// 当在表内双击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            TB_Change.IsEnabled = false;
            Replace(false);
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        /// <summary>
        /// 当表内选择元素变化时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 若当前窗口为停靠状态，可能会覆盖到导航的位置，
            // 所以先将窗口固定，保证主界面的完整显示
            if (!ifParent.WNDMain.LAReplace.IsFloat
             && !ifParent.WNDMain.LAReplace.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Search, ifParent.WNDMain.LAReplace.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Search, ifParent.WNDMain.LAReplace.AutoHideHeight.ToString());
                ifParent.WNDMain.LAReplace.ToggleAutoHide();
            }
            // 未选择元素则不导航
            if (DG_List.SelectedIndex < 0) return;
            // 导航到选择元素对应的位置
            ReplaceElement fele = items[DG_List.SelectedIndex];
            ifParent.Navigate(fele.Unit);
        }
        /// <summary>
        /// 当要判断是否能撤销时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanUndo;
        }
        /// <summary>
        /// 当要撤销操作时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }
        /// <summary>
        /// 当要判断是否能恢复时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanRedo;
        }
        /// <summary>
        /// 当要恢复操作时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }
        /// <summary>
        /// 当要显示设置选项界面时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConfigClick(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Hidden;
            G_Config.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 当确定设置时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BC_Ensure_Click(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Visible;
            G_Config.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 当取消设置时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BC_Cancel_Click(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Visible;
            G_Config.Visibility = Visibility.Hidden;
        }

        #endregion
        
    }
    
    /// <summary>
    /// 输入信息的格式
    /// </summary>
    public class ReplaceFormat
    {
        #region Type
        /// <summary> 格式类型：非法 </summary>
        public const int TYPE_INVALID = 0x00;
        /// <summary> 格式类型：梯形图指令 </summary>
        public const int TYPE_LADDER = 0x01;
        /// <summary> 格式类型：单个软元件 </summary>
        public const int TYPE_REGISTER = 0x02;
        /// <summary> 格式 </summary>
        public int Type { get; private set; }
        #endregion

        #region Arg
        /// <summary> 参数类型：非法 </summary>
        public const int ARG_INVAILD = 0x00;
        /// <summary> 参数类型：指令名称 </summary>
        public const int ARG_INSTRUCTION = 0x01;
        /// <summary> 参数类型：软元件 </summary>
        public const int ARG_REGISTER = 0x02;
        /// <summary> 参数类型：*（任意内容） </summary>
        public const int ARG_ANYONE = 0x03;
        /// <summary> 参数类型：.（任意后缀） </summary>
        public const int ARG_ANYSUFFIX = 0x04;
        /// <summary> 参数类型：函数名称 </summary>
        public const int ARG_NAME = 0x05;
        /// <summary> 参数类型：???（无参数）</summary>
        public const int ARG_UNDEFINED = 0x06;
        /// <summary> 参数格式 </summary>
        public struct ReplaceFormatArg
        {
            /// <summary> 数据类型 </summary>
            public int Type;
            /// <summary> 文本 </summary>
            public string Text;
            /// <summary> 基地址名称 </summary>
            public string Base;
            /// <summary> 基地址左范围 </summary>
            public int Low;
            /// <summary> 基地址右范围 </summary>
            public int High;
            /// <summary> 偏移地址名称 </summary>
            public string Offset;
            /// <summary> 偏移地址左范围 </summary>
            public int OLow;
            /// <summary> 偏移地址右范围 </summary>
            public int OHigh;
        }
        /// <summary> 所有参数 </summary>
        private ReplaceFormatArg[] args = new ReplaceFormatArg[0];
        /// <summary> 参数数量 </summary>
        public int ArgsCount { get { return args.Length; } }
        /// <summary>
        /// 根据标号来获取参数
        /// </summary>
        /// <param name="id">标号</param>
        /// <returns></returns>
        public ReplaceFormatArg GetArgs(int id) { return args[id]; }
        #endregion

        #region Regex
        /// <summary> 识别（X0）的正则符 </summary>
        private static Regex VRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary> 识别（X[0..9]）的正则符 </summary>
        private static Regex ARegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)\[([0-9]+)\.\.([0-9]+)\]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary> 识别（X0V0）的正则符 </summary>
        private static Regex VVRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)(V|Z)([0-9]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary> 识别（X[0..9]V0）的正则符 </summary>
        private static Regex AVRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)\[([0-9]+)\.\.([0-9]+)\](V|Z)([0-9]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary> 识别（X0V[0..3]）的正则符 </summary>
        private static Regex VARegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)(V|Z)\[([0-9]+)\.\.([0-9]+)\]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary> 识别（X[0..9]V[0..3]）的正则符 </summary>
        private static Regex AARegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)\[([0-9]+)\.\.([0-9]+)\](V|Z)\[([0-9]+)\.\.([0-9]+)\]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        /// <summary> 文本 </summary>
        private string text;
        /// <summary> 文本 </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                // 空文本非法，参数个数为0
                if (text.Equals(String.Empty))
                {
                    this.args = new ReplaceFormatArg[0];
                    Type = TYPE_INVALID;
                    return;
                }
                // 根据空格分隔参数
                string[] sargs = text.Split(' ');
                this.args = new ReplaceFormatArg[sargs.Length];
                // 分析文本项，建立每个参数
                for (int i = 0; i < args.Length; i++)
                {
                    // 初始化参数信息
                    args[i].Text = sargs[i];
                    args[i].Base = String.Empty;
                    args[i].Low = args[i].High = 0;
                    args[i].Offset = String.Empty;
                    args[i].OLow = args[i].OHigh = 0;
                    // 分析文本项
                    switch (sargs[i])
                    {
                        // 指令的空参数
                        case "?":
                        case "???":
                            args[i].Type = ARG_UNDEFINED;
                            break;
                        // 任意项匹配
                        case "*":
                            args[i].Type = ARG_ANYONE;
                            break;
                        // 任意后缀匹配
                        case ".":
                            args[i].Type = ARG_ANYSUFFIX;
                            break;
                        // 分析其他格式（寄存器，指令名称，函数名称）
                        default:
                            args[i].Type = ARG_REGISTER;
                            Match m1 = VVRegex.Match(sargs[i]);
                            Match m2 = AVRegex.Match(sargs[i]);
                            Match m3 = VARegex.Match(sargs[i]);
                            Match m4 = AARegex.Match(sargs[i]);
                            Match m5 = VRegex.Match(sargs[i]);
                            Match m6 = ARegex.Match(sargs[i]);
                            // 寄存器格式：X0V0
                            if (m1.Success)
                            {
                                args[i].Base = m1.Groups[1].Value;
                                args[i].Low = int.Parse(m1.Groups[2].Value);
                                args[i].High = args[i].Low;
                                args[i].Offset = m1.Groups[3].Value;
                                args[i].OLow = int.Parse(m1.Groups[4].Value);
                                args[i].OHigh = args[i].OLow;
                            }
                            // 寄存器格式：X[0..9]V0
                            else if (m2.Success)
                            {
                                args[i].Base = m2.Groups[1].Value;
                                args[i].Low = int.Parse(m2.Groups[2].Value);
                                args[i].High = int.Parse(m2.Groups[3].Value);
                                args[i].Offset = m2.Groups[4].Value;
                                args[i].OLow = int.Parse(m2.Groups[5].Value);
                                args[i].OHigh = args[i].OLow;
                            }
                            // 寄存器格式：X0V[0..3]
                            else if (m3.Success)
                            {
                                args[i].Base = m3.Groups[1].Value;
                                args[i].Low = int.Parse(m3.Groups[2].Value);
                                args[i].High = args[i].Low;
                                args[i].Offset = m3.Groups[3].Value;
                                args[i].OLow = int.Parse(m3.Groups[4].Value);
                                args[i].OHigh = int.Parse(m3.Groups[5].Value);
                            }
                            // 寄存器格式：X[0..9]V[0..3]
                            else if (m4.Success)
                            {
                                args[i].Base = m4.Groups[1].Value;
                                args[i].Low = int.Parse(m4.Groups[2].Value);
                                args[i].High = int.Parse(m4.Groups[3].Value);
                                args[i].Offset = m4.Groups[4].Value;
                                args[i].OLow = int.Parse(m4.Groups[5].Value);
                                args[i].OHigh = int.Parse(m4.Groups[6].Value);
                            }
                            // 寄存器格式：X0
                            else if (m5.Success)
                            {
                                args[i].Base = m5.Groups[1].Value;
                                args[i].Low = int.Parse(m5.Groups[2].Value);
                                args[i].High = args[i].Low;
                            }
                            // 寄存器格式：X[0..9]
                            else if (m6.Success)
                            {
                                args[i].Base = m6.Groups[1].Value;
                                args[i].Low = int.Parse(m6.Groups[2].Value);
                                args[i].High = int.Parse(m6.Groups[3].Value);
                            }
                            // 第一个参数必须为指令名称
                            else if (i == 0)
                            {
                                if (LadderUnitModel.NameOfTypes.Contains(args[i].Text))
                                {
                                    args[i].Type = ARG_INSTRUCTION;
                                }
                                else
                                {
                                    args[i].Type = ARG_INVAILD;
                                }
                            }
                            // 其他位置可能是函数名称
                            else
                            {
                                args[i].Type = ARG_NAME;
                            }
                            break;
                    }
                    // 根据参数格式来确定信息格式
                    switch (args[i].Type)
                    {
                        // 格式非法
                        case ARG_INVAILD:
                            Type = TYPE_INVALID;
                            return;
                        // 指令名称出现在不正确的位置
                        case ARG_INSTRUCTION:
                            if (i > 0)
                            {
                                Type = TYPE_INVALID;
                                return;
                            }
                            break;
                        // 指令参数
                        case ARG_REGISTER:
                        case ARG_NAME:
                        case ARG_UNDEFINED:
                            // 多参数并且位置非法
                            if (i == 0 && sargs.Length > 1)
                            {
                                Type = TYPE_INVALID;
                                return;
                            }
                            // 单独出现的寄存器格式
                            if (i == 0)
                            {
                                Type = TYPE_REGISTER;
                                return;
                            }
                            break;
                        // 任意项匹配
                        case ARG_ANYONE:
                            break;
                        // 任意后缀匹配，保证出现在末尾
                        case ARG_ANYSUFFIX:
                            if (i < sargs.Length - 1)
                            {
                                Type = TYPE_INVALID;
                                return;
                            }
                            break;
                    }
                }
                Type = TYPE_LADDER;
            }
        }
        /// <summary> 构造函数 </summary>
        public ReplaceFormat()
        {
            Text = String.Empty;
        }
        /// <summary> 构造函数 </summary>
        /// <param name="_text">文本</param>
        public ReplaceFormat(string _text)
        {
            Text = _text;
        }
        /// <summary> 对指令文本进行匹配检查 </summary>
        public bool Match(string input)
        {
            // 格式化指令文本
            ReplaceFormat iformat = new ReplaceFormat(input);
            switch (Type)
            {
                // 非法格式不能匹配
                case TYPE_INVALID:
                    return false;
                // 指令格式逐个匹配
                case TYPE_LADDER:
                    for (int i = 0; i < ArgsCount; i++)
                    {
                        // 【任意后缀】参数，匹配成功
                        if (GetArgs(i).Type == ARG_ANYSUFFIX)
                            return true;
                        // 超过了指令文本的参数个数，匹配失败
                        if (iformat.ArgsCount <= i)
                            return false;
                        // 该项匹配失败
                        if (!Match(GetArgs(i), iformat.GetArgs(i)))
                            return false;
                    }
                    // 通过所有检查，匹配成功
                    return true;
                // 寄存器格式需要查找
                case TYPE_REGISTER:
                    // 指令无参数
                    if (iformat.ArgsCount < 2)
                    {
                        return false;
                    }
                    // 查找是否存在符合匹配的指令参数
                    for (int i = 1; i < iformat.ArgsCount; i++)
                    {
                        if (Match(GetArgs(0), iformat.GetArgs(i)))
                            return true;
                    }
                    // 未找到，匹配失败
                    return false;
                // 其他格式不能匹配
                default:
                    return false;
            }
        }
        /// <summary>
        /// 检查两个参数项是否匹配
        /// </summary>
        /// <param name="arg1">参数项1</param>
        /// <param name="arg2">参数项2</param>
        /// <returns>匹配结果</returns>
        private bool Match(ReplaceFormatArg arg1, ReplaceFormatArg arg2)
        {
            // 根据项1的格式
            switch (arg1.Type)
            {
                // 非法格式，匹配失败
                case ARG_INVAILD:
                    return false;
                // 指令名称，是否相同
                case ARG_INSTRUCTION:
                    return (arg2.Type == ARG_INSTRUCTION && arg2.Text.Equals(arg1.Text));
                // 任意匹配，一定成功
                case ARG_ANYONE:
                    return true;
                case ARG_ANYSUFFIX:
                    return true;
                // 寄存器格式
                case ARG_REGISTER:
                    // 基地址是否相同
                    if (!arg1.Base.Equals(arg2.Base))
                        return false;
                    // 范围是否相交
                    if (arg2.High < arg1.Low)
                        return false;
                    if (arg2.Low > arg1.High)
                        return false;
                    // 若存在偏移地址则同理
                    if (!arg1.Offset.Equals(String.Empty))
                    {
                        if (!arg1.Offset.Equals(arg2.Offset))
                            return false;
                        if (arg2.OHigh < arg1.OLow)
                            return false;
                        if (arg2.OLow > arg1.OHigh)
                            return false;
                    }
                    return true;
                // 名称格式，是否相同
                case ARG_NAME:
                    return arg2.Type == ARG_NAME ? arg1.Text.Equals(arg2.Text) : false;
                // 是否都为未定义的参数
                case ARG_UNDEFINED:
                    return (arg2.Type == ARG_UNDEFINED);
                // 格式未知，匹配失败
                default:
                    return false;
            }
        }
        /// <summary>
        /// 根据该格式进行替换操作，生成替换命令
        /// </summary>
        /// <param name="prototype">查找格式</param>
        /// <param name="unit">要替换的元件</param>
        /// <returns>替换命令</returns>
        public ReplaceCommand Replace(ReplaceFormat prototype, LadderUnitModel unit)
        {
            string input = unit.ToInstString();
            // 将指令文本转为正则格式
            ReplaceFormat iformat = new ReplaceFormat(input);
            // 要匹配的参数项
            ReplaceFormatArg arg;
            arg.Type = ARG_INVAILD;
            arg.Low = arg.High = arg.OLow = arg.OHigh = 0;
            arg.Offset = arg.Base = arg.Text = String.Empty;
            // 替换后的指令文本
            string output = String.Empty;
            // 检查原指令的每个参数
            for (int i = 0; i < iformat.ArgsCount; i++)
            {
                // 替换格式为寄存器格式
                if (prototype.Type == TYPE_REGISTER)
                {
                    // 查找格式也为寄存器格式，并且不匹配
                    if (Type == TYPE_REGISTER
                     && !Match(prototype.GetArgs(0), iformat.GetArgs(i)))
                    {
                        // 保留原样
                        output += iformat.GetArgs(i).Text + " ";
                    }
                    else
                    {
                        arg.Type = ARG_INVAILD;
                        // 寄存器格式只读第一个参数
                        if (Type == TYPE_REGISTER)
                        {
                            arg = GetArgs(0);
                        }
                        // 指令格式读相应位置的参数
                        else if (i <= ArgsCount)
                        {
                            arg = GetArgs(i);
                        }
                        // 当前的参数格式
                        switch (arg.Type)
                        {
                            // 指令名称
                            case ARG_INSTRUCTION:
                                // 对应位置不是指令时保留原样
                                if (iformat.GetArgs(i).Type != ARG_INSTRUCTION)
                                {
                                    output += iformat.GetArgs(i).Text + " ";
                                    break;
                                }
                                // 替换对应的指令名称
                                output += arg.Text + " ";
                                break;
                            // 寄存器
                            case ARG_REGISTER:
                                // 对应位置不是寄存器时保留原样
                                if (iformat.GetArgs(i).Type != ARG_REGISTER
                                 && iformat.GetArgs(i).Type != ARG_UNDEFINED)
                                {
                                    output += iformat.GetArgs(i).Text + " ";
                                    break;
                                }
                                // 判断查找模型的这个参数是否制定唯一的寄存器
                                bool isunique = false;
                                isunique = (!prototype.GetArgs(0).Base.Equals(arg.Base));
                                isunique |= (prototype.GetArgs(0).Low == prototype.GetArgs(0).High);
                                isunique |= (arg.Low == arg.High);
                                int baseid = arg.Low;
                                // 不是唯一的情况下需要地址转移
                                if (!isunique)
                                    baseid += iformat.GetArgs(i).Low - prototype.GetArgs(0).Low;
                                // 得到对应的转换后的寄存器的文本
                                output += String.Format("{0:s}{1:d}",
                                        arg.Base, baseid);
                                if (!arg.Offset.Equals(String.Empty))
                                    output += String.Format("{0:s}{1:d}",
                                        arg.Offset, arg.OLow);
                                output += " ";
                                break;
                            // 其他格式
                            default:
                                output += iformat.GetArgs(i).Text + " ";
                                break;
                        }
                    }
                    continue;
                }
                // 超出的部分保留原样
                if (i >= ArgsCount)
                {
                    output += iformat.GetArgs(i).Text + " ";
                    continue;
                }
                // 根据当前参数的格式
                switch (GetArgs(i).Type)
                {
                    // 指令名称
                    case ARG_INSTRUCTION:
                        output += GetArgs(i).Text + " ";
                        break;
                    // 寄存器
                    case ARG_REGISTER:
                        // 策略和上面一致
                        bool isunique = false;
                        isunique |= (i >= prototype.ArgsCount);
                        if (!isunique)
                        {
                            isunique = (prototype.GetArgs(i).Low == prototype.GetArgs(i).High);
                            isunique |= (GetArgs(i).Low == GetArgs(i).High);
                        }
                        int baseid = GetArgs(i).Low;
                        if (!isunique)
                            baseid += iformat.GetArgs(i).Low - prototype.GetArgs(i).Low;
                        output += String.Format("{0:s}{1:d}",
                                GetArgs(i).Base, baseid);
                        if (!GetArgs(i).Offset.Equals(String.Empty))
                            output += String.Format("{0:s}{1:d}",
                                GetArgs(i).Offset, GetArgs(i).OLow);
                        output += " ";
                        break;
                    // 其他格式
                    default:
                        output += iformat.GetArgs(i).Text + " ";
                        break;
                }
            }
            // 建立命令并返回
            return new ReplaceCommand(unit, output);
        }
    }
    /// <summary> 替换命令 </summary>
    public class ReplaceCommand : IDisposable
    {
        public ReplaceCommand(LadderUnitModel _unit, string _output)
        {
            oldunit = _unit;
            network = oldunit.Parent;
            string[] _newtexts = _output.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            LadderUnitModel.Types newtype = LadderUnitModel.TypeOfNames[_newtexts[0]];
            string[] newargs = new string[_newtexts.Length - 1];
            Array.Copy(_newtexts, 1, newargs, 0, newargs.Length);
            newunit = new LadderUnitModel(null, newtype) { X = oldunit.X, Y = oldunit.Y };
            newunit.Parse(newargs);
        }

        public void Dispose()
        {
            oldunit = null;
            newunit = null;
        }

        #region Number

        private LadderNetworkModel network;
        private LadderUnitModel oldunit;
        private LadderUnitModel newunit;

        #endregion

        #region Undo & Redo
        
        public void Redo()
        {
            network.Remove(oldunit);
            try
            {
                network.Add(newunit);
            }
            catch (LadderUnitChangedEventException exce)
            {
                network.Add(oldunit);
                throw exce;
            }
        }

        public void Undo()
        {
            network.Remove(newunit);
            try
            {
                network.Add(oldunit);
            }
            catch (LadderUnitChangedEventException exce)
            {
                network.Add(newunit);
                throw exce;
            }
        }

        #endregion
    }
    /// <summary> 替换命令的集合 </summary>
    public class ReplaceCommandGroup : IDisposable
    {
        public ReplaceCommandGroup()
        {
            commands = new List<ReplaceCommand>();
        }

        public void Dispose()
        {
            foreach (ReplaceCommand cmd in commands) cmd.Dispose();
            commands.Clear();
        }

        private List<ReplaceCommand> commands;

        public void Add(ReplaceCommand cmd)
        {
            commands.Add(cmd);
        }

        public void Undo()
        {
            foreach (ReplaceCommand cmd in commands) cmd.Undo();
        }
        
        public void Redo()
        {
            foreach (ReplaceCommand cmd in commands) cmd.Redo();
        }

    }
    /// <summary> 要替换的元素 </summary>
    public class ReplaceElement : INotifyPropertyChanged, IDisposable
    {
        #region Numbers

        /// <summary> 替换窗口 </summary>
        private ReplaceWindow parent;

        /// <summary> 元件 </summary>
        private LadderUnitModel unit;
        /// <summary> 元件 </summary>
        public LadderUnitModel Unit { get { return this.unit; } }
        /// <summary> 元件文本 </summary>
        public string Detail { get { return unit.ToInstString(); } }
        /// <summary> 网络标号 </summary>
        public string Network { get { return unit.Parent.ID.ToString(); } }
        /// <summary> 程序名称 </summary>
        public string Diagram { get { return unit.Parent.Parent.Name; } }
        /// <summary> 是否被列表选择 </summary>
        private bool isselected;
        /// <summary> 是否被列表选择 </summary>
        public bool IsSelected
        {
            get { return this.isselected; }
            set
            {
                this.isselected = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        #endregion

        /// <summary> 构造函数 </summary>
        public ReplaceElement
        (
            ReplaceWindow _parent,
            LadderUnitModel _unit
        )
        {
            parent = _parent;
            unit = _unit;
        }

        public virtual void Dispose()
        {
            parent = null;
            unit = null;
        }

        #region Event Handler

        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #endregion
    }
}
