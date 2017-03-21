using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Shell
{
    public class IntPointComparer : IComparer<IntPoint>
    {
        public int Compare(IntPoint x, IntPoint y)
        {
            if (x.X < y.X) return -1;
            if (x.X > y.X) return 1;
            if (x.Y < y.Y) return -1;
            if (x.Y > y.Y) return 1;
            return 0;
        }
    }

    public class IntPoint
    {
        public int X;
        public int Y;
        public IntPoint(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }

        internal static IntPoint GetIntpointByDouble(double x, double y, int v)
        {
            return new IntPoint((int)(Math.Floor(x / v)), (int)(Math.Floor(y / v)));
        }
    }
}
