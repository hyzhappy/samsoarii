using SamSoarII.LadderInstViewModel;
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
        public OptionDialog(InteractionFacade _interactionFacade)
        {
            InitializeComponent();
            _widget.Add(new FontSelectionWidget());
            //_widget.Add(new ColorSelectionWidget());
            _widget.Add(new OtherSettingWidget(_interactionFacade));
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
            FontManager.GetTitle().Setup(
                DemoFontManager.GetTitle());
            FontManager.GetComment().Setup(
                DemoFontManager.GetComment());
            FontManager.GetInst().Setup(
                DemoFontManager.GetInst());
            FontManager.GetLadder().Setup(
                DemoFontManager.GetLadder());
            FontManager.GetFunc().Setup(
                DemoFontManager.GetFunc());
            ColorManager.GetComment().Setup(
                DemoColorManager.GetComment());
            ColorManager.GetDiagramTitle().Setup(
                DemoColorManager.GetDiagramTitle());
            ColorManager.GetFuncScreen().Setup(
                DemoColorManager.GetFuncScreen());
            ColorManager.GetInst().Setup(
                DemoColorManager.GetInst());
            ColorManager.GetInstScreen().Setup(
                DemoColorManager.GetInstScreen());
            ColorManager.GetLadder().Setup(
                DemoColorManager.GetLadder());
            ColorManager.GetLadderScreen().Setup(
                DemoColorManager.GetLadderScreen());
            ColorManager.GetNetworkTitle().Setup(
                DemoColorManager.GetNetworkTitle());
            Close();
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
        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Window window = sender as Window;
            e.Cancel = true;
            window.Hide();
        }
    }
}
