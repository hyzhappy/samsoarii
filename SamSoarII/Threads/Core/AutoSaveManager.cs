using SamSoarII.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Threads
{
    public class AutoSaveManager : TimerThreadManager
    {
        public AutoSaveManager(InteractionFacade _parent) : base(false)
        {
            parent = _parent;
            TimeSpan = GlobalSetting.SaveTimeSpan * 60 * 1000;
        }

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.Parent; } }

        protected override void Handle()
        {
            try
            {
                if (GlobalSetting.IsSavedByTime && parent.MDProj != null && parent.MDProj.FileName != null)
                    parent.SaveProject();
            }
            catch (Exception)
            {
                Handle();
            }
        }
    }
}
