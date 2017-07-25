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
using SamSoarII.Core.Simulate;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// BreakpointWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BreakpointWindow : UserControl, IWindow, INotifyPropertyChanged
    {
        public BreakpointWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            SimulateManager.PropertyChanged += OnSimulatePropertyChanged;
            BreakpointManager.EnableItemsChanged += OnEnableItemsChanged;
            items = new ObservableCollection<LadderBrpoTableElement>();
            items.CollectionChanged += OnItemsChanged;
            OnSimulatePropertyChanged(this, new PropertyChangedEventArgs("IsEnable"));
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        public SimulateManager SimulateManager { get { return ifParent.MNGSimu; } }
        public BreakpointManager BreakpointManager { get { return ifParent.MNGSimu.MNGBrpo; } }

        private ObservableCollection<LadderBrpoTableElement> items;
        public IEnumerable<LadderBrpoTableElement> Items { get { return this.items; } }
        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            UpdateButtonEnable();
        }
        #endregion

        private void UpdateButtonEnable()
        {
            BT_Jump.IsEnabled = SimulateManager.IsEnable && DG_Main.SelectedItem != null;
            BT_Active.IsEnabled = DG_Main.SelectedItem != null;
            BT_Remove.IsEnabled = DG_Main.SelectedItem != null;
            BT_RemoveAll.IsEnabled = items.Count() > 0;
        }

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
        }
        
        private void OnSimulatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnable":
                    DGC_Cond.IsReadOnly = !SimulateManager.IsEnable;
                    DGC_Skip.IsReadOnly = !SimulateManager.IsEnable;
                    UpdateButtonEnable();
                    break;
            }
        }

        private void OnEnableItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (IBreakpoint ibrpo in e.OldItems)
                {
                    if (ibrpo is LadderBrpoModel)
                        items.Add(new LadderBrpoTableElement((LadderBrpoModel)ibrpo));
                }
            if (e.NewItems != null)
                foreach (IBreakpoint ibrpo in e.NewItems)
                {
                    if (ibrpo is LadderBrpoModel)
                        items.Remove(((LadderBrpoModel)ibrpo).Element);
                }
        }
        
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IBreakpoint brpo = null;
            if (DG_Main.SelectedItem != null)
                brpo = ((LadderBrpoTableElement)(DG_Main.SelectedItem)).Parent;
            if (sender == BT_Active)
                brpo.IsActive = brpo.IsActive ? false : true;
            if (sender == BT_Jump)
                SimulateManager.JumpTo(brpo.Address);
            if (sender == BT_Remove)
                brpo.IsEnable = false;
            if (sender == BT_RemoveAll)
                foreach (LadderBrpoTableElement element in items.ToArray())
                    element.Parent.IsEnable = false;
        }

        #endregion

    }
}
