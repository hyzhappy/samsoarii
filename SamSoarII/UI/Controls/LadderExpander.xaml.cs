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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// LadderExpander.xaml 的交互逻辑
    /// </summary>
    public partial class LadderExpander : UserControl
    {
        public static readonly DependencyProperty IsExpandProperty;
        static LadderExpander()
        {
            IsExpandProperty = DependencyProperty.Register("IsExpand", typeof(bool), typeof(LadderExpander));
        }
        public bool IsExpand
        {
            get
            {
                return (bool)GetValue(IsExpandProperty);
            }
            set
            {
                SetValue(IsExpandProperty, value);
            }
        }
        public LadderExpander()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
