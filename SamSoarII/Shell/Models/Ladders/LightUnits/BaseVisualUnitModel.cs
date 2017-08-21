using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
using SamSoarII.Core.Communication;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using SamSoarII.Utility;

namespace SamSoarII.Shell.Models
{
    public abstract class BaseVisualUnitModel : IViewModel, IResource
    {
        #region IResource

        private int resourceid;
        public int ResourceID
        {
            get { return this.resourceid; }
            set { this.resourceid = value; }
        }

        public abstract IResource Create(params object[] args);

        protected bool recreating = false;
        public virtual void Recreate(params object[] args)
        {
            Core = (LadderUnitModel)args[0];
            oldladdermode = Core.LadderMode;
            if (LadderMode != LadderModes.Edit)
            {
                if (LadderMode == LadderModes.Simulate)
                {
                    MNGSimu.Started += OnSimulateStarted;
                    MNGSimu.Aborted += OnSimulateAborted;
                }
                if (LadderMode == LadderModes.Monitor)
                {
                    MNGComu.Started += OnMonitorStarted;
                    MNGComu.Aborted += OnMonitorAborted;
                }
                foreach (ValueModel vmodel in Core.Children)
                    if (vmodel.Store != null)
                        vmodel.Store.PropertyChanged += OnValueStorePropertyChanged;
            }

            Visuals[0] = new LadderUnitDrawingVisual(this, VisualType.Unit);
            Visuals[1] = new LadderUnitDrawingVisual(this, VisualType.Property);
            if (IsCommentMode) Visuals[2] = new LadderUnitDrawingVisual(this, VisualType.Comment);
            else Visuals[2] = null;
        }

        #endregion
        public BaseVisualUnitModel()
        {

        }

        #region Visuals

        private IRenderModel[] visuals = new IRenderModel[4];

        public IRenderModel[] Visuals { get { return visuals; } }
        protected void RenderUnit()
        {
            visuals[0]?.Render();
        }

        protected void RenderProperty()
        {
            visuals[1]?.Render();
        }

        protected void RenderComment()
        {
            visuals[2]?.Render();
        }
        protected void RenderBrpo()
        {
            visuals[3]?.Render();
        }
        protected void RenderAll()
        {
            RenderUnit();
            RenderProperty();
            if (IsCommentMode) RenderComment();
        }
        #endregion

        #region Core
        private LadderUnitModel core;
        public virtual LadderUnitModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                LadderUnitModel _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ViewPropertyChanged -= OnCorePropertyChanged;
                    _core.Changed -= OnCoreChanged;
                    if (_core.Visual != null) _core.Visual = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.ViewPropertyChanged += OnCorePropertyChanged;
                    core.Changed += OnCoreChanged;
                    if (core.Visual != this) core.Visual = this;
                }
            }
        }
        public SimulateManager MNGSimu { get { return Core.IFParent.MNGSimu; } }
        public CommunicationManager MNGComu { get { return Core.IFParent.MNGComu; } }

        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (LadderUnitModel)value; }
        }
        
        public virtual void Dispose()
        {
            if (LadderMode != LadderModes.Edit)
            {
                if (LadderMode == LadderModes.Simulate)
                {
                    MNGSimu.Started -= OnSimulateStarted;
                    MNGSimu.Aborted -= OnSimulateAborted;
                }
                if (LadderMode == LadderModes.Monitor)
                {
                    MNGComu.Started -= OnMonitorStarted;
                    MNGComu.Aborted -= OnMonitorAborted;
                }
                foreach (ValueModel vmodel in Core.Children)
                    if (vmodel.Store != null)
                        vmodel.Store.PropertyChanged -= OnValueStorePropertyChanged;
            }
            Core = null;
        }

        public virtual void Update(RenderType type)
        {
            switch (type)
            {
                case RenderType.All:
                    RenderAll();
                    break;
                case RenderType.Unit:
                    RenderUnit();
                    break;
                case RenderType.Property:
                    RenderProperty();
                    break;
                case RenderType.Comment:
                    RenderComment();
                    break;
                case RenderType.Opacity:
                    break;
                case RenderType.Brpo:
                    break;
                case RenderType.State:
                    RenderProperty();
                    break;
                default:
                    break;
            }
        }

        private void OnCoreChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            if (Core != sender) return;
            switch (e.Action)
            {
                case LadderUnitAction.ADD:
                    //if (ViewParent != null)
                    //    ViewParent.LadderCanvas.Children.Add(this);
                    //Update(UPDATE_PROPERTY);
                    break;
                case LadderUnitAction.REMOVE:
                    //if (ViewParent != null)
                    //    ViewParent.LadderCanvas.Children.Remove(this);
                    break;
                case LadderUnitAction.MOVE:
                    //Update(UPDATE_TOP | UPDATE_LEFT);
                    break;
                case LadderUnitAction.UPDATE:
                    //Update(UPDATE_PROPERTY);
                    break;
            }
        }

        protected virtual void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "X": Update(RenderType.All); break;
                case "Y": Update(RenderType.All); break;
                case "IsUsed": Update(RenderType.Opacity); break;
                case "IsCommentMode": Update(RenderType.Comment); break;
                case "LadderMode":
                    if (oldladdermode != LadderModes.Edit)
                    {
                        if (oldladdermode == LadderModes.Simulate)
                        {
                            MNGSimu.Started -= OnSimulateStarted;
                            MNGSimu.Aborted -= OnSimulateAborted;
                        }
                        if (oldladdermode == LadderModes.Monitor)
                        {
                            MNGComu.Started -= OnMonitorStarted;
                            MNGComu.Aborted -= OnMonitorAborted;
                        }
                        foreach (ValueModel vmodel in Core.Children)
                            if (vmodel.Store != null)
                                vmodel.Store.PropertyChanged -= OnValueStorePropertyChanged;
                    }
                    if (LadderMode != LadderModes.Edit)
                    {
                        if (LadderMode == LadderModes.Simulate)
                        {
                            MNGSimu.Started += OnSimulateStarted;
                            MNGSimu.Aborted += OnSimulateAborted;
                        }
                        if (LadderMode == LadderModes.Monitor)
                        {
                            MNGComu.Started += OnMonitorStarted;
                            MNGComu.Aborted += OnMonitorAborted;
                        }
                        foreach (ValueModel vmodel in Core.Children)
                            if (vmodel.Store != null)
                                vmodel.Store.PropertyChanged += OnValueStorePropertyChanged;
                    }
                    Update(RenderType.State);
                    oldladdermode = LadderMode;
                    break;
                case "BPEnable":
                case "BPCursor":
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { Update(RenderType.Brpo); });
                    break;
            }
        }
        #endregion

        #region Shell
        public LadderNetworkViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        public int X { get { return core.X; } }
        public int Y { get { return core.Y; } }

        public double Opacity { get { return core.IsUsed ? 1 : 0.3; } }

        private LadderModes oldladdermode;
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }
        
        private void OnValueStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () {  });
        }

        private void OnSimulateStarted(object sender, RoutedEventArgs e)
        {
            Update(RenderType.State);
        }

        private void OnSimulateAborted(object sender, RoutedEventArgs e)
        {
            Update(RenderType.State);
        }

        private void OnMonitorStarted(object sender, RoutedEventArgs e)
        {
            Update(RenderType.State);
        }

        private void OnMonitorAborted(object sender, RoutedEventArgs e)
        {
            Update(RenderType.State);
        }
        #endregion
    }
    public enum RenderType
    {
        All,
        Unit,
        Property,
        Comment,
        Opacity,
        Brpo,
        State
    }
}
