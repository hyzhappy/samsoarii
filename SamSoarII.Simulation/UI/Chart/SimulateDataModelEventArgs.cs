using SamSoarII.Simulation.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.UI.Chart
{
    public delegate void SimulateDataModelEventHandler(object sender, SimulateDataModelEventArgs e);

    public class SimulateDataModelEventArgs
    {
        public SimulateDataModel SDModel
        {
            get; set;
        }

        public SimulateDataViewModel SDVModel
        {
            get; set;
        }

        public SimulateDataChartModel SDCModel
        {
            get; set;
        }

        public SimulateDataModelEventArgs()
        {
            SDModel = null;
            SDVModel = null;
            SDCModel = null;
        }
    }
}
