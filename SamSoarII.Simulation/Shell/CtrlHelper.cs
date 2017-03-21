using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSoarII.Simulation.Shell
{
    public class CtrlHelper
    {
        static public void Scale(Control ctrl, double scale)
        {
            ctrl.Width = (int)(ctrl.Width * scale);
            ctrl.Height = (int)(ctrl.Height * scale);
            foreach (Control sctrl in ctrl.Controls)
            {
                Scale(sctrl, scale);
            }
        }
    }
}
