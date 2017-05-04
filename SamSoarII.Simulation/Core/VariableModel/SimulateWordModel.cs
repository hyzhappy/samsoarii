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
    public class SimulateWordUnit : SimulateVariableUnit
    {
        protected Int32 value;
        override public Object Value
        {
            get { return (Object)(this.value); }
            set
            {
                Int32 _value = this.value;
                this.value = (Int32)(value);
                if (_value != this.value)
                    ValueChanged(this, new RoutedEventArgs());
            }
        }
        public override string Type
        {
            get
            {
                return "WORD";
            }
        }
        protected override bool _Check_Name(string _name)
        {
            try
            {
                if (ValueParser.ParseWordValue(_name) != WordValue.Null)
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
        public override string ToString()
        {
            string _name = Name;
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s}={1:d}{2:s}", _name, value, Islocked ? "(Lock)" : String.Empty);
        }

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Update(SimulateDllModel dllmodel)
        {
            if (Islocked) return;
            Int32[] ivalues = dllmodel.GetValue_Word(Name, 1);
            Value = ivalues[0];
        }
        public override void Set(SimulateDllModel dllmodel)
        {
            Int32[] ivalues = { this.value };
            dllmodel.SetValue_Word(Name, 1, ivalues);
        }
    }

    public class SimulateWordModel : SimulateVariableModel
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
                    this.values[i] = new SimulateWordUnit();
                    this.values[i].Name = String.Format("{0:s}{1:d}", Base, Offset + i);
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
            int[] ivalues = dllmodel.GetValue_Word(Name, Size);
            for (int i = 0; i < size; i++)
            {
                this.values[i].Value = ivalues[i];
            }
        }


        public override void Set(SimulateDllModel dllmodel)
        {
            int[] ivalues = new int[size];
            for (int i = 0; i < size; i++)
            {
                ivalues[i] = (int)(this.values[i].Value);
            }
            dllmodel.SetValue_Word(Name, Size, ivalues);
        }

        static public new SimulateWordModel Create(IEnumerable<SimulateVariableUnit> svunits)
        {
            SimulateWordUnit swunit = (SimulateWordUnit)(svunits.First());
            SimulateWordModel swmodel = new SimulateWordModel();
            string _name = swunit.Name;
            int i = 0;
            while (char.IsLetter(_name[i])) i++;
            swmodel.Base = _name.Substring(0, i);
            swmodel.Offset = int.Parse(_name.Substring(i));
            swmodel.size = svunits.Count();
            swmodel.values = new SimulateWordUnit[swmodel.size];
            i = 0;
            foreach (SimulateVariableUnit svunit in svunits)
            {
                swmodel.values[i++] = (SimulateWordUnit)(svunit);
            }
            return swmodel;
        }
    }
}
