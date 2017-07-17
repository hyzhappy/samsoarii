using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Threads
{
    public abstract class TimerThreadManager : BaseThreadManager
    {
        public TimerThreadManager(bool _isMTA) : base(_isMTA) { }

        protected override void _Thread_Handle()
        {
            if (ThActive) Handle();
        }

    }
}
