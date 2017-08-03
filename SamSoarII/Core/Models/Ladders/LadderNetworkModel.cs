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
    public class LadderNetworkModel : IModel
    {
        public LadderNetworkModel(LadderDiagramModel _parent, int _id)
        {
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

        public void Dispose()
        {
            foreach (LadderUnitModel unit in children)
                unit.Dispose();
            children.Clear();
            children = null;
            parent = null;
            brief = null;
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
                if (ValueManager != null) ValueManager.Remove(this);
                this.parent = value;
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
            get { return this.rowcount; }
            set { this.rowcount = value; PropertyChanged(this, new PropertyChangedEventArgs("RowCount")); }
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
            }
        }

        private bool isexpand;
        public bool IsExpand
        {
            get { return this.isexpand; }
            set { this.isexpand = value; PropertyChanged(this, new PropertyChangedEventArgs("IsExpand")); }
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
                PropertyChanged(this, new PropertyChangedEventArgs("LadderMode"));
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
            brief = xele.Element("Brief").Value;
            description = xele.Element("Description").Value;
            XElement xele_lc = xele.Element("LadderContent");
            children.Clear();
            foreach (XElement xele_unit in xele_lc.Elements("InstEle"))
            {
                LadderUnitModel unit = new LadderUnitModel(this, xele_unit);
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

        #endregion

        #region Unit Modify

        public LadderUnitModel Add(LadderUnitModel lumodel)
        {
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
                yield return _lumodel;
            }
        }

        #endregion

        #region VLine Modify

        public LadderUnitModel AddV(LadderUnitModel lumodel)
        {
            LadderUnitModel _lumodel = vlines[lumodel.X, lumodel.Y];
            if (_lumodel != null) Remove(_lumodel);
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
                yield return _lumodel;
            }
        }

        #endregion

        #region Row Modify

        public void InsertR(int y1, int y2)
        {
            RowCount += y2 - y1 + 1;
            if (y1 < RowCount)
            {
                Move(children.SelectRange(0, 11, y1, RowCount - 1).ToArray(), 0, y2 - y1 + 1);
                MoveV(vlines.SelectRange(0, 11, y1, RowCount - 1).ToArray(), 0, y2 - y1 + 1);
            }
        }

        public IEnumerable<LadderUnitModel> RemoveR(int y1, int y2)
        {
            IEnumerable<LadderUnitModel> removes = children.SelectRange(0, 11, y1, y2);
            removes = removes.Concat(vlines.SelectRange(0, 11, y1, y2));
            removes = removes.ToArray();
            Remove(children.SelectRange(0, 11, y1, y2));
            RemoveV(vlines.SelectRange(0, 11, y1, y2));
            Move(children.SelectRange(0, 11, y2 + 1, RowCount - 1).ToArray(), 0, -(y2 - y1 + 1));
            MoveV(vlines.SelectRange(0, 11, y2 + 1, RowCount - 1).ToArray(), 0, -(y2 - y1 + 1));
            RowCount -= y2 - y1 + 1;
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
        
        #region Compile relative
        public void ClearSearchedFlag()
        {
            foreach (var model in Children.Concat(VLines))
            {
                model.IsSearched = false;
            }
        }
        public void ClearNextElements()
        {
            foreach (var ele in Children)
            {
                ele.NextElements.Clear();
            }
        }
        
        public void PreCompile()
        {
            ClearSearchedFlag();
            ClearNextElements();
            Queue<LadderUnitModel> tempQueue = new Queue<LadderUnitModel>(Children.Where(
                x => { return x.Shape == LadderUnitModel.Shapes.Output || x.Shape == LadderUnitModel.Shapes.OutputRect; }));
            while (tempQueue.Count > 0)
            {
                var tempElement = tempQueue.Dequeue();
                if (tempElement.Type != LadderUnitModel.Types.NULL)
                {
                    var templist = SearchFrom(tempElement);
                    foreach (var ele in templist)
                    {
                        tempQueue.Enqueue(ele);
                    }
                }
            }
            InitializeSubElements();
        }
        
        private void InitializeSubElements()
        {
            ClearSearchedFlag();
            ClearSubElements();
            var rootElements = Children.Where(x => { return x.Shape == LadderUnitModel.Shapes.Output || x.Shape == LadderUnitModel.Shapes.OutputRect; });
            foreach (var rootElement in rootElements)
            {
                GetSubElements(rootElement);
            }
        }
        private void ClearSubElements()
        {
            foreach (var ele in Children)
            {
                ele.SubElements.Clear();
            }
        }
        //得到所有子元素，包括自身和NULL元素。
        private List<LadderUnitModel> GetSubElements(LadderUnitModel model)
        {
            if (model.IsSearched)
            {
                return model.SubElements;
            }
            List<LadderUnitModel> result = new List<LadderUnitModel>();
            foreach (var ele in model.NextElements)
            {
                var tempList = GetSubElements(ele);
                foreach (var item in tempList)
                {
                    if (!result.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }
            result.Add(model);
            model.SubElements = result;
            model.IsSearched = true;
            return result;
        }
        public bool CheckVerticalLine(LadderUnitModel VLine)
        {
            IntPoint pos = new IntPoint();
            IntPoint one = new IntPoint();
            IntPoint two = new IntPoint();
            pos.X = VLine.X;
            pos.Y = VLine.Y;
            one.X = pos.X + 1;
            one.Y = pos.Y;
            two.X = pos.X;
            two.Y = pos.Y - 1;
            if (Children[one.X, one.Y] == null
             && VLines[two.X, two.Y] == null)
            {
                return false;
            }
            if (Children[pos.X, pos.Y] == null
             && VLines[two.X, two.Y] == null)
            {
                return false;
            }
            pos.Y += 1;
            one.Y += 1;
            two.Y += 2;
            if (Children[pos.X, pos.Y] == null
             && Children[one.X, one.Y] == null
             && VLines[two.X, two.Y] == null)
            {
                return false;
            }
            return true;
        }
        private List<LadderUnitModel> SearchFrom(LadderUnitModel viewmodel)
        {
            if (viewmodel.IsSearched)
            {
                return viewmodel.NextElements;
            }
            List<LadderUnitModel> result = new List<LadderUnitModel>();
            if (viewmodel.X == 0)
            {
                viewmodel.IsSearched = true;
                LadderUnitModel nullModel = new LadderUnitModel(this, LadderUnitModel.Types.NULL);
                nullModel.X = 0;
                nullModel.Y = viewmodel.Y;
                result.Add(nullModel);
                //result.Add(BaseViewModel.Null);
                viewmodel.NextElements = result;
                return result;
            }
            else
            {
                int x = viewmodel.X;
                int y = viewmodel.Y;
                var relativePoints = GetRelativePoint(x - 1, y);
                foreach (var p in relativePoints)
                {
                    LadderUnitModel leftmodel = Children[p.X, p.Y];
                    if (leftmodel != null)
                    {
                        if (leftmodel.Type == LadderUnitModel.Types.HLINE)
                        {
                            var _leftresult = SearchFrom(leftmodel);
                            foreach (var temp in SearchFrom(leftmodel))
                            {
                                result.Add(temp);
                            }
                        }
                        else
                        {
                            result.Add(leftmodel);
                        }
                    }
                }
                viewmodel.IsSearched = true;
                viewmodel.NextElements = result;
                return result;
            }
        }
        private IEnumerable<IntPoint> GetRelativePoint(int x, int y)
        {
            List<IntPoint> result = new List<IntPoint>();
            Stack<IntPoint> tempStack = new Stack<IntPoint>();
            var p = new IntPoint() { X = x, Y = y };

            var q1 = new IntPoint() { X = x, Y = y };
            var q2 = new IntPoint() { X = x, Y = y };
            while (SearchUpVLine(q1))
            {
                q1 = new IntPoint() { X = q1.X, Y = q1.Y - 1 };
                tempStack.Push(q1);
            }
            while (tempStack.Count > 0)
            {
                result.Add(tempStack.Pop());
            }
            result.Add(p);
            while (SearchDownVLine(q2))
            {
                q2 = new IntPoint() { X = q2.X, Y = q2.Y + 1 };
                result.Add(q2);
            }
            return result;
        }
        
        private bool SearchUpVLine(IntPoint p)
        {
            return VLines.Any(e => { return (e.X == p.X) && (e.Y == p.Y - 1); });
        }

        private bool SearchDownVLine(IntPoint p)
        {
            return VLines.Any(e => { return (e.X == p.X) && (e.Y == p.Y); });
        }
        #endregion

        #endregion

    }
}
