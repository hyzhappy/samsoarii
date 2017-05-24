using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
using SamSoarII.LadderInstViewModel;
using System.Configuration;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// FontSelectionWidget.xaml 的交互逻辑
    /// </summary>
    public partial class FontSelectionWidget : UserControl
    {
        public FontSelectionWidget()
        {
            InitializeComponent();
            InitializeFontSizeComboBox();
            InitializeFontFamilyComboBox();
            OptionDialog.EnsureButtonClick += OptionDialog_EnsureButtonClick;
        }
        private void OptionDialog_EnsureButtonClick(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = FontSizeComboBox.SelectedItem as ComboBoxItem;
            string temp = item.Content.ToString();
            int selectFontSize = int.Parse(temp);
            if (selectFontSize != FontManager.GetFontDataProvider().SelectFontSize)
            {
                GlobalSetting.SelectedIndexOfFontSizeComboBox = selectFontSize - 20;
                FontManager.GetFontDataProvider().SelectFontSize = selectFontSize;
            }
            item = FontFamilyComboBox.SelectedItem as ComboBoxItem;
            temp = item.Content.ToString();
            if (!FontManager.GetFontDataProvider().SelectFontFamily.FamilyNames.Values.Contains(temp))
            {
                FontManager.GetFontDataProvider().SelectFontFamily = new FontFamily(temp);
            }
        }

        private void InitializeFontSizeComboBox()
        {
            for (int i = 20; i <= 50; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                FontSizeComboBox.Items.Add(item);
            }
        }
        private void InitializeFontFamilyComboBox()
        {
            foreach (var fontFamily in (new InstalledFontCollection()).Families)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = fontFamily.Name;
                FontFamilyComboBox.Items.Add(item);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalSetting.SelectedIndexOfFontFamilyComboBox = (sender as ComboBox).SelectedIndex;
        }
    }
}
