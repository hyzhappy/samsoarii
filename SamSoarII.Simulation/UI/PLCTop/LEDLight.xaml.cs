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

namespace SamSoarII.Simulation.UI.PLCTop
{
    /// <summary>
    /// LEDLight.xaml 的交互逻辑
    /// </summary>
    public partial class LEDLight : UserControl
    {
        public const int STATUS_LIGHT = 0x01;
        public const int STATUS_DARK = 0x00;
        public const int STATUS_ERROR = 0x02;

        private int status;

        public int Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                this.Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
                {
                    switch (status)
                    {
                        case STATUS_LIGHT:
                            GreenLight.Opacity = 1.0;
                            DarkLight.Opacity = 0.0;
                            RedLight.Opacity = 0.0;
                            break;
                        case STATUS_DARK:
                            GreenLight.Opacity = 0.0;
                            DarkLight.Opacity = 1.0;
                            RedLight.Opacity = 0.0;
                            break;
                        case STATUS_ERROR:
                            GreenLight.Opacity = 0.0;
                            DarkLight.Opacity = 0.0;
                            RedLight.Opacity = 1.0;
                            break;
                        default:
                            break;
                    }
                }));
            }
        }

        public string Text
        {
            get
            {
                return PinName.Text;
            }
            set
            {
                PinName.Text = value;
            }
        }

        public LEDLight()
        {
            InitializeComponent();
            
        }
    }
}
