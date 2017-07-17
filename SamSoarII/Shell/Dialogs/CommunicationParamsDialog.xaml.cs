using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// CommunicationParamsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationParamsDialog : Window, IDisposable
    {
        private CommunicationParams communicationParams;
        public CommunicationParamsDialog(CommunicationParams communicationParams)
        {
            InitializeComponent();
            this.communicationParams = communicationParams;
            DataContext = this.communicationParams;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
            KeyDown += CommunicationsettingParamsDialog_KeyDown;
            Binding binding = new Binding();
            binding.Source = this.communicationParams;
            binding.Path = new PropertyPath("Timeout");
            rangeTextbox.textbox.SetBinding(TextBox.TextProperty, binding);
        }
        public void Dispose()
        {
            communicationParams = null;
        }
        private void CommunicationsettingParamsDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }
        public void Save()
        {
            communicationParams.SerialPortIndex = SerialPortComboBox.SelectedIndex;
            communicationParams.CheckCodeIndex = CheckCodeComboBox.SelectedIndex;
            communicationParams.DataBitIndex = DataBitComboBox.SelectedIndex;
            communicationParams.StopBitIndex = StopBitComboBox.SelectedIndex;
            communicationParams.Timeout = int.Parse(rangeTextbox.textbox.Text);
            communicationParams.BaudRateIndex = BaudRateComboBox.SelectedIndex;
        }
    }
}
