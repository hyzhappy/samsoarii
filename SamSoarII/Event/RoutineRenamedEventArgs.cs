using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class RoutineRenamedEventArgs : EventArgs
    {
        public string NewName { get; set; }

        public RoutineRenamedEventArgs(string newname)
        {
            NewName = newname;
        }
    }
}
