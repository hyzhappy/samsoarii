using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.LadderInstViewModel
{
    public class FontManager : INotifyPropertyChanged
    {
        private static FontManager FontDataProvider = new FontManager();
        private FontFamily _selectFontFamily;
        private int _selectFontSize;
        private int _selectFontSizeIndex;
        private int _selectFontFamilyIndex;
        private FontManager(){ }
        public void Initialize()
        {
            _selectFontSize = _selectFontSizeIndex + 20;
            _selectFontFamily = new FontFamily((new InstalledFontCollection()).Families[_selectFontFamilyIndex].Name);
        }
        public static FontManager GetFontDataProvider()
        {
            return FontDataProvider;
        }
        public int SelectFontSizeIndex
        {
            get
            {
                return _selectFontSizeIndex;
            }
            set
            {
                _selectFontSizeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectFontSizeIndex"));
            }
        }
        public int SelectFontFamilyIndex
        {
            get
            {
                return _selectFontFamilyIndex;
            }
            set
            {
                _selectFontFamilyIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectFontFamilyIndex"));
            }
        }
        public FontFamily SelectFontFamily
        {
            get
            {
                return _selectFontFamily;
            }
            set
            {
                _selectFontFamily = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("SelectFontFamily"));
            }
        }
        public int SelectFontSize
        {
            get
            {
                return _selectFontSize;
            }
            set
            {
                _selectFontSize = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectFontSize"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
