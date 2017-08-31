using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// CAMDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CAMDialog : Window, IDisposable, INotifyPropertyChanged
    {
        public CAMDialog(CAMModel _core)
        {
            InitializeComponent();
            Core = _core;
            DataContext = this;
            UpdateMenuItems();
        }

        public void Dispose()
        {
            Core = null;
            DataContext = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private CAMModel core;
        public CAMModel Core
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
                    TBO_CV.Text = core.Children[0].Text;
                    TBO_Num.Text = core.NumStore.Text;
                    TBO_Max.Text = core.MaxTarget.Text;
                    TBO_Ref.Text = core.RefAddr.Text;
                    CB_Mode.SelectedIndex = (int)(core.RefMode);
                    UpdateReflict();
                    PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
                }
            }
        }

        public IList<CAMElement> Elements { get { return core != null ? core.Elements : new CAMElement[] { }; } }

        #endregion

        public void Save()
        {
            core.Parse(new string[] { TBO_CV.Text, String.Format("K{0:d}", Elements.Count) }, false);
            core.NumStore.Text = TBO_Num.Text;
            core.MaxTarget.Text = TBO_Max.Text;
            core.RefAddr.Text = TBO_Ref.Text;
            core.RefMode = (CAMModel.ReflictModes)(CB_Mode.SelectedIndex);
            foreach (CAMElement element in Elements)
                element.LoadFromDataGrid();
            core.ValueManager.Add(core);
        }

        #region Event Handler

        private void UpdateReflict()
        {
            TBO_Ref.IsEnabled = CB_Mode.SelectedIndex == 1;
        }

        private void UpdateMenuItems()
        {
            MI_Add.IsEnabled = true;
            MI_Insert.IsEnabled = GD_Ele.SelectedItems.Count == 1;
            MI_Remove.IsEnabled = GD_Ele.SelectedItems.Count > 0;
            MI_Up.IsEnabled = GD_Ele.SelectedItems.Count > 0 && GD_Ele.SelectedIndex > 0;
            MI_Down.IsEnabled = GD_Ele.SelectedItems.Count > 0 && GD_Ele.SelectedIndex + GD_Ele.SelectedItems.Count < Elements.Count;
        }

        private void CB_Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateReflict();
        }
        
        private void GD_Ele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMenuItems();
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender == MI_Add)
                Elements.Add(new CAMElement(core));
            if (sender == MI_Insert)
                Elements.Insert(GD_Ele.SelectedIndex, new CAMElement(core));
            if (sender == MI_Remove)
            {
                CAMElement[] removes = GD_Ele.SelectedItems.Cast<CAMElement>().ToArray();
                foreach (CAMElement remove in removes)
                    Elements.Remove(remove);
            }
            if (sender == MI_Up)
                core.MoveUp(GD_Ele.SelectedIndex, GD_Ele.SelectedIndex + GD_Ele.SelectedItems.Count - 1);
            if (sender == MI_Down)
                core.MoveDown(GD_Ele.SelectedIndex, GD_Ele.SelectedIndex + GD_Ele.SelectedItems.Count - 1);
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            UpdateMenuItems();
        }

        public event RoutedEventHandler Ensure = delegate { };
        public event RoutedEventHandler Cancel = delegate { };
        public event RoutedEventHandler Help = delegate { };
        
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender == BT_Ensure)
                Ensure(this, new RoutedEventArgs());
            if (sender == BT_Cancel)
                Cancel(this, new RoutedEventArgs());
            if (sender == BT_Help)
                Help(this, new RoutedEventArgs());
        }

        #endregion

    }
}
