using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    public class ValueStore : INotifyPropertyChanged, IDisposable
    {
        public ValueStore(ValueInfo _parent, ValueModel.Types _type, ValueModel.Bases _ibs = ValueModel.Bases.NULL, int _ifs = 0)
        {
            parent = _parent;
            if (parent != null)
                parent.PropertyChanged += OnParentPropertyChanged;
            type = _type;
            ibs = _ibs;
            ifs = _ifs;
        }
        
        public void Dispose()
        {
            if (parent != null)
                parent.PropertyChanged -= OnParentPropertyChanged;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ValueInfo parent;
        public ValueInfo Parent
        {
            get { return this.parent; }
        }
        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        protected ValueModel.Bases ibs;
        public ValueModel.Bases Intra { get { return this.ibs; } }

        protected int ifs;
        public int IntraOffset { get { return this.ifs; } }

        public string Name
        {
            get
            {
                switch (ibs)
                {
                    case ValueModel.Bases.V: return String.Format("{0:s}V{1:d}", parent.Name, ifs);
                    case ValueModel.Bases.Z: return String.Format("{0:s}Z{1:d}", parent.Name, ifs);
                    default: return parent.Name;
                }
            }
        }

        private ValueModel.Types type;
        public ValueModel.Types Type
        {
            get { return this.type; }
        }

        private object value;
        public object Value
        {
            get { return this.value; }
            set { this.value = value; PropertyChanged(this, new PropertyChangedEventArgs("Value")); }
        }
        public string ShowValue
        {
            get
            {
                switch (type)
                {
                    case ValueModel.Types.BOOL:
                        return (int)value == 1 ? "ON" : "OFF";
                    default:
                        return value.ToString();
                }
            }
        }

        public bool IsLocked
        {
            get { return parent.IsLocked; }
            set { parent.IsLocked = value; }
        }

        private int refnum;
        public int RefNum
        {
            get { return this.refnum; }
            set { this.refnum = value; }
        }

        private MonitorElement visual;
        public MonitorElement Visual
        {
            get { return this.visual; }
            set { this.visual = value; }
        }

        public event ValueStoreWriteEventHandler Post = delegate { };
        public void Write(object value, bool tolock = false, bool unsigned = false)
        {
            Post(this, new ValueStoreWriteEventArgs(this, value,
                ValueStoreWriteEventArgs.FLAGS_ISWRITE |
                (tolock ? ValueStoreWriteEventArgs.FLAGS_TOLOCK : 0) |
                (unsigned ? ValueStoreWriteEventArgs.FLAGS_UNSIGNED : 0)));
        }
        public void Unlock(bool all = false)
        {
            Post(this, new ValueStoreWriteEventArgs(this, null,
                all ? ValueStoreWriteEventArgs.FLAGS_UNLOCKALL : ValueStoreWriteEventArgs.FLAGS_UNLOCK));
        }
    }

    public class ValueStoreWriteEventArgs : EventArgs
    {
        private ValueStore store;
        public ValueStore Store { get { return this.store; } }
        
        public const int FLAGS_ISWRITE = 0x01;
        public const int FLAGS_TOLOCK = 0x02;
        public const int FLAGS_UNLOCK = 0x04;
        public const int FLAGS_UNLOCKALL = 0x08;
        public const int FLAGS_UNSIGNED = 0x10;
        private int flags;
        public int Flags { get { return this.flags; } }
        public bool IsWrite { get { return (flags & FLAGS_ISWRITE) != 0; } }
        public bool ToLock { get { return (flags & FLAGS_TOLOCK) != 0; } }
        public bool Unlock { get { return (flags & FLAGS_UNLOCK) != 0; } }
        public bool UnlockAll { get { return (flags & FLAGS_UNLOCKALL) != 0; } }
        public bool Unsigned { get { return (flags & FLAGS_UNSIGNED) != 0; } }

        private object tovalue;
        public object ToValue { get { return this.tovalue; } }
        
        public ValueStoreWriteEventArgs(ValueStore _store, object _tovalue, int _flags)
        {
            store = _store;
            flags = _flags;
            tovalue = _tovalue;
        }
    }

    public delegate void ValueStoreWriteEventHandler(object sender, ValueStoreWriteEventArgs e);
}
