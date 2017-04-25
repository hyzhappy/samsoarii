﻿using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
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
    public partial class ColorSelectionWidget : UserControl
    {
        public ColorSelectionWidget()
        {
            InitializeComponent();
            OptionDialog.EnsureButtonClick += OptionDialog_EnsureButtonClick; ;
        }
        private void OptionDialog_EnsureButtonClick(object sender, RoutedEventArgs e)
        {
            var cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Color selectColor = colorPicker.SelectedColor;
            Color color = ColorManager.GetColorDataProvider().SelectColor.Color;
            if (selectColor.A != color.A || selectColor.R != color.R || selectColor.G != color.G || selectColor.B != color.B)
            {
                GlobalSetting.SelectColor = selectColor;
                ColorManager.GetColorDataProvider().SelectColor = new SolidColorBrush(selectColor);
            }
            if (sender is Window)
            {
                cfa.Save();
                Window window = (Window)sender;
                window.Close();
            }
        }
        private void OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            colorShowTextBox.Background = new SolidColorBrush(colorPicker.SelectedColor);
        }
    }
}