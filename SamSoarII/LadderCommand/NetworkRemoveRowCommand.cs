using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkRemoveRowCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;

        private int _rowNumber;

        private HashSet<BaseViewModel> _removedElements;

        private HashSet<VerticalLineViewModel> _removedVerticalLines;

        private int _oldRowCount;

        public NetworkRemoveRowCommand(LadderNetworkViewModel network, int rowNumber)
        {
            _network = network;
            _rowNumber = rowNumber;
        }

        public void Execute()
        {
            _removedElements = new HashSet<BaseViewModel>(_network.GetElements().Where(e => e.Y == _rowNumber));
            _removedVerticalLines = new HashSet<VerticalLineViewModel>(_network.GetVerticalLines().Where(e => e.Y == _rowNumber));
            if(_rowNumber == _network.RowCount - 1)
            {
                _removedVerticalLines = new HashSet<VerticalLineViewModel>(_network.GetVerticalLines().Where(e => e.Y == _rowNumber - 1));
            }
            else
            {
                _removedVerticalLines = new HashSet<VerticalLineViewModel>(_network.GetVerticalLines().Where(e => e.Y == _rowNumber));
            }
            Redo();
        }

        public void Redo()
        {
            foreach (var ele in _removedElements)
            {
                _network.RemoveElement(ele);
            }
            foreach (var vline in _removedVerticalLines)
            {
                _network.RemoveVerticalLine(vline);
            }

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
            _oldRowCount = _network.RowCount;
            _network.RowCount--;
            _network.INVModel.Setup(_network);
        }

        public void Undo()
        {
            _network.RowCount = _oldRowCount;
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

            foreach (var ele in _removedElements)
            {
                _network.ReplaceElement(ele);
            }
            foreach (var vline in _removedVerticalLines)
            {
                _network.ReplaceVerticalLine(vline);
            }
            _network.INVModel.Setup(_network);
        }
    }
}
