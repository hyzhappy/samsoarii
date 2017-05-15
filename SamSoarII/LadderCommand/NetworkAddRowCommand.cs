using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkAddRowCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;

        private int _rowNumber;

        private int _oldRowCount;

        public NetworkAddRowCommand(LadderNetworkViewModel network, int rowNumber)
        {
            _network = network;
            _rowNumber = rowNumber;
        }

        public void Execute()
        {
            // ToList确保在遍历时可以修改 
            _oldRowCount = _network.RowCount;
            _network.RowCount++;
            var movedElements = _network.GetElements().Where(e => e.Y >= _rowNumber).ToList();
            var movedVLines = _network.GetVerticalLines().Where(e => e.Y >= _rowNumber).ToList();
            foreach (var ele in movedElements)
            {
                _network.RemoveElement(ele);
                ele.Y++;
                _network.ReplaceElement(ele);
            }
            foreach (var vline in movedVLines)
            {
                _network.RemoveVerticalLine(vline);
                vline.Y++;
                _network.ReplaceVerticalLine(vline);
            }
            _network.INVModel.Setup(_network);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            _network.RowCount = _oldRowCount;
            // ToList确保在遍历时可以修改 
            var movedElements = _network.GetElements().Where(e => e.Y > _rowNumber).ToList();
            var movedVLines = _network.GetVerticalLines().Where(e => e.Y > _rowNumber).ToList();
            foreach (var ele in movedElements)
            {
                _network.RemoveElement(ele);
                ele.Y--;
                _network.ReplaceElement(ele);
            }
            foreach (var vline in movedVLines)
            {
                _network.RemoveVerticalLine(vline);
                vline.Y--;
                _network.ReplaceVerticalLine(vline);
            }
            _network.INVModel.Setup(_network);
        }
    }
}
