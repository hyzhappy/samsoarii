using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSoarII.Simulation.Core.VariableModel;

namespace SamSoarII.Simulation.Core.DataModel
{
    public class SimulateDataModel
    {
        private SimulateVariableUnit svunit;

        private List<ValueSegment> values;

        public SimulateVariableUnit SVUnit
        {
            get { return this.svunit; }
            set { this.svunit = value; }
        }

        public IEnumerable<ValueSegment> Values
        {
            get { return this.values; }
        }

        public string Name
        {
            get { return this.svunit == null ? string.Empty : this.svunit.Name; }
        }

        public string Var
        {
            get { return this.svunit == null ? string.Empty : this.svunit.Var; }
        }

        public string Type
        {
            get { return this.svunit == null ? string.Empty : this.svunit.Type; }
        }

        public SimulateDataModel()
        {
            values = new List<ValueSegment>();
        }

        public SimulateDataModel(SimulateVariableUnit _svunit)
        {
            SVUnit = _svunit;
            values = new List<ValueSegment>();
        }

        public static SimulateDataModel Create(string name)
        {
            SimulateVariableUnit svunit = SimulateVariableUnit.Create(name);
            if (svunit == null) return null;
            return new SimulateDataModel(svunit);
        }
    }
}
