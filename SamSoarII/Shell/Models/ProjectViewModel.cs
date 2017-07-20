using SamSoarII.Core.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;
using System.Threading;

namespace SamSoarII.Shell.Models
{
    public class ProjectViewModel : IViewModel, INotifyPropertyChanged
    {
        public ProjectViewModel(ProjectModel _core)
        {
            Core = _core;
            Core.MainDiagram.Tab = new MainTabDiagramItem(Core.Parent.TCMain, Core.MainDiagram, Core.MainDiagram.Inst);
            Core.Modbus.View = new ModbusTableViewModel(Core.Modbus, Core.Parent.TCMain);
        }
        
        public void Dispose()
        {
            Core = null;
        }
        
        public Dispatcher Dispatcher { get { return Application.Current.Dispatcher; } }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #region Core

        private ProjectModel core;
        public ProjectModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                ProjectModel _core = core;
                this.core = value;
                if (_core != null)
                {
                    _core.DiagramChanged -= OnCoreDiagramChanged;
                    _core.FuncBlockChanged -= OnCoreFuncBlockChanged;
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.DiagramChanged += OnCoreDiagramChanged;
                    core.FuncBlockChanged += OnCoreFuncBlockChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        
        IModel IViewModel.Core
        {
            get { return Core; }
            set { Core = (ProjectModel)value; }
        }

        #endregion

        #region Shell
        
        public InteractionFacade IFParent { get { return Core.Parent; } }

        public IViewModel ViewParent { get { return null; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }
        
        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get { return core.LadderMode; }
            set { core.LadderMode = value; }
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
                foreach (LadderDiagramModel ldmodel in Core.Diagrams)
                {
                    if (ldmodel.View != null)
                        ldmodel.View.IsCommentMode = iscommentmode;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode"));
            }
        }

        public void Reset()
        {
            foreach (LadderDiagramModel diagram in core.Diagrams)
            {
                diagram.View = null;
                foreach (LadderNetworkModel network in diagram.Children)
                {
                    network.View = null;
                    foreach (LadderUnitModel unit in network.Children)
                    {
                        unit.View = null;
                    }
                }
            }
        }

        public void UpdateUnit(int flags)
        {
            foreach (LadderDiagramModel diagram in core.Diagrams)
                foreach (LadderNetworkModel network in diagram.Children)
                    foreach (LadderUnitModel unit in network.Children)
                        if (unit.View != null) unit.View.Update(flags);
        }
        
        #endregion

        #region Event Handler

        private void OnCoreDiagramChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (LadderDiagramModel diagram in e.NewItems)
                {
                    if (diagram.View == null)
                        diagram.View = new LadderDiagramViewModel(diagram);
                    if (diagram.Inst?.View == null)
                        diagram.Inst.View = new InstructionDiagramViewModel(diagram.Inst);
                }
            if (e.OldItems != null)
                foreach (LadderDiagramModel diagram in e.OldItems)
                    if (diagram.Tab != null)
                    {
                        diagram.Tab.Dispose();
                        diagram.Tab = null;
                    }
        }

        private void OnCoreFuncBlockChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (FuncBlockModel funcblock in e.NewItems)
                    if (funcblock.View == null)
                        funcblock.View = new FuncBlockViewModel(funcblock, IFParent.TCMain);
            if (e.OldItems != null)
                foreach (FuncBlockModel funcblock in e.OldItems)
                    if (funcblock.View != null)
                    {
                        funcblock.View.Dispose();
                        funcblock.View = null;
                    }
        }

        #endregion
    }
}
