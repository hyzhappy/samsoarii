using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.AppMain.Command
{
    public static class MyAppCommands
    {
        public static RoutedUICommand NewSubRoutineCommand = new RoutedUICommand();
        public static RoutedUICommand NewFuncBlockCommand = new RoutedUICommand();
        static MyAppCommands()
        {

        }
    }
}
