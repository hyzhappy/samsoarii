using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class LadderDiagramRemoveElementsCommand:IUndoableCommand
    {
        private List<NetworkRemoveElementsCommand> _commands;
        public LadderDiagramRemoveElementsCommand()
        {
            _commands = new List<NetworkRemoveElementsCommand>();
        }
        public void AddCommand(NetworkRemoveElementsCommand command)
        {
            _commands.Add(command);
        }
        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Redo()
        {
            foreach (var command in _commands)
            {
                command.Redo();
            }
        }

        public void Undo()
        {
            foreach (var command in _commands)
            {
                command.Undo();
            }
        }
    }
}
