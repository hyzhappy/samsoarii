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
        public static IntPoint GetIntpointByDouble(double x, double y, int scale)
        {
            int xx = ((int)x) / scale;
            int yy = ((int)y) / scale;
            return new IntPoint() { X = xx, Y = yy };
        }

        public static IntPoint GetIntpointByDouble(double x, double y, int scalex, int scaley)
        {
            int xx = ((int)x) / scalex;
            int yy = ((int)y) / scaley;
            return new IntPoint() { X = xx, Y = yy };
        }
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
