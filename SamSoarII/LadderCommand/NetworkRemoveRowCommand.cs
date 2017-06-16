using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
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
        private NetworkChangeElementArea _oldarea;

        public NetworkRemoveRowCommand(LadderNetworkViewModel network, int rowNumber)
        {
            _network = network;
            _rowNumber = rowNumber;
            _oldarea = new NetworkChangeElementArea();
            _oldarea.SU_Select = SelectStatus.MultiSelected;
            _oldarea.SU_Cross = CrossNetworkState.NoCross;
            _oldarea.NetworkNumberStart = network.NetworkNumber;
            _oldarea.NetworkNumberEnd = _oldarea.NetworkNumberStart;
            _oldarea.X1 = 0;
            _oldarea.X2 = 11;
            _oldarea.Y1 = rowNumber;
            _oldarea.Y2 = _oldarea.Y1;
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
            //_network.INVModel.Setup(_network);
            // 将梯形图光标移到删除的行的位置
            _network.AcquireSelectRect();
            LadderDiagramViewModel ldvmodel = _network.LDVModel;
            ldvmodel.SelectionRect.X = 0;
            ldvmodel.SelectionRect.Y = (_rowNumber < _network.RowCount ? _rowNumber : _rowNumber - 1);
            ldvmodel.ProjectModel.IFacade.NavigateToNetwork(
                new NavigateToNetworkEventArgs(
                    _network.NetworkNumber,
                    ldvmodel.ProgramName,
                    ldvmodel.SelectionRect.X,
                    ldvmodel.SelectionRect.Y));
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
            if (_oldarea != null)
            {
                _oldarea.Select(_network);
            }
        }
    }
}
