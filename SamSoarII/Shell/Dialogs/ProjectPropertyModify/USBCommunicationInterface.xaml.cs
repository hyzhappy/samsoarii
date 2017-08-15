using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
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
    /// USBCommunicationInterface.xaml 的交互逻辑
    /// </summary>
    public partial class USBCommunicationInterface : UserControl, IDisposable
    {
        public USBCommunicationInterface(USBCommunicationParams usbParams)
        {
            InitializeComponent();
            core = usbParams;
            DataContext = core;
            Binding binding = new Binding();
            binding.Source = core;
            binding.Path = new PropertyPath("Timeout");
            RT_box.GetTextBox().SetBinding(TextBox.TextProperty, binding);
        }

        private USBCommunicationParams core;
        public USBCommunicationParams Core { get { return this.core; } }

        public void Dispose()
        {
            core = null;
            DataContext = null;
        }

        public void Save()
        {
            core.Timeout = int.Parse(RT_box.GetTextBox().Text);
        }
    }
}
