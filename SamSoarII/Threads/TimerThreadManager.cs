using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Threads
{
    public abstract class TimerThreadManager : BaseThreadManager
    {
        public TimerThreadManager(bool _isMTA, bool forceabort = false) : base(_isMTA, forceabort) { }

        protected override void _Thread_Handle()
        {
            if (ThActive) Handle();
        }

    }
}
