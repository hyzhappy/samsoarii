using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Command
{
    public interface IUndoableCommand
    {
        void Undo();
        void Redo();
    }
}
