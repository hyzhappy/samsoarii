using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Simulate
{
    public class BreakpointPauseEventArgs : EventArgs
    {
        private BreakpointCursor cursor;
        public BreakpointCursor Cursor { get { return this.cursor; } }

        public BreakpointPauseEventArgs(BreakpointCursor _cursor)
        {
            cursor = _cursor;
        }
    }

    public delegate void BreakpointPauseEventHandler(object sender, BreakpointPauseEventArgs e);
}
