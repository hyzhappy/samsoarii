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
                    try
                    {
                        oldscrolloffset = current.Scroll.VerticalOffset;
                        oldinstoffset = current.Core.Inst.View.Scroll.VerticalOffset;
                        oldoutlineoffset = current.Outline.Scroll.VerticalOffset;
                        for (int i = 0; i < current.Core.Children.Count; i++)
                        {
                            current.Core.Children[i].View.DynamicUpdate();
                            current.Core.Children[i].Inst.View.DynamicUpdate();
                        }
                        current.Outline.DynamicUpdate();
                    }
                    catch (Exception)
                    {
                    }
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
                if (newscrolloffset - oldscrolloffset > 2.0)
                {
                    for (int i = 0; i < current.Core.Children.Count; i++)
                        current.Core.Children[i].View.DynamicUpdate();
                    oldscrolloffset = newscrolloffset;
                }
                else if (oldscrolloffset - newscrolloffset > 2.0)
                {
                    for (int i = current.Core.Children.Count - 1; i >= 0; i--)
                        current.Core.Children[i].View.DynamicUpdate();
                    oldscrolloffset = newscrolloffset;
                }
                double newinstoffset = current.Core.Inst.View.Scroll.VerticalOffset;
                if (newinstoffset - oldinstoffset > 2.0)
                {
                    for (int i = 0; i < current.Core.Children.Count; i++)
                        current.Core.Children[i].Inst.View.DynamicUpdate();
                    oldinstoffset = newinstoffset;
                }
                else if (oldinstoffset - newinstoffset > 2.0)
                {
                    for (int i = current.Core.Children.Count - 1; i >= 0; i--)
                        current.Core.Children[i].Inst.View.DynamicUpdate();
                    oldinstoffset = newinstoffset;
                }
                double newoutlineoffset = current.Outline.Scroll.VerticalOffset;
                if (Math.Abs(newoutlineoffset - oldoutlineoffset) > 2.0)
                {
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
                for (int i = 0; i < current.Core.Children.Count; i++)
                {
                    current.Core.Children[i].View.DynamicDispose();
                    current.Core.Children[i].Inst.View.DynamicDispose();
                }
            }
            catch (Exception)
            {
                DestoryCurrent();
            }
        }
        
        private void OnAborted(object sender, RoutedEventArgs e)
        {
        }
    }
}
