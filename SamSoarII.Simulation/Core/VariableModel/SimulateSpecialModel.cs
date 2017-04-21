using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateSpecialUnit : SimulateVariableUnit
    {
        public SimulateVariableUnit Prototype { get; private set; }
        
        public override string Type
        {
            get
            {
                return Prototype.Type;
            }
        }
        

        public override object Value
        {
            get
            {
                return Prototype.Value;
            }

            set
            {
                Prototype.Value = value;
            }
        }

        public override event RoutedEventHandler ValueChanged = delegate { };

        public bool CanRead { get; private set; }

        public bool CanWrite { get; private set; }

        public override void Set(SimulateDllModel dllmodel)
        {
            if (CanWrite)
            {
                Prototype.Set(dllmodel);
            }
        }

        public override void Update(SimulateDllModel dllmodel)
        {
            if (CanRead)
            {
                Prototype.Update(dllmodel);
            }
        }

        protected override bool _Check_Name(string _name)
        {
            return true;
        }
        
    }

    public class SimulateSpecialModel : SimulateVariableModel
    {
        static private SimulateSpecialUnit[] units;

        static SimulateSpecialModel()
        {
            units = new SimulateSpecialUnit[100];
            
        }

        public override int Size
        {
            get
            {
                return units.Length;
            }

            set
            {

            }
        }

        public override SimulateVariableUnit[] Values
        {
            get
            {
                return units;
            }
        }

        public override void Set(SimulateDllModel dllmodel)
        {
            foreach (SimulateSpecialUnit ssunit in units)
            {
                ssunit.Set(dllmodel);
            }
        }

        public override void Update(SimulateDllModel dllmodel)
        {
            foreach (SimulateSpecialUnit ssunit in units)
            {
                ssunit.Update(dllmodel);
            }
        }
    }
}
