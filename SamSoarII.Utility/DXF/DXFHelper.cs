using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Utility.DXF
{
    public class DXFHelper
    {
        public static double ComputeRotateAngle(Point p1, Point p2)
        {
            if (Math.Abs(p1.Y - p2.Y) < 1E-10)
            {
                return 0;
            }
            return Math.Atan2(p2.Y - p1.Y, p1.X - p2.X);
        }

        public static double ComputeLength(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static Point ComputePoint(Point source, double radius, double angle)
        {
            Point p = new Point();
            p.X = source.X + radius * Math.Cos(angle / 180 * Math.PI);
            p.Y = source.Y + radius * Math.Sin(angle / 180 * Math.PI);
            return p;
        }
    }
}
