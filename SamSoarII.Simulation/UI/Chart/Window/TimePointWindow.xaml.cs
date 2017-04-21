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

namespace SamSoarII.Simulation.UI.Chart.Window
{
    /// <summary>
    /// TimePointWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TimePointWindow : System.Windows.Window
    {
        public TimePointWindow()
        {
            InitializeComponent();
            CB_TimeUnit.Text = "ms";
            TB_Time.TextChanged += TB_Time_TextChanged;
        }
        
        public double Value
        {
            get
            {
                switch (CB_TimeUnit.Text)
                {
                    case "ms":
                        return double.Parse(TB_Time.Text);
                    case "s":
                        return double.Parse(TB_Time.Text)*1000;
                    default:
                        throw new FormatException();
                }
            }
            set
            {
                switch (CB_TimeUnit.Text)
                {
                    case "ms":
                        TB_Time.Text = String.Format("{0}", value);
                        break;
                    case "s":
                        TB_Time.Text = String.Format("{0}", value / 1000);
                        break;
                    default:
                        throw new FormatException();
                }
            }
        }

        public event RoutedEventHandler ValueChanged;
        private void TB_Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new RoutedEventArgs());
            }
        }
    }
}
