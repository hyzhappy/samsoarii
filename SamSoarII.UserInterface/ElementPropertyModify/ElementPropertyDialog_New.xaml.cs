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
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementPropertyDialog_New.xaml 的交互逻辑
    /// </summary>
    public partial class ElementPropertyDialog_New : Window
    {
        public ElementPropertyDialog_New()
        {
            InitializeComponent();
        }

        private BasePropModel bpmodel;
        public BasePropModel BPModel
        {
            get
            {
                return this.bpmodel;
            }
            set
            {
                if (bpmodel != null)
                {
                    GD_Main.Children.Remove(bpmodel);
                }
                this.bpmodel = value;
                if (bpmodel != null)
                {
                    Grid.SetRow(bpmodel, 0);
                    Grid.SetColumn(bpmodel, 0);
                    GD_Main.Children.Add(bpmodel);
                }
            }
        }
        
        public RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Ensure(this, new RoutedEventArgs());
            Close();
        }

        public RoutedEventHandler Cancel = delegate { };
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Cancel(this, new RoutedEventArgs());
            Close();
        }
    }
}
