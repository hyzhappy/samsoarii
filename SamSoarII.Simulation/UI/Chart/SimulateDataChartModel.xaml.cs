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

using SamSoarII.Simulation.Core.DataModel;

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// SimulateDataChartModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimulateDataChartModel : UserControl
    {
        private SimulateDataModel sdmodel;

        public SimulateDataChartModel()
        {
            InitializeComponent();
        }

        public SimulateDataChartModel(SimulateDataModel _sdmodel)
        {
            InitializeComponent();
            this.sdmodel = _sdmodel;
        }
    }
}
