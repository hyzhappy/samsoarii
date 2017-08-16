using SamSoarII.Global;
using SamSoarII.Shell.Managers;
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
    /// OptionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OptionDialog : Window
    {
        private InteractionFacade ifparent;
        private FontSettingWidget wdFont;
        private OtherSettingWidget wdOther;
        private List<UserControl> _widget = new List<UserControl>();

        public event RoutedEventHandler EnsureButtonClick = delegate { };

        public OptionDialog(InteractionFacade _ifparent)
        {
            ifparent = _ifparent;
            InitializeComponent();
            wdFont = new FontSettingWidget();
            wdOther = new OtherSettingWidget();
            _widget.Add(wdFont);
            _widget.Add(wdOther);
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
            FontManager.GetLadder().Setup(
                DemoFontManager.GetLadder());
            FontManager.GetFunc().Setup(
                DemoFontManager.GetFunc());
            wdOther.Save();
            ifparent.ThMNGCore.MNGInst.TimeSpan = GlobalSetting.InstTimeSpan;
            ifparent.ThMNGCore.MNGSave.TimeSpan = GlobalSetting.SaveTimeSpan;
            EnsureButtonClick.Invoke(this, new RoutedEventArgs(ButtonBase.ClickEvent));
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
