using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.AppMain.LadderCommand
{
    public class LadderNetworkMaskCommand : IUndoableCommand
    {
        private LadderNetworkViewModel network;
        public LadderNetworkMaskCommand(LadderNetworkViewModel _network)
        {
            network = _network;
        }
        public void Execute()
        {
            //to do nothing
        }

        public void Redo()
        {
            network.IsInvokeByCommand = true;
            network.IsMasked = !network.IsMasked;
            network.IsInvokeByCommand = false;
        }

        public void Undo()
        {
            Redo();
        }
    }
}
