using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DXFArc : DXFCircle
    {
        //这里代表角度
        public double SAngle,EAngle;
        public Point StartP, EndP;
        public DXFArc(string name, DXFReader reader, DXFModel parent) : base(reader, parent)
        {
            Name = name;
            Type = EntityType.Arc;
            ReadProperties();
            parent.Graph.AddEdge(new DXFEdge(this));
        }
        public override void ReadProperties()
        {
            while (true)
            {
                Reader.MoveNext();
                if (Reader.CurrentCode == 0) break;
                switch (Reader.CurrentCode)
                {
                    case 10:
                        CenterP.X = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 20:
                        CenterP.Y = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 40:
                        radius = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 50:
                        SAngle = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 51:
                        EAngle = Convert.ToDouble(Reader.CurrentValue);
                        break;
                }
            }
            if (EAngle == 0) EAngle = 360;
            StartP = DXFHelper.ComputePoint(CenterP, radius, SAngle);
            EndP = DXFHelper.ComputePoint(CenterP, radius, EAngle);
        }
        public override void Render(DrawingContext context)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = DXFImage.GetMatPoint(StartP);
            ArcSegment segement = new ArcSegment(DXFImage.GetMatPoint(EndP), new Size(radius, radius), 0, EAngle - SAngle > 180, SweepDirection.Counterclockwise, true);
            figure.Segments.Add(segement);
            geometry.Figures.Add(figure);
            context.DrawGeometry(Brushes.Transparent, DXFImage.BlackPen, geometry);
        }
    }
}
