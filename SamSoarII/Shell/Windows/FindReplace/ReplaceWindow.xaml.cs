using SamSoarII.Core.Models;
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
    public partial class ReplaceWindow : UserControl, IWindow, INotifyPropertyChanged
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
                if (mode == value) return;
                this.mode = value;
                if (IsLoaded && IsEnabled && Visibility == Visibility.Visible) Find();
                PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
            }
        }
        /// <summary> 用户的输入信息，经过整理后得到的格式类 </summary>
        private ReplaceFormat RF_Input = new ReplaceFormat("");
        /// <summary> 用户要替换的信息，经过整理后得到的格式类 </summary>
        private ReplaceFormat RF_Change = new ReplaceFormat("");

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
            if (RF_Input.Mode == ReplaceFormat.Modes.Error) return;
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
                Find(current.View.SelectionArea.Core.SelectUnits);
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
                        if (RF_Input.Match(unit))
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
            // 等待UI线程暂停再操作
            IFParent.ThMNGView.Pause();
            _showdialog = showdialog;
            if (IFParent.ThMNGView.IsActive)
                IFParent.ThMNGView.Paused += OnViewThreadPauseToReplace;
            else
                _Replace();
        }

        private void OnViewThreadPauseToReplace(object sender, RoutedEventArgs e)
        {
            IFParent.ThMNGView.Paused -= OnViewThreadPauseToReplace;
            _Replace();
        }

        private bool _showdialog;
        private void _Replace()
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
                    command = RF_Change.Replace(unit, RF_Input);
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
            if (_showdialog || error > 0)
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
            IFParent.ThMNGView.Start();
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
            RF_Input = new ReplaceFormat(TB_Input.Text);
            switch (RF_Input.Mode)
            {
                case ReplaceFormat.Modes.Error:
                    // 输入为空时涂白色
                    if (TB_Input.Text.Length == 0)
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
            RF_Change = new ReplaceFormat(TB_Change.Text);
            switch (RF_Change.Mode)
            {
                case ReplaceFormat.Modes.Value:
                case ReplaceFormat.Modes.Base:
                    // 输入和替换的格式均为相同格式才合法
                    if (RF_Input.Mode == ReplaceFormat.Modes.Value
                     || RF_Input.Mode == ReplaceFormat.Modes.Base)
                        TB_Change.Background = Brushes.LightGreen;
                    // 输入为空时涂白色
                    else if (TB_Change.Text.Length == 0)
                        TB_Change.Background = Brushes.White;
                    // 非法时，将输入框涂红
                    else
                        TB_Change.Background = Brushes.Red;
                    break;
                case ReplaceFormat.Modes.Unit:
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
