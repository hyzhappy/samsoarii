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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// ValueBrpoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ValueBrpoWindow : UserControl, IWindow, IViewModel, INotifyPropertyChanged
    {
        public ValueBrpoWindow(InteractionFacade _ifparent)
        {
            InitializeComponent();
            ifparent = _ifparent;
            ifparent.PostIWindowEvent += OnReceiveIWindowEvent;
            elements = new ObservableCollection<ValueBrpoTableElement>();
            DataContext = this;
        }

        public void Dispose()
        {
            Core = null;
            ifparent.PostIWindowEvent -= OnReceiveIWindowEvent;
            ifparent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private InteractionFacade ifparent;
        public InteractionFacade IFParent { get { return this.ifparent; } }

        private ValueBrpoModel core;
        public ValueBrpoModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                ValueBrpoModel _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= CorePropertyChanged;
                    _core.ChildrenChanged -= CoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                    foreach (ValueBrpoTableElement element in elements)
                        element.Dispose();
                }
                this.core = value;
                elements.Clear();
                if (core != null)
                {
                    core.PropertyChanged += CorePropertyChanged;
                    core.ChildrenChanged += CoreChildrenChanged;
                    if (core.View != this) core.View = this;
                    foreach (ValueBrpoElement element in core.Children)
                        elements.Add(new ValueBrpoTableElement(element));
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            }
        }
        IModel IViewModel.Core { get { return Core; } set { Core = (ValueBrpoModel)value; } }
        IViewModel IViewModel.ViewParent { get { return Core?.Parent?.View; } }
        private void CorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
        private void CoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                for (int i = e.NewItems.Count - 1; i >= 0; i--)
                    elements.Insert(e.NewStartingIndex, new ValueBrpoTableElement((ValueBrpoElement)(e.NewItems[i])));
            if (e.OldItems != null)
                foreach (ValueBrpoElement element in e.OldItems)
                {
                    elements.Remove(element.View);
                    element.View.Dispose();
                }
            PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
        }

        private ObservableCollection<ValueBrpoTableElement> elements;
        public IList<ValueBrpoTableElement> Elements { get { return this.elements; } }

        #endregion

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };
        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {

        }

        #region Button

        public void UpdateButtonEnable()
        {
            BT_Add.IsEnabled = core != null;
            BT_Active.IsEnabled = core != null && DG_Main.SelectedItem != null && ((ValueBrpoTableElement)(DG_Main.SelectedItem)).Core.IsValid;
            BT_Remove.IsEnabled = core != null && DG_Main.SelectedItem != null;
            BT_RemoveAll.IsEnabled = core != null && Elements.Count > 0;
        }

        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            core.Children.Add(new ValueBrpoElement(core));
            //PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
        }

        private void BT_Active_Click(object sender, RoutedEventArgs e)
        {
            ((ValueBrpoTableElement)(DG_Main.SelectedItem)).Core.IsActive ^= true;
        }

        private void BT_Remove_Click(object sender, RoutedEventArgs e)
        {
            core.Children.Remove(((ValueBrpoTableElement)(DG_Main.SelectedItem)).Core);
            //PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
        }

        private void BT_RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            core.Children.Clear();
            //PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
        }

        #endregion

        #region Radio

        private void RB_Spe_Checked(object sender, RoutedEventArgs e)
        {
            RB_All.IsChecked = false;
        }

        private void RB_All_Checked(object sender, RoutedEventArgs e)
        {
            RB_Spe.IsChecked = false;
        }

        #endregion

        #region DataGrid

        private void DG_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonEnable();
        }
        
        private void DG_Main_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }
        
        private void DG_Main_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ValueBrpoTableElement element = (ValueBrpoTableElement)(e.Row.DataContext);
            if (e.Column == DGC_LValue) element.LeftValue = ((TextBox)(e.EditingElement)).Text;
            if (e.Column == DGC_Cond) element.Operation = ((ComboBox)(e.EditingElement)).Text;
            if (e.Column == DGC_RValue) element.RightValue = ((TextBox)(e.EditingElement)).Text;
            if (e.Column == DGC_Type) element.ValueType = ((ComboBox)(e.EditingElement)).Text;
            element.SaveToCore();
            element.LoadFromCore();
        }

        private void DG_Main_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

        }

        #endregion

        #endregion

    }
}
