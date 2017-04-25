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
        public SimulateSpecialUnit(
            SimulateVariableUnit _prototype, 
            bool _canread, bool _canwrite)
        {
            Prototype = _prototype;
            CanRead = _canread;
            CanWrite = _canwrite;
            Prototype.ValueChanged += OnPrototypeValueChanged;
            Prototype.LockChanged += OnPrototypeLockChanged;
        }

        public SimulateVariableUnit Prototype { get; private set; }

        public override string Name
        {
            get
            {
                return Prototype.Name;
            }

            set
            {
                Prototype.Name = value;
            }
        }

        public override string Var
        {
            get
            {
                return Prototype.Var;
            }

            set
            {
                Prototype.Var = value;
            }
        }
        
        public override string Type
        {
            get
            {
                return Prototype.Type;
            }
        }

        public override bool Islocked
        {
            get
            {
                return Prototype.Islocked;
            }

            set
            {
                Prototype.Islocked = value;
            }
        }

        public override bool CanLock
        {
            get
            {
                return Prototype.CanLock;
            }

            set
            {
                Prototype.CanLock = value;
            }
        }

        public override bool CanClose
        {
            get
            {
                return Prototype.CanClose;
            }

            set
            {
                Prototype.CanClose = value;
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

        private void OnPrototypeValueChanged(object sender, RoutedEventArgs e)
        {
            ValueChanged(this, e);
        }

        public override event RoutedEventHandler LockChanged = delegate { };

        private void OnPrototypeLockChanged(object sender, RoutedEventArgs e)
        {
            LockChanged(this, e);
        }

        public bool CanRead { get; private set; } = true;

        private bool canwrite = true;

        public bool CanWrite
        {
            get
            {
                return this.canwrite;
            }
            private set
            {
                this.canwrite = value;
                CanLock = value;
            }
        }

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

        public override string ToString()
        {
            return Prototype?.ToString();
        }


    }
}
