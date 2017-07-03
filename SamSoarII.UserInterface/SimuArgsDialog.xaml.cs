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
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    public class SimuArgsDialogValuesArgs : EventArgs
    {
        public object[] Values { get; set; }
        public bool[] IsLocks { get; set; }
    }
    public delegate void SimuArgsDialogValuesHandler(object sender, SimuArgsDialogValuesArgs e); 

    public class SimuArgsDialogUnlockValue
    {

    }

    /// <summary>
    /// SimuArgsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SimuArgsDialog : Window
    {
        public SimuArgsDialog()
        {
            InitializeComponent();
        }

        private string[] _types;

        public SimuArgsDialog(
            string[] labels, 
            string[] values, 
            string[] types,
            bool[] islocks)
        {
            bool[] canlocks = null;
            if (types != null)
            {
                canlocks = new bool[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    canlocks[i] = !types[i].Equals("READONLY");
                }
            }
            InitializeComponent();
            this._types = types;
            if (labels == null || labels.Length < 5)
            {
                LB_Arg5.Visibility = Visibility.Collapsed;
                LE_Arg5.Visibility = Visibility.Collapsed;
                TB_Arg5.Visibility = Visibility.Collapsed;
                TB_Arg5.Text = String.Empty;
                CB_Lock5.Visibility = Visibility.Collapsed;
                TB_Lock5.Visibility = Visibility.Collapsed;
            }
            else
            {
                LB_Arg5.Text = labels[4];
                TB_Arg5.Text = values[4];
                TB_Arg5.IsEnabled = canlocks[4];
                CB_Lock5.IsEnabled = canlocks[4];
                CB_Lock5.IsChecked = islocks[4];
                CB_Lock5.IsChecked |= (values[4].Length > 0);
            }
            if (labels == null || labels.Length < 4)
            {
                LB_Arg4.Visibility = Visibility.Collapsed;
                LE_Arg4.Visibility = Visibility.Collapsed;
                TB_Arg4.Visibility = Visibility.Collapsed;
                TB_Arg4.Text = String.Empty;
                CB_Lock4.Visibility = Visibility.Collapsed;
                TB_Lock4.Visibility = Visibility.Collapsed;
            }
            else
            {
                LB_Arg4.Text = labels[3];
                TB_Arg4.Text = values[3];
                TB_Arg4.IsEnabled = canlocks[3];
                CB_Lock4.IsEnabled = canlocks[3];
                CB_Lock4.IsChecked = islocks[3];
                CB_Lock4.IsChecked |= (values[3].Length > 0);
            }
            if (labels == null || labels.Length < 3)
            {
                LB_Arg3.Visibility = Visibility.Collapsed;
                LE_Arg3.Visibility = Visibility.Collapsed;
                TB_Arg3.Visibility = Visibility.Collapsed;
                TB_Arg3.Text = String.Empty;
                CB_Lock3.Visibility = Visibility.Collapsed;
                TB_Lock3.Visibility = Visibility.Collapsed;
            }
            else
            {
                LB_Arg3.Text = labels[2];
                TB_Arg3.Text = values[2];
                TB_Arg3.IsEnabled = canlocks[2];
                CB_Lock3.IsEnabled = canlocks[2];
                CB_Lock3.IsChecked = islocks[2];
                CB_Lock3.IsChecked |= (values[2].Length > 0);
            }
            if (labels == null || labels.Length < 2)
            {
                LB_Arg2.Visibility = Visibility.Collapsed;
                LE_Arg2.Visibility = Visibility.Collapsed;
                TB_Arg2.Visibility = Visibility.Collapsed;
                TB_Arg2.Text = String.Empty;
                CB_Lock2.Visibility = Visibility.Collapsed;
                TB_Lock2.Visibility = Visibility.Collapsed;
            }
            else
            {
                LB_Arg2.Text = labels[1];
                TB_Arg2.Text = values[1];
                TB_Arg2.IsEnabled = canlocks[1];
                CB_Lock2.IsEnabled = canlocks[1];
                CB_Lock2.IsChecked = islocks[1];
                CB_Lock2.IsChecked |= (values[1].Length > 0);
            }
            if (labels == null || labels.Length < 1)
            {
                LB_Arg1.Visibility = Visibility.Collapsed;
                LE_Arg1.Visibility = Visibility.Collapsed;
                TB_Arg1.Visibility = Visibility.Collapsed;
                TB_Arg1.Text = String.Empty;
                CB_Lock1.Visibility = Visibility.Collapsed;
                TB_Lock1.Visibility = Visibility.Collapsed;
            }
            else
            {
                LB_Arg1.Text = labels[0];
                TB_Arg1.Text = values[0];
                TB_Arg1.IsEnabled = canlocks[0];
                CB_Lock1.IsEnabled = canlocks[0];
                CB_Lock1.IsChecked = islocks[0];
                CB_Lock1.IsChecked |= (values[0].Length > 0);
            }
        }

        private object Parse(string text, string type)
        {
            if (text.Equals(String.Empty))
            {
                return new SimuArgsDialogUnlockValue();
            }
            Int32 valuew;
            Int64 valued;
            double valuef;
            try
            {
                switch (type)
                {
                    case "BIT":
                        valuew = int.Parse(text);
                        if (valuew < 0 || valuew > 1)
                            return null;
                        return valuew;
                    case "WORD":
                        valuew = int.Parse(text);
                        return valuew;
                    case "DWORD":
                        valued = int.Parse(text);
                        return valued;
                    case "FLOAT":
                        valuef = float.Parse(text);
                        return valuef;
                    default:
                        return null;
                }
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private void ShowErrorDialog(string label, string text, string type)
        {
            string msg1 = String.Format("{0:s}的值{1:s}的格式非法或者超出范围！");
            string msg2 = String.Empty;
            switch (type)
            {
                case "BIT": msg2 = string.Format("{0}(0~1)。", Properties.Resources.Valid_Range); break;
                case "WORD": msg2 = string.Format("{0}(-(2^31)~(2^31)-1)。", Properties.Resources.Valid_Range); break;
                case "DWORD": msg2 = string.Format("{0}(-(2^63)~(2^63)-1)。", Properties.Resources.Valid_Range); break;
                default: break;
            }
            LocalizedMessageBox.Show(msg1 + msg2,LocalizedMessageIcon.Warning);
        }

        public event SimuArgsDialogValuesHandler EnsureClick = delegate { };
        private void B_OK_Click(object sender, RoutedEventArgs e)
        {
            if (TB_Arg1.Background.Equals(Brushes.Red))
            {
                ShowErrorDialog(LB_Arg1.Text, TB_Arg1.Text, _types[0]);
            }
            if (TB_Arg2.Background.Equals(Brushes.Red))
            {
                ShowErrorDialog(LB_Arg2.Text, TB_Arg2.Text, _types[0]);
            }
            if (TB_Arg3.Background.Equals(Brushes.Red))
            {
                ShowErrorDialog(LB_Arg3.Text, TB_Arg3.Text, _types[0]);
            }
            if (TB_Arg4.Background.Equals(Brushes.Red))
            {
                ShowErrorDialog(LB_Arg4.Text, TB_Arg4.Text, _types[0]);
            }
            if (TB_Arg5.Background.Equals(Brushes.Red))
            {
                ShowErrorDialog(LB_Arg5.Text, TB_Arg5.Text, _types[0]);
            }
            SimuArgsDialogValuesArgs _e = new SimuArgsDialogValuesArgs();
            _e.Values = new object[5];
            _e.IsLocks = new bool[5];
            if (_types != null && _types.Length > 0)
                _e.Values[0] = Parse(TB_Arg1.Text, _types[0]);
            if (_types != null && _types.Length > 1)
                _e.Values[1] = Parse(TB_Arg2.Text, _types[1]);
            if (_types != null && _types.Length > 2)
                _e.Values[2] = Parse(TB_Arg3.Text, _types[2]);
            if (_types != null && _types.Length > 3)
                _e.Values[3] = Parse(TB_Arg4.Text, _types[3]);
            if (_types != null && _types.Length > 4)
                _e.Values[4] = Parse(TB_Arg5.Text, _types[4]);
            _e.IsLocks[0] = (CB_Lock1.IsChecked == true);
            _e.IsLocks[1] = (CB_Lock1.IsChecked == true);
            _e.IsLocks[2] = (CB_Lock1.IsChecked == true);
            _e.IsLocks[3] = (CB_Lock1.IsChecked == true);
            _e.IsLocks[4] = (CB_Lock1.IsChecked == true);
            EnsureClick(this, _e);
        }

        public event SimuArgsDialogValuesHandler CancelClick = delegate { };
        private void B_Cancel_Click(object sender, RoutedEventArgs e)
        {
            SimuArgsDialogValuesArgs _e = new SimuArgsDialogValuesArgs();
            /*
            _e.Values = new string[5];
            if (_types != null && _types.Length > 0)
                _e.Values[0] = Parse(TB_Arg1.Text, _types[0]);
            if (_types != null && _types.Length > 1)
                _e.Values[1] = Parse(TB_Arg2.Text, _types[1]);
            if (_types != null && _types.Length > 2)
                _e.Values[2] = Parse(TB_Arg3.Text, _types[2]);
            if (_types != null && _types.Length > 3)
                _e.Values[3] = Parse(TB_Arg4.Text, _types[3]);
            if (_types != null && _types.Length > 4)
                _e.Values[4] = Parse(TB_Arg5.Text, _types[4]);
            */
            CancelClick(this, _e);
        }

        private void TB_Arg1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_types == null || _types.Length <= 0)
            {
                return;
            }
            if (Parse(TB_Arg1.Text, _types[0]) == null)
            {
                TB_Arg1.Background = Brushes.Red;
            }
            else
            {
                TB_Arg1.Background = Brushes.White;
            }
        }

        private void TB_Arg2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_types == null || _types.Length <= 1)
            {
                return;
            }
            if (Parse(TB_Arg2.Text, _types[1]) == null)
            {
                TB_Arg2.Background = Brushes.Red;
            }
            else
            {
                TB_Arg2.Background = Brushes.White;
            }
        }

        private void TB_Arg3_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (_types == null || _types.Length <= 2)
            {
                return;
            }
            if (Parse(TB_Arg3.Text, _types[2]) == null)
            {
                TB_Arg3.Background = Brushes.Red;
            }
            else
            {
                TB_Arg3.Background = Brushes.White;
            }
        }

        private void TB_Arg4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_types == null || _types.Length <= 3)
            {
                return;
            }
            if (Parse(TB_Arg4.Text, _types[3]) == null)
            {
                TB_Arg4.Background = Brushes.Red;
            }
            else
            {
                TB_Arg4.Background = Brushes.White;
            }
        }

        private void TB_Arg5_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_types == null || _types.Length <= 4)
            {
                return;
            }
            if (Parse(TB_Arg5.Text, _types[4]) == null)
            {
                TB_Arg5.Background = Brushes.Red;
            }
            else
            {
                TB_Arg5.Background = Brushes.White;
            }
        }
    }
}
