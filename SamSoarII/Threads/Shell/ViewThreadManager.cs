using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.Threads
{
    public class ViewThreadManager : TimerThreadManager
    {
        public ViewThreadManager(InteractionFacade _parent) : base(true, true)
        {
            parent = _parent;
            Aborted += OnAborted;
            oldscrolloffset = 0;
            current = null;
            TimeSpan = 50;
        }
        
        #region Number

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }

        #endregion

        private double oldscrolloffset;
        private double oldinstoffset;
        private double oldoutlineoffset;
        private BaseTabItem currenttab;
        private LadderDiagramViewModel current;
        protected override void Handle()
        {
            if (currenttab != parent.CurrentTabItem)
            {
                if (currenttab != null)
                    currenttab.FloatOpened -= OnCurrentFloatOpened;
                if (current != null)
                { 
                    currenttab.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (!currenttab.IsLoaded)
                            currenttab.Loaded -= OnCurrentLoaded;
                    });
                    if (!currenttab.IsFloat && parent.CurrentTabItem != null && !parent.CurrentTabItem.IsFloat)
                        DestoryCurrent();
                }
                currenttab = parent.CurrentTabItem;
                current = parent.CurrentLadder;
                if (currenttab != null)
                    currenttab.FloatOpened += OnCurrentFloatOpened;
                if (current != null)
                {
                    currenttab.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (!currenttab.IsLoaded)
                            currenttab.Loaded += OnCurrentLoaded;
                        else
                            GenerateCurrent();
                    });
                }
            }
            if (current == null)
            {
                oldscrolloffset = 0;
                oldoutlineoffset = 0;
                oldinstoffset = 0;
                return;
            }
            try
            {
                double newscrolloffset = current.Scroll.VerticalOffset;
                if (current.IsViewModified || Math.Abs(newscrolloffset - oldscrolloffset) > 5.0)
                {
                    current.IsViewModified = false;
                    current.DynamicUpdate();
                    oldscrolloffset = newscrolloffset;
                }

                double newinstoffset = current.Inst.Scroll.VerticalOffset;
                if (current.Inst.IsViewModified || Math.Abs(newinstoffset - oldinstoffset) > 5.0)
                {
                    current.Inst.IsViewModified = false;
                    current.Inst.DynamicUpdate();
                    oldinstoffset = newinstoffset;
                }

                double newoutlineoffset = current.Outline.Scroll.VerticalOffset;
                if (current.Outline.IsViewModified || Math.Abs(newoutlineoffset - oldoutlineoffset) > 2.0)
                {
                    current.Outline.IsViewModified = false;
                    current.Outline.DynamicUpdate();
                    oldoutlineoffset = newoutlineoffset;
                }
            }
            catch (Exception)
            {
            }
        }
        

        private void DestoryCurrent()
        {
            Destory(current);
        }

        private void Destory(LadderDiagramViewModel _current)
        {
            try
            {
                _current.Outline.DynamicDispose();
                _current.Inst.DynamicDispose();
                _current.DynamicDispose();
            }
            catch (Exception)
            {
            }
        }

        private void GenerateCurrent()
        {
            try
            {
                oldscrolloffset = current.Scroll.VerticalOffset;
                oldinstoffset = current.Core.Inst.View.Scroll.VerticalOffset;
                oldoutlineoffset = current.Outline.Scroll.VerticalOffset;
                Generate(current);
            }
            catch (Exception)
            {
            }
        }

        private void Generate(LadderDiagramViewModel _current)
        {
            try
            {
                _current.DynamicUpdate();
                _current.Inst.DynamicUpdate();
                _current.Outline.DynamicUpdate();
            }
            catch (Exception)
            {
            }
        }

        private void OnCurrentFloatOpened(object sender, RoutedEventArgs e)
        {
            BaseTabItem tab = (BaseTabItem)sender;
            tab.FloatOpened -= OnCurrentFloatOpened;
            tab.FloatClosed += OnCurrentFloatClosed;
            LayoutContent content = parent.TCMain.SelectedContent;
            if (content != null && content.Content is MainTabDiagramItem)
            {
                MainTabDiagramItem diatab = (MainTabDiagramItem)(content.Content);
                Generate(diatab.LDVModel);
            }
        }
        
        private void OnCurrentFloatClosed(object sender, RoutedEventArgs e)
        {
            BaseTabItem tab = (BaseTabItem)sender;
            tab.FloatClosed -= OnCurrentFloatClosed;
            if (tab is MainTabDiagramItem)
            {
                MainTabDiagramItem diatab = (MainTabDiagramItem)tab;
                Destory(diatab.LDVModel);
            }
        }

        private void OnCurrentLoaded(object sender, RoutedEventArgs e)
        {
            if (currenttab == null) return;
            currenttab.Loaded -= OnCurrentLoaded;
            GenerateCurrent();
        }

        private void OnAborted(object sender, RoutedEventArgs e)
        {
        }
    }
}
