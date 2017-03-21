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
    /// Trigger.xaml 的交互逻辑
    /// </summary>
    public partial class Trigger : UserControl
    {
        public const int STATUS_STOP = 0x00;
        public const int STATUS_RUN = 0x01;

        public event RoutedEventHandler Run;
        public event RoutedEventHandler Stop;

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
                switch (status)
                {
                    case STATUS_STOP:
                        StopRect.Opacity = 1.0;
                        RunRect.Opacity = 0.0;
                        if (Stop != null)
                        {
                            Stop.Invoke(this, new RoutedEventArgs());
                        }
                        break;
                    case STATUS_RUN:
                        StopRect.Opacity = 0.0;
                        RunRect.Opacity = 1.0;
                        if (Run != null)
                        {
                            Run.Invoke(this, new RoutedEventArgs());
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public Trigger()
        {
            InitializeComponent();
            StopRect.MouseDown += MouseDown_StopRect;
            RunRect.MouseDown += MouseDown_RunRect;
        }

        private void MouseDown_RunRect(object sender, MouseButtonEventArgs e)
        {
            Status = STATUS_STOP;
        }

        private void MouseDown_StopRect(object sender, MouseButtonEventArgs e)
        {
            Status = STATUS_RUN;
        }
    }
}
