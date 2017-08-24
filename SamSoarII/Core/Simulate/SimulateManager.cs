using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Core.Simulate
{
    public class SimulateManager : IThreadManager, INotifyPropertyChanged
    {
        public SimulateManager(InteractionFacade _ifParent)
        {
            ifParent = _ifParent;
            dllmodel = new SimulateDllModel(this);
            dllmodel.Started += OnStarted;
            dllmodel.Paused += OnPaused;
            dllmodel.Aborted += OnAborted;
            viewer = new SimulateViewer(this);
            //viewer.Started += OnStarted;
            //viewer.Paused += OnPaused;
            //viewer.Aborted += OnAborted;
            viewer.BreakpointPaused += OnBreakpointPaused;
            viewer.BreakpointResumed += OnBreakpointResumed;
            mngBrpo = new BreakpointManager(this);
            mngVBrpo = new ValueBrpoManager(this);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        
        private SimulateDllModel dllmodel;
        public SimulateDllModel DllModel { get { return this.dllmodel; } }

        private SimulateViewer viewer;
        public SimulateViewer Viewer { get { return this.viewer; } }

        private BreakpointManager mngBrpo;
        public BreakpointManager MNGBrpo { get { return this.mngBrpo; } }

        private ValueBrpoManager mngVBrpo;
        public ValueBrpoManager MNGVBrpo { get { return this.mngVBrpo; } }

        private bool isenable;
        public bool IsEnable
        {
            get
            {
                return this.isenable;
            }
            set
            {
                this.isenable = value;
                viewer.IsEnable = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsEnable"));
            }
        }
        
        #endregion
        
        #region Thread

        public bool IsAlive { get { return dllmodel.IsAlive; } }
        public bool IsActive { get { return dllmodel.IsActive; } }
        
        public event RoutedEventHandler Started = delegate { };
        public event RoutedEventHandler Paused = delegate { };
        public event RoutedEventHandler Aborted = delegate { };

        public void Abort()
        {
            if (viewer.IsBPPause) viewer.Resume();
            viewer.Abort();
            //mngVBrpo.Abort();
            dllmodel.Abort();
        }

        public void Pause()
        {
            dllmodel.Pause();
            viewer.Pause();
        }

        public void Start()
        {
            if (viewer.IsBPPause) viewer.Resume();
            dllmodel.Start();
            viewer.Start();
            //mngVBrpo.Start();
        }
        
        private void OnStarted(object sender, RoutedEventArgs e)
        {
            Started(this, new RoutedEventArgs());
        }

        private void OnPaused(object sender, RoutedEventArgs e)
        {
            Paused(this, new RoutedEventArgs());
        }

        private void OnAborted(object sender, RoutedEventArgs e)
        {
            Aborted(this, new RoutedEventArgs());
        }

        #endregion

        #region Breakpoint

        public bool IsBPPause { get { return viewer.IsBPPause; } }

        public event BreakpointPauseEventHandler BreakpointPaused = delegate { };
        private void OnBreakpointPaused(object sender, BreakpointPauseEventArgs e)
        {
            BreakpointPaused(this, e);
        }

        public event BreakpointPauseEventHandler BreakpointResumed = delegate { };
        private void OnBreakpointResumed(object sender, BreakpointPauseEventArgs e)
        {
            BreakpointResumed(this, e);
        }

        public void MoveStep()
        {
            viewer.MoveStep();
            if (!IsAlive) Start();
        }

        public void CallStep()
        {
            viewer.CallStep();
            if (!IsAlive) Start();
        }

        public void JumpOut()
        {
            viewer.JumpOut();
            if (!IsAlive) Start();
        }
        
        public void JumpTo(int bpaddr)
        {
            viewer.JumpTo(bpaddr);
            if (!IsAlive) Start();
        }

        #endregion

    }
}
