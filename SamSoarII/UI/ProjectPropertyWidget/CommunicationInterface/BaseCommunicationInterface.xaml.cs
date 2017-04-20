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
    public partial class BaseCommunicationInterface : UserControl,INotifyPropertyChanged,ISaveDialog
    {
        public BaseCommunicationInterface()
        {
            InitializeComponent();
            stackpaneldata1.DataContext = this;
            stackpaneldata2.DataContext = this;
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
            if (oldvalue == 0 && (e.Key == Key.D0 || e.Key == Key.NumPad0))
            {
                e.Handled = true;
            }
            if (KeyInputHelper.CanInputAssert(e.Key))
            {
                int newvalue;
                if (textbox.Name == "NTextBox")
                {
                    if (KeyInputHelper.NumAssert(e.Key))
                    {
                        newvalue = 10 * oldvalue + KeyInputHelper.GetKeyValue(e.Key);
                        if (newvalue > 128)
                        {
                            e.Handled = true;
                        }
                    }
                }
                else if(textbox.Name == "TTextBox")
                {
                    if (KeyInputHelper.NumAssert(e.Key))
                    {
                        newvalue = 10 * oldvalue + KeyInputHelper.GetKeyValue(e.Key);
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
        public virtual void SetCommunicationType(CommunicationType type)
        {
            switch (type)
            {
                case CommunicationType.Master:
                    Master.IsChecked = true;
                    break;
                case CommunicationType.Slave:
                    Slave.IsChecked = true;
                    break;
                case CommunicationType.FreePort:
                    FreeButton.IsChecked = true;
                    break;
            }
        }
        public virtual CommunicationType GetCommunicationType()
        {
            if (Master.IsChecked.HasValue && Master.IsChecked.Value)
            {
                return CommunicationType.Master;
            }
            if (Slave.IsChecked.HasValue && Slave.IsChecked.Value)
            {
                return CommunicationType.Slave;
            }
            if (FreeButton.IsChecked.HasValue && FreeButton.IsChecked.Value)
            {
                return CommunicationType.FreePort;
            }
            return CommunicationType.Master;
        }
        public virtual void Save()
        {
            if (this is COM232)
            {
                (this as COM232).Save();
            }
            else
            {
                (this as COM485).Save();
            }
        }
    }
}
