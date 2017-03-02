using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkReplaceElementsCommand : IUndoableCommand
    {

        private LadderNetworkViewModel _network;
        private HashSet<BaseViewModel> _elements;
        private HashSet<VerticalLineViewModel> _vlines;
        private HashSet<BaseViewModel> _oldelements;
        private HashSet<VerticalLineViewModel> _oldvlines;

        public NetworkReplaceElementsCommand(LadderNetworkViewModel network, IEnumerable<BaseViewModel> elements, IEnumerable<VerticalLineViewModel> vlines, IEnumerable<BaseViewModel> oldelements, IEnumerable<VerticalLineViewModel> oldvlines)
        {
            _network = network;
            _elements = new HashSet<BaseViewModel>(elements);
            _vlines = new HashSet<VerticalLineViewModel>(vlines);
            _oldelements = new HashSet<BaseViewModel>(oldelements);
            _oldvlines = new HashSet<VerticalLineViewModel>(oldvlines);
        }

        public NetworkReplaceElementsCommand(LadderNetworkViewModel network, IEnumerable<BaseViewModel> elements, IEnumerable<BaseViewModel> oldelements)
        {
            _network = network;
            _elements = new HashSet<BaseViewModel>(elements);
            _oldelements = new HashSet<BaseViewModel>(oldelements);
            _vlines = new HashSet<VerticalLineViewModel>();
            _oldvlines = new HashSet<VerticalLineViewModel>();
        }

        public void Execute()
        {
            foreach(var oldele in _oldelements)
            {
                _network.RemoveElement(oldele);
            }
            foreach(var oldvline in _oldvlines)
            {
                _network.RemoveVerticalLine(oldvline);
            }
            foreach(var ele in _elements)
            {
                _network.ReplaceElement(ele);
            }
            foreach(var vline in _vlines)
            {
                _network.ReplaceVerticalLine(vline);
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            foreach (var ele in _elements)
            {
                _network.RemoveElement(ele);
            }
            foreach (var vline in _vlines)
            {
                _network.RemoveVerticalLine(vline);
            }

            foreach (var oldele in _oldelements)
            {
                _network.ReplaceElement(oldele);
            }
            foreach (var oldvline in _oldvlines)
            {
                _network.ReplaceVerticalLine(oldvline);
            }
        }
    }
}
