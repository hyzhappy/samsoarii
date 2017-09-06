using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    public class ValueInfo : INotifyPropertyChanged, IDisposable
    {
        private class UnitComparer : IComparer<LadderUnitModel>
        {
            public int Compare(LadderUnitModel unit1, LadderUnitModel unit2)
            {
                LadderNetworkModel net1 = unit1.Parent;
                LadderNetworkModel net2 = unit2.Parent;
                LadderDiagramModel dia1 = net1?.Parent;
                LadderDiagramModel dia2 = net2?.Parent;
                if (dia1 == null) return 1;
                if (dia2 == null) return -1;
                if (net1 == null) return 1;
                if (net2 == null) return -1;
                int ret = 0;
                ret = dia1.IsMainLadder.CompareTo(dia2.IsMainLadder);
                if (ret != 0) return ret;
                ret = dia1.Name.CompareTo(dia2.Name);
                if (ret != 0) return ret;
                ret = net1.ID.CompareTo(net2.ID);
                if (ret != 0) return ret;
                ret = unit1.Y.CompareTo(unit2.Y);
                if (ret != 0) return ret;
                ret = unit1.X.CompareTo(unit2.X);
                return ret;
            }
        }

        public ValueInfo(ValueManager _parent, ValuePrototype _prototype, int _dataaddr)
        {
            parent = _parent;
            prototype = _prototype;
            dataaddr = _dataaddr;
            values = new ObservableCollection<ValueModel>();
            stores = new ObservableCollection<ValueStore>();
            values.CollectionChanged += OnValueCollectionChanged;
            stores.CollectionChanged += OnStoreCollectionChanged;
        }
        
        public void Dispose()
        {
            foreach (ValueStore store in stores)
            {
                store.Post -= OnReceiveValueStoreEvent;
                store.Dispose();
            }
            values.Clear();
            stores.Clear();
            values.CollectionChanged -= OnValueCollectionChanged;
            stores.CollectionChanged -= OnStoreCollectionChanged;
            ValuesChanged = null;
            StoresChanged = null;
            prototype.Dispose();
            prototype = null;
            values = null;
            stores = null;
            comment = null;
            alias = null;
            initmodel = null;
            PropertyChanged = null;
            parent = null;
        }

        public void Initialize(bool storedispose = true)
        {
            foreach (ValueStore store in stores)
            {
                store.Post -= OnReceiveValueStoreEvent;
                if (storedispose) store.Dispose();
            }
            if (comment != null) Comment = null;
            if (alias != null) Alias = null;
            values.Clear();
            if (units != null) units.Clear();
            stores.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ValueManager parent;
        public ValueManager Parent { get { return this.parent; } }

        private ValuePrototype prototype;
        public ValuePrototype Prototype { get { return this.prototype; } }
        public string Name { get { return prototype.Text; } }

        private int dataaddr;
        public int DataAddr { get { return dataaddr; } }

        private ObservableCollection<ValueModel> values;
        public IList<ValueModel> Values { get { return this.values; } }
        public event NotifyCollectionChangedEventHandler ValuesChanged = delegate { };
        private void OnValueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ValuesChanged(this, e);
        }
        public void Add(ValueModel value)
        {
            if (this != parent.EmptyInfo)
                Values.Add(value);
            if (value.Store != null && value.Store.Parent == parent.EmptyInfo && this != parent.EmptyInfo)
                value.Store = null;
            if (value.Store != null)
            {
                value.Store.Parent = this;
                value.Store.RefNum++;
                value.Store.VisualRefNum += value.Parent.View != null ? 1 : 0;
                if (this != parent.EmptyInfo && value.Store.RefNum == 1)
                    Stores.Add(value.Store);
            }
            else
            {
                int flag = value.IsWordBit ? (value.Offset & 15)
                    : value.IsBitWord || value.IsBitDoubleWord ? value.Size : 1;
                IEnumerable<ValueStore> fits = Stores.Where(s => s.Name.Equals(value.Text) && s.Type == value.Type);
                ValueStore vstore = fits.FirstOrDefault();
                if (vstore == null)
                {
                    vstore = new ValueStore(this, value.Type, value.Intra, value.IntraOffset, flag);
                    if (this != parent.EmptyInfo)
                        Stores.Add(vstore);
                }
                vstore.RefNum++;
                vstore.VisualRefNum += value.Parent.View != null ? 1 : 0;
                value.Store = vstore;
            }
        }
        public void Remove(ValueModel value)
        {
            if (this != parent.EmptyInfo)
                Values.Remove(value);
            if (value.Store != null)
            {
                value.Store.RefNum--;
                value.Store.VisualRefNum -= value.Parent.View != null ? 1 : 0;
                if (this != parent.EmptyInfo)
                {
                    if (value.Store.RefNum == 0)
                        Stores.Remove(value.Store);
                    value.Store = null;
                }
            }
        }

        private ObservableCollection<ValueStore> stores;
        public IList<ValueStore> Stores { get { return this.stores; } }
        public event NotifyCollectionChangedEventHandler StoresChanged = delegate { };
        private void OnStoreCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ValueStore store in e.NewItems)
                    store.Post += OnReceiveValueStoreEvent;
            if (e.OldItems != null)
                foreach (ValueStore store in e.OldItems)
                {
                    store.Post -= OnReceiveValueStoreEvent;
                    store.Dispose();
                }
            StoresChanged(this, e);
        }
        
        private Dictionary<LadderUnitModel, int> units;
        public IEnumerable<LadderUnitModel> UsedUnits { get { return units != null ? units.Keys.Where(u => u.IsUsed) : new LadderUnitModel[] { }; } }
        public IList<LadderUnitModel> Units
        {
            get
            {
                if (units == null) return new LadderUnitModel[] { };
                List<LadderUnitModel> ret = units.Keys.ToList();
                ret.Sort(new UnitComparer());
                return ret;
            }
        }
        public void Add(LadderUnitModel unit)
        {
            if (this == parent.EmptyInfo) return;
            if (units == null) units = new Dictionary<LadderUnitModel, int>();
            if (!units.ContainsKey(unit))
            {
                units.Add(unit, 1);
                PropertyChanged(this, new PropertyChangedEventArgs("Units"));
            }
            else
                units[unit]++;
        }
        public void Remove(LadderUnitModel unit)
        {
            if (this == parent.EmptyInfo) return;
            if (units == null) return;
            if (!units.ContainsKey(unit)) return;
            if (--units[unit] == 0)
            {
                units.Remove(unit);
                PropertyChanged(this, new PropertyChangedEventArgs("Units"));
            }
        }

        private string comment;
        public string Comment
        {
            get
            {
                return comment == null ? String.Empty : comment;
            }
            set
            {
                this.comment = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Comment"));
                foreach (ValueModel vmodel in values)
                    vmodel.Parent?.Invoke(LadderUnitAction.UPDATE);
            }
        }

        private string alias;
        public string Alias
        {
            get
            {
                return alias == null ? String.Empty : alias;
            }
            set
            {
                this.alias = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Alias"));
            }
        }

        private IElementInitializeModel initmodel;
        public IElementInitializeModel InitModel
        {
            get
            {
                return this.initmodel;
            }
            set
            {
                IElementInitializeModel _initmodel = initmodel;
                this.initmodel = null;
                if (_initmodel != null && _initmodel.Parent != null)
                    _initmodel.Parent = null;
                this.initmodel = value;
                if (initmodel != null && initmodel.Parent != this)
                    initmodel.Parent = this;
                PropertyChanged(this, new PropertyChangedEventArgs("InitModel"));
            }
        }

        private bool islocked;
        public bool IsLocked
        {
            get { return this.islocked; }
            set { this.islocked = value; PropertyChanged(this, new PropertyChangedEventArgs("IsLocked")); }
        }

        public bool IsUsed
        {
            get { return comment != null || alias != null || initmodel != null; }
        }

        #endregion

        #region Event Handler

        public event ValueStoreWriteEventHandler PostValueStoreEvent = delegate { }; 

        private void OnReceiveValueStoreEvent(object sender, ValueStoreWriteEventArgs e)
        {
            PostValueStoreEvent(sender, e);
        }

        #endregion
    }
    
}
