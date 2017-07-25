using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Simulate
{
    public interface IBreakpoint : INotifyPropertyChanged
    {
        int Address { get; set; }
        bool IsEnable { get; set; }
        bool IsActive { get; set; }
        BreakpointCursor Cursor { get; set; }
    }
}
