using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ColorSelectionWidget.xaml 的交互逻辑
    /// </summary>
    public partial class ColorSelectionWidget : UserControl,INotifyPropertyChanged
    {
        private SolidColorBrush _oldColor = Brushes.Transparent;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public SolidColorBrush OldColor
        {
            get
            {
                return _oldColor;
            }
            set
            {
                _oldColor = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("OldColor"));
            }
        }
        public ColorSelectionWidget()
        {
            InitializeComponent();
            DataContext = this;
            OptionDialog.EnsureButtonClick += OptionDialog_EnsureButtonClick; ;
            OldColor = new SolidColorBrush(colorPicker.SelectedColor);
        }
        private void OptionDialog_EnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Color selectColor = colorPicker.SelectedColor;
            Color color = ColorManager.GetColorDataProvider().SelectColor.Color;
            if (selectColor.A != color.A || selectColor.R != color.R || selectColor.G != color.G || selectColor.B != color.B)
            {
                GlobalSetting.SelectColor = selectColor;
                ColorManager.GetColorDataProvider().SelectColor = new SolidColorBrush(selectColor);
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && OldColor.Color != colorPicker.SelectedColor)
            {
                OldColor = new SolidColorBrush(colorPicker.SelectedColor);
            }
        }
    }
}
