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

        public static RoutedUICommand CompileCommand { get; set; }

        public static RoutedUICommand DownloadCommand { get; set; }
        public static RoutedUICommand CommentModeToggleCommand { get; set; }
        public static RoutedUICommand ShowPropertyDialogCommand { get; set; }
        public static RoutedUICommand ShowOptionDialogCommand { get; set; }
        public static RoutedUICommand CheckNetworkErrorCommand { get; set; }
        public static RoutedUICommand CloseProjectCommand { get; set; }
        public static RoutedUICommand SimulateCommand { get; set; }
        static GlobalCommand()
        {
            AddNewFuncBlockCommand = new RoutedUICommand();
            AddNewSubRoutineCommand = new RoutedUICommand();
            ZoomInCommand = new RoutedUICommand();
            ZoomOutCommand = new RoutedUICommand();
            CompileCommand = new RoutedUICommand();
            DownloadCommand = new RoutedUICommand();
            ShowPropertyDialogCommand = new RoutedUICommand();
            ShowOptionDialogCommand = new RoutedUICommand();
            CommentModeToggleCommand = new RoutedUICommand();
            CheckNetworkErrorCommand = new RoutedUICommand();
            CloseProjectCommand = new RoutedUICommand();
            SimulateCommand = new RoutedUICommand();
        }
    }
}
