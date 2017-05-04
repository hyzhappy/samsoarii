using SamSoarII.UserInterface;
using SamSoarII.Utility;
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
    public partial class BaseCommunicationInterface : UserControl,ISaveDialog
    {
        public BaseCommunicationInterface()
        {
            InitializeComponent();
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
                    rangeTextbox1.GetTextBox().Text = string.Format("2");
                    rangeTextbox2.GetTextBox().Text = string.Format("20");
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
