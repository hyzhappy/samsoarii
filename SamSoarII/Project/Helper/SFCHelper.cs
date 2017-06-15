using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSoarII.AppMain.Project.Program.SFC;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using SamSoarII.LadderInstModel;

namespace SamSoarII.AppMain.Project.Helper
{
    public class SFCHelper
    {
        static private SFCTimerModel[] timers
            = new SFCTimerModel[256];
        
        static public void Initialize(ProjectModel pmodel)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                timers[i] = null;
            }
            Initialize(pmodel.MainRoutine);
            
        }
        static private void Initialize(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                foreach (TONViewModel bvmodel in lnvmodel.GetElements())
                {
                    IValueModel ivmodel = ((TONModel)(bvmodel.Model)).TimerValue;
                    if (ivmodel is TVWordValue)
                    {
                        int offset = (int)(ivmodel.Index);
                        // TO BE CONTINUE...
                    }
                }
            }
        }
    }
}
