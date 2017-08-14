using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// TBLDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TBLDialog : Window, INotifyPropertyChanged, IDisposable
    {
        public TBLDialog(TBLModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;
            UpdateButtonEnable();
        }

        public void Dispose()
        {
            Core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private TBLModel core;
        public TBLModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                this.core = value;
                if (core != null)
                {
                    core.CreateToDataGrid();
                    TB_Addr.Text = core.Children[0].Text.Equals("???") ? "" : core.Children[0].Text;
                    TB_Out.Text = core.Children[1].Text.Equals("???") ? "" : core.Children[1].Text;
                    TB_Dir.Text = core.Children[2].Text.Equals("???") ? "" : core.Children[2].Text;
                    CB_Mode.SelectedIndex = (int)(core.Mode);
                    PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
                }
                
            }
        }

        public IList<TBLElement> Elements { get { return core != null ? core.Elements : new TBLElement[] { }; } }

        #endregion

        public void SaveToCore()
        {
            core.Children[0].Text = TB_Addr.Text;
            core.Children[1].Text = TB_Out.Text;
            core.Children[2].Text = TB_Dir.Text;
            core.Mode = (TBLModel.Modes)(CB_Mode.SelectedIndex);
            foreach (TBLElement element in Elements)
                element.LoadFromDataGrid();
        }

        #region Event Handler

        public event RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Ensure(this, e);
        }
        
        public event RoutedEventHandler Cancel = delegate { };
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Cancel(this, e);
        }
        
        private void GD_Ele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonEnable();
        }

        private void UpdateButtonEnable()
        {
            MI_Add.IsEnabled = true;
            MI_Insert.IsEnabled = GD_Ele.SelectedItems.Count == 1;
            MI_Remove.IsEnabled = GD_Ele.SelectedItems.Count > 0;
            MI_Up.IsEnabled = GD_Ele.SelectedItems.Count > 0 && GD_Ele.SelectedIndex > 0;
            MI_Down.IsEnabled = GD_Ele.SelectedItems.Count > 0 && GD_Ele.SelectedIndex + GD_Ele.SelectedItems.Count < Elements.Count;
        }
        
        private void MI_Add_Click(object sender, RoutedEventArgs e)
        {
            Elements.Add(new TBLElement(core));
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            UpdateButtonEnable();
        }

        private void MI_Insert_Click(object sender, RoutedEventArgs e)
        {
            Elements.Insert(GD_Ele.SelectedIndex, new TBLElement(core));
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            UpdateButtonEnable();
        }

        private void MI_Remove_Click(object sender, RoutedEventArgs e)
        {
            TBLElement[] removes = GD_Ele.SelectedItems.Cast<TBLElement>().ToArray();
            foreach (TBLElement remove in removes)
                Elements.Remove(remove);
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            UpdateButtonEnable();
        }

        private void MI_Up_Click(object sender, RoutedEventArgs e)
        {
            core.MoveUp(GD_Ele.SelectedIndex, GD_Ele.SelectedIndex + GD_Ele.SelectedItems.Count - 1);
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            UpdateButtonEnable();
        }

        private void MI_Down_Click(object sender, RoutedEventArgs e)
        {
            core.MoveDown(GD_Ele.SelectedIndex, GD_Ele.SelectedIndex + GD_Ele.SelectedItems.Count - 1);
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            UpdateButtonEnable();
        }
        #endregion

    }
}
