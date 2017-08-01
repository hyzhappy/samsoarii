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
            parent = _parent;
            if (parent != null)
                parent.PropertyChanged += OnParentPropertyChanged;
            type = _type;
            ibs = _ibs;
            ifs = _ifs;
            flag = _flag;
            value = 0;
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
        
        private object value;
        public object Value
        {
            get { return this.value; }
            set
            {
                if (this.value.Equals(value)) return;
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
                        return (int)value == 1 ? "ON" : "OFF";
                    case ValueModel.Types.UWORD:
                        return ((ushort)value).ToString();
                    case ValueModel.Types.UDWORD:
                        return ((uint)value).ToString();
                    case ValueModel.Types.HEX:
                        return String.Format("0x{0:x4}", (short)value);
                    case ValueModel.Types.DHEX:
                        return String.Format("0x{0:x8}", (int)value);
                    case ValueModel.Types.BCD:
                        return (ushort)value > 9999 ? "???" : ValueConverter.ToBCD((ushort)value).ToString(); break;
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
        public void Write(object value, bool tolock = false)
        {
            Post(this, new ValueStoreWriteEventArgs(this, value,
                ValueStoreWriteEventArgs.FLAGS_ISWRITE |
                (tolock ? ValueStoreWriteEventArgs.FLAGS_TOLOCK : 0)));
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
        
        public ValueStoreWriteEventArgs(ValueStore _store, object _tovalue, int _flags)
        {
            store = _store;
            flags = _flags;
            tovalue = _tovalue;
        }
    }

    public delegate void ValueStoreWriteEventHandler(object sender, ValueStoreWriteEventArgs e);
}
