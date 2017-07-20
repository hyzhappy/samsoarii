using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.Shell.Windows
{
    public abstract class BaseTabItem : UserControl, ITabItem
    {
        public BaseTabItem()
        {

        }

        public BaseTabItem(MainTabControl _tabcontrol)
        {
            tabcontrol = _tabcontrol;
            tabcontrol.Add(this);
        }
        
        public virtual void Dispose()
        {
            PropertyChanged = delegate { };
            tabcontrol.Remove(this);
            tabcontrol = null;
            TabContainer.Content = null;
            TabContainer = null;
            FloatWindow = null;
            FloatControl = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void InvokePropertyChanged(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        #region Tab

        public bool IsTab { get { return tabcontrol.Children.Contains(tabcontainer); } }
        public abstract string TabHeader { get; }
        private LayoutDocument tabcontainer;
        public LayoutDocument TabContainer
        {
            get
            {
                return this.tabcontainer;
            }
            set
            {
                if (tabcontainer != null) tabcontainer.IsActiveChanged -= OnActiveChanged;
                this.tabcontainer = value;
                if (tabcontainer != null) tabcontainer.IsActiveChanged += OnActiveChanged;
            }
        }
        
        private MainTabControl tabcontrol;
        public MainTabControl TabControl { get { return this.tabcontrol; } }
        
        public void Invoke(TabAction action)
        {
            if (tabcontrol != null) tabcontrol.Invoke(this, action);
        }

        #endregion

        #region Float

        public bool IsFloat { get; set; }
        public LayoutFloatingWindow FloatWindow { get; set; }
        private LayoutFloatingWindowControl floatcontrol;
        public LayoutFloatingWindowControl FloatControl
        {
            get
            {
                return this.floatcontrol;
            }
            set
            {
                if (floatcontrol != null) floatcontrol.Closed -= OnFloatClosed;
                this.floatcontrol = value;
                if (floatcontrol != null) floatcontrol.Closed += OnFloatClosed;
            }
        }
        private void OnFloatClosed(object sender, EventArgs e)
        {
            IsFloat = false;
            FloatWindow = null;
            FloatControl = null;
            Invoke(TabAction.CLOSE);
        }

        #endregion

        #region Event Handler
        
        private void OnActiveChanged(object sender, EventArgs e)
        {
            if (tabcontainer.IsActive) Invoke(TabAction.ACTIVE);
        }

        #endregion

    }
}
