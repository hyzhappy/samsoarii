﻿using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages;
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

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages.Function_Features_Block.Getting_Start_To_C
{
    /// <summary>
    /// Page7103.xaml 的交互逻辑
    /// </summary>
    public partial class Page7103 : Page,IPageItem
    {
        public Page7103()
        {
            InitializeComponent();
        }

        public int PageIndex
        {
            get
            {
                return 7103;
            }
        }

        public string TabHeader
        {
            get
            {
                return string.Format("基本运算符和表达式");
            }
        }
    }
}
