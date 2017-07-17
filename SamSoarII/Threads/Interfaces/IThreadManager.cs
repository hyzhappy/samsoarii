using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Threads
{
    public interface IThreadManager
    {
        void Start();
        void Pause();
        void Abort();
        bool IsAlive { get; }
        bool IsActive { get; }
        event RoutedEventHandler Started;
        event RoutedEventHandler Paused;
        event RoutedEventHandler Aborted;
    }
}
