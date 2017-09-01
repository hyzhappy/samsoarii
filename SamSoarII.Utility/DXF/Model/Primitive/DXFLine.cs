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
        private Point PF = new Point();
        private Point PS = new Point();
        public DXFLine(string name,DXFReader reader) : base(reader)
        {
            Name = name;
            Type = EntityType.Line;
            ReadProperties();
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
                        PF.X = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 20:
                        PF.Y = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 11:
                        PS.X = Convert.ToDouble(Reader.CurrentValue);
                        break;
                    case 21:
                        PS.Y = Convert.ToDouble(Reader.CurrentValue);
                        break;
                }
            }
        }

        public override void Render(DrawingContext context)
        {
            context.DrawLine(DXFImage.BlackPen, DXFImage.GetMatPoint(PF), DXFImage.GetMatPoint(PS));
        }
    }
}
