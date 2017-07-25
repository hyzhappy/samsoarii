using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Simulate
{
    public class BreakpointCursor : IDisposable
    {
        public BreakpointCursor(SimulateViewer _parent)
        {
            parent = _parent;
        }

        public void Dispose()
        {
            parent = null;
        }

        #region Number

        private SimulateViewer parent;
        public SimulateViewer Parent { get { return this.parent; } }
        public BreakpointManager BreakpointManager { get { return parent.Parent.MNGBrpo; } }

        private int address;
        public int Address
        {
            get
            {
                return this.address;
            }
            set
            {
                if (address == value) return;
                this.address = value;
                if (address >= 0)
                {
                    if (address < BreakpointManager.Items.Count)
                        Current = BreakpointManager.Items[address];
                    else
                        address = -1;
                }
                if (address < 0) Current = null;
            }
        }

        private IBreakpoint current;
        public IBreakpoint Current
        {
            get
            {
                return this.current;
            }
            set
            {
                if (current == value) return;
                IBreakpoint _current = current;
                this.current = null;
                if (_current != null)
                {
                    address = -1;
                    if (_current.Cursor != null) _current.Cursor = null;
                }
                this.current = value;
                if (current != null)
                {
                    address = current.Address;
                    if (current.Cursor != this) current.Cursor = this;
                }
            }
        }
        
        #endregion


    }
}
