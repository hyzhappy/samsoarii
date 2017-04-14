using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterTab.Components
{
    public static class Extensions
    {
        #region Methods

        public static bool LessThan(this double t1, double t2)
        {
            return t1 < t2 && Math.Abs(t1 - t2) > 0.0001;
        }

        #endregion Methods
    }
}
