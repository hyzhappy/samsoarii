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
    /// FilterSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class FilterSettingWidget : UserControl,ISaveDialog
    {
        private FilterParams FilterParams;
        public FilterSettingWidget()
        {
            InitializeComponent();
            FilterParams = (FilterParams)ProjectPropertyManager.ProjectPropertyDic["FilterParams"];
            DataContext = FilterParams;
        }

        public void Save()
        {
            FilterParams.IsChecked = (bool)CheckBox.IsChecked;
            FilterParams.FilterTimeIndex = ComboBox.SelectedIndex;
        }
    }
}
