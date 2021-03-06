﻿using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
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

namespace SamSoarII.AppMain.UI.OptionWidget.DemoViewModel
{
    /// <summary>
    /// SMOVDemoViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class SMOVDemoViewModel : UserControl, IPosition
    {
        public SMOVDemoViewModel()
        {
            InitializeComponent();
            Canvas.SetLeft(this, 300 * X);
            Canvas.SetTop(this, Y);
        }

        public int X
        {
            get
            {
                return 2;
            }
        }

        public int Y
        {
            get
            {
                return 0;
            }
        }
    }
}
