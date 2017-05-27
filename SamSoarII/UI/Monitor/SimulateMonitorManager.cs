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
using SamSoarII.LadderInstViewModel.Monitor;
using SamSoarII.UserInterface;

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
            IsRunning = true;
            foreach (KeyValuePair<SimulateVariableUnit, ElementModel> kvp in dict)
            {
                UpdateValue(kvp.Value, kvp.Key);
            }
            Started(this, new RoutedEventArgs());
        }

        public void Abort()
        {
            IsRunning = false;
            foreach (KeyValuePair<SimulateVariableUnit, ElementModel> kvp in dict)
            {
                UpdateValue(kvp.Value, kvp.Key);
            }
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
                emodel = dict[svunit];
                emodel.RefCount++;
            }
            UpdateValue(emodel, svunit);
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
                for (int i = 0; i < table.Elements.Count(); i++)
                {
                    table.Elements[i] = Get(table.Elements[i]);
                }
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
                            svunit.Value = 1;
                            break;
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
            //emodel.CurrentValue = svunit.Value.ToString();
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
                    smodel.SManager.UnlockAll();
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

        public void Handle(IMoniValueModel mvmodel, ElementValueModifyEventArgs e)
        {
            if (mvmodel is SimuMoniValueModel)
            {
                SimuMoniValueModel smvmodel = (SimuMoniValueModel)mvmodel;
                switch (e.Type)
                {
                    case ElementValueModifyEventType.ForceON:
                    case ElementValueModifyEventType.ForceOFF:
                        smvmodel.Lock(e.Value, e.VarType);
                        break;
                    case ElementValueModifyEventType.WriteON:
                    case ElementValueModifyEventType.WriteOFF:
                    case ElementValueModifyEventType.Write:
                        smvmodel.Write(e.Value, e.VarType);
                        break;
                    case ElementValueModifyEventType.ForceCancel:
                        smvmodel.Unlock();
                        break;
                    case ElementValueModifyEventType.AllCancel:
                        smvmodel.UnlockAll();
                        break;
                }
            }
            if (mvmodel is ElementModel)
            {
                ElementModel element = (ElementModel)mvmodel;
                ICommunicationCommand command = null;
                switch (e.Type)
                {
                    case ElementValueModifyEventType.ForceON:
                        element.SetValue = "ON";
                        Lock(element);
                        break;
                    case ElementValueModifyEventType.ForceOFF:
                        element.SetValue = "OFF";
                        Lock(element);
                        break;
                    case ElementValueModifyEventType.ForceCancel:
                        element.SetValue = String.Empty;
                        command = new ForceCancelCommand(false, element);
                        Add(command);
                        break;
                    case ElementValueModifyEventType.AllCancel:
                        element.SetValue = String.Empty;
                        command = new ForceCancelCommand(true, element);
                        Add(command);
                        break;
                    case ElementValueModifyEventType.WriteOFF:
                        element.SetValue = "OFF";
                        Write(element);
                        break;
                    case ElementValueModifyEventType.WriteON:
                        element.SetValue = "ON";
                        Write(element);
                        break;
                    case ElementValueModifyEventType.Write:
                        element.SetValue = e.Value;
                        Write(element);
                        break;
                }
            }
        }

        #region Event Handler

        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (sender is SimulateVariableUnit)
            {
                if (!IsRunning) return;
                SimulateVariableUnit svunit = (SimulateVariableUnit)sender;
                if (!dict.ContainsKey(svunit)) return;
                ElementModel emodel = dict[svunit];
                UpdateValue(emodel, svunit);
            }
        }

        private void UpdateValue(ElementModel emodel, SimulateVariableUnit svunit)
        {
            if (!IsRunning)
            {
                emodel.CurrentValue = "????";
                emodel.SetValue = String.Empty;
                return;
            }
            if (svunit.Type.Equals("BIT"))
            {
                switch (svunit.Value.ToString())
                {
                    case "0":
                    case "OFF":
                    case "FALSE":
                        emodel.CurrentValue = "OFF";
                        break;
                    case "1":
                    case "ON":
                    case "TRUE":
                        emodel.CurrentValue = "ON";
                        break;
                }
            }
            else
            {
                emodel.CurrentValue = svunit.Value.ToString();
            }
        }

        #endregion
    }
}
