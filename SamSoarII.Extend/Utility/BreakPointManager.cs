using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Extend.Utility
{
    public class BreakPointManager
    {
        static private int count = 0;
        static private Dictionary<int, BaseViewModel> bvmodels
            = new Dictionary<int, BaseViewModel>();
        static private Dictionary<int, FuncBlock> fblocks
            = new Dictionary<int, FuncBlock>();
        
        static public void Initialize()
        {
            count = 0;
            bvmodels.Clear();
        }

        static public void Register(BaseViewModel bvmodel)
        {
            bvmodel.BPAddress = ++count;
            bvmodels.Add(bvmodel.BPAddress, bvmodel);
        }

        static public void Register(FuncBlock fblock)
        {
            fblock.IsBreakpoint = true;
            fblock.BPAddress = ++count;
            fblocks.Add(fblock.BPAddress, fblock);
        }

        static public BaseViewModel GetBVModel(int bpaddr)
        {
            return bvmodels.ContainsKey(bpaddr)
                ? bvmodels[bpaddr] : null;
        }

        static public FuncBlock GetFBlock(int bpaddr)
        {
            return fblocks.ContainsKey(bpaddr)
                ? fblocks[bpaddr] : null;
        }
    }
}
