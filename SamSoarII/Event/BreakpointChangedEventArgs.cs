using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class BreakpointChangedEventArgs
    {
        public LadderNetworkViewModel LNVModel { get; private set; }
        public BreakpointRect BPRect_old { get; private set; }
        public BreakpointRect BPRect_new { get; private set; }

        public BreakpointChangedEventArgs
        (
            LadderNetworkViewModel _lnvmodel,
            BreakpointRect _bprect_old,
            BreakpointRect _bprect_new
        )
        {
            LNVModel = _lnvmodel;
            BPRect_old = _bprect_old;
            BPRect_new = _bprect_new;
        }
    }
}
