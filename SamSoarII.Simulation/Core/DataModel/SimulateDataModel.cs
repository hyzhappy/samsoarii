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

        public bool IsLock
        {
            get; set;
        }

        public bool IsView
        {
            get; set;
        }
        
        public int TimeStart
        {
            get
            {
                return values.First().TimeStart;
            }
        }

        public int TimeEnd
        {
            get
            {
                return values.Last().TimeEnd;
            }
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

        public void Init(int starttime)
        {
            values.Clear();
            ValueSegment vs;
            switch (Type)
            {
                case "BIT": case "WORD": case "DWORD":
                    vs = new IntSegment();
                    vs.Value = 0;
                    break;
                case "FLOAT":
                    vs = new FloatSegment();
                    vs.Value = 0;
                    break;
                case "DOUBLE":
                    vs = new DoubleSegment();
                    vs.Value = 0;
                    break;
                default:
                    throw new ArgumentException();
            }
            vs.TimeStart = starttime;
            vs.TimeEnd = starttime;
            values.Add(vs);
        }

        public void Add(ValueSegment vs)
        {
            values.Add(vs);
        }

        public void Remove(ValueSegment vs)
        {
            values.Remove(vs);
        }
        
        public void SortByTime()
        {
            values.Sort(new ValueSegmentTimeComparer());
        }

        public SimulateDataModel Clone()
        {
            SimulateDataModel sdmodel = new SimulateDataModel();
            sdmodel.SVUnit = SVUnit;
            foreach (ValueSegment vs in Values)
            {
                sdmodel.Add(vs.Clone());
            }
            return sdmodel;
        }

        public override string ToString()
        {
            string text = "";
            foreach (ValueSegment vs in Values)
            {
                text += vs.ToString();
            }
            return text;
        }

        public static SimulateDataModel Create(string name)
        {
            SimulateVariableUnit svunit = SimulateVariableUnit.Create(name);
            if (svunit == null) return null;
            return new SimulateDataModel(svunit);
        }
        
    }
}
