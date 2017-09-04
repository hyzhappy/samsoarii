using SamSoarII.Core.Models;
using SamSoarII.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Threads
{
    public class AutoInstManager : TimerThreadManager
    {
        public AutoInstManager(InteractionFacade _parent) : base(false, true)
        {
            parent = _parent;
            TimeSpan = GlobalSetting.InstTimeSpan * 1000;
        }

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.Parent; } }
        
        protected override void Handle()
        {
            try
            {
                if (!GlobalSetting.IsInstByTime) return;
                ProjectModel proj = parent.MDProj;
                if (proj != null)
                {
                    foreach (LadderDiagramModel ldmodel in proj.Diagrams)
                    {
                        foreach (LadderNetworkModel lnmodel in ldmodel.Children)
                        {
                            if (lnmodel.Inst.IsModified)
                                lnmodel.Inst.Update();
                            if (!ThAlive || !ThActive) return;
                        }
                        if (!ThAlive || !ThActive) return;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
