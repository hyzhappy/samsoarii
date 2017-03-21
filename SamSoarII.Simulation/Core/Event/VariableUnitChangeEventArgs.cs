using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSoarII.Simulation.Core.VariableModel;

namespace SamSoarII.Simulation.Core.Event
{
    public delegate void VariableUnitChangeEventHandler(Object sender, VariableUnitChangeEventArgs e);

    public class VariableUnitChangeEventArgs : EventArgs
    {
        public SimulateVariableUnit Old;
        public SimulateVariableUnit New;
    }
}
