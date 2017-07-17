using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Threads
{
    public class CoreThreadManager : IThreadManager, IDisposable
    {
        public CoreThreadManager(InteractionFacade _parent)
        {
            mngSave = new AutoSaveManager(_parent);
            mngInst = new AutoInstManager(_parent);
            Install(mngSave);
            Install(mngInst);
        }
        
        public void Dispose()
        {
            Uninstall(mngSave);
            Uninstall(mngSave);
            mngSave = null;
            mngInst = null;
        }

        private void Install(IThreadManager mng)
        {
            mng.Started += OnStarted;
            mng.Paused += OnPaused;
            mng.Aborted += OnAborted;
        }

        private void Uninstall(IThreadManager mng)
        {
            mng.Started -= OnStarted;
            mng.Paused -= OnPaused;
            mng.Aborted -= OnAborted;
        }

        #region IThreadManager

        public bool IsAlive { get { return mngInst.IsAlive || mngSave.IsAlive; } }
        public bool IsActive { get { return mngInst.IsActive || mngSave.IsActive; } }

        public event RoutedEventHandler Started = delegate { };
        public event RoutedEventHandler Paused = delegate { };
        public event RoutedEventHandler Aborted = delegate { };

        public void Abort()
        {
            mngSave.Abort();
            mngInst.Abort();
        }

        public void Pause()
        {
            mngSave.Pause();
            mngInst.Pause();
        }

        public void Start()
        {
            mngSave.Start();
            mngInst.Start();
        }

        #endregion

        #region Number

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }

        private AutoSaveManager mngSave;
        private AutoInstManager mngInst;
        public AutoSaveManager MNGSave { get { return this.mngSave; } }
        public AutoInstManager MNGInst { get { return this.mngInst; } }

        #endregion

        #region Event Handler

        private void OnAborted(object sender, RoutedEventArgs e)
        {
            Aborted(sender, e);
            if (!IsAlive) Aborted(this, e);
        }

        private void OnPaused(object sender, RoutedEventArgs e)
        {
            Paused(sender, e);
            if (!IsActive) Paused(this, e);
        }

        private void OnStarted(object sender, RoutedEventArgs e)
        {
            Started(sender, e);
            if (mngSave.IsActive && mngInst.IsActive) Started(this, e);
        }

        #endregion
    }
}
