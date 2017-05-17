using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class CommandManager
    {
        private LadderDiagramViewModel ldvmodel;
        public CommandManager() { }
        public CommandManager(LadderDiagramViewModel LDVmodel)
        {
            ldvmodel = LDVmodel;
        }
        private bool ismodify;
        public bool IsModify
        {
            get { return this.ismodify; }
            set { this.ismodify = value; }
        }

        public Stack<IUndoableCommand> UndoStack = new Stack<IUndoableCommand>();
        public Stack<IUndoableCommand> RedoStack = new Stack<IUndoableCommand>();

        public bool CanRedo
        {
            get { return RedoStack.Count > 0; }
        }
        
        public bool CanUndo
        {
            get { return UndoStack.Count > 0; }
        }

        public void Execute(IUndoableCommand command)
        {
            command.Execute();
            InvokeLDNetwordsChangedEvent(command);
            UndoStack.Push(command);
            RedoStack.Clear();
            IsModify = true;
        }

        public void Undo()
        {
            if(CanUndo)
            {
                var command = UndoStack.Pop();
                RedoStack.Push(command);
                command.Undo();
                InvokeLDNetwordsChangedEvent(command);
                IsModify = true;
            }
        }

        public void Redo()
        {
            if(CanRedo)
            {
                var command = RedoStack.Pop();
                UndoStack.Push(command);
                command.Redo();
                InvokeLDNetwordsChangedEvent(command);
                IsModify = true;
            }
        }
        private void InvokeLDNetwordsChangedEvent(IUndoableCommand command)
        {
            if (command is LadderDiagramReplaceNetworksCommand 
             || command is LadderDiagramRemoveNetworksCommand
             || command is LadderDiagramExchangeNetworkCommand)
            {
                ldvmodel.InvokeLDNetwordsEvent();
            }
        }
    }

}
