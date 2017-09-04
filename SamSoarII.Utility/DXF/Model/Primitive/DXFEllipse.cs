using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DXFEllipse : DXFEntity
    {
        public Point CenterP = new Point();
        //长轴端点
        public Point LongP = new Point();
        //这里代表弧度
        private double SRadian, ERadian;
        //短轴与长轴的比例
        private double ratio;

        public DXFEllipse(string name, DXFReader reader,DXFModel parent) : base(reader, parent)
        {
            Name = name;
            Type = EntityType.Ellipse;
            ReadProperties();
            parent.Graph.Add(new DXFEdge(this));
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
                    case 11:
                        LongP.X = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 21:
                        LongP.Y = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 40:
                        ratio = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 41:
                        SRadian = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 42:
                        ERadian = Convert.ToDouble(Reader.CurrentValue);
                        break;
                }
            }
        }
        public override void Render(DrawingContext context)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = DXFImage.GetMatPoint(LongP);
            double angleSpan = (ERadian - SRadian) * 180 / Math.PI;
            double longLenght = DXFHelper.ComputeLength(CenterP, LongP);
            ArcSegment segement = new ArcSegment(figure.StartPoint, new Size(longLenght, Math.Abs(longLenght * ratio)), DXFHelper.ComputeRotateAngle(CenterP, LongP), angleSpan > 180, SweepDirection.Counterclockwise, true);
            figure.Segments.Add(segement);
            geometry.Figures.Add(figure);
            context.DrawGeometry(Brushes.Transparent, DXFImage.BlackPen, geometry);
        }
    }
}
