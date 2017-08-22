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
using System.Threading;
using System.Windows.Threading;

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
                    if (relativeobject is LadderNetworkModel)
                    {
                        ((LadderNetworkModel)relativeobject).PropertyChanged -= OnNetworkPropertyChanged;
                    }
                    if (relativeobject is FuncBlockModel)
                    {
                        ((FuncBlockModel)relativeobject).PropertyChanged -= OnFuncBlockPropertyChanged;
                    }
                    if (relativeobject is ModbusTableModel)
                    {
                        ((ModbusTableModel)relativeobject).ChildrenChanged -= OnModbusTableChanged;
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
                    if (relativeobject is LadderNetworkModel)
                    {
                        ((LadderNetworkModel)relativeobject).PropertyChanged += OnNetworkPropertyChanged;
                    }
                    if (relativeobject is FuncBlockModel)
                    {
                        ((FuncBlockModel)relativeobject).PropertyChanged += OnFuncBlockPropertyChanged;
                    }
                    if (relativeobject is ModbusTableModel)
                    {
                        ((ModbusTableModel)relativeobject).ChildrenChanged += OnModbusTableChanged;
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
                            LadderNetworkModel lnmodel = (LadderNetworkModel)RelativeObject;
                            if (lnmodel.Brief == null || lnmodel.Brief.Length == 0)
                                Text = String.Format("{0} {1:d}", Properties.Resources.Network, lnmodel.ID);
                            else
                                Text = String.Format("{0:d}-{1:s}", lnmodel.ID, lnmodel.Brief);
                            TBL_Text.Foreground = lnmodel.IsMasked ? Brushes.Gray : Brushes.Black;
                            TBS_Text.Foreground = lnmodel.IsMasked ? Brushes.Gray : Brushes.Black;
                            //TBO_Text.Foreground = lnmodel.IsMasked ? Brushes.Gray : Brushes.Black;
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
                            switch (LadderUnitModel.Formats[(int)type].Shape)
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
                            Text = LadderUnitModel.Formats[(int)type].Name;
                            SubText = LadderUnitModel.Formats[(int)type].Describe;
                            ToolTip = LadderUnitModel.Formats[(int)type].Detail;
                            switch (Text)
                            {
                                case "LD": Text = "-|　|-"; break;
                                case "LDIM": Text = "-|｜|-"; break;
                                case "LDI": Text = "-|／|-";  break;
                                case "LDIIM": Text = "-|/||-";  break;
                                case "LDP": Text = "-|↑|-"; break;
                                case "LDF": Text = "-|↓|-"; break;
                                case "LDWEQ": Text = "-Ｗ＝-"; break;
                                case "LDWNE": Text = "-Ｗ≠-"; break;
                                case "LDWLE": Text = "-Ｗ≤-"; break;
                                case "LDWGE": Text = "-Ｗ≥-"; break;
                                case "LDWL": Text = "-Ｗ＜-"; break;
                                case "LDWG": Text = "-Ｗ＞-"; break;
                                case "LDDEQ": Text = "-Ｄ＝-"; break;
                                case "LDDNE": Text = "-Ｄ≠-"; break;
                                case "LDDLE": Text = "-Ｄ≤-"; break;
                                case "LDDGE": Text = "-Ｄ≥-"; break;
                                case "LDDL": Text = "-Ｄ＜-"; break;
                                case "LDDG": Text = "-Ｄ＞-"; break;
                                case "LDFEQ": Text = "-Ｆ＝-"; break;
                                case "LDFNE": Text = "-Ｆ≠-"; break;
                                case "LDFLE": Text = "-Ｆ≤-"; break;
                                case "LDFGE": Text = "-Ｆ≥-"; break;
                                case "LDFL": Text = "-Ｆ＜-"; break;
                                case "LDFG": Text = "-Ｆ＞-"; break;
                                case "OUT": Text = "-(　) "; break;
                                case "OUTIM": Text = "-(｜) "; break;
                                case "SET": Text = "-(Ｓ) "; break;
                                case "SETIM": Text = "-(SI) "; break;
                                case "RST": Text = "-(Ｒ) "; break;
                                case "RSTIM": Text = "-(RI) "; break;
                                case "MEP": Text = "- ↑ -"; break;
                                case "MEF": Text = "- ↓ -"; break;
                                case "INV": Text = "- ／ -"; break;
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

        private void OnNetworkPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Flags = Flags;
        }

        private void OnFuncBlockPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Funcs": IsExpanded = false; break;
            }
        }
        
        private void OnModbusTableChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ptview.Handle(this, e, TYPE_MODBUS);
        }

        private void OnModbusChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //ptview.Handle(this, e, TYPE_MODBUS);
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
                        //Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)(delegate ()
                        //{
                            if (lnmodel.Brief == null || lnmodel.Brief.Length == 0)
                                Text = String.Format("{0} {1:d}", Properties.Resources.Network, lnmodel.ID);
                            else
                                Text = String.Format("{0:d}-{1:s}", lnmodel.ID, lnmodel.Brief);
                        //}));
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
            if(Items.Count > 0)
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
                        e.Handled = false;
                        break;
                }
            }
            else e.Handled = true;
        }

        private void OnCollapsed(object sender, RoutedEventArgs e)
        {
            var item = sender as ProjectTreeViewItem;
            if (Items.Count > 0)
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
            else e.Handled = true;
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (Items.Count == 0)
            {
                switch (Flags & 0xf)
                {
                    case TYPE_FUNCBLOCKFLODER:
                    case TYPE_MODBUSFLODER:
                    case TYPE_NETWORKFLODER:
                    case TYPE_ROUTINEFLODER:
                        IconSource = "/Resources/Image/MainStyle/folderClose.png";
                        break;
                    default:
                        break;
                }
            }
            else if (Items.Count == 1 && IsExpanded)
            {
                switch (Flags & 0xf)
                {
                    case TYPE_FUNCBLOCKFLODER:
                    case TYPE_MODBUSFLODER:
                    case TYPE_NETWORKFLODER:
                    case TYPE_ROUTINEFLODER:
                        IconSource = "/Resources/Image/MainStyle/folderOpen.png";
                        break;
                    default:
                        break;
                }
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
                    Header = profix + Properties.Resources.Property_Proj; break;
            }
        }
    }
}
