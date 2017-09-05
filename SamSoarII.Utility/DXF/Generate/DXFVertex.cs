using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Utility.DXF
{
    public class DXFVertex : IVertex
    {
        private Point p;
        public Point P { get { return p; } }
        private bool isSreached;
        public bool IsSreached { get { return isSreached; } set { isSreached = value; } }
        Point IVertex.P
        {
            get
            {
                return P;
            }
        }

        public DXFVertex(Point p)
        {
            this.p = p;
            isSreached = false;
        }
        public int CompareTo(IVertex other)
        {
            int value = ComputeOffset(p.Y, other.P.Y);
            if (value != 0) return value;
            else return ComputeOffset(p.X, other.P.X);
        }
        private int ComputeOffset(double x1,double x2)
        {
            if (Math.Abs(x1 - x2) < 0.0001) return 0;
            if (x1 > x2) return 1;
            else return -1;
        }
    }
}
