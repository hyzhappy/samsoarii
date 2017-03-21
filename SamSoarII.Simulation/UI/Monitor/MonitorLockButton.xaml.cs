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

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorLockButton.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorLockButton : UserControl
    {
        public const int STATUS_LOCKOFF = 0x00;
        public const int STATUS_LOCKON = 0x01;
        
        private int status;
        public bool IsLocked
        {
            get
            {
                return (status == 1);
            }
            set
            {
                if (value)
                {
                    status = 1;
                }
                else
                {
                    status = 0;
                }
                switch (status)
                {
                    case STATUS_LOCKOFF:
                        Image_LockOff.Opacity = 0.3;
                        Image_LockOn.Opacity = 0.0;
                        break;
                    case STATUS_LOCKON:
                        Image_LockOff.Opacity = 0.0;
                        Image_LockOn.Opacity = 1.0;
                        break;
                }
            }
        }

        public MonitorLockButton()
        {
            InitializeComponent();
            IsLocked = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            switch (status)
            {
                case STATUS_LOCKOFF:
                    Image_LockOff.Opacity = 0.8;
                    break;
                case STATUS_LOCKON:
                    Image_LockOn.Opacity = 0.3;
                    break;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            switch (status)
            {
                case STATUS_LOCKOFF:
                    Image_LockOff.Opacity = 0.3;
                    break;
                case STATUS_LOCKON:
                    Image_LockOn.Opacity = 0.8;
                    break;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            switch (status)
            {
                case STATUS_LOCKOFF:
                    IsLocked = true;
                    break;
                case STATUS_LOCKON:
                    IsLocked = false;
                    break;
            }
        }
        
    }
}
