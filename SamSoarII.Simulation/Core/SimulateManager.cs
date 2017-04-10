using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.UI.Chart;
using System.Threading;

namespace SamSoarII.Simulation.Core
{
    public class SimulateManager
    {
        private SimulateDllModel dllmodel;

        private LinkedList<SimulateVariableModel> vlist;
        
        private Dictionary<string, List<SimulateVariableUnit> > udict;

        private Dictionary<string, List<SimulateVariableUnit> > ldict;

        private Dictionary<string, string> vndict;

        private Dictionary<string, SimulateDataModel> lddict;

        private Dictionary<string, SimulateDataModel> vddict;

        public SimulateManager()
        {
            dllmodel = new SimulateDllModel();
            vlist = new LinkedList<SimulateVariableModel>();
            udict = new Dictionary<string, List<SimulateVariableUnit> >();
            ldict = new Dictionary<string, List<SimulateVariableUnit> >();
            vndict = new Dictionary<string, string>();
            lddict = new Dictionary<string, SimulateDataModel>();
            vddict = new Dictionary<string, SimulateDataModel>();

            dllmodel.RunDataFinished += OnRunDataFinished;
            dllmodel.RunDrawFinished += OnRunDrawFinished;

            updateactive = false;
            updatethread = null;
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

        #region Update Thread
        private Thread updatethread;
        private bool updateactive;

        private void Update()
        {
            while (updateactive)
            {
                foreach (SimulateVariableModel svmodel in Variables)
                {
                    svmodel.Update(dllmodel);
                }
                foreach (List<SimulateVariableUnit> svulist in udict.Values)
                {
                    foreach (SimulateVariableUnit svunit in svulist)
                    {
                        svunit.Update(dllmodel);
                    }
                }
                foreach (List<SimulateVariableUnit> svulist in ldict.Values)
                {
                    foreach (SimulateVariableUnit svunit in svulist)
                    {
                        svunit.Set(dllmodel);
                    }
                }
                Thread.Sleep(50);
            }
        }

        private void UpdateStart()
        {
            if (SimuStatus != SIMU_RUNNING)
                return;
            if (updatethread == null)
            {
                updateactive = true;
                updatethread = new Thread(Update);
                updatethread.Start();
            }
        }

        private void UpdateStop()
        {
            if (updatethread != null)
            {
                updateactive = false;
                updatethread.Abort();
                updatethread = null;
            }
        }
        #endregion

        #region Simulation Control
        private const int SIMU_STOP = 0x00;
        private const int SIMU_RUNNING = 0x01;
        private const int SIMU_PAUSE = 0x02;
        private int simustatus;
        private int SimuStatus
        {
            get { return this.simustatus; }
            set { this.simustatus = value; }
        }

        public void Start()
        {
            if (SimuStatus == SIMU_RUNNING)
                return;
            SimuStatus = SIMU_RUNNING;
            dllmodel.Start();
            UpdateStart();
        }

        public void Pause()
        {
            if (SimuStatus == SIMU_PAUSE || SimuStatus == SIMU_STOP)
                return;
            SimuStatus = SIMU_PAUSE;
            dllmodel.Pause();
            UpdateStop();
        }

        public void Stop()
        {
            if (SimuStatus == SIMU_STOP)
                return;
            SimuStatus = SIMU_STOP;
            dllmodel.Abort();
            UpdateStop();
            int[] emptyBuffer = new int[8192];
            for (int i = 0; i < emptyBuffer.Length; i++)
            {
                emptyBuffer[i] = 0;
            }
            dllmodel.SetValue_Bit("X0", 128, emptyBuffer);
            dllmodel.SetValue_Bit("Y0", 128, emptyBuffer);
            dllmodel.SetValue_Bit("M0", 256 << 5, emptyBuffer);
            dllmodel.SetValue_Bit("C0", 256, emptyBuffer);
            dllmodel.SetValue_Bit("T0", 256, emptyBuffer);
            dllmodel.SetValue_Bit("S0", 32 << 5, emptyBuffer);
            dllmodel.SetValue_Word("D0", 8192, emptyBuffer);
            dllmodel.SetValue_Word("TV0", 256, emptyBuffer);
            dllmodel.SetValue_Word("CV0", 200, emptyBuffer);
            dllmodel.SetValue_DWord("CV200", 56, emptyBuffer);
        }
        #endregion

        #region Variable Manipulation
        public void Add(string name, int size, string type)
        {
            UpdateStop();
            SimulateVariableModel svmodel = SimulateVariableModel.Create(name, size, type);
            vlist.AddLast(svmodel);
            UpdateStart();
        }

        public void Add(SimulateVariableModel svmodel)
        {
            UpdateStop();
            vlist.AddLast(svmodel);
            UpdateStart();
        }

        public void Add(SimulateVariableUnit svunit)
        {
            UpdateStop();
            if (svunit != null)
            {
                List<SimulateVariableUnit> svulist = null;
                if (!udict.ContainsKey(svunit.Name))
                {
                    svulist = new List<SimulateVariableUnit>();
                    svulist.Add(svunit);
                    udict.Add(svunit.Name, svulist);
                }
                else
                {
                    svulist = udict[svunit.Name];
                    svulist.Add(svunit);
                }
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
            UpdateStart();
        }
        
        public void Remove(string name, int size)
        {
            UpdateStop();
            foreach (SimulateVariableModel svmodel in vlist)
            {
                if (svmodel.Name.Equals(name) && svmodel.Size == size)
                {
                    vlist.Remove(svmodel);
                    break;
                }
            }
            UpdateStart();
        }

        public void Remove(SimulateVariableModel svmodel)
        {
            UpdateStop();
            if (svmodel != null)
            {
                Remove(svmodel.Name, svmodel.Size);
            }
            UpdateStart();
        }

        public void Remove(SimulateVariableUnit svunit)
        {
            UpdateStop();
            if (svunit != null)
            {
                List<SimulateVariableUnit> svulist = null;
                if (udict.ContainsKey(svunit.Name))
                {
                    svulist = udict[svunit.Name];
                    if (svulist.Contains(svunit))
                        svulist.Remove(svunit);
                    if (vndict.ContainsKey(svunit.Name))
                    {
                        vndict.Remove(svunit.Name);
                    }
                }
                if (ldict.ContainsKey(svunit.Name))
                {
                    svulist = udict[svunit.Name];
                    if (svulist.Contains(svunit))
                        svulist.Remove(svunit);
                    if (vndict.ContainsKey(svunit.Name))
                    {
                        vndict.Remove(svunit.Name);
                    }
                }
            }
            UpdateStart();
        }

        public void Replace(SimulateVariableUnit oldUnit, SimulateVariableUnit newUnit)
        {
            UpdateStop();
            if (oldUnit != null && ldict.ContainsKey(oldUnit.Name))
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
            UpdateStart();
        }

        public void Rename(string bname, string vname)
        {
            UpdateStop();
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
            UpdateStart();
        }
        #endregion

        public SimulateVariableUnit GetVariableUnit(SimulateVariableUnit unit)
        {
            if (unit == null)
            {
                return null;
            }
            if (udict.ContainsKey(unit.Name))
            {
                List<SimulateVariableUnit> svulist = udict[unit.Name];
                if (svulist.Count() == 0) return null;
                return svulist.First();
            }
            else if (ldict.ContainsKey(unit.Name))
            {
                List<SimulateVariableUnit> svulist = ldict[unit.Name];
                if (svulist.Count() == 0) return null;
                return svulist.First();
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

        #region Lock & View
        public void Lock(SimulateVariableUnit svunit)
        {
            UpdateStop();
            if (svunit == null)
            {
                UpdateStart();
                return;
            }
            List<SimulateVariableUnit> ulist;
            List<SimulateVariableUnit> llist;
            if (udict.ContainsKey(svunit.Name))
            {
                ulist = udict[svunit.Name];
                if (!ulist.Contains(svunit))
                    ulist.Add(svunit);
                udict.Remove(svunit.Name);
            }
            else
            {
                ulist = new List<SimulateVariableUnit>();
                ulist.Add(svunit);
            }
            if (ldict.ContainsKey(svunit.Name))
            {
                llist = ldict[svunit.Name];
                llist = llist.Union(ulist).ToList();
                ldict[svunit.Name] = llist;
            }
            else
            {
                llist = ulist;
                ldict.Add(svunit.Name, llist);
            }
            foreach (SimulateVariableUnit _svunit in llist)
            {
                _svunit.Value = svunit.Value;
                _svunit.Islocked = true;
            }
            dllmodel.Lock(svunit.Name);
            UpdateStart();
        }

        public void Unlock(SimulateVariableUnit svunit)
        {
            UpdateStop();
            if (svunit == null)
            {
                UpdateStart();
                return;
            }
            List<SimulateVariableUnit> llist;
            List<SimulateVariableUnit> ulist;
            if (ldict.ContainsKey(svunit.Name))
            {
                llist = ldict[svunit.Name];
                if (!llist.Contains(svunit))
                    llist.Add(svunit);
                ldict.Remove(svunit.Name);
            }
            else
            {
                llist = new List<SimulateVariableUnit>();
                llist.Add(svunit);
            }
            if (udict.ContainsKey(svunit.Name))
            {
                ulist = udict[svunit.Name];
                ulist = ulist.Union(llist).ToList();
                udict[svunit.Name] = ulist;
            }
            else
            {
                ulist = llist;
                udict.Add(svunit.Name, ulist);
            }
            foreach (SimulateVariableUnit _svunit in llist)
            {
                _svunit.Islocked = false;
            }
            dllmodel.Unlock(svunit.Name);
            UpdateStart();
        }

        public void Lock(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (!lddict.ContainsKey(sdmodel.Name))
            {
                lddict.Add(sdmodel.Name, sdmodel);
                //sdmodel.IsLock = true;
                dllmodel.Lock(sdmodel);
            }
            UpdateStart();
        }
        
        public void View(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (!vddict.ContainsKey(sdmodel.Name))
            {
                vddict.Add(sdmodel.Name, sdmodel);
                //sdmodel.IsView = true;
                dllmodel.View(sdmodel);
            }
            UpdateStart();
        }

        public void Unlock(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (lddict.ContainsKey(sdmodel.Name))
            {
                lddict.Remove(sdmodel.Name);
                //sdmodel.IsLock = false;
                dllmodel.Unlock(sdmodel);
            }
            UpdateStart();
        }
        
        public void Unview(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (vddict.ContainsKey(sdmodel.Name))
            {
                vddict.Remove(sdmodel.Name);
                //sdmodel.IsView = false;
                dllmodel.Unview(sdmodel);
            }
            UpdateStart();
        }
        #endregion

        public void RunData(double timestart, double timeend)
        {
            dllmodel.RunData(timestart, timeend);
        }

        public void RunDraw(double timestart, double timeend)
        {
            dllmodel.RunDraw(timestart, timeend);
        }

        public void UpdateView(double timestart, double timeend)
        {
            StreamReader fin = new StreamReader("simulog.log");
            while (!fin.EndOfStream)
            {
                string text = fin.ReadLine();
                string[] args = text.Split(' ');
                string name = args[0];
                int time = int.Parse(args[1]);
                ValueSegment vs = null, vsp = null;
                SimulateDataModel sdmodel = vddict[name];
                switch (sdmodel.Type)
                {
                    case "BIT": case "WORD": case "DWORD":
                        vs = new IntSegment();
                        vs.Value = int.Parse(args[2]);
                        break;
                    case "FLOAT":
                        vs = new FloatSegment();
                        vs.Value = float.Parse(args[2]);
                        break;
                    case "DOUBLE":
                        vs = new DoubleSegment();
                        vs.Value = double.Parse(args[2]);
                        break;
                }
                if (sdmodel.Values.Count() == 0)
                {
                    vsp = vs.Clone();
                    vsp.Value = 0;
                    vsp.TimeStart = (int)(timestart);
                    vs.TimeStart = vsp.TimeEnd = time;
                    sdmodel.Add(vsp);
                    sdmodel.Add(vs);
                }
                else
                {
                    vsp = sdmodel.Values.Last();
                    vs.TimeStart = vsp.TimeEnd = time;
                    sdmodel.Add(vs);
                }
            }
            foreach (SimulateDataModel sdmodel in vddict.Values)
            {
                if (sdmodel.Values.Count() > 0)
                {
                    sdmodel.Values.Last().TimeEnd = (int)(timeend);
                }
            }
        }

        #region Event Handler
        public event SimulateDataModelEventHandler RunDataFinished;
        private void OnRunDataFinished(object sender, SimulateDataModelEventArgs e)
        {
            UpdateView(e.TimeStart, e.TimeEnd);
            if (RunDataFinished != null)
            {
                RunDataFinished(this, e);
            }
        }

        public event SimulateDataModelEventHandler RunDrawFinished;
        private void OnRunDrawFinished(object sender, SimulateDataModelEventArgs e)
        {
            UpdateView(e.TimeStart, e.TimeEnd);
            if (RunDrawFinished != null)
            {
                RunDrawFinished(this, e);
            }
        }

        #endregion

    }
}
