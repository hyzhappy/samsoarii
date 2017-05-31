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

        public override int SelectedIndex
        {
            get
            {
                return base.SelectedIndex;
            }
            set
            {
                base.SelectedIndex = value;
                switch (SelectedIndex)
                {
                    case 0:
                        ValueTextBox.Focus();
                        Keyboard.Focus(ValueTextBox);
                        break;
                    case 1:
                        CountTextBox.Focus();
                        Keyboard.Focus(CountTextBox);
                        break;
                }
            }
        }

        public override string ValueString1
        {
            get
            {
                if (ValueTextBox == null)
                    return String.Empty;
                return ValueTextBox.Text;
            }
            set
            {
                if (ValueTextBox == null)
                    return;
                ValueTextBox.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString2
        {
            get
            {
                if (CountTextBox == null)
                    return String.Empty;
                return CountTextBox.Text;
            }
            set
            {
                if (CountTextBox == null)
                    return;
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
        private void ValueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 0;
        }
        private void CountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 1;
        }
    }
}
