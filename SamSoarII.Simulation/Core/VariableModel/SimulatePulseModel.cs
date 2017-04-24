using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulatePulseUnit : SimulateVariableUnit
    {
        public override string Type
        {
            get
            {
                return "PULSE";
            }
        }

        public Int64 value;

        public override object Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = (Int64)(value);
                ValueChanged(this, new RoutedEventArgs());
            }
        }

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Set(SimulateDllModel dllmodel)
        {
        }

        public override void Update(SimulateDllModel dllmodel)
        {
            Value = dllmodel.GetFeq(Name);
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
                case "Y":
                    if (offset < 0 || offset >= 4)
                        return false;
                    return true;
                default:
                    return false;
            }
        }
    }

    public class SimulatePulseModel : SimulateVariableModel
    {
        public override int Size
        {
            get
            {
                return 4;
            }

            set
            {
            }
        }

        private SimulatePulseUnit[] values;

        public override SimulateVariableUnit[] Values
        {
            get
            {
                return values;
            }
        }

        public override void Set(SimulateDllModel dllmodel)
        {
            foreach (SimulatePulseUnit value in values)
            {
                value.Set(dllmodel);
            }
        }

        public override void Update(SimulateDllModel dllmodel)
        {
            foreach (SimulatePulseUnit value in values)
            {
                value.Update(dllmodel);
            }
        }
    }
}
