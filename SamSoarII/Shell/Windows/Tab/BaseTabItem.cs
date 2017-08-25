﻿using System;
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
            floatcontrol = null;
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
        public InteractionFacade IFParent { get { return TabControl?.IFParent; } }
        
        public void Invoke(TabAction action)
        {
            if (tabcontrol != null) tabcontrol.Invoke(this, action);
        }

        #endregion

        #region Float

        public bool IsFloat { get { return floatcontrol != null; } }
        private LayoutFloatingWindowControl floatcontrol;
        public LayoutFloatingWindowControl FloatControl
        {
            get
            {
                return this.floatcontrol;
            }
            set
            {
                if (floatcontrol != null)
                {
                    floatcontrol.Closed -= OnFloatClosed;
                    FloatClosed(this, new RoutedEventArgs());
                    if (floatcontrol is LayoutDocumentFloatingWindowControl)
                    {
                        ((LayoutDocumentFloatingWindowControl)floatcontrol).FloatContent = null;
                        ViewThreadStart();
                    }
                }
                this.floatcontrol = value;
                if (floatcontrol != null)
                {
                    floatcontrol.Closed += OnFloatClosed;
                    FloatOpened(this, new RoutedEventArgs());
                    if (floatcontrol is LayoutDocumentFloatingWindowControl)
                        ((LayoutDocumentFloatingWindowControl)floatcontrol).FloatContent = this;
                }
            }
        }
        public event RoutedEventHandler FloatOpened = delegate { };
        public event RoutedEventHandler FloatClosed = delegate { };
        private void OnFloatClosed(object sender, EventArgs e)
        {
            FloatControl = null; 
            Invoke(TabAction.CLOSE);
        }

        public bool IsViewThreadActive
        {
            get
            {
                return IFParent.ThMNGView.IsActive;
            }
        }

        public void ViewThreadPause()
        {
            if (IsViewThreadActive)
                IFParent.ThMNGView.Paused += OnViewThreadPaused;
            IFParent.ThMNGView.Pause();
        }

        public void ViewThreadStart()
        {
            IFParent.ThMNGView.Start();
        }
        
        public event RoutedEventHandler ViewThreadPaused = delegate { };
        private void OnViewThreadPaused(object sender, RoutedEventArgs e)
        {
            IFParent.ThMNGView.Paused -= OnViewThreadPaused;
            ViewThreadPaused(this, new RoutedEventArgs());
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
