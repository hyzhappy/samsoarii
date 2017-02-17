using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Command
{
    public class LadderDiagramCommandManager
    {
        public Stack<IUndoableCommand> UndoCommandStack = new Stack<IUndoableCommand>();
        public Stack<IUndoableCommand> RedoCommandStack = new Stack<IUndoableCommand>();

    }
}
