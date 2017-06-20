using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SamSoarII.AppMain.Project.Helper
{
    public class StatusBarHepler
    {
        private Thread _updateThread;
        private InteractionFacade IFacade;
        private string _message;
        public StatusBarHepler(InteractionFacade _interactionFacade, string message)
        {
            IFacade = _interactionFacade;
            _message = message;
        }
        public void Start()
        {
            _updateThread = new Thread(ThreadStartingPoint);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }
        public void Abort()
        {
            _updateThread.Abort();
        }
        private void ThreadStartingPoint()
        {
            IFacade.MainWindow.SB_Message.Text = _message;
            Dispatcher.Run();
        }
    }
}
