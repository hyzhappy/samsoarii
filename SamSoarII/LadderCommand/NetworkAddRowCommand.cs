using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.AppMain.UI;

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
            var movedElements = _network.GetElements().Where(e => e.Y >= _rowNumber).ToList().OrderBy(x => { return x.Y; }).Reverse();
            var movedVLines = _network.GetVerticalLines().Where(e => e.Y >= _rowNumber).ToList().OrderBy(x => { return x.Y; }).Reverse();
            foreach (var ele in movedElements)
            {
                _network.RemoveEle(ele.X,ele.Y);
                ele.Y++;
                _network.ReplaceEle(ele);
            }
            foreach (var vline in movedVLines)
            {
                _network.RemoveVLine(vline.X,vline.Y);
                vline.Y++;
                _network.ReplaceVLine(vline);
            }
            //_network.INVModel.Setup(_network);
            // 将梯形图光标移到新生成的行的头部
            _network.AcquireSelectRect();
            LadderDiagramViewModel ldvmodel = _network.LDVModel;
            ldvmodel.SelectionRect.X = 0;
            ldvmodel.SelectionRect.Y = _rowNumber;
            ldvmodel.ProjectModel.IFacade.NavigateToNetwork(
                new NavigateToNetworkEventArgs(
                    _network.NetworkNumber,
                    ldvmodel.ProgramName,
                    ldvmodel.SelectionRect.X,
                    ldvmodel.SelectionRect.Y));
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            // ToList确保在遍历时可以修改 
            var movedElements = _network.GetElements().Where(e => e.Y > _rowNumber).ToList().OrderBy(x => { return x.Y; });
            var movedVLines = _network.GetVerticalLines().Where(e => e.Y > _rowNumber).ToList().OrderBy(x => { return x.Y; });
            foreach (var ele in movedElements)
            {
                _network.RemoveEle(ele.X, ele.Y);
                ele.Y--;
                _network.ReplaceEle(ele);
            }
            foreach (var vline in movedVLines)
            {
                _network.RemoveVLine(vline.X, vline.Y);
                vline.Y--;
                _network.ReplaceVLine(vline);
            }
            _network.RowCount = _oldRowCount;
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
    }
}
