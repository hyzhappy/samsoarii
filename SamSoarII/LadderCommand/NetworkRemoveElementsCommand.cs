using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkRemoveElementsCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;
        private HashSet<BaseViewModel> _elements;
        private HashSet<VerticalLineViewModel> _vlines;

        public NetworkRemoveElementsCommand(LadderNetworkViewModel network, IEnumerable<BaseViewModel> elements, IEnumerable<VerticalLineViewModel> vlines)
        {
            _network = network;
            _elements = new HashSet<BaseViewModel>(elements);
            _vlines = new HashSet<VerticalLineViewModel>(vlines);
        }

        public NetworkRemoveElementsCommand(LadderNetworkViewModel network, IEnumerable<BaseViewModel> elements)
        {
            _network = network;
            _elements = new HashSet<BaseViewModel>(elements);
            _vlines = new HashSet<VerticalLineViewModel>();
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
            _network.INVModel.Setup(_network);
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
            _network.INVModel.Setup(_network);
        }
    }
}
