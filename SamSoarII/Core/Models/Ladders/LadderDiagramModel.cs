using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Global;

namespace SamSoarII.Core.Models
{
    public class LadderDiagramModel : IModel
    {
        public LadderDiagramModel(ProjectModel _parent, string _name = "")
        {
            children = new ObservableCollection<LadderNetworkModel>();
            children.CollectionChanged += Children_CollectionChanged;
            LadderNetworkModel lnmodel = new LadderNetworkModel(this, 0);
            children.Add(lnmodel);
            Name = _name;
            Inst = new InstructionDiagramModel(this);
            IsExpand = true;
            Parent = _parent;
        }
        
        public void Dispose()
        {
            Parent = null;
            foreach (LadderNetworkModel network in children)
            {
                network.PropertyChanged -= OnChildrenPropertyChanged;
                network.Dispose();
            }
            children.CollectionChanged -= Children_CollectionChanged;
            children.Clear();
            children = null;
            inst.Dispose();
            inst = null;
            undos.Clear();
            redos.Clear();
            undos = null;
            redos = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (ValueManager != null) ValueManager.Remove(this);
                this.parent = value;
                if (ValueManager != null) ValueManager.Add(this);
            }
        }
        IModel IModel.Parent { get { return Parent; } }
        public InteractionFacade IFParent { get { return parent?.Parent; } }
        public ValueManager ValueManager { get { return IFParent?.MNGValue; } }

        private ObservableCollection<LadderNetworkModel> children;
        public IList<LadderNetworkModel> Children { get { return this.children; } }
        public int NetworkCount { get { return children.Count(); } }
        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };
        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (LadderNetworkModel lnmodel in e.OldItems)
                    lnmodel.PropertyChanged -= OnChildrenPropertyChanged;
            if (e.NewItems != null)
                foreach (LadderNetworkModel lnmodel in e.NewItems)
                    lnmodel.PropertyChanged += OnChildrenPropertyChanged;
            PropertyChanged(this, new PropertyChangedEventArgs("NetworkCount"));
            ChildrenChanged(this, e);
        }
        private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpand" || e.PropertyName == "LadderMode") return;
            parent.InvokeModify((LadderNetworkModel)sender);
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
        }

        private string brief;
        public string Brief
        {
            get { return this.brief; }
            set { this.brief = value; PropertyChanged(this, new PropertyChangedEventArgs("Brief")); }
        }

        private string path;
        public string Path
        {
            get { return this.path; }
        }
        
        private bool isexpand;
        public bool IsExpand
        {
            get { return this.isexpand; }
            set { this.isexpand = value; PropertyChanged(this, new PropertyChangedEventArgs("IsExpand")); }
        }

        private bool ismain;
        public bool IsMainLadder
        {
            get { return this.ismain; }
            set { this.ismain = value; PropertyChanged(this, new PropertyChangedEventArgs("IsMainLadder")); }
        }

        private bool isitpr;
        public bool IsInterruptLadder
        { 
            get { return this.isitpr; }
            set { this.isitpr = value;}
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
                if (Inst != null) Inst.LadderMode = laddermode;
                foreach (LadderNetworkModel network in Children)
                    network.LadderMode = laddermode;
                PropertyChanged(this, new PropertyChangedEventArgs("LadderMode"));
            }
        }
        

        #endregion

        #region Inst

        private InstructionDiagramModel inst;
        public InstructionDiagramModel Inst
        {
            get
            {
                return this.inst;
            }
            set
            {
                if (inst == value) return;
                InstructionDiagramModel _inst = inst;
                this.inst = value;
                if (_inst != null && _inst.Parent != null) _inst.Parent = null;
                if (inst != null && inst.Parent != this) inst.Parent = this;
            }
        }

        #endregion

        #region Shell

        private LadderDiagramViewModel view;
        public LadderDiagramViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                LadderDiagramViewModel _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return view; }
            set { View = (LadderDiagramViewModel)value; }
        }
        
        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        private MainTabDiagramItem tab;
        public MainTabDiagramItem Tab
        {
            get { return this.tab; }
            set { this.tab = value; }
        }

        #endregion

        #region Load & Save

        public void Load(XElement xele)
        {
            name = xele.Attribute("Name").Value;
            XAttribute xatt = xele.Attribute("IsExpand");
            isexpand = xatt == null ? true : bool.Parse(xatt.Value);
            xatt = xele.Attribute("IsMain");
            ismain = xatt == null ? false : bool.Parse(xatt.Value);
            xatt = xele.Attribute("Path");
            path = xatt == null ? null : xatt.Value;
            xatt = xele.Attribute("Brief");
            brief = xatt == null ? string.Empty : xatt.Value ;
            foreach (LadderNetworkModel network in children)
            {
                network.PropertyChanged -= OnChildrenPropertyChanged;
                network.Dispose();
            }
            children.Clear();
            foreach (XElement xele_ch in xele.Elements("Network"))
            {
                LadderNetworkModel lnmodel = new LadderNetworkModel(this, children.Count());
                lnmodel.Load(xele_ch);
                children.Add(lnmodel);
            }
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Name", name);
            xele.SetAttributeValue("IsExpand", isexpand);
            xele.SetAttributeValue("IsMain", ismain);
            xele.SetAttributeValue("Brief", Brief);
            path = ptvitem == null ? null : ptvitem.Path;
            if (path != null) xele.SetAttributeValue("Path", path);
            foreach (LadderNetworkModel lnmodel in children)
            {
                XElement xele_ch = new XElement("Network");
                lnmodel.Save(xele_ch);
                xele.Add(xele_ch);
            }
        }

        #endregion

        #region Commands
        
        public const int CMDTYPE_ReplaceNetwork = 0x01;
        public const int CMDTYPE_ReplaceRow = 0x02;
        public const int CMDTYPE_ReplaceUnit = 0x04;
        public const int CMDTYPE_MoveUnit = 0x08;
        public const int CMDTYPE_ChangeProperty = 0x10;
        public const int CMDTYPE_ChangeComments = 0x20;
        private class Command
        {
            public int Type;
            public LadderNetworkModel Network;
            public LadderUnitModel Unit;
            public IList<LadderNetworkModel> OldNetworks;
            public IList<LadderNetworkModel> NewNetworks;
            public IList<int> OldNetIDs;
            public IList<int> NewNetIDs;
            public IList<LadderUnitModel> OldUnits;
            public IList<LadderUnitModel> NewUnits;
            public IList<LadderUnitModel> MoveUnits;
            public int MoveX;
            public int MoveY;
            public IList<int> OldRows;
            public IList<int> NewRows;
            public IList<string> OldProperties;
            public IList<string> NewProperties;
            public IList<string> OldComments;
            public IList<string> NewComments;
        }

        private class RelativeArea
        {
            public enum Status { NULL, SINGLE, MULTIUNIT, MULTINET};
            private Status status;
            private LadderNetworkModel network;
            private LadderDiagramModel diagram;
            private int x1, x2, y1, y2;
            private int start, end;

            public RelativeArea()
            {
                status = Status.NULL;
            }

            public void Update(LadderUnitModel unit)
            {
                int unit_x = unit.X;
                int unit_y = unit.Y;
                if (unit.Shape == LadderUnitModel.Shapes.VLine) unit_x++;
                switch (status)
                {
                    case Status.NULL:
                        status = Status.SINGLE;
                        network = unit.Parent;
                        x1 = x2 = unit_x;
                        y1 = y2 = unit_y;
                        break;
                    case Status.SINGLE:
                    case Status.MULTIUNIT:
                        if (unit.Parent == network)
                        {
                            status = Status.MULTIUNIT;
                            x1 = Math.Min(x1, unit_x);
                            x2 = Math.Max(x2, unit_x);
                            y1 = Math.Min(y1, unit_y);
                            y2 = Math.Max(y2, unit_y);
                        }
                        else
                        {
                            status = Status.MULTINET;
                            diagram = network.Parent;
                            start = Math.Min(network.ID, unit.Parent.ID);
                            end = Math.Max(network.ID, unit.Parent.ID);
                        }
                        break;
                    case Status.MULTINET:
                        Update(unit.Parent);
                        break;
                }
            }
            public void Update(IEnumerable<LadderUnitModel> units)
            {
                foreach (LadderUnitModel unit in units) Update(unit);
            }
            public void Update(LadderNetworkModel network)
            {
                switch (status)
                {
                    case Status.NULL:
                    case Status.SINGLE:
                    case Status.MULTIUNIT:
                        status = Status.MULTINET;
                        diagram = network.Parent;
                        start = end = network.ID;
                        break;
                    case Status.MULTINET:
                        start = Math.Min(start, network.ID);
                        end = Math.Max(start, network.ID);
                        break;
                }
            }
            public void Update(IEnumerable<LadderNetworkModel> networks)
            {
                foreach (LadderNetworkModel network in networks) Update(network);
            }
            public void Update(LadderNetworkModel _network, int _x1, int _x2, int _y1, int _y2)
            {
                switch (status)
                {
                    case Status.NULL:
                    case Status.SINGLE:
                        status = Status.MULTIUNIT;
                        network = _network;
                        x1 = _x1;
                        x2 = _x2;
                        y1 = _y1;
                        y2 = _y2;
                        break;
                    case Status.MULTIUNIT:
                        if (network == _network)
                        {
                            x1 = Math.Min(x1, _x1);
                            x2 = Math.Max(x2, _x2);
                            y1 = Math.Min(y1, _y1);
                            y2 = Math.Max(y2, _y2);
                        }
                        else
                        {
                            Update(_network);
                        }
                        break;
                    default:
                        Update(_network);
                        break;
                }
            }
            public void Select(InteractionFacade ifparent)
            {
                switch (status)
                {
                    case Status.SINGLE:
                        ifparent.Navigate(network, x1, y1);
                        break;
                    case Status.MULTIUNIT:
                        ifparent.Select(network, x1, x2, y1, y2);
                        break;
                    case Status.MULTINET:
                        ifparent.Select(diagram, start, end);
                        break;
                }
            }
        }

        private Stack<Command> undos = new Stack<Command>();
        private Stack<Command> redos = new Stack<Command>();
        public bool CanUndo { get { return LadderMode == LadderModes.Edit && undos != null && undos.Count() > 0; } }
        public bool CanRedo { get { return LadderMode == LadderModes.Edit && redos != null && redos.Count() > 0; } }
        public void ClearUndoRedoAction()
        {
            undos.Clear();
            redos.Clear();
        }
        public void Undo()
        {
            if (!CanUndo) return;
            Command cmd = undos.Pop();
            RelativeArea area = new RelativeArea();
            LadderNetworkModel net = null;
            int i1 = 0, i2 = 0;
            IList<int> newrows = null;
            if ((cmd.Type & CMDTYPE_ReplaceNetwork) != 0)
            {
                for (i2 = cmd.NewNetworks.Count - 1; i2 >= 0; i2--)
                {
                    net = (LadderNetworkModel)(cmd.NewNetworks[i2]);
                    net.ID = cmd.NewNetIDs[i2];
                    children.RemoveAt(net.ID);
                    net.Parent = null;
                }
                for (i1 = 0, i2 = 0; i1 < children.Count || i2 < cmd.OldNetworks.Count; i1++)
                {
                    if (i2 < cmd.OldNetworks.Count)
                    {
                        net = (LadderNetworkModel)(cmd.OldNetworks[i2]);
                        net.ID = cmd.OldNetIDs[i2];
                        if (i1 == net.ID)
                        {
                            net.Parent = this;
                            children.Insert(i1, net);
                            i2++;
                        }
                    }
                    children[i1].ID = i1;
                }
                area.Update(cmd.OldNetworks);
            }
            if ((cmd.Type & CMDTYPE_ReplaceUnit) != 0)
            {
                if (cmd.NewUnits.Where(u => u.Parent != cmd.Network).Count() > 0)
                {
                    foreach (LadderNetworkModel network in children)
                    {
                        network.Remove(cmd.NewUnits.Where((_unit) => { return _unit.Parent == network && _unit.Shape != LadderUnitModel.Shapes.VLine; }));
                        network.RemoveV(cmd.NewUnits.Where((_unit) => { return _unit.Parent == network && _unit.Shape == LadderUnitModel.Shapes.VLine; }));
                    }
                }
                else
                {
                    cmd.Network.Remove(cmd.NewUnits.Where((_unit) => { return _unit.Shape != LadderUnitModel.Shapes.VLine; }));
                    cmd.Network.RemoveV(cmd.NewUnits.Where((_unit) => { return _unit.Shape == LadderUnitModel.Shapes.VLine; }));
                }
            }
            if ((cmd.Type & CMDTYPE_ReplaceRow) != 0)
            {
                newrows = cmd.NewRows.ToArray();
                for (i2 = 0; i2 < cmd.OldRows.Count; i2++)
                {
                    int y1 = (int)(cmd.OldRows[i2]);
                    int y2 = y1;
                    while (i2 < cmd.OldRows.Count - 1)
                    {
                        int y = (int)(cmd.OldRows[i2 + 1]);
                        if (y == y2 + 1) { y2++; i2++; } else break;
                    }
                    cmd.Network.InsertR(y1, y2);
                    for (i1 = 0; i1 < newrows.Count; i1++)
                        if (newrows[i1] >= y1) newrows[i1] += y2 - y1 + 1;
                    area.Update(cmd.Network, 0, GlobalSetting.LadderXCapacity, y1, y2);
                }
            }
            if ((cmd.Type & CMDTYPE_MoveUnit) != 0)
            {
                cmd.Network.Move(cmd.MoveUnits.Where((_unit) => { return _unit.Shape != LadderUnitModel.Shapes.VLine; }), -cmd.MoveX, -cmd.MoveY);
                cmd.Network.MoveV(cmd.MoveUnits.Where((_unit) => { return _unit.Shape == LadderUnitModel.Shapes.VLine; }), -cmd.MoveX, -cmd.MoveY);
            }
            if ((cmd.Type & CMDTYPE_ReplaceUnit) != 0)
            { 
                if (cmd.OldUnits.Where(u => u.OldParent != null && u.OldParent != cmd.Network).Count() > 0)
                {
                    foreach (LadderNetworkModel network in children)
                    {
                        network.Add(cmd.OldUnits.Where((_unit) => { return (_unit.OldParent == network || _unit.OldParent == null && network == cmd.Network) && _unit.Shape != LadderUnitModel.Shapes.VLine; }));
                        network.AddV(cmd.OldUnits.Where((_unit) => { return (_unit.OldParent == network || _unit.OldParent == null && network == cmd.Network) && _unit.Shape == LadderUnitModel.Shapes.VLine; }));
                    }
                }
                else
                {
                    cmd.Network.Add(cmd.OldUnits.Where((_unit) => { return _unit.Shape != LadderUnitModel.Shapes.VLine; }));
                    cmd.Network.AddV(cmd.OldUnits.Where((_unit) => { return _unit.Shape == LadderUnitModel.Shapes.VLine; }));
                }
                area.Update(cmd.OldUnits);
            }
            if ((cmd.Type & CMDTYPE_ReplaceRow) != 0)
            {
                for (i2 = newrows.Count - 1; i2 >= 0; i2--)
                {
                    int y2 = (int)(newrows[i2]);
                    int y1 = y2;
                    while (i2 > 0)
                    {
                        int y = (int)(newrows[i2 - 1]);
                        if (y == y1 - 1) { y1--; i2--; } else break;
                    }
                    cmd.Network.RemoveR(y1, y2);
                }
            }
            if ((cmd.Type & CMDTYPE_ChangeProperty) != 0)
            {
                cmd.Unit.InstArgs = cmd.OldProperties.ToArray();
                area.Update(cmd.Unit);
            }
            if ((cmd.Type & CMDTYPE_ChangeComments) != 0)
            {
                for (i1 = 0; i1 < cmd.Unit.Children.Count; i1++)
                {
                    try
                    {
                        ValueManager[cmd.Unit.Children[i1]].Comment = cmd.OldComments[i1];
                    }
                    catch (ValueParseException)
                    {
                    }
                }
            }
            if (View != null && View.IsNavigatable 
             && (cmd.Type & CMDTYPE_MoveUnit) != 0)
                area.Select(IFParent);
            redos.Push(cmd);
            Parent.InvokeModify(this, true);
        }

        public void Redo()
        {
            if (!CanRedo) return;
            Command cmd = redos.Pop();
            if ((cmd.Type & CMDTYPE_ReplaceRow) != 0)
            {
                if (cmd.Network.RowCount - cmd.OldRows.Count() + cmd.NewRows.Count() <= 0)
                {
                    cmd.NewRows = cmd.NewRows.Concat(new int[] { 0 }).ToArray();
                }
            }
            if ((cmd.Type & CMDTYPE_ReplaceNetwork) != 0)
            {
                if (Children.Where(n => !n.IsMasked).Count() - cmd.OldNetworks.Count() + cmd.NewNetworks.Count() <= 0)
                {
                    cmd.NewNetworks = cmd.NewNetworks.Concat(new LadderNetworkModel[] { new LadderNetworkModel(null, 0) }).ToArray();
                }
            }
            LadderNetworkModel net = null;
            RelativeArea area = new RelativeArea();
            int i1 = 0, i2 = 0;
            IList<int> oldrows = null;
            if ((cmd.Type & CMDTYPE_ChangeProperty) != 0)
            {
                cmd.Unit.InstArgs = cmd.NewProperties.ToArray();
            }
            if ((cmd.Type & CMDTYPE_ChangeComments) != 0)
            {
                for (i1 = 0; i1 < cmd.Unit.Children.Count; i1++)
                {
                    try
                    {
                        ValueManager[cmd.Unit.Children[i1]].Comment = cmd.NewComments[i1];
                    }
                    catch (ValueParseException)
                    {
                    }
                }
            }
            if ((cmd.Type & CMDTYPE_ReplaceUnit) != 0)
            {
                if (cmd.OldUnits.Where(u => u.Parent != cmd.Network).Count() > 0)
                {
                    foreach (LadderNetworkModel network in children)
                    {
                        network.Remove(cmd.OldUnits.Where((_unit) => { return _unit.Parent == network && _unit.Shape != LadderUnitModel.Shapes.VLine; }));
                        network.RemoveV(cmd.OldUnits.Where((_unit) => { return _unit.Parent == network && _unit.Shape == LadderUnitModel.Shapes.VLine; }));
                    }
                }
                else
                {
                    cmd.Network.Remove(cmd.OldUnits.Where((_unit) => { return _unit.Shape != LadderUnitModel.Shapes.VLine; }));
                    cmd.Network.RemoveV(cmd.OldUnits.Where((_unit) => { return _unit.Shape == LadderUnitModel.Shapes.VLine; }));
                }
            }
            if ((cmd.Type & CMDTYPE_ReplaceRow) != 0)
            {
                oldrows = cmd.OldRows.ToArray();
                for (i2 = 0; i2 < cmd.NewRows.Count; i2++)
                {
                    int y1 = (int)(cmd.NewRows[i2]);
                    int y2 = y1;
                    while (i2 < cmd.NewRows.Count - 1)
                    {
                        int y = (int)(cmd.NewRows[i2 + 1]);
                        if (y == y2 + 1) { y2++; i2++; } else break;
                    }
                    cmd.Network.InsertR(y1, y2);
                    area.Update(cmd.Network, 0, GlobalSetting.LadderXCapacity, y1, y2);
                    for (i1 = 0; i1 < oldrows.Count; i1++)
                        if (oldrows[i1] >= y1) oldrows[i1] += y2 - y1 + 1;
                }
            }
            if ((cmd.Type & CMDTYPE_MoveUnit) != 0)
            {
                List<LadderUnitModel> olds = new List<LadderUnitModel>();
                olds.AddRange(cmd.Network.Move(cmd.MoveUnits.Where((_unit) => { return _unit.Shape != LadderUnitModel.Shapes.VLine; }), cmd.MoveX, cmd.MoveY));
                olds.AddRange(cmd.Network.MoveV(cmd.MoveUnits.Where((_unit) => { return _unit.Shape == LadderUnitModel.Shapes.VLine; }), cmd.MoveX, cmd.MoveY));
                if (olds.Count() > 0)
                {
                    if ((cmd.Type & CMDTYPE_ReplaceUnit) == 0)
                    {
                        cmd.Type |= CMDTYPE_ReplaceUnit;
                        cmd.OldUnits = olds.ToArray();
                        cmd.NewUnits = new LadderUnitModel[] { };
                    }
                    else
                    {
                        cmd.OldUnits = cmd.OldUnits.Union(olds).ToArray();
                    }
                }
            }
            if ((cmd.Type & CMDTYPE_ReplaceUnit) != 0)
            {
                List<LadderUnitModel> olds = new List<LadderUnitModel>();
                if (cmd.NewUnits.Where(u => u.OldParent != null && u.OldParent != cmd.Network).Count() > 0)
                {
                    foreach (LadderNetworkModel network in children)
                    {
                        olds.AddRange(network.Add(cmd.NewUnits.Where((_unit) => { return (_unit.OldParent == network || _unit.OldParent == null && network == cmd.Network) && _unit.Shape != LadderUnitModel.Shapes.VLine; })));
                        olds.AddRange(network.AddV(cmd.NewUnits.Where((_unit) => { return (_unit.OldParent == network || _unit.OldParent == null && network == cmd.Network) && _unit.Shape == LadderUnitModel.Shapes.VLine; })));
                    }
                }
                else
                {
                    olds.AddRange(cmd.Network.Add(cmd.NewUnits.Where((_unit) => { return _unit.Shape != LadderUnitModel.Shapes.VLine; })));
                    olds.AddRange(cmd.Network.AddV(cmd.NewUnits.Where((_unit) => { return _unit.Shape == LadderUnitModel.Shapes.VLine; })));
                }
                if (olds.Count() > 0)
                {
                    cmd.OldUnits = cmd.OldUnits.Union(olds).ToArray();
                }
                area.Update(cmd.NewUnits);
            }
            if ((cmd.Type & CMDTYPE_ReplaceRow) != 0)
            {
                for (i2 = oldrows.Count - 1; i2 >= 0; i2--)
                {
                    int y2 = (int)(oldrows[i2]);
                    int y1 = y2;
                    while (i2 > 0)
                    {
                        int y = (int)(oldrows[i2 - 1]);
                        if (y == y1 - 1) { y1--; i2--; } else break;
                    }
                    cmd.Network.RemoveR(y1, y2);
                }
            }
            if ((cmd.Type & CMDTYPE_ReplaceNetwork) != 0)
            {
                for (i2 = cmd.OldNetworks.Count - 1; i2 >= 0; i2--)
                {
                    net = (LadderNetworkModel)(cmd.OldNetworks[i2]);
                    net.ID = cmd.OldNetIDs[i2];
                    children.RemoveAt(net.ID);
                    net.Parent = null;
                }
                for (i1 = 0, i2 = 0; i1 < children.Count || i2 < cmd.NewNetworks.Count; i1++)
                {
                    if (i2 < cmd.NewNetworks.Count)
                    {
                        net = (LadderNetworkModel)(cmd.NewNetworks[i2]);
                        net.ID = cmd.NewNetIDs[i2];
                        if (i1 == net.ID)
                        {
                            net.Parent = this;
                            children.Insert(i1, net);
                            i2++;
                        }
                    }
                    children[i1].ID = i1;
                }
                area.Update(cmd.NewNetworks);
            }
            if (View != null && View.IsNavigatable
             && (cmd.Type & CMDTYPE_MoveUnit) != 0)
                area.Select(IFParent);
            undos.Push(cmd);
            Parent.InvokeModify(this);
        }

        private void Execute(int _type, object _target, IList<object> _olds, IList<object> _news)
        {
            Command cmd = new Command();
            cmd.Type = _type;
            switch (cmd.Type)
            {
                case CMDTYPE_ReplaceRow:
                    List<LadderUnitModel> oldunits = new List<LadderUnitModel>();
                    cmd.Network = (LadderNetworkModel)_target;
                    cmd.OldRows = _olds.Cast<int>().ToArray();
                    cmd.NewRows = _news.Cast<int>().ToArray();
                    foreach (int row in cmd.OldRows)
                    {
                        oldunits.AddRange(cmd.Network.Children.SelectRange(0, GlobalSetting.LadderXCapacity - 1, row, row));
                        if(cmd.Network.RowCount == 2)
                            oldunits.AddRange(cmd.Network.VLines.SelectRange(0, GlobalSetting.LadderXCapacity - 1, 0, 1));
                        else
                            oldunits.AddRange(cmd.Network.VLines.SelectRange(0, GlobalSetting.LadderXCapacity - 1, row, row));
                    }
                    cmd.Type |= CMDTYPE_ReplaceUnit;
                    cmd.OldUnits = oldunits;
                    cmd.NewUnits = new LadderUnitModel[] { };
                    break;
                case CMDTYPE_ReplaceUnit:
                    cmd.Network = (LadderNetworkModel)_target;
                    cmd.OldUnits = _olds.Cast<LadderUnitModel>().ToArray();
                    cmd.NewUnits = _news.Cast<LadderUnitModel>().ToArray();
                    if (cmd.NewUnits.Count() > 0)
                    {
                        int maxrow = cmd.NewUnits.Select(u => u.Y + (u.Type == LadderUnitModel.Types.VLINE ? 1 : 0)).Max();
                        if (maxrow >= cmd.Network.RowCount)
                        {
                            List<int> newrows = new List<int>();
                            for (int row = cmd.Network.RowCount; row <= maxrow; row++)
                                newrows.Add(row);
                            cmd.Type |= CMDTYPE_ReplaceRow;
                            cmd.OldRows = new int[] { };
                            cmd.NewRows = newrows;
                        }
                    }
                    break;
                case CMDTYPE_ChangeProperty:
                    cmd.Unit = (LadderUnitModel)_target;
                    cmd.OldProperties = _olds.Cast<string>().ToArray();
                    cmd.NewProperties = _news.Cast<string>().ToArray();
                    break;
                default:
                    throw new ArgumentException(String.Format("Cannot execute type {0:s}", _type));
            }
            redos.Clear();
            redos.Push(cmd);
            Redo();
        }

        #endregion

        #region Manipulation (Base)
        
        private static Comparison<LadderNetworkModel> NetworkComparer
            = delegate (LadderNetworkModel net1, LadderNetworkModel net2)
            {
                return net1.ID.CompareTo(net2.ID);
            };

        public void AddN(int id, LadderNetworkModel lnmodel = null)
        {
            if (lnmodel == null) lnmodel = new LadderNetworkModel(null, id);
            //Execute(CMDTYPE_ReplaceNetwork, this, new object[] { }, new object[] { lnmodel});
            ReplaceN(new LadderNetworkModel[] { }, new LadderNetworkModel[] { lnmodel });
        }

        public void RemoveN(int id, LadderNetworkModel lnmodel)
        {
            //Execute(CMDTYPE_ReplaceNetwork, this, new object[] { lnmodel }, new object[] { });
            ReplaceN(new LadderNetworkModel[] { lnmodel }, new LadderNetworkModel[] { });
        }
        
        public void ReplaceN(IEnumerable<LadderNetworkModel> _olds, IEnumerable<LadderNetworkModel> _news)
        {
            IEnumerable<int> newids = _news.Select(n => n.ID);
            ReplaceN(_olds, _news, newids);
        }

        public void ReplaceN(IEnumerable<LadderNetworkModel> _olds, IEnumerable<LadderNetworkModel> _news, IEnumerable<int> _newids)
        {
            List<LadderNetworkModel> olds = _olds.ToList();
            List<LadderNetworkModel> news = _news.ToList();
            olds.Sort(NetworkComparer);
            List<int> oldids = olds.Select(n => n.ID).ToList();
            List<int> newids = _newids.ToList();
            IEnumerable<LadderNetworkModel> nomasks = Children.Where(n => !n.IsMasked);
            //保证操作后，界面至少存在一个可编辑的网络
            if (_news.Count() == 0 && olds.Count() == nomasks.Count())
            {
                LadderNetworkModel newone = new LadderNetworkModel(null, nomasks.First().ID);
                news.Add(newone);
                newids.Add(newone.ID);
            }
            for (int i = 0; i < news.Count; i++)
                news[i].ID = newids[i];
            news.Sort(NetworkComparer);
            newids = news.Select(n => n.ID).ToList();

            Command cmd = new Command();
            cmd.Type = CMDTYPE_ReplaceNetwork;
            cmd.OldNetworks = olds;
            cmd.NewNetworks = news;
            cmd.OldNetIDs = oldids;
            cmd.NewNetIDs = newids;
            redos.Clear();
            redos.Push(cmd);
            Redo();
        }

        public void ExchangeN(LadderNetworkModel net1, LadderNetworkModel net2)
        {
            ReplaceN(new LadderNetworkModel[] { net1, net2 }, new LadderNetworkModel[] { net2, net1 }, new int[] { net1.ID, net2.ID });
        }

        public void AddR(LadderNetworkModel lnmodel, int y1 = -1, int y2 = -1)
        {
            if (y1 == -1) y1 = lnmodel.RowCount;
            if (y2 == -1) y2 = y1;
            object[] news = new object[y2 - y1 + 1];
            for (int y = y1; y <= y2; y++)
                news[y - y1] = y;
            Execute(CMDTYPE_ReplaceRow, lnmodel, new object[] { }, news);
        }

        public void RemoveR(LadderNetworkModel lnmodel, int y1 = -1, int y2 = -1)
        {
            if (y1 == -1) y1 = lnmodel.RowCount - 1;
            if (y2 == -1) y2 = y1;
            object[] olds = new object[y2 - y1 + 1];
            for (int y = y1; y <= y2; y++)
                olds[y - y1] = y;
            Execute(CMDTYPE_ReplaceRow, lnmodel, olds, new object[] { });
        }

        public void AddU(LadderNetworkModel lnmodel, IEnumerable<LadderUnitModel> lumodels)
        {
            ReplaceU(lnmodel, new LadderUnitModel[] { }, lumodels);
        }

        public void RemoveU(LadderNetworkModel lnmodel, IEnumerable<LadderUnitModel> lumodels)
        {
            ReplaceU(lnmodel, lumodels, new LadderUnitModel[] { });
        }

        public void ReplaceU(LadderNetworkModel lnmodel, IEnumerable<LadderUnitModel> olds, IEnumerable<LadderUnitModel> news)
        {
            Execute(CMDTYPE_ReplaceUnit, lnmodel, olds.ToArray(), news.ToArray());
        }
        
        public void UpdateU(LadderUnitModel lumodel, string[] args)
        {
            Execute(CMDTYPE_ChangeProperty, lumodel, lumodel.InstArgs, args);
        }

        public void MoveU(LadderNetworkModel lnmodel, IEnumerable<LadderUnitModel> units, int dx, int dy)
        {
            ReplaceMoveU(lnmodel, new LadderUnitModel[] { }, new LadderUnitModel[] { }, units, dx, dy);
        }

        public void ReplaceMoveU(LadderNetworkModel lnmodel, 
            IEnumerable<LadderUnitModel> olds, IEnumerable<LadderUnitModel> news, 
            IEnumerable<LadderUnitModel> units, int dx, int dy, int rowdelta = 0)
        {
            redos.Clear();
            Command cmd = new Command();
            cmd.Type = CMDTYPE_ReplaceUnit | CMDTYPE_MoveUnit;
            cmd.Network = lnmodel;
            cmd.OldUnits = olds.ToArray();
            cmd.NewUnits = news.ToArray();
            cmd.MoveUnits = units.ToArray();
            cmd.MoveX = dx;
            cmd.MoveY = dy;
            if (rowdelta != 0)
            {
                cmd.Type |= CMDTYPE_ReplaceRow;
                List<int> newrows = new List<int>();
                List<int> oldrows = new List<int>();
                for (int row = lnmodel.RowCount; row < lnmodel.RowCount + rowdelta; row++)
                    newrows.Add(row);
                for (int row = lnmodel.RowCount - 1; row >= lnmodel.RowCount + rowdelta; row--)
                    oldrows.Add(row);
                cmd.OldRows = oldrows;
                cmd.NewRows = newrows;
            }
            redos.Push(cmd);
            Redo();
        }

        public void UpdateUC(LadderUnitModel unit, IList<string> properties)
        {
            List<string> instargs = new List<string>();
            List<string> oldcomments = new List<string>();
            List<string> newcomments = new List<string>();
            for (int i = 0; i < properties.Count / 2; i++)
            {
                string value = properties[i * 2];
                instargs.Add(value);
                try
                {
                    oldcomments.Add(ValueManager[value].Comment);
                    newcomments.Add(properties[i * 2 + 1]);
                }
                catch (ValueParseException)
                {
                    oldcomments.Add("");
                    newcomments.Add("");
                }
            }
            redos.Clear();
            Command cmd = new Command();
            cmd.Type = CMDTYPE_ChangeProperty | CMDTYPE_ChangeComments;
            cmd.Unit = unit;
            cmd.OldProperties = unit.InstArgs;
            cmd.NewProperties = instargs;
            cmd.OldComments = oldcomments;
            cmd.NewComments = newcomments;
            redos.Push(cmd);
            Redo();
        }

        #endregion

        #region Manipulation

        public void AddSingleUnit(LadderUnitModel unit, LadderNetworkModel net, bool cover = true)
        {
            if (unit.X < 0 || unit.X >= GlobalSetting.LadderXCapacity || unit.Y < 0) return;
            LadderUnitModel old = null;
            switch (unit.Shape)
            {
                case LadderUnitModel.Shapes.Output:
                case LadderUnitModel.Shapes.OutputRect:
                    List<LadderUnitModel> news = new List<LadderUnitModel>();
                    for (int x = Math.Max(0, unit.X) ; x < GlobalSetting.LadderXCapacity - 1; x++)
                    {
                        LadderUnitModel hline = new LadderUnitModel(net, LadderUnitModel.Types.HLINE);
                        hline.X = x;
                        hline.Y = unit.Y;
                        if (cover || net.Children[hline.X, hline.Y] == null) news.Add(hline);
                    }
                    int lastX = unit.X;
                    unit.X = GlobalSetting.LadderXCapacity - 1;
                    news.Add(unit);
                    old = net.Children[unit.X, unit.Y];
                    IEnumerable<LadderUnitModel> olds = (cover ? net.Children.SelectRange(lastX, GlobalSetting.LadderXCapacity, unit.Y, unit.Y).ToArray()
                        : old != null ? new LadderUnitModel[] { old } : new LadderUnitModel[] { });
                    ReplaceU(net, olds, news);
                    view.IFParent.Navigate(unit);
                    break;
                case LadderUnitModel.Shapes.VLine:
                    old = net.VLines[unit.X, unit.Y];
                    if (old != null && !cover) return;
                    ReplaceU(net, old != null ? new LadderUnitModel[] { old } : new LadderUnitModel[] { }, new LadderUnitModel[] { unit });
                    break;
                default:
                    if (unit.X == GlobalSetting.LadderXCapacity - 1) return;
                    old = net.Children[unit.X, unit.Y];
                    //if (old != null && !cover) return;
                    ReplaceU(net, old != null ? new LadderUnitModel[] { old } : new LadderUnitModel[] { }, new LadderUnitModel[] { unit });
                    break;
            }
        }

        public void AddSingleUnit(string text, SelectRectCore rect, LadderNetworkModel net, bool cover = true)
        {
            string[] items = text.ToUpper().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0)
                throw new ValueParseException(Properties.Resources.Message_Input_Empty, null);
            if (!LadderUnitModel.TypeOfNames.ContainsKey(items[0]))
                throw new ValueParseException(Properties.Resources.Message_Instruction_Not_Exist, null);
            LadderUnitModel.Types type = LadderUnitModel.TypeOfNames[items[0]];
            LadderUnitModel newunit = null;
            if (type == LadderUnitModel.Types.CALLM)
            {
                if (items.Length < 2)
                    throw new ValueParseException(Properties.Resources.Message_Func_Name_Required, null);
                IEnumerable<FuncModel> fit = Parent.Funcs.Where(_func => _func.Name.Equals(items[1]));
                if (fit.Count() == 0)
                    throw new ValueParseException(String.Format("{0}{1:s}", Properties.Resources.Message_CFunc_Not_Found, items[1]), LadderUnitModel.Formats[(int)type].Formats[0]);
                FuncModel func = fit.First();
                newunit = new LadderUnitModel(null, func);
            }
            else
            {
                newunit = new LadderUnitModel(null, type);
            }
            if (newunit.Children.Count < items.Length - 1)
            {
                newunit.Dispose();
                throw new ValueParseException(Properties.Resources.Message_Func_Params_Num_Error, null);
            }
            string[] instargs = new string[newunit.Children.Count];
            Array.Copy(items, 1, instargs, 0, items.Length - 1);
            for (int i = items.Length; i < instargs.Length; i++)
                instargs[i] = "???";
            try
            {
                newunit.InstArgs = instargs;
                newunit.X = rect.X;
                newunit.Y = rect.Y;
                AddSingleUnit(newunit, net, cover);
            }
            catch (ValueParseException e)
            {
                newunit.Dispose();
                throw e;
            }
        }

        public void RemoveSingleUnit(LadderUnitModel unit)
        {
            RemoveU(unit.Parent, new LadderUnitModel[] { unit });
        }
        
        public void QuickInsertElement(LadderUnitModel.Types type, SelectRectCore rect, bool cover = true)
        {
            QuickInsertElement(type, rect.Parent, rect.X, rect.Y, cover);
        }

        public void QuickInsertElement(LadderUnitModel.Types type, LadderNetworkModel net, int x, int y, bool cover = true)
        {
            LadderUnitModel unit = new LadderUnitModel(null, type) { X = x, Y = y };
            AddSingleUnit(unit, net, cover);
        }
        
        #endregion
    }
}
