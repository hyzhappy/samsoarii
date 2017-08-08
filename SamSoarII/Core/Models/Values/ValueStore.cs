using SamSoarII.Utility;
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
        public ValueStore(ValueInfo _parent, ValueModel.Types _type, ValueModel.Bases _ibs = ValueModel.Bases.NULL, int _ifs = 0, int _flag = 1)
        {
            Parent = _parent;
            type = _type;
            ibs = _ibs;
            ifs = _ifs;
            flag = _flag;
            value = 0;
            isnew = true;
        }
        
        public void Dispose()
        {
            Parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int id;
        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        private ValueInfo parent;
        public ValueInfo Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                ValueInfo _parent = parent;
                this.parent = null;
                if (_parent != null) 
                    _parent.PropertyChanged -= OnParentPropertyChanged;
                this.parent = value;
                if (parent != null)
                    parent.PropertyChanged += OnParentPropertyChanged;
            }
        }
        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        public ValueModel.Bases Base { get { return parent.Prototype.Base; } }

        public int Offset { get { return parent.Prototype.Offset; } }

        public int flag;
        public int Flag { get { return this.flag; } }
        
        protected ValueModel.Bases ibs;
        public ValueModel.Bases Intra { get { return this.ibs; } }

        protected int ifs;
        public int IntraOffset { get { return this.ifs; } }

        public string Name
        {
            get
            {
                string name = IsWordBit ? String.Format("{0:s}.{1:X}", parent.Name, flag)
                    : IsBitWord || IsBitDoubleWord ? String.Format("K{1:d}{0:s}", parent.Name, flag)
                    : parent.Name;
                switch (ibs)
                {
                    case ValueModel.Bases.V: return String.Format("{0:s}V{1:d}", name, ifs);
                    case ValueModel.Bases.Z: return String.Format("{0:s}Z{1:d}", name, ifs);
                    default: return name;
                }
            }
        }

        public string BaseName
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

        public bool IsWordBit
        {
            get { return Type == ValueModel.Types.BOOL 
                    && (Base == ValueModel.Bases.D || Base == ValueModel.Bases.V || Base == ValueModel.Bases.Z); }
        }

        public bool IsBitWord
        {
            get { return (Type == ValueModel.Types.WORD || Type == ValueModel.Types.UWORD || Type == ValueModel.Types.BCD || Type == ValueModel.Types.HEX) 
                    && (Base == ValueModel.Bases.X || Base == ValueModel.Bases.Y || Base == ValueModel.Bases.M || Base == ValueModel.Bases.S); }
        }
        
        public bool IsBitDoubleWord
        {
            get { return (Type == ValueModel.Types.DWORD || Type == ValueModel.Types.UDWORD || Type == ValueModel.Types.DHEX) 
                    && (Base == ValueModel.Bases.X || Base == ValueModel.Bases.Y || Base == ValueModel.Bases.M || Base == ValueModel.Bases.S); }
        }

        public int ByteCount
        {
            get
            {
                if (IsWordBit)
                    return 2;
                if (IsBitWord || IsBitDoubleWord)
                    return flag;
                switch (type)
                {
                    case ValueModel.Types.BOOL:
                        return 1;
                    case ValueModel.Types.WORD:
                    case ValueModel.Types.UWORD:
                    case ValueModel.Types.BCD:
                    case ValueModel.Types.HEX:
                        return 2;
                    case ValueModel.Types.DWORD:
                    case ValueModel.Types.UDWORD:
                    case ValueModel.Types.FLOAT:
                    case ValueModel.Types.DHEX:
                        return 4;
                    default:
                        return 0;
                }
            }
        }

        private bool isnew;
        private object value;
        public object Value
        {
            get { return this.value; }
            set
            {
                if (!isnew && this.value.Equals(value)) return;
                isnew = false;
                this.value = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
        public string ShowValue
        {
            get
            {
                switch (type)
                {
                    case ValueModel.Types.BOOL:
                        return byte.Parse(value.ToString()) == 1 ? "ON" : "OFF";
                    case ValueModel.Types.HEX:
                        return String.Format("0x{0:x4}", ushort.Parse(value.ToString()));
                    case ValueModel.Types.DHEX:
                        return String.Format("0x{0:x8}", uint.Parse(value.ToString()));
                    case ValueModel.Types.BCD:
                        return (ushort)value > 9999 ? "???" : ValueConverter.ToBCD(ushort.Parse(value.ToString())).ToString();
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

        private int visualrefnum;
        public int VisualRefNum
        {
            get { return this.visualrefnum; }
            set { this.visualrefnum = value; }
        }
        
        public event ValueStoreWriteEventHandler Post = delegate { };
        public void Write(object value, bool tolock = false, ValueModel.Types _type = ValueModel.Types.NULL)
        {
            Post(this, new ValueStoreWriteEventArgs(this, value,
                ValueStoreWriteEventArgs.FLAGS_ISWRITE |
                (tolock ? ValueStoreWriteEventArgs.FLAGS_TOLOCK : 0),
                _type != ValueModel.Types.NULL ? _type : type));
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
        private int flags;
        public int Flags { get { return this.flags; } }
        public bool IsWrite { get { return (flags & FLAGS_ISWRITE) != 0; } }
        public bool ToLock { get { return (flags & FLAGS_TOLOCK) != 0; } }
        public bool Unlock { get { return (flags & FLAGS_UNLOCK) != 0; } }
        public bool UnlockAll { get { return (flags & FLAGS_UNLOCKALL) != 0; } }

        private object tovalue;
        public object ToValue { get { return this.tovalue; } }
        private ValueModel.Types type;
        public ValueModel.Types Type { get { return this.type; } }
        
        public ValueStoreWriteEventArgs(ValueStore _store, object _tovalue, int _flags, ValueModel.Types _type = ValueModel.Types.NULL)
        {
            store = _store;
            flags = _flags;
            tovalue = _tovalue;
            type = _type;
        }
    }

    public delegate void ValueStoreWriteEventHandler(object sender, ValueStoreWriteEventArgs e);
}
