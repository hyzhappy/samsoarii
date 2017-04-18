using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// OptionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OptionDialog : Window
    {
        private List<UserControl> _widget = new List<UserControl>();
        public static event RoutedEventHandler EnsureButtonClick = delegate { };
        public OptionDialog()
        {
            InitializeComponent();
            _widget.Add(new FontSelectionWidget());
            _widget.Add(new ColorSelectionWidget());
            ShowWidget(0);
        }
        private void ShowWidget(int index)
        {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(_widget[index]);
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                ShowWidget(listBox.SelectedIndex);
            }
        }

        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick.Invoke(this,e);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}
