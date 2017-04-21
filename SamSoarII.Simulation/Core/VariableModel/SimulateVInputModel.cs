using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateVInputUnit : SimulateVariableUnit
    {
        private string _value = "";

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Update(SimulateDllModel dllmodel)
        {
        }

        public override void Set(SimulateDllModel dllmodel)
        {
        }

        public override string Type
        {
            get
            {
                return "Input";
            }
        }

        public override object Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value.ToString();
            }
        }

        protected override bool _Check_Name(string _name)
        {
            if (Regex.Match(_name, @"^\w+\[\d+\.\.\d+\]$").Length > 0)
            {
                return true;
            }
            if (Regex.Match(_name, @"^\w+\d+$").Length > 0)
            {
                return true;
            }
            return false;
            
        }

    }

    public class SimulateVInputModel
    {
    }
}
