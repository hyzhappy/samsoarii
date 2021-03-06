﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SamSoarII.AppMain.Project.Helper
{
    public class VisualHost : FrameworkElement
    {
        Visual child;

        public VisualHost(Visual child)
        {
            if (child == null)
            {
                throw new ArgumentException("child");
            }
            else
            {
                this.child = child;
            }
            AddVisualChild(child);
        }

        protected override Visual GetVisualChild(int index)
        {
            return (index == 0) ? child : null;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }
    }
    public class StatusBarHepler
    {
        public static InteractionFacade IFacade;
        private static string _message;
        public static bool IsLoading { get; set; } = false;
        public static void UpdateMessageAsync(string message)
        {
            _message = message;
            ThreadPool.QueueUserWorkItem(ThreadStartingPoint);
        }
        private static void ThreadStartingPoint(object obj)
        {
            IFacade.MainWindow.Main_SB.Dispatcher.BeginInvoke(DispatcherPriority.Normal,(ThreadStart) delegate()
            {
                IFacade.MainWindow.SB_Message.Text = _message;
                if (IsLoading)
                {
                    IFacade.MainWindow.Main_SB.Background = LadderHelper.LoadingBrush;
                    IFacade.MainWindow.SB_FontColor = Brushes.White;
                }
            });
            Dispatcher.Run();
        }
    }
}
