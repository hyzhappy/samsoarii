using SamSoarII.Core.Models;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// InputPropModel.xaml 的交互逻辑
    /// </summary>
    public partial class InputPropModel : BasePropModel
    {
        public InputPropModel(LadderUnitModel _core) : base(_core)
        {
            InitializeComponent();
            ReinitializeComponent();
        }

        private void ReinitializeComponent()
        {
            // 开始绘图
            CenterCanvas.Children.Clear();
            Line line = null;
            switch (InstructionName)
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
                default:
                    CenterTextBlock.Text = "";
                    break;
            }
            // 画个-[/]-表示【取反】
            switch (InstructionName)
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
            switch (InstructionName)
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
            switch (InstructionName)
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
            switch (InstructionName)
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
            ValueTextBox.Visibility = Count >= 1
                ? Visibility.Visible
                : Visibility.Hidden;
            Value2TextBox.Visibility = Count >= 2
                ? Visibility.Visible
                : Visibility.Hidden;
            if (Count >= 1) ValueTextBox.Text = GetValueString(0);
            if (Count >= 2) Value2TextBox.Text = GetValueString(1);
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
                            if (!ValueTextBox.IsFocused)
                            {
                                ValueTextBox.Focus();
                                Keyboard.Focus(ValueTextBox);
                            }
                            ValueTextBox.SelectAll();
                            break;
                        case 1:
                            if (!Value2TextBox.IsFocused)
                            {
                                Value2TextBox.Focus();
                                Keyboard.Focus(Value2TextBox);
                            }
                            Value2TextBox.SelectAll();
                            break;
                    }
                }
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(0, ValueTextBox.Text);
        }
        private void Value2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(1, Value2TextBox.Text);
        }
        private void ValueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 0;
        }
        private void Value2TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 1;
        }
    }
}
