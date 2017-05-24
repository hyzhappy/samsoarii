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

        public void Execute(IUndoableCommand command)
        {
            if (ldvmodel.LadderMode != LadderMode.Edit)
            {
                MessageBox.Show("当前模式不能对梯形图进行修改，请先切换到编辑模式！");
                return;
            }
            command.Execute();
            InvokeLDNetworksChangedEvent(command);
            UndoStack.Push(command);
            RedoStack.Clear();
            IsModify = true;
        }

        public void Undo()
        {
            if (ldvmodel.LadderMode != LadderMode.Edit)
            {
                MessageBox.Show("当前模式不能对梯形图进行修改，请先切换到编辑模式！");
                return;
            }
            if (CanUndo)
            {
                var command = UndoStack.Pop();
                RedoStack.Push(command);
                command.Undo();
                InvokeLDNetworksChangedEvent(command);
                IsModify = true;
            }
        }

        public void Redo()
        {
            if (ldvmodel.LadderMode != LadderMode.Edit)
            {
                MessageBox.Show("当前模式不能对梯形图进行修改，请先切换到编辑模式！");
                return;
            }
            if (CanRedo)
            {
                var command = RedoStack.Pop();
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
