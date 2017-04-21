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
        public int ID
        {
            get; set;
        }

        public SimulateDataModel SDModel_old
        {
            get; set;
        }

        public SimulateDataModel SDModel_new
        {
            get; set;
        }

        public IEnumerable<SimulateDataModel> SDModels
        {
            get; set;
        }

        public double TimeStart
        {
            get; set;
        }

        public double TimeEnd
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
            ID = 0;
            SDModel_old = null;
            SDModel_new = null;
            SDModels = null;
            TimeStart = 0;
            TimeEnd = 0;
            SDVModel = null;
            SDCModel = null;
        }
    }
}
