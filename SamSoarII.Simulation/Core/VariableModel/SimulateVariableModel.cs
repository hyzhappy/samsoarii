using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateVariableUintComparer : IComparer<SimulateVariableUnit>
    {
        public int Compare(SimulateVariableUnit unit1, SimulateVariableUnit unit2)
        {
            int type1 = 0;
            int type2 = 0;

            if (unit1 is SimulateBitUnit)
                type1 = 1;
            if (unit1 is SimulateWordUnit)
                type1 = 2;
            if (unit1 is SimulateDWordUnit)
                type1 = 3;
            if (unit1 is SimulateFloatUnit)
                type1 = 4;
            if (unit1 is SimulateDoubleUnit)
                type1 = 5;

            if (unit2 is SimulateBitUnit)
                type2 = 1;
            if (unit2 is SimulateWordUnit)
                type2 = 2;
            if (unit2 is SimulateDWordUnit)
                type2 = 3;
            if (unit2 is SimulateFloatUnit)
                type2 = 4;
            if (unit2 is SimulateDoubleUnit)
                type2 = 5;

            if (type1 < type2)
                return -1;
            if (type1 > type2)
                return 1;

            return unit1.Name.CompareTo(unit2.Name);
        }
    }

    public abstract class SimulateVariableUnit
    {
        protected string name;
        protected string type;
        protected string basename;
        protected int offset;
        protected string varname;
        protected SimulateManager manager = null;
       
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (!_Check_Name(value)) return;
                this.name = value;
                int i = 0;
                while (char.IsLetter(name[i])) i++;
                this.basename = this.name.Substring(0, i);
                this.offset = int.Parse(this.name.Substring(i));
            }
        }
        public string Var
        {
            get
            {
                return this.varname;
            }
            set
            {
                this.varname = value;
            }
        }
        abstract public string Type
        {
            get;
        }
        abstract public Object Value
        {
            get;
            set;
        }

        abstract protected bool _Check_Name(string _name);

        abstract public void Update(SimulateDllModel dllmodel);

        abstract public void Set(SimulateDllModel dllmodel);

        public void Setup(SimulateManager _manager)
        {
            manager = _manager;
        }

        static public SimulateVariableUnit Create(string _name)
        {
            SimulateBitUnit sbunit = new SimulateBitUnit();
            if (sbunit._Check_Name(_name))
            {
                sbunit.Name = _name;
                return sbunit;
            }
            SimulateWordUnit swunit = new SimulateWordUnit();
            if (swunit._Check_Name(_name))
            {
                swunit.Name = _name;
                return swunit;
            }
            SimulateDWordUnit sdwunit = new SimulateDWordUnit();
            if (sdwunit._Check_Name(_name))
            {
                sdwunit.Name = _name;
                return sdwunit;
            }
            SimulateFloatUnit sfunit = new SimulateFloatUnit();
            if (sfunit._Check_Name(_name))
            {
                sfunit.Name = _name;
                return sfunit;
            }
            SimulateDoubleUnit sdfunit = new SimulateDoubleUnit();
            if (sdfunit._Check_Name(_name))
            {
                sdfunit.Name = _name;
                return sdfunit;
            }
            return null;
        }
    }

    public abstract class SimulateVariableModel
    {
        protected string basename;
        protected int offset;
        protected int size;
        
        public string Base
        {
            get { return this.basename; }
            set { this.basename = value; }
        }

        public int Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }
        
        abstract public int Size
        {
            get; set;
        }

        public string Name
        {
            get { return Base + Offset; }
        }

        abstract public SimulateVariableUnit[] Values
        {
            get;
        }

        abstract public void Update(SimulateDllModel dllmodel);
    
        public static SimulateVariableModel Create(string name)
        {
            if (Regex.Match(name, @"^\w+\[\d+\.\.\d+\]$").Length == 0)
            {
                return null;
            }
            int i = 0;
            while (char.IsLetter(name[i])) i++;
            string basename = name.Substring(0, i);
            int j = i + 1;
            while (char.IsDigit(name[++i])) ;
            int offsetlow = int.Parse(name.Substring(j, i - j));
            j = i + 2;
            i = j + 1;
            while (char.IsDigit(name[i])) i++;
            int offsethigh = int.Parse(name.Substring(j, i - j));
            if (offsetlow > offsethigh)
                return null;
            if (offsetlow < 0)
                return null;
            string _name = String.Format("{0:s}{1:d}",
                    basename, offsetlow);
            string type = "BIT";
            return Create(_name, offsethigh - offsetlow, type);
        }

        public static SimulateVariableModel Create(string name, int size, string type)
        {
            int i = 0;
            while (char.IsLetter(name[i])) i++;
            string bname = name.Substring(0, i);
            int offset = int.Parse(name.Substring(i));

            SimulateVariableModel svmodel = null;
            switch (type)
            {
                case "WORD":
                    svmodel = new SimulateWordModel();
                    break;
                case "DWORD":
                    svmodel = new SimulateDWordModel();
                    break;
                case "FLOAT":
                    svmodel = new SimulateFloatModel();
                    break;
                case "DOUBLE":
                    svmodel = new SimulateDoubleModel();
                    break;
                default:
                    return svmodel;
            }

            svmodel.Base = bname;
            svmodel.Offset = offset;
            svmodel.size = size;
            return svmodel;
        }
        
        
    }
}
