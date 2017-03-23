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
    /// MonitorExpandButton.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorExpandButton : UserControl
    {
        private SimulateUnitSeries ssunit;

        public const int STATUS_EXPANDOFF = 0x00;
        public const int STATUS_EXPANDON = 0x01;
        private int status;
        public bool IsExpanded
        {
            get
            {
                return (status == STATUS_EXPANDON);
            }
            set
            {
                if (value)
                {
                    status = STATUS_EXPANDON;
                }
                else
                {
                    status = STATUS_EXPANDOFF;
                }
                switch (status)
                {
                    case STATUS_EXPANDOFF:
                        Image_ExpandOff.Opacity = 0.3;
                        Image_ExpandOn.Opacity = 0.0;
                        ssunit.IsExpand = false;
                        break;
                    case STATUS_EXPANDON:
                        Image_ExpandOff.Opacity = 0.0;
                        Image_ExpandOn.Opacity = 0.8;
                        ssunit.IsExpand = true;
                        break;
                }
            }
        }


        public MonitorExpandButton(SimulateUnitSeries _ssunit)
        {
            InitializeComponent();
            ssunit = _ssunit;
            //IsExpanded = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            switch (status)
            {
                case STATUS_EXPANDOFF:
                    Image_ExpandOff.Opacity = 0.8;
                    break;
                case STATUS_EXPANDON:
                    Image_ExpandOn.Opacity = 0.3;
                    break;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            switch (status)
            {
                case STATUS_EXPANDOFF:
                    Image_ExpandOff.Opacity = 0.3;
                    break;
                case STATUS_EXPANDON:
                    Image_ExpandOn.Opacity = 0.8;
                    break;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            switch (status)
            {
                case STATUS_EXPANDOFF:
                    IsExpanded = true;
                    break;
                case STATUS_EXPANDON:
                    IsExpanded = false;
                    break;
            }
        }
    }
}
