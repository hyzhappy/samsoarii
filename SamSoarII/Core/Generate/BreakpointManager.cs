using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public abstract class BreakpointManager
    {
        static private int count = 0;
        static private Dictionary<int, LadderUnitModel> bvmodels
            = new Dictionary<int, LadderUnitModel>();
        static private Dictionary<int, FuncBlock> fblocks
            = new Dictionary<int, FuncBlock>();

        static public void Initialize()
        {
            count = 0;
            bvmodels.Clear();
        }

        static public void Register(LadderUnitModel lumodel)
        {
            lumodel.BPAddress = ++count;
            bvmodels.Add(lumodel.BPAddress, lumodel);
        }

        static public void Register(FuncBlock fblock)
        {
            fblock.IsBreakpoint = true;
            fblock.BPAddress = ++count;
            fblocks.Add(fblock.BPAddress, fblock);
        }

        static public LadderUnitModel GetLadderUnit(int bpaddr)
        {
            return bvmodels.ContainsKey(bpaddr)
                ? bvmodels[bpaddr] : null;
        }

        static public FuncBlock GetFuncUnit(int bpaddr)
        {
            return fblocks.ContainsKey(bpaddr)
                ? fblocks[bpaddr] : null;
        }
    }
}
