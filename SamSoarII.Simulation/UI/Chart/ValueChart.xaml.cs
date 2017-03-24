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

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// ValueChart.xaml 的交互逻辑
    /// </summary>
    public partial class ValueChart : UserControl
    {
        private IEnumerable<SimulateDataChartModel> sdcmodels;
        public IEnumerable<SimulateDataChartModel> SDCModels
        {
            get
            {
                return this.sdcmodels;
            }
            set
            {
                this.sdcmodels = value;
                Update();
            }
        }

        public ValueChart()
        {
            InitializeComponent();
        }

        public void Update()
        {
            
        }
    }
}
