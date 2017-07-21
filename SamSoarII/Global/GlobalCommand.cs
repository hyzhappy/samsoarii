using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.Global
{
    public static class GlobalCommand
    {
        public static HashSet<RoutedUICommand> commands;
        public static RoutedUICommand AddNewFuncBlockCommand { get; set; }
        public static RoutedUICommand AddNewSubRoutineCommand { get; set; }
        public static RoutedUICommand AddNewModbusCommand { get; set; }
        public static RoutedUICommand InsertRowCommand { get; set; }
        public static RoutedUICommand DeleteRowCommand { get; set; }
        public static RoutedUICommand ZoomInCommand { get; set; }
        public static RoutedUICommand ZoomOutCommand { get; set; }
        public static RoutedUICommand ShowProjectTreeViewCommand { get; set; }
        public static RoutedUICommand ShowCommunicationSettingDialogCommand { get; set; }
        public static RoutedUICommand CompileCommand { get; set; }
        public static RoutedUICommand MonitorCommand { get; set; }
        public static RoutedUICommand EditCommand { get; set; }
        public static RoutedUICommand DownloadCommand { get; set; }
        public static RoutedUICommand UploadCommand { get; set; }
        public static RoutedUICommand ShowMainMonitorCommand { get; set; }
        public static RoutedUICommand ShowErrorListCommand { get; set; }
        public static RoutedUICommand ShowElemListCommand { get; set; }
        public static RoutedUICommand ShowElemInitCommand { get; set; }
        public static RoutedUICommand ShowBreakpointCommand { get; set; }
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
        public static RoutedUICommand BrpoStepCommand { get; set; }
        public static RoutedUICommand BrpoCallCommand { get; set; }
        public static RoutedUICommand BrpoNowCommand { get; set; }
        public static RoutedUICommand BrpoOutCommand { get; set; }
        public static RoutedUICommand EleListOpenCommand { get; set; }
        public static RoutedUICommand EleInitializeCommand { get; set; }
        public static RoutedUICommand ChangeToChineseCommand { get; set; }
        public static RoutedUICommand ChangeToEnglishCommand { get; set; }
        public static RoutedUICommand ShowHelpDocumentCommand { get; set; }
        public static RoutedUICommand OnlineHelpCommand { get; set; }
        public static RoutedUICommand ShowAboutCommand { get; set; }
        
        static GlobalCommand()
        {
            commands = new HashSet<RoutedUICommand>();
            AddNewFuncBlockCommand = new RoutedUICommand();
            commands.Add(AddNewFuncBlockCommand);
            AddNewSubRoutineCommand = new RoutedUICommand();
            commands.Add(AddNewSubRoutineCommand);
            AddNewModbusCommand = new RoutedUICommand();
            commands.Add(AddNewModbusCommand);
            InsertRowCommand = new RoutedUICommand();
            commands.Add(InsertRowCommand);
            DeleteRowCommand = new RoutedUICommand();
            commands.Add(DeleteRowCommand);
            ZoomInCommand = new RoutedUICommand();
            commands.Add(ZoomInCommand);
            ZoomOutCommand = new RoutedUICommand();
            commands.Add(ZoomOutCommand);
            ShowProjectTreeViewCommand = new RoutedUICommand();
            commands.Add(ShowProjectTreeViewCommand);
            ShowCommunicationSettingDialogCommand = new RoutedUICommand();
            commands.Add(ShowCommunicationSettingDialogCommand);
            ShowMainMonitorCommand = new RoutedUICommand();
            commands.Add(ShowMainMonitorCommand);
            ShowErrorListCommand = new RoutedUICommand();
            commands.Add(ShowErrorListCommand);
            ShowElemListCommand = new RoutedUICommand();
            commands.Add(ShowElemListCommand);
            ShowElemInitCommand = new RoutedUICommand();
            commands.Add(ShowElemInitCommand);
            ShowBreakpointCommand = new RoutedUICommand();
            commands.Add(ShowBreakpointCommand);
            CompileCommand = new RoutedUICommand();
            commands.Add(CompileCommand);
            MonitorCommand = new RoutedUICommand();
            commands.Add(MonitorCommand);
            EditCommand = new RoutedUICommand();
            commands.Add(EditCommand);
            DownloadCommand = new RoutedUICommand();
            commands.Add(DownloadCommand);
            UploadCommand = new RoutedUICommand();
            commands.Add(UploadCommand);
            ShowPropertyDialogCommand = new RoutedUICommand();
            commands.Add(ShowPropertyDialogCommand);
            InstShortCutOpenCommand = new RoutedUICommand();
            commands.Add(InstShortCutOpenCommand);
            ShowOptionDialogCommand = new RoutedUICommand();
            commands.Add(ShowOptionDialogCommand);
            LadderModeToggleCommand = new RoutedUICommand();
            commands.Add(LadderModeToggleCommand);
            InstModeToggleCommand = new RoutedUICommand();
            commands.Add(InstModeToggleCommand);
            CommentModeToggleCommand = new RoutedUICommand();
            commands.Add(CommentModeToggleCommand);
            CheckNetworkErrorCommand = new RoutedUICommand();
            commands.Add(CheckNetworkErrorCommand);
            CheckFuncBlockCommand = new RoutedUICommand();
            commands.Add(CheckFuncBlockCommand);
            CloseProjectCommand = new RoutedUICommand();
            commands.Add(CloseProjectCommand);
            SimulateCommand = new RoutedUICommand();
            commands.Add(SimulateCommand);
            SimuStartCommand = new RoutedUICommand();
            commands.Add(SimuStartCommand);
            SimuStopCommand = new RoutedUICommand();
            commands.Add(SimuStopCommand);
            SimuPauseCommand = new RoutedUICommand();
            commands.Add(SimuPauseCommand);
            BrpoStepCommand = new RoutedUICommand();
            commands.Add(BrpoStepCommand);
            BrpoCallCommand = new RoutedUICommand();
            commands.Add(BrpoCallCommand);
            BrpoNowCommand = new RoutedUICommand();
            commands.Add(BrpoNowCommand);
            BrpoOutCommand = new RoutedUICommand();
            commands.Add(BrpoOutCommand);
            ChangeToChineseCommand = new RoutedUICommand();
            commands.Add(ChangeToChineseCommand);
            ChangeToEnglishCommand = new RoutedUICommand();
            commands.Add(ChangeToEnglishCommand);
            ShowHelpDocumentCommand = new RoutedUICommand();
            commands.Add(ShowHelpDocumentCommand);
            OnlineHelpCommand = new RoutedUICommand();
            commands.Add(OnlineHelpCommand);
            ShowAboutCommand = new RoutedUICommand();
            commands.Add(ShowAboutCommand);
        }
    }
}
