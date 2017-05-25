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

namespace SamSoarII.AppMain.UI.OptionWidget.DemoViewModel
{
    /// <summary>
    /// LDDemoViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LDDemoViewModel : UserControl
    {
        public LDDemoViewModel()
        {
            InitializeComponent();
            Canvas.SetLeft(this,0);
            Canvas.SetTop(this,0);
        }
    }
}
