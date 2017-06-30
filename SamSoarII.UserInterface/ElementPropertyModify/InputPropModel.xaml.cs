using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// InputPropModel.xaml 的交互逻辑
    /// </summary>
    public partial class InputPropModel : BasePropModel
    {
        public InputPropModel()
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
                // 开始绘图
                CenterCanvas.Children.Clear();
                Line line = null;
                switch (instname)
                {
                    case "LDWEQ":
                        CenterTextBlock.Text = "W==";
                        break;
                    case "LDWNE":
                        CenterTextBlock.Text = "W<>";
                        break;
                    case "LDWGE":
                        CenterTextBlock.Text = "W>=";
                        break;
                    case "LDWLE":
                        CenterTextBlock.Text = "W<=";
                        break;
                    case "LDWG":
                        CenterTextBlock.Text = "W>";
                        break;
                    case "LDWL":
                        CenterTextBlock.Text = "W<";
                        break;
                    case "LDDEQ":
                        CenterTextBlock.Text = "D==";
                        break;
                    case "LDDNE":
                        CenterTextBlock.Text = "D<>";
                        break;
                    case "LDDGE":
                        CenterTextBlock.Text = "D>=";
                        break;
                    case "LDDLE":
                        CenterTextBlock.Text = "D<=";
                        break;
                    case "LDDG":
                        CenterTextBlock.Text = "D>";
                        break;
                    case "LDDL":
                        CenterTextBlock.Text = "D<";
                        break;
                    case "LDFEQ":
                        CenterTextBlock.Text = "F==";
                        break;
                    case "LDFNE":
                        CenterTextBlock.Text = "F<>";
                        break;
                    case "LDFGE":
                        CenterTextBlock.Text = "F>=";
                        break;
                    case "LDFLE":
                        CenterTextBlock.Text = "F<=";
                        break;
                    case "LDFG":
                        CenterTextBlock.Text = "F>";
                        break;
                    case "LDFL":
                        CenterTextBlock.Text = "F<";
                        break;
                }
                // 画个-[/]-表示【取反】
                switch (instname)
                {
                    case "LDI":
                    case "LDIIM":
                        line = new Line();
                        line.X1 = 75;
                        line.X2 = 25;
                        line.Y1 = 0;
                        line.Y2 = 100;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);
                        break;
                    default:
                        break;
                }
                // 画个-[|]-表示【立即】
                switch (instname)
                {
                    case "LDIM":
                    case "LDIIM":
                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 50;
                        line.Y1 = 0;
                        line.Y2 = 100;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);
                        break;
                }
                // 画个-[↑]-表示【上升沿】
                switch (instname)
                {
                    case "LDP":
                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 70;
                        line.Y1 = 0;
                        line.Y2 = 20;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);

                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 30;
                        line.Y1 = 0;
                        line.Y2 = 20;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);

                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 50;
                        line.Y1 = 0;
                        line.Y2 = 100;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);
                        break;
                    default:
                        break;
                }
                // 画个-[↓]-表示【下降沿】
                switch (instname)
                {
                    case "LDF":
                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 50;
                        line.Y1 = 0;
                        line.Y2 = 100;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);

                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 70;
                        line.Y1 = 100;
                        line.Y2 = 80;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);

                        line = new Line();
                        line.X1 = 50;
                        line.X2 = 30;
                        line.Y1 = 100;
                        line.Y2 = 80;
                        line.StrokeThickness = 4;
                        line.Stroke = Brushes.Black;
                        CenterCanvas.Children.Add(line);
                        break;
                    default:
                        break;
                }
                CenterCanvas.Children.Add(CenterTextBlock);
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
                Value2TextBox.Visibility = count >= 2
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
                        Value2TextBox.Focus();
                        Keyboard.Focus(Value2TextBox);
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
                if (Value2TextBox == null)
                    return String.Empty;
                return Value2TextBox.Text;
            }
            set
            {
                if (Value2TextBox == null)
                    return;
                Value2TextBox.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString3 { get; set; } = String.Empty;
        public override string ValueString4 { get; set; } = String.Empty;
        public override string ValueString5 { get; set; } = String.Empty;

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(ValueString1);
        }
        private void Value2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(ValueString2);
        }
        private void ValueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 0;
        }
        private void Value2TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 1;
        }
    }
}
