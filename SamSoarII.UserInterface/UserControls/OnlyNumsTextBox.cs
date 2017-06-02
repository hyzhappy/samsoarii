using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SamSoarII.UserInterface
{
    public class OnlyNumsTextBox : TextBox
    {
        public static readonly DependencyProperty TopRangeProperty;
        public static readonly DependencyProperty LowRangeProperty;
        public static readonly DependencyProperty IsNumsOnlyProperty;
        private int oldvalue;
        static OnlyNumsTextBox()
        {
            IsNumsOnlyProperty = DependencyProperty.Register("IsNumsOnly",typeof(bool), typeof(OnlyNumsTextBox));
            TopRangeProperty = DependencyProperty.Register("TopRange", typeof(int), typeof(OnlyNumsTextBox));
            LowRangeProperty = DependencyProperty.Register("LowRange", typeof(int), typeof(OnlyNumsTextBox));
        }
        public OnlyNumsTextBox() : base()
        {
            GotFocus += OnlyNumsTextBox_GotFocus;
            LostFocus += OnlyNumsTextBox_LostFocus;
            TextChanged += OnlyNumsTextBox_TextChanged;
        }

        private void OnlyNumsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsNumsOnly)
            {
                try
                {
                    int value = int.Parse(Text);
                    if (!AssertRange(value))
                        Background = Brushes.OrangeRed;
                    else
                        Background = Brushes.Transparent;
                }
                catch (Exception)
                {
                    Background = Brushes.OrangeRed;
                }
            }
        }

        private void OnlyNumsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Text == string.Empty && IsNumsOnly)
            {
                Text = oldvalue.ToString();
            }
        }
        private void OnlyNumsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            oldvalue = int.Parse((sender as OnlyNumsTextBox).Text);
        }
        private bool AssertRange(int value)
        {
            return value >= LowRange && value <= TopRange;
        }

        public bool IsNumsOnly
        {
            get
            {
                return (bool)GetValue(IsNumsOnlyProperty);
            }
            set
            {
                SetValue(IsNumsOnlyProperty,value);
            }
        }
        public int TopRange
        {
            get
            {
                return (int)GetValue(TopRangeProperty);
            }
            set
            {
                SetValue(TopRangeProperty, value);
            }
        }
        public int LowRange
        {
            get
            {
                return (int)GetValue(LowRangeProperty);
            }
            set
            {
                SetValue(LowRangeProperty, value);
            }
        }
    }
}
