using SamSoarII.ValueModel;
using SamSoarII.LadderInstViewModel.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public abstract class SimulateVariableUnit
    {
        protected string name = "";
        protected string type = "";
        protected string varname = "";
        protected bool islocked = false;
        protected SimulateManager manager = null;
       
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (_Check_Name(value))
                    this.name = value;
            }
        }

        public virtual string Var
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

        public virtual bool CanLock { get; set; } = true;

        public virtual bool CanClose { get; set; } = true;

        public virtual event RoutedEventHandler LockChanged = delegate { };

        public virtual bool Islocked
        {
            get
            {
                return this.islocked;
            }
            set
            {
                if (!CanLock) return;
                this.islocked = value;
                LockChanged(this, new RoutedEventArgs());
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
        
        abstract public event RoutedEventHandler ValueChanged;

        abstract public void Update(SimulateDllModel dllmodel);
        
        abstract public void Set(SimulateDllModel dllmodel);

        public virtual void Setup(SimulateManager _manager)
        {
            manager = _manager;
        }
        
        static public SimulateVariableUnit Create(string _name, string type = "")
        {
            SimulateVariableUnit result = null;
            SpecialValue svalue = SpecialValueManager.ValueOfName(_name);
            if (svalue != null)
                type = svalue.Type;
            if (type.Length == 0)
            {
                result = _Create(_name);
            }
            else
            {
                result = _Create(_name, type);
            }
            if (result == null)
            {
                return result;
            }
            if (svalue != null)
            {
                SimulateSpecialUnit ssunit = new SimulateSpecialUnit(
                    result, svalue.CanRead, svalue.CanWrite);
                ssunit.Var = svalue.NickName;
                return ssunit;
            }
            return result;
        }

        static private SimulateVariableUnit _Create(string _name)
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
            SimulateUnitSeries ssunit = new SimulateUnitSeries();
            if (ssunit._Check_Name(_name))
            {
                ssunit.Name = _name;
                ssunit.CreateExpand();
                return ssunit;
            }
            return null;
        }

        static private SimulateVariableUnit _Create(string _name, string type)
        {
            switch (type)
            {
                case "BIT":
                    SimulateBitUnit sbunit = new SimulateBitUnit();
                    if (sbunit._Check_Name(_name))
                    {
                        sbunit.Name = _name;
                        return sbunit;
                    }       
                    break;
                case "WORD":
                    SimulateWordUnit swunit = new SimulateWordUnit();
                    if (swunit._Check_Name(_name))
                    {
                        swunit.Name = _name;
                        return swunit;
                    }
                    break;
                case "DWORD":
                    SimulateDWordUnit sdwunit = new SimulateDWordUnit();
                    if (sdwunit._Check_Name(_name))
                    {
                        sdwunit.Name = _name;
                        return sdwunit;
                    }
                    break;
                case "FLOAT":
                    SimulateFloatUnit sfunit = new SimulateFloatUnit();
                    if (sfunit._Check_Name(_name))
                    {
                        sfunit.Name = _name;
                        return sfunit;
                    }
                    break;
                case "PULSE":
                    SimulatePulseUnit spunit = new SimulatePulseUnit();
                    if (spunit._Check_Name(_name))
                    {
                        spunit.Name = _name;
                        return spunit;
                    }
                    break;
                default:
                    SimulateUnitSeries ssunit = new SimulateUnitSeries();
                    if (ssunit._Check_Name(_name))
                    {
                        ssunit.Name = _name;
                        ssunit.CreateExpand();
                        return ssunit;
                    }
                    break;
            }
            return null;
        }
    }

    public class SimuMoniValueModel : IMoniValueModel
    {
        private SimulateVariableUnit svunit;

        public event RoutedEventHandler ValueChanged = delegate { };

        public string Value
        {
            get
            {
                return svunit.Value.ToString();
            }
        }

        public SimuMoniValueModel(SimulateVariableUnit _svunit)
        {
            svunit = _svunit;
            svunit.ValueChanged += OnValueChanged;
        }
        
        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            ValueChanged(this, e);
        }
    }
    
    public abstract class SimulateVariableModel
    {
        protected string basename = "";
        protected int offset = 0;
        protected int size = 0;
        
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

        abstract public void Set(SimulateDllModel dllmodel);
    
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

        public static SimulateVariableModel Create(string name, int size)
        {
            int i = 0;
            while (char.IsLetter(name[i])) i++;
            string bname = name.Substring(0, i);
            int offset = int.Parse(name.Substring(i));
            
            switch (bname)
            {
                case "X": case "Y": case "C": case "T": case "S": case "M":
                    return Create(name, size, "BIT");
                case "D": case "CV":
                    return Create(name, size, "WORD");
                case "TV":
                    if (offset < 200 && offset + size - 1 < 200)
                        return Create(name, size, "WORD");
                    if (offset > 200 && offset + size - 1 < 256)
                        return Create(name, size / 2, "DWORD");
                    break;
                default:
                    break;
            }
            return null;
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
                case "BIT":
                    svmodel = new SimulateBitModel();
                    break;
                case "WORD":
                    svmodel = new SimulateWordModel();
                    break;
                case "DWORD":
                    svmodel = new SimulateDWordModel();
                    break;
                case "FLOAT":
                    svmodel = new SimulateFloatModel();
                    break;
                default:
                    return svmodel;
            }
            svmodel.Base = bname;
            svmodel.Offset = offset;
            svmodel.Size = size;
            return svmodel;
        }

        public static SimulateVariableModel Create(IEnumerable<SimulateVariableUnit> svunits)
        {
            SimulateVariableUnit svunitf = svunits.First();
            if (svunitf is SimulateBitUnit)
            {
                return SimulateBitModel.Create(svunits);
            }
            if (svunitf is SimulateWordUnit)
            {
                return SimulateWordModel.Create(svunits);
            }
            if (svunitf is SimulateDWordUnit)
            {
                return SimulateDWordModel.Create(svunits);
            }
            if (svunitf is SimulateFloatUnit)
            {
                return SimulateFloatModel.Create(svunits);
            }
            return null;
        }
        
    }
}
