﻿using System;
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
    /// CommunicationSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationSettingWidget : UserControl, IDisposable
    {
        private List<BaseCommunicationInterface> _widget = new List<BaseCommunicationInterface>();
        
        public CommunicationSettingWidget(CommunicationInterfaceParams com232, CommunicationInterfaceParams com485)
        {
            InitializeComponent();
            _widget.Add(new BaseCommunicationInterface(com232));
            _widget.Add(new BaseCommunicationInterface(com485));
            ShowWidget(0);
        }

        public void Dispose()
        {
            foreach (var item in _widget) item.Dispose();
            _widget.Clear();
            _widget = null;
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