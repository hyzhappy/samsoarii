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
        static private SFCStatusModel[] status
            = new SFCStatusModel[1000];
        
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
                foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
                {
                    BaseModel bmodel = bvmodel.Model;
                    if (bmodel == null) continue;
                    if (bmodel is TONModel)
                    {
                        TONModel tmodel = (TONModel)(bmodel);
                        IValueModel ivmodel = tmodel.TimerValue;
                        if (ivmodel.IsVariable) continue;
                        if (ivmodel is TVWordValue)
                        {
                            int index = (int)(ivmodel.Index);
                            timers[index] = new SFCTimerModel(tmodel);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < bmodel.ParaCount; i++)
                        {
                            IValueModel ivmodel = bmodel.GetPara(i);
                            if (ivmodel is SBitValue)
                            {
                                int index = (int)(ivmodel.Index);
                                if (status[index] == null)
                                {
                                    status[index] = new SFCStatusModel(ldvmodel);
                                }
                                else if (status[index].LDVModel != ldvmodel)
                                {
                                    status[index] = new SFCInvalidStatusModel();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
