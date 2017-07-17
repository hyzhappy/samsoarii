using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Simulate
{
    public class BreakpointPauseEventArgs : EventArgs
    {
        private int address;
        public int Address { get { return this.address; } }

        public BreakpointPauseEventArgs(int _address)
        {
            address = _address;
        }
    }

    public delegate void BreakpointPauseEventHandler(object sender, BreakpointPauseEventArgs e);
}
