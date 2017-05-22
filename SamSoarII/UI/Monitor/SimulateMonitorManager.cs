using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.Simulation;
using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.Core;
using System.Windows;
using SamSoarII.Communication.Command;

namespace SamSoarII.AppMain.UI.Monitor
{
    public class SimulateMonitorManager : IMonitorManager
    {
        private MainMonitor mmonitor;
        private SimulateModel smodel;
        private Dictionary<SimulateVariableUnit, ElementModel> dict
            = new Dictionary<SimulateVariableUnit, ElementModel>();
        private List<ElementModel> pretable
            = new List<ElementModel>();
        
        public SimulateMonitorManager(
            MainMonitor _mmonitor, SimulateModel _smodel)
        {
            mmonitor = _mmonitor;
            mmonitor.Manager = this;
            smodel = _smodel;
        }

        private string _Type(int datatype)
        {
            switch (datatype)
            {
                case 0: return "BIT";
                case 1: return "WORD";
                case 3: return "DWORD";
                case 6: return "FLOAT";
                default: return String.Empty;
            }
        }
        
        #region Interfaces
        public bool CanLock => true;
        
        public bool IsRunning { get; private set; }

        public event RoutedEventHandler Started = delegate { };

        public event RoutedEventHandler Aborted = delegate { };

        public void Start()
        {
            foreach (KeyValuePair<SimulateVariableUnit, ElementModel> kvp in dict)
            {
                kvp.Value.CurrentValue = kvp.Key.Value.ToString();
            }
            IsRunning = true;
            Started(this, new RoutedEventArgs());
        }

        public void Abort()
        {
            foreach (KeyValuePair<SimulateVariableUnit, ElementModel> kvp in dict)
            {
                kvp.Value.CurrentValue = "???";
            }
            IsRunning = false;
            Aborted(this, new RoutedEventArgs());
        }

        public void Add(ElementModel emodel)
        {
            SimulateVariableUnit svunit = smodel.GetVariableUnit(emodel.ShowName, _Type(emodel.DataType));
            if (!dict.ContainsKey(svunit))
            {
                dict.Add(svunit, emodel);
                emodel.RefCount = 1;
                svunit.ValueChanged += OnValueChanged;
            }
            else
            {
                dict[svunit].RefCount++;
            }
        }

        public void Add(IEnumerable<ElementModel> emodels)
        {
            foreach (ElementModel emodel in emodels)
            {
                Add(emodel);
            }
        }

        public ElementModel Get(ElementModel emodel)
        {
            SimulateVariableUnit svunit = smodel.GetVariableUnit(emodel.ShowName, _Type(emodel.DataType));
            if (dict.ContainsKey(svunit))
            {
                return dict[svunit];
            }
            else
            {
                return null;
            }
        }

        public void Initialize()
        {
            foreach (SimulateVariableUnit svunit in dict.Keys.ToArray())
            {
                dict[svunit].RefCount = 1;
                Remove(dict[svunit]);
            }
            foreach (MonitorVariableTable table in mmonitor.tables)
            {
                Add(table.Elements);
            }
        }

        public void Initialize(ProjectModel pmodel)
        {
            SimulateHelper.Simulate(pmodel);
        }

        public void Lock(ElementModel emodel)
        {
            Write(emodel);
            SimulateVariableUnit svunit = smodel.GetVariableUnit(emodel.ShowName, _Type(emodel.DataType));
            if (svunit.Islocked) return;
            smodel.SManager.Lock(svunit);
        }

        public void Remove(ElementModel emodel)
        {
            emodel = Get(emodel);
            if (emodel == null) return;
            if (--emodel.RefCount == 0)
            {
                SimulateVariableUnit svunit = smodel.GetVariableUnit(emodel.ShowName, _Type(emodel.DataType));
                if (svunit.CanClose)
                {
                    smodel.SManager.Remove(svunit);
                    dict.Remove(svunit);
                    svunit.ValueChanged -= OnValueChanged;
                }
            }
        }

        public void Remove(IEnumerable<ElementModel> emodels)
        {
            foreach (ElementModel emodel in emodels)
            {
                Remove(emodel);
            }
        }
        
        public void Unlock(ElementModel emodel)
        {
            SimulateVariableUnit svunit = smodel.GetVariableUnit(emodel.ShowName, _Type(emodel.DataType));
            if (!svunit.Islocked) return;
            smodel.SManager.Unlock(svunit);
            emodel.SetValue = String.Empty;
        }

        public void Write(ElementModel emodel)
        {
            SimulateVariableUnit svunit = smodel.GetVariableUnit(emodel.ShowName, _Type(emodel.DataType));
            switch (svunit.Type)
            {
                case "BIT":
                    switch (emodel.SetValue.ToUpper())
                    {
                        case "0": case "OFF": case "FALSE":
                            svunit.Value = 0; break;
                        case "1": case "ON": case "TRUE":
                            svunit.Value = 1; break;
                    }
                    break;
                case "WORD":
                    svunit.Value = Int32.Parse(emodel.SetValue); break;
                case "DWORD":
                    svunit.Value = Int64.Parse(emodel.SetValue); break;
                case "FLOAT":
                    svunit.Value = double.Parse(emodel.SetValue); break;
            }
            svunit.Set(smodel.SManager.DllModel);
            emodel.CurrentValue = svunit.Value.ToString();
        }

        public void Write(IEnumerable<ElementModel> emodels)
        {
            foreach (ElementModel emodel in emodels)
            {
                Write(emodel);
            }
        }
        
        public void Add(ICommunicationCommand cmd)
        {
            ElementModel emodel = null;
            if (cmd is GeneralWriteCommand)
            {
                GeneralWriteCommand gwcmd = (GeneralWriteCommand)cmd;
                Write(gwcmd.RefElements_A);
                Write(gwcmd.RefElements_B);
            }
            if (cmd is IntrasegmentWriteCommand)
            {

            }
            if (cmd is ForceCancelCommand)
            {
                ForceCancelCommand fccmd = (ForceCancelCommand)cmd;
                emodel = fccmd.RefElement;
                if (fccmd.IsAll)
                {
                    foreach (ElementModel _emodel in dict.Values)
                    {
                        Unlock(_emodel);
                    }
                }
                else
                {
                    Unlock(emodel);
                }
            }
        }

        public void Remove(ICommunicationCommand cmd)
        {
        }

        #endregion

        #region Event Handler

        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (sender is SimulateVariableUnit)
            {
                if (!IsRunning) return;
                SimulateVariableUnit svunit = (SimulateVariableUnit)sender;
                if (!dict.ContainsKey(svunit)) return;
                ElementModel emodel = dict[svunit];
                emodel.CurrentValue = svunit.Value.ToString();
            }
        }

        #endregion
    }
}
