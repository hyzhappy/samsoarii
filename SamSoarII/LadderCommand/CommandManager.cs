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
                MessageBox.Show("当前模式不能对梯形图进行修改，请先切换到编辑模式！");
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
            RedoStack.Clear();
            IsModify = true;
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = UndoStack.First();
                if (!AssertEdit(command)) return;
                UndoStack.Pop();
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
                var command = RedoStack.First();
                if (!AssertEdit(command)) return;
                RedoStack.Pop();
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
             || command is LadderDiagramExchangeNetworkCommand)
            {
                ldvmodel.InvokeLDNetworksChanged();
            }
        }
    }

}
