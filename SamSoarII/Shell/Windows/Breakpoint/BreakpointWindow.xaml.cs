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
using SamSoarII.Shell.Models;

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
            fitems = new ObservableCollection<FuncBrpoTableElement>();
            DataContext = this;
            OnSimulatePropertyChanged(this, new PropertyChangedEventArgs("IsEnable"));
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        public SimulateManager SimulateManager { get { return ifParent.MNGSimu; } }
        public BreakpointManager BreakpointManager { get { return ifParent.MNGSimu.MNGBrpo; } }

        private ObservableCollection<LadderBrpoTableElement> items;
        public IList<LadderBrpoTableElement> Items { get { return this.items; } }

        private ObservableCollection<FuncBrpoTableElement> fitems;
        public IList<FuncBrpoTableElement> FItems { get { return this.fitems; } }

        public bool IsSelectedItem { get { return DG_Main.IsEnabled && DG_Main.SelectedItem != null || DG_FMain.IsEnabled && DG_FMain.SelectedItem != null; } }

        #endregion
        
        private void UpdateButtonEnable()
        {
            BT_Jump.IsEnabled = SimulateManager.IsEnable && IsSelectedItem;
            BT_Active.IsEnabled = IsSelectedItem;
            BT_Remove.IsEnabled = IsSelectedItem;
            BT_RemoveAll.IsEnabled = items.Count() + fitems.Count() > 0;
        }

        public void Select(LadderUnitModel unit)
        {
            LadderBrpoTableElement element = items.Where(e => e.Parent.Parent == unit).FirstOrDefault();
            DG_Main.SelectedItem = element;
        }
        
        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            // TAB界面发过来的事件
            if (sender is MainTabControl && e is MainTabControlEventArgs)
            {
                MainTabControlEventArgs e1 = (MainTabControlEventArgs)e;
                if (e1.Action == TabAction.SELECT)
                {
                    // 当前界面是函数块
                    if (e1.Tab is FuncBlockViewModel)
                    {
                        DG_Main.IsEnabled = false;
                        DG_Main.Visibility = Visibility.Hidden;
                        DG_FMain.IsEnabled = true;
                        DG_FMain.Visibility = Visibility.Visible;
                    }
                    // 否则显示梯形图断点列表
                    else
                    {
                        DG_Main.IsEnabled = true;
                        DG_Main.Visibility = Visibility.Visible;
                        DG_FMain.IsEnabled = false;
                        DG_FMain.Visibility = Visibility.Hidden;
                    }
                    UpdateButtonEnable();
                }
            }
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
            if (e.NewItems != null)
                foreach (IBreakpoint ibrpo in e.NewItems)
                {
                    if (ibrpo is LadderBrpoModel)
                        items.Add(new LadderBrpoTableElement((LadderBrpoModel)ibrpo));
                    if (ibrpo is FuncBrpoModel)
                        fitems.Add(new FuncBrpoTableElement((FuncBrpoModel)ibrpo));
                }
            if (e.OldItems != null)
                foreach (IBreakpoint ibrpo in e.OldItems)
                {
                    if (ibrpo is LadderBrpoModel)
                        items.Remove(((LadderBrpoModel)ibrpo).Element);
                    if (ibrpo is FuncBrpoModel)
                        fitems.Remove(((FuncBrpoModel)ibrpo).Element);
                }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            UpdateButtonEnable();
        }
        
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IBreakpoint brpo = null;
            if (DG_Main.IsEnabled && DG_Main.SelectedItem != null)
                brpo = ((LadderBrpoTableElement)(DG_Main.SelectedItem)).Parent;
            if (DG_FMain.IsEnabled && DG_FMain.SelectedItem != null)
                brpo = ((FuncBrpoTableElement)(DG_FMain.SelectedItem)).Parent;
            if (sender == BT_Active)
                brpo.IsActive = brpo.IsActive ? false : true;
            if (sender == BT_Jump)
                SimulateManager.JumpTo(brpo.Address);
            if (sender == BT_Remove)
            {
                brpo.IsActive = false;
                brpo.IsEnable = false;
            }
            if (sender == BT_RemoveAll)
            {
                foreach (LadderBrpoTableElement element in items.ToArray())
                {
                    element.Parent.IsActive = false;
                    element.Parent.IsEnable = false;
                }
                foreach (FuncBrpoTableElement element in fitems.ToArray())
                {
                    element.Parent.IsActive = false;
                    element.Parent.IsEnable = false;
                }
            }
        }
        
        private void DG_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonEnable();
        }

        private void DG_FMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonEnable();
        }

        #endregion

    }
}
