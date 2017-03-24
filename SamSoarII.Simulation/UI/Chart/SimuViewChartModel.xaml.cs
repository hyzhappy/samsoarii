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
using SamSoarII.Simulation.Core.DataModel;

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// SimuViewChartModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewChartModel : UserControl
    {
        private LinkedList<SimulateDataViewModel> sdvmodels;
        private LinkedList<SimulateDataChartModel> sdcmodels;

        public SimuViewChartModel()
        {
            InitializeComponent();
            sdvmodels = new LinkedList<SimulateDataViewModel>();
            sdcmodels = new LinkedList<SimulateDataChartModel>();
            ReinitializeComponent();
        }

        public SimuViewChartModel(IEnumerable<SimulateDataModel> sdmodels)
        {
            InitializeComponent();
            sdvmodels = new LinkedList<SimulateDataViewModel>();
            sdcmodels = new LinkedList<SimulateDataChartModel>();
            foreach (SimulateDataModel sdmodel in sdmodels)
            {
                SimulateDataViewModel sdvmodel = new SimulateDataViewModel(sdmodel);
                SimulateDataChartModel sdcmodel = new SimulateDataChartModel(sdmodel);
                sdvmodels.AddLast(sdvmodel);
                sdcmodels.AddLast(sdcmodel);
            }
            ReinitializeComponent();
        }

        public void ReinitializeComponent()
        {
            VList.SDVModels = sdvmodels;
            VChart.SDCModels = sdcmodels;
        }
        
    }
}
