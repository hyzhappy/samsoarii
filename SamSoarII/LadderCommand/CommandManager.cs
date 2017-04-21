using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class CommandManager
    {
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
            UndoStack.Push(command);
            IsModify = true;
        }

        public void Undo()
        {
            if(CanUndo)
            {
                var command = UndoStack.Pop();
                RedoStack.Push(command);
                command.Undo();
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
                IsModify = true;
            }
        }
    }

}
