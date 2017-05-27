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
    public partial class ColorSettingDialog : Window
    {
        public const int TYPE_FONT = 0x01;
        public const int TYPE_BACKGROUND = 0x02;
        public const int TYPE_FOREGROUND = 0x03;
        

        public object Relative { get; private set; }

        public int Type { get; private set; }
        
        public Color Color
        {
            get
            {
                FontData fdat = null;
                switch (Type)
                {
                    case TYPE_FONT:
                        fdat = (FontData)Relative;
                        return ((SolidColorBrush)fdat.FontColor).Color;
                    default:
                        return Colors.Black;
                }
            }
        }
        
        public ColorSettingDialog(object _relative, int _type)
        {
            InitializeComponent();
            DataContext = this;
            Relative = _relative;
            Type = _type;
            CP_Color.SelectedColor = Color;
            label.Background = new SolidColorBrush(CP_Color.SelectedColor);
        }

        private void B_Ensure_Click(object sender, RoutedEventArgs e)
        {
            FontData fdat = null;
            switch (Type)
            {
                case TYPE_FONT:
                    fdat = (FontData)Relative;
                    fdat.FontColor = new SolidColorBrush(CP_Color.SelectedColor);
                    break;
            }
            Close();
        }

        private void B_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                label.Background = new SolidColorBrush(CP_Color.SelectedColor);
            }
        }
    }
    
}
