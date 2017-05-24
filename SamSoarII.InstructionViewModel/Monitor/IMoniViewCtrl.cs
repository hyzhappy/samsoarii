using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.LadderInstViewModel.Monitor
{
    public interface IMoniViewCtrl
    {
        bool IsRunning { get; }
        event RoutedEventHandler Started;
        event RoutedEventHandler Aborted;
    }
}
