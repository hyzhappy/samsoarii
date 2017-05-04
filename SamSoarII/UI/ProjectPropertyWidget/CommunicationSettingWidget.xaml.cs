using SamSoarII.AppMain.UI.ProjectPropertyWidget.CommunicationInterface;
using SamSoarII.Utility;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget
{
    /// <summary>
    /// CommunicationSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationSettingWidget : UserControl,ISaveDialog
    {
        private List<BaseCommunicationInterface> _widget = new List<BaseCommunicationInterface>();
        public CommunicationSettingWidget()
        {
            InitializeComponent();
            _widget.Add(new COM232());
            _widget.Add(new COM485());
            ShowWidget(0);
        }
        private void ShowWidget(int index)
        {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(_widget[index]);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                ShowWidget(listBox.SelectedIndex);
            }
        }

        public void Save()
        {
            foreach (var item in _widget)
            {
                item.Save();
            }
        }
    }
}
