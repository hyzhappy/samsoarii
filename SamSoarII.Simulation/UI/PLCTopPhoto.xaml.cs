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

using SamSoarII.Simulation.UI.PLCTop;

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// PLCTopPhoto.xaml 的交互逻辑
    /// </summary>
    public partial class PLCTopPhoto : UserControl
    {
        public PLCTopPhoto()
        {
            InitializeComponent();

            ErrorLight.Text = "Error";
            ErrorLight.Status = StatusLight.STATUS_DARK;
            StopLight.Text = "Stop";
            StopLight.Status = StatusLight.STATUS_LIGHT;
            RunLight.Text = "Running";
            RunLight.Status = StatusLight.STATUS_DARK;

            LEDLight_Y0.Text = "Y0";
            LEDLight_Y0.Status = LEDLight.STATUS_DARK;
            LEDLight_Y1.Text = "Y1";
            LEDLight_Y1.Status = LEDLight.STATUS_DARK;
            LEDLight_Y2.Text = "Y2";
            LEDLight_Y2.Status = LEDLight.STATUS_DARK;
            LEDLight_Y3.Text = "Y3";
            LEDLight_Y3.Status = LEDLight.STATUS_DARK;
            LEDLight_Y4.Text = "Y4";
            LEDLight_Y4.Status = LEDLight.STATUS_DARK;
            LEDLight_Y5.Text = "Y5";
            LEDLight_Y5.Status = LEDLight.STATUS_DARK;
            LEDLight_Y6.Text = "Y6";
            LEDLight_Y6.Status = LEDLight.STATUS_DARK;
            LEDLight_Y7.Text = "Y7";
            LEDLight_Y7.Status = LEDLight.STATUS_DARK;

            RunTrigger.Status = PLCTop.Trigger.STATUS_STOP;
            
        }
    }
}
