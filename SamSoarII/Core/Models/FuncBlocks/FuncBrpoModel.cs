using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SamSoarII.Core.Simulate;

namespace SamSoarII.Core.Models
{
    public class FuncBrpoModel : IBreakpoint, IDisposable
    {
        public FuncBrpoModel(FuncBlock _parent)
        {
            parent = _parent;
        }

        public void Dispose()
        {
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private FuncBlock parent;
        public FuncBlock Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                FuncBlock _parent = parent;
                this.parent = null;
                if (_parent != null)
                {
                    if (_parent.Breakpoint != null) _parent.Breakpoint = null;
                }
                this.parent = value;
                if (parent != null)
                {
                    if (parent.Breakpoint != this) parent.Breakpoint = this;
                }
            }
        }

        private int address;
        public int Address
        {
            get { return this.address; }
            set { this.address = value; PropertyChanged(this, new PropertyChangedEventArgs("Address")); }
        }

        private bool isenable;
        public bool IsEnable
        {
            get { return this.isenable; }
            set { this.isenable = value; PropertyChanged(this, new PropertyChangedEventArgs("IsEnable")); }
        }
        
        private bool isactive;
        public bool IsActive
        {
            get { return this.isactive; }
            set { this.isactive = value; PropertyChanged(this, new PropertyChangedEventArgs("IsActive")); }
        }

        private BreakpointCursor cursor;
        public BreakpointCursor Cursor
        {
            get { return this.cursor; }
            set { this.cursor = value; PropertyChanged(this, new PropertyChangedEventArgs("Cursor")); }
        }

        #endregion
    }
}
