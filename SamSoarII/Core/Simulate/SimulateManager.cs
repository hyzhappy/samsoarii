using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Core.Simulate
{
    public class SimulateManager : IThreadManager
    {
        public SimulateManager(InteractionFacade _ifParent)
        {
            ifParent = _ifParent;
            dllmodel = new SimulateDllModel(this);
            dllmodel.Started += OnStarted;
            dllmodel.Paused += OnPaused;
            dllmodel.Aborted += OnAborted;
            viewer = new SimulateViewer(this);
            viewer.Started += OnStarted;
            viewer.Paused += OnPaused;
            viewer.Aborted += OnAborted;
            viewer.BreakpointPaused += OnBreakpointPaused;
            viewer.BreakpointResumed += OnBreakpointResumed;
        }
        
        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        
        private SimulateDllModel dllmodel;
        public SimulateDllModel DllModel { get { return this.dllmodel; } }

        private SimulateViewer viewer;
        public SimulateViewer Viewer { get { return this.viewer; } }

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
            }
        }
        
        #endregion
        
        #region Thread

        public bool IsAlive { get { return dllmodel.IsAlive || viewer.IsAlive; } }
        public bool IsActive { get { return dllmodel.IsActive || viewer.IsActive; } }
        
        public event RoutedEventHandler Started = delegate { };
        public event RoutedEventHandler Paused = delegate { };
        public event RoutedEventHandler Aborted = delegate { };

        public void Abort()
        {
            if (viewer.ISBPPause) viewer.Resume();
            dllmodel.Abort();
            viewer.Abort();
        }

        public void Pause()
        {
            dllmodel.Pause();
            viewer.Pause();
        }

        public void Start()
        {
            if (viewer.ISBPPause) viewer.Resume();
            dllmodel.Start();
            viewer.Start();
        }
        
        private void OnStarted(object sender, RoutedEventArgs e)
        {
            if (dllmodel.IsActive && viewer.IsActive)
                Started(this, new RoutedEventArgs());
        }

        private void OnPaused(object sender, RoutedEventArgs e)
        {
            if (!IsActive) Paused(this, new RoutedEventArgs());
        }

        private void OnAborted(object sender, RoutedEventArgs e)
        {
            if (!IsAlive) Aborted(this, new RoutedEventArgs());
        }

        #endregion

        #region Breakpoint

        public bool IsBPPause { get { return viewer.ISBPPause; } }

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

        #endregion

    }
}
