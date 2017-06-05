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
    public class SimulateBitUnit : SimulateVariableUnit
    {
        protected int value;
        override public Object Value
        {
            get { return (Object)(this.value); }
            set
            {
                int _value = this.value;
                this.value = (int)(value);
                if (_value != this.value)
                    ValueChanged(this, new RoutedEventArgs());
            }
        }
        public override string ToString()
        {
            string _name = Name;
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s}={1:d}{2:s}", _name, value, Islocked ? "(Lock)" : String.Empty);
        }
        public override string Type
        {
            get
            {
                return "BIT";
            }
        }
        protected override bool _Check_Name(string _name)
        {
            return true;
        }

        override public event RoutedEventHandler ValueChanged = delegate { };
        
        public override void Update(SimulateDllModel dllmodel)
        {
            if (Islocked) return;
            Int32[] ivalues = dllmodel.GetValue_Bit(Name, 1);
            Value = ivalues[0];
        }

        public override void Set(SimulateDllModel dllmodel)
        {
            Int32[] ivalues = { this.value };
            dllmodel.SetValue_Bit(Name, 1, ivalues);
        }
    }

    public class SimulateBitModel : SimulateVariableModel
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
                    this.values[i] = new SimulateBitUnit();
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
            int[] ivalues = dllmodel.GetValue_Bit(Name, Size);
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
            dllmodel.SetValue_Bit(Name, Size, ivalues);
        }

        static public new SimulateBitModel Create(IEnumerable<SimulateVariableUnit> svunits)
        {
            SimulateBitUnit sbunit = (SimulateBitUnit)(svunits.First());
            SimulateBitModel sbmodel = new SimulateBitModel();
            string _name = sbunit.Name;
            int i = 0;
            while (char.IsLetter(_name[i])) i++;
            sbmodel.Base = _name.Substring(0, i);
            sbmodel.Offset = int.Parse(_name.Substring(i));
            sbmodel.size = svunits.Count();
            sbmodel.values = new SimulateBitUnit[sbmodel.size];
            i = 0;
            foreach (SimulateVariableUnit svunit in svunits)
            {
                sbmodel.values[i++] = (SimulateBitUnit)(svunit);
            }
            return sbmodel;
        }

    }
}
