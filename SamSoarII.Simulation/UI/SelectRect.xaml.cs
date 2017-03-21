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

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// SelectRect.xaml 的交互逻辑
    /// </summary>
    public partial class SelectRect : UserControl
    {
        int _x;
        
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                Canvas.SetLeft(this, _x * 300);
            }
        }
        int _y;

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                Canvas.SetTop(this, _y * 300);
            }
        }
        public SelectRect()
        {
            InitializeComponent();
        }
    }
}
