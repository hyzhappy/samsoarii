using ICSharpCode.AvalonEdit.Document;
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
            istranslated = false;
            BuildAll(code);
        }

        public void Dispose()
        {
            Root.Dispose();
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
            set { this.name = value; InvokePropertyChanged("Name"); }
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
            set { this.code = value; InvokePropertyChanged("Code"); }
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

        #region Translate

        private bool istranslated;
        public bool IsTranslated { get { return this.istranslated; } }

        public void Translate()
        {
            if (istranslated) return;
            istranslated = true;
            code = code.Replace("WORD", "WORD*");
            code = code.Replace("BIT", "BIT*");
            BuildAll(code);
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

        static private string[] registernames = {
            "XBit", "YBit", "SBit", "MBit", "CBit", "TBit",
            "DWord", "VWord", "ZWord", "TVWord", "CVWord", "CVDoubleWord", "AIWord", "AOWord"};
        public List<string> GetCodeCompleteNames(string profix)
        {
            List<string> ret = new List<string>();
            ret.AddRange(registernames.Where(rn => rn.Contains(profix)));
            if (Current != null && Current is FuncBlock_Comment)
            {
                return ret;
            }
            FuncBlock _current = Current;
            while (_current != null)
            {
                if (_current is FuncBlock_Local)
                {
                    FuncBlock_Local fblocal = (FuncBlock_Local)_current;
                    if (fblocal.VirtualAssigns != null)
                    {
                        foreach (FuncBlock_Assignment fbassign in fblocal.VirtualAssigns)
                            if (fbassign.IndexEnd < Current.IndexStart && fbassign.Name.Contains(profix))
                                ret.Add(fbassign.Name);
                    }
                }
                foreach (FuncBlock subblock in _current.Childrens)
                {
                    if (subblock is FuncBlock_Assignment)
                    {
                        FuncBlock_Assignment fbassign = (FuncBlock_Assignment)subblock;
                        if (fbassign.IndexEnd < Current.IndexStart && fbassign.Name.Contains(profix))
                            ret.Add(fbassign.Name);
                    }
                    if (subblock is FuncBlock_AssignmentSeries)
                    {
                        FuncBlock_AssignmentSeries fbassser = (FuncBlock_AssignmentSeries)subblock;
                        foreach (FuncBlock_Assignment fbassign in fbassser.Defines)
                            if (fbassign.IndexEnd < Current.IndexStart && fbassign.Name.Contains(profix))
                                ret.Add(fbassign.Name);
                    }
                }
                _current = _current.Parent;
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
            Root?.Dispose();
            Root = new FuncBlock_Root(this, code);
            this.current = new LinkedListNode<FuncBlock>(Root);
            //Root.Build(text);   
            InvokePropertyChanged("Funcs");
            //Parent.InvokeModify(this);
        }

        #endregion

        #region Breakpoint System

        public event PropertyChangedEventHandler BreakpointPropertyChanged = delegate { };

        public void InvokeBreakpointPropertyChanged(FuncBlock fblock, string pname)
        {
            BreakpointPropertyChanged(fblock, new PropertyChangedEventArgs(pname));
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
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
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

        public event PropertyChangedEventHandler ViewPropertyChanged = delegate { };

        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get { return this.laddermode; }
            set { this.laddermode = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("LadderMode")); }
        }

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Name", name);
            xele.Value = view != null ? view.Code : Code;
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
