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

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages.PLC_Instruction
{
    /// <summary>
    /// Page5104.xaml 的交互逻辑
    /// </summary>
    public partial class Page5104 : Page,IPageItem
    {
        public Page5104()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 5104;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("-| /| |-(立即常闭)");
            }
        }
    }
}
