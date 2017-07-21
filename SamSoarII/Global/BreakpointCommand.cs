using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SamSoarII.Global
{
    class BreakpointCommand
    {
        public static RoutedUICommand Add;
        public static RoutedUICommand Setting;
        public static RoutedUICommand Remove;
        public static RoutedUICommand JumpTo;

        static BreakpointCommand()
        {
            Add = new RoutedUICommand();
            Setting = new RoutedUICommand();
            Remove = new RoutedUICommand();
            JumpTo = new RoutedUICommand();
        }

    }
}
