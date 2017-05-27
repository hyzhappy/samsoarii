using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace SamSoarII.LadderInstViewModel
{
    public class ColorManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
  
        static public Brush Parse(string text)
        {
            string[] args = text.Split(' ');
            Color color = new Color();
            color.A = byte.Parse(args[0]);
            color.R = byte.Parse(args[1]);
            color.G = byte.Parse(args[2]);
            color.B = byte.Parse(args[3]);
            return new SolidColorBrush(color);
        }

        static public string ToString(Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                SolidColorBrush scbrush = (SolidColorBrush)brush;
                Color color = scbrush.Color;
                return String.Format("{0:d} {1:d} {2:d} {3:d}",
                    color.A, color.R, color.G, color.B);
            }
            return String.Empty;
        }

    }
}
