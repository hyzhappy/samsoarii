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
    public class ViewThreadManager : BaseThreadManager
    {
        public ViewThreadManager(InteractionFacade _parent) : base(true, true)
        {
            parent = _parent;
            Aborted += OnAborted;
        }
        
        #region Number

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }

        #endregion

        private double oldscrolloffset;
        protected override void Handle()
        {
            if (parent.CurrentLadder == null)
            {
                oldscrolloffset = 0;
                return;
            }
            LadderDiagramViewModel current = parent.CurrentLadder;
            double newscrolloffset = current.Scroll.VerticalOffset;
            if (newscrolloffset > oldscrolloffset)
            {
                for (int i = 0; i < current.Core.Children.Count; i++)
                {
                    current.Core.Children[i].View.DynamicUpdate();
                    current.Core.Children[i].Inst.View.DynamicUpdate();
                }
            }
            else
            {
                for (int i = current.Core.Children.Count - 1; i >= 0; i--)
                {
                    current.Core.Children[i].View.DynamicUpdate();
                    current.Core.Children[i].Inst.View.DynamicUpdate();
                }
            }
            oldscrolloffset = newscrolloffset;
        }
        
        private void OnAborted(object sender, RoutedEventArgs e)
        {
        }
    }
}
