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
using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// BaseCommunicationInterface.xaml 的交互逻辑
    /// </summary>
    public partial class BaseCommunicationInterface : UserControl, IDisposable
    {
        public BaseCommunicationInterface()
        {
            InitializeComponent();
        }

        public BaseCommunicationInterface(CommunicationInterfaceParams _core)
        {
            InitializeComponent();
            core = _core;
            DataContext = core;
            SetCommunicationType(core.ComType);
        }

        public virtual void Dispose()
        {
            core = null;
            DataContext = null;
        }

        private CommunicationInterfaceParams core;
        public CommunicationInterfaceParams Core { get { return this.core; } }
        
        private void DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            using (DefaultValueDialog dialog = new DefaultValueDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    Combox1.SelectedIndex = 1;
                    Combox3.SelectedIndex = 0;
                    Combox4.SelectedIndex = 0;
                    Master.IsChecked = true;
                    rangeTextbox1.GetTextBox().Text = string.Format("2");
                    rangeTextbox2.GetTextBox().Text = string.Format("20");
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }
        
        public virtual void SetCommunicationType(CommunicationInterfaceParams.ComTypes type)
        {
            switch (type)
            {
                case CommunicationInterfaceParams.ComTypes.Master:
                    Master.IsChecked = true;
                    break;
                case CommunicationInterfaceParams.ComTypes.Slave:
                    Slave.IsChecked = true;
                    break;
                case CommunicationInterfaceParams.ComTypes.FreePort:
                    FreeButton.IsChecked = true;
                    break;
            }
        }
        public virtual CommunicationInterfaceParams.ComTypes GetCommunicationType()
        {
            if (Master.IsChecked.HasValue && Master.IsChecked.Value)
            {
                return CommunicationInterfaceParams.ComTypes.Master;
            }
            if (Slave.IsChecked.HasValue && Slave.IsChecked.Value)
            {
                return CommunicationInterfaceParams.ComTypes.Slave;
            }
            if (FreeButton.IsChecked.HasValue && FreeButton.IsChecked.Value)
            {
                return CommunicationInterfaceParams.ComTypes.FreePort;
            }
            return CommunicationInterfaceParams.ComTypes.Master;
        }

        public void Save()
        {
            core.ComType = GetCommunicationType();
            core.StationNumber = int.Parse(rangeTextbox1.GetTextBox().Text);
            core.Timeout = int.Parse(rangeTextbox2.GetTextBox().Text);
        }
    }
}
