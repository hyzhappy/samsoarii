using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
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
        private MainTabDiagramItem currenttab;
        private LadderDiagramViewModel current;
        protected override void Handle()
        {
            if (current != parent.CurrentLadder)
            {
                if (current != null)
                {
                    currenttab.FloatOpened -= OnCurrentFloatOpened;
                    if (!currenttab.IsFloat && parent.CurrentTabItem != null && !parent.CurrentTabItem.IsFloat)
                        DestoryCurrent();
                }
                currenttab = parent.CurrentTabItem;
                current = parent.CurrentLadder;
                if (current != null)
                {
                    currenttab.FloatOpened += OnCurrentFloatOpened;
                    current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (!current.Scroll.IsLoaded)
                            current.Loaded += OnCurrentLoaded;
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
            MainTabDiagramItem tab = (MainTabDiagramItem)sender;
            tab.FloatOpened -= OnCurrentFloatOpened;
            tab.FloatClosed += OnCurrentFloatClosed;
            LayoutContent content = parent.TCMain.SelectedContent;
            if (content != null && content.Content is MainTabDiagramItem)
            {
                tab = (MainTabDiagramItem)(content.Content);
                Generate(tab.LDVModel);
            }
        }

        private void OnCurrentFloatClosed(object sender, RoutedEventArgs e)
        {
            MainTabDiagramItem tab = (MainTabDiagramItem)sender;
            tab.FloatClosed -= OnCurrentFloatClosed;
            Destory(tab.LDVModel);
        }

        private void OnCurrentLoaded(object sender, RoutedEventArgs e)
        {
            current.Loaded -= OnCurrentLoaded;
            GenerateCurrent();
        }

        private void OnAborted(object sender, RoutedEventArgs e)
        {
        }
    }
}
