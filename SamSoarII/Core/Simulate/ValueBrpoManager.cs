using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SamSoarII.Core.Models;
using SamSoarII.Threads;
using System.Threading;

namespace SamSoarII.Core.Simulate
{
    public class ValueBrpoManager : TimerThreadManager
    {
        public ValueBrpoManager(SimulateManager _parent) : base(false)
        {
            parent = _parent;
            TimeSpan = 500;
        }
        
        #region Number

        private SimulateManager parent;
        public SimulateManager Parent { get { return this.parent; } }
        public InteractionFacade IFParent { get { return Parent?.IFParent; } }
        public ValueBrpoModel Core { get { return IFParent?.MDProj?.ValueBrpo; } }

        #endregion

        #region Thread

        protected override void Handle()
        {
            if (Core != null && Core.IsModified && SimulateDllModel.IsDllAlive() > 0)
            {
                Core.IsModified = false;
                while (SimulateDllModel.FreeItrpDll() != 0)
                    Thread.Sleep(10);
                Core.Compile();
                string cpath = Utility.FileHelper.AppRootPath;
                string dllpath = String.Format(@"{0:s}\simug\simuitrp.dll", cpath);
                if (!ThAlive) return;
                while (SimulateDllModel.SetItrpDll(dllpath) != 0)
                    Thread.Sleep(10);
            }
        }

        protected override void Before()
        {
            base.Before();
            Core.IsModified = true;
        }

        #endregion
    }
}
