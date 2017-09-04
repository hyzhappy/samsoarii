using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DXFImage : DrawingVisual
    {
        public static Pen BlackPen = new Pen(Brushes.Black, 1);
        private DXFModel model;
        public static Point BaseP;
        public double ScaleX;
        public double ScaleY;

        public DXFImage(DXFModel model)
        {
            this.model = model;
            ScaleX = 1;
            ScaleY = 1;
        }

        public void DrawImage()
        {
            using(DrawingContext context = RenderOpen())
            {
                foreach (var section in model.Sections)
                {
                    foreach (var entity in section.Entities)
                        entity.Render(context);
                }
            }
        }

        public static Point GetMatPoint(Point p)
        {
            return new Point(p.X, BaseP.Y - p.Y);
        }
    }
}
