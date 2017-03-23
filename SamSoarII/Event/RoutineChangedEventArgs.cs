using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class RoutineChangedEventArgs : EventArgs
    {
        public string RoutineName { get; set; }

        public RoutineChangedEventArgs(string name)
        {
            RoutineName = name;
        }
    }
}
