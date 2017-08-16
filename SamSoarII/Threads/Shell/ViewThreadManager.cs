using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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
            TimeSpan = 20;
        }
        
        #region Number

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }

        #endregion

        private double oldscrolloffset;
        private double oldinstoffset;
        private double oldoutlineoffset;
        private LadderDiagramViewModel current;
        protected override void Handle()
        {
            if (current != parent.CurrentLadder)
            {
                if (current != null) DestoryCurrent();
                current = parent.CurrentLadder;
                if (current != null)
                {
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
            try
            {
                current.Outline.DynamicDispose();
                current.Inst.DynamicDispose();
                current.DynamicDispose();
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
                current.DynamicUpdate();
                current.Inst.DynamicUpdate();
                current.Outline.DynamicUpdate();
            }
            catch (Exception)
            {
            }
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
