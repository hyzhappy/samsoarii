using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace SamSoarII.Threads
{
    public abstract class BaseThreadManager : IThreadManager
    {
        public BaseThreadManager(bool _isMTA)
        {
            isMTA = _isMTA;
        }

        #region IThreadManager

        private Thread thread;
        private bool isalive;
        private bool isactive;
        private bool thalive;
        private bool thactive;
        private bool isMTA;
        private int timespan = 10;
        public bool IsAlive { get { return this.isalive && thread != null && thread.IsAlive; } }
        public bool IsActive { get { return this.isactive && thread != null; } }
        public bool ThAlive { get { return this.thalive; } }
        public bool ThActive { get { return this.thactive; } }
        public int TimeSpan
        {
            get { return this.timespan; }
            set { this.timespan = value; }
        }
        public event RoutedEventHandler Started = delegate { };
        public event RoutedEventHandler Paused = delegate { };
        public event RoutedEventHandler Aborted = delegate { };

        private void _Thread_Run()
        {
            Before();
            while (thalive)
            {
                _Invoke_Start();
                if (!thalive) break;
                _Thread_Handle();
                if (!thalive) break;
                _Invoke_Pause();
                if (!thalive) break;
                do
                {
                    Thread.Sleep(timespan);
                } while (thalive && !thactive);
                if (!thalive) break;
            }
            After();
            _Invoke_Abort();
        }
        protected virtual void _Thread_Handle()
        {
            while (thalive && thactive) Handle();
        }
        protected abstract void Handle();
        protected virtual void Before() { }
        protected virtual void After() { }

        private void _Invoke_Start()
        {
            isalive = true;
            isactive = true;
            Started(this, new RoutedEventArgs());
        }
        private void _Invoke_Pause()
        {
            isactive = false;
            Paused(this, new RoutedEventArgs());
        }
        private void _Invoke_Abort()
        {
            isalive = false;
            isactive = false;
            thread.Abort();
            thread = null;
            Aborted(this, new RoutedEventArgs());
        }

        public virtual void Abort()
        {
            if (!IsAlive) return;
            thalive = false;
            if (isactive) return;
            _Invoke_Abort();
        }

        public virtual void Pause()
        {
            thactive = false;
        }

        public virtual void Start()
        {
            thalive = true;
            thactive = true;
            if (IsAlive) return;
            thread = new Thread(_Thread_Run);
            if (isMTA) thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
        }

        #endregion
    }
}
