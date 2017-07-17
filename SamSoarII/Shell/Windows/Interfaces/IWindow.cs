using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Windows
{
    public interface IWindow
    {
        InteractionFacade IFParent { get; }
        event IWindowEventHandler Post;
    }
}
