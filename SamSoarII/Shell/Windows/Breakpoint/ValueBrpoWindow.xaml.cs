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

using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
using System.ComponentModel;

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
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += CorePropertyChanged;
                    if (core.View != this) core.View = this;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Elements"));
            }
        }
        IModel IViewModel.Core { get { return Core; } set { Core = (ValueBrpoModel)value; } }
        IViewModel IViewModel.ViewParent { get { return Core?.Parent?.View; } }
        private void CorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public IList<ValueBrpoElement> Elements
        {
            get
            {
                return core != null ? core.Children : new ValueBrpoElement[] { };
            }
        }

        #endregion

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };
        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {

        }

        #region Button
        
        public void UpdateButtonEnable()
        {
            BT_Add.IsEnabled = true;
            BT_Active.IsEnabled = DG_Main.SelectedItem != null;
            BT_Remove.IsEnabled = DG_Main.SelectedItem != null;
            BT_RemoveAll.IsEnabled = Elements.Count > 0;
        }

        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BT_Active_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Remove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_RemoveAll_Click(object sender, RoutedEventArgs e)
        {

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

        #endregion

        #endregion
        
    }
}
