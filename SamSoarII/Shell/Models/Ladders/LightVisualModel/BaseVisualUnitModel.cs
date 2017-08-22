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
        }

        #endregion
        public BaseVisualUnitModel()
        {

        }

        #region Visuals
        private bool hasAdded = false;
        public bool HasAdded { get { return hasAdded; } set { hasAdded = value; } }

        private LadderDrawingVisual[] visuals = new LadderDrawingVisual[4];

        public LadderDrawingVisual[] Visuals { get { return visuals; } }
        protected void RenderUnit()
        {
            if (visuals[0] == null)
                visuals[0] = new LadderDrawingVisual(this, VisualType.Unit);
            visuals[0].Render();
        }

        protected void RenderProperty()
        {
            if (visuals[1] == null)
                visuals[1] = new LadderDrawingVisual(this, VisualType.Property);
            visuals[1].Render();
        }

        protected void RenderComment()
        {
            if (visuals[2] == null)
                visuals[2] = new LadderDrawingVisual(this, VisualType.Comment);
            visuals[2].Render();
        }
        protected void RenderBrpo()
        {
            if (visuals[3] == null)
                visuals[3] = new LadderDrawingVisual(this, VisualType.Brop);
            visuals[3].Render();
        }
        protected void RenderAll()
        {
            RenderUnit();
            if(!NoPropertyModel())
                RenderProperty();
            if (IsCommentMode) RenderComment();
        }

        private bool NoPropertyModel()
        {
            return core.Type == LadderUnitModel.Types.HLINE || core.Type == LadderUnitModel.Types.VLINE
                || core.Type == LadderUnitModel.Types.MEP || core.Type == LadderUnitModel.Types.MEF
                || core.Type == LadderUnitModel.Types.INV;
        }
        #endregion

        static public BaseVisualUnitModel Create(LadderUnitModel _core)
        {
            BaseVisualUnitModel ret = null;
            switch (_core.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    ret = AllResourceManager.CreateVisualInput(_core); break;
                case LadderUnitModel.Shapes.Output:
                    ret = AllResourceManager.CreateVisualOutput(_core); break;
                case LadderUnitModel.Shapes.OutputRect:
                    ret = AllResourceManager.CreateVisualOutRec(_core); break;
                case LadderUnitModel.Shapes.Special:
                    ret = AllResourceManager.CreateVisualSpecial(_core); break;
                case LadderUnitModel.Shapes.HLine:
                    ret = AllResourceManager.CreateVisualHLine(_core); break;
                case LadderUnitModel.Shapes.VLine:
                    ret = AllResourceManager.CreateVisualVLine(_core); break;
                default:
                    ret = null; break;
            }
            return ret;
        }

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
            for (int i = 0; i < visuals.Length; i++)
            {
                if (ViewParent.LadderCanvas.Contains(visuals[i]))
                    ViewParent.LadderCanvas.RemoveVisual(visuals[i]);
                if (visuals[i] != null)
                {
                    visuals[i]?.Dispose();
                    visuals[i] = null;
                }
            }
            Core.Visual = null;
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
                    RenderUnit();
                    RenderProperty();
                    RenderComment();
                    break;
                case RenderType.Opacity:
                    RenderAll();
                    break;
                case RenderType.Brpo:
                    RenderBrpo();
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
                    //RenderAll();
                    break;
                case LadderUnitAction.REMOVE:
                    Dispose();
                    break;
                case LadderUnitAction.MOVE:
                    //Update(RenderType.All);
                    break;
                case LadderUnitAction.UPDATE:
                    Update(RenderType.Property);
                    Update(RenderType.Comment);
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
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { Update(RenderType.Property); });
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
