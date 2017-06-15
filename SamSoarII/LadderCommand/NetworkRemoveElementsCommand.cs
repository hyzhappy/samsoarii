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
    public class NetworkRemoveElementsCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;
        private HashSet<BaseViewModel> _elements;
        private HashSet<VerticalLineViewModel> _vlines;
        private NetworkChangeElementArea _area;

        public NetworkRemoveElementsCommand(
            LadderNetworkViewModel network, 
            IEnumerable<BaseViewModel> elements, IEnumerable<VerticalLineViewModel> vlines,
            NetworkChangeElementArea area = null)
        {
            _network = network;
            _elements = new HashSet<BaseViewModel>(elements);
            _vlines = new HashSet<VerticalLineViewModel>(vlines);
            _area = area;
        }

        public NetworkRemoveElementsCommand(
            LadderNetworkViewModel network, 
            IEnumerable<BaseViewModel> elements,
            NetworkChangeElementArea area = null)
        {
            _network = network;
            _elements = new HashSet<BaseViewModel>(elements);
            _vlines = new HashSet<VerticalLineViewModel>();
            _area = area;
        }

        public void Execute()
        {
            foreach(var ele in _elements)
            {
                _network.RemoveElement(ele);
            }
            foreach(var vline in _vlines)
            {
                _network.RemoveVerticalLine(vline);
            }
            //_network.INVModel.Setup(_network);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            foreach (var ele in _elements)
            {
                _network.ReplaceElement(ele);
            }
            foreach (var vline in _vlines)
            {
                _network.ReplaceVerticalLine(vline);
            }
            //_network.INVModel.Setup(_network);
            if (_elements.Count() + _vlines.Count() == 1)
            {
                // 将梯形图光标移到新生成的单个元件
                BaseViewModel bvmodel = _elements.Count() == 1
                    ? _elements.First() : _vlines.First();
                _network.AcquireSelectRect();
                LadderDiagramViewModel ldvmodel = _network.LDVModel;
                ldvmodel.SelectionRect.X = bvmodel.X;
                ldvmodel.SelectionRect.Y = bvmodel.Y;
                if (bvmodel is VerticalLineViewModel)
                    ldvmodel.SelectionRect.X++;
                ldvmodel.ProjectModel.IFacade.NavigateToNetwork(
                    new NavigateToNetworkEventArgs(
                        _network.NetworkNumber,
                        ldvmodel.ProgramName,
                        ldvmodel.SelectionRect.X,
                        ldvmodel.SelectionRect.Y));
            }
            else if (_area != null)
            {
                _area.Select(_network);
            }
        }
    }
}
