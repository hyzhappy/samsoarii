using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetterTab
{
    public class BetterTabItem : TabItem
    {
        #region .ctors

        static BetterTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BetterTabItem), new FrameworkPropertyMetadata(typeof(BetterTabItem)));
        }

        #endregion .ctors
    }
}
