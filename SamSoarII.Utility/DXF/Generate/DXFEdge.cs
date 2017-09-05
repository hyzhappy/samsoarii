using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Utility.DXF
{
    public class DXFEdge
    {
        private bool isSreached;
        public bool IsSreached { get { return isSreached; } set { isSreached = value; } }
        private DXFEntity entity;
        public DXFEntity Entity { get { return entity; } }
        public DXFEdge(DXFEntity entity)
        {
            this.entity = entity;
            isSreached = false;
            ComputeVertex();
        }
        public DXFEdge(DXFVertex start, DXFVertex end, DXFModel model)
        {
            Start = start;
            End = end;
            isSreached = false;
            entity = new DXFLine(model, start.P, end.P);
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
                    End = Start;
                    break;
                case EntityType.Circle:
                    DXFCircle circle = (DXFCircle)entity;
                    Point p = new Point(circle.CenterP.X - circle.radius, circle.CenterP.Y);
                    Start = new DXFVertex(p);
                    End = Start;
                    break;
            }
        }
        public void Flip()
        {
            DXFVertex temp = Start;
            Start = End;
            End = temp;
        }
    }
}
