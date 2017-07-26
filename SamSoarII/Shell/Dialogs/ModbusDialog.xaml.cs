using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ModbusDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusDialog : Window
    {
        public ModbusDialog()
        {
            InitializeComponent();
            KeyDown += ModbusDialog_KeyDown;
        }

        private void ModbusDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                B_Ensure.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    B_Cancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
    }
}
