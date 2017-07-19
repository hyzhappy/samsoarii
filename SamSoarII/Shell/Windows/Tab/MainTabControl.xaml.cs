using SamSoarII.Core.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// MainTabControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainTabControl : LayoutDocumentPane, IWindow
    {
        public MainTabControl(InteractionFacade _ifParent)
        {
            InitializeComponent();
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            tabitems = new ObservableCollection<ITabItem>();
        }

        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }

        private ObservableCollection<ITabItem> tabitems;
        public IList<ITabItem> TabItems { get; set; }

        private ITabItem selecteditem;
        public ITabItem SelectedItem
        {
            get
            {
                return this.selecteditem;
            }
            set
            {
                if (selecteditem != value)
                {
                    this.selecteditem = value;
                    Post(this, new MainTabControlEventArgs(TabAction.SELECT, selecteditem));
                }
            }
        }

        #endregion

        #region View Mode
        
        public const int VIEWMODE_LADDER = 0x01;
        public const int VIEWMODE_INST = 0x02;

        private int viewmode;
        public int ViewMode
        {
            get
            {
                return this.viewmode;
            }
            set
            {
                this.viewmode = value;
                foreach (ITabItem tab in tabitems)
                    if (tab is MainTabDiagramItem)
                        ((MainTabDiagramItem)tab).ViewMode = value;
                Post(this, new MainTabControlEventArgs(TabAction.VIEWMODE, selecteditem));
            }
        }

        #endregion

        #region Tab

        public ITabItem ToTabItem(IModel model)
        {
            if (model.View == null)
                return null;
            if (model is LadderDiagramModel)
                return ((LadderDiagramModel)model).Tab;
            if (model is FuncBlockModel)
                return ((FuncBlockModel)model).View;
            if (model is ModbusTableModel)
                return ((ModbusTableModel)model).View;
            return null;
        }

        public void Invoke(ITabItem tab, TabAction action)
        {
            if (action == TabAction.ACTIVE) SelectedItem = tab;
            Post(this, new MainTabControlEventArgs(action, tab));
        }

        public void Reset()
        {
            foreach (ITabItem tab in tabitems.ToArray())
            {
                CloseItem(tab);
                tab.Dispose();
            }
            tabitems.Clear();
            selecteditem = null;
        }

        public void Add(ITabItem tab)
        {
            tabitems.Add(tab);
            tab.TabContainer = new LayoutDocument();
            tab.TabContainer.Title = tab.TabHeader;
            tab.PropertyChanged += OnTabPropertyChanged;
        }

        public void Remove(ITabItem tab)
        {
            CloseItem(tab);
            tabitems.Remove(tab);
            //tab.TabContainer = null;
            tab.PropertyChanged -= OnTabPropertyChanged;
        }
        
        public void ShowItem(ITabItem tab)
        {
            if (tab.IsFloat)
            {
                tab.FloatControl.Focus();
                return;
            }
            if (!tab.IsTab)
            {
                tab.TabContainer.Content = tab;
                Children.Add(tab.TabContainer);
            }
            int id = Children.IndexOf(tab.TabContainer);
            SelectedContentIndex = id;
            SelectedItem = tab;
        }
        
        public void CloseItem(ITabItem tab)
        {
            if (tab.IsFloat)
            {
                tab.FloatControl.Close();
                return;
            }
            if (tab.IsTab) Children.Remove(tab.TabContainer);
        }

        public void CloseItem(IModel model)
        {
            ITabItem tab = ToTabItem(model);
            if (tab != null) CloseItem(tab);
        }

        #endregion

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };
        
        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
        }
        
        private void OnTabPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITabItem tab = (ITabItem)sender;
            switch (e.PropertyName)
            {
                case "TabHeader": tab.TabContainer.Title = tab.TabHeader; break;
            }
        }

        #endregion
    }

    public class MainTabControlEventArgs : IWindowEventArgs
    {
        public MainTabControlEventArgs(TabAction _action, ITabItem _tab)
        {
            action = _action;
            tab = _tab;
        }

        private TabAction action;
        public TabAction Action { get { return this.action; } }
        private ITabItem tab;
        public object Tab { get { return this.tab; } }

        int IWindowEventArgs.Flags { get { return (int)action; } }
        object IWindowEventArgs.RelativeObject { get { return tab; } }
        object IWindowEventArgs.TargetedObject { get { return null; } }
    }
}
