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
        public Point StartP;
        public Point EndP;
        private bool isReal;
        public bool IsReal { get { return isReal; } }
        public DXFLine(string name, DXFModel parent) : base(parent)
        {
            Name = name;
            Type = EntityType.Line;
            isReal = true;
            ReadProperties();
            parent.Graph.AddEdge(new DXFEdge(this));
        }
        //虚线的构造函数
        public DXFLine(DXFModel parent, Point startP, Point endP) : base(parent)
        {
            Name = "LINE";
            Type = EntityType.Line;
            isReal = false;
            StartP = startP;
            EndP = endP;
        }

        public override void ReadProperties()
        {
            while (true)
            {
                Parent.Reader.MoveNext();
                if (Parent.Reader.CurrentCode == 0) break;
                switch (Parent.Reader.CurrentCode)
                {
                    case 10:
                        StartP.X = Convert.ToDouble(Parent.Reader.CurrentValue);
                        break;
                    case 20:
                        StartP.Y = Convert.ToDouble(Parent.Reader.CurrentValue);
                        break;
                    case 11:
                        EndP.X = Convert.ToDouble(Parent.Reader.CurrentValue);
                        break;
                    case 21:
                        EndP.Y = Convert.ToDouble(Parent.Reader.CurrentValue);
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
