using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.AppMain.Project
{
    public class AutoInstManager
    {
        public AutoInstManager(InteractionFacade _ifacade)
        {
            ifacade = _ifacade;
            thread = null;
            isalive = false;
            isactive = false;
        }

        private InteractionFacade ifacade;

        #region Thread Control

        private Thread thread;
        private bool thalive;
        private bool isalive;
        public bool IsAlive { get { return this.isalive; } }
        private bool thactive;
        private bool isactive;
        public bool IsActive { get { return this.isactive; } }
        private void _Thread_Run()
        {
            isalive = true;
            Started(this, new RoutedEventArgs());

            while (thalive)
            {
                do
                {
                    isactive = false;
                    Paused(this, new RoutedEventArgs());
                    Thread.Sleep(GlobalSetting.InstTimeSpan * 1000);
                } while (thalive && (!GlobalSetting.IsInstByTime || !thactive));
                if (!thalive) break;

                isactive = true;
                Resumed(this, new RoutedEventArgs());

                ProjectModel pmodel = ifacade.ProjectModel;
                if (pmodel == null) continue;
                LadderDiagramViewModel main = pmodel.MainRoutine;
                if (main == null) continue;
                _Thread_Update(main);
                if (!thalive) break;
                foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
                {
                    _Thread_Update(ldvmodel);
                    if (!thalive) break;
                }
                if (!thalive) break;
            }

            isalive = false;
            thread = null;
            Aborted(this, new RoutedEventArgs());
        }
        private void _Thread_Update(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                InstructionNetworkViewModel invmodel = lnvmodel.INVModel;
                if (invmodel.IsModified)
                {
                    invmodel.Dispatcher.Invoke(() =>
                    {
                        invmodel.Update();
                    });
                }
                if (!thalive) return;
            }
        }

        public event RoutedEventHandler Started = delegate { };
        public event RoutedEventHandler Resumed = delegate { };
        public event RoutedEventHandler Paused = delegate { };
        public event RoutedEventHandler Aborted = delegate { };

        public void Start()
        {
            thactive = true;
            if (isalive) return;
            thalive = true;
            thactive = true;
            thread = new Thread(_Thread_Run);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        public void Pause()
        {
            thactive = false;
        }
        public void Abort()
        {
            thalive = false;
            if (isactive) return;
            isalive = false;
            thread.Abort();
            thread = null;
            Aborted(this, new RoutedEventArgs());
        }

        #endregion


    }
}
