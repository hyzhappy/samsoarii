using SamSoarII.PLCDevice;
using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.IO;
using SamSoarII.Utility;
using SamSoarII.Shell.Windows;
using System.Windows;

namespace SamSoarII.Core.Models
{
    public class ProjectModel : IModel
    {
        public ProjectModel(InteractionFacade _parent, string _projname, string _filename = null)
        {
            parent = _parent;
            projname = _projname;
            filename = _filename;
            diagrams = new ObservableCollection<LadderDiagramModel>();
            funcblocks = new ObservableCollection<FuncBlockModel>();
            diagrams.CollectionChanged += OnDiagramCollectionChanged;
            funcblocks.CollectionChanged += OnFuncBlockCollectionChanged;
            StreamReader sr = new StreamReader(String.Format(@"{0:s}\simug\simuflib.c", FileHelper.AppRootPath));
            libfuncblock = new FuncBlockModel(this, SamSoarII.Properties.Resources.Library_Function, sr.ReadToEnd(), true);
            funcblocks.Add(libfuncblock);
            if (filename != null)
            {
                XDocument xdoc = XDocument.Load(filename);
                XElement xele_r = xdoc.Element("Root");
                XElement xele_p = xele_r.Element("Project");
                Load(xele_p);
            }
            else
            {
                device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
                maindiagram = new LadderDiagramModel(this, "Main");
                maindiagram.IsMainLadder = true;
                diagrams.Add(maindiagram);
                modbus = new ModbusTableModel(this);
                monitor = new MonitorModel(this);
                paraProj = new ProjectPropertyParams(this);
            }
            ismodified = false;
        }

        public void Dispose()
        {
            foreach (LadderDiagramModel diagram in diagrams)
            {
                diagram.PropertyChanged -= OnDiagramPropertyChanged;
                diagram.Dispose();
            }
            diagrams.CollectionChanged -= OnDiagramCollectionChanged;
            diagrams.Clear();
            diagrams = null;
            maindiagram = null;
            foreach (FuncBlockModel funcblock in funcblocks)
            {
                funcblock.PropertyChanged -= OnFuncBlockPropertyChanged;
                funcblock.Dispose();
            }
            funcblocks.CollectionChanged -= OnFuncBlockCollectionChanged;
            funcblocks.Clear();
            funcblocks = null;
            modbus.Dispose();
            modbus = null;
            paraProj.Dispose();
            paraProj = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Numbers

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return null; } }
        public ValueManager ValueManager { get { return parent.MNGValue; } }
        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; }
        }
        private string projname;
        public string ProjName { get { return this.projname; } }

        private string filename;
        public string FileName { get { return this.filename; } }

        private Device device;
        public Device Device
        {
            get
            {
                return this.device;
            }
            set
            {
                this.device = value;
                ValueManager.RemoveInvalidValues();
                foreach (LadderDiagramModel diagram in Diagrams)
                    diagram.ClearUndoRedoAction();
                PropertyChanged(this, new PropertyChangedEventArgs("Device"));
            }
        }

        private bool ismodified;
        public bool IsModified
        {
            get { return this.ismodified; }
            private set
            {
                if (ismodified == value) return;
                this.ismodified = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsModified"));
            }
        }

        public event RoutedEventHandler Modified = delegate { };
        public void InvokeModify(IModel source, bool undo = false)
        {
            if (!isLoaded) return;
            if (source is LadderDiagramModel)
            {
                LadderDiagramModel diagram = (LadderDiagramModel)source;
                undodiagram = undo ? diagram : null;
                redodiagram = undo ? null : diagram;
            }
            IsModified = true;
            Modified(source, new RoutedEventArgs());
        }
        public void InvokeModify(IParams source)
        {
            IsModified = true;
            Modified(source, new RoutedEventArgs());
        }
        public void InvokeModify(IWindow window)
        {
            IsModified = true;
            Modified(window, new RoutedEventArgs());
        }
        public void ChangeModify(bool isModify)
        {
            ismodified = isModify;
        }
        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get
            {
                return this.laddermode;
            }
            set
            {
                this.laddermode = value;
                foreach (LadderDiagramModel diagram in Diagrams)
                    diagram.LadderMode = laddermode;
                foreach (FuncBlockModel funcblock in FuncBlocks)
                    funcblock.LadderMode = laddermode;
                PropertyChanged(this, new PropertyChangedEventArgs("LadderMode"));
            }
        }
        
        #region Models

        private LadderDiagramModel maindiagram;
        public LadderDiagramModel MainDiagram { get { return this.maindiagram; } }

        private LadderDiagramModel undodiagram;
        public LadderDiagramModel UndoDiagram { get { return this.undodiagram; } }

        private LadderDiagramModel redodiagram;
        public LadderDiagramModel RedoDiagram { get { return this.redodiagram; } }

        private ObservableCollection<LadderDiagramModel> diagrams;
        public IList<LadderDiagramModel> Diagrams { get { return this.diagrams; } }

        private FuncBlockModel libfuncblock;
        public FuncBlockModel LibFuncBlock { get { return this.libfuncblock; } }

        private ObservableCollection<FuncBlockModel> funcblocks;
        public IList<FuncBlockModel> FuncBlocks { get { return this.funcblocks; } }
        public IEnumerable<FuncModel> Funcs
        {
            get
            {
                foreach (FuncBlockModel fbmodel in FuncBlocks)
                    foreach (FuncModel fmodel in fbmodel.Funcs)
                        yield return fmodel;
            }
        }

        private ModbusTableModel modbus;
        public ModbusTableModel Modbus { get { return this.modbus; } }

        private MonitorModel monitor;
        public MonitorModel Monitor { get { return this.monitor; } }

        private ProjectPropertyParams paraProj;
        public ProjectPropertyParams PARAProj { get { return this.paraProj; } }

        #endregion

        #endregion
        
        #region View

        private ProjectViewModel view;
        public ProjectViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view != null) view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (ProjectViewModel)value; }
        }

        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion
        
        #region Save & Load

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Name", projname);
            xele.SetAttributeValue("DeviceType", Device.Type);
            foreach (LadderDiagramModel diagram in diagrams)
            {
                XElement xele_d = new XElement("Ladder");
                diagram.Save(xele_d);
                xele.Add(xele_d);
            }
            foreach (FuncBlockModel funcblock in funcblocks)
            {
                if (funcblock.IsLibrary) continue;
                XElement xele_f = new XElement("FuncBlock");
                funcblock.Save(xele_f);
                xele.Add(xele_f);
            }
            XElement xele_m = new XElement("Modbus");
            modbus.Save(xele_m);
            xele.Add(xele_m);
            XElement xele_pp = new XElement("ProjectPropertyParams");
            paraProj.Save(xele_pp);
            xele.Add(xele_pp);
            XElement xele_vm = new XElement("ValueManager");
            ValueManager.Save(xele_vm);
            xele.Add(xele_vm);
            XElement xele_mn = new XElement("Monitor");
            monitor.Save(xele_mn);
            xele.Add(xele_mn);
            IsModified = false;
        }

        public void Save(string _filename)
        {
            if (_filename == null) return;
            filename = _filename;
            projname = FileHelper.GetFileName(_filename);
            XDocument xdoc = new XDocument();
            XElement xele_r = new XElement("Root");
            XElement xele_p = new XElement("Project");
            xdoc.Add(xele_r);
            xele_r.Add(xele_p);
            Save(xele_p);
            if (!Directory.Exists(Directory.GetParent(_filename).FullName))
            {
                Directory.CreateDirectory(Directory.GetParent(_filename).FullName);
            }
            if (!File.Exists(_filename))
            {
                xdoc.Save(File.Create(_filename));
            }
            else
            {
                xdoc.Save(filename);
            }
        }

        public void Save()
        {
            Save(filename);
        }

        public void Load(XElement xele)
        {
            projname = xele.Attribute("Name").Value;
            PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType((PLCDeviceType)Enum.Parse(typeof(PLCDeviceType), xele.Attribute("DeviceType").Value));
            device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            foreach (XElement xele_f in xele.Elements("FuncBlock"))
            {
                FuncBlockModel funcblock = new FuncBlockModel(this, xele_f.Attribute("Name").Value, xele_f.Value);
                FuncBlocks.Add(funcblock);
            }
            foreach (XElement xele_d in xele.Elements("Ladder"))
            {
                LadderDiagramModel diagram = new LadderDiagramModel(this);
                diagram.Load(xele_d);
                Diagrams.Add(diagram);
                if (diagram.IsMainLadder) maindiagram = diagram;
            }
            modbus = new ModbusTableModel(this);
            modbus.Load(xele.Element("Modbus"));
            monitor = new MonitorModel(this);
            monitor.Load(xele.Element("Monitor"));
            ValueManager.Load(xele.Element("ValueManager"));
            paraProj = new ProjectPropertyParams(this);
            paraProj.Load(xele.Element("ProjectPropertyParams"));
        }

        #endregion

        #region Event Handler

        public event NotifyCollectionChangedEventHandler DiagramChanged = delegate { };

        private void OnDiagramCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (LadderDiagramModel ldmodel in e.OldItems)
                {
                    ldmodel.PropertyChanged -= OnDiagramPropertyChanged;
                    ldmodel.Parent = null;
                }
            if (e.NewItems != null)
                foreach (LadderDiagramModel ldmodel in e.NewItems)
                {
                    ldmodel.PropertyChanged += OnDiagramPropertyChanged;
                    ldmodel.Parent = this;
                }
            InvokeModify(this);
            DiagramChanged(this, e);
        }
        private void OnDiagramPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LadderMode") return;
            InvokeModify(this);
        }

        public event NotifyCollectionChangedEventHandler FuncBlockChanged = delegate { };

        private void OnFuncBlockCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (FuncBlockModel fbmodel in e.OldItems)
                    fbmodel.PropertyChanged -= OnFuncBlockPropertyChanged;
            if (e.NewItems != null)
                foreach (FuncBlockModel fbmodel in e.NewItems)
                    fbmodel.PropertyChanged += OnFuncBlockPropertyChanged;
            InvokeModify(this);
            FuncBlockChanged(this, e);
        }
        private void OnFuncBlockPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LadderMode") return;
            InvokeModify(this);
        }

        #endregion

    }
}
