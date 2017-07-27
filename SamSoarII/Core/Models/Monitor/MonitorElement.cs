using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class MonitorElement : IDisposable, INotifyPropertyChanged
    {
        public MonitorElement(MonitorTable _parent, ValueStore _store)
        {
            parent = _parent;
            datatype = (int)(_store.Type);
            Store = _store;
        }

        public MonitorElement(MonitorTable _parent, int _datatype, string _addrtype, int _startaddr, string _intratype, int _intraaddr)
        {
            parent = _parent;
            datatype = _datatype;
            string name = String.Format("{0:s}{1:d}", _addrtype, _startaddr);
            ValueInfo vinfo = ValueManager[name];
            Store = _FindStore(vinfo, _intratype, _intraaddr);
        }
        
        public MonitorElement(MonitorTable _parent, XElement xele)
        {
            parent = _parent;
            Load(xele);
        }
        
        public void Dispose()
        {
            Store = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private MonitorTable parent;
        public MonitorTable Parent { get { return this.parent; } }
        public ValueManager ValueManager { get { return parent.ValueManager; } }
        public InteractionFacade IFParent { get { return parent?.Parent?.Parent?.Parent; } }
        
        private ValueStore store;
        public ValueStore Store
        {
            get
            {
                return this.store;
            }
            set
            {
                ValueStore _store = store;
                this.store = value;
                if (_store != null)
                {
                    _store.RefNum--;
                    _store.PropertyChanged -= OnStorePropertyChanged;
                    IFParent.MNGSimu.Started -= OnSimulateStarted;
                    IFParent.MNGSimu.Aborted -= OnSimulateAborted;
                    IFParent.MNGComu.Started -= OnMonitorStarted;
                    IFParent.MNGComu.Aborted -= OnMonitorAborted;
                }
                if (store != null)
                {
                    store.RefNum++;
                    store.PropertyChanged += OnStorePropertyChanged;
                    IFParent.MNGSimu.Started += OnSimulateStarted;
                    IFParent.MNGSimu.Aborted += OnSimulateAborted;
                    IFParent.MNGComu.Started += OnMonitorStarted;
                    IFParent.MNGComu.Aborted += OnMonitorAborted;
                    //datatype = (int)(store.Type);
                    if (!IFParent.MNGSimu.IsAlive && !IFParent.MNGComu.IsAlive)
                        CurrentValue = "???";
                }
                if (_store != null && _store.RefNum == 0) _store.Parent.Stores.Remove(_store);
            }
        }
        
        private ValueStore _FindStore(ValueInfo vinfo, string _intratype, int _intraaddr)
        {
            ValueModel.Types type = ValueModel.Types.NULL;
            ValueModel.Bases ibase = ValueModel.Bases.NULL;
            switch (_intratype)
            {
                case "V": ibase = ValueModel.Bases.V; break;
                case "Z": ibase = ValueModel.Bases.Z; break;
            }
            type = (ValueModel.Types)(datatype);
            ValueStore _store = vinfo.Stores.Where(vs => vs.Type == type && vs.Intra == ibase && vs.IntraOffset == _intraaddr).FirstOrDefault();
            if (_store == null)
            {
                _store = new ValueStore(vinfo, type, ibase, _intraaddr);
                vinfo.Stores.Add(_store);
            }
            return _store;
        }

        public string AddrType { get { return store.Parent.Prototype.Base.ToString(); } }
        public int StartAddr { get { return store.Parent.Prototype.Offset; } }
        public bool IsIntrasegment { get { return store.Intra != ValueModel.Bases.NULL; } }
        public string IntrasegmentType { get { return store.Intra == ValueModel.Bases.NULL ? String.Empty : store.Intra.ToString(); } }
        public int IntrasegmentAddr { get { return store.IntraOffset; } }
        public string ShowName { get { return store.Name; } }
        public string FlagName
        {
            get
            {
                if (!IsIntrasegment)
                {
                    return String.Format("{0}_{1}_{2}", AddrType, StartAddr, DataType);
                }
                else
                {
                    return String.Format("{0}_{1}{2}_{3}_{4}", AddrType, IntrasegmentType, IntrasegmentAddr, StartAddr, DataType);
                }
            }
        }

        private bool unknown = false;
        public object CurrentValue
        {
            get
            {
                return unknown ? "???" : store.ShowValue;
            }
            set
            {
                if (value.ToString().Equals("???"))
                {
                    unknown = true;
                }
                else
                {
                    unknown = false;
                    store.Value = value;
                }
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue")); });     
            }
        }
        
        private string setvalue;
        public string SetValue
        {
            get
            {
                return setvalue;
            }
            set
            {
                setvalue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SetValue"));
            }
        }

        static private string[] bool_Showtypes = { "BOOL" };
        static private string[] word_ShowTypes = { "WORD", "UWORD", "DWORD", "UDWORD", "BCD", "FLOAT" };
        public string[] ShowTypes
        {
            get
            {
                if (store.Type == ValueModel.Types.BOOL) return bool_Showtypes;
                return word_ShowTypes;
            }
        }
        public string ShowType
        {
            get
            {
                switch (DataType)
                {
                    case 0: return "BOOL";
                    case 1: return "WORD";
                    case 2: return "UWORD";
                    case 3: return "DWORD";
                    case 4: return "UDWORD";
                    case 5: return "BCD";
                    case 6: return "FLOAT";
                    default: return "null";
                }
            }
            set
            {
                switch (value)
                {
                    case "BOOL": DataType = 0; break;
                    case "WORD": DataType = 1; break;
                    case "UWORD": DataType = 2; break;
                    case "DWORD": DataType = 3; break;
                    case "UDWORD": DataType = 4; break;
                    case "BCD": DataType = 5; break;
                    case "FLOAT": DataType = 6; break;
                }
            }
        }

        private int datatype;
        public int DataType
        {
            get
            {
                return this.datatype;
            }
            set
            {
                this.datatype = value;
                Store = _FindStore(store.Parent, IntrasegmentType, IntrasegmentAddr);
                PropertyChanged(this, new PropertyChangedEventArgs("DataType"));
                PropertyChanged(this, new PropertyChangedEventArgs("ShowType"));
                PropertyChanged(this, new PropertyChangedEventArgs("SelectIndex"));
            }
        }
        
        public int ByteCount
        {
            get
            {
                switch (DataType)
                {
                    case 1:
                    case 2:
                    case 5:
                        return 2;
                    case 3:
                    case 4:
                        return 4;
                    case 6:
                        return 4;
                    default:
                        return 1;
                }
            }
        }
        
        public int SelectIndex
        {
            get
            {
                if (DataType == 0)
                {
                    return DataType;
                }
                else
                {
                    return DataType - 1;
                }
            }
            set
            {
                if (DataType != 0)
                {
                    if (DataType != value + 1)
                    {
                        DataType = value + 1;
                    }
                }
            }
        }

        #endregion

        #region Load & Save

        public void Load(XElement xele)
        {
            datatype = int.Parse(xele.Attribute("DataType").Value);
            string name = xele.Attribute("Name").Value;
            ValueInfo vinfo = ValueManager[name];
            string intratype = xele.Attribute("IntraType").Value;
            int intraaddr = int.Parse(xele.Attribute("IntraAddr").Value);
            Store = _FindStore(vinfo, intratype, intraaddr);
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Name", ShowName);
            xele.SetAttributeValue("DataType", DataType);
            xele.SetAttributeValue("IntraType", IntrasegmentType);
            xele.SetAttributeValue("IntraAddr", IntrasegmentAddr);
        }

        #endregion

        #region Event Handler

        private void OnStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Value":
                    unknown = false;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue")); });
                    break;
            }
        }

        private void OnSimulateStarted(object sender, RoutedEventArgs e)
        {
            unknown = false;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
        }
        
        private void OnSimulateAborted(object sender, RoutedEventArgs e)
        {
            CurrentValue = "???";
        }

        private void OnMonitorStarted(object sender, RoutedEventArgs e)
        {
            unknown = false;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
        }

        private void OnMonitorAborted(object sender, RoutedEventArgs e)
        {
            CurrentValue = "???";
        }

        #endregion
    }
}
