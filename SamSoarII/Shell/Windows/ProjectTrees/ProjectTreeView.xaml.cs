using SamSoarII.Core.Models;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

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

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl, IWindow
    {
        public ProjectTreeView(InteractionFacade _ifParent)
        {
            InitializeComponent();
            ProjectTreeViewItem.HasRenaming = false;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
        }
        
        #region Numbers

        /// <summary> 交互管理器 </summary>
        private InteractionFacade ifParent;
        /// <summary> 交互管理器 </summary>
        public InteractionFacade IFParent { get { return this.ifParent; } }
        /// <summary> 工程模型 </summary>
        public ProjectModel Project { get { return ifParent.MDProj; } }
        /// <summary> 要添加的树元素的父亲 </summary>
        private ProjectTreeViewItem newparent;

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
            Properties.Resources.Inst_Auxiliar, Properties.Resources.Inst_PID};

        #endregion

        #endregion

        #region Build

        public void ReinitializeComponent()
        {
            PTVI_Root = CreatePTVItem(
                null,
                ProjectTreeViewItem.TYPE_ROOT,
                Project);
            TV_Main.Items.Add(PTVI_Root);

            PTVI_Program = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_PROGRAM,
                Properties.Resources.Routine);

            PTVI_MainRoutine = CreatePTVItem(
                PTVI_Program,
                ProjectTreeViewItem.TYPE_ROUTINE
              | ProjectTreeViewItem.FLAG_CREATENETWORK,
                Project.MainDiagram);

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
                Project.LibFuncBlock);
            Rebuild(PTVI_LibFuncBlock, Project.LibFuncBlock);

            PTVI_Modbus = CreatePTVItem(
                PTVI_Root,
                ProjectTreeViewItem.TYPE_MODBUSFLODER
              | ProjectTreeViewItem.FLAG_CREATEMODBUS,
                Project.Modbus);

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
            PTVI_Ladders.IsExpanded = true;

            PTVI_Insts = new ProjectTreeViewItem[PTVIH_Insts.Length];
            for (int i = 0; i < PTVIH_Insts.Length; i++)
            {
                PTVI_Insts[i] = CreatePTVItem(
                    PTVI_Ladders,
                    ProjectTreeViewItem.TYPE_LADDERS,
                    PTVIH_Insts[i]);
            }
            for (int i = 0; i < (int)(LadderUnitModel.Types.VLINE); i++)
            {
                int id = (int)(LadderUnitModel.Formats[i].Outline);
                CreatePTVItem(PTVI_Insts[id], ProjectTreeViewItem.TYPE_INSTRUCTION, (LadderUnitModel.Types)i, false);
            }

            foreach (LadderDiagramModel ldmodel in Project.Diagrams)
            {
                if (ldmodel.IsMainLadder) continue;
                string path = ldmodel.Path;
                string next = null;
                ProjectTreeViewItem ptvitem = PTVI_SubRoutines;
                if (path != null)
                {
                    string name = PathMove(ref path);
                    name = PathMove(ref path);
                    name = PathMove(ref path);
                    name = PathMove(ref path);
                    next = PathMove(ref path);
                    while (next != null)
                    {
                        IEnumerable<ProjectTreeViewItem> fit = ptvitem.Items.Cast<ProjectTreeViewItem>();
                        fit = fit.Where((ptvi) => { return ptvi.Text.Equals(name); });
                        ptvitem = fit.Count() > 0 ? fit.First()
                            : CreatePTVItem(ptvitem,
                                ProjectTreeViewItem.TYPE_ROUTINEFLODER
                              | ProjectTreeViewItem.FLAG_CREATEFOLDER
                              | ProjectTreeViewItem.FLAG_CREATEROUTINE
                              | ProjectTreeViewItem.FLAG_REMOVE,
                                name, false, true);
                        name = next;
                        next = PathMove(ref path);
                    }
                }
                ldmodel.PTVItem = CreatePTVItem(ptvitem,
                            ProjectTreeViewItem.TYPE_ROUTINE
                          | ProjectTreeViewItem.FLAG_CREATENETWORK
                          | ProjectTreeViewItem.FLAG_RENAME
                          | ProjectTreeViewItem.FLAG_REMOVE,
                            ldmodel, false);
            }

            foreach (FuncBlockModel fbmodel in Project.FuncBlocks)
            {
                if (fbmodel.IsLibrary) continue;
                string path = fbmodel.Path;
                string next = null;
                ProjectTreeViewItem ptvitem = PTVI_FuncBlocks;
                if (path != null)
                {
                    string name = PathMove(ref path);
                    name = PathMove(ref path);
                    name = PathMove(ref path);
                    name = PathMove(ref path);
                    next = PathMove(ref path);
                    while (next != null)
                    {
                        IEnumerable<ProjectTreeViewItem> fit = ptvitem.Items.Cast<ProjectTreeViewItem>();
                        fit = fit.Where((ptvi) => { return ptvi.Text.Equals(name); });
                        ptvitem = fit.Count() > 0 ? fit.First()
                            : CreatePTVItem(ptvitem,
                                ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER
                              | ProjectTreeViewItem.FLAG_CREATEFOLDER
                              | ProjectTreeViewItem.FLAG_CREATEFUNCBLOCK
                              | ProjectTreeViewItem.FLAG_REMOVE,
                                name, false, true);
                        name = next;
                        next = PathMove(ref path);
                    }
                }
                fbmodel.PTVItem = CreatePTVItem(ptvitem,
                            ProjectTreeViewItem.TYPE_FUNCBLOCK
                          | ProjectTreeViewItem.FLAG_RENAME
                          | ProjectTreeViewItem.FLAG_REMOVE,
                            fbmodel, false);
            }

            PTVI_Root.IsExpanded = true;
            PTVI_Program.IsExpanded = true;
        }

        private string PathMove(ref string path)
        {
            if (path == null) return null;
            string name = null;
            int id = path.IndexOf('.');
            if (id == -1)
            {
                name = path;
                path = null;
            }
            else
            {
                name = path.Substring(0, id);
                path = path.Substring(id + 1);
            }
            return name;
        }

        private ProjectTreeViewItem CreatePTVItem
        (
            ProjectTreeViewItem parent,
            int flags,
            object relativeObject,
            bool iscritical = true,
            bool isorder = false
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
                    parent.Items.Add(createitem);
            }
            createitem.MouseWheel += OnPTVIMouseWheel;
            createitem.MouseDoubleClick += OnPTVIDoubleClick;
            createitem.MenuItemClick += ONPTVIMenuClick;
            createitem.Renamed += OnPTVIRenamed;
            createitem.Expanded += OnPTVIExpanded;
            createitem.IsCritical = iscritical;
            createitem.IsOrder = isorder;
            if (relativeObject is LadderDiagramModel)
            {
                LadderDiagramModel ldmodel = (LadderDiagramModel)relativeObject;
                Rebuild(createitem, ldmodel);
            }
            if (relativeObject is FuncBlockModel)
            {
                FuncBlockModel fbmodel = (FuncBlockModel)relativeObject;
                Rebuild(createitem, fbmodel);
                createitem.IsExpanded = false;
            }
            if (relativeObject is ModbusTableModel)
            {
                ModbusTableModel mtmodel = (ModbusTableModel)relativeObject;
                Rebuild(createitem, mtmodel);
            }
            return createitem;
        }
        
        private void Clear(ProjectTreeViewItem ptvitem)
        {
            foreach (ProjectTreeViewItem subitem in ptvitem.Items)
                Dispose(subitem);
            ptvitem.Items.Clear();
        }
        
        private void Rebuild(ProjectTreeViewItem ptvitem, LadderDiagramModel ldmodel)
        {
            ptvitem.RelativeObject = ldmodel;
            foreach (ProjectTreeViewItem subitem in ptvitem.Items.Cast<ProjectTreeViewItem>().ToArray())
            {
                LadderNetworkModel lnmodel = (LadderNetworkModel)(subitem.RelativeObject);
                if (lnmodel.Parent == null)
                {
                    Dispose(subitem);
                    ptvitem.Items.Remove(subitem);
                    lnmodel.PTVItem = null;
                }
            }
            ptvitem.Items.Clear();
            foreach (LadderNetworkModel lnmodel in ldmodel.Children)
            {
                if (lnmodel.PTVItem == null)
                    lnmodel.PTVItem = CreatePTVItem(
                        null,
                        ProjectTreeViewItem.TYPE_NETWORK
                      | ProjectTreeViewItem.FLAG_REMOVE
                      | ProjectTreeViewItem.FLAG_CREATENETWORKBEFORE
                      | ProjectTreeViewItem.FLAG_CREATENETWORKAFTER
                      | ProjectTreeViewItem.FLAG_CONFIG,
                        lnmodel, false);
                ptvitem.Items.Add(lnmodel.PTVItem);
            }
        }

        private void Rebuild(ProjectTreeViewItem ptvitem, FuncBlockModel fbmodel)
        {
            ptvitem.RelativeObject = fbmodel;
            Clear(ptvitem);
            IEnumerable<FuncModel> funcs = fbmodel.Funcs;
            if (funcs.Count() == 0)
            {
                CreatePTVItem(
                    ptvitem,
                    ProjectTreeViewItem.TYPE_CONST,
                    Properties.Resources.PTV_No_Function);
            }
            foreach (FuncModel fmodel in funcs)
            {
                CreatePTVItem(
                    ptvitem,
                    ProjectTreeViewItem.TYPE_FUNC,
                    fmodel, false);
            }
        }

        private void Rebuild(ProjectTreeViewItem ptvitem, ModbusTableModel mtvmodel)
        {
            ptvitem.RelativeObject = mtvmodel;
            Clear(ptvitem);
            foreach (ModbusModel mmodel in mtvmodel.Children)
            {
                CreatePTVItem(
                    ptvitem,
                    ProjectTreeViewItem.TYPE_MODBUS
                  | ProjectTreeViewItem.FLAG_RENAME
                  | ProjectTreeViewItem.FLAG_REMOVE,
                    mmodel, false);
            }
        }

        public void AddNewSubRoutines()
        {
            newparent = (TV_Main.SelectedItem is ProjectTreeViewItem ? (ProjectTreeViewItem)(TV_Main.SelectedItem) : PTVI_SubRoutines);
            if ((newparent.Flags & 0xf) != ProjectTreeViewItem.TYPE_ROUTINEFLODER)
                newparent = PTVI_SubRoutines;
            LadderDiagramModel ldmodel = new LadderDiagramModel(Project, "# New");
            Project.Diagrams.Add(ldmodel);
        }

        public void AddNewFuncBlock()
        {
            newparent = (TV_Main.SelectedItem is ProjectTreeViewItem ? (ProjectTreeViewItem)(TV_Main.SelectedItem) : PTVI_FuncBlocks);
            if ((newparent.Flags & 0xf) != ProjectTreeViewItem.TYPE_FUNCBLOCKFLODER)
                newparent = PTVI_FuncBlocks;
            FuncBlockModel fbmodel = new FuncBlockModel(Project, "# New", "");
            Project.FuncBlocks.Add(fbmodel);
        }

        public void AddNewModbus()
        {
            OnPTVIDoubleClick(PTVI_Modbus, null);
            ModbusTableModel mtmodel = Project.Modbus;
            mtmodel.View.InitializeDialog(ModbusTableViewModel.DIALOG_CREATE);
        }

        #endregion

        #region Destory
        
        public void Reset()
        {
            Dispose(PTVI_Root);
            TV_Main.Items.Clear();
        }

        private void Dispose(ProjectTreeViewItem ptvitem)
        {
            foreach (ProjectTreeViewItem subitem in ptvitem.Items)
            {
                Dispose(subitem);
            }
            ptvitem.RelativeObject = null;
            ptvitem.MouseWheel -= OnPTVIMouseWheel;
            ptvitem.MouseDoubleClick -= OnPTVIDoubleClick;
            ptvitem.MenuItemClick -= ONPTVIMenuClick;
            ptvitem.Renamed -= OnPTVIRenamed;
            ptvitem.Expanded -= OnPTVIExpanded;
        }

        private void Remove(ProjectTreeViewItem ptvitem)
        {
            LadderDiagramModel ldmodel = null;
            LadderNetworkModel lnmodel = null;
            FuncBlockModel fbmodel = null;
            ModbusModel mmodel = null;
            if (ptvitem.RelativeObject is LadderDiagramModel)
            {
                ldmodel = (LadderDiagramModel)(ptvitem.RelativeObject);
                Project.Diagrams.Remove(ldmodel);
            }
            else if (ptvitem.RelativeObject is FuncBlockModel)
            {
                fbmodel = (FuncBlockModel)(ptvitem.RelativeObject);
                Project.FuncBlocks.Remove(fbmodel);
            }
            else if (ptvitem.RelativeObject is ModbusModel)
            {
                mmodel = (ModbusModel)(ptvitem.RelativeObject);
                mmodel.Parent.Children.Remove(mmodel);
            }
            else if (ptvitem.RelativeObject is LadderNetworkModel)
            {
                lnmodel = (LadderNetworkModel)(ptvitem.RelativeObject);
                ldmodel = lnmodel.Parent;
                ldmodel.RemoveN(lnmodel.ID, lnmodel);
            }
            else
            {
                foreach (ProjectTreeViewItem subitem in ptvitem.Items.Cast<ProjectTreeViewItem>().ToArray())
                    Remove(subitem);
                Dispose(ptvitem);
                ProjectTreeViewItem ptvparent = (ProjectTreeViewItem)(ptvitem.Parent);
                ptvparent.Items.Remove(ptvitem);
            }
        }

        #endregion
        
        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
        }

        #region ProjectTreeViewItem
        
        public void Handle(ProjectTreeViewItem sender, NotifyCollectionChangedEventArgs e, int flags)
        {
            ProjectTreeViewItem ptvitem = null;
            ProjectTreeViewItem ptvparent = null;
            bool isnew = false;
            switch (flags)
            {
                case ProjectTreeViewItem.TYPE_LADDERS:
                    if (e.OldItems != null)
                        foreach (LadderDiagramModel ldmodel in e.OldItems)
                        {
                            ptvitem = ldmodel.PTVItem;
                            ptvparent = (ProjectTreeViewItem)(ptvitem.Parent);
                            ptvparent.Items.Remove(ptvitem);
                            Dispose(ptvitem);
                        }
                    if (e.NewItems != null)
                        foreach (LadderDiagramModel ldmodel in e.NewItems)
                        {
                            ptvparent = ldmodel.Name.Equals("# New") 
                                ? newparent : PTVI_SubRoutines;
                            if (ldmodel.Name.Equals("# New"))
                            {
                                isnew = true;
                                string newname = Properties.Resources.New_Routine;
                                int newnameid = 0;
                                while (Project.Diagrams.Where(d => d.Name.Equals(newname)).Count() > 0)
                                    newname = String.Format("{0:s}_{1:d}", Properties.Resources.New_Routine, newnameid++);
                                ldmodel.Name = newname;
                            }
                            ptvitem = CreatePTVItem(
                                ptvparent,
                                ProjectTreeViewItem.TYPE_ROUTINE
                              | ProjectTreeViewItem.FLAG_CREATENETWORK
                              | ProjectTreeViewItem.FLAG_RENAME
                              | ProjectTreeViewItem.FLAG_REMOVE,
                                ldmodel, false);
                            ptvparent.IsExpanded = true;
                        }
                    break;
                case ProjectTreeViewItem.TYPE_FUNCBLOCK:
                    if (e.OldItems != null)
                        foreach (FuncBlockModel fbmodel in e.OldItems)
                        {
                            ptvitem = fbmodel.PTVItem;
                            ptvparent = (ProjectTreeViewItem)(ptvitem.Parent);
                            ptvparent.Items.Remove(ptvitem);
                            Dispose(ptvitem);
                        }
                    if (e.NewItems != null)
                        foreach (FuncBlockModel fbmodel in e.NewItems)
                        {
                            ptvparent = fbmodel.Name.Equals("# New")
                                ? newparent : PTVI_FuncBlocks;
                            if (fbmodel.Name.Equals("# New"))
                            {
                                isnew = true;
                                string newname = Properties.Resources.New_FuncBlock;
                                int newnameid = 0;
                                while (Project.FuncBlocks.Where(d => d.Name.Equals(newname)).Count() > 0)
                                    newname = String.Format("{0:s}_{1:d}", Properties.Resources.New_FuncBlock, newnameid++);
                                fbmodel.Name = newname;
                            }
                            ptvitem = CreatePTVItem(
                                ptvparent,
                                ProjectTreeViewItem.TYPE_FUNCBLOCK
                              | ProjectTreeViewItem.FLAG_RENAME
                              | ProjectTreeViewItem.FLAG_REMOVE,
                                fbmodel, false);
                            ptvparent.IsExpanded = true;
                        }
                    break;
                case ProjectTreeViewItem.TYPE_NETWORK:
                    {
                        LadderDiagramModel ldmodel = (LadderDiagramModel)(sender.RelativeObject);
                        if (ldmodel.IsExecuting) break;
                        Rebuild(sender, ldmodel);
                    }
                    break;
                case ProjectTreeViewItem.TYPE_MODBUS:
                    Rebuild(sender, (ModbusTableModel)(sender.RelativeObject));
                    break;
            }
            if (isnew) ptvitem.Loaded += OnPTVILoaded;
        }
        
        private void OnPTVIDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
            ProjectTreeViewEventArgs _e = new ProjectTreeViewEventArgs(
                (ptvitem.Flags & 0xf) | ProjectTreeViewEventArgs.FLAG_DOUBLECLICK,
                ptvitem.RelativeObject, ptvitem);
            Post(this, _e);
        }

        private void ONPTVIMenuClick(object sender, RoutedEventArgs e)
        {
            ProjectMenuItem pmitem = (ProjectMenuItem)sender;
            ProjectTreeViewItem ptvitem = pmitem.PTVItem;
            ProjectTreeViewItem newitem = null;
            ProjectTreeViewEventArgs _e = null;
            LadderDiagramModel ldmodel = null;
            LadderNetworkModel lnmodel = null;
            FuncBlockModel fbmodel = null;
            ModbusTableModel mtmodel = null;
            ptvitem.IsExpanded = true;
            switch (pmitem.Flags)
            {
                case ProjectTreeViewItem.FLAG_CREATEFOLDER:
                    string newname = Properties.Resources.New_Folder;
                    int newnameid = 0;
                    while (ptvitem.Items.Cast<ProjectTreeViewItem>().Where(i => i.Text.Equals(newname)).Count() > 0)
                        newname = String.Format("{0:s}_{1:d}", Properties.Resources.New_Folder, newnameid++);
                    newitem = CreatePTVItem(
                        ptvitem,
                        ptvitem.Flags
                      | ProjectTreeViewItem.FLAG_RENAME
                      | ProjectTreeViewItem.FLAG_REMOVE,
                        newname, false, true);
                    ptvitem.IsExpanded = true;
                    newitem.Loaded += OnPTVILoaded;
                    break;
                case ProjectTreeViewItem.FLAG_CREATEROUTINE:
                    newparent = ptvitem;
                    ldmodel = new LadderDiagramModel(Project, "# New");
                    Project.Diagrams.Add(ldmodel);
                    break;
                case ProjectTreeViewItem.FLAG_CREATEFUNCBLOCK:
                    newparent = ptvitem;
                    fbmodel = new FuncBlockModel(Project, "# New", "");
                    Project.FuncBlocks.Add(fbmodel);
                    break;
                case ProjectTreeViewItem.FLAG_CREATENETWORK:
                    ldmodel = (LadderDiagramModel)(ptvitem.RelativeObject);
                    ldmodel.AddN(ldmodel.NetworkCount);
                    break;
                case ProjectTreeViewItem.FLAG_CREATENETWORKBEFORE:
                    lnmodel = (LadderNetworkModel)(ptvitem.RelativeObject);
                    ldmodel = lnmodel.Parent;
                    ldmodel.AddN(lnmodel.ID);
                    break;
                case ProjectTreeViewItem.FLAG_CREATENETWORKAFTER:
                    lnmodel = (LadderNetworkModel)(ptvitem.RelativeObject);
                    ldmodel = lnmodel.Parent;
                    ldmodel.AddN(lnmodel.ID + 1);
                    break;
                case ProjectTreeViewItem.FLAG_CREATEMODBUS:
                    OnPTVIDoubleClick(ptvitem, null);
                    mtmodel = (ModbusTableModel)(ptvitem.RelativeObject);
                    mtmodel.View.InitializeDialog(ModbusTableViewModel.DIALOG_CREATE);
                    break;
                case ProjectTreeViewItem.FLAG_RENAME:
                    ptvitem.Rename();
                    break;
                case ProjectTreeViewItem.FLAG_CONFIG:
                    _e = new ProjectTreeViewEventArgs(
                        (ptvitem.Flags & 0xf) | ProjectTreeViewEventArgs.FLAG_CONFIG,
                        ptvitem.RelativeObject, ptvitem);
                    Post(this, _e);
                    break;
                case ProjectTreeViewItem.FLAG_REMOVE:
                    if (ptvitem.RelativeObject is LadderNetworkModel)
                    {
                        Remove(ptvitem);
                        break;
                    }
                    LocalizedMessageResult result = LocalizedMessageBox.Show(
                            Properties.Resources.Message_Delete, Properties.Resources.Important, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Warning);
                    if (result == LocalizedMessageResult.Yes)
                    {
                        Remove(ptvitem);
                    }
                    break;
            }
        }
        
        private void OnPTVILoaded(object sender, RoutedEventArgs e)
        {
            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
            ptvitem.Loaded -= OnPTVILoaded;
            ptvitem.Rename();
        }

        private void OnPTVIRenamed(object sender, RoutedEventArgs e)
        {
            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
            ProjectTreeViewItem ptvparent = (ProjectTreeViewItem)(ptvitem.Parent);
            if (ptvitem.Text.Equals(String.Empty))
            {
                ptvitem.Rename(Properties.Resources.Message_Name_Needed);
                return;
            }
            for (int i = 0; i < ptvitem.Text.Length; i++)
            {
                switch (ptvitem.Text[i])
                {
                    case '#':
                    case '@':
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
            if (ptvitem.RelativeObject is LadderDiagramModel)
            {
                LadderDiagramModel ldmodel = (LadderDiagramModel)(ptvitem.RelativeObject);
                foreach (LadderDiagramModel _ldmodel in ldmodel.Parent.Diagrams)
                {
                    if (_ldmodel != ldmodel && _ldmodel.Name.Equals(ptvitem.Text))
                    {
                        ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Subroutine_Exist, ptvitem.Text));
                        return;
                    }
                }
                ptvitem.RenameClose();
                ldmodel.Name = ptvitem.Text;
            }
            else if (ptvitem.RelativeObject is FuncBlockModel)
            {
                FuncBlockModel fbmodel = (FuncBlockModel)(ptvitem.RelativeObject);
                foreach (FuncBlockModel _fbmodel in fbmodel.Parent.FuncBlocks)
                {
                    if (_fbmodel != fbmodel && _fbmodel.Name.Equals(ptvitem.Text))
                    {
                        ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Funcblock_Exist, ptvitem.Text));
                        return;
                    }
                }
                ptvitem.RenameClose();
                fbmodel.Name = ptvitem.Text;
            }
            else if (ptvitem.RelativeObject is ModbusModel)
            {
                ModbusModel mmodel = (ModbusModel)(ptvitem.RelativeObject);
                foreach (ModbusModel _mmodel in mmodel.Parent.Children)
                {
                    if (_mmodel != mmodel && _mmodel.Name.Equals(ptvitem.Text))
                    {
                        ptvitem.Rename(String.Format("{0} {1:s}。", Properties.Resources.Message_Funcblock_Exist, ptvitem.Text));
                        return;
                    }
                }
                ptvitem.RenameClose();
                mmodel.Name = ptvitem.Text;
            }
            else
            {
                ptvitem.RenameClose();
            }
        }

        private void OnPTVIExpanded(object sender, RoutedEventArgs e)
        {
            ProjectTreeViewItem ptvitem = (ProjectTreeViewItem)sender;
            if (ptvitem.RelativeObject is FuncBlockModel)
            {
                Rebuild(ptvitem, (FuncBlockModel)(ptvitem.RelativeObject));
            }
        }

        #endregion

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
                if (ifParent.VMDProj.LadderMode != LadderModes.Edit) return;
                if (value.IsCritical) return;
                if (value.IsRenaming) return;
                if ((value.Flags & 0xf) == ProjectTreeViewItem.TYPE_NETWORK)
                {
                    LadderNetworkModel lnmodel = (LadderNetworkModel)(value.RelativeObject);
                    LadderDiagramModel ldmodel = lnmodel.Parent;
                    if (ldmodel.Children.Count() <= 1) return;
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
            var p = e.GetPosition(Scroll);
            if (Scroll.ViewportHeight < p.Y)
                Scroll.ScrollToVerticalOffset(Scroll.VerticalOffset + 10.0);
            else if (p.Y < 10)
                Scroll.ScrollToVerticalOffset(Scroll.VerticalOffset - 10.0);
            else if (p.X < 10)
                Scroll.ScrollToHorizontalOffset(Scroll.HorizontalOffset - 10.0);
            else if (Scroll.ViewportWidth < p.X)
            {
                Scroll.ScrollToHorizontalOffset(Scroll.HorizontalOffset + 10.0);
                //Scroll.ScrollToVerticalOffset(TV_Main.ActualHeight * (p.Y - Scroll.VerticalOffset) / Scroll.ViewportHeight);
            }
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
                LadderNetworkModel lnmodel_old = (LadderNetworkModel)(DragItem.RelativeObject);
                LadderNetworkModel lnmodel_new = null;
                LadderDiagramModel ldmodel_old = lnmodel_old.Parent;
                LadderDiagramModel ldmodel_new = null;
                int netid_old = lnmodel_old.ID;
                int netid_new = 0;
                if (lnmodel_old.View != null)
                    lnmodel_old.View.ReleaseSelectRect();
                //XElement xele = new XElement("Network");
                //lnmodel_old.Save(xele);
                switch (CurrentItem.Flags & 0xf)
                {
                    case ProjectTreeViewItem.TYPE_ROUTINE:
                        ldmodel_new = (LadderDiagramModel)(CurrentItem.RelativeObject);
                        netid_new = 0;
                        break;
                    case ProjectTreeViewItem.TYPE_NETWORK:
                        LadderNetworkModel prev = (LadderNetworkModel)(CurrentItem.RelativeObject);
                        ldmodel_new = prev.Parent;
                        netid_new = prev.ID + 1;
                        break;
                    default:
                        DragItem = null;
                        return;
                }
                if (ldmodel_new != null)
                {
                    ProjectTreeViewEventArgs _e = null;
                    if (ldmodel_new == ldmodel_old)
                    {
                        lnmodel_new = lnmodel_old;
                        ldmodel_old.ReplaceN(
                            new LadderNetworkModel[] { lnmodel_old },
                            new LadderNetworkModel[] { lnmodel_new },
                            new int[] { netid_new - (netid_new > netid_old ? 1 : 0) });
                    }
                    else
                    {
                        lnmodel_new = new LadderNetworkModel(ldmodel_new, netid_new);
                        XElement xele = new XElement("Network");
                        lnmodel_old.Save(xele);
                        lnmodel_new.Load(xele);
                        lnmodel_new.ID = netid_new;
                        ldmodel_old.RemoveN(lnmodel_old.ID, lnmodel_old);
                        ldmodel_new.AddN(lnmodel_new.ID, lnmodel_new);
                    }
                }
            }
            else
            {
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
                DragItem.IsSelected = true;
            }
            DragItem = null;
            CurrentItem = CurrentItem;
        }

        private void TV_Main_DragLeave(object sender, DragEventArgs e)
        {
            CurrentItem = null;
            if ((DragItem.Flags & 0xf) == ProjectTreeViewItem.TYPE_INSTRUCTION)
                ifParent.WNDMain.LACProj.Hide();
        }
        
        private void OnPTVIMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Scroll.ScrollToVerticalOffset(Scroll.VerticalOffset - e.Delta * 2);
        }

        #endregion

        #endregion

    }
}
