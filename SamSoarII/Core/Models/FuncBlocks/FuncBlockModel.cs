using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class FuncBlockModel : IModel
    {
        public FuncBlockModel(ProjectModel _parent, string _name, string _code, bool _islibrary = false)
        {
            parent = _parent;
            name = _name;
            code = _code;
            islibrary = _islibrary;
            BuildAll(code);
        }

        public void Dispose()
        {
            Root = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void InvokePropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #region Numbers

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string path;
        public string Path
        {
            get { return this.path; }
        }

        private string code;
        public string Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        private bool islibrary;
        public bool IsLibrary { get { return this.islibrary; } }

        public IEnumerable<FuncModel> Funcs
        {
            get
            {
                foreach (FuncBlock fb in Root.Childrens)
                {
                    if (fb is FuncBlock_FuncHeader)
                    {
                        FuncBlock_FuncHeader fbfh = (FuncBlock_FuncHeader)(fb);
                        yield return fbfh.FuncModel;
                    }
                }
            }
        }

        #endregion

        #region Code Structure System

        protected FuncBlock root;
        public FuncBlock Root
        {
            get { return this.root; }
            set { this.root = value; }
        }

        protected LinkedListNode<FuncBlock> current;
        public FuncBlock Current
        {
            get { return this.current?.Value; }
        }
        public LinkedListNode<FuncBlock> CurrentNode
        {
            get { return this.current; }
            set { this.current = value; }
        }

        protected int index;
        public int CurrentIndex
        {
            get { return this.index; }
            protected set { this.index = value; }
        }

        public Dictionary<string, SortedList<int, FuncBlock_Assignment>> Assigns { get; }
            = new Dictionary<string, SortedList<int, FuncBlock_Assignment>>();
        
        public List<string> GetCodeCompleteNames(string profix)
        {
            List<string> ret = new List<string>();
            if (Current != null && Current is FuncBlock_Comment)
            {
                return ret;
            }
            FuncBlock _current = Current;
            string _profix;
            while (_current != null)
            {
                _profix = String.Format("{0:s}::{1:s}", _current.Namespace, profix);
                if (Assigns.ContainsKey(_profix))
                {
                    SortedList<int, FuncBlock_Assignment> subassigns = Assigns[_profix];
                    foreach (FuncBlock_Assignment fbassign in subassigns.Values)
                    {
                        if (fbassign.IndexEnd < CurrentIndex)
                        {
                            ret.Add(fbassign.Name);
                        }
                    }
                }
                if (_current is FuncBlock_Assignment || _current is FuncBlock_Statement)
                {
                    if (_current.Parent is FuncBlock_Root)
                    {
                        break;
                    }
                    _current = _current.Parent.Parent;
                }
                else
                {
                    _current = _current.Parent;
                }
            }
            ret.Sort();
            return ret;
        }

        public void Move(int index)
        {
            CurrentIndex = index;
            while (Current.Parent != null && !Current.Contains(index))
            {
                while (index > Current.IndexEnd)
                {
                    if (current.Next != null && current.Next.Value.IndexStart <= index)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        if (Current.Parent == null)
                        {
                            break;
                        }
                        Current.Parent.Current = current;
                        if (Current.Parent.Parent == null)
                        {
                            current = new LinkedListNode<FuncBlock>(Root);
                        }
                        else
                        {
                            current = Current.Parent.Parent.Current;
                        }
                    }
                }
                while (index < Current.IndexStart)
                {
                    if (current.Previous != null && current.Previous.Value.IndexEnd >= index)
                    {
                        current = current.Previous;
                    }
                    else
                    {
                        if (Current.Parent == null)
                        {
                            break;
                        }
                        Current.Parent.Current = current;
                        if (Current.Parent.Parent == null)
                        {
                            current = new LinkedListNode<FuncBlock>(Root);
                        }
                        else
                        {
                            current = Current.Parent.Parent.Current;
                        }
                    }
                }
            }
            if (Current.Parent == null && !Current.Contains(index))
            {
                return;
            }
            while (Current.Current != null)
            {
                Current.CastOffset();
                while (index < Current.Current.Value.IndexStart)
                {
                    if (Current.Current.Previous == null)
                    {
                        break;
                    }
                    else
                    {
                        Current.Current = Current.Current.Previous;
                    }
                }
                while (index > Current.Current.Value.IndexEnd)
                {
                    if (Current.Current.Next == null)
                    {
                        break;
                    }
                    else
                    {
                        Current.Current = Current.Current.Next;
                    }
                }
                if (Current.Current.Value.Contains(index))
                {
                    current = Current.Current;
                }
                else if (Current.Current.Next != null &&
                    Current.Current.Next.Value.Contains(index))
                {
                    Current.Current = Current.Current.Next;
                    current = Current.Current.Next;
                }
                else
                {
                    break;
                }
            }
        }

        public void BuildAll(string code)
        {
            Root = new FuncBlock_Root(this, code);
            this.current = new LinkedListNode<FuncBlock>(Root);
            //Root.Build(text);   
            FuncBlock_Assignment assign = null;
            assign = new FuncBlock_Assignment(this, Root, "uint_32* XBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* YBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* MBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* CBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* TBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* SBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_16* DWord;");
            assign = new FuncBlock_Assignment(this, Root, "uint_16* CVWord;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* CVDoubleWord;");
            assign = new FuncBlock_Assignment(this, Root, "uint_16* TVWord;");
            InvokePropertyChanged("Funcs");
            Parent.IsModified = true;
        }

        #endregion

        #region Breakpoint System

        private int bpaddrmin;
        private int bpaddrmax;

        public void InitBP()
        {
            InitBP(Root);
        }

        private void InitBP(FuncBlock fblock)
        {
            fblock.IsBreakpoint = false;
            if (fblock is FuncBlock_ForHeader)
            {
                FuncBlock_ForHeader fblockfh = (FuncBlock_ForHeader)fblock;
                InitBP(fblockfh.Start);
                InitBP(fblockfh.Cond);
                InitBP(fblockfh.Next);
            }
            if (fblock is FuncBlock_WhileHeader)
            {
                FuncBlock_WhileHeader fblockwh = (FuncBlock_WhileHeader)fblock;
                InitBP(fblockwh.Cond);
            }
            if (fblock is FuncBlock_IfHeader)
            {
                FuncBlock_IfHeader fblockih = (FuncBlock_IfHeader)fblock;
                InitBP(fblockih.Cond);
            }
            if (fblock is FuncBlock_WhileEnd)
            {
                FuncBlock_WhileEnd fblockwe = (FuncBlock_WhileEnd)fblock;
                InitBP(fblockwe.Cond);
            }
            foreach (FuncBlock child in fblock.Childrens)
            {
                InitBP(child);
            }
        }

        public void GetBPAddrRange()
        {
            bpaddrmin = 0x3fffffff;
            bpaddrmax = -0x3fffffff;
            GetBPAddrRange(Root);
        }

        private void GetBPAddrRange(FuncBlock fblock)
        {
            if (fblock.IsBreakpoint)
            {
                bpaddrmin = Math.Max(bpaddrmin, fblock.BPAddress);
                bpaddrmax = Math.Max(bpaddrmax, fblock.BPAddress);
            }
            if (fblock is FuncBlock_ForHeader)
            {
                FuncBlock_ForHeader fblockfh = (FuncBlock_ForHeader)fblock;
                GetBPAddrRange(fblockfh.Start);
                GetBPAddrRange(fblockfh.Cond);
                GetBPAddrRange(fblockfh.Next);
            }
            if (fblock is FuncBlock_WhileHeader)
            {
                FuncBlock_WhileHeader fblockwh = (FuncBlock_WhileHeader)fblock;
                GetBPAddrRange(fblockwh.Cond);
            }
            if (fblock is FuncBlock_IfHeader)
            {
                FuncBlock_IfHeader fblockih = (FuncBlock_IfHeader)fblock;
                GetBPAddrRange(fblockih.Cond);
            }
            if (fblock is FuncBlock_WhileEnd)
            {
                FuncBlock_WhileEnd fblockwe = (FuncBlock_WhileEnd)fblock;
                GetBPAddrRange(fblockwe.Cond);
            }
            foreach (FuncBlock child in fblock.Childrens)
            {
                GetBPAddrRange(child);
            }
        }

        public bool ContainBP(int bpaddr)
        {
            return bpaddr >= bpaddrmin && bpaddr <= bpaddrmax;
        }

        #endregion
        
        #region Shell

        private FuncBlockViewModel view;
        public FuncBlockViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                FuncBlockViewModel _view = view;
                this.view = value;
                if (_view != null && _view.Core != null) _view.Core = null;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return view; }
            set { View = (FuncBlockViewModel)value; }
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
            xele.SetAttributeValue("Name", name);
            xele.Value = Code;
            path = ptvitem == null ? null : ptvitem.Path;
            if (path != null) xele.SetAttributeValue("Path", path);
        }

        public void Load(XElement xele)
        {
            name = xele.Attribute("Name").Value;
            Code = xele.Value;
            XAttribute xatt = xele.Attribute("Path");
            path = xele != null ? xele.Value : null;
        }

        #endregion
    }
}
