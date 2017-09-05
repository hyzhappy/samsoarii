using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DXFLine : DXFEntity
    {
        public Point StartP = new Point();
        public Point EndP = new Point();
        public DXFLine(string name,DXFReader reader,DXFModel parent) : base(reader, parent)
        {
            Name = name;
            Type = EntityType.Line;
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
                        StartP.X = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 20:
                        StartP.Y = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 11:
                        EndP.X = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 21:
                        EndP.Y = Convert.ToDouble(Reader.CurrentValue);
                        break;
                }
            }
        }

        public override void Render(DrawingContext context)
        {
            context.DrawLine(DXFImage.BlackPen, DXFImage.GetMatPoint(StartP), DXFImage.GetMatPoint(EndP));
        }
    }
}
