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

        static public BaseViewModel GetBVModel(int bpaddr)
        {
            return bvmodels.ContainsKey(bpaddr)
                ? bvmodels[bpaddr] : null;
        }
    }
}
