using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateDoubleUnit : SimulateVariableUnit
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
        public override string ToString()
        {
            string _name = String.Format("{0:s}({1:s}{2:d}) = {3}", Name, basename, offset+1, value);
            if (manager != null)
                _name = manager.GetVariableName(this);
            return _name;
        }
        public override string Type
        {
            get
            {
                return "DOUBLE";
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

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Update(SimulateDllModel dllmodel)
        {
            if (Islocked) return;
            double[] dvalues = dllmodel.GetValue_Double(Name, 1);
            Value = dvalues[0];
        }
        public override void Set(SimulateDllModel dllmodel)
        {
            double[] dvalues = { this.value };
            dllmodel.SetValue_Double(Name, 1, dvalues);
        }
    }

    public class SimulateDoubleModel : SimulateVariableModel
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
                this.size = value;
                this.values = new SimulateVariableUnit[size];
                for (int i = 0; i < size; i++)
                {
                    this.values[i] = new SimulateDoubleUnit();
                    this.values[i].Name = String.Format("{0:s}{1:d}", Base, Offset + i*2);
                    this.values[i].Value = 0;
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
            double[] dvalues = dllmodel.GetValue_Double(Name, Size);
            for (int i = 0; i < size; i++)
            {
                this.values[i].Value = dvalues[i];
            }
        }
        
        public override void Set(SimulateDllModel dllmodel)
        {
            double[] dvalues = new double[size];
            for (int i = 0; i < size; i++)
            {
                dvalues[i] = (double)(this.values[i].Value);
            }
            dllmodel.SetValue_Double(Name, Size, dvalues);
        }

        static public new SimulateDoubleModel Create(IEnumerable<SimulateVariableUnit> svunits)
        {
            SimulateDoubleUnit sdunit = (SimulateDoubleUnit)(svunits.First());
            SimulateDoubleModel sdmodel = new SimulateDoubleModel();
            string _name = sdunit.Name;
            int i = 0;
            while (char.IsLetter(_name[i])) i++;
            sdmodel.Base = _name.Substring(0, i);
            sdmodel.Offset = int.Parse(_name.Substring(i));
            sdmodel.size = svunits.Count();
            sdmodel.values = new SimulateDoubleUnit[sdmodel.size];
            i = 0;
            foreach (SimulateVariableUnit svunit in svunits)
            {
                sdmodel.values[i++] = (SimulateDoubleUnit)(svunit);
            }
            return sdmodel;
        }
    }
}
