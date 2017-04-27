using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public override bool CanLock
        {
            get
            {
                return false;
            }

            set
            {
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
            switch (_name)
            {
                case "Y0": case "Y1": case "Y2": case "Y3":
                    return true;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return String.Format("F{0:s}={1}", Name, value);
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
