using System;
using System.Collections.Generic;
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
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.UserInterface;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SamSoarII.Extend.FuncBlockModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using static SamSoarII.AppMain.Project.LadderDiagramViewModel;

/// <summary>
/// Namespace : SamSoarII.AppMain.UI
/// ClassName : ProjectTreeView
/// Version   : 1.0
/// Date      : 2017/6/9
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 工程资源管理器
/// </remarks>

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Numbers
        /// <summary> 所有子程序的对应的目录项 </summary>
        private Dictionary<string, ProjectTreeViewItem> dpdict
            = new Dictionary<string, ProjectTreeViewItem>();
        /// <summary> 所有函数块的对应的目录项 </summary>
        private Dictionary<string, ProjectTreeViewItem> fpdict
            = new Dictionary<string, ProjectTreeViewItem>();
        /// <summary> 工程模型 </summary>
        private ProjectModel _projectModel;
        /// <summary> 工程模型 </summary>
        public ProjectModel Project
        {
            get { return _projectModel; }
        }

        #region Components

        private ProjectTreeViewItem PTVI_Root;
        private ProjectTreeViewItem PTVI_Program;
        private ProjectTreeViewItem PTVI_MainRoutine;
        private ProjectTreeViewItem PTVI_SubRoutines;
        private ProjectTreeViewItem PTVI_FuncBlocks;
        private ProjectTreeViewItem PTVI_LibFuncBlock;
        private ProjectTreeViewItem PTVI_ElementList;
        private ProjectTreeViewItem PTVI_ELementInitWdow;
        private ProjectTreeViewItem PTVI_Modbus;
        private ProjectTreeViewItem PTVI_Ladders;
        private ProjectTreeViewItem[] PTVI_Insts;
        static private string[] PTVIH_Insts
        = { Properties.Resources.Inst_Bit, Properties.Resources.Inst_Compare, Properties.Resources.Inst_Convert,
            Properties.Resources.Inst_LogicOperation,Properties.Resources.Inst_Move,
            Properties.Resources.Inst_FloatCalculation, Properties.Resources.Inst_IntegerCalculation,
            Properties.Resources.Inst_Timer,Properties.Resources.Inst_Counter, Properties.Resources.Inst_ProgramControl,
            Properties.Resources.Inst_Shift, Properties.Resources.Inst_Interrupt,
            Properties.Resources.Inst_RealTime, Properties.Resources.Inst_Communication,
            Properties.Resources.Inst_Pulse, Properties.Resources.Inst_HighCount,
            Properties.Resources.Inst_Auxiliar};

        #endregion

        #endregion

        #region Initialize
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="project">工程</param>
        /// <param name="xele">目录文档</param>
        public ProjectTreeView(ProjectModel project, XElement xele = null)
        {
            InitializeComponent();
            ProjectTreeViewItem.HasRenaming = false;
            _projectModel = project;
            InteractionFacade _ifacade = Project.IFacade;
            _ifacade.PTVEvent += OnGotPTVEvent;
            DataContext = Project;
            Project.MTVModel.ModelChanged += OnModbusChanged;
            // 构造目录
            ReinitializeComponent();
            if (xele != null) Load(xele);
            ProjectTreeViewItem ptvitem = null;
            // 构造子程序的目录
            foreach (LadderDiagramViewModel ldvmodel in project.SubRoutines)
            {
                ProjectTreeViewItem parent = null;
                if (dpdict.ContainsKey(ldvmodel.ProgramName))
                {
                    parent = dpdict[ldvmodel.ProgramName];
                    dpdict.Remove(ldvmodel.ProgramName);
                }
                else
                {
                    parent = PTVI_SubRoutines;
                }
                ptvitem = CreatePTVItem(
                        parent,
                        ProjectTreeViewItem.TYPE_ROUTINE
                      | ProjectTreeViewItem.FLAG_CREATENETWORK
                      | ProjectTreeViewItem.FLAG_RENAME
                      | ProjectTreeViewItem.FLAG_REMOVE,
                        ldvmodel, false);
                Rebuild(ptvitem, ldvmodel);
                dpdict.Add(ldvmodel.ProgramName, ptvitem);
            }
            // 构造函数块的目录
            foreach (FuncBlockViewModel fbvmodel in project.FuncBlocks)
            {
                fbvmodel.TextChanged += OnFuncBlockTextChanged;
                ProjectTreeViewItem parent = null;
                if (fpdict.ContainsKey(fbvmodel.ProgramName))
                {
                    parent = fpdict[fbvmodel.ProgramName];
                    fpdict.Remove(fbvmodel.ProgramName);
                }
                else
                {
                    parent = PTVI_FuncBlocks;
                }
                ptvitem = CreatePTVItem(
                        parent,
                        ProjectTreeViewItem.TYPE_FUNCBLOCK
                      | ProjectTreeViewItem.FLAG_RENAME
                      | ProjectTreeViewItem.FLAG_REMOVE,
                        fbvmodel, false);
                Rebuild(ptvitem, fbvmodel);
                fpdict.Add(fbvmodel.ProgramName, ptvitem);
            }
            PTVI_Root.IsExpanded = true;
            PTVI_Program.IsExpanded = true;
            Rebuild(PTVI_MainRoutine, project.MainRoutine);
            if (dpdict.ContainsKey(project.MainRoutine.ProgramName))
            {
                dpdict.Remove(project.MainRoutine.ProgramName);
            }
            dpdict.Add(project.MainRoutine.ProgramName, PTVI_MainRoutine);
            Rebuild(PTVI_Modbus, project.MTVModel);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        public void Dispose()
        {
            _projectModel.IFacade.PTVEvent -= OnGotPTVEvent;
            _projectModel.MTVModel.ModelChanged -= OnModbusChanged;
        }

        private void ReinitializeComponent()
        {
            PTVI_Root = CreatePTVItem(
                null,
                ProjectTreeViewItem.TYPE_ROOT,
                _projectModel);
            TV_Main.Items.Add(PTVI_Root);

            PTVI_Program = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_PROGRAM,
                Properties.Resources.Routine);

            PTVI_MainRoutine = CreatePTVItem(
                PTVI_Program,
                ProjectTreeViewItem.TYPE_ROUTINE
              | ProjectTreeViewItem.FLAG_CREATENETWORK,
                _projectModel.MainRoutine);

            PTVI_SubRoutines = CreatePTVItem(
                PTVI_Program,
                ProjectTreeViewItem.TYPE_ROUTINEFLODER
              | ProjectTreeViewItem.FLAG_CREATEFOLDER
              | ProjectTreeViewItem.FLAG_CREATEROUTINE,
                Properties.Resources.SubRoutine, true, true);

            PTVI_FuncBlocks = CreatePTVItem(
                PTVI_Program,
                ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER
              | ProjectTreeViewItem.FLAG_CREATEFOLDER
              | ProjectTreeViewItem.FLAG_CREATEFUNCBLOCK,
                Properties.Resources.FuncBlock, true, true);

            PTVI_LibFuncBlock = CreatePTVItem(
                PTVI_FuncBlocks,
                ProjectTreeViewItem.TYPE_FUNCBLOCK,
                _projectModel.LibFuncBlock);
            Rebuild(PTVI_LibFuncBlock, _projectModel.LibFuncBlock);

            PTVI_Modbus = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_MODBUSFLODER
              | ProjectTreeViewItem.FLAG_CREATEMODBUS,
                _projectModel.MTVModel);

            PTVI_ElementList = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_ELEMENTLIST,
                new object());

            PTVI_ELementInitWdow = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_ELEMENTINITIALIZE,
                new object());

            PTVI_Ladders = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_LADDERS,
                Properties.Resources.Instruction);

            PTVI_Insts = new ProjectTreeViewItem[PTVIH_Insts.Length];
            for (int i = 0; i < PTVIH_Insts.Length; i++)
            {
                PTVI_Insts[i] = CreatePTVItem(
                    PTVI_Ladders,
                    ProjectTreeViewItem.TYPE_LADDERS,
                    PTVIH_Insts[i]);
            }
            foreach (BaseViewModel bvmodel in LadderInstViewModelPrototype.GetElementViewModels())
            {
                int id = bvmodel.GetCatalogID() / 100 - 2;
                if (id >= 0 && id < PTVI_Insts.Length)
                {
                    CreatePTVItem(
                        PTVI_Insts[id],
                        ProjectTreeViewItem.TYPE_INSTRUCTION,
                        bvmodel,
                        false);
                }
            }
        }

        private ProjectTreeViewItem CreatePTVItem
        (
            ProjectTreeViewItem     parent,
            int                     flags,
            object                  relativeObject,
            bool                    iscritical = true,
            bool                    isorder = false
        )
        {
            ProjectTreeViewItem createitem = new ProjectTreeViewItem(this);
            createitem.RelativeObject = relativeObject;
            createitem.Flags = flags;
            if (parent != null)
            {
                if (parent.IsOrder)
                {
                    for (int i = 0; i < parent.Items.Count; i++)
                    {
                        if (createitem.CompareTo((ProjectTreeViewItem)(parent.Items[i])) <= 0)
                        {
                            parent.Items.Insert(i, createitem);
                            break;
                        }
                    }
                }
                if (!parent.Items.Contains(createitem))
                {
                    parent.Items.Add(createitem);
                }
                createitem.RegisterPath();
            }
            createitem.MouseDoubleClick += OnPTVIDoubleClick;
            createitem.MenuItemClick += ONPTVIMenuClick;
            createitem.Renamed += OnPTVIRenamed;
            createitem.Expanded += OnPTVIExpanded;
            createitem.IsCritical = iscritical;
            createitem.IsOrder = isorder;
            return createitem;
        }

        #endregion

        #region Rebuild Tree

        private void Rebuild(ProjectTreeViewItem ptvitem, LadderDiagramViewModel ldvmodel)
        {
            ptvitem.RelativeObject = ldvmodel;
            ptvitem.Items.Clear();
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                CreatePTVItem(
                    ptvitem,
                    ProjectTreeViewItem.TYPE_NETWORK
                  | ProjectTreeViewItem.FLAG_REMOVE
                  | ProjectTreeViewItem.FLAG_CREATENETWORKBEFORE
                  | ProjectTreeViewItem.FLAG_CREATENETWORKAFTER
                  | ProjectTreeViewItem.FLAG_CONFIG,
                    lnvmodel, false);
            }
        }

        private void Rebuild(ProjectTreeViewItem ptvitem, FuncBlockViewModel fbvmodel)
        {
            ptvitem.RelativeObject = fbvmodel;
            ptvitem.Items.Clear();
            if (fbvmodel.Funcs.Count() == 0)
            {
                CreatePTVItem(
                    ptvitem,
                    ProjectTreeViewItem.TYPE_CONST,
                    Properties.Resources.PTV_No_Function);
            }
            foreach (FuncModel fmodel in fbvmodel.Funcs)
            {
                CreatePTVItem(
                    ptvitem,
                    ProjectTreeViewItem.TYPE_FUNC,
                    fmodel, false);
            }
        }

        private void Rebuild(ProjectTreeViewItem ptvitem, ModbusTableViewModel mtvmodel)
        {
            ptvitem.RelativeObject = mtvmodel;
            ptvitem.Items.Clear();
            foreach (ModbusTableModel mtmodel in mtvmodel.Models)
            {
                CreatePTVItem(
                    ptvitem, 
                    ProjectTreeViewItem.TYPE_MODBUS
                  | ProjectTreeViewItem.FLAG_RENAME
                  | ProjectTreeViewItem.FLAG_REMOVE,
                    mtmodel,
                    false);
            }
        }

        #endregion

        #region Modification

        private void RemoveAll(ProjectTreeViewItem ptvitem)
        {
            ProjectTreeViewEventArgs _e = null;
            switch (ptvitem.Flags & 0xf)
            {
                case ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER:
                case ProjectTreeViewItem.TYPE_ROUTINEFLODER:
                case ProjectTreeViewItem.TYPE_MODBUSFLODER:
                case ProjectTreeViewItem.TYPE_NETWORKFLODER:
                    List<ProjectTreeViewItem> ptvis = new List<ProjectTreeViewItem>();
                    foreach (ProjectTreeViewItem _ptvitem in ptvitem.Items)
                    {
                        ptvis.Add(_ptvitem);
                    }
                    foreach (ProjectTreeViewItem _ptvitem in ptvis)
                    {
                        RemoveAll(_ptvitem);
                    }
                    break;
                case ProjectTreeViewItem.TYPE_FUNCBLOCK:
                case ProjectTreeViewItem.TYPE_ROUTINE:
                case ProjectTreeViewItem.TYPE_MODBUS:
                case ProjectTreeViewItem.TYPE_NETWORK:
                    if (ptvitem.Parent is ProjectTreeViewItem)
                    {
                        var parent = ptvitem.Parent as ProjectTreeViewItem;
                        if (parent.Items.Count == 1)
                            parent.IsExpanded = false;
                    }
                    _e = new ProjectTreeViewEventArgs((ptvitem.Flags & 0xf) | ProjectTreeViewEventArgs.FLAG_REMOVE,
                        ptvitem.RelativeObject, ptvitem);
                    PTVHandle(this, _e);
                    break;
                default:
                    throw new ArgumentException(String.Format("Cannot remove this item : {0:s}", ptvitem));
            }
            if (ptvitem.Parent is ProjectTreeViewItem)
            {
                var parent = ptvitem.Parent as ProjectTreeViewItem;
                if (parent.Items.Count == 1)
                    parent.IsExpanded = false;
                parent.Items.Remove(ptvitem);
            }
        }

        #endregion

        #region Event Handler

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        public event ShowTabItemEventHandler TabItemOpened = delegate { };

        public event NavigateToNetworkEventHandler NavigatedToNetwork = delegate { };

        public event ProjectTreeViewEventHandler PTVHandle = delegate { };

        public event RoutedEventHandler PTVRenamed = delegate { };

        public event MouseButtonEventHandler InstructionTreeItemDoubleClick = delegate { };

        private void OnPTVIDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ProjectTreeViewItem)
            {
                ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
                if (ptvitem.IsRenaming) return;
                switch (ptvitem.Flags & 0x0f)
                {
                    case ProjectTreeViewItem.TYPE_ROUTINE:
                        TabItemOpened(ptvitem.RelativeObject, new ShowTabItemEventArgs(TabType.Program));
                        break;
                    case ProjectTreeViewItem.TYPE_NETWORK:
                        if (ptvitem.RelativeObject is LadderNetworkViewModel)
                        {
                            LadderNetworkViewModel lnvmodel = (LadderNetworkViewModel)(ptvitem.RelativeObject);
                            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
                            NavigateToNetworkEventArgs _e1 = new NavigateToNetworkEventArgs(
                                lnvmodel.NetworkNumber,
                                ldvmodel.ProgramName,
                                0, 0);
                            NavigatedToNetwork(_e1);
                        }
                        break;
                    case ProjectTreeViewItem.TYPE_FUNCBLOCK:
                        TabItemOpened(ptvitem.RelativeObject, new ShowTabItemEventArgs(TabType.Program));
                        break;
                    case ProjectTreeViewItem.TYPE_FUNC:
                        ProjectTreeViewItem parent = (ProjectTreeViewItem)(ptvitem.Parent);
                        FuncBlockViewModel fbvmodel = (FuncBlockViewModel)(parent.RelativeObject);
                        FuncModel fmodel = (FuncModel)(ptvitem.RelativeObject);
                        Project.IFacade.NavigateToFuncBlock(fbvmodel, fmodel.Offset);
                        break;
                    case ProjectTreeViewItem.TYPE_MODBUS:
                        TabItemOpened(ptvitem.RelativeObject, new ShowTabItemEventArgs(TabType.Modbus));
                        if (ptvitem.RelativeObject is ModbusTableModel)
                        {
                            _projectModel.MTVModel.Current = (ModbusTableModel)ptvitem.RelativeObject;
                        }
                        break;
                    case ProjectTreeViewItem.TYPE_MODBUSFLODER:
                        TabItemOpened(ptvitem.RelativeObject, new ShowTabItemEventArgs(TabType.Modbus));
                        break;
                    case ProjectTreeViewItem.TYPE_ELEMENTLIST:
                        Project.IFacade.MainWindow.LACElemList.Show();
                        break;
                    case ProjectTreeViewItem.TYPE_ELEMENTINITIALIZE:
                        Project.IFacade.MainWindow.LACElemInit.Show();
                        break;
                    case ProjectTreeViewItem.TYPE_INSTRUCTION:
                        ProjectTreeViewEventArgs _e2 = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_INSTRUCTION 
                          | ProjectTreeViewEventArgs.FLAG_REPLACE, 
                            ptvitem.RelativeObject, ptvitem.RelativeObject);
                        PTVHandle(this, _e2);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ONPTVIMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is ProjectMenuItem)
            {
                ProjectMenuItem pmitem = (ProjectMenuItem)sender;
                ProjectTreeViewItem ptvitem = pmitem.PTVItem;
                ProjectTreeViewEventArgs _e = null;
                int flags_type = ptvitem.Flags & 0xf;
                switch (pmitem.Flags)
                {
                    case ProjectTreeViewItem.FLAG_CREATEROUTINE:
                        _e = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_ROUTINE 
                          | ProjectTreeViewEventArgs.FLAG_CREATE,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_CREATENETWORK:
                        _e = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_NETWORK
                          | ProjectTreeViewEventArgs.FLAG_CREATE,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_CREATENETWORKBEFORE:
                        _e = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_NETWORK
                          | ProjectTreeViewEventArgs.FLAG_CREATEBEFORE,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_CREATENETWORKAFTER:
                        _e = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_NETWORK
                          | ProjectTreeViewEventArgs.FLAG_CREATEAFTER,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_CONFIG:
                        _e = new ProjectTreeViewEventArgs(
                            flags_type
                          | ProjectTreeViewEventArgs.FLAG_CONFIG,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_CREATEFUNCBLOCK:
                        _e = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_FUNCBLOCK
                          | ProjectTreeViewEventArgs.FLAG_CREATE,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_CREATEMODBUS:
                        _e = new ProjectTreeViewEventArgs(
                            ProjectTreeViewEventArgs.TYPE_MODBUS
                          | ProjectTreeViewEventArgs.FLAG_CREATE,
                            pmitem.RelativeObject, ptvitem);
                        PTVHandle(this, _e);
                        break;
                    case ProjectTreeViewItem.FLAG_RENAME:
                        ptvitem.Rename();
                        break;
                    case ProjectTreeViewItem.FLAG_REMOVE:
                        MessageBoxResult result = MessageBox.Show(
                            Properties.Resources.Message_Delete, Properties.Resources.Important, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                            RemoveAll(ptvitem);
                        break;
                    case ProjectTreeViewItem.FLAG_CREATEFOLDER:
                        ProjectTreeViewItem createitem = CreatePTVItem(
                            ptvitem,
                            ptvitem.Flags 
                          | ProjectTreeViewItem.FLAG_RENAME 
                          | ProjectTreeViewItem.FLAG_REMOVE, 
                            String.Empty, 
                            false, true);
                        ptvitem.IsExpanded = true;
                        createitem.Loaded += OnCreateItemLoaded;
                        break;
                }
            }
        }

        private void OnCreateItemLoaded(object sender, RoutedEventArgs e)
        {
            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
            ptvitem.Loaded -= OnCreateItemLoaded;
            ptvitem.Rename();
        }
        
        private void OnPTVIRenamed(object sender, RoutedEventArgs e)
        {
            if (sender is ProjectTreeViewItem)
            {
                ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
                ProjectTreeViewItem ptvparent = (ProjectTreeViewItem)(ptvitem.Parent);
                ProjectTreeViewEventArgs _e = null;
                if (ptvitem.Text.Equals(String.Empty))
                {
                    ptvitem.Rename(Properties.Resources.Message_Name_Needed);
                    return;
                }
                for (int i = 0; i < ptvitem.Text.Length; i++)
                {
                    switch (ptvitem.Text[i])
                    {
                        case '#': case '@':
                            ptvitem.Rename(String.Format("{0}{1:c}", Properties.Resources.Message_illegal_Char, ptvitem.Text[i]));
                            return;
                    }
                }
                foreach (ProjectTreeViewItem _ptvitem in ptvparent.Items)
                {
                    if (_ptvitem != ptvitem && _ptvitem.Text.Equals(ptvitem.Text))
                    {
                        ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Fold_File_Exist, ptvitem.Text));
                        return;
                    }
                }
                if (ptvitem.RelativeObject is LadderDiagramViewModel)
                {
                    Match m = Regex.Match(ptvitem.Text, @"^[a-zA-Z_]\w*$");
                    if (!m.Success)
                    {
                        ptvitem.Rename(Properties.Resources.Message_Name_Format_illegal);
                        return;
                    }
                    LadderDiagramViewModel ldvmodel = (LadderDiagramViewModel)(ptvitem.RelativeObject);
                    foreach (LadderDiagramViewModel _ldvmodel in _projectModel.SubRoutines)
                    {
                        if (_ldvmodel != ldvmodel && _ldvmodel.ProgramName.Equals(ptvitem.Text))
                        {
                            ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Subroutine_Exist, ptvitem.Text));
                            return;
                        }
                    }
                    ptvitem.RenameClose();
                    dpdict.Remove(ldvmodel.ProgramName);
                    ldvmodel.ProgramName = ptvitem.Text;
                    dpdict.Add(ldvmodel.ProgramName, ptvitem);
                    _e = new ProjectTreeViewEventArgs(
                        ProjectTreeViewEventArgs.TYPE_ROUTINE | ProjectTreeViewEventArgs.FLAG_REPLACE,
                        ldvmodel, ptvitem.Text);
                    PTVHandle(this, _e);
                }
                else if (ptvitem.RelativeObject is FuncBlockViewModel)
                {
                    FuncBlockViewModel fbvmodel = (FuncBlockViewModel)(ptvitem.RelativeObject);
                    foreach (FuncBlockViewModel _fbvmodel in _projectModel.FuncBlocks)
                    {
                        if (_fbvmodel != fbvmodel && _fbvmodel.ProgramName.Equals(ptvitem.Text))
                        {
                            ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Funcblock_Exist, ptvitem.Text));
                            return;
                        }
                    }
                    ptvitem.RenameClose();
                    fpdict.Remove(fbvmodel.ProgramName);
                    fbvmodel.ProgramName = ptvitem.Text;
                    fpdict.Add(fbvmodel.ProgramName, ptvitem);
                    _e = new ProjectTreeViewEventArgs(
                        ProjectTreeViewEventArgs.TYPE_FUNCBLOCK | ProjectTreeViewEventArgs.FLAG_REPLACE,
                        fbvmodel, ptvitem.Text);
                    PTVHandle(this, _e);
                }
                else if (ptvitem.RelativeObject is ModbusTableModel)
                {
                    ModbusTableModel mtmodel = (ModbusTableModel)(ptvitem.RelativeObject);
                    foreach (ModbusTableModel _mtmodel in _projectModel.MTVModel.Models)
                    {
                        if (_mtmodel != mtmodel && _mtmodel.Name.Equals(ptvitem.Text))
                        {
                            ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Funcblock_Exist, ptvitem.Text));
                            return;
                        }
                    }
                    ptvitem.RenameClose();
                    mtmodel.Name = ptvitem.Text;
                    _e = new ProjectTreeViewEventArgs(
                        ProjectTreeViewEventArgs.TYPE_MODBUS | ProjectTreeViewEventArgs.FLAG_REPLACE,
                        mtmodel, ptvitem.Text);
                    PTVHandle(this, _e);
                }
                else
                {
                    ptvitem.RenameClose();
                }
                PTVRenamed(this,null);
            }
        }
        
        private void OnPTVIExpanded(object sender, RoutedEventArgs e)
        {
            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
            if (ptvitem.RelativeObject is FuncBlockViewModel)
            {
                Rebuild(ptvitem, (FuncBlockViewModel)(ptvitem.RelativeObject));
            }
        }

        private void OnGotPTVEvent(object sender, ProjectTreeViewEventArgs e)
        {
            ProjectTreeViewItem selectitem = null;
            ProjectTreeViewItem createitem = null;
            ProjectTreeViewItem deleteitem = null;
            string rname = null;
            string tname = null;
            switch (e.Flags & ~0xf)
            {
                case ProjectTreeViewEventArgs.FLAG_CREATE:
                case ProjectTreeViewEventArgs.FLAG_CREATEBEFORE:
                case ProjectTreeViewEventArgs.FLAG_CREATEAFTER:
                    if (ProjectTreeViewItem.HasRenaming)
                    {
                        MessageBox.Show("存在正在被命名的项目，不能进行添加操作！");
                        return;
                    }
                    break;
            }
            if (e.TargetedObject is ProjectTreeViewItem)
            {
                selectitem = (ProjectTreeViewItem)e.TargetedObject;
            }
            switch (e.Flags & 0xf)
            {
                case ProjectTreeViewEventArgs.TYPE_ROUTINE:
                    if (!(e.RelativeObject is LadderDiagramViewModel))
                    {
                        throw new ArgumentException(String.Format("Unsupported RelativeObject {0:s}", e.RelativeObject));
                    }
                    rname = ((LadderDiagramViewModel)(e.RelativeObject)).ProgramName;
                    if (selectitem == null)
                    {
                        if (e.TargetedObject is LadderDiagramViewModel)
                        {
                            tname = ((LadderDiagramViewModel)(e.TargetedObject)).ProgramName;
                            if (dpdict.ContainsKey(tname))
                            {
                                selectitem = dpdict[tname];
                                selectitem = (ProjectTreeViewItem)(selectitem.Parent);
                            }
                        }
                    }
                    if (selectitem == null)
                    {
                        selectitem = PTVI_SubRoutines;
                    }
                    switch (e.Flags & ~0xf)
                    {
                        case ProjectTreeViewEventArgs.FLAG_CREATE:
                            createitem = CreatePTVItem(
                                selectitem,
                                ProjectTreeViewItem.TYPE_ROUTINE
                              | ProjectTreeViewItem.FLAG_CREATENETWORK
                              | ProjectTreeViewItem.FLAG_RENAME
                              | ProjectTreeViewItem.FLAG_REMOVE,
                                e.RelativeObject,
                                false);
                            selectitem.IsExpanded = true;
                            Rebuild(createitem, (LadderDiagramViewModel)(e.RelativeObject));
                            //dpdict.Add(rname, createitem);
                            createitem.Rename();
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            if (!dpdict.ContainsKey(rname))
                            {
                                throw new ArgumentException(String.Format("Cannot found routine {0:s} in the ProjectTreeView.", rname));
                            }
                            deleteitem = dpdict[rname];
                            selectitem = (ProjectTreeViewItem)(deleteitem.Parent);
                            selectitem.Items.Remove(deleteitem);
                            dpdict.Remove(rname);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REPLACE:
                            if (tname == null)
                            {
                                tname = e.TargetedObject.ToString();
                            }
                            if (dpdict.ContainsKey(tname))
                            {
                                selectitem = dpdict[tname];
                            }
                            else
                            {
                                throw new ArgumentException(String.Format("Cannot found routine {0:s}", tname));
                            }
                            Rebuild(selectitem, (LadderDiagramViewModel)(e.RelativeObject));
                            dpdict.Remove(tname);
                            dpdict.Add(rname, selectitem);
                            break;
                    }
                    break;
                case ProjectTreeViewEventArgs.TYPE_NETWORK:
                    
                    if (!(e.RelativeObject is LadderNetworkViewModel)
                     && !(e.RelativeObject is LadderDiagramViewModel)
                     && !(e.RelativeObject is int))
                    {
                        throw new ArgumentException(String.Format("Unsupported RelativeObject {0:s}", e.RelativeObject));
                    }
                    LadderNetworkViewModel lnvmodel = null;
                    LadderDiagramViewModel ldvmodel = null;
                    if (e.RelativeObject is LadderNetworkViewModel)
                    {
                        lnvmodel = (LadderNetworkViewModel)(e.RelativeObject);
                    }
                    if (e.RelativeObject is LadderDiagramViewModel)
                    {
                        ldvmodel = (LadderDiagramViewModel)(e.RelativeObject);
                    }
                    if (e.RelativeObject is int)
                    {
                        if (e.TargetedObject is LadderNetworkViewModel)
                        {
                            lnvmodel = (LadderNetworkViewModel)(e.TargetedObject);
                        }
                    }
                    if (ldvmodel == null)
                    {
                        ldvmodel = lnvmodel.LDVModel;
                    }
                    if (!dpdict.ContainsKey(ldvmodel.ProgramName))
                    {
                        throw new ArgumentException(String.Format("Cannot found routine {0:s} in the ProjectTreeView.", ldvmodel.ProgramName));
                    }
                    selectitem = dpdict[ldvmodel.ProgramName];
                    Rebuild(selectitem, ldvmodel);
                    break;
                case ProjectTreeViewEventArgs.TYPE_FUNCBLOCK:
                    if (!(e.RelativeObject is FuncBlockViewModel))
                    {
                        throw new ArgumentException(String.Format("Unsupported RelativeObject {0:s}", e.RelativeObject));
                    }
                    rname = ((FuncBlockViewModel)(e.RelativeObject)).ProgramName;
                    if (selectitem == null)
                    {
                        if (e.TargetedObject is FuncBlockViewModel)
                        {
                            tname = ((FuncBlockViewModel)(e.TargetedObject)).ProgramName;
                            if (dpdict.ContainsKey(tname))
                            {
                                selectitem = dpdict[tname];
                                selectitem = (ProjectTreeViewItem)(selectitem.Parent);
                            }
                        }
                    }
                    if (selectitem == null)
                    {
                        selectitem = PTVI_FuncBlocks;
                    }
                    switch (e.Flags & ~0xf)
                    {
                        case ProjectTreeViewEventArgs.FLAG_CREATE:
                            createitem = CreatePTVItem(
                                selectitem,
                                ProjectTreeViewItem.TYPE_FUNCBLOCK
                              | ProjectTreeViewItem.FLAG_RENAME
                              | ProjectTreeViewItem.FLAG_REMOVE,
                                e.RelativeObject,
                                false);
                            selectitem.IsExpanded = true;
                            ((FuncBlockViewModel)(e.RelativeObject)).TextChanged += OnFuncBlockTextChanged;
                            Rebuild(createitem, (FuncBlockViewModel)(e.RelativeObject));
                            //fpdict.Add(rname, createitem);
                            createitem.Rename();
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REMOVE:
                            if (!fpdict.ContainsKey(rname))
                            {
                                throw new ArgumentException(String.Format("Cannot found funcblock {0:s} in the ProjectTreeView.", rname));
                            }
                            deleteitem = fpdict[rname];
                            selectitem = (ProjectTreeViewItem)(deleteitem.Parent);
                            selectitem.Items.Remove(deleteitem);
                            fpdict.Remove(rname);
                            break;
                        case ProjectTreeViewEventArgs.FLAG_REPLACE:
                            if (tname == null)
                            {
                                tname = e.TargetedObject.ToString();
                            }
                            if (fpdict.ContainsKey(tname))
                            {
                                selectitem = fpdict[tname];
                            }
                            else
                            {
                                throw new ArgumentException(String.Format("Cannot found FuncBlock {0:s}", tname));
                            }
                            Rebuild(selectitem, (FuncBlockViewModel)(e.RelativeObject));
                            fpdict.Remove(tname);
                            fpdict.Add(rname, selectitem);
                            break;
                    }
                    break;
                case ProjectTreeViewEventArgs.TYPE_MODBUS:
                    if (!(e.RelativeObject is ModbusTableViewModel))
                    {
                        throw new ArgumentException(String.Format("Unsupported RelativeObject {0:s}", e.RelativeObject));
                    }
                    Rebuild(PTVI_Modbus, (ModbusTableViewModel)(e.RelativeObject));
                    break;
            }
        }

        private void OnModbusChanged(object sender, RoutedEventArgs e)
        {
            ProjectTreeViewEventArgs _e = new ProjectTreeViewEventArgs(
                ProjectTreeViewEventArgs.TYPE_MODBUS, _projectModel.MTVModel, _projectModel.MTVModel);
            OnGotPTVEvent(sender, _e);
        }

        private void OnFuncBlockTextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is FuncBlockViewModel)
            {
                FuncBlockViewModel fbvmodel = (FuncBlockViewModel)sender;
                if (fpdict.ContainsKey(fbvmodel.ProgramName))
                {
                    ProjectTreeViewItem ptvitem = fpdict[fbvmodel.ProgramName];
                    if (ptvitem.IsExpanded == true)
                    {
                        ptvitem.IsExpanded = false;
                    }
                }
            }
        }
        
        #region Drag & Drop

        private ProjectTreeViewItem dragitem = null;
        public ProjectTreeViewItem DragItem
        {
            get { return this.dragitem; }
            private set
            {
                if (value == null)
                {
                    this.dragitem = null;
                    return;
                }
                if (_projectModel.LadderMode != LadderMode.Edit) return;
                if (value.IsCritical) return;
                if (value.IsRenaming) return;
                if ((value.Flags & 0xf) == ProjectTreeViewItem.TYPE_NETWORK)
                {
                    LadderNetworkViewModel lnvmodel = (LadderNetworkViewModel)(value.RelativeObject);
                    LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
                    if (ldvmodel.GetNetworks().Count() <= 1) return;
                }
                this.dragitem = value;
                DragDrop.DoDragDrop(TV_Main, dragitem, DragDropEffects.Move);
            }
        }

        private ProjectTreeViewItem currentitem = null;
        public ProjectTreeViewItem CurrentItem
        {
            get { return this.currentitem; }
            private set
            {
                //if (currentitem == value) return;
                if (currentitem != null)
                {
                    currentitem.Underline.Visibility = Visibility.Hidden;
                    currentitem.Background = Brushes.Transparent;
                }
                this.currentitem = value;
                if (currentitem != null && dragitem != null
                 && currentitem != dragitem)
                {
                    if ((dragitem.Flags & 0xf) == ProjectTreeViewItem.TYPE_NETWORK)
                    {
                        switch (currentitem.Flags & 0xf)
                        {
                            case ProjectTreeViewItem.TYPE_NETWORK:
                                currentitem.Underline.Visibility = Visibility.Visible;
                                break;
                            case ProjectTreeViewItem.TYPE_ROUTINE:
                                currentitem.Underline.Visibility = Visibility.Visible;
                                break;
                        }
                        return;
                    }
                    if (dragitem.IsAncestorOf(currentitem))
                        return;
                    foreach (ProjectTreeViewItem ptvitem in currentitem.Items)
                    {
                        if (ptvitem.Text.Equals(dragitem.Text))
                        {
                            return;
                        }
                    }
                    switch (dragitem.Flags & 0xf)
                    {
                        case ProjectTreeViewItem.TYPE_ROUTINE:
                        case ProjectTreeViewItem.TYPE_ROUTINEFLODER:
                            if ((currentitem.Flags & 0xf) != ProjectTreeViewItem.TYPE_ROUTINEFLODER)
                                return;
                            break;
                        case ProjectTreeViewItem.TYPE_FUNCBLOCK:
                        case ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER:
                            if ((currentitem.Flags & 0xf) != ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER)
                                return;
                            break;
                        default:
                            return;
                    }
                    currentitem.Background = Brushes.BlueViolet;
                }
            }
        }

        private ProjectTreeViewItem GetPTVIParent(object obj)
        {
            if (obj is FrameworkElement)
            {
                FrameworkElement fele = (FrameworkElement)obj;
                while (!(fele is ProjectTreeViewItem)
                    && (fele.Parent is FrameworkElement))
                {
                    fele = (FrameworkElement)(fele.Parent);
                }
                if (fele is ProjectTreeViewItem)
                {
                    return (ProjectTreeViewItem)fele;
                }
            }
            return null;
        }

        private ProjectTreeView GetThisParent(object obj)
        {
            if (obj is FrameworkElement)
            {
                FrameworkElement fele = (FrameworkElement)obj;
                while (fele != this 
                    && (fele.Parent is FrameworkElement))
                {
                    fele = (FrameworkElement)(fele.Parent);
                }
                if (fele == this)
                {
                    return this;
                }
            }
            return null;
        }

        private void OnPTVIMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                DragItem = null;
                return;
            }
            CurrentItem = GetPTVIParent(e.OriginalSource);
            if (CurrentItem == null) return;
            if (DragItem == null && TV_Main.SelectedItem != null
             && TV_Main.SelectedItem == CurrentItem)
            {
                DragItem = CurrentItem;
            }
        }

        private void OnPTVIDragOver(object sender, DragEventArgs e)
        {
            CurrentItem = GetPTVIParent(e.OriginalSource);
        }

        private void TV_Main_Drop(object sender, DragEventArgs e)
        {
            if (CurrentItem == null
             || CurrentItem.Background != Brushes.BlueViolet
             && CurrentItem.Underline.Visibility != Visibility.Visible)
            {
                DragItem = null;
                return;
            }
            if ((DragItem.Flags & 0xf) == ProjectTreeViewItem.TYPE_NETWORK)
            {
                LadderNetworkViewModel lnvmodel_old = (LadderNetworkViewModel)(DragItem.RelativeObject);
                LadderNetworkViewModel lnvmodel_new = null;
                LadderDiagramViewModel ldvmodel_old = lnvmodel_old.LDVModel;
                LadderDiagramViewModel ldvmodel_new = null;
                lnvmodel_old.ReleaseSelectRect();
                lnvmodel_old.IsSelectAreaMode = false;
                XElement xele = ProjectHelper.CreateXElementByLadderNetwork(lnvmodel_old);
                switch (CurrentItem.Flags & 0xf)
                {
                    case ProjectTreeViewItem.TYPE_ROUTINE:
                        ldvmodel_new = (LadderDiagramViewModel)(CurrentItem.RelativeObject);
                        lnvmodel_new = ProjectHelper.CreateLadderNetworkByXElement(xele, ldvmodel_new);
                        lnvmodel_new.NetworkNumber = 0;
                        break;
                    case ProjectTreeViewItem.TYPE_NETWORK:
                        LadderNetworkViewModel prev = (LadderNetworkViewModel)(CurrentItem.RelativeObject);
                        ldvmodel_new = prev.LDVModel;
                        lnvmodel_new = ProjectHelper.CreateLadderNetworkByXElement(xele, ldvmodel_new);
                        lnvmodel_new.NetworkNumber = prev.NetworkNumber + 1;
                        break;
                    default:
                        DragItem = null;
                        return;
                }
                ProjectTreeViewEventArgs _e = null;
                if (ldvmodel_new == ldvmodel_old)
                {
                    _e = new ProjectTreeViewEventArgs(
                        ProjectTreeViewEventArgs.TYPE_NETWORK |
                        ProjectTreeViewEventArgs.FLAG_REPLACE,
                        lnvmodel_new.NetworkNumber, lnvmodel_old);
                    PTVHandle(this, _e);
                }
                else
                {
                    _e = new ProjectTreeViewEventArgs(
                        ProjectTreeViewEventArgs.TYPE_NETWORK |
                        ProjectTreeViewEventArgs.FLAG_REMOVE,
                        lnvmodel_old, ldvmodel_old);
                    PTVHandle(this, _e);
                    _e = new ProjectTreeViewEventArgs(
                        ProjectTreeViewEventArgs.TYPE_NETWORK |
                        ProjectTreeViewEventArgs.FLAG_INSERT,
                        lnvmodel_new, ldvmodel_new);
                    PTVHandle(this, _e);
                }
            }
            else
            {
                DragItem.ReleasePath();
                ((ProjectTreeViewItem)(DragItem.Parent)).Items.Remove(DragItem);
                if (CurrentItem.IsOrder)
                {
                    for (int i = 0; i < CurrentItem.Items.Count; i++)
                    {
                        if (DragItem.CompareTo((ProjectTreeViewItem)(CurrentItem.Items[i])) <= 0)
                        {
                            CurrentItem.Items.Insert(i, DragItem);
                            break;
                        }
                    }
                }
                if (!CurrentItem.Items.Contains(DragItem))
                {
                    CurrentItem.Items.Add(DragItem);
                }
                CurrentItem.IsExpanded = true;
                DragItem.RegisterPath();
                DragItem.IsSelected = true;
            }
            DragItem = null;
            CurrentItem = CurrentItem;
        }

        private void TV_Main_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Source == TV_Main)
            {
                _projectModel.IFacade.MainWindow.LACProj.Hide();
            }
        }

        #endregion

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            XElement xele_sr = new XElement("PTVI_SubRoutines");
            Save(PTVI_SubRoutines, xele_sr);
            xele.Add(xele_sr);
            XElement xele_fb = new XElement("PTVI_FuncBlocks");
            Save(PTVI_FuncBlocks, xele_fb);
            xele.Add(xele_fb);
            foreach (KeyValuePair<string, ProjectTreeViewItem> kvs in dpdict)
            {
                XElement xele_dp = new XElement("Routine");
                xele_dp.SetAttributeValue("Name", kvs.Key);
                xele_dp.SetAttributeValue("Path", ((ProjectTreeViewItem)(kvs.Value.Parent)).Path);
                xele.Add(xele_dp);
            }
            foreach (KeyValuePair<string, ProjectTreeViewItem> kvs in fpdict)
            {
                XElement xele_fp = new XElement("FuncBlock");
                xele_fp.SetAttributeValue("Name", kvs.Key);
                xele_fp.SetAttributeValue("Path", ((ProjectTreeViewItem)(kvs.Value.Parent)).Path);
                xele.Add(xele_fp);
            }
        }
        
        public void Save(ProjectTreeViewItem ptvitem, XElement xele)
        {
            xele.SetAttributeValue("Text", 
                ptvitem.IsRenaming ? "# 正在被重命名" : ptvitem.Text);
            xele.SetAttributeValue("Flags", ptvitem.Flags);
            foreach (ProjectTreeViewItem _ptvitem in ptvitem.Items)
            {
                switch (_ptvitem.Flags & 0xf)
                {
                    case ProjectTreeViewItem.TYPE_ROUTINEFLODER:
                    case ProjectTreeViewItem.TYPE_NETWORKFLODER:
                    case ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER:
                    case ProjectTreeViewItem.TYPE_MODBUSFLODER:
                        XElement xele_i = new XElement("Item");
                        xele.Add(xele_i);
                        Save(_ptvitem, xele_i);
                        break;
                }
            }
        }
        
        public void Load(XElement xele)
        {
            XElement xele_sr = xele.Element("PTVI_SubRoutines");
            Load(PTVI_SubRoutines, xele_sr);
            XElement xele_fb = xele.Element("PTVI_FuncBlocks");
            Load(PTVI_FuncBlocks, xele_fb);
            dpdict.Clear();
            fpdict.Clear();
            foreach (XElement xele_dp in xele.Elements("Routine"))
            {
                string name = xele_dp.Attribute("Name").Value;
                string path = xele_dp.Attribute("Path").Value;
                ProjectTreeViewItem ptvitem = ProjectTreeViewItem.GetPTVIFromPath(path);
                if (ptvitem != null) dpdict.Add(name, ptvitem);
            }
            foreach (XElement xele_fp in xele.Elements("FuncBlock"))
            {
                string name = xele_fp.Attribute("Name").Value;
                string path = xele_fp.Attribute("Path").Value;
                ProjectTreeViewItem ptvitem = ProjectTreeViewItem.GetPTVIFromPath(path);
                if (ptvitem != null) fpdict.Add(name, ptvitem);
            }
        }
        
        public void Load(ProjectTreeViewItem ptvitem, XElement xele)
        {
            foreach (XElement xele_i in xele.Elements("Item"))
            {
                int flags = int.Parse(xele_i.Attribute("Flags").Value);
                string text = xele_i.Attribute("Text").Value;
                ProjectTreeViewItem _ptvitem = null;
                _ptvitem = CreatePTVItem(
                    ptvitem, flags, text, false, true);
                Load(_ptvitem, xele_i);
            }
        }

        #endregion
    }
    
　  public class ProjectTreeViewEventArgs : EventArgs
    {
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

        public const int FLAG_CREATE = 0x10;
        public const int FLAG_REPLACE = 0x20;
        public const int FLAG_REMOVE = 0x40;
        public const int FLAG_CREATEBEFORE = 0x80;
        public const int FLAG_CREATEAFTER = 0x100;
        public const int FLAG_CONFIG = 0x200;
        public const int FLAG_INSERT = 0x400;

        public int Flags { get; private set; }

        public object RelativeObject { get; set; }

        public object TargetedObject { get; set; }

        public ProjectTreeViewEventArgs
        (
            int _flags,
            object _relativeObject,
            object _targetedObject
        )
        {
            Flags = _flags;
            RelativeObject = _relativeObject;
            TargetedObject = _targetedObject;
        }
    }

    public delegate void ProjectTreeViewEventHandler(object sender, ProjectTreeViewEventArgs e);
}
