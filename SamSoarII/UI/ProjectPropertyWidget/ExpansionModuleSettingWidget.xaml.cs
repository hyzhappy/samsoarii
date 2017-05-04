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
    /// ExpansionModuleSettingWidget.xaml 的交互逻辑
    /// </summary>
    public enum ModuleType
    {
        input_IO,
        output_IO,
        input_AD,
        output_DA
    }
    public partial class ExpansionModuleSettingWidget : UserControl,ISaveDialog
    {
        private int oldIndex = 0;
        private ExpansionModuleParams ExpanModuleParams;
        public ExpansionModuleSettingWidget()
        {
            InitializeComponent();
            ExpanModuleParams = (ExpansionModuleParams)ProjectPropertyManager.ParamsDic["ExpanModuleParams"];
            DataContext = ExpanModuleParams;
        }
        public void Save()
        {
            ExpanModuleParams.IsExpansion = (bool)checkbox.IsChecked;
            ExpanModuleParams.ModuleIndex = combox.SelectedIndex;
            ExpanModuleParams.Module1TypeIndex = combox1.SelectedIndex;
            ExpanModuleParams.Module2TypeIndex = combox2.SelectedIndex;
            ExpanModuleParams.Module3TypeIndex = combox3.SelectedIndex;
            ExpanModuleParams.Module4TypeIndex = combox4.SelectedIndex;
            ExpanModuleParams.Module5TypeIndex = combox5.SelectedIndex;
            ExpanModuleParams.Module6TypeIndex = combox6.SelectedIndex;
            ExpanModuleParams.Module7TypeIndex = combox7.SelectedIndex;
            ExpanModuleParams.Module8TypeIndex = combox8.SelectedIndex;
            ExpanModuleParams.Module1IsUsed = (bool)checkbox0.IsChecked;
            ExpanModuleParams.Module2IsUsed = (bool)checkbox1.IsChecked;
            ExpanModuleParams.Module3IsUsed = (bool)checkbox2.IsChecked;
            ExpanModuleParams.Module4IsUsed = (bool)checkbox3.IsChecked;
            ExpanModuleParams.Module5IsUsed = (bool)checkbox4.IsChecked;
            ExpanModuleParams.Module6IsUsed = (bool)checkbox5.IsChecked;
            ExpanModuleParams.Module7IsUsed = (bool)checkbox6.IsChecked;
            ExpanModuleParams.Module8IsUsed = (bool)checkbox7.IsChecked;
        }
        private void SetZIndex(int index,bool isUp)
        {
            switch (index)
            {
                case 0:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox0, 11);
                        Panel.SetZIndex(combox1, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox0, 10);
                        Panel.SetZIndex(combox1, 10);
                    }
                    break;
                case 1:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox1, 11);
                        Panel.SetZIndex(combox2, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox1, 10);
                        Panel.SetZIndex(combox2, 10);
                    }
                    break;
                case 2:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox2, 11);
                        Panel.SetZIndex(combox3, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox2, 10);
                        Panel.SetZIndex(combox3, 10);
                    }
                    break;
                case 3:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox3, 11);
                        Panel.SetZIndex(combox4, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox3, 10);
                        Panel.SetZIndex(combox4, 10);
                    }
                    break;
                case 4:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox4, 11);
                        Panel.SetZIndex(combox5, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox4, 10);
                        Panel.SetZIndex(combox5, 10);
                    }
                    break;
                case 5:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox5, 11);
                        Panel.SetZIndex(combox6, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox5, 10);
                        Panel.SetZIndex(combox6, 10);
                    }
                    break;
                case 6:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox6, 11);
                        Panel.SetZIndex(combox7, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox6, 10);
                        Panel.SetZIndex(combox7, 10);
                    }
                    break;
                case 7:
                    if (isUp)
                    {
                        Panel.SetZIndex(checkbox7, 11);
                        Panel.SetZIndex(combox8, 11);
                    }
                    else
                    {
                        Panel.SetZIndex(checkbox7, 10);
                        Panel.SetZIndex(combox8, 10);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetZIndex(oldIndex, false);
            oldIndex = combox.SelectedIndex;
            SetZIndex(oldIndex, true);
        }
    }
}
