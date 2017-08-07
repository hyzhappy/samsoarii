using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindWindow : UserControl, IWindow, INotifyPropertyChanged
    {
        public FindWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            DataContext = this;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            items = new ObservableCollection<FindElement>();
        }
        
        /// <summary> 属性更改时触发 </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        /// <summary> 主交互接口 </summary>
        private InteractionFacade ifParent;
        /// <summary> 主交互接口 </summary>
        public InteractionFacade IFParent { get { return this.ifParent; } }
        /// <summary> 工程文件 </summary>
        public ProjectModel Project { get { return ifParent.MDProj; } }
        /// <summary> 当前梯形图 </summary>
        private LadderDiagramModel current;
        /// <summary> 当前查找到的元素 </summary>
        private ObservableCollection<FindElement> items;
        /// <summary> 当前查找到的元素 </summary>
        public IEnumerable<FindElement> Items { get { return this.items; } }
        /// <summary> 模式：查找所有程序 </summary>
        public const int MODE_CURRENT = 0x00;
        /// <summary> 模式：查找当前程序 </summary>
        public const int MODE_ALL = 0x01;
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
        private ReplaceFormat RF_Input = new ReplaceFormat("");

        #endregion
        
        /// <summary> 初始化 </summary>
        public void Initialize()
        {
            foreach (FindElement item in items) item.Dispose();
            items.Clear();
        }
        /// <summary> 查找指令 </summary>
        private void Find()
        {
            Initialize();
            // 输入信息非法则不执行
            if (RF_Input.Mode == ReplaceFormat.Modes.Error) return;
            switch (Mode)
            {
                // 查找当前程序
                case MODE_CURRENT:
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
            // 遍历所有网络
            foreach (LadderNetworkModel network in diagram.Children.Where(n => !n.IsMasked))
            {
                // 遍历所有元件
                foreach (LadderUnitModel unit in network.Children)
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
                                items.Add(new FindElement(this, unit));
                            break;
                    }
                }
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
                        current = ((MainTabDiagramItem)(e1.Tab)).LDVModel.Core;
                        if (Mode == MODE_CURRENT) Find();
                    }
                    // 否则隐藏窗口
                    else
                    {
                        Visibility = Visibility.Hidden;
                        current = null;
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
        /// 当在表格内选择元素时触发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件信息</param>
        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 若当前窗口为停靠状态，可能会覆盖到导航的位置，
            // 所以先将窗口固定，保证主界面的完整显示
            if (!ifParent.WNDMain.LAFind.IsFloat
             && !ifParent.WNDMain.LAFind.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Search, ifParent.WNDMain.LAFind.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Search, ifParent.WNDMain.LAFind.AutoHideHeight.ToString());
                ifParent.WNDMain.LAFind.ToggleAutoHide();
            }
            // 未选择元素则不导航
            if (DG_List.SelectedIndex < 0) return;
            // 导航到选择元素对应的位置
            FindElement fele = items[DG_List.SelectedIndex];
            ifParent.Navigate(fele.Unit);
        }

        #endregion
    }
    
    /// <summary>
    /// 查找的元素，继承于替换模块
    /// </summary>
    public class FindElement : ReplaceElement
    {
        private FindWindow parent;

        public FindElement
        (
            FindWindow _parent,
            LadderUnitModel _unit
        ) : base(null, _unit)
        {
            parent = _parent;
        }

        public override void Dispose()
        {
            base.Dispose();
            parent = null;
        }
    }
}
