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
        public DXFCircle(string name, DXFModel parent) : base(parent)
        {
            Name = name;
            Type = EntityType.Circle;
            ReadProperties();
            parent.Graph.AddEdge(new DXFEdge(this));
        }
        protected DXFCircle(DXFModel parent) : base(parent) { }
        
        public override void ReadProperties()
        {
            while (true)
            {
                Parent.Reader.MoveNext();
                if (Parent.Reader.CurrentCode == 0) break;
                switch (Parent.Reader.CurrentCode)
                {
                    case 10:
                        CenterP.X = Convert.ToDouble(Parent.Reader.CurrentValue);
                        break;
                    case 20:
                        CenterP.Y = Convert.ToDouble(Parent.Reader.CurrentValue);
                        break;
                    case 40:
                        radius = Convert.ToDouble(Parent.Reader.CurrentValue);
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
