using SamSoarII.Core.Models;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// OutputPropModel.xaml 的交互逻辑
    /// </summary>
    public partial class OutputPropModel : BasePropModel
    {
        public OutputPropModel(LadderUnitModel _core) : base(_core)
        {
            InitializeComponent();
            ReinitializeComponent();
        }
        
        private void ReinitializeComponent()
        {
            switch (InstructionName)
            {
                case "OUT": CenterTextBlock.Text = String.Empty; break;
                case "OUTIM": CenterTextBlock.Text = "I"; break;
                case "RST": CenterTextBlock.Text = "R"; break;
                case "RSTIM": CenterTextBlock.Text = "RI"; break;
                case "SET": CenterTextBlock.Text = "S"; break;
                case "SETIM": CenterTextBlock.Text = "SI"; break;
            }
            ValueTextBox.Visibility = Count >= 1
                ? Visibility.Visible
                : Visibility.Hidden;
            CountTextBox.Visibility = Count >= 2
                ? Visibility.Visible
                : Visibility.Hidden;
            if (Count >= 1) ValueTextBox.Text = Core.Children[0].Text;
            if (Count >= 2) CountTextBox.Text = Core.Children[1].Text;

        }

        public override int SelectedIndex
        {
            get
            {
                return base.SelectedIndex;
            }
            set
            {
                if (value >= 0 && value < Count && value != SelectedIndex)
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
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(0, ValueTextBox.Text);
        }
        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(1, CountTextBox.Text);
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
