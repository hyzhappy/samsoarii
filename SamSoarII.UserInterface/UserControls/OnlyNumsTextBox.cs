using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SamSoarII.UserInterface
{
    public class OnlyNumsTextBox : TextBox
    {
        public static readonly DependencyProperty IsNumsOnlyProperty;
        private int oldvalue;
        static OnlyNumsTextBox()
        {
            IsNumsOnlyProperty = DependencyProperty.Register("IsNumsOnly",typeof(bool), typeof(OnlyNumsTextBox));
        }
        public OnlyNumsTextBox() : base()
        {
            PreviewKeyDown += OnlyNumsTextBox_PreviewKeyDown;
            GotFocus += OnlyNumsTextBox_GotFocus;
            LostFocus += OnlyNumsTextBox_LostFocus;
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

        private void OnlyNumsTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (IsNumsOnly)
            {
                int oldvalue = Text == string.Empty ? 0 : int.Parse(Text);
                if (oldvalue == 0 && KeyInputHelper.NumAssert(e.Key) && Text != string.Empty)
                {
                    e.Handled = true;
                }
                if (!KeyInputHelper.CanInputAssert(e.Key))
                {
                    e.Handled = true;
                }
            }
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
    }
}
