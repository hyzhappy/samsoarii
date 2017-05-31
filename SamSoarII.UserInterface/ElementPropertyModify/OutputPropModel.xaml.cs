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

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// OutputPropModel.xaml 的交互逻辑
    /// </summary>
    public partial class OutputPropModel : BasePropModel
    {
        public OutputPropModel()
        {
            InitializeComponent();
        }

        private string instname;
        public override string InstructionName
        {
            get
            {
                return this.instname;
            }
            set
            {
                this.instname = value;
                switch (instname)
                {
                    case "OUT": CenterTextBlock.Text = String.Empty; break;
                    case "OUTIM": CenterTextBlock.Text = "I"; break;
                    case "RST": CenterTextBlock.Text = "R"; break;
                    case "RSTIM": CenterTextBlock.Text = "RI"; break;
                    case "SET": CenterTextBlock.Text = "S"; break;
                    case "SETIM": CenterTextBlock.Text = "SI"; break;
                }
            }
        }

        private int count;
        public override int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
                ValueTextBox.Visibility = count >= 1
                    ? Visibility.Visible
                    : Visibility.Hidden;
                CountTextBox.Visibility = count >= 2
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }
        }
        
        public override string ValueString1
        {
            get
            {
                return ValueTextBox.Text;
            }
            set
            {
                ValueTextBox.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString2
        {
            get
            {
                return CountTextBox.Text;
            }
            set
            {
                CountTextBox.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString3 { get; set; }
        public override string ValueString4 { get; set; }
        public override string ValueString5 { get; set; }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(ValueTextBox.Text);
        }

        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(CountTextBox.Text);
        }
    }
}
