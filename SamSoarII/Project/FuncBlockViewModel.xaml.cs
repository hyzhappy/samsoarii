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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// FuncBlockViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class FuncBlockViewModel : UserControl
    {
        public string FuncBlockName { get; set; }

        public string Code
        {
            get
            {
                return CodeTextBox.Text;
            }
            set
            {
                CodeTextBox.Text = value;
            }

        }

        public FuncBlockViewModel(string name)
        {
            InitializeComponent();
            FuncBlockName = name;
            CodeTextBox.DataContext = this;
        }
    }
}
