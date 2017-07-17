using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// RangeTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class RangeTextBox : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TopRangeProperty;
        public static readonly DependencyProperty LowRangeProperty;
        public static readonly DependencyProperty DefaultValueProperty;
        public event PropertyChangedEventHandler PropertyChanged;

        static RangeTextBox()
        {
            TopRangeProperty = DependencyProperty.Register("TopRange", typeof(int), typeof(RangeTextBox));
            LowRangeProperty = DependencyProperty.Register("LowRange", typeof(int), typeof(RangeTextBox));
            DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(int), typeof(RangeTextBox));
        }
        private int oldvalue;
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
        public int DefaultValue
        {
            get
            {
                return (int)GetValue(DefaultValueProperty);
            }
            set
            {
                SetValue(DefaultValueProperty, value);
            }
        }
        public bool CanUp
        {
            get
            {
                return textbox.Text == string.Empty || int.Parse(textbox.Text) < TopRange;
            }
        }
        public bool CanDown
        {
            get
            {
                return textbox.Text == string.Empty || int.Parse(textbox.Text) > LowRange;
            }
        }
        public RangeTextBox()
        {
            InitializeComponent();
            DataContext = this;
            PropertyChanged = delegate { };
            Loaded += RangeTextBox_Loaded;
        }

        private void RangeTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (DefaultValue != -1 && textbox.Text == string.Empty)
            {
                textbox.Text = DefaultValue.ToString();
            }
        }
        //private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    TextBox textbox = sender as TextBox;
        //    int oldvalue = textbox.Text == string.Empty ? 0 : int.Parse(textbox.Text);
        //    if (oldvalue == 0 && KeyInputHelper.NumAssert(e.Key) && textbox.Text != string.Empty)
        //    {
        //        e.Handled = true;
        //    }
        //    if (KeyInputHelper.CanInputAssert(e.Key))
        //    {
        //        int newvalue;
        //        if (KeyInputHelper.NumAssert(e.Key))
        //        {
        //            newvalue = 10 * oldvalue + KeyInputHelper.GetKeyValue(e.Key);
        //            if (!AssertRange(newvalue))
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}
        public TextBox GetTextBox()
        {
            return textbox;
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int value = int.Parse(textbox.Text);
                if (!AssertRange(value))
                {
                    textbox.Background = Brushes.OrangeRed;
                }
                else
                {
                    textbox.Background = Brushes.Transparent;
                }
            }
            catch (Exception)
            {
                textbox.Background = Brushes.OrangeRed;
            }
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CanUp"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CanDown"));
        }
        private void OnUpClick(object sender, RoutedEventArgs e)
        {
            int value;
            if (CanUp)
            {
                if (textbox.Text == string.Empty)
                {
                    textbox.Text = string.Format("{0}", DefaultValue);
                }
                else
                {
                    value = int.Parse(textbox.Text) + 1;
                    textbox.Text = value.ToString();
                }
            }
        }
        private void OnDownClick(object sender, RoutedEventArgs e)
        {
            int value;
            if (CanDown)
            {
                if (textbox.Text == string.Empty)
                {
                    textbox.Text = string.Format("{0}", DefaultValue);
                }
                else
                {
                    value = int.Parse(textbox.Text) - 1;
                    textbox.Text = value.ToString();
                }
            }
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            int value = textbox.Text == string.Empty ? 0 : int.Parse(textbox.Text);
            if (!AssertRange(value))
            {
                textbox.Text = oldvalue.ToString();
            }
        }
        private bool AssertRange(int value)
        {
            return value >= LowRange && value <= TopRange;
        }
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            oldvalue = int.Parse((sender as TextBox).Text);
        }
    }
}
