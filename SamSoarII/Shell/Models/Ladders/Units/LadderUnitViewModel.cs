using SamSoarII.Core.Communication;
using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SamSoarII.Shell.Models
{
    public abstract class LadderUnitViewModel : UserControl, IViewModel, IResource
    {
        #region IResource

        public abstract IResource Create(params object[] args);

        public virtual void Recreate(params object[] args)
        {
            Core = (LadderUnitModel)args[0];
        }
        
        private bool isdisposed;
        public virtual bool IsDisposed
        {
            get { return this.isdisposed; }
            set { this.isdisposed = value; }
        }

        #endregion

        public LadderUnitViewModel()
        {

        }

        public virtual void Dispose()
        {
            Core = null;
        }

        static public LadderUnitViewModel Create(LadderUnitModel _core)
        {
            LadderUnitViewModel ret = null;
            switch (_core.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    ret = AllResourceManager.CreateInput(_core); break;
                case LadderUnitModel.Shapes.Output:
                    ret = AllResourceManager.CreateOutput(_core); break;
                case LadderUnitModel.Shapes.OutputRect:
                    ret = AllResourceManager.CreateOutRec(_core); break;
                case LadderUnitModel.Shapes.Special:
                    ret = AllResourceManager.CreateSpecial(_core); break;
                case LadderUnitModel.Shapes.HLine:
                    ret = AllResourceManager.CreateHLine(_core); break;
                case LadderUnitModel.Shapes.VLine:
                    ret = AllResourceManager.CreateVLine(_core); break;
                default:
                    ret = null; break;
            }
            if (ret != null && ret.Parent is Canvas)
            {
                ((Canvas)(ret.Parent)).Children.Remove(ret);
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
                if (core == value) return;
                LadderUnitModel _core = core;
                this.core = value;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.Changed -= OnCoreChanged;
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.Changed += OnCoreChanged;
                    if (core.View != this) core.View = this;
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

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "X": Update(UPDATE_LEFT); break;
                case "Y": Update(UPDATE_TOP); break;
            }
        }
        
        private void OnCoreChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            if (Core != sender) return;
            switch (e.Action)
            {
                case LadderUnitAction.ADD:
                    if (ViewParent != null)
                        ViewParent.LadderCanvas.Children.Add(this);
                    Update(UPDATE_PROPERTY);
                    break;
                case LadderUnitAction.REMOVE:
                    if (ViewParent != null)
                        ViewParent.LadderCanvas.Children.Remove(this);
                    break;
                case LadderUnitAction.MOVE:
                    //Update(UPDATE_TOP | UPDATE_LEFT);
                    break;
                case LadderUnitAction.UPDATE:
                    Update(UPDATE_PROPERTY);
                    break;
            }
        }

        public int X { get { return core.X; } }

        public int Y { get { return core.Y; } }

        #endregion

        #region Shell
        
        public LadderNetworkViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        private LadderModes laddermode;
        public virtual LadderModes LadderMode
        {
            get
            {
                return this.laddermode;
            }
            set
            {
                LadderModes _laddermode = laddermode;
                this.laddermode = value;
                if (_laddermode != LadderModes.Edit)
                {
                    if (_laddermode == LadderModes.Simulate)
                    {
                        MNGSimu.Started -= OnSimulateStarted;
                        MNGSimu.Aborted -= OnSimulateAborted;
                    }
                    if (laddermode == LadderModes.Monitor)
                    {
                        MNGComu.Started -= OnMonitorStarted;
                        MNGComu.Aborted -= OnMonitorAborted;
                    }
                    foreach (ValueModel vmodel in Core.Children)
                        vmodel.Store.PropertyChanged -= OnValueStorePropertyChanged;
                }
                if (laddermode != LadderModes.Edit)
                {
                    if (laddermode == LadderModes.Simulate)
                    {
                        MNGSimu.Started += OnSimulateStarted;
                        MNGSimu.Aborted += OnSimulateAborted;
                    }
                    if (laddermode == LadderModes.Monitor)
                    {
                        MNGComu.Started += OnMonitorStarted;
                        MNGComu.Aborted += OnMonitorAborted;
                    }
                    foreach (ValueModel vmodel in Core.Children)
                        vmodel.Store.PropertyChanged += OnValueStorePropertyChanged;
                }
                Update(UPDATE_PROPERTY);
            }
        }
        
        private bool iscommentmode;
        public virtual bool IsCommentMode
        {
            get
            {
                return this.iscommentmode;
            }
            set
            {
                this.iscommentmode = value;
                Update(UPDATE_TOP | UPDATE_HEIGHT);
            }
        }

        public const int UPDATE_ALL = 0xff;
        public const int UPDATE_TOP = 0x01;
        public const int UPDATE_LEFT = 0x02;
        public const int UPDATE_WIDTH = 0x04;
        public const int UPDATE_HEIGHT = 0x08;
        public const int UPDATE_PROPERTY = 0x10;
        public const int UPDATE_STYLE = 0x20;
        public virtual void Update(int flags = UPDATE_ALL)
        {
            switch (flags)
            {
                case 0:
                    break;
                case UPDATE_TOP:
                    Canvas.SetTop(this, Y * (iscommentmode ? 500 : 300));
                    break;
                case UPDATE_LEFT:
                    Canvas.SetLeft(this, X * 300);
                    break;
                case UPDATE_WIDTH:
                    Width = 300;
                    break;
                case UPDATE_HEIGHT:
                    Height = (iscommentmode ? 500 : 300);
                    break;
                case UPDATE_PROPERTY:
                    break;
                case UPDATE_STYLE:
                    break;
                default:
                    Update(flags & UPDATE_TOP);
                    Update(flags & UPDATE_LEFT);
                    Update(flags & UPDATE_WIDTH);
                    Update(flags & UPDATE_HEIGHT);
                    Update(flags & UPDATE_PROPERTY);
                    Update(flags & UPDATE_STYLE);
                    break;
            }
        }
        
        #endregion

        #region Event Handler

        private void OnValueStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { Update(UPDATE_PROPERTY); });
        }
        
        private void OnSimulateStarted(object sender, RoutedEventArgs e)
        {
            Update(UPDATE_PROPERTY);
        }

        private void OnSimulateAborted(object sender, RoutedEventArgs e)
        {
            Update(UPDATE_PROPERTY);
        }

        private void OnMonitorStarted(object sender, RoutedEventArgs e)
        {
            Update(UPDATE_PROPERTY);
        }

        private void OnMonitorAborted(object sender, RoutedEventArgs e)
        {
            Update(UPDATE_PROPERTY);
        }

        #endregion
    }
}
