using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class ShowTabItemEventArgs : EventArgs
    {
        public string TabName { get; set; }

        public TabType Type { get; set; }

        public ShowTabItemEventArgs(string tabName, TabType type)
        {
            TabName = tabName;
            Type = type;
        }
    }
}
