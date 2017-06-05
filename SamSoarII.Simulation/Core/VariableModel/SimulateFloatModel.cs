using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateFloatUnit : SimulateVariableUnit
    {
        protected double value;
        override public Object Value
        {
            get { return (Object)(this.value); }
            set
            {
                double _value = this.value;
                this.value = (double)(value);
                if (_value != this.value)
                    ValueChanged(this, new RoutedEventArgs());
            }
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
            return true;
        }

        public override string ToString()
        {
            string _name = Name;
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s}={1}{2:s}", Name, value, Islocked ? "(Lock)" : String.Empty);
        }

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Update(SimulateDllModel dllmodel)
        {
            double[] fvalues = dllmodel.GetValue_Float(Name, 1);
            double value_old = value;
            Value = fvalues[0];
        }

        public override void Set(SimulateDllModel dllmodel)
        {
            double[] fvalues = { this.value };
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
                    this.values[i].Value = (double)(0.0);
                    this.values[i].CanClose = false;
                    this.values[i].CanLock = false;
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
            double[] fvalues = dllmodel.GetValue_Float(Name, size);
            for (int i = 0; i < size; i++)
            {
                this.values[i].Value = fvalues[i];
            }
        }
        
        public override void Set(SimulateDllModel dllmodel)
        {
            double[] fvalues = new double[size];
            for (int i = 0; i < size; i++)
            {
                fvalues[i] = (double)(this.values[i].Value);
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
