using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SamSoarII.LadderInstViewModel
{
    public class ColorManager : INotifyPropertyChanged
    {
        private static ColorManager ColorDataProvider = new ColorManager();
        private ColorManager() { }
        public static ColorManager GetColorDataProvider()
        {
            return ColorDataProvider;
        }
        private Color _selectColor;
        public SolidColorBrush SelectColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush(_selectColor);
                return brush;
            }
            set
            {
                _selectColor = value.Color;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("SelectColor"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
