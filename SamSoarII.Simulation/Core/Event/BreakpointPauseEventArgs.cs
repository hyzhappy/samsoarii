using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core.Event
{
    public enum BreakpointStatus
    {
        NORMAL, STEP, CALL, JUMP, OUT, SOF
    }

    public class BreakpointPauseEventArgs
    {
        public BreakpointStatus Status { get; private set; } 
        public int Address { get; private set; }
        
        public BreakpointPauseEventArgs(int _address, BreakpointStatus _status)
        {
            Address = _address;
            Status = _status;
        }
    }

    public delegate void BreakpointPauseEventHandler(object sender, BreakpointPauseEventArgs e);
}
