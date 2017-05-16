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
    /// MicroNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MicroNetworkViewModel : UserControl
    {
        public MicroNetworkViewModel()
        {
            InitializeComponent();
        }

        public int RowCount
        {
            get { return G_Content.RowDefinitions.Count(); }
            set
            {
                G_Content.RowDefinitions.Clear();
                for (int i = 0; i < value; i++)
                {
                    RowDefinition rdef = new RowDefinition();
                    rdef.Height = new GridLength(64);
                    G_Content.RowDefinitions.Add(rdef);
                }
            }
        }

        private MicroViewModel selecteditem;
        public MicroViewModel SelectedItem
        {
            get { return this.selecteditem; }
            set
            {
                this.selecteditem = value;
                if (selecteditem != null)
                {
                    Grid.SetRow(B_Cursor, selecteditem.X);
                    Grid.SetColumn(B_Cursor, selecteditem.Y);
                }
            }
        }

        public event MouseButtonEventHandler ItemMouseDown = delegate { };
        private void MicroViewModel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ItemMouseDown(this, e);
        }

        public event MouseEventHandler ItemMouseMove = delegate { };
        private void MicroViewModel_MouseMove(object sender, MouseEventArgs e)
        {
            SelectedItem = (MicroViewModel)sender;
            ItemMouseMove(this, e);  
        }
    }
}
