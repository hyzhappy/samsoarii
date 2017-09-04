using Microsoft.Win32;
using System;
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
using System.Windows.Shapes;
using System.ComponentModel;

using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// PLSBlockDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PLSBlockDialog : Window, IDisposable, INotifyPropertyChanged
    {
        public PLSBlockDialog(ProjectModel _core)
        {
            InitializeComponent();
            core = _core;
            foreach (PLSBlockModel element in Elements)
                element.CreateToDataGrid();
            DataContext = this;
            UpdateButtonEnable();
        }

        public void Dispose()
        {
            DataContext = null;
            core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel core;
        public ProjectModel Core { get { return this.core; } }
        public IList<PLSBlockModel> Elements { get { return core != null ? core.PLSBlocks : new PLSBlockModel[] { }; } }

        #endregion

        public void Save()
        {
            foreach (PLSBlockModel element in Elements)
                element.LoadFromDataGrid();
        }

        #region Event Handler

        public event RoutedEventHandler Ensure = delegate { };

        public event RoutedEventHandler Help = delegate { };
        
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender == BT_Import)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = string.Format("{0}|*.{1}", Properties.Resources.DXF_File, "dxf");
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == true)
                {
                    Elements.Add(new PLSBlockModel(core, dialog.FileName));
                    PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
                } 
            }
            if (sender == BT_Delete)
            {
                Elements.Remove((PLSBlockModel)(GD_Ele.SelectedItem));
                PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            }
            if (sender == BT_Ensure) Ensure(this, e);
            if (sender == BT_Help) Help(this, e);
        }
        
        private void GD_Ele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonEnable();
        }

        private void UpdateButtonEnable()
        {
            BT_Delete.IsEnabled = GD_Ele.SelectedIndex >= 0;
        }

        #endregion
        
    }
}
