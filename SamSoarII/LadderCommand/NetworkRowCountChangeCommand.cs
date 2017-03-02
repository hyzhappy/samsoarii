using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkRowCountChangeCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;

        private int _oldRowCount;
        private int _newRowCount;

        public NetworkRowCountChangeCommand(LadderNetworkViewModel network, int rowcount)
        {
            _newRowCount = rowcount;
            _network = network;
        }

        public void Execute()
        {
            _oldRowCount = _network.RowCount;
            _network.RowCount = _newRowCount;
        }

        public void Redo()
        {
            _network.RowCount = _newRowCount;
        }

        public void Undo()
        {
            _network.RowCount = _oldRowCount;
        }
    }
}
