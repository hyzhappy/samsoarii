using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateDWordUnit : SimulateVariableUnit
    {
        protected Int64 value;
        override public Object Value
        {
            get { return (Object)(this.value); }            
			set
            {
                Int64 _value = this.value;
                this.value = (Int64)(value);
                if (_value != this.value)
                    ValueChanged(this, new RoutedEventArgs());
            }
        }
        public override string ToString()
        {
            string _name = Name;
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s}={1}{2:s}", Name, value, Islocked ? "(Lock)" : String.Empty);
        }
        public override string Type
        {
            get
            {
                return "DWORD";
            }
        }
        protected override bool _Check_Name(string _name)
        {
            try
            {
                if (ValueParser.ParseDoubleWordValue(_name) != DoubleWordValue.Null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (ValueParseException e)
            {
                return false;
            }
        }
        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Update(SimulateDllModel dllmodel)
        {
			if (Islocked) return;
            Int64[] ivalues = dllmodel.GetValue_DWord(Name, 1);
            Value = ivalues[0];
        }
        public override void Set(SimulateDllModel dllmodel)
        {
            Int64[] ivalues = { this.value };
            dllmodel.SetValue_DWord(Name, 1, ivalues);
        }
    }

    public class SimulateDWordModel : SimulateVariableModel
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
                int factor = 1;
                if (Base.Equals("D"))
                    factor = 2;
                this.size = value/factor;
                this.values = new SimulateVariableUnit[size];
                for (int i = 0; i < size; i++)
                {
                    this.values[i] = new SimulateDWordUnit();
                    this.values[i].Name = String.Format("{0:s}{1:d}", Base, Offset + i*factor);
                    this.values[i].Value = 0;
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
            long[] ivalues = dllmodel.GetValue_DWord(Name, Size);
            for (int i = 0; i < size; i++)
            {
                this.values[i].Value = ivalues[i];
            }
        }

        public override void Set(SimulateDllModel dllmodel)
        {
            long[] ivalues = new long[size];
            for (int i = 0; i < size; i++)
            {
                ivalues[i] = (int)(this.values[i].Value);
            }
            dllmodel.SetValue_DWord(Name, Size, ivalues);
        }

        static public new SimulateDWordModel Create(IEnumerable<SimulateVariableUnit> svunits)
        {
            SimulateDWordUnit sdunit = (SimulateDWordUnit)(svunits.First());
            SimulateDWordModel sdmodel = new SimulateDWordModel();
            string _name = sdunit.Name;
            int i = 0;
            while (char.IsLetter(_name[i])) i++;
            sdmodel.Base = _name.Substring(0, i);
            sdmodel.Offset = int.Parse(_name.Substring(i));
            sdmodel.size = svunits.Count();
            sdmodel.values = new SimulateDWordUnit[sdmodel.size];
            i = 0;
            foreach (SimulateVariableUnit svunit in svunits)
            {
                sdmodel.values[i++] = (SimulateDWordUnit)(svunit);
            }
            return sdmodel;
        }
    }
}
