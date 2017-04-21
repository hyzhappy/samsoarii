using SamSoarII.UserInterface;
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

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget.CommunicationInterface
{
    /// <summary>
    /// COM232.xaml 的交互逻辑
    /// </summary>
    public partial class BaseCommunicationInterface : UserControl,INotifyPropertyChanged
    {
        public BaseCommunicationInterface()
        {
            InitializeComponent();
            DataContext = this;
        }
        public bool NCanUp
        {
            get
            {
                return NTextBox.Text == string.Empty || int.Parse(NTextBox.Text) < 128;
            }
        }
        public bool NCanDown
        {
            get
            {
                return NTextBox.Text == string.Empty || int.Parse(NTextBox.Text) > 1;
            }
        }
        public bool TCanUp
        {
            get
            {
                return TTextBox.Text == string.Empty || int.Parse(TTextBox.Text) < 255;
            }
        }
        public bool TCanDown
        {
            get
            {
                return TTextBox.Text == string.Empty || int.Parse(TTextBox.Text) > 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            int oldvalue = textbox.Text == string.Empty ? 0: int.Parse(textbox.Text);
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right)
            {
                int newvalue;
                if (textbox.Name == "NTextBox")
                {
                    if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                    {
                        newvalue = 10 * oldvalue + GetKeyValue(e.Key);
                        if (newvalue > 128)
                        {
                            e.Handled = true;
                        }
                    }
                }
                else if(textbox.Name == "TTextBox")
                {
                    if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                    {
                        newvalue = 10 * oldvalue + GetKeyValue(e.Key);
                        if (newvalue > 255)
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        private int GetKeyValue(Key key)
        {
            switch (key)
            {
                case Key.D0:
                case Key.NumPad0:
                    return 0;
                case Key.D1:
                case Key.NumPad1:
                    return 1;
                case Key.D2:
                case Key.NumPad2:
                    return 2;
                case Key.D3:
                case Key.NumPad3:
                    return 3;
                case Key.D4:
                case Key.NumPad4:
                    return 4;
                case Key.D5:
                case Key.NumPad5:
                    return 5;
                case Key.D6:
                case Key.NumPad6:
                    return 6;
                case Key.D7:
                case Key.NumPad7:
                    return 7;
                case Key.D8:
                case Key.NumPad8:
                    return 8;
                case Key.D9:
                case Key.NumPad9:
                    return 9;
                default:
                    return -1;
            }
        }

        private void UpClick(object sender, RoutedEventArgs e)
        {
            int value;
            if (sender == NUpButton)
            {
                if (NTextBox.Text == string.Empty)
                {
                    NTextBox.Text = string.Format("2");
                }
                else
                {
                    value = int.Parse(NTextBox.Text) + 1;
                    NTextBox.Text = value.ToString();
                }
            }
            else
            {
                if (TTextBox.Text == string.Empty)
                {
                    TTextBox.Text = string.Format("20");
                }
                else
                {
                    value = int.Parse(TTextBox.Text) + 1;
                    TTextBox.Text = value.ToString();
                }
            }
        }
        private void DownClick(object sender, RoutedEventArgs e)
        {
            int value;
            if (sender == NDownButton)
            {
                if (NTextBox.Text == string.Empty)
                {
                    NTextBox.Text = string.Format("2");
                }
                else
                {
                    value = int.Parse(NTextBox.Text) - 1;
                    NTextBox.Text = value.ToString();
                }
            }
            else
            {
                if (TTextBox.Text == string.Empty)
                {
                    TTextBox.Text = string.Format("20");
                }
                else
                {
                    value = int.Parse(TTextBox.Text) - 1;
                    TTextBox.Text = value.ToString();
                }
            }
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == NTextBox)
            {
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("NCanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NCanDown"));
            }
            else
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TCanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TCanDown"));
            }
        }

        private void DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            using (DefaultValueDialog dialog = new DefaultValueDialog())
            {
                dialog.EnsureButtonClick += (sender1,e1) => 
                {
                    Combox1.SelectedIndex = 1;
                    Combox3.SelectedIndex = 0;
                    Combox4.SelectedIndex = 0;
                    Master.IsChecked = true;
                    NTextBox.Text = string.Format("2");
                    TTextBox.Text = string.Format("20");
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }
    }
}
