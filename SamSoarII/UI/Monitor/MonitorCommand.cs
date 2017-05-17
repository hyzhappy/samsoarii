using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.AppMain.UI.Monitor
{
    public class MonitorCommand
    {
        public static RoutedUICommand AddElementCommand { get; set; }
        public static RoutedUICommand QuickAddElementsCommand { get; set; }
        public static RoutedUICommand DeleteElementsCommand { get; set; }
        public static RoutedUICommand DeleteAllElementCommand { get; set; }
        public static RoutedUICommand StartCommand { get; set; }
        public static RoutedUICommand StopCommand { get; set; }
        static MonitorCommand()
        {
            AddElementCommand = new RoutedUICommand();
            QuickAddElementsCommand = new RoutedUICommand();
            DeleteElementsCommand = new RoutedUICommand();
            DeleteAllElementCommand = new RoutedUICommand();
            StartCommand = new RoutedUICommand();
            StopCommand = new RoutedUICommand();
        }
    }
}
