using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
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
using System.Collections.Specialized;
using SamSoarII.Shell.Models;

namespace SamSoarII.Shell.Windows
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
            IconSourceProperty = DependencyProperty.Register("IconSource", typeof(string), typeof(ProjectTreeViewItem), metadata);
        }
        private static void ProjectTreeViewItem_PropertyChangedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProjectTreeViewItem item = (ProjectTreeViewItem)d;
            item.RaisePropertyChanged(item, "IconSource");
        }
        public void RaisePropertyChanged(ProjectTreeViewItem item, string propertyName)
        {
            PropertyChanged.Invoke(item, new PropertyChangedEventArgs(propertyName));
        }

        private ProjectTreeView ptview;
        public bool IsCritical { get; set; }
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
                if (relativeobject is IModel)
                {
                    ((IModel)relativeobject).PTVItem = null;
                    if (relativeobject is ProjectModel)
                    {
                        ((ProjectModel)relativeobject).DiagramChanged -= OnDiagramChanged;
                        ((ProjectModel)relativeobject).FuncBlockChanged -= OnFuncBlockChanged;
                    }
                    if (relativeobject is LadderDiagramModel)
                    {
                        ((LadderDiagramModel)relativeobject).ChildrenChanged -= OnNetworkChanged;
                    }
                    if (relativeobject is ModbusModel)
                    {
                        ((ModbusModel)relativeobject).ChildrenChanged -= OnModbusChanged;
                    }
                }
                this.relativeobject = value;
                if (relativeobject is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)relativeobject).PropertyChanged += OnRelativePropertyChanged;
                }
                if (relativeobject is IModel)
                {
                    ((IModel)relativeobject).PTVItem = this;
                    if (relativeobject is ProjectModel)
                    {
                        ((ProjectModel)relativeobject).DiagramChanged += OnDiagramChanged;
                        ((ProjectModel)relativeobject).FuncBlockChanged += OnFuncBlockChanged;
                    }
                    if (relativeobject is LadderDiagramModel)
                    {
                        ((LadderDiagramModel)relativeobject).ChildrenChanged += OnNetworkChanged;
                    }
                    if (relativeobject is ModbusModel)
                    {
                        ((ModbusModel)relativeobject).ChildrenChanged += OnModbusChanged;
                    }
                }
            }
        }
        
        public string IconSource
        {
            get
            {
                return (string)GetValue(IconSourceProperty);
            }
            set
            {
                SetValue(IconSourceProperty, value);
            }
        }
        private string text;
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                RaisePropertyChanged(this, "Text");
            }
        }

        public string SubText
        {
            get { return TBS_Text.Text; }
            set { TBS_Text.Text = value; }
        }
        
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
        

        public void InitializeContextMenu()
        {
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
        }

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
                switch (flags & 0xf)
                {
                    case TYPE_ROOT:
                        IconSource = "/Resources/Image/MainStyle/Project.png";
                        if (RelativeObject is ProjectModel)
                        {
                            Text = String.Format("{0} - {1:s}", Properties.Resources.Project, ((ProjectModel)RelativeObject).ProjName);
                        }
                        break;
                    case TYPE_ROUTINE:
                        IconSource = "/Resources/Image/MainStyle/Routines.png";
                        if (RelativeObject is LadderDiagramModel)
                        {
                            string name = ((LadderDiagramModel)RelativeObject).Name;
                            if (name.Equals("Main"))
                            {
                                Text = String.Format("{0} - {1}", Properties.Resources.MainRoutine, name);
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
                        if (RelativeObject is LadderNetworkModel)
                        {
                            LadderNetworkModel lnvmodel = (LadderNetworkModel)RelativeObject;
                            if (lnvmodel.Brief == null
                             || lnvmodel.Brief.Length == 0)
                            {
                                Text = String.Format("{0} {1:d}", Properties.Resources.Network, lnvmodel.ID);
                            }
                            else
                            {
                                Text = String.Format("{0:d}-{1:s}", lnvmodel.ID, lnvmodel.Brief);
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
                        if (RelativeObject is FuncBlockModel)
                        {
                            Text = ((FuncBlockModel)RelativeObject).Name;
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
                            FuncModel fmodel = (FuncModel)RelativeObject;
                            Text = fmodel.Name;
                            if (fmodel.Comment != null)
                            {
                                SubText = fmodel.Comment;
                            }
                        }
                        break;
                    case TYPE_MODBUS:
                        IconSource = "/Resources/Image/MainStyle/ModBusTable.png";
                        if (RelativeObject is ModbusModel)
                        {
                            Text = ((ModbusModel)RelativeObject).Name;
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
                        Text = Properties.Resources.Modbus_Table;
                        break;
                    case TYPE_ELEMENTLIST:
                        IconSource = "/Resources/Image/ElementTable.png";
                        Text = Properties.Resources.MainWindow_Soft_Element_Manager;
                        break;
                    case TYPE_ELEMENTINITIALIZE:
                        IconSource = "/Resources/Image/ElementInitialize.png";
                        Text = Properties.Resources.MainWindow_Soft_Element_Init;
                        break;
                    case TYPE_PROGRAM:
                        IconSource = "/Resources/Image/MainStyle/Program.png";
                        Text = Properties.Resources.Routine;
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
                        if (RelativeObject is LadderUnitModel.Types)
                        {
                            LadderUnitModel.Types type = (LadderUnitModel.Types)RelativeObject;
                            switch (LadderUnitModel.ShapeOfTypes[(int)type])
                            {
                                case LadderUnitModel.Shapes.Input:
                                    IconSource = "/Resources/Image/TreeViewIcon/Instruction_Input1.png";
                                    break;
                                case LadderUnitModel.Shapes.Special:
                                    IconSource = "/Resources/Image/TreeViewIcon/Instruction_Input2.png";
                                    break;
                                case LadderUnitModel.Shapes.Output:
                                    IconSource = "/Resources/Image/TreeViewIcon/Instruction_Output1.png";
                                    break;
                                case LadderUnitModel.Shapes.OutputRect:
                                    IconSource = "/Resources/Image/TreeViewIcon/Instruction_Output2.png";
                                    break;
                            }
                            Text = LadderUnitModel.NameOfTypes[(int)type];
                            switch (Text)
                            {
                                case "LD": Text = "-|　|-"; SubText = Properties.Resources.MainWindow_Normally_Open_Contact; break;
                                case "LDIM": Text = "-|｜|-"; SubText = Properties.Resources.MainWindow_Immediately_Open; break;
                                case "LDI": Text = "-|／|-"; SubText = Properties.Resources.MainWindow_Normally_Closed; break;
                                case "LDIIM": Text = "-|/||-"; SubText = Properties.Resources.MainWindow_Immediately_Close; break;
                                case "LDP": Text = "-|↑|-"; SubText = Properties.Resources.MainWindow_Rising_Edge_Pulse; break;
                                case "LDF": Text = "-|↓|-"; SubText = Properties.Resources.MainWindow_Falling_Edge_Pulse; break;
                                case "LDWEQ": Text = "-Ｗ＝-"; SubText = Properties.Resources.Word_Equal; break;
                                case "LDWNE": Text = "-Ｗ≠-"; SubText = Properties.Resources.Word_Not_Equal; break;
                                case "LDWLE": Text = "-Ｗ≤-"; SubText = Properties.Resources.Word_Not_More; break;
                                case "LDWGE": Text = "-Ｗ≥-"; SubText = Properties.Resources.Word_Not_Less; break;
                                case "LDWL": Text = "-Ｗ＜-"; SubText = Properties.Resources.Word_Less; break;
                                case "LDWG": Text = "-Ｗ＞-"; SubText = Properties.Resources.Word_More; break;
                                case "LDDEQ": Text = "-Ｄ＝-"; SubText = Properties.Resources.DWord_Equal; break;
                                case "LDDNE": Text = "-Ｄ≠-"; SubText = Properties.Resources.DWord_Not_Equal; break;
                                case "LDDLE": Text = "-Ｄ≤-"; SubText = Properties.Resources.DWord_Not_More; break;
                                case "LDDGE": Text = "-Ｄ≥-"; SubText = Properties.Resources.DWord_Not_Less; break;
                                case "LDDL": Text = "-Ｄ＜-"; SubText = Properties.Resources.DWord_Less; break;
                                case "LDDG": Text = "-Ｄ＞-"; SubText = Properties.Resources.DWord_More; break;
                                case "LDFEQ": Text = "-Ｆ＝-"; SubText = Properties.Resources.Float_Equal; break;
                                case "LDFNE": Text = "-Ｆ≠-"; SubText = Properties.Resources.Float_Not_Equal; break;
                                case "LDFLE": Text = "-Ｆ≤-"; SubText = Properties.Resources.Float_Not_More_Than; break;
                                case "LDFGE": Text = "-Ｆ≥-"; SubText = Properties.Resources.Float_Not_Less_Than; break;
                                case "LDFL": Text = "-Ｆ＜-"; SubText = Properties.Resources.Float_Less_Than; break;
                                case "LDFG": Text = "-Ｆ＞-"; SubText = Properties.Resources.Float_More_Than; break;
                                case "OUT": Text = "-(　) "; SubText = Properties.Resources.MainWindow_Output_Coil; break;
                                case "OUTIM": Text = "-(｜) "; SubText = Properties.Resources.MainWindow_Immediately_Output_Coil; break;
                                case "SET": Text = "-(Ｓ) "; SubText = Properties.Resources.Set_Coil; break;
                                case "SETIM": Text = "-(SI) "; SubText = Properties.Resources.Set_Immediately; break;
                                case "RST": Text = "-(Ｒ) "; SubText = Properties.Resources.Reset_Coil; break;
                                case "RSTIM": Text = "-(RI) "; SubText = Properties.Resources.Reset_Immediately; break;
                                case "MEP": Text = "- ↑ -"; SubText = Properties.Resources.MainWindow_Rising_Edge_Of_Result; break;
                                case "MEF": Text = "- ↓ -"; SubText = Properties.Resources.MainWindow_Falling_Edge_Of_Result; break;
                                case "INV": Text = "- ／ -"; SubText = Properties.Resources.MainWindow_Reversed_Result; break;
                                case "ALT": SubText = Properties.Resources.Alternating_Output; break;
                                case "ALTP": SubText = Properties.Resources.Pulse_Alternation; break;
                                case "WTOD": SubText = Properties.Resources.Word_To_DWord; break;
                                case "DTOW": SubText = Properties.Resources.DWord_To_Word; break;
                                case "DTOF": SubText = Properties.Resources.DWord_To_Float; break;
                                case "BIN": SubText = Properties.Resources.BCD_Code_To_Integer; break;
                                case "BCD": SubText = Properties.Resources.Integer_To_BCD_Code; break;
                                case "ROUND": SubText = Properties.Resources.Round; break;
                                case "TRUNC": SubText = Properties.Resources.Trunc; break;
                                case "INVW": SubText = Properties.Resources.Word_Reverse; break;
                                case "INVD": SubText = Properties.Resources.DWord_Reverse; break;
                                case "ANDW": SubText = Properties.Resources.Word_And; break;
                                case "ANDD": SubText = Properties.Resources.DWord_And; break;
                                case "ORW": SubText = Properties.Resources.Word_Or; break;
                                case "ORD": SubText = Properties.Resources.DWord_Or; break;
                                case "XORW": SubText = Properties.Resources.Word_XOR; break;
                                case "XORD": SubText = Properties.Resources.DWord_XOR; break;
                                case "MOV": SubText = Properties.Resources.Move_Word; break;
                                case "MOVD": SubText = Properties.Resources.Move_DWord; break;
                                case "MOVF": SubText = Properties.Resources.Move_Float; break;
                                case "MVBLK": SubText = Properties.Resources.Move_Blocks_Word; break;
                                case "MVDBLK": SubText = Properties.Resources.Move_Blocks_DWord; break;
                                case "ADD": SubText = Properties.Resources.Word_Add; break;
                                case "ADDD": SubText = Properties.Resources.DWord_Add; break;
                                case "ADDF": SubText = Properties.Resources.Float_Add; break;
                                case "SUB": SubText = Properties.Resources.Word_Minus; break;
                                case "SUBD": SubText = Properties.Resources.DWord_Minus; break;
                                case "SUBF": SubText = Properties.Resources.Float_Minus; break;
                                case "MUL": SubText = Properties.Resources.Multiply; break;
                                case "MULW": SubText = Properties.Resources.Word_Multiply; break;
                                case "MULD": SubText = Properties.Resources.DWord_Multiply; break;
                                case "MULF": SubText = Properties.Resources.Float_Multiply; break;
                                case "DIV": SubText = Properties.Resources.Divide; break;
                                case "DIVW": SubText = Properties.Resources.Word_Divide; break;
                                case "DIVD": SubText = Properties.Resources.DWord_Divide; break;
                                case "DIVF": SubText = Properties.Resources.Float_Divide; break;
                                case "INC": SubText = Properties.Resources.Word_Add_One; break;
                                case "INCD": SubText = Properties.Resources.DWord_Add_One; break;
                                case "DEC": SubText = Properties.Resources.Word_Minus_One; break;
                                case "DECD": SubText = Properties.Resources.DWord_Minus_One; break;
                                case "SIN": SubText = Properties.Resources.Sin_Operation; break;
                                case "COS": SubText = Properties.Resources.Cos_Operation; break;
                                case "TAN": SubText = Properties.Resources.Tan_Operation; break;
                                case "SQRT": SubText = Properties.Resources.Sqrt_Operation; break;
                                case "EXP": SubText = Properties.Resources.EXP_Operation; break;
                                case "LN": SubText = Properties.Resources.LN_Operation; break;
                                case "TON": SubText = Properties.Resources.TON_Inst; break;
                                case "TONR": SubText = Properties.Resources.TONR_Inst; break;
                                case "TOF": SubText = Properties.Resources.TOF_Inst; break;
                                case "CTU": SubText = Properties.Resources.CTU_Inst; break;
                                case "CTD": SubText = Properties.Resources.CTD_Inst; break;
                                case "CTUD": SubText = Properties.Resources.CTUD_Inst; break;
                                case "FOR": SubText = Properties.Resources.FOR_Inst; break;
                                case "NEXT": SubText = Properties.Resources.NEXT_Inst; break;
                                case "JMP": SubText = Properties.Resources.JMP_Inst; break;
                                case "LBL": SubText = Properties.Resources.LBL_Inst; break;
                                case "CALL": SubText = Properties.Resources.CALL_Inst; break;
                                case "CALLM": SubText = Properties.Resources.CALLM_Inst; break;
                                case "SHL": SubText = Properties.Resources.SHL_Inst; break;
                                case "SHR": SubText = Properties.Resources.SHR_Inst; break;
                                case "SHLD": SubText = Properties.Resources.SHLD_Inst; break;
                                case "SHRD": SubText = Properties.Resources.SHRD_Inst; break;
                                case "SHLB": SubText = Properties.Resources.SHLB_Inst; break;
                                case "SHRB": SubText = Properties.Resources.SHRB_Inst; break;
                                case "ROL": SubText = Properties.Resources.ROL_Inst; break;
                                case "ROR": SubText = Properties.Resources.ROR_Inst; break;
                                case "ROLD": SubText = Properties.Resources.ROLD_Inst; break;
                                case "RORD": SubText = Properties.Resources.RORD_Inst; break;
                                case "ATCH": SubText = Properties.Resources.ATCH_Inst; break;
                                case "DTCH": SubText = Properties.Resources.DTCH_Inst; break;
                                case "EI": SubText = Properties.Resources.EI_Inst; break;
                                case "DI": SubText = Properties.Resources.DI_Inst; break;
                                case "TRD": SubText = Properties.Resources.TRD_Inst; break;
                                case "TWR": SubText = Properties.Resources.TWR_Inst; break;
                                case "MBUS": SubText = Properties.Resources.MBUS_Inst; break;
                                case "SEND": SubText = Properties.Resources.SEND_Inst; break;
                                case "REV": SubText = Properties.Resources.REV_Inst; break;
                                case "PLSF": SubText = Properties.Resources.PLSF_Inst; break;
                                case "DPLSF": SubText = Properties.Resources.DPLSF_Inst; break;
                                case "PWM": SubText = Properties.Resources.PWM_Inst; break;
                                case "DPWM": SubText = Properties.Resources.DPWM_Inst; break;
                                case "PLSY": SubText = Properties.Resources.PLSY_Inst; break;
                                case "DPLSY": SubText = Properties.Resources.DPLSY_Inst; break;
                                case "PLSR": SubText = Properties.Resources.PLSR_Inst; break;
                                case "DPLSR": SubText = Properties.Resources.DPLSR_Inst; break;
                                case "PLSRD": SubText = Properties.Resources.PLSRD_Inst; break;
                                case "DPLSRD": SubText = Properties.Resources.DPLSRD_Inst; break;
                                case "PLSNEXT": SubText = Properties.Resources.PLSNEXT_Inst; break;
                                case "PLSSTOP": SubText = Properties.Resources.PLSSTOP_Inst; break;
                                case "ZRN": SubText = Properties.Resources.ZRN_Inst; break;
                                case "DZRN": SubText = Properties.Resources.DZRN_Inst; break;
                                case "PTO": SubText = Properties.Resources.PTO_Inst; break;
                                case "DRVI": SubText = Properties.Resources.DRVI_Inst; break;
                                case "DDRVI": SubText = Properties.Resources.DDRVI_Inst; break;
                                case "HCNT": SubText = Properties.Resources.Inst_HighCount; break;
                                case "LOG": SubText = Properties.Resources.LOG_Inst; break;
                                case "POW": SubText = Properties.Resources.POW_Inst; break;
                                case "FACT": SubText = Properties.Resources.FACT_Inst; break;
                                case "CMP": SubText = Properties.Resources.CMP_Inst; break;
                                case "CMPD": SubText = Properties.Resources.CMPD_Inst; break;
                                case "CMPF": SubText = Properties.Resources.CMPF_Inst; break;
                                case "ZCP": SubText = Properties.Resources.ZCP_Inst; break;
                                case "ZCPD": SubText = Properties.Resources.ZCPD_Inst; break;
                                case "ZCPF": SubText = Properties.Resources.ZCPF_Inst; break;
                                case "NEG": SubText = Properties.Resources.NEG_Inst; break;
                                case "NEGD": SubText = Properties.Resources.NEGD_Inst; break;
                                case "XCH": SubText = Properties.Resources.XCH_Inst; break;
                                case "XCHD": SubText = Properties.Resources.XCHD_Inst; break;
                                case "XCHF": SubText = Properties.Resources.XCHF_Inst; break;
                                case "CML": SubText = Properties.Resources.CML_Inst; break;
                                case "CMLD": SubText = Properties.Resources.CMLD_Inst; break;
                                case "SMOV": SubText = Properties.Resources.SMOV_Inst; break;
                                case "FMOV": SubText = Properties.Resources.FMOV_Inst; break;
                                case "FMOVD": SubText = Properties.Resources.FMOVD_Inst; break;
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
                MessageBox.Show(Properties.Resources.Message_Renamed_Error);
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
                TBO_Text.Focus();
                TBO_Text.SelectAll();
            }
            else
            {
                TB_ErrorMsg.Visibility = Visibility.Collapsed;
            }
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

        #region Core
        
        private void OnDiagramChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ptview.Handle(this, e, TYPE_LADDERS);
        }

        private void OnFuncBlockChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ptview.Handle(this, e, TYPE_FUNCBLOCK);
        }

        private void OnNetworkChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ptview.Handle(this, e, TYPE_NETWORK);
        }

        private void OnModbusChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ptview.Handle(this, e, TYPE_MODBUS);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnRelativePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ID": case "Brief":
                    if (RelativeObject is LadderNetworkModel)
                    {
                        LadderNetworkModel lnmodel = (LadderNetworkModel)RelativeObject;
                        if (lnmodel.Brief == null || lnmodel.Brief.Length == 0)
                            Text = String.Format("{0} {1:d}", Properties.Resources.Network, lnmodel.ID);
                        else
                            Text = String.Format("{0:d}-{1:s}", lnmodel.ID, lnmodel.Brief);
                    }
                    break;
                case "ProjName":
                    if (RelativeObject is ProjectModel)
                    {
                        ProjectModel pmodel = (ProjectModel)RelativeObject;
                        Text = String.Format("{0} - {1:s}", Properties.Resources.Project, pmodel.ProjName);
                    }
                    break;
                case "Name":
                    if (RelativeObject is LadderDiagramModel)
                    {
                        LadderDiagramModel ldmodel = (LadderDiagramModel)RelativeObject;
                        Text = ldmodel.Name;
                    }
                    if (RelativeObject is FuncBlockModel)
                    {
                        FuncBlockModel fbmodel = (FuncBlockModel)RelativeObject;
                        Text = fbmodel.Name;
                    }
                    if (RelativeObject is ModbusModel)
                    {
                        ModbusModel mmodel = (ModbusModel)RelativeObject;
                        Text = mmodel.Name;
                    }
                    break;
            }
        }

        public event RoutedEventHandler MenuItemClick = delegate { };

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
            if (ContextMenu == null)
                InitializeContextMenu();
            if (ptview.IFParent.VMDProj.LadderMode != LadderModes.Edit
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

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            if (RelativeObject is FuncBlockViewModel)
            {
                return;
            }
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

        private void TBO_Text_Loaded(object sender, RoutedEventArgs e)
        {
            TBO_Text.Focus();
            TBO_Text.SelectAll();
        }
        
        #endregion
        
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
                    profix = Properties.Resources.PTV_Folder; break;
                case ProjectTreeViewItem.TYPE_ROUTINE:
                    profix = Properties.Resources.SubRoutine; break;
                case ProjectTreeViewItem.TYPE_NETWORK:
                    profix = Properties.Resources.Network; break;
                case ProjectTreeViewItem.TYPE_FUNCBLOCK:
                    profix = Properties.Resources.FuncBlock; break;
                case ProjectTreeViewItem.TYPE_MODBUS:
                    profix = Properties.Resources.PTV_Table; break;
            }
            switch (Flags)
            {
                case ProjectTreeViewItem.FLAG_CREATEFOLDER:
                    Header = Properties.Resources.PTV_New_Folder; break;
                case ProjectTreeViewItem.FLAG_CREATEROUTINE:
                    Header = Properties.Resources.PTV_New_SubRoutine; break;
                case ProjectTreeViewItem.FLAG_CREATENETWORK:
                    Header = Properties.Resources.PTV_New_Network; break;
                case ProjectTreeViewItem.FLAG_CREATEFUNCBLOCK:
                    Header = Properties.Resources.PTV_New_Funcblock; break;
                case ProjectTreeViewItem.FLAG_CREATEMODBUS:
                    Header = Properties.Resources.PTV_New_Modbus_Table; break;
                case ProjectTreeViewItem.FLAG_RENAME:
                    Header = profix + Properties.Resources.PTV_Rename; break;
                case ProjectTreeViewItem.FLAG_REMOVE:
                    Header = profix + Properties.Resources.MainWindow_Del; break;
                case ProjectTreeViewItem.FLAG_CREATENETWORKBEFORE:
                    Header = Properties.Resources.LadderNetwork_Insert_before; break;
                case ProjectTreeViewItem.FLAG_CREATENETWORKAFTER:
                    Header = Properties.Resources.LadderNetwork_Insert_After; break;
                case ProjectTreeViewItem.FLAG_CONFIG:
                    Header = profix + Properties.Resources.MainWindow_Property_Proj; break;
            }
        }
    }
}
