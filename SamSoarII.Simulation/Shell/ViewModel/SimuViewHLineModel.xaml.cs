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

namespace SamSoarII.Simulation.Shell.ViewModel
{
    /// <summary>
    /// SimuViewHLineModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewHLineModel : SimuViewBaseModel
    {
        public SimuViewHLineModel(SimulateModel parent) : base(parent)
        {
            InitializeComponent();
        }

        public override void Setup(string text)
        {
        }

        public override void Update()
        {
        }
    }
}
