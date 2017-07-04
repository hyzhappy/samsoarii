using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.ComponentModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SamSoarII.AppMain.UI;
using System.Windows.Controls;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;
using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Communication;
using System.Windows.Media;
using System.Threading;
using SamSoarII.Utility;

namespace SamSoarII.AppMain.Project
{
    public enum ChangeType
    {
        Add,
        Remove,
        Modify,
        Clear
    }
    public class ProjectModel : INotifyPropertyChanged, IDisposable
    {
        public bool IsModify
        {
            get
            {
                bool ret = false;
                ret |= MainRoutine.IsModify;
                foreach (LadderDiagramViewModel ldvmodel in SubRoutines)
                {
                    ret |= ldvmodel.IsModify;
                }
                foreach (var funcblock in FuncBlocks)
                {
                    ret |= funcblock.IsModify;
                }
                ret |= MTVModel.IsModify;
                ret |= ProjectPropertyManager.IsModify;
                return ret;
            }
            set
            {
                MainRoutine.IsModify = value;
                foreach (LadderDiagramViewModel ldvmodel in SubRoutines)
                {
                    ldvmodel.IsModify = value;
                }
                foreach (var funcblock in FuncBlocks)
                {
                    funcblock.IsModify = value;
                }
                ProjectPropertyManager.IsModify = value;
                MTVModel.IsModify = value;
                if (value)
                {
                    OnPropertyChanged("ProjectModel");
                }
            }
        }

        private LadderMode _laddermode = LadderMode.Edit;
        public LadderMode LadderMode
        {
            get
            {
                return this._laddermode;
            }
            set
            {
                if (_laddermode != value)
                {
                    this._laddermode = value;
                    MainRoutine.LadderMode = value;
                    foreach (LadderDiagramViewModel ldvmodel in SubRoutines)
                    {
                        ldvmodel.LadderMode = value;
                    }
                    foreach (FuncBlockViewModel fbvmodel in FuncBlocks)
                    {
                        fbvmodel.IsReadOnly = (_laddermode != LadderMode.Edit);
                    }
                }
                switch (value)
                {
                    case LadderMode.Edit:
                        IFacade.MainWindow.Main_SB.Background = Brushes.AliceBlue;
                        IFacade.MainWindow.SB_FontColor = Brushes.Black;
                        break;
                    case LadderMode.Monitor:
                        IFacade.MainWindow.Main_SB.Background = LadderHelper.MonitorBrush;
                        IFacade.MainWindow.SB_FontColor = Brushes.White;
                        break;
                    case LadderMode.Simulate:
                        IFacade.MainWindow.Main_SB.Background = LadderHelper.SimulateBrush;
                        IFacade.MainWindow.SB_FontColor = Brushes.White;
                        break;
                    default:
                        break;
                }
            }
        }

        private bool _isCommentMode;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged(string messsage)
        {
            PropertyChanged(this,new PropertyChangedEventArgs(messsage));
        }

        public AutoSavedManager autoSavedManager { get; set; }
        public AutoInstManager AutoInstManager { get; set; }
        public bool IsCommentMode
        {
            get { return _isCommentMode; }
            set
            {
                _isCommentMode = value;
                MainRoutine.IsCommentMode = _isCommentMode;
                foreach (var ldmodel in SubRoutines)
                {
                    ldmodel.IsCommentMode = _isCommentMode;
                }
            }
        }
        public string ProjectName { get; set; }
        public InteractionFacade IFacade { get; set; }
        public LadderDiagramViewModel MainRoutine { get; set; }
        public ObservableCollection<LadderDiagramViewModel> SubRoutines { get; set; } = new ObservableCollection<LadderDiagramViewModel>();
        //public Dictionary<LadderDiagramViewModel, ObservableCollection<string>> RefNetworksBrief { get; set; } = new Dictionary<LadderDiagramViewModel, ObservableCollection<string>>();
        public FuncBlockViewModel LibFuncBlock { get; set; }
        public ObservableCollection<FuncBlockViewModel> FuncBlocks { get; set; } = new ObservableCollection<FuncBlockViewModel>();
        public ModbusTableViewModel MTVModel { get; set; }
        public MonitorManager MMonitorManager { get; private set; }
        public SerialPortManager PManager { get; private set; }
        public USBManager UManager { get; private set; } 
        public Device CurrentDevice
        {
            get { return PLCDeviceManager.GetPLCDeviceManager().SelectDevice; }
            set
            {
                PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(value.Type);
            }
        }
        public XElement EleInitializeData { get; set; }
        public IEnumerable<FuncModel> Funcs
        {
            get
            {
                List<FuncModel> result = new List<FuncModel>();
                result.AddRange(LibFuncBlock.Funcs);
                foreach (FuncBlockViewModel fbvmodel in FuncBlocks)
                {
                    result.AddRange(fbvmodel.Funcs);
                }
                return result;
            }
        }
        public ProjectModel()
        {

        }
        public ProjectModel(string projectname)
        {
            ProjectName = projectname;
            MainRoutine = new LadderDiagramViewModel("Main", this);
            MainRoutine.IsMainLadder = true;
            MMonitorManager = new MonitorManager(this);
            MTVModel = new ModbusTableViewModel(this);
            MMonitorManager.MMWindow.Manager = MMonitorManager;
            PManager = new SerialPortManager();
            UManager = new USBManager();
            StreamReader sr = new StreamReader(
                String.Format(@"{0:s}\simug\simuflib.c", FileHelper.AppRootPath));
            FuncBlockViewModel libfuncblock = new FuncBlockViewModel(Properties.Resources.Library_Function, this);
            libfuncblock.Code = sr.ReadToEnd();
            libfuncblock.IsReadOnly = true;
            LibFuncBlock = libfuncblock;
        }
        public void Dispose()
        {
            if (autoSavedManager != null)
            {
                autoSavedManager.Abort();
            }
            if (AutoInstManager != null
             && AutoInstManager.IsAlive)
            {
                AutoInstManager.Abort();
            }
            MainRoutine.Dispose();
            MainRoutine = null;
            foreach (var subRoutine in SubRoutines)
                subRoutine.Dispose();
            SubRoutines.Clear();
            MMonitorManager.Dispose();
            MTVModel.Dispose();
            MMonitorManager = null;
            //MTVModel = null;
            PManager = null;
            UManager = null;
            LibFuncBlock.Dispose();
            LibFuncBlock = null;
            foreach (var funcBlock in FuncBlocks)
            {
                funcBlock.Dispose();
            }
            FuncBlocks.Clear();
        }
        public void Add(LadderDiagramViewModel ldmodel)
        {
            if (!SubRoutines.Contains(ldmodel))
            {
                SubRoutines.Add(ldmodel);
                IsModify = true;
            }
        }
        public void Add(FuncBlockViewModel fbmodel)
        {
            if (!FuncBlocks.Contains(fbmodel))
            {
                FuncBlocks.Add(fbmodel);
                IsModify = true;
            }
        }
        public void Remove(LadderDiagramViewModel ldmodel)
        {
            if (SubRoutines.Contains(ldmodel))
            {
                SubRoutines.Remove(ldmodel);
                IsModify = true;
            }
        }
        public void Remove(FuncBlockViewModel fbmodel)
        {
            if (FuncBlocks.Contains(fbmodel))
            {
                FuncBlocks.Remove(fbmodel);
                IsModify = true;
            }
        }
        /// <summary>
        /// Save the ProjectModel to a xml format file
        /// </summary>
        /// <param name="filepath"></param>
        public void Save(XElement rootNode)
        {
            rootNode.SetAttributeValue("Name", ProjectName);
            rootNode.SetAttributeValue("DeviceType", CurrentDevice.Type);
            var settingNode = new XElement("Setting");
            rootNode.Add(settingNode);
            rootNode.Add(ProjectHelper.CreateXElementByValueComments());
            rootNode.Add(ProjectHelper.CreateXElementByValueAlias());
            rootNode.Add(ProjectPropertyManager.CreateProjectPropertyXElement());
            rootNode.Add(ProjectHelper.CreateXElementByLadderDiagram(MainRoutine));
            rootNode.Add(MMonitorManager.MMWindow.CreateXElementByTables());
            rootNode.Add(IFacade.MainWindow.ElemInitWind.CreatXElementByElements());
            foreach (var ldmodel in SubRoutines)
            {
                if (ldmodel.ProgramName.Length == 0) continue;
                rootNode.Add(ProjectHelper.CreateXElementByLadderDiagram(ldmodel));
            }
            foreach (var fbmodel in FuncBlocks)
            {
                if (fbmodel.ProgramName.Length == 0) continue;
                rootNode.Add(ProjectHelper.CreateXElementByFuncBlock(fbmodel));
            }
            var mtnode = new XElement("Modbus");
            MTVModel.Save(mtnode);
            rootNode.Add(mtnode);
        }
        public bool Open(XElement rootNode)
        {
            ProjectName = rootNode.Attribute("Name").Value;
            PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType((PLCDeviceType)Enum.Parse(typeof(PLCDeviceType), rootNode.Attribute("DeviceType").Value));
            SubRoutines.Clear();
            FuncBlocks.Clear();
            ValueAliasManager.Clear();
            ValueCommentManager.Clear();
            InstructionCommentManager.Clear();
            ProjectHelper.LoadValueCommentsByXElement(rootNode.Element("ValueComments"));
            ProjectHelper.LoadValueAliasByXElement(rootNode.Element("ValueAlias"));
            ProjectPropertyManager.LoadProjectPropertyByXElement(rootNode.Element("ProjectPropertyParams"));
            MMonitorManager.MMWindow.LoadTablesByXElement(rootNode.Element("Tables"));
            EleInitializeData = rootNode.Element("EleInitialize");
            var ldnodes = rootNode.Elements("Ladder");
            foreach (XElement ldnode in ldnodes)
            {
                var ldmodel = ProjectHelper.CreateLadderDiagramByXElement(ldnode, this);
                if (ldmodel.IsMainLadder)
                {
                    MainRoutine = ldmodel;
                }
                else
                {
                    SubRoutines.Add(ldmodel);
                    TreeViewItem item = new TreeViewItem();
                    item.Header = ldmodel;
                }
            }
            // Open FunctionBlock
            var fbnodes = rootNode.Elements("FuncBlock");
            foreach (XElement fbnode in fbnodes)
            {
                var fbmodel = ProjectHelper.CreateFuncBlockByXElement(fbnode, this);
                FuncBlocks.Add(fbmodel);
            }
            var mtnodes = rootNode.Element("Modbus");
            var mtmodel = new ModbusTableViewModel(this);
            mtmodel.Load(mtnodes);
            MTVModel = mtmodel;
            return true;
        }
        
        public LadderNetworkViewModel GetNetwork(BaseViewModel bvmodel)
        {
            if (LadderMode != LadderMode.Simulate)
                return null;
            LadderNetworkViewModel lnvmodel = null;
            lnvmodel = GetNetwork(bvmodel, MainRoutine);
            if (lnvmodel != null)
                return lnvmodel;
            foreach (LadderDiagramViewModel ldvmodel in SubRoutines)
            {
                lnvmodel = GetNetwork(bvmodel, ldvmodel);
                if (lnvmodel != null)
                    return lnvmodel;
            }
            return null;
        }

        public LadderNetworkViewModel GetNetwork(BaseViewModel bvmodel, LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                if (lnvmodel.ContainBPAddr(bvmodel.BPAddress))
                {
                    return lnvmodel;
                }
            }
            return null;
        }
        
        public FuncBlockViewModel GetFuncBlock(FuncBlock fblock)
        {
            if (LibFuncBlock.Model == fblock.Model)
                return LibFuncBlock;
            foreach (FuncBlockViewModel fbvmodel in FuncBlocks)
            {
                if (fbvmodel.Model == fblock.Model)
                    return fbvmodel;
            }
            return null;
        }

    }
}

