using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DXFCircle : DXFEntity
    {
        public Point CenterP = new Point();
        public double radius;
        public DXFCircle(string name, DXFReader reader, DXFModel parent) : base(reader, parent)
        {
            Name = name;
            Type = EntityType.Circle;
            ReadProperties();
            parent.Graph.AddEdge(new DXFEdge(this));
        }
        protected DXFCircle(DXFReader reader, DXFModel parent) : base(reader, parent) { }
        
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
                }
            }
        }
        public override void Render(DrawingContext context)
        {
            context.DrawEllipse(Brushes.Transparent, DXFImage.BlackPen, DXFImage.GetMatPoint(CenterP), radius, radius);
        }
    }
}
