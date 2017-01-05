using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility
{
    public struct IntPoint : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int CompareTo(object obj)
        {
            var p = (IntPoint)obj;
            if (Y < p.Y)
            {
                return -1;
            }
            if (Y > p.Y)
            {
                return 1;
            }
            if (X < p.X)
            {
                return -1;
            }
            if (X > p.X)
            {
                return 1;
            }
            return 0;
        }
    }
}
