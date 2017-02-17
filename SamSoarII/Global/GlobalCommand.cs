using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.AppMain
{
    public static class GlobalCommand
    {
        public static RoutedUICommand AddNewFuncBlockCommand { get; set; }
        public static RoutedUICommand AddNewSubRoutineCommand { get; set; }

        public static RoutedUICommand ZoomInCommand { get; set; }
        public static RoutedUICommand ZoomOutCommand { get; set; }
        static GlobalCommand()
        {
            AddNewFuncBlockCommand = new RoutedUICommand();
            AddNewSubRoutineCommand = new RoutedUICommand();
            ZoomInCommand = new RoutedUICommand();
            ZoomOutCommand = new RoutedUICommand();
        }
    }
}
