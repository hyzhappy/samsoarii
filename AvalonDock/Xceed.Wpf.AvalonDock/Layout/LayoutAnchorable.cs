﻿/************************************************************************

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
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Globalization;
using System.ComponentModel;
using Xceed.Wpf.AvalonDock.Global;
using Xceed.Wpf.AvalonDock.Controls;

namespace Xceed.Wpf.AvalonDock.Layout
{
    [Serializable]
    public class LayoutAnchorable : LayoutContent
    {
        public LayoutAnchorControl AnchorControl
        {
            get; set;
        } 

        #region IsVisible
        [XmlIgnore]
        public bool IsVisible
        {
            get
            {
                return Parent != null && !(Parent is LayoutRoot);
            }
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        public event EventHandler IsVisibleChanged;

        void NotifyIsVisibleChanged()
        {
            if (IsVisibleChanged != null)
                IsVisibleChanged(this, EventArgs.Empty);
        }

        [XmlIgnore]
        public bool IsHidden
        {
            get
            {
                return (Parent is LayoutRoot);
            }
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            UpdateParentVisibility();
            RaisePropertyChanged("IsVisible");
            NotifyIsVisibleChanged();
            RaisePropertyChanged("IsHidden");
            RaisePropertyChanged("IsAutoHidden");
            base.OnParentChanged(oldValue, newValue);
        }

        void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            if (parentPane != null)
                parentPane.ComputeVisibility();
        }

        #endregion

        #region AutoHideWidth

        private double _autohideWidth = 0.0;
        public double AutoHideWidth
        {
            get { return _autohideWidth; }
            set
            {
                if (_autohideWidth != value)
                {
                    RaisePropertyChanging("AutoHideWidth");
                    value = Math.Max(value, _autohideMinWidth);
                    _autohideWidth = value;
                    RaisePropertyChanged("AutoHideWidth");
                }
            }
        }

        #endregion

        #region AutoHideMinWidth

        private double _autohideMinWidth = 100.0;
        public double AutoHideMinWidth
        {
            get { return _autohideMinWidth; }
            set
            {
                if (_autohideMinWidth != value)
                {
                    RaisePropertyChanging("AutoHideMinWidth");
                    if (value < 0)
                        throw new ArgumentException("value");
                    _autohideMinWidth = value;
                    RaisePropertyChanged("AutoHideMinWidth");
                }
            }
        }

        #endregion

        #region AutoHideHeight

        private double _autohideHeight = 0.0;
        public double AutoHideHeight
        {
            get { return _autohideHeight; }
            set
            {
                if (_autohideHeight != value)
                {
                    RaisePropertyChanging("AutoHideHeight");
                    value = Math.Max(value, _autohideMinHeight);
                    _autohideHeight = value;
                    RaisePropertyChanged("AutoHideHeight");
                }
            }
        }

        #endregion

        #region AutoHideMinHeight

        private double _autohideMinHeight = 100.0;
        public double AutoHideMinHeight
        {
            get { return _autohideMinHeight; }
            set
            {
                if (_autohideMinHeight != value)
                {
                    RaisePropertyChanging("AutoHideMinHeight");
                    if (value < 0)
                        throw new ArgumentException("value");
                    _autohideMinHeight = value;
                    RaisePropertyChanged("AutoHideMinHeight");
                }
            }
        }

        #endregion

        /// <summary>
        /// Hide this contents
        /// </summary>
        /// <remarks>Add this content to <see cref="ILayoutRoot.Hidden"/> collection of parent root.</remarks>
        public void Hide(bool cancelable = true)
        {
            if (!IsVisible)
            {
                IsSelected = true;
                IsActive = true;
                return;
            }

            if (cancelable)
            {
                CancelEventArgs args = new CancelEventArgs();
                OnHiding(args);
                if (args.Cancel)
                    return;
            }

            RaisePropertyChanging("IsHidden");
            RaisePropertyChanging("IsVisible");
            //if (Parent is ILayoutPane)
            {
                var parentAsGroup = Parent as ILayoutGroup;
                PreviousContainer = parentAsGroup;
                PreviousContainerIndex = parentAsGroup.IndexOfChild(this);
            }
            Root.Hidden.Add(this);
            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
            NotifyIsVisibleChanged();
        }

        public event EventHandler<CancelEventArgs> Hiding;

        protected virtual void OnHiding(CancelEventArgs args)
        {
            if (Hiding != null)
                Hiding(this, args);
        }


        /// <summary>
        /// Show the content
        /// </summary>
        /// <remarks>Try to show the content where it was previously hidden.</remarks>
        public void Show()
        {
            if (IsVisible)
                return;

            if (!IsHidden)
                throw new InvalidOperationException();

            RaisePropertyChanging("IsHidden");
            RaisePropertyChanging("IsVisible");

            bool added = false;
            var root = Root;
            if (root != null && root.Manager != null)
            {
                if (root.Manager.LayoutUpdateStrategy != null)
                    added = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root as LayoutRoot, this, PreviousContainer);
            }

            if (!added && PreviousContainer != null)
            {
                var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;
                if (PreviousContainerIndex < previousContainerAsLayoutGroup.ChildrenCount)
                    previousContainerAsLayoutGroup.InsertChildAt(PreviousContainerIndex, this);
                else
                    previousContainerAsLayoutGroup.InsertChildAt(previousContainerAsLayoutGroup.ChildrenCount, this);
                IsSelected = true;
                IsActive = true;
            }

            if (root != null && root.Manager != null)
            {
                if (root.Manager.LayoutUpdateStrategy != null)
                {
                    root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root as LayoutRoot, this);
                }
            }

            PreviousContainer = null;
            PreviousContainerIndex = -1;

            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
            NotifyIsVisibleChanged();
        }

        protected override void InternalDock()
        {
            var root = Root as LayoutRoot;
            LayoutAnchorablePane anchorablePane = null;

            if (root.ActiveContent != null &&
                root.ActiveContent != this)
            {
                //look for active content parent pane
                anchorablePane = root.ActiveContent.Parent as LayoutAnchorablePane;
            }

            if (anchorablePane == null)
            {
                //look for a pane on the right side
                anchorablePane = root.Descendents().OfType<LayoutAnchorablePane>().Where(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right).FirstOrDefault();
            }

            if (anchorablePane == null)
            {
                //look for an available pane
                anchorablePane = root.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
            }


            bool added = false;
            if (root.Manager.LayoutUpdateStrategy != null)
            {
                added = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root, this, anchorablePane);
            }

            if (!added)
            {
                if (anchorablePane == null)
                {
                    var mainLayoutPanel = new LayoutPanel() { Orientation = Orientation.Horizontal };
                    if (root.RootPanel != null)
                    {
                        mainLayoutPanel.Children.Add(root.RootPanel);
                    }

                    root.RootPanel = mainLayoutPanel;
                    anchorablePane = new LayoutAnchorablePane() { DockWidth = new GridLength(200.0, GridUnitType.Pixel) };
                    mainLayoutPanel.Children.Add(anchorablePane);
                }

                anchorablePane.Children.Add(this);
                added = true;
            }

            if (root.Manager.LayoutUpdateStrategy != null)
            {
                root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root, this);
            }

            base.InternalDock();
        }

        /// <summary>
        /// Add the anchorable to a DockingManager layout
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="strategy"></param>
        public void AddToLayout(DockingManager manager, AnchorableShowStrategy strategy)
        {
            if (IsVisible ||
                IsHidden)
                throw new InvalidOperationException();


            bool most = (strategy & AnchorableShowStrategy.Most) == AnchorableShowStrategy.Most;
            bool left = (strategy & AnchorableShowStrategy.Left) == AnchorableShowStrategy.Left;
            bool right = (strategy & AnchorableShowStrategy.Right) == AnchorableShowStrategy.Right;
            bool top = (strategy & AnchorableShowStrategy.Top) == AnchorableShowStrategy.Top;
            bool bottom = (strategy & AnchorableShowStrategy.Bottom) == AnchorableShowStrategy.Bottom;

            if (!most)
            { 
                var side = AnchorSide.Left;
                if (left)
                    side = AnchorSide.Left;
                if (right)
                    side = AnchorSide.Right;
                if (top)
                    side = AnchorSide.Top;
                if (bottom)
                    side = AnchorSide.Bottom;

                var anchorablePane = manager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == side);
                if (anchorablePane != null)
                    anchorablePane.Children.Add(this);
                else
                    most = true;
            }


            if (most)
            {
                if (manager.Layout.RootPanel == null)
                    manager.Layout.RootPanel = new LayoutPanel() { Orientation = (left || right ? Orientation.Horizontal : Orientation.Vertical) };

                if (left || right)
                {
                    if (manager.Layout.RootPanel.Orientation == Orientation.Vertical &&
                        manager.Layout.RootPanel.ChildrenCount > 1)
                    {
                        manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
                    }

                    manager.Layout.RootPanel.Orientation = Orientation.Horizontal;

                    if (left)
                        manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
                    else
                        manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
                }
                else
                {
                    if (manager.Layout.RootPanel.Orientation == Orientation.Horizontal &&
                        manager.Layout.RootPanel.ChildrenCount > 1)
                    {
                        manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
                    }

                    manager.Layout.RootPanel.Orientation = Orientation.Vertical;

                    if (top)
                        manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
                    else
                        manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
                }

            }

            if (left)
            {
                LayoutSetting.AddDefaultSideAnchorable(Title, "LEFT");
            }
            if (right)
            {
                LayoutSetting.AddDefaultSideAnchorable(Title, "RIGHT");
            }
            if (top)
            {
                LayoutSetting.AddDefaultSideAnchorable(Title, "TOP");
            }
            if (bottom)
            {
                LayoutSetting.AddDefaultSideAnchorable(Title, "BOTTOM");
            }
        }


        /// <summary>
        /// Get a value indicating if the anchorable is anchored to a border in an autohide status
        /// </summary>
        public bool IsAutoHidden
        {
            get { return Parent != null && Parent is LayoutAnchorGroup; }
        }


        #region AutoHide
        public void ToggleAutoHide()
        {
            #region Anchorable is already auto hidden
            if (IsAutoHidden)
            {
                var parentGroup = Parent as LayoutAnchorGroup;
                var parentSide = parentGroup.Parent as LayoutAnchorSide;
                var previousContainer = ((ILayoutPreviousContainer)parentGroup).PreviousContainer as LayoutAnchorablePane;

                if (previousContainer == null)
                {
                    AnchorSide side = (parentGroup.Parent as LayoutAnchorSide).Side;
                    switch (side)
                    {
                        case AnchorSide.Right:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Add(previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Horizontal };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(oldRootPanel);
                                panel.Children.Add(previousContainer);
                            }
                            break;
                        case AnchorSide.Left:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Insert(0, previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Horizontal };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(previousContainer);
                                panel.Children.Add(oldRootPanel);
                            }
                            break;
                        case AnchorSide.Top:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Insert(0, previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Vertical };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(previousContainer);
                                panel.Children.Add(oldRootPanel);
                            }
                            break;
                        case AnchorSide.Bottom:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Add(previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Vertical };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(oldRootPanel);
                                panel.Children.Add(previousContainer);
                            }
                            break;
                    }
                    // Do something about initializing the docks of previousContainer
                    GridLength[] defaultdock = LayoutSetting.GetDefaultDockAnchorable(Title);
                    previousContainer.DockWidth = defaultdock[0];
                    previousContainer.DockHeight = defaultdock[1];
                }
                else
                { 
                    //I'm about to remove parentGroup, redirect any content (ie hidden contents) that point to it
                    //to previousContainer
                    LayoutRoot root = parentGroup.Root as LayoutRoot;
                    foreach (var cnt in root.Descendents().OfType<ILayoutPreviousContainer>().Where(c => c.PreviousContainer == parentGroup))
                    {
                        cnt.PreviousContainer = previousContainer;
                    }
                }


                foreach (var anchorableToToggle in parentGroup.Children.ToArray())
                {
                    previousContainer.Children.Add(anchorableToToggle);
                }

                parentSide.Children.Remove(parentGroup);
            }
            #endregion
            #region Anchorable is docked
            else if (Parent is LayoutAnchorablePane)
            {
                IsActive = false;

                var root = Root;
                var parentPane = Parent as LayoutAnchorablePane;

                var newAnchorGroup = new LayoutAnchorGroup();

                ((ILayoutPreviousContainer)newAnchorGroup).PreviousContainer = parentPane;

                foreach (var anchorableToImport in parentPane.Children.ToArray())
                    newAnchorGroup.Children.Add(anchorableToImport);

                //detect anchor side for the pane
                var anchorSide = parentPane.GetSide();

                switch (anchorSide)
                {
                    case AnchorSide.Right:
                        root.RightSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Left:
                        root.LeftSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Top:
                        root.TopSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Bottom:
                        root.BottomSide.Children.Add(newAnchorGroup);
                        break;
                }
            }
            #endregion
        }

        #endregion

        #region CanHide

        private bool _canHide = true;
        public bool CanHide
        {
            get { return _canHide; }
            set
            {
                if (_canHide != value)
                {
                    _canHide = value;
                    RaisePropertyChanged("CanHide");
                }
            }
        }

        #endregion

        #region CanAutoHide

        private bool _canAutoHide = true;
        public bool CanAutoHide
        {
            get { return _canAutoHide; }
            set
            {
                if (_canAutoHide != value)
                {
                    _canAutoHide = value;
                    RaisePropertyChanged("CanAutoHide");
                }
            }
        }

        #endregion


        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("CanHide"))
                CanHide = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CanAutoHide"))
                CanAutoHide = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("AutoHideWidth"))
                AutoHideWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("AutoHideHeight"))
                AutoHideHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("AutoHideMinWidth"))
                AutoHideMinWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            if (reader.MoveToAttribute("AutoHideMinHeight"))
                AutoHideMinHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);

            base.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!CanHide)
                writer.WriteAttributeString("CanHide", CanHide.ToString());
            if (!CanAutoHide)
                writer.WriteAttributeString("CanAutoHide", CanAutoHide.ToString(CultureInfo.InvariantCulture));
            if (AutoHideWidth > 0)
                writer.WriteAttributeString("AutoHideWidth", AutoHideWidth.ToString(CultureInfo.InvariantCulture));
            if (AutoHideHeight > 0)
                writer.WriteAttributeString("AutoHideHeight", AutoHideHeight.ToString(CultureInfo.InvariantCulture));
            if (AutoHideMinWidth != 25.0)
                writer.WriteAttributeString("AutoHideMinWidth", AutoHideMinWidth.ToString(CultureInfo.InvariantCulture));
            if (AutoHideMinHeight != 25.0)
                writer.WriteAttributeString("AutoHideMinHeight", AutoHideMinHeight.ToString(CultureInfo.InvariantCulture));


            base.WriteXml(writer);
        }


#if TRACE
        public override void ConsoleDump(int tab)
        {
          System.Diagnostics.Trace.Write( new string( ' ', tab * 4 ) );
          System.Diagnostics.Trace.WriteLine( "Anchorable()" );
        }
#endif
    }
}
