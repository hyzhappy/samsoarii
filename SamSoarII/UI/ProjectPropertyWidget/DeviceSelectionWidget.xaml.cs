﻿using SamSoarII.AppMain.UI.ProjectPropertyWidget;
using SamSoarII.PLCDevice;
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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// DeviceSelectionWidget.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceSelectionWidget : UserControl,ISaveDialog
    {
        private static List<UserControl> _widget;
        public DeviceSelectionWidget()
        {
            InitializeComponent();
            DataContext = PLCDeviceManager.GetPLCDeviceManager();
        }
        static DeviceSelectionWidget()
        {
            _widget = new List<UserControl>(PLCDeviceManager.GetPLCDeviceManager().GetDeviceMessageDialogs());
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            showMessageDialog(GetDialogIndex(listbox.SelectedIndex));
        }
        private void showMessageDialog(int index)
        {
            ContentGrid.Children.Clear();
            if (_widget[index].Parent == null)
            {
                ContentGrid.Children.Add(_widget[index]);
            }
        }
        private int GetDialogIndex(int selectindex)
        {
            switch (selectindex)
            {
                case 0:
                case 1:
                    return 0;
                case 2:
                case 3:
                    return 1;
                case 4:
                case 5:
                    return 2;
                case 6:
                case 7:
                    return 3;
                case 8:
                case 9:
                    return 4;
                case 10:
                case 11:
                    return 5;
                case 12:
                    return 6;
                case 13:
                    return 7;
                case 14:
                    return 8;
                default:
                    return -1;
            }
        }

        public void Save()
        {
            PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType((PLCDeviceType)Enum.ToObject(typeof(PLCDeviceType),MainList.SelectedIndex));
        }
    }
}
