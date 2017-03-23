using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class ShowTabItemEventArgs : EventArgs
    {
        public TabType Type { get; set; }

        public ShowTabItemEventArgs(TabType type)
        {
            Type = type;
        }
    }
}
