using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace SamSoarII.Core.Simulate
{
    public class BreakpointManager
    {
        public BreakpointManager(SimulateManager _parent)
        {
            parent = _parent;
            parent.PropertyChanged += OnParentPropertyChanged;
            items = new ObservableCollection<IBreakpoint>();
            enableitems = new ObservableCollection<IBreakpoint>();
            activeitems = new ObservableCollection<IBreakpoint>();
            items.CollectionChanged += OnItemsChanged;
            enableitems.CollectionChanged += OnEnableItemsChanged;
            activeitems.CollectionChanged += OnActiveItemsChanged;
        }
        
        #region Number

        private SimulateManager parent;
        public SimulateManager Parent { get { return this.parent; } }
        public InteractionFacade IFParent { get { return parent.IFParent; } }

        private ObservableCollection<IBreakpoint> items;
        public IList<IBreakpoint> Items { get { return this.items; } }
        public event NotifyCollectionChangedEventHandler ItemsChanged = delegate { };
        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (IBreakpoint ibrpo in e.NewItems)
                    ibrpo.PropertyChanged += OnBrpoPropertyChanged;
            if (e.OldItems != null)
                foreach (IBreakpoint ibrpo in e.OldItems)
                    ibrpo.PropertyChanged -= OnBrpoPropertyChanged;
            ItemsChanged(this, e);
        }

        private ObservableCollection<IBreakpoint> enableitems;
        //private IBreakpoint[] enableitemcache;
        private bool enableitemlock;
        public IList<IBreakpoint> EnableItems { get { return this.items; } }
        public event NotifyCollectionChangedEventHandler EnableItemsChanged = delegate { };
        private void OnEnableItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            enableitemlock = true;
            if (e.NewItems != null)
                foreach (IBreakpoint ibrpo in e.NewItems)
                    ibrpo.IsEnable = true;
            if (e.OldItems != null)
                foreach (IBreakpoint ibrpo in e.OldItems)
                    ibrpo.IsEnable = false;
            enableitemlock = false;
            EnableItemsChanged(this, e);
        }

        private ObservableCollection<IBreakpoint> activeitems;
        //private IBreakpoint[] activeitemcache;
        private bool activeitemlock;
        public IList<IBreakpoint> ActiveItems { get { return this.items; } }
        public event NotifyCollectionChangedEventHandler ActiveItemsChanged = delegate { };
        private void OnActiveItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            activeitemlock = true;
            if (e.NewItems != null)
                foreach (IBreakpoint ibrpo in e.NewItems)
                {
                    ibrpo.IsActive = true;
                    Active(ibrpo);
                }
            if (e.OldItems != null)
                foreach (IBreakpoint ibrpo in e.OldItems)
                {
                    ibrpo.IsActive = false;
                    Unactive(ibrpo);
                }
            activeitemlock = false;
            ActiveItemsChanged(this, e);
        }
        
        #endregion

        #region Modify

        private void Active(IBreakpoint ibrpo)
        {
            if (SimulateDllModel.IsDllAlive() == 0) return;
            if (ibrpo is LadderBrpoModel)
            {
                LadderBrpoModel lbrpo = (LadderBrpoModel)ibrpo;
                SimulateDllModel.SetBPCount(lbrpo.Address, lbrpo.SkipCount);
                switch (lbrpo.Condition)
                {
                    case LadderBrpoModel.Conditions.NONE:
                        SimulateDllModel.SetBPAddr(lbrpo.Address, 1);
                        SimulateDllModel.SetCPAddr(lbrpo.Address, 0);
                        break;
                    case LadderBrpoModel.Conditions.OFF:
                        SimulateDllModel.SetBPAddr(lbrpo.Address, 0);
                        SimulateDllModel.SetCPAddr(lbrpo.Address, 1);
                        break;
                    case LadderBrpoModel.Conditions.ON:
                        SimulateDllModel.SetBPAddr(lbrpo.Address, 0);
                        SimulateDllModel.SetCPAddr(lbrpo.Address, 2);
                        break;
                    case LadderBrpoModel.Conditions.UPEDGE:
                        SimulateDllModel.SetBPAddr(lbrpo.Address, 0);
                        SimulateDllModel.SetCPAddr(lbrpo.Address, 4);
                        break;
                    case LadderBrpoModel.Conditions.DOWNEDGE:
                        SimulateDllModel.SetBPAddr(lbrpo.Address, 0);
                        SimulateDllModel.SetCPAddr(lbrpo.Address, 8);
                        break;
                    case LadderBrpoModel.Conditions.EDGE:
                        SimulateDllModel.SetBPAddr(lbrpo.Address, 0);
                        SimulateDllModel.SetCPAddr(lbrpo.Address, 12);
                        break;
                }
            }
            else
            {
                SimulateDllModel.SetBPAddr(ibrpo.Address, 1);
            }
        }

        private void Unactive(IBreakpoint ibrpo)
        {
            if (SimulateDllModel.IsDllAlive() == 0) return;
            SimulateDllModel.SetBPAddr(ibrpo.Address, 0);
            if (ibrpo is LadderBrpoModel)
                SimulateDllModel.SetCPAddr(ibrpo.Address, 0);
        }
        
        public void Initialize()
        {
            foreach (IBreakpoint ibrpo in items)
                ibrpo.PropertyChanged -= OnBrpoPropertyChanged;
            items.Clear();
            foreach (IBreakpoint ibrpo in activeitems.ToArray())
                activeitems.Remove(ibrpo);
            foreach (IBreakpoint ibrpo in enableitems.ToArray())
                enableitems.Remove(ibrpo);
            if (IFParent.MDProj == null) return;
            foreach (LadderDiagramModel diagram in IFParent.MDProj.Diagrams)
                foreach (LadderNetworkModel network in diagram.Children)
                    foreach (LadderUnitModel unit in network.Children)
                    {
                        if (unit.Breakpoint == null) continue;
                        items.Add(unit.Breakpoint);
                        unit.Breakpoint.Address = items.Count() - 1;
                        if (unit.Breakpoint.IsEnable)
                        {
                            enableitems.Add(unit.Breakpoint);
                            if (unit.Breakpoint.IsActive)
                                activeitems.Add(unit.Breakpoint);
                        }
                    }
            foreach (FuncBlockModel funcblock in IFParent.MDProj.FuncBlocks)
            {
                if (funcblock.View != null)
                    funcblock.Code = funcblock.View.Code;
                funcblock.BuildAll(funcblock.Code);
                Initialize(funcblock.Root);    
            }
        }

        private void Initialize(FuncBlock fblock)
        {
            if (fblock is FuncBlock_Statement
                 || ((fblock is FuncBlock_Assignment || fblock is FuncBlock_AssignmentSeries)
                    && !(fblock.Parent is FuncBlock_Root)))
            {
                fblock.Breakpoint = new FuncBrpoModel(fblock);
                items.Add(fblock.Breakpoint);
                fblock.Breakpoint.Address = items.Count() - 1;
            }
            foreach (FuncBlock sub in fblock.Childrens)
                Initialize(sub);
        }
        
        #endregion

        #region Event Handler
        
        private void OnBrpoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IBreakpoint ibrpo = (IBreakpoint)sender;
            switch (e.PropertyName)
            {
                case "IsEnable":
                    if (enableitemlock) break;
                    if (ibrpo.IsEnable)
                        enableitems.Add(ibrpo);
                    else
                        enableitems.Remove(ibrpo);
                    break;
                case "IsActive":
                    if (activeitemlock) break;
                    if (ibrpo.IsActive)
                        activeitems.Add(ibrpo);
                    else
                        activeitems.Remove(ibrpo);
                    break;
                default:
                    if (ibrpo.IsActive) Active(ibrpo);
                    break;
            }
        }


        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnable":
                    if (parent.IsEnable)
                        foreach (IBreakpoint ibrpo in activeitems)
                            Active(ibrpo);
                    break;
            }
        }

        #endregion
    }
}
