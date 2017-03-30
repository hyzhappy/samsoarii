using SamSoarII.Simulation.UI.Base;
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
    /// SimuViewAllDiaModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewAllDiaModel : SimuViewTabModel
    {
        private SimulateModel _parent;

        public SimuViewAllDiaModel(SimulateModel parent)
        {
            InitializeComponent();
            this._parent = parent;
            MainRoutineExpander.Content = parent.MainRoutine;
            foreach (SimuViewDiagramModel svdmodel in parent.SubRoutines)
            {
                Expander expander = new Expander();
                TextBlock tblock = new TextBlock();
                tblock.FontSize = 16;
                tblock.Name = svdmodel.Name;
                expander.Header = tblock;
                expander.Content = svdmodel;
            }
        }
    }
}
