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

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// MicroDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MicroDiagramViewModel : UserControl
    {
        public MicroDiagramViewModel()
        {
            InitializeComponent();
        }

        private MicroNetworkViewModel selecteditem;
        public MicroNetworkViewModel SelectedItem
        {
            get
            {
                return this.selecteditem;
            }
            set
            {
                if (selecteditem != null && selecteditem != value)
                {
                    selecteditem.SelectedItem = null;
                }
                this.selecteditem = value;
            }
        }

        private void MicroNetworkViewModel_ItemMouseMove(object sender, MouseEventArgs e)
        {
            SelectedItem = (MicroNetworkViewModel)sender;
        }

        public event RoutedEventHandler ItemSelected = delegate { };
        private void MicroNetworkViewModel_ItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            ItemSelected(this, new RoutedEventArgs());
        }
    }
}
