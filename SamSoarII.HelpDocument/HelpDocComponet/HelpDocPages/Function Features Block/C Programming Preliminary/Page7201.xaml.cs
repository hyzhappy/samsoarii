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

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Function_Features_Block.C_Programming_Preliminary
{
    /// <summary>
    /// Page7201.xaml 的交互逻辑
    /// </summary>
    public partial class Page7201 : Page, IPageItem
    {
        public Page7201()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 7201;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("C语言语句");
            }
        }
    }
}
