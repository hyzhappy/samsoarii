﻿using System;
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

namespace SamSoarII.LadderInstViewModel.Monitor
{
    /// <summary>
    /// MoniVLineViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MoniVLineViewModel : MoniBaseViewModel
    {
        public MoniVLineViewModel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override int X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
                Canvas.SetLeft(this, base.X * 300 + 280);
            }
        }

        public override int Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
                Canvas.SetTop(this, base.Y * 300 + 100);
            }
        }

        public override void Update()
        {
        }
    }
}
