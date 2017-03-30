using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.Core.DataModel;

namespace SamSoarII.Simulation.Core
{
    public class SimulateManager
    {
        private SimulateDllModel dllmodel;

        private LinkedList<SimulateVariableModel> vlist;
        
        private Dictionary<SimulateVariableUnit, SimulateVariableUnit> udict;

        private Dictionary<SimulateVariableUnit, SimulateVariableUnit> ldict;

        private Dictionary<string, string> vndict;

        private Dictionary<SimulateDataModel, SimulateDataModel> lddict;

        private Dictionary<SimulateDataModel, SimulateDataModel> vddict;

        public SimulateManager()
        {
            dllmodel = new SimulateDllModel();
            vlist = new LinkedList<SimulateVariableModel>();
            udict = new Dictionary<SimulateVariableUnit, SimulateVariableUnit>();
            ldict = new Dictionary<SimulateVariableUnit, SimulateVariableUnit>();
            vndict = new Dictionary<string, string>();
            lddict = new Dictionary<SimulateDataModel, SimulateDataModel>();
            vddict = new Dictionary<SimulateDataModel, SimulateDataModel>();
        }

        public IEnumerable<SimulateVariableModel> Variables
        {
            get { return this.vlist; }
        }

        public IEnumerable<SimulateVariableUnit> Values
        {
            get
            {
                IEnumerable<SimulateVariableUnit> ret = new List<SimulateVariableUnit>();
                foreach (SimulateVariableModel svmodel in Variables)
                {
                    ret = ret.Union(svmodel.Values);
                }
                return ret;
            }
        }

        public void Add(string name, int size, string type)
        {
            SimulateVariableModel svmodel = SimulateVariableModel.Create(name, size, type);
            vlist.AddLast(svmodel);
        }

        public void Add(SimulateVariableModel svmodel)
        {
            vlist.AddLast(svmodel);
        }

        public void Add(SimulateVariableUnit svunit)
        {
            if (svunit != null && !udict.ContainsKey(svunit))
            {
                udict.Add(svunit, svunit);
                if (vndict.ContainsKey(svunit.Name))
                {
                    vndict.Remove(svunit.Name);
                }
                if (svunit.Var != null && !svunit.Var.Equals(""))
                {
                    vndict.Add(svunit.Name, svunit.Var);
                }
                svunit.Setup(this);
            }
        }
        
        public void Remove(string name, int size)
        {
            foreach (SimulateVariableModel svmodel in vlist)
            {
                if (svmodel.Name.Equals(name) && svmodel.Size == size)
                {
                    vlist.Remove(svmodel);
                    break;
                }
            }
        }

        public void Remove(SimulateVariableModel svmodel)
        {
            if (svmodel != null)
            {
                Remove(svmodel.Name, svmodel.Size);
            }
        }

        public void Remove(SimulateVariableUnit svunit)
        {
            if (svunit != null)
            {
                if (udict.ContainsKey(svunit))
                {
                    udict.Remove(svunit);
                    if (vndict.ContainsKey(svunit.Name))
                    {
                        vndict.Remove(svunit.Name);
                    }
                }
                if (ldict.ContainsKey(svunit))
                {
                    ldict.Remove(svunit);
                    if (vndict.ContainsKey(svunit.Name))
                    {
                        vndict.Remove(svunit.Name);
                    }
                }
            }
            
        }

        public void Replace(SimulateVariableUnit oldUnit, SimulateVariableUnit newUnit)
        {
            if (oldUnit != null && ldict.ContainsKey(oldUnit))
            {
                Remove(oldUnit);
                Add(newUnit);
                Lock(newUnit);
            }
            else
            {
                Remove(oldUnit);
                Add(newUnit);
            }
        }

        public void Rename(string bname, string vname)
        {
            if (vname.Equals(""))
            {
                if (vndict.ContainsKey(bname))
                {
                    vndict.Remove(bname);
                }
            }
            else
            {
                if (vndict.ContainsKey(bname))
                {
                    vndict.Remove(bname);
                }
                vndict.Add(bname, vname);
            }
        }

        public void Update()
        {
            foreach (SimulateVariableModel svmodel in Variables)
            {
                svmodel.Update(dllmodel);
            }
            foreach (SimulateVariableUnit svunit in udict.Values)
            {
                svunit.Update(dllmodel);
            }
            foreach (SimulateVariableUnit svunit in ldict.Values)
            {
                svunit.Set(dllmodel);
            }
        }
        
        public SimulateVariableUnit GetVariableUnit(SimulateVariableUnit unit)
        {
            if (unit == null)
            {
                return null;
            }
            if (udict.ContainsKey(unit))
            {
                SimulateVariableUnit _unit = udict[unit];
                return _unit;
            }
            else if (ldict.ContainsKey(unit))
            {
                SimulateVariableUnit _unit = ldict[unit];
                return _unit;
            }
            else
            {
                Add(unit);
                return unit;
            }
        }
        
        public string GetVariableName(SimulateVariableUnit svunit)
        {
            if (svunit == null)
            {
                return String.Empty;
            }
            if (vndict.ContainsKey(svunit.Name))
            {
                return vndict[svunit.Name];
            }
            return svunit.Name;
        }

        public void Lock(SimulateVariableUnit svunit)
        {
            if (svunit == null)
            {
                return;
            }
            if (udict.ContainsKey(svunit))
            {
                udict.Remove(svunit);
            }
            if (!ldict.ContainsKey(svunit))
            {
                ldict.Add(svunit, svunit);
            }
            dllmodel.Lock(svunit.Name);
        }

        public void Unlock(SimulateVariableUnit svunit)
        {
            if (svunit == null)
            {
                return;
            }
            if (ldict.ContainsKey(svunit))
            {
                ldict.Remove(svunit);
            }
            if (!udict.ContainsKey(svunit))
            {
                udict.Add(svunit, svunit);
            }
            dllmodel.Unlock(svunit.Name);
        }

        public void Lock(SimulateDataModel sdmodel)
        {
            if (!lddict.ContainsKey(sdmodel))
            {
                lddict.Add(sdmodel, sdmodel);
                sdmodel.IsLock = true;
                dllmodel.Lock(sdmodel);
            }
        }
        
        public void View(SimulateDataModel sdmodel)
        {
            if (!vddict.ContainsKey(sdmodel))
            {
                vddict.Add(sdmodel, sdmodel);
                sdmodel.IsView = true;
                dllmodel.View(sdmodel);
            }
        }

        public void Unlock(SimulateDataModel sdmodel)
        {
            if (lddict.ContainsKey(sdmodel))
            {
                lddict.Remove(sdmodel);
                sdmodel.IsLock = false;
                dllmodel.Unlock(sdmodel);
            }
        }
        
        public void Unview(SimulateDataModel sdmodel)
        {
            if (vddict.ContainsKey(sdmodel))
            {
                vddict.Remove(sdmodel);
                sdmodel.IsView = false;
                dllmodel.Unview(sdmodel);
            }
        }

        public void Start()
        {
            dllmodel.Start();
        }

        public void Stop()
        {
            dllmodel.Abort();
            int[] emptyBuffer = new int[8192];
            for (int i = 0; i < emptyBuffer.Length; i++)
            {
                emptyBuffer[i] = 0;
            }
            dllmodel.SetValue_Bit("X0", 128, emptyBuffer);
            dllmodel.SetValue_Bit("Y0", 128, emptyBuffer);
            dllmodel.SetValue_Bit("M0", 256<<5, emptyBuffer);
            dllmodel.SetValue_Bit("C0", 256, emptyBuffer);
            dllmodel.SetValue_Bit("T0", 256, emptyBuffer);
            dllmodel.SetValue_Bit("S0", 32<<5, emptyBuffer);
            dllmodel.SetValue_Word("D0", 8192, emptyBuffer);
            dllmodel.SetValue_Word("TV0", 256, emptyBuffer);
            dllmodel.SetValue_Word("CV0", 200, emptyBuffer);
            dllmodel.SetValue_DWord("CV200", 56, emptyBuffer);
        }
        
    }
}
