﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateDWordUnit : SimulateVariableUnit
    {
        protected int value;
        override public Object Value
        {
            get { return (Object)(this.value); }
            set { this.value = (int)(value); }
        }
        public override string ToString()
        {
            string _name = String.Format("{0:s}({1:s}{2:d}) = {3}", Name, basename, offset + 1);
            if (manager != null)
                _name = manager.GetVariableName(this);
            return String.Format("{0:s} = {3:d}", _name, value);
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
                case "K":
                    return true;
                case "D":
                    if (offset < 0 || offset >= 8192)
                        return false;
                    return true;
                case "CV":
                    if (offset < 200 || offset >= 256)
                        return false;
                    return true;
                default:
                    return false;
            }
        }
        public override void Update(SimulateDllModel dllmodel)
        {
            int[] ivalues = dllmodel.GetValue_DWord(Name, 1);
            this.value = ivalues[0];
        }
        public override void Set(SimulateDllModel dllmodel)
        {
            int[] ivalues = { this.value };
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
                this.size = value;
                this.values = new SimulateVariableUnit[size];
                int factor = 1;
                if (Base.Equals("D"))
                    factor = 2;
                for (int i = 0; i < size; i++)
                {
                    this.values[i] = new SimulateDWordUnit();
                    this.values[i].Name = String.Format("{0:s}{1:d}", Base, Offset + i*factor);
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
            int[] ivalues = dllmodel.GetValue_DWord(Name, Size);
            for (int i = 0; i < size; i++)
            {
                this.values[i].Value = ivalues[i];
            }
        }
    }
}
