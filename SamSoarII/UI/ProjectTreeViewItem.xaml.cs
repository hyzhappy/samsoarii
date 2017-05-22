using SamSoarII.AppMain.Project;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ProjectTreeViewItem.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeViewItem : TreeViewItem, INotifyPropertyChanged, IComparable<ProjectTreeViewItem>
    {
        public static readonly DependencyProperty IconSourceProperty;
        static ProjectTreeViewItem()
        {
            PropertyMetadata metadata = new PropertyMetadata();
            metadata.PropertyChangedCallback += ProjectTreeViewItem_PropertyChangedEvent;
            IconSourceProperty = DependencyProperty.Register("IconSource",typeof(string),typeof(ProjectTreeViewItem),metadata);
        }
        private static void ProjectTreeViewItem_PropertyChangedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProjectTreeViewItem item = (ProjectTreeViewItem)d;
            item.RaisePropertyChanged(item,"IconSource");
        }
        public bool IsCritical { get; set; }
        private ProjectTreeView ptview;
        public bool IsOrder { get; set; }

        private object relativeobject;
        public object RelativeObject
        {
            get
            {
                return this.relativeobject;
            }
            set
            {
                if (relativeobject is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)relativeobject).PropertyChanged -= OnRelativePropertyChanged;
                }
                this.relativeobject = value;
                if (relativeobject is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)relativeobject).PropertyChanged += OnRelativePropertyChanged;
                }
            }
        }
        public void RaisePropertyChanged(ProjectTreeViewItem item, string propertyName)
        {
            PropertyChanged.Invoke(item,new PropertyChangedEventArgs(propertyName));
        }
        public string IconSource
        {
            get
            {
                return (string)GetValue(IconSourceProperty);
            }
            set
            {
                SetValue(IconSourceProperty,value);
            }
        }
        private string text;
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                RaisePropertyChanged(this,"Text");
            }
        }

        public string SubText
        {
            get { return TBS_Text.Text; }
            set { TBS_Text.Text = value; }
        }
        
        #region Path System

        static private Dictionary<string, ProjectTreeViewItem> ppdict
            = new Dictionary<string, ProjectTreeViewItem>();

        public string Path
        {
            get
            {
                string profix = String.Empty;
                if (Parent is ProjectTreeViewItem)
                {
                    profix = ((ProjectTreeViewItem)Parent).Path + ".";
                }
                return profix + Text;
            }
        }
        
        public static ProjectTreeViewItem GetPTVIFromPath(string path)
        {
            if (ppdict.ContainsKey(path))
            {
                return ppdict[path];
            }
            else
            {
                return null;
            }
        }

        public static void ClearPath()
        {
            ppdict.Clear();
        }

        public void RegisterPath()
        {
            string path = Path;
            if (!ppdict.ContainsKey(path))
            {
                ppdict.Add(path, this);
            }
            else
            {
                ppdict[path] = this;
            }
        }

        public void ReleasePath()
        {
            string path = Path;
            if (ppdict.ContainsKey(path))
            {
                ppdict.Remove(path);
            }
        }

        #endregion

        #region Flags

        public const int TYPE_ROOT = 0x0;
        public const int TYPE_ROUTINEFLODER = 0x1;
        public const int TYPE_NETWORKFLODER = 0x2;
        public const int TYPE_FUNCBLOCKFLODER = 0x3;
        public const int TYPE_MODBUSFLODER = 0x4;
        public const int TYPE_ROUTINE = 0x5;
        public const int TYPE_NETWORK = 0x6;
        public const int TYPE_FUNCBLOCK = 0x7;
        public const int TYPE_FUNC = 0x8;
        public const int TYPE_MODBUS = 0x9;
        
        public const int TYPE_PROGRAM = 0xa;
        public const int TYPE_LADDERS = 0xb;
        public const int TYPE_INSTRUCTION = 0xc;
        public const int TYPE_CONST = 0xd;
        public const int TYPE_ELEMENTLIST = 0xe;
        public const int TYPE_ELEMENTINITIALIZE = 0xf;

        public const int FLAG_CREATEFOLDER = 0x10;
        public const int FLAG_CREATEROUTINE = 0x20;
        public const int FLAG_CREATENETWORK = 0x40;
        public const int FLAG_CREATEFUNCBLOCK = 0x80;
        public const int FLAG_CREATEMODBUS = 0x100;
        public const int FLAG_RENAME = 0x200;
        public const int FLAG_REMOVE = 0x400;
        public const int FLAG_CREATENETWORKAFTER = 0x800;
        public const int FLAG_CREATENETWORKBEFORE = 0x1000;
        public const int FLAG_CONFIG = 0x2000;

        private int flags;
        public int Flags
        {
            get { return this.flags; }
            set
            {
                this.flags = value;
                ContextMenu = new ContextMenu();
                if ((flags & ~0xf) != 0)
                {
                    //this.ContextMenu = new ContextMenu();
                    ProjectMenuItem pmitem = null;
                    int _flags = FLAG_CREATEFOLDER;
                    while (_flags <= FLAG_CONFIG)
                    {
                        if ((flags & _flags) != 0)
                        {
                            pmitem = new ProjectMenuItem(this, _flags);
                            pmitem.Click += OnMenuItemClick;
                            this.ContextMenu.Items.Add(pmitem);
                        }
                        _flags <<= 1;
                    }
                }
                switch (flags & 0xf)
                {
                    case TYPE_ROOT:
                        IconSource = "/Resources/Image/MainStyle/Project.png";
                        if (RelativeObject is ProjectModel)
                        {
                            Text = String.Format("工程 - {0:s}", ((ProjectModel)RelativeObject).ProjectName);
                        }
                        break;
                    case TYPE_ROUTINE:
                        IconSource = "/Resources/Image/MainStyle/Routines.png";
                        if (RelativeObject is LadderDiagramViewModel)
                        {
                            string name = ((LadderDiagramViewModel)RelativeObject).ProgramName;
                            if (name.Equals("Main"))
                            {
                                Text = String.Format("主程序 - {0}",name);
                            }
                            else
                            {
                                Text = name;
                            }
                        }
                        break;
                    case TYPE_ROUTINEFLODER:
                        if (IsExpanded)
                        {
                            IconSource = "/Resources/Image/MainStyle/folderOpen.png";
                        }
                        else
                        {
                            IconSource = "/Resources/Image/MainStyle/folderClose.png";
                        }
                        Text = RelativeObject.ToString();
                        break;
                    case TYPE_NETWORK:
                        IconSource = "/Resources/Image/MainStyle/Network.png";
                        Image.Height = 14;
                        if (RelativeObject is LadderNetworkViewModel)
                        {
                            LadderNetworkViewModel lnvmodel = (LadderNetworkViewModel)RelativeObject;
                            if (lnvmodel.NetworkBrief == null 
                             || lnvmodel.NetworkBrief.Equals(String.Empty))
                            {
                                Text = String.Format("网络 {0:d}", lnvmodel.NetworkNumber);
                            }
                            else
                            {
                                Text = String.Format("{0:d}-{1:s}", lnvmodel.NetworkNumber, lnvmodel.NetworkBrief);
                            }
                        }
                        break;
                    case TYPE_NETWORKFLODER:
                        if (IsExpanded)
                        {
                            IconSource = "/Resources/Image/MainStyle/folderOpen.png";
                        }
                        else
                        {
                            IconSource = "/Resources/Image/MainStyle/folderClose.png";
                        }
                        Text = RelativeObject.ToString();
                        break;
                    case TYPE_FUNCBLOCK:
                        IconSource = "/Resources/Image/MainStyle/FUNC.png";
                        if (RelativeObject is FuncBlockViewModel)
                        {
                            Text = ((FuncBlockViewModel)RelativeObject).ProgramName;
                        }
                        break;
                    case TYPE_FUNCBLOCKFLODER:
                        if (IsExpanded)
                        {
                            IconSource = "/Resources/Image/MainStyle/folderOpen.png";
                        }
                        else
                        {
                            IconSource = "/Resources/Image/MainStyle/folderClose.png";
                        }
                        Text = RelativeObject.ToString();
                        break;
                    case TYPE_FUNC:
                        IconSource = "/Resources/Image/TreeViewIcon/Func.png";
                        if (RelativeObject is FuncModel)
                        {
                            Text = ((FuncModel)RelativeObject).Name;
                        }
                        break;
                    case TYPE_MODBUS:
                        IconSource = "/Resources/Image/MainStyle/ModBusTable.png";
                        if (RelativeObject is ModbusTableModel)
                        {
                            Text = ((ModbusTableModel)RelativeObject).Name;
                        }
                        break;
                    case TYPE_MODBUSFLODER:
                        if (IsExpanded)
                        {
                            IconSource = "/Resources/Image/MainStyle/folderOpen.png";
                        }
                        else
                        {
                            IconSource = "/Resources/Image/MainStyle/folderClose.png";
                        }
                        Text = "Modbus表格";
                        break;
                    case TYPE_ELEMENTLIST:
                        IconSource = "/Resources/Image/ElementTable.png";
                        Text = "软元件管理器";
                        break;
                    case TYPE_ELEMENTINITIALIZE:
                        IconSource = "/Resources/Image/ElementInitialize.png";
                        Text = "软元件初始化";
                        break;
                    case TYPE_PROGRAM:
                        IconSource = "/Resources/Image/MainStyle/Program.png";
                        Text = "程序";
                        break;
                    case TYPE_LADDERS:
                        IconSource = "/Resources/Image/MainStyle/Instruction.png";
                        Image.Height = 20;
                        Text = RelativeObject.ToString();
                        break;
                    case TYPE_CONST:
                        IconSource = String.Empty;
                        Text = RelativeObject.ToString();
                        break;
                    case TYPE_INSTRUCTION:
                        if (RelativeObject is BaseViewModel)
                        {
                            BaseViewModel bvmodel = (BaseViewModel)RelativeObject;
                            if (bvmodel is InputBaseViewModel)
                            {
                                IconSource = "/Resources/Image/TreeViewIcon/Instruction_Input1.png";
                                switch (bvmodel.InstructionName)
                                {
                                    case "LD":   Text = "-|　|-"; SubText = "普通常开"; break;
                                    case "LDIM": Text = "-|｜|-"; SubText = "立即常开"; break;
                                    case "LDI": Text = "-|／|-"; SubText = "普通常闭"; break;
                                    case "LDIIM": Text = "-|/||-"; SubText = "立即常闭"; break;
                                    case "LDP": Text = "-|↑|-"; SubText = "上升脉冲"; break;
                                    case "LDF": Text = "-|↓|-"; SubText = "下降脉冲"; break;
                                    case "LDWEQ": Text = "-Ｗ＝-"; SubText = "单字相等"; break;
                                    case "LDWNE": Text = "-Ｗ≠-"; SubText = "单字不等"; break;
                                    case "LDWLE": Text = "-Ｗ≤-"; SubText = "单字小等"; break;
                                    case "LDWGE": Text = "-Ｗ≥-"; SubText = "单字大等"; break;
                                    case "LDWL": Text = "-Ｗ＜-"; SubText = "单字小于"; break;
                                    case "LDWG": Text = "-Ｗ＞-"; SubText = "单字大于"; break;
                                    case "LDDEQ": Text = "-Ｄ＝-"; SubText = "双字相等"; break;
                                    case "LDDNE": Text = "-Ｄ≠-"; SubText = "双字不等"; break;
                                    case "LDDLE": Text = "-Ｄ≤-"; SubText = "双字小等"; break;
                                    case "LDDGE": Text = "-Ｄ≥-"; SubText = "双字大等"; break;
                                    case "LDDL": Text = "-Ｄ＜-"; SubText = "双字小于"; break;
                                    case "LDDG": Text = "-Ｄ＞-"; SubText = "双字大于"; break;
                                    case "LDFEQ": Text = "-Ｆ＝-"; SubText = "浮点相等"; break;
                                    case "LDFNE": Text = "-Ｆ≠-"; SubText = "浮点不等"; break;
                                    case "LDFLE": Text = "-Ｆ≤-"; SubText = "浮点小等"; break;
                                    case "LDFGE": Text = "-Ｆ≥-"; SubText = "浮点大等"; break;
                                    case "LDFL": Text = "-Ｆ＜-"; SubText = "浮点小于"; break;
                                    case "LDFG": Text = "-Ｆ＞-"; SubText = "浮点大于"; break;
                                    default: Text = bvmodel.InstructionName; break;
                                }
                            }
                            else if (bvmodel is SpecialBaseViewModel)
                            {
                                IconSource = "/Resources/Image/TreeViewIcon/Instruction_Input2.png";
                                switch (bvmodel.InstructionName)
                                {
                                    case "MEP": Text = "- ↑ -"; SubText = "结果上升"; break;
                                    case "MEF": Text = "- ↓ -"; SubText = "结果下降"; break;
                                    case "INV": Text = "- ／ -"; SubText = "结果取反"; break;
                                    default: Text = bvmodel.InstructionName; break;
                                }
                            }
                            else if (bvmodel is OutputBaseViewModel)
                            {
                                IconSource = "/Resources/Image/TreeViewIcon/Instruction_Output1.png";
                                switch (bvmodel.InstructionName)
                                {
                                    case "OUT": Text = "-(　) "; SubText = "输出线圈"; break;
                                    case "OUTIM": Text = "-(｜) "; SubText = "立即输出"; break;
                                    case "SET": Text = "-(Ｓ) "; SubText = "置位线圈"; break;
                                    case "SETIM": Text = "-(SI) "; SubText = "立即置位"; break;
                                    case "RST": Text = "-(Ｒ) "; SubText = "复位线圈"; break;
                                    case "RSTIM": Text = "-(RI) "; SubText = "立即复位"; break;
                                    default: Text = bvmodel.InstructionName; break;
                                }
                            }
                            else if (bvmodel is OutputRectBaseViewModel)
                            {
                                IconSource = "/Resources/Image/TreeViewIcon/Instruction_Output2.png";
                                Text = String.Format("-{0:s}", bvmodel.InstructionName);
                                switch (bvmodel.InstructionName)
                                {
                                    case "ALT": SubText = "交替输出"; break;
                                    case "ALTP": SubText = "脉冲交替"; break;
                                    case "WTOD": SubText = "单字转双字"; break;
                                    case "DTOW": SubText = "双字转单字"; break;
                                    case "DTOF": SubText = "双子转浮点"; break;
                                    case "BIN": SubText = "BCD码转整数"; break;
                                    case "BCD": SubText = "整数转BCD码"; break;
                                    case "ROUND": SubText = "截位取整"; break;
                                    case "TRUNC": SubText = "四舍五入"; break;
                                    case "INVW": SubText = "反转单字"; break;
                                    case "INVD": SubText = "反转双字"; break;
                                    case "ANDW": SubText = "单字相与"; break;
                                    case "ANDD": SubText = "双字相与"; break;
                                    case "ORW": SubText = "单字相或"; break;
                                    case "ORD": SubText = "双字相或"; break;
                                    case "XORW": SubText = "单字异或"; break;
                                    case "XORD": SubText = "双字异或"; break;
                                    case "MOV": SubText = "传送单字"; break;
                                    case "MOVD": SubText = "传送双字"; break;
                                    case "MOVF": SubText = "传送浮点"; break;
                                    case "MVBLK": SubText = "成块移动单字"; break;
                                    case "MVDBLK": SubText = "成块移动双字"; break;
                                    case "ADD": SubText = "单字相加"; break;
                                    case "ADDD": SubText = "双字相加"; break;
                                    case "ADDF": SubText = "浮点相加"; break;
                                    case "SUB": SubText = "单字相减"; break;
                                    case "SUBD": SubText = "双字相减"; break;
                                    case "SUBF": SubText = "浮点相减"; break;
                                    case "MUL": SubText = "相乘"; break;
                                    case "MULW": SubText = "单子相乘"; break;
                                    case "MULD": SubText = "双字相乘"; break;
                                    case "MULF": SubText = "浮点相乘"; break;
                                    case "DIV": SubText = "相除"; break;
                                    case "DIVW": SubText = "单子相除"; break;
                                    case "DIVD": SubText = "双字相除"; break;
                                    case "DIVF": SubText = "浮点相除"; break;
                                    case "INC": SubText = "单字加一"; break;
                                    case "INCD": SubText = "双字加一"; break;
                                    case "DEC": SubText = "单字减一"; break;
                                    case "DECD": SubText = "双字减一"; break;
                                    case "SIN": SubText = "正弦运算"; break;
                                    case "COS": SubText = "余弦运算"; break;
                                    case "TAN": SubText = "正切运算"; break;
                                    case "SQRT": SubText = "求平方根"; break;
                                    case "EXP": SubText = "自然指数"; break;
                                    case "LN": SubText = "自然对数"; break;
                                    case "TON": SubText = "接通延时定时器"; break;
                                    case "TONR": SubText = "接通延时保护定时器"; break;
                                    case "TOF": SubText = "断开延时定时器"; break;
                                    case "CTU": SubText = "向上计数器"; break;
                                    case "CTD": SubText = "向下计数器"; break;
                                    case "CTUD": SubText = "向上向下计数器"; break;
                                    case "FOR": SubText = "循环开始"; break;
                                    case "NEXT": SubText = "循环结束"; break;
                                    case "JMP": SubText = "程序跳转"; break;
                                    case "LBL": SubText = "跳转标签"; break;
                                    case "CALL": SubText = "子程序调用"; break;
                                    case "CALLM": SubText = "函数块调用"; break;
                                    case "SHL": SubText = "单字左移"; break;
                                    case "SHR": SubText = "单字右移"; break;
                                    case "SHLD": SubText = "双字左移"; break;
                                    case "SHRD": SubText = "双字右移"; break;
                                    case "SHLB": SubText = "位左移"; break;
                                    case "SHRB": SubText = "位右移"; break;
                                    case "ROL": SubText = "单字循环左移"; break;
                                    case "ROR": SubText = "单字循环右移"; break;
                                    case "ROLD": SubText = "双字循环左移"; break;
                                    case "RORD": SubText = "双字循环右移"; break;
                                    case "ATCH": SubText = "中断绑定"; break;
                                    case "DTCH": SubText = "中断释放"; break;
                                    case "EI": SubText = "中断允许"; break;
                                    case "DI": SubText = "中断禁止"; break;
                                    case "TRD": SubText = "实时读取"; break;
                                    case "TWR": SubText = "实时写入"; break;
                                    case "MBUS": SubText = "Modbus通信"; break;
                                    case "SEND": SubText = "自由发送"; break;
                                    case "REV": SubText = "自由接收"; break;
                                    case "PLSF": SubText = "单字变频脉冲"; break;
                                    case "DPLSF": SubText = "双字变频脉冲"; break;
                                    case "PWM": SubText = "单字脉宽调制"; break;
                                    case "DPWM": SubText = "双字脉宽调制"; break;
                                    case "PLSY": SubText = "单字单段无加减速脉冲"; break;
                                    case "DPLSY": SubText = "双字单段无加减速脉冲"; break;
                                    case "PLSR": SubText = "单字单向多段脉冲"; break;
                                    case "DPLSR": SubText = "双字单向多段脉冲"; break;
                                    case "PLSRD": SubText = "单字双向多段脉冲"; break;
                                    case "DPLSRD": SubText = "双字双向多段脉冲"; break;
                                    case "PLSNEXT": SubText = "脉冲跳转"; break;
                                    case "PLSSTOP": SubText = "脉冲停止"; break;
                                    case "ZRN": SubText = "单字回归原点"; break;
                                    case "DZRN": SubText = "双字回归原点"; break;
                                    case "PTO": SubText = "双字相对位置多段脉冲控制"; break;
                                    case "DRVI": SubText = "单字相对位置单段脉冲输出"; break;
                                    case "DDRVI": SubText = "双字相对位置单段脉冲输出"; break;
                                    case "HCNT": SubText = "高速计数"; break;
                                    case "LOG": SubText = "10的对数"; break;
                                    case "POW": SubText = "幂级数"; break;
                                    case "FACT": SubText = "整数阶乘"; break;
                                    case "CMP": SubText = "单字比较"; break;
                                    case "CMPD": SubText = "双字比较"; break;
                                    case "CMPF": SubText = "浮点比较"; break;
                                    case "ZCP": SubText = "单字区间比较"; break;
                                    case "ZCPD": SubText = "双字区间比较"; break;
                                    case "ZCPF": SubText = "浮点区间比较"; break;
                                    case "NEG": SubText = "单字求补"; break;
                                    case "NEGD": SubText = "双字求补"; break;
                                    case "XCH": SubText = "单字交换"; break;
                                    case "XCHD": SubText = "双字交换"; break;
                                    case "XCHF": SubText = "浮点交换"; break;
                                    case "CML": SubText = "单字取反"; break;
                                    case "CMLD": SubText = "双字取反"; break;
                                    case "SMOV": SubText = "移位传送"; break;
                                    case "FMOV": SubText = "单字多点传送"; break;
                                    case "FMOVD": SubText = "双字多点传送"; break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        #endregion
        
        public int CompareTo(ProjectTreeViewItem that)
        {
            int rank1 = 0;
            switch (this.Flags & 0xf)
            {
                case ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER:
                case ProjectTreeViewItem.TYPE_ROUTINEFLODER:
                case ProjectTreeViewItem.TYPE_NETWORKFLODER:
                case ProjectTreeViewItem.TYPE_MODBUSFLODER:
                    rank1 = 0;
                    break;
                default:
                    rank1 = 1;
                    break;
            }
            int rank2 = 0;
            switch (that.Flags & 0xf)
            {
                case ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER:
                case ProjectTreeViewItem.TYPE_ROUTINEFLODER:
                case ProjectTreeViewItem.TYPE_NETWORKFLODER:
                case ProjectTreeViewItem.TYPE_MODBUSFLODER:
                    rank2 = 0;
                    break;
                default:
                    rank2 = 1;
                    break;
            }
            if (rank1 < rank2) return -1;
            if (rank1 > rank2) return 1;
            return this.Text.CompareTo(that.Text);
        }

        public ProjectTreeViewItem(ProjectTreeView _ptview)
        {
            InitializeComponent();
            DataContext = this;
            ptview = _ptview;
            RelativeObject = String.Empty;
            Flags = ProjectTreeViewItem.TYPE_CONST;
        }

        #region Rename

        public bool IsRenaming { get; private set; } = false;

        static public bool HasRenaming { get; set; } = false;

        private ContextMenu _contextmenu;

        public void Rename(string errormsg = null)
        {
            if (errormsg == null && HasRenaming)
            {
                MessageBox.Show("已存在正在重命名的项目！");
                return;
            }
            _contextmenu = this.ContextMenu;
            this.ContextMenu = null;
            IsRenaming = true;
            HasRenaming = true;
            IsSelected = true;
            TBL_Text.Visibility = Visibility.Hidden;
            TBO_Text.Visibility = Visibility.Visible;
            TBO_Text.IsEnabled = true;
            if (errormsg != null)
            {
                TB_ErrorMsg.Visibility = Visibility.Visible;
                TB_ErrorMsg.Text = errormsg;
            }
            else
            {
                TB_ErrorMsg.Visibility = Visibility.Collapsed;
            }
            TBO_Text.Focus();
            TBO_Text.SelectAll();
        }

        public void RenameClose()
        {
            this.ContextMenu = _contextmenu;
            IsRenaming = false;
            HasRenaming = false;
            TBL_Text.Visibility = Visibility.Visible;
            TBO_Text.Visibility = Visibility.Hidden;
            TB_ErrorMsg.Visibility = Visibility.Collapsed;
            TBO_Text.IsEnabled = false;
        }

        #endregion

        #region Event Handler
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private void OnRelativePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "NetworkMessage":
                    if (RelativeObject is LadderNetworkViewModel)
                    {
                        LadderNetworkViewModel lnvmodel = (LadderNetworkViewModel)RelativeObject;
                        if (lnvmodel.NetworkBrief == null
                         || lnvmodel.NetworkBrief.Equals(String.Empty))
                        {
                            Text = String.Format("网络 {0:d}", lnvmodel.NetworkNumber);
                        }
                        else
                        {
                            Text = String.Format("{0:d}-{1:s}", lnvmodel.NetworkNumber, lnvmodel.NetworkBrief);
                        }
                    }
                    break;
            }
        }

        public event RoutedEventHandler MenuItemClick = delegate { };

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
            if (ptview.Project.LadderMode != LadderMode.Edit
             || (Flags & ~0xf) == 0
             || HasRenaming)
            {
                ContextMenu.Visibility = Visibility.Hidden;
            }
            else
            {
                ContextMenu.Visibility = Visibility.Visible;
            }
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItemClick(sender, e);
        }

        public event RoutedEventHandler Renamed = delegate { };

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TB_ErrorMsg.Visibility = Visibility.Collapsed;
            if (e.Key == Key.Enter)
            {
                Text = TBO_Text.Text;
                Renamed(this, new RoutedEventArgs());
            }
        }
        
        private void TBO_Text_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Text = TBO_Text.Text;
            Renamed(this, new RoutedEventArgs());
        }
        
        #endregion

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            var item = sender as ProjectTreeViewItem;
            if (item.Items.Count > 0)
            {
                switch (item.Flags & 0xf)
                {
                    case TYPE_FUNCBLOCKFLODER:
                    case TYPE_MODBUSFLODER:
                    case TYPE_NETWORKFLODER:
                    case TYPE_ROUTINEFLODER:
                        IconSource = "/Resources/Image/MainStyle/folderOpen.png";
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        private void OnCollapsed(object sender, RoutedEventArgs e)
        {
            var item = sender as ProjectTreeViewItem;
            if (item.Items.Count > 0)
            {
                switch (item.Flags & 0xf)
                {
                    case TYPE_FUNCBLOCKFLODER:
                    case TYPE_MODBUSFLODER:
                    case TYPE_NETWORKFLODER:
                    case TYPE_ROUTINEFLODER:
                        IconSource = "/Resources/Image/MainStyle/folderClose.png";
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }

    public class ProjectMenuItem : MenuItem
    {
        private ProjectTreeViewItem parent;

        public ProjectTreeViewItem PTVItem
        {
            get { return this.parent; }
        }

        public int ParentFlags
        {
            get { return parent.Flags; }
        }

        public object RelativeObject
        {
            get { return parent.RelativeObject; }
        }

        public int Flags { get; private set; }

        public ProjectMenuItem(ProjectTreeViewItem _parent, int _flags)
        {
            parent = _parent;
            Flags = _flags;
            string profix = String.Empty;
            switch (parent.Flags & 0xf)
            {
                case ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER:
                case ProjectTreeViewItem.TYPE_MODBUSFLODER:
                case ProjectTreeViewItem.TYPE_NETWORKFLODER:
                case ProjectTreeViewItem.TYPE_ROUTINEFLODER:
                    profix = "文件夹"; break;
                case ProjectTreeViewItem.TYPE_ROUTINE:
                    profix = "子程序"; break;
                case ProjectTreeViewItem.TYPE_NETWORK:
                    profix = "网络"; break;
                case ProjectTreeViewItem.TYPE_FUNCBLOCK:
                    profix = "函数块"; break;
                case ProjectTreeViewItem.TYPE_MODBUS:
                    profix = "表格"; break;
            }
            switch (Flags)
            {
                case ProjectTreeViewItem.FLAG_CREATEFOLDER:
                    Header = "新建文件夹"; break;
                case ProjectTreeViewItem.FLAG_CREATEROUTINE:
                    Header = "新建子程序"; break;
                case ProjectTreeViewItem.FLAG_CREATENETWORK:
                    Header = "新建网络"; break;
                case ProjectTreeViewItem.FLAG_CREATEFUNCBLOCK:
                    Header = "新建函数块"; break;
                case ProjectTreeViewItem.FLAG_CREATEMODBUS:
                    Header = "新建MODBUS表格"; break;
                case ProjectTreeViewItem.FLAG_RENAME:
                    Header = profix + "重命名"; break;
                case ProjectTreeViewItem.FLAG_REMOVE:
                    Header = profix + "删除"; break;
                case ProjectTreeViewItem.FLAG_CREATENETWORKBEFORE:
                    Header = "向前插入"; break;
                case ProjectTreeViewItem.FLAG_CREATENETWORKAFTER:
                    Header = "向后插入"; break;
                case ProjectTreeViewItem.FLAG_CONFIG:
                    Header = profix + "属性"; break;
            }
        }
    }
}
