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
    /// MonitorCloseButton.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorCloseButton : UserControl
    {
        public MonitorCloseButton()
        {
            InitializeComponent();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ButtonCanvas.Opacity = 0.8;
            Line1.StrokeThickness = 2;
            Line2.StrokeThickness = 2;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            ButtonCanvas.Opacity = 0.3;
            Line1.StrokeThickness = 1;
            Line2.StrokeThickness = 1;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            ButtonCanvas.Opacity = 0.3;
            Line1.StrokeThickness = 2;
            Line2.StrokeThickness = 2;
        }
    }
}
