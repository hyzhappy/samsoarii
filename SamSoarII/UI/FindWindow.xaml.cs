using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Xceed.Wpf.AvalonDock.Global;

/// <summary>
/// Namespace : SamSoarII.AppMain.UI
/// ClassName : FindWindow
/// Version   : 1.0
/// Date      : 2017/6/8
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 根据给定信息来查找符合的元件指令的界面
/// </remarks>

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindWindow : UserControl, INotifyPropertyChanged
    {
        #region Numbers
        /// <summary>
        /// 主交互接口
        /// </summary>
        private InteractionFacade parent;
        /// <summary>
        /// 当前查找到的元素
        /// </summary>
        private ObservableCollection<FindElement> items
            = new ObservableCollection<FindElement>();
        /// <summary>
        /// 当前查找到的元素
        /// </summary>
        public IEnumerable<FindElement> Items
        {
            get
            {
                return this.items;
            }
        }
        /// <summary>
        /// 模式：查找所有程序
        /// </summary>
        public const int MODE_CURRENT = 0x00;
        /// <summary>
        /// 模式：查找当前程序
        /// </summary>
        public const int MODE_ALL = 0x01;
        /// <summary>
        /// 模式
        /// </summary>
        private int mode;
        /// <summary>
        /// 模式
        /// </summary>
        public int Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
                Find();
            }
        }
        /// <summary>
        /// 用户的输入信息，经过整理后得到的格式类
        /// </summary>
        private ReplaceFormat RF_Input { get; set; } 
            = new ReplaceFormat();

        #endregion
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_parent">交互父类</param>
        public FindWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            parent.CurrentTabChanged += OnCurrentTabChanged;
            Mode = MODE_CURRENT;
            TB_Input.Background = Brushes.Red;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            items.Clear();
        }
        /// <summary>
        /// 查找指令
        /// </summary>
        private void Find()
        {
            items.Clear();
            // 输入信息非法则不执行
            if (RF_Input.Type == ReplaceFormat.TYPE_INVALID)
                return;
            switch (Mode)
            {
                // 查找当前程序
                case MODE_CURRENT:
                    ITabItem currenttab = parent.MainTabControl.SelectedItem;
                    if (currenttab is MainTabDiagramItem)
                    {
                        MainTabDiagramItem mtditem = (MainTabDiagramItem)currenttab;
                        LadderDiagramViewModel ldvmodel = mtditem.LDVM_ladder;
                        Find(ldvmodel);
                    }
                    if (currenttab is LadderDiagramViewModel)
                    {
                        Find((LadderDiagramViewModel)currenttab);
                    }
                    break;
                // 查找所有程序
                case MODE_ALL:
                    ProjectModel pmodel = parent.ProjectModel;
                    Find(pmodel.MainRoutine);
                    foreach (LadderDiagramViewModel _ldvmodel in pmodel.SubRoutines)
                    {
                        Find(_ldvmodel);
                    }
                    break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }
        /// <summary>
        /// 在给定程序内查找指令
        /// </summary>
        /// <param name="ldvmodel"></param>
        private void Find(LadderDiagramViewModel ldvmodel)
        {
            // 遍历所有网络
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                // 遍历所有元件
                foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
                {
                    // 忽略连线
                    if (bvmodel is HorizontalLineViewModel
                     || bvmodel is VerticalLineViewModel)
                        continue;
                    // 检查元件
                    BaseModel bmodel = bvmodel.Model;
                    if (RF_Input.Match(bvmodel.ToInstString()))
                    {
                        items.Add(new FindElement(this, bvmodel, ldvmodel, lnvmodel));
                    }
                }
            }
        }

        #region Event Handler

        /// <summary>
        /// 属性更改时触发
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
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
                    // 非法时，将输入框涂红，并禁止操作
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
            if (!parent.MainWindow.LAFind.IsFloat
             && !parent.MainWindow.LAFind.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Search, parent.MainWindow.LAFind.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Search, parent.MainWindow.LAFind.AutoHideHeight.ToString());
                parent.MainWindow.LAFind.ToggleAutoHide();
            }
            // 未选择元素则不导航
            if (DG_List.SelectedIndex < 0) return;
            // 导航到选择元素对应的位置
            FindElement fele = items[DG_List.SelectedIndex];
            BaseViewModel bvmodel = fele.BVModel;
            int x = bvmodel.X;
            int y = bvmodel.Y;
            string diagram = fele.Diagram;
            int network = int.Parse(fele.Network);
            NavigateToNetworkEventArgs _e = new NavigateToNetworkEventArgs(network, diagram, x, y);
            parent.NavigateToNetwork(_e);
        }
        /// <summary>
        /// 当前TAB界面更改时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCurrentTabChanged(object sender, SelectionChangedEventArgs e)
        {
            ITabItem currenttab = parent.MainTabControl.SelectedItem;
            // 当前界面是梯形图程序时进行重查
            if (currenttab is MainTabDiagramItem
             || currenttab is LadderDiagramViewModel)
            {
                Visibility = Visibility.Visible;
                if (mode == MODE_CURRENT) Find();
            }
            // 否则隐藏窗口
            else
            {
                Visibility = Visibility.Hidden;
            }
        }

        #endregion
    }
    /// <summary>
    /// 输入信息的格式，继承于替换模块
    /// </summary>
    public class FindFormat : ReplaceFormat
    {

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
            BaseViewModel _bvmodel,
            LadderDiagramViewModel _ldvmodel,
            LadderNetworkViewModel _lnvmodel
        ) : base(null, _bvmodel, _ldvmodel, _lnvmodel)
        {
            parent = _parent;
        }

        public override void Dispose()
        {
            base.Dispose();
            parent = null;
        }

        protected override void OnElementChanged(object sender, LadderElementChangedArgs e)
        {
            //base.OnElementChanged(sender, e);
            if (e.BVModel_old == BVModel)
            {
                parent.Initialize();
            }
        }
    }
}
