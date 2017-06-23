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
    public class NetworkRemoveRowsCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;
        private int _startRow;
        private int _count;
        private int _oldRowCount;
        private HashSet<BaseViewModel> _removedElements;
        private HashSet<VerticalLineViewModel> _removedVerticalLines;
        private NetworkChangeElementArea _oldarea;
        public NetworkRemoveRowsCommand(LadderNetworkViewModel network,int startRow,int count)
        {
            _network = network;
            _startRow = startRow;
            _count = count;
            _oldarea = new NetworkChangeElementArea();
            _oldarea.SU_Select = SelectStatus.MultiSelected;
            _oldarea.SU_Cross = CrossNetworkState.NoCross;
            _oldarea.NetworkNumberStart = network.NetworkNumber;
            _oldarea.NetworkNumberEnd = _oldarea.NetworkNumberStart;
            _oldarea.X1 = 0;
            _oldarea.X2 = 11;
            _oldarea.Y1 = startRow;
            _oldarea.Y2 = startRow + count - 1;
        }
        public void Execute()
        {
            _removedElements = new HashSet<BaseViewModel>(_network.GetElements().Where(e => { return (e.Y >= _startRow) && (e.Y <= _startRow + _count - 1); }));
            _removedVerticalLines = new HashSet<VerticalLineViewModel>(_network.GetVerticalLines().Where(e => { return (e.Y >= _startRow) && (e.Y <= _startRow + _count - 1); }));
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
            var movedElements = _network.GetElements().Where(e => e.Y > _startRow + _count - 1).ToList().OrderBy(x => { return x.Y; });
            var movedVLines = _network.GetVerticalLines().Where(e => e.Y > _startRow + _count - 1).ToList().OrderBy(x => { return x.Y; });
            foreach (var ele in movedElements)
            {
                _network.RemoveEle(ele.X,ele.Y);
                ele.Y -= _count;
                _network.ReplaceEle(ele);
            }
            foreach (var vline in movedVLines)
            {
                _network.RemoveVLine(vline.X,vline.Y);
                vline.Y -= _count;
                _network.ReplaceVLine(vline);
            }
            _oldRowCount = _network.RowCount;
            _network.RowCount -= _count;
            _network.AcquireSelectRect();
            LadderDiagramViewModel ldvmodel = _network.LDVModel;
            ldvmodel.SelectionRect.X = 0;
            ldvmodel.SelectionRect.Y = (_startRow < _network.RowCount ? _startRow : _startRow - 1);
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
            var movedElements = _network.GetElements().Where(e => e.Y >= _startRow).ToList().OrderBy(x => { return x.Y; }).Reverse();
            var movedVLines = _network.GetVerticalLines().Where(e => e.Y >= _startRow).ToList().OrderBy(x => { return x.Y; }).Reverse();
            foreach (var ele in movedElements)
            {
                _network.RemoveEle(ele.X, ele.Y);
                ele.Y += _count;
                _network.ReplaceEle(ele);
            }
            foreach (var vline in movedVLines)
            {
                _network.RemoveVLine(vline.X, vline.Y);
                vline.Y += _count;
                _network.ReplaceVLine(vline);
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
