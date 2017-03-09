using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class RoutineRenamedEventArgs : EventArgs
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public RoutineRenamedEventArgs(string oldname, string newname)
        {
            OldName = oldname;
            NewName = newname;
        }
    }
}
