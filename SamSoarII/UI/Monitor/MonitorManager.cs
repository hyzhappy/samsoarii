using SamSoarII.AppMain.Project;
using SamSoarII.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI.Monitor
{
    public class MonitorManager
    {
        public MainMonitor MMWindow { get; set; }
        public Thread MonitorReadThread { get; set; }
        public Thread MonitorWriteThread { get; set; }
        public MonitorDataHandle DataHandle { get; set; }
        public MonitorManager(ProjectModel projectModel)
        {
            MMWindow = new MainMonitor(projectModel);
            DataHandle = new MonitorDataHandle(MMWindow);
            MonitorReadThread = new Thread(DataHandle.ReadRun);
            MonitorWriteThread = new Thread(DataHandle.WriteRun);
        }
    }
}
