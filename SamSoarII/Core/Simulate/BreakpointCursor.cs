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
                    LadderUnitModel unit = BreakpointManager.GetLadderUnit(address);
                    FuncBlock func = BreakpointManager.GetFuncUnit(address);
                    if (unit != null)
                        Current = unit;
                    else if (func != null)
                        Current = func;
                    else
                        address = -1;
                }
                if (address < 0) Current = null;
            }
        }

        private object current;
        private object Current
        {
            set
            {
                if (CurrentUnit != null) CurrentUnit.BPCursor = null;
                if (CurrentFunc != null) CurrentFunc.BPCursor = null;
                this.current = value;
                if (CurrentUnit != null) CurrentUnit.BPCursor = this;
                if (CurrentFunc != null) CurrentFunc.BPCursor = this;
            }
        }
        public LadderUnitModel CurrentUnit
        {
            get
            {
                return current is LadderUnitModel ? (LadderUnitModel)current : null;
            }
        }
        public FuncBlock CurrentFunc
        {
            get
            {
                return current is FuncBlock ? (FuncBlock)current : null;
            }
        }
        
        #endregion


    }
}
