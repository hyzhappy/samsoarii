using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateBitUnit : SimulateVariableUnit
    {
        protected int value;
        override public Object Value
        {
            get { return (Object)(this.value); }
            set { this.value = (int)(value); }
        }
        public override string ToString()
        {
            string _name = Name;
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s} = {1:d}", _name, value);
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
                case "X": case "Y":
                    if (offset < 0 || offset >= 128)
                        return false;
                    return true;
                case "M":
                    if (offset < 0 || offset >= 256 * 32)
                        return false;
                    return true;
                case "C": case "T":
                    if (offset < 0 || offset >= 256)
                        return false;
                    return true;
                case "S":
                    if (offset < 0 || offset >= 32 * 32)
                        return false;
                    return true;
                default:
                    return false;
            }
        }
        public override void Update(SimulateDllModel dllmodel)
        {
            int[] ivalues = dllmodel.GetValue_Bit(Name, 1);
            this.value = ivalues[0];
        }
        public override void Set(SimulateDllModel dllmodel)
        {
            int[] ivalues = { this.value };
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
    }
}
