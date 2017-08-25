/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Windows.Shell;
using SamSoarII.Shell.Windows;
using System.Threading;
using System.Windows.Threading;

namespace Xceed.Wpf.AvalonDock.Controls
{
    public class LayoutDocumentFloatingWindowControl : LayoutFloatingWindowControl
    {
        static LayoutDocumentFloatingWindowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentFloatingWindowControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentFloatingWindowControl)));
        } 

        internal LayoutDocumentFloatingWindowControl(LayoutDocumentFloatingWindow model)
            :base(model)
        {
            _model = model;
        }


        LayoutDocumentFloatingWindow _model;

        public override ILayoutElement Model
        {
            get { return _model; }
        }

        public LayoutItem RootDocumentLayoutItem
        {
            get { return _model.Root.Manager.GetLayoutItemFromModel(_model.RootDocument); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_model.RootDocument == null)
            {
                InternalClose();
            }
            else
            {
                var manager = _model.Root.Manager;

                Content = manager.CreateUIElementForModel(_model.RootDocument);

                _model.RootDocumentChanged += new EventHandler(_model_RootDocumentChanged);
            }
        }

        void _model_RootDocumentChanged(object sender, EventArgs e)
        {
            if (_model.RootDocument == null)
            {
                InternalClose();
            }
        }

        private IFloat ifloat;
        public IFloat FloatContent
        {
            get { return this.ifloat; }
            set { this.ifloat = value; }
        }
        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WM_NCLBUTTONDOWN: //Left button down on title -> start dragging over docking manager
                    if (wParam.ToInt32() == Win32Helper.HT_CAPTION)
                    {
                        if (FloatContent != null)
                        {
                            FloatContent.ViewThreadPause();
                            if (FloatContent.IsViewThreadActive)
                            {
                                if (!iswaittingtoactive)
                                {
                                    iswaittingtoactive = true;
                                    FloatContent.ViewThreadPaused += OnViewThreadPausedToActive;
                                }
                                return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
                            }
                        }
                        if (_model.RootDocument != null)
                            _model.RootDocument.IsActive = true;
                    }
                    break;
                case Win32Helper.WM_NCRBUTTONUP:
                    if (wParam.ToInt32() == Win32Helper.HT_CAPTION)
                    {
                        if (OpenContextMenu())
                            handled = true;
                        if (_model.Root.Manager.ShowSystemMenu)
                            WindowChrome.GetWindowChrome(this).ShowSystemMenu = !handled;
                        else
                            WindowChrome.GetWindowChrome(this).ShowSystemMenu = false;
                        if (FloatContent != null) FloatContent.ViewThreadStart();
                    }
                    break;

            }
            return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
        }
        private bool iswaittingtoactive = false;
        private void OnViewThreadPausedToActive(object sender, RoutedEventArgs e)
        {
            IFloat ifloat = (IFloat)sender;
            ifloat.ViewThreadPaused -= OnViewThreadPausedToActive;
            iswaittingtoactive = false;
            if (_model.RootDocument != null)
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate () 
                {
                    _model.RootDocument.IsActive = true;
                }));
        }

        bool OpenContextMenu()
        {
            var ctxMenu = _model.Root.Manager.DocumentContextMenu;
            if (ctxMenu != null && RootDocumentLayoutItem != null)
            {
                ctxMenu.PlacementTarget = null;
                ctxMenu.Placement = PlacementMode.MousePoint;
                ctxMenu.DataContext = RootDocumentLayoutItem;
                ctxMenu.IsOpen = true;
                return true;
            }

            return false;
        }


        protected override void OnClosed(EventArgs e)
        {
            var root = Model.Root;
            root.Manager.RemoveFloatingWindow(this);
            root.CollectGarbage();

            base.OnClosed(e);

            if (!CloseInitiatedByUser)
            {
                root.FloatingWindows.Remove(_model);
            }

            _model.RootDocumentChanged -= new EventHandler(_model_RootDocumentChanged);
        }

    }
}
