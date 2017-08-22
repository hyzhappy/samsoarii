using SamSoarII.Core.Generate;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class InstructionNetworkModel : IModel
    {
        public InstructionNetworkModel(LadderNetworkModel _parent)
        {
            Parent = _parent;
            isexpand = true;
            IsModified = false;
        }

        public void Dispose()
        {
            if (View != null) View.Dispose();
            if (lchart != null)
            {
                lchart.Dispose();
                lchart = null;
            }
            if (lgraph != null)
            {
                lgraph.Dispose();
                lgraph = null;
            }
            foreach (PLCOriginInst inst in insts)
                inst.Inst = null;
            Parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderNetworkModel parent;
        public LadderNetworkModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                LadderNetworkModel _parent = parent;
                this.parent = value;
                if (_parent != null)
                {
                    _parent.PropertyChanged -= OnParentPropertyChanged;
                    _parent.ViewPropertyChanged -= OnParentPropertyChanged;
                    _parent.ChildrenChanged -= OnParentChildrenChanged;
                    if (_parent.Inst != null) _parent.Inst = null;
                }
                if (parent != null)
                {
                    parent.PropertyChanged += OnParentPropertyChanged;
                    parent.ViewPropertyChanged += OnParentPropertyChanged;
                    parent.ChildrenChanged += OnParentChildrenChanged;
                    if (parent.Inst != this) parent.Inst = this;
                    Compile();
                }
            }
        }
        IModel IModel.Parent { get { return Parent; } }

        public int ID { get { return parent != null ? parent.ID : 0; } }
        public bool IsMasked { get { return parent != null ? parent.IsMasked : false; } }

        private bool ismodified;
        public bool IsModified
        {
            get { return this.ismodified; }
            private set { this.ismodified = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("IsModified")); }
        }
        
        #endregion
        
        #region Shell

        private InstructionNetworkViewModel view;
        public InstructionNetworkViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                InstructionNetworkViewModel _view = view;
                this.view = value;
                if (_view != null && _view.Core != null) _view.Core = null;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return view; }
            set { View = (InstructionNetworkViewModel)value; }
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

        private bool iscommentmode;
        public bool IsCommentMode
        {
            get { return this.iscommentmode; }
            set { this.iscommentmode = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode")); }
        }
        
        private bool isexpand;
        public bool IsExpand
        {
            get { return this.isexpand; }
            set { this.isexpand = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("IsExpand")); parent.Parent.Inst.UpdateCanvasTop(); }
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
        
        #endregion

        #region Save & Load

        public void Load(XElement xele)
        {
            throw new NotImplementedException();
        }

        public void Save(XElement xele)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event Handler

        private void OnParentChildrenChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            IsModified = true;
        }

        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsMasked": parent?.Parent?.Inst?.UpdateCanvasTop(); break;
            }
            PropertyChanged(this, e);
        }

        #endregion

        #region Inst

        private LadderChart lchart;
        private LadderGraph lgraph;
        private List<PLCOriginInst> insts = new List<PLCOriginInst>();
        public IList<PLCOriginInst> Insts { get { return this.insts; } }
        private bool isopencircuit;
        public bool IsOpenCircuit { get { return this.isopencircuit; } }
        private bool isshortcircuit;
        public bool IsShortCircuit { get { return this.isshortcircuit; } }
        private bool isfusioncircuit;
        public bool IsFusionCircuit { get { return this.isfusioncircuit; } }
        public bool Invalid { get { return IsMasked || IsOpenCircuit || IsShortCircuit || IsFusionCircuit || Insts.Count == 0; } }

        public void Update()
        {
            IsModified = false;
            Compile();
            View?.BaseUpdate();
            if (parent.Parent.Inst.View != null)
                parent.Parent.Inst.View.Dispatcher.Invoke(
                    DispatcherPriority.Normal, (ThreadStart)(delegate () { parent.Parent.Inst.UpdateCanvasTop(); }));
        }

        private void Compile()
        {
            insts.Clear();
            if (lchart != null)
            {
                lchart.Dispose();
                lchart = null;
            }
            if (lgraph != null)
            {
                lgraph.Dispose();
                lgraph = null;
            }
            lchart = new LadderChart(parent);
            isopencircuit = false;
            isshortcircuit = false;
            isfusioncircuit = false;
            if (lchart.CheckOpenCircuit())
            {
                isopencircuit = true;
                return;
            }
            lgraph = lchart.ToGraph();
            if (lgraph.CheckShortCircuit())
            {
                isshortcircuit = true;
                return;
            }
            if (lgraph.CheckFusionCircuit())
            {
                isfusioncircuit = true;
                return;
            }
            List<PLCInstruction> _insts = lgraph.GenInst();
            SortedSet<int> prototypeids = new SortedSet<int>();
            foreach (PLCInstruction inst in _insts)
            {
                insts.Add(inst.ToOrigin());
                if (inst.PrototypeID != -1)
                {
                    if (prototypeids.Contains(inst.PrototypeID))
                    {
                        isfusioncircuit = true;
                        return;
                    }
                    prototypeids.Add(inst.PrototypeID);
                }
            }
        }

        #endregion
    }
}
