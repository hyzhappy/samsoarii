using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateStringUnit : SimulateVariableUnit
    {
        public SimulateStringUnit(string _value)
        {
            Value = _value;
        }

        public override string Type
        {
            get
            {
                return "STRING";
            }
        }

        private string value;

        public override object Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value.ToString();
            }
        }

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Set(SimulateDllModel dllmodel)
        {
        }

        public override void Update(SimulateDllModel dllmodel)
        {
        }

        protected override bool _Check_Name(string _name)
        {
            return false;
        }

        public override string ToString()
        {
            return value?.ToString();
        }
    }
}
