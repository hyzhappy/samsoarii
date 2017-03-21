using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Shell.Event
{
    public class ShowTabItemEventArgs : EventArgs
    {
        public string TabName { get; set; }
        public ShowTabItemEventArgs(string tabName)
        {
            TabName = tabName;
        }
    }
}
