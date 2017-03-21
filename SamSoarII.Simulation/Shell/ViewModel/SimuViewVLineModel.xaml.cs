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

namespace SamSoarII.Simulation.Shell.ViewModel
{
    /// <summary>
    /// SimuViewVLineModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewVLineModel : SimuViewBaseModel
    {
        public SimuViewVLineModel(SimulateModel parent) : base(parent)
        {
            InitializeComponent();
        }

        public override int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                Canvas.SetLeft(this, _x * 300 + 280);
            }
        }

        public override int Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
                Canvas.SetTop(this, _y * 300 + 100);
            }
        }

        public override void Setup(string text)
        {
        }

        public override void Update()
        {
        }
    }
}
