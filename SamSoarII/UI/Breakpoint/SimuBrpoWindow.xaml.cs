using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SamSoarII.Simulation.UI.Breakpoint
{
    /// <summary>
    /// SimuBrpoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimuBrpoWindow : UserControl
    {
        public SimuBrpoWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private ObservableCollection<SimuBrpoElement> items
            = new ObservableCollection<SimuBrpoElement> ();
        public IEnumerable<SimuBrpoElement> Items
        {
            get { return this.items; }
        }
        public IEnumerable<string> SelectedConditions
        {
            get { return new string[]{"无", "0", "1", "上升沿", "下降沿", "边沿" }; }
        }

        
    }
}
