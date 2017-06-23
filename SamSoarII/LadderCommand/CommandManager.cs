using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.AppMain.LadderCommand
{
    public class CommandManager
    {
        private const int UNDO_LIMIT = 16;

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
                if (value == true)
                    LDVModel.ProjectModel.OnPropertyChanged("Ladder");
            }
        }

        public LinkedList<IUndoableCommand> UndoStack = new LinkedList<IUndoableCommand>();
        public LinkedList<IUndoableCommand> RedoStack = new LinkedList<IUndoableCommand>();

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
            UndoStack.AddFirst(command);
            if (UndoStack.Count() > UNDO_LIMIT)
                UndoStack.RemoveLast();
            RedoStack.Clear();
            IsModify = true;
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = UndoStack.First();
                if (!AssertEdit(command)) return;
                UndoStack.RemoveFirst();
                RedoStack.AddFirst(command);
                command.Undo();
                InvokeLDNetworksChangedEvent(command);
                IsModify = true;
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = RedoStack.First();
                if (!AssertEdit(command)) return;
                RedoStack.RemoveFirst();
                UndoStack.AddFirst(command);
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
    }

}
