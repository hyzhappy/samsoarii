using SamSoarII.Core.Generate;
using SamSoarII.Global;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class RowChangedEventArgs : EventArgs
    {
        public enum Actions { INSERT, REMOVE };
        public Actions Action { get; private set; }
        public int Start { get; private set; }
        public int Count { get; private set; }
        public int End { get { return Start + Count - 1; } }

        public RowChangedEventArgs(Actions _action, int _start, int _count)
        {
            Action = _action;
            Start = _start;
            Count = _count;
        }
    }

    public delegate void RowChangedEventHandler(LadderNetworkModel sender, RowChangedEventArgs e);

    public enum LadderNetworkActions { INSERT, REMOVE, UPDATE};
    
    public class LadderNetworkChangedEventArgs : EventArgs
    {
        public LadderNetworkActions Action { get; private set; }
        public LadderNetworkChangedEventArgs(LadderNetworkActions _action) { Action = _action; }
    }

    public delegate void LadderNetworkChangedEventHandler(LadderNetworkModel sender, LadderNetworkChangedEventArgs e);
    
    public class LadderNetworkModel : IModel
    {
        public LadderNetworkModel(LadderDiagramModel _parent, int _id)
        {
            commentareaheight = 56.233;
            children = new GridDictionary<LadderUnitModel>(GlobalSetting.LadderXCapacity);
            vlines = new GridDictionary<LadderUnitModel>(GlobalSetting.LadderXCapacity);
            ID = _id;
            Brief = String.Empty;
            Description = String.Empty;
            RowCount = 1;
            Inst = new InstructionNetworkModel(this);
            IsExpand = true;
            IsMasked = false;
            Parent = _parent;
        }

        public LadderNetworkModel(LadderDiagramModel _parent, XElement xele)
        {
            commentareaheight = 56.233;
            children = new GridDictionary<LadderUnitModel>(GlobalSetting.LadderXCapacity);
            vlines = new GridDictionary<LadderUnitModel>(GlobalSetting.LadderXCapacity);
            Parent = _parent;
            Load(xele);
            Inst = new InstructionNetworkModel(this);
        }

        public void Dispose()
        {
            Parent = null;
            foreach (LadderUnitModel unit in children)
                unit.Dispose();
            foreach (LadderUnitModel unit in vlines)
                unit.Dispose();
            children.Clear();
            children = null;
            vlines.Clear();
            vlines = null;
            brief = null;
            description = null;
            PTVItem = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderDiagramModel parent;
        public LadderDiagramModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                if (ValueManager != null) ValueManager.Remove(this);
                this.parent = value;
                if (parent != null)
                {
                    LadderMode = parent.LadderMode;
                    IsCommentMode = parent.IsCommentMode;
                }
                if (ValueManager != null) ValueManager.Add(this);
            }
        }
        IModel IModel.Parent { get { return Parent; } }
        public ValueManager ValueManager
        {
            get { return parent?.Parent?.Parent.MNGValue; }
        }

        private GridDictionary<LadderUnitModel> children;
        public GridDictionary<LadderUnitModel> Children
        {
            get { return children; }
        }
        private GridDictionary<LadderUnitModel> vlines;
        public GridDictionary<LadderUnitModel> VLines
        {
            get { return vlines; }
        }

        private int rowcount;
        public int RowCount
        {
            get
            {
                return this.rowcount;
            }
            set
            {
                if (rowcount == value) return;
                this.rowcount = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
                parent?.UpdateCanvasTop();
            }
        }

        private int id;
        public int ID
        {
            get { return this.id; }
            set { this.id = value; PropertyChanged(this, new PropertyChangedEventArgs("ID")); }
        }

        private string brief;
        public string Brief
        {
            get { return brief != null ? this.brief : ""; }
            set { this.brief = value; PropertyChanged(this, new PropertyChangedEventArgs("Brief")); }
        }

        private string description;
        public string Description
        {
            get { return description != null ? this.description : ""; }
            set { this.description = value; PropertyChanged(this, new PropertyChangedEventArgs("Description")); }
        }

        private bool ismasked;
        public bool IsMasked
        {
            get
            {
                return this.ismasked;
            }
            set
            {
                //if (!ismasked && value) ValueManager.Remove(this);
                //if (ismasked && !value) ValueManager.Add(this);
                this.ismasked = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsMasked"));
                foreach (LadderUnitModel unit in Children.Concat(VLines))
                    unit.InvokePropertyChanged("IsUsed");
            }
        }

        #endregion

        #region Inst

        private InstructionNetworkModel inst;
        public InstructionNetworkModel Inst
        {
            get
            {
                return this.inst;
            }
            set
            {
                if (inst == value) return;
                InstructionNetworkModel _inst = inst;
                this.inst = value;
                if (_inst != null && _inst.Parent != null) _inst.Parent = null;
                if (inst != null && inst.Parent != this) inst.Parent = this;
            }
        }

        #endregion

        #region Shell

        private LadderNetworkViewModel view;
        public LadderNetworkViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                LadderNetworkViewModel _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return view; }
            set { View = (LadderNetworkViewModel)value; }
        }
        
        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        public event PropertyChangedEventHandler ViewPropertyChanged = delegate { };

        private bool isexpand;
        public bool IsExpand
        {
            get
            {
                return this.isexpand;
            }
            set
            {
                if (isexpand == value) return;
                this.isexpand = value;
                ViewPropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
                parent?.UpdateCanvasTop();
            }
        }

        private bool isbriefexpand;
        public bool IsBriefExpand
        {
            get
            {
                return this.isbriefexpand;
            }
            set
            {
                if (isbriefexpand == value) return;
                this.isbriefexpand = value;
                ViewPropertyChanged(this, new PropertyChangedEventArgs("IsBriefExpand"));
            }
        }

        private double commentareaheight;
        public double CommentAreaHeight
        {
            get
            {
                return this.commentareaheight;
            }
            set
            {
                this.commentareaheight = value;
                parent?.UpdateCanvasTop();
            }
        }
        
        public int BriefRowCount
        {
            get
            {
                int rows = 1;
                for (int i = 0; i < Description.Length; i++)
                    if (Description[i] == '\n') rows++;
                return rows;
            }
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
                foreach (LadderUnitModel unit in Children.Concat(VLines))
                    unit.LadderMode = laddermode;
                ViewPropertyChanged(this, new PropertyChangedEventArgs("LadderMode"));
            }
        }

        private bool iscommentmode;
        public bool IsCommentMode
        {
            get
            {
                return this.iscommentmode;
            }
            set
            {
                this.iscommentmode = value;
                foreach (LadderUnitModel unit in Children.Concat(VLines))
                    unit.IsCommentMode = iscommentmode;
                ViewPropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode"));
            }
        }

        private double canvastop;
        public double CanvasTop
        {
            get { return this.canvastop; }
            set { this.canvastop = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("CanvasTop")); }
        }

        private double viewheight;
        public double ViewHeight
        {
            get { return this.viewheight; }
            set { this.viewheight = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("ViewHeight")); }
        }

        private double unitbasetop;
        public double UnitBaseTop
        {
            get { return this.unitbasetop; }
            set { this.unitbasetop = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("UnitBaseTop")); }
        }
        
        #endregion

        #region Load & Save

        public void Load(XElement xele)
        {
            id = int.Parse(xele.Attribute("Number").Value);
            rowcount = int.Parse(xele.Attribute("RowCount").Value);
            XAttribute xatt = xele.Attribute("IsMasked");
            ismasked = xatt == null ? false : bool.Parse(xatt.Value);
            xatt = xele.Attribute("IsExpand");
            isexpand = xatt == null ? true : bool.Parse(xatt.Value);
            XElement xele_b = xele.Element("Brief");
            brief = xele_b != null ? xele_b.Value : "";
            XElement xele_d = xele.Element("Description");
            description = xele_d != null ? xele_d.Value : "";
            XElement xele_lc = xele.Element("LadderContent");
            children.Clear();
            foreach (XElement xele_unit in xele_lc.Elements("InstEle"))
            {
                LadderUnitModel unit = LadderUnitModel.Create(this, xele_unit);
                if (unit.Shape == LadderUnitModel.Shapes.VLine)
                    vlines[unit.X, unit.Y] = unit;
                else
                    children[unit.X, unit.Y] = unit;
                rowcount = Math.Max(rowcount, unit.Y + 1);
            }
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Number", id);
            xele.SetAttributeValue("RowCount", rowcount);
            xele.SetAttributeValue("IsMasked", ismasked);
            xele.SetAttributeValue("IsExpand", isexpand);
            xele.SetElementValue("Brief", brief);
            xele.SetElementValue("Description", description);
            XElement xele_lc = new XElement("LadderContent");
            xele.Add(xele_lc);
            foreach (LadderUnitModel unit in children)
            {
                XElement xele_unit = new XElement("InstEle");
                unit.Save(xele_unit);
                xele_lc.Add(xele_unit);
            }
            foreach (LadderUnitModel unit in vlines)
            {
                XElement xele_unit = new XElement("InstEle");
                unit.Save(xele_unit);
                xele_lc.Add(xele_unit);
            }
        }

        public void CopyToClipboard()
        {
            XElement xele = new XElement("Root");
            XElement xele_ns = new XElement("Networks");
            XElement xele_n = new XElement("Network");
            Save(xele_n);
            xele_ns.Add(xele_n);
            xele.Add(xele_ns);
            Clipboard.SetData("LadderContent", xele.ToString());
        }

        #endregion

        #region Modify

        #region Event

        public event LadderUnitChangedEventHandler ChildrenChanged = delegate { };
        
        public void InvokeByChildren(LadderUnitModel unit, LadderUnitAction action)
        {
            ChildrenChanged(unit, new LadderUnitChangedEventArgs(action));
        }

        public event LadderNetworkChangedEventHandler Changed = delegate { };

        public void Invoke(LadderNetworkActions action)
        {
            Changed(this, new LadderNetworkChangedEventArgs(action));
        }

        #endregion

        #region Unit Modify

        public LadderUnitModel Add(LadderUnitModel lumodel)
        {
            if (lumodel.Shape == LadderUnitModel.Shapes.VLine)
                return AddV(lumodel);
            if (lumodel.X < 0 || lumodel.Y < 0)
                throw new LadderUnitChangedEventException(LadderUnitAction.ADD, Properties.Resources.LadderUnit_LocationError);
            switch (lumodel.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                case LadderUnitModel.Shapes.Special:
                case LadderUnitModel.Shapes.HLine:
                    if (lumodel.X >= GlobalSetting.LadderXCapacity - 1)
                        throw new LadderUnitChangedEventException(LadderUnitAction.ADD, Properties.Resources.LadderUnit_LocationError);
                    break;
                case LadderUnitModel.Shapes.Output:
                case LadderUnitModel.Shapes.OutputRect:
                    if (lumodel.X != GlobalSetting.LadderXCapacity - 1)
                        throw new LadderUnitChangedEventException(LadderUnitAction.ADD, Properties.Resources.LadderUnit_LocationError);
                    break;
            }
            LadderUnitModel _lumodel = children[lumodel.X, lumodel.Y];
            if (_lumodel != null) Remove(_lumodel);
            children[lumodel.X, lumodel.Y] = lumodel;
            lumodel.Parent = this;
            lumodel.Invoke(LadderUnitAction.ADD);
            return _lumodel;
        }
        
        public IEnumerable<LadderUnitModel> Add(IEnumerable<LadderUnitModel> lumodels)
        {
            foreach (LadderUnitModel lumodel in lumodels)
                yield return Add(lumodel);
        }

        public void Remove(LadderUnitModel lumodel)
        {
            if (lumodel.Shape == LadderUnitModel.Shapes.VLine)
                RemoveV(lumodel);
            lumodel.Invoke(LadderUnitAction.REMOVE);
            children[lumodel.X, lumodel.Y] = null;
            lumodel.Parent = null;
        }
        public void Remove(IEnumerable<LadderUnitModel> lumodels)
        {
            foreach (LadderUnitModel lumodel in lumodels) Remove(lumodel);
        }

        public LadderUnitModel Move(LadderUnitModel lumodel, int dx, int dy)
        {
            if (lumodel.Shape == LadderUnitModel.Shapes.VLine)
                MoveV(lumodel, dx, dy);
            children[lumodel.X, lumodel.Y] = null;
            lumodel.X += dx;
            lumodel.Y += dy;
            LadderUnitModel _lumodel = children[lumodel.X, lumodel.Y];
            if (_lumodel != null) Remove(_lumodel);
            children[lumodel.X, lumodel.Y] = lumodel;
            lumodel.Invoke(LadderUnitAction.MOVE);
            return _lumodel;
        }

        public IEnumerable<LadderUnitModel> Move(IEnumerable<LadderUnitModel> lumodels, int dx, int dy)
        {
            foreach (LadderUnitModel lumodel in lumodels)
            {
                children[lumodel.X, lumodel.Y] = null;
                lumodel.X += dx;
                lumodel.Y += dy;
            }
            foreach (LadderUnitModel lumodel in lumodels)
            {
                LadderUnitModel _lumodel = children[lumodel.X, lumodel.Y];
                if (_lumodel != null) Remove(_lumodel);
                children[lumodel.X, lumodel.Y] = lumodel;
                lumodel.Invoke(LadderUnitAction.MOVE);
                if (_lumodel != null) yield return _lumodel;
            }
        }

        #endregion

        #region VLine Modify

        public LadderUnitModel AddV(LadderUnitModel lumodel)
        {
            if (lumodel.Shape != LadderUnitModel.Shapes.VLine)
                return Add(lumodel);
            LadderUnitModel _lumodel = vlines[lumodel.X, lumodel.Y];
            if (_lumodel != null) RemoveV(_lumodel);
            vlines[lumodel.X, lumodel.Y] = lumodel;
            lumodel.Parent = this;
            lumodel.Invoke(LadderUnitAction.ADD);
            return _lumodel;
        }
        public IEnumerable<LadderUnitModel> AddV(IEnumerable<LadderUnitModel> lumodels)
        {
            foreach (LadderUnitModel lumodel in lumodels)
                yield return AddV(lumodel);
        }

        public void RemoveV(LadderUnitModel lumodel)
        {
            if (lumodel.Shape != LadderUnitModel.Shapes.VLine)
                Remove(lumodel);
            lumodel.Invoke(LadderUnitAction.REMOVE);
            vlines[lumodel.X, lumodel.Y] = null;
            lumodel.Parent = null;
        }
        public void RemoveV(IEnumerable<LadderUnitModel> lumodels)
        {
            foreach (LadderUnitModel lumodel in lumodels) RemoveV(lumodel);
        }

        public LadderUnitModel MoveV(LadderUnitModel lumodel, int dx, int dy)
        {
            if (lumodel.Shape != LadderUnitModel.Shapes.VLine)
                Move(lumodel, dx, dy);
            vlines[lumodel.X, lumodel.Y] = null;
            lumodel.X += dx;
            lumodel.Y += dy;
            LadderUnitModel _lumodel = vlines[lumodel.X, lumodel.Y];
            if (_lumodel != null) RemoveV(_lumodel);
            vlines[lumodel.X, lumodel.Y] = lumodel;
            lumodel.Invoke(LadderUnitAction.MOVE);
            return _lumodel;
        }
        public IEnumerable<LadderUnitModel> MoveV(IEnumerable<LadderUnitModel> lumodels, int dx, int dy)
        {
            foreach (LadderUnitModel lumodel in lumodels)
            {
                vlines[lumodel.X, lumodel.Y] = null;
                lumodel.X += dx;
                lumodel.Y += dy;
            }
            foreach (LadderUnitModel lumodel in lumodels)
            {
                LadderUnitModel _lumodel = vlines[lumodel.X, lumodel.Y];
                if (_lumodel != null) RemoveV(_lumodel);
                vlines[lumodel.X, lumodel.Y] = lumodel;
                lumodel.Invoke(LadderUnitAction.MOVE);
                if (_lumodel != null) yield return _lumodel;
            }
        }

        #endregion

        #region Row Modify

        public event RowChangedEventHandler RowChanged = delegate { };

        public void InsertR(int y1, int y2)
        {
            RowCount += y2 - y1 + 1;
            if (y1 < RowCount)
            {
                List<LadderUnitModel> olds = new List<LadderUnitModel>();
                olds.AddRange(Move(children.SelectRange(0, 11, y1, RowCount - 1).ToArray(), 0, y2 - y1 + 1));
                olds.AddRange(MoveV(vlines.SelectRange(0, 11, y1, RowCount - 1).ToArray(), 0, y2 - y1 + 1));
            }
            RowChanged(this, new RowChangedEventArgs(RowChangedEventArgs.Actions.INSERT, y1, y2 - y1 + 1));
        }

        public IEnumerable<LadderUnitModel> RemoveR(int y1, int y2)
        {
            IEnumerable<LadderUnitModel> removes = children.SelectRange(0, 11, y1, y2);
            removes = removes.Concat(vlines.SelectRange(0, 11, y1, y2));
            removes = removes.ToArray();
            Remove(children.SelectRange(0, 11, y1, y2));
            RemoveV(vlines.SelectRange(0, 11, y1, y2));
            List<LadderUnitModel> olds = new List<LadderUnitModel>();
            olds.AddRange(Move(children.SelectRange(0, 11, y2 + 1, RowCount - 1).ToArray(), 0, -(y2 - y1 + 1)));
            olds.AddRange(MoveV(vlines.SelectRange(0, 11, y2 + 1, RowCount - 1).ToArray(), 0, -(y2 - y1 + 1)));
            removes = removes.Concat(olds).Where(u => u != null).ToArray();
            RowCount -= y2 - y1 + 1;
            RowChanged(this, new RowChangedEventArgs(RowChangedEventArgs.Actions.REMOVE, y1, y2 - y1 + 1));
            return removes;
        }

        #endregion

        #endregion

        #region For LadderCheckModule

        public HashSet<LadderUnitModel> ErrorModels { get; set; } = new HashSet<LadderUnitModel>();

        public Dictionary<int, LadderLogicModule> LadderLogicModules { get; set; }

        public int GetMaxY()
        {
            IEnumerable<int> ys = Children.Concat(VLines).Select(u => u.Y);
            return ys.Count() > 0 ? ys.Max() : 0;
        }

        #region generate LadderLogicModule
        
        public void InitializeLadderLogicModules()
        {
            int cnt = 0, maxY = RowCount - 1;
            List<LadderUnitModel> models = new List<LadderUnitModel>();
            List<LadderUnitModel> vlines = new List<LadderUnitModel>();
            LadderLogicModules = new Dictionary<int, LadderLogicModule>();
            for (int i = 0; i <= maxY; i++)
            {
                var tempmodels = Children.Where(x => { return x.Y == i; });
                var tempvlines = VLines.Where(x => { return x.Y == i; });
                if (tempmodels.Count() > 0)
                {
                    models.AddRange(tempmodels);
                    if (tempvlines.Count() > 0)
                    {
                        vlines.AddRange(tempvlines);
                    }
                    else
                    {
                        LadderLogicModules.Add(cnt, new LadderLogicModule(this, models, vlines));
                        if (i < maxY)
                        {
                            cnt++;
                            models = new List<LadderUnitModel>();
                            vlines = new List<LadderUnitModel>();
                        }
                    }
                }
                else if (tempvlines.Count() > 0)
                {
                    vlines.AddRange(tempvlines);
                }
            }
        }

        public int GetKeyByLadderLogicModule(LadderLogicModule ladderLogicModule)
        {
            foreach (var item in LadderLogicModules)
            {
                if (item.Value == ladderLogicModule)
                {
                    return item.Key;
                }
            }
            return -1;
        }
        public LadderLogicModule GetLadderLogicModuleByKey(int key)
        {
            return LadderLogicModules[key];
        }
        
        #endregion

        #endregion

    }
}
