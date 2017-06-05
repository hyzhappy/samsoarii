using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public delegate void LadderElementChangedHandler(
        object sender, LadderElementChangedArgs e);
    public delegate void BreakpointChangedEventHandler(
        object sender, BreakpointChangedEventArgs e);
    public delegate void RoutineRenamedEventHandler(
        object sender, RoutineRenamedEventArgs e);
    public delegate void RoutineChangedEventHandler(
        object sender, RoutineChangedEventArgs e);
    public delegate void ShowTabItemEventHandler(
        object sender, ShowTabItemEventArgs e);
}
