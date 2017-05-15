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
        public static RoutedUICommand AddNewModbusCommand { get; set; }
        public static RoutedUICommand InsertRowCommand { get; set; }
        public static RoutedUICommand DeleteRowCommand { get; set; }
        public static RoutedUICommand ZoomInCommand { get; set; }
        public static RoutedUICommand ZoomOutCommand { get; set; }
        public static RoutedUICommand ShowProjectTreeViewCommand { get; set; }
        public static RoutedUICommand ShowSimulateTreeViewCommand { get; set; }
        public static RoutedUICommand ShowOutputCommand { get; set; }
        public static RoutedUICommand ShowCommunicationSettingDialogCommand { get; set; }
        public static RoutedUICommand CompileCommand { get; set; }
        public static RoutedUICommand MonitorCommand { get; set; }
        public static RoutedUICommand DownloadCommand { get; set; }
        public static RoutedUICommand UploadCommand { get; set; }
        public static RoutedUICommand ShowSimuMonitorCommand { get; set; }
        public static RoutedUICommand ShowMainMonitorCommand { get; set; }
        public static RoutedUICommand ShowErrorListCommand { get; set; }
        public static RoutedUICommand LadderModeToggleCommand { get; set; }
        public static RoutedUICommand InstModeToggleCommand { get; set; }
        public static RoutedUICommand CommentModeToggleCommand { get; set; }
        public static RoutedUICommand ShowPropertyDialogCommand { get; set; }
        public static RoutedUICommand InstShortCutOpenCommand { get; set; }
        public static RoutedUICommand ShowOptionDialogCommand { get; set; }
        public static RoutedUICommand CheckNetworkErrorCommand { get; set; }
        public static RoutedUICommand CheckFuncBlockCommand { get; set; }
        public static RoutedUICommand CloseProjectCommand { get; set; }
        public static RoutedUICommand SimulateCommand { get; set; }
        public static RoutedUICommand SimuStartCommand { get; set; }
        public static RoutedUICommand SimuPauseCommand { get; set; }
        public static RoutedUICommand SimuStopCommand { get; set; }
        public static RoutedUICommand EleListOpenCommand { get; set; }
        public static RoutedUICommand EleInitializeCommand { get; set; }
        static GlobalCommand()
        {
            AddNewFuncBlockCommand = new RoutedUICommand();
            AddNewSubRoutineCommand = new RoutedUICommand();
            AddNewModbusCommand = new RoutedUICommand();
            InsertRowCommand = new RoutedUICommand();
            DeleteRowCommand = new RoutedUICommand();
            ZoomInCommand = new RoutedUICommand();
            ZoomOutCommand = new RoutedUICommand();
            ShowProjectTreeViewCommand = new RoutedUICommand();
            ShowSimulateTreeViewCommand = new RoutedUICommand();
            ShowCommunicationSettingDialogCommand = new RoutedUICommand();
            ShowSimuMonitorCommand = new RoutedUICommand();
            ShowMainMonitorCommand = new RoutedUICommand();
            ShowOutputCommand = new RoutedUICommand();
            ShowErrorListCommand = new RoutedUICommand();
            CompileCommand = new RoutedUICommand();
            MonitorCommand = new RoutedUICommand();
            DownloadCommand = new RoutedUICommand();
            UploadCommand = new RoutedUICommand();
            ShowPropertyDialogCommand = new RoutedUICommand();
            InstShortCutOpenCommand = new RoutedUICommand();
            ShowOptionDialogCommand = new RoutedUICommand();
            LadderModeToggleCommand = new RoutedUICommand();
            InstModeToggleCommand = new RoutedUICommand();
            CommentModeToggleCommand = new RoutedUICommand();
            CheckNetworkErrorCommand = new RoutedUICommand();
            CheckFuncBlockCommand = new RoutedUICommand();
            CloseProjectCommand = new RoutedUICommand();
            SimulateCommand = new RoutedUICommand();
            SimuStartCommand = new RoutedUICommand();
            SimuStopCommand = new RoutedUICommand();
            SimuPauseCommand = new RoutedUICommand();
            EleListOpenCommand = new RoutedUICommand();
            EleInitializeCommand = new RoutedUICommand();
        }
    }
}
