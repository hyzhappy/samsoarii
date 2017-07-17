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
    public class ProjectViewModel : ILoadModel, INotifyPropertyChanged
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

        #region Load

        public bool IsFullLoaded
        {
            get
            {
                foreach (LadderDiagramModel ldmodel in Core.Diagrams)
                {
                    if (ldmodel.View == null) return false;
                    if (ldmodel.Inst.View == null) return false;
                }
                return true;
            }
        }

        public ViewThreadManager ViewThread
        {
            get { return Core.Parent.ThMNGView; }
        }

        public IEnumerable<ILoadModel> LoadChildren
        {
            get
            {
                foreach (LadderDiagramModel ldmodel in Core.Diagrams)
                {
                    if (ldmodel.View != null && !ldmodel.View.IsFullLoaded) yield return ldmodel.View;
                    if (ldmodel.Inst.View != null && !ldmodel.Inst.View.IsFullLoaded) yield return ldmodel.Inst.View;
                }
                /*
                foreach (FuncBlockModel fbmodel in Core.FuncBlocks)
                {
                    if (fbmodel.View != null && !fbmodel.View.IsFullLoaded) yield return fbmodel.View;
                }
                */
            }
        }

        public void UpdateFullLoadProgress() { }

        #endregion

        public void FullLoad()
        {
            foreach (LadderDiagramModel ldmodel in Core.Diagrams)
            {
                if (ldmodel.View == null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        ldmodel.View = new LadderDiagramViewModel(ldmodel);
                    });
                }
                if (ldmodel.Inst.View == null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        ldmodel.Inst.View = new InstructionDiagramViewModel(ldmodel.Inst);
                    });
                }
            }
            /*
            foreach (FuncBlockModel fbmodel in Core.FuncBlocks)
            {
                if (fbmodel.View == null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        fbmodel.View = new FuncBlockViewModel(fbmodel, IFParent.TCMain);
                    });
                }
            }
            */
        }

        public void Update() { }

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
                foreach (LadderDiagramModel ldmodel in Core.Diagrams)
                {
                    if (ldmodel.View != null)
                        ldmodel.View.LadderMode = laddermode;
                }
                foreach (FuncBlockModel fbmodel in Core.FuncBlocks)
                {
                    if (fbmodel.View != null)
                        fbmodel.View.LadderMode = laddermode;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("LadderMode"));
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
                foreach (LadderDiagramModel ldmodel in Core.Diagrams)
                {
                    if (ldmodel.View != null)
                        ldmodel.View.IsCommentMode = iscommentmode;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode"));
            }
        }
        
        #endregion

        #region Event Handler

        private void OnCoreDiagramChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LadderDiagramModel diagram = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    diagram = (LadderDiagramModel)(e.NewItems[0]);
                    if (diagram.View == null)
                    {
                        diagram.View = new LadderDiagramViewModel(diagram);
                        ViewThread.Add(diagram.View);
                    }
                    break;
            }
        }

        private void OnCoreFuncBlockChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FuncBlockModel funcblock = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    funcblock = (FuncBlockModel)(e.NewItems[0]);
                    if (funcblock.View == null)
                    {
                        funcblock.View = new FuncBlockViewModel(funcblock, IFParent.TCMain);
                        ViewThread.Add(funcblock.View);
                    }
                    break;
            }
        }

        #endregion
    }
}
