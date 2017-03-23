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

using SamSoarII.Simulation.Core.VariableModel;

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorBitButton.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorBitButton : Button
    {
        public MonitorBitButton()
        {
            InitializeComponent();
            //IsReadOnly = false;
        }

        private SimulateVariableUnit svunit;

        public SimulateVariableUnit SVUnit
        {
            set
            {
                svunit = value;
                if (svunit is SimulateBitUnit)
                {
                    this.Opacity = 1.0;
                }
                else
                {
                    this.Opacity = 0.0;
                }
                SetText();
            }
        }

        public const int STATUS_ON = 0x01;
        public const int STATUS_OFF = 0x00;
        public const int STATUS_ERROR = 0x02;
        private int status;
        public int Status
        {
            get
            {
                return this.status;
            }
            private set
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.status = value;
                    switch (this.status)
                    {
                        case STATUS_ON:
                            Title.Text = "ON";
                            Background = Brushes.Transparent;
                            break;
                        case STATUS_OFF:
                            Title.Text = "OFF";
                            Background = Brushes.Transparent;
                            break;
                        case STATUS_ERROR:
                            Title.Text = "ERROR";
                            Background = Brushes.Red;
                            break;
                    }
                });
            }
        }

        public void SetText()
        {
            if (svunit is SimulateBitUnit)
            {
                switch ((int)(svunit.Value))
                {
                    case 0:
                        Status = STATUS_OFF;
                        break;
                    case 1:
                        Status = STATUS_ON;
                        break;
                    default:
                        Status = STATUS_ERROR;
                        break;
                }
            }
        }
        
        public bool IsReadOnly;
        public event RoutedEventHandler TextLegalChanged;
        protected override void OnClick()
        {
            if (!IsReadOnly)
            {
                base.OnClick();
                switch (Status)
                {
                    case STATUS_ON:
                        Status = STATUS_OFF;
                        break;
                    case STATUS_OFF:
                        Status = STATUS_ON;
                        break;
                }
                if (TextLegalChanged != null)
                {
                    TextLegalChanged(this, new RoutedEventArgs());
                }
            }
        }
        
    }
}
