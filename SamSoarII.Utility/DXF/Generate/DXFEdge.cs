using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Utility.DXF
{
    public class DXFEdge
    {
        private bool isReal;
        public bool IsReal { get { return isReal; } }
        private bool isSreached;
        public bool IsSreached { get { return isSreached; } set { isSreached = value; } }
        private DXFEntity entity;
        public DXFEntity Entity { get { return entity; } }
        public DXFEdge(DXFEntity entity)
        {
            this.entity = entity;
            //代表实线
            isReal = true;
            isSreached = false;
            ComputeVertex();
        }
        public DXFEdge(DXFVertex start, DXFVertex end)
        {
            Start = start;
            End = end;
            //代表虚线
            isReal = false;
            isSreached = false;
        }
        public DXFVertex Start, End;

        private void ComputeVertex()
        {
            switch (Entity.Type)
            {
                case EntityType.Line:
                    DXFLine line = (DXFLine)entity;
                    Start = new DXFVertex(line.StartP);
                    End = new DXFVertex(line.EndP);
                    break;
                case EntityType.Arc:
                    DXFArc arc = (DXFArc)entity;
                    Start = new DXFVertex(arc.StartP);
                    End = new DXFVertex(arc.EndP);
                    break;
                case EntityType.Ellipse:
                    DXFEllipse ellipsec = (DXFEllipse)entity;
                    Start = new DXFVertex(ellipsec.LongP);
                    End = new DXFVertex(ellipsec.LongP);
                    break;
                case EntityType.Circle:
                    DXFCircle circle = (DXFCircle)entity;
                    Point p = new Point(circle.CenterP.X - circle.radius, circle.CenterP.Y);
                    Start = new DXFVertex(p);
                    End = new DXFVertex(p);
                    break;
            }
        }
    }
}
