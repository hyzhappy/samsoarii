using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ColorSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ColorSettingDialog : Window, INotifyPropertyChanged
    {
        public const int TYPE_FONT = 0x01;
        public const int TYPE_BACKGROUND = 0x02;
        public const int TYPE_FOREGROUND = 0x03;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public object Relative { get; private set; }

        public int Type { get; private set; }
        
        public Brush Color
        {
            get
            {
                FontData fdat = null;
                ColorData cdat = null;
                switch (Type)
                {
                    case TYPE_FONT:
                        fdat = (FontData)Relative;
                        return fdat.FontColor;
                    case TYPE_BACKGROUND:
                        cdat = (ColorData)Relative;
                        return cdat.Background;
                    case TYPE_FOREGROUND:
                        cdat = (ColorData)Relative;
                        return cdat.Foreground;
                    default:
                        return Brushes.Black;
                }
            }
        }
        
        public ColorSettingDialog(object _relative, int _type)
        {
            InitializeComponent();
            Relative = _relative;
            Type = _type;
            PropertyChanged(this, new PropertyChangedEventArgs("Color"));
        }

        private void B_Ensure_Click(object sender, RoutedEventArgs e)
        {
            FontData fdat = null;
            ColorData cdat = null;
            switch (Type)
            {
                case TYPE_FONT:
                    fdat = (FontData)Relative;
                    fdat.FontColor = new SolidColorBrush(CP_Color.SelectedColor);
                    break;
                case TYPE_BACKGROUND:
                    cdat = (ColorData)Relative;
                    cdat.Background = new SolidColorBrush(CP_Color.SelectedColor);
                    break;
                case TYPE_FOREGROUND:
                    cdat = (ColorData)Relative;
                    cdat.Foreground = new SolidColorBrush(CP_Color.SelectedColor);
                    break;
            }
            Close();
        }

        private void B_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    
}
