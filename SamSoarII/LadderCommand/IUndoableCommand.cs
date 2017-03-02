using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public interface IUndoableCommand
    {
        void Execute();
        void Undo();
        void Redo();
    }
}
