using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Windows
{
    public interface IWindowEventArgs
    {
        int Flags { get; }
        object TargetedObject { get; }
        object RelativeObject { get; }
    }

    public delegate void IWindowEventHandler(IWindow sender, IWindowEventArgs e);
}
