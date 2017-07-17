using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SamSoarII.Threads
{
    public enum ViewThreadAction { DIAGRAM, FUNCBLOCK, INSTRUCTION }

    public class ViewThreadManager : BaseThreadManager
    {
        public ViewThreadManager(InteractionFacade _parent) : base(true)
        {
            parent = _parent;
            loads = new Stack<ILoadModel>();
            Aborted += OnAborted;
        }
        
        #region Number

        private InteractionFacade parent;
        public InteractionFacade Parent { get { return this.parent; } }

        private Stack<ILoadModel> loads;
        #endregion

        protected override void Handle()
        {
            Thread.Sleep(10);
            if (loads.Count() == 0) return;
            ILoadModel load = loads.Pop();
            Thread thread = new Thread(() =>
            {
                while (!load.IsFullLoaded)
                {
                    load.UpdateFullLoadProgress();
                    Thread.Sleep(50);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            load.FullLoad();
            thread.Abort();
            if (!load.IsFullLoaded)
            {
                loads.Push(load);
                return;
            }
            load.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                load.Update();
            });
            List<ILoadModel> children = load.LoadChildren.ToList();
            children.Reverse();
            foreach (ILoadModel sub in children)
                loads.Push(sub);
        }

        public void Add(ILoadModel load)
        {
            loads.Push(load);
        }
        
        private void OnAborted(object sender, RoutedEventArgs e)
        {
            loads.Clear();
        }
    }
}
