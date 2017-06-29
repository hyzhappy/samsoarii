using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SamSoarII.AppMain.LadderCommand
{
    public class CommandManager:IDisposable
    {
        private const int UNDO_LIMIT = 128;

        private LadderDiagramViewModel ldvmodel;
        public LadderDiagramViewModel LDVModel
        {
            get { return this.ldvmodel; }
            set { this.ldvmodel = value; }
        }
        public CommandManager() { }
        public CommandManager(LadderDiagramViewModel LDVmodel)
        {
            ldvmodel = LDVmodel;
        }
        private bool ismodify;
        public bool IsModify
        {
            get { return this.ismodify; }
            set
            {
                ismodify = value;
                if (value == true && LDVModel != null)
                    LDVModel.ProjectModel.OnPropertyChanged("Ladder");
            }
        }

        public Stack<IUndoableCommand> UndoStack = new Stack<IUndoableCommand>(UNDO_LIMIT);
        public Stack<IUndoableCommand> RedoStack = new Stack<IUndoableCommand>(UNDO_LIMIT);

        public bool CanRedo
        {
            get { return RedoStack.Count > 0; }
        }
        
        public bool CanUndo
        {
            get { return UndoStack.Count > 0; }
        }

        public void Initialize()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }

        public bool AssertEdit(IUndoableCommand command)
        {
            if (ldvmodel != null
             && ldvmodel.LadderMode != LadderMode.Edit
             && !(command is NetworkReplaceBreakpointCommand))
            {
                MessageBox.Show(Properties.Resources.Change_Mode);
                return false;
            }
            return true;
        }

        public void Execute(IUndoableCommand command)
        {
            if (!AssertEdit(command)) return;
            command.Execute();
            InvokeLDNetworksChangedEvent(command);
            UndoStack.Push(command);
            //if (UndoStack.Count() > UNDO_LIMIT)
            //    UndoStack.RemoveLast();
            RedoStack.Clear();
            IsModify = true;
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = UndoStack.Pop();
                if (!AssertEdit(command)) return;
                //UndoStack.RemoveFirst();
                RedoStack.Push(command);
                command.Undo();
                InvokeLDNetworksChangedEvent(command);
                IsModify = true;
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = RedoStack.Pop();
                if (!AssertEdit(command)) return;
                //RedoStack.RemoveFirst();
                UndoStack.Push(command);
                command.Redo();
                InvokeLDNetworksChangedEvent(command);
                IsModify = true;
            }
        }
        private void InvokeLDNetworksChangedEvent(IUndoableCommand command)
        {
            if (command is LadderDiagramReplaceNetworksCommand
             || command is LadderDiagramRemoveNetworksCommand
             || command is LadderDiagramExchangeNetworkCommand
             || command is LadderDiagramMoveNetworkCommand)
            {
                ldvmodel.InvokeLDNetworksChanged();
            }
        }

        public void Dispose()
        {
            UndoStack.Clear();
            RedoStack.Clear();
            LDVModel = null;
        }
    }

}
