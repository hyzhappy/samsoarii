using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateFloatUnit : SimulateVariableUnit
    {
        protected float value;
        override public Object Value
        {
            get { return (Object)(this.value); }
            set { this.value = (float)(value); }
        }
        public override string Type
        {
            get
            {
                return "FLOAT";
            }
        }
        protected override bool _Check_Name(string _name)
        {
            int i = 0;
            while (char.IsLetter(_name[i])) i++;
            string _base = _name.Substring(0, i);
            int offset = 0;
            try
            {
                offset = int.Parse(_name.Substring(i));
            }
            catch (FormatException e)
            {
                return false;
            }
            switch (_base)
            {
                case "F":
                    return true;
                case "D":
                    if (offset < 0 || offset >= 8192)
                        return false;
                    return true;
                default:
                    return false;
            }
        }
        public override string ToString()
        {
            string _name = Name;
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s} = {1}", _name, value);
        }
        public override void Update(SimulateDllModel dllmodel)
        {
            float[] fvalues = dllmodel.GetValue_Float(Name, 1);
            this.value = fvalues[0];
        }
        public override void Set(SimulateDllModel dllmodel)
        {
            float[] fvalues = { this.value };
            dllmodel.SetValue_Float(Name, 1, fvalues);
        }

    }

    public class SimulateFloatModel : SimulateVariableModel
    {
        protected SimulateVariableUnit[] values;

        public override int Size
        {
            get
            {
                return this.size;
            }

            set
            {
                this.size = value/2;
                this.values = new SimulateVariableUnit[size];
                for (int i = 0; i < size; i++)
                {
                    this.values[i] = new SimulateFloatUnit();
                    this.values[i].Name = String.Format("{0:s}{1:d}", Base, Offset + i*2);
                    this.values[i].Value = (float)(0.0);
                }
            }
        }

        public override SimulateVariableUnit[] Values
        {
            get
            {
                return this.values;
            }
        }

        public override void Update(SimulateDllModel dllmodel)
        {
            float[] fvalues = dllmodel.GetValue_Float(Name, size);
            for (int i = 0; i < size; i++)
            {
                this.values[i].Value = fvalues[i];
            }
        }
        
        public override void Set(SimulateDllModel dllmodel)
        {
            float[] fvalues = new float[size];
            for (int i = 0; i < size; i++)
            {
                fvalues[i] = (float)(this.values[i].Value);
            }
            dllmodel.SetValue_Float(Name, Size, fvalues);
        }

        static public new SimulateFloatModel Create(IEnumerable<SimulateVariableUnit> svunits)
        {
            SimulateFloatUnit sfunit = (SimulateFloatUnit)(svunits.First());
            SimulateFloatModel sfmodel = new SimulateFloatModel();
            string _name = sfunit.Name;
            int i = 0;
            while (char.IsLetter(_name[i])) i++;
            sfmodel.Base = _name.Substring(0, i);
            sfmodel.Offset = int.Parse(_name.Substring(i));
            sfmodel.size = svunits.Count();
            sfmodel.values = new SimulateFloatUnit[sfmodel.size];
            i = 0;
            foreach (SimulateVariableUnit svunit in svunits)
            {
                sfmodel.values[i++] = (SimulateFloatUnit)(svunit);
            }
            return sfmodel;
        }
    }
}
