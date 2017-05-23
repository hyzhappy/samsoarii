using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SamSoarII.AppMain.Project
{
    public class AutoSavedManager
    {
        private InteractionFacade _iFacade;
        private Thread AutoSaveThread { get; set; }
        public AutoSavedManager(InteractionFacade IFacade)
        {
            _iFacade = IFacade;
            AutoSaveThread = new Thread(Run);
        }
        public bool IsRunning
        {
            get
            {
                return AutoSaveThread.ThreadState == ThreadState.Running;
            }
        }
        public void Start()
        {
            AutoSaveThread.Start();
        }
        public void Abort()
        {
            AutoSaveThread.Abort();
        }
        public void Run()
        {
            while (GlobalSetting.IsSavedByTime)
            {
                Thread.Sleep(TimeSpan.FromMinutes(GlobalSetting.SaveTimeSpan));
                _iFacade.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { _iFacade.SaveProject(); });
            }
        }
    }
}
