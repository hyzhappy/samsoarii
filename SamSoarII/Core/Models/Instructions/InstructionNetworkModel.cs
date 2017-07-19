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
            ismodify = true;
        }

        public void Dispose()
        {
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
                    _parent.ChildrenChanged -= OnParentChildrenChanged;
                    if (_parent.Inst != null) _parent.Inst = null;
                }
                if (parent != null)
                {
                    parent.PropertyChanged += OnParentPropertyChanged;
                    parent.ChildrenChanged += OnParentChildrenChanged;
                    if (parent.Inst != this) parent.Inst = this;
                }
            }
        }
        IModel IModel.Parent { get { return Parent; } }

        public int ID { get { return parent != null ? parent.ID : 0; } }
        public bool IsMasked { get { return parent != null ? parent.IsMasked : false; } }

        private bool ismodify;
        public bool IsModify { get { return this.ismodify; } }

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
            ismodify = true;
        }

        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
        
        public void Update()
        {
            ismodify = false;
            Compile();
            if (View != null) View.BaseUpdate();
        }

        private void Compile()
        {
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
            insts.Clear();
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
