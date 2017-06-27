using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.AppMain.UI;
using System.Threading;
using System.Windows.Threading;

namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkRemoveElementsCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;
        private IEnumerable<BaseViewModel> _elements;
        private IEnumerable<VerticalLineViewModel> _vlines;
        private NetworkChangeElementArea _area;

        public NetworkRemoveElementsCommand(
            LadderNetworkViewModel network, 
            IEnumerable<BaseViewModel> elements, IEnumerable<VerticalLineViewModel> vlines,
            NetworkChangeElementArea area = null)
        {
            _network = network;
            _elements = elements;
            _vlines = vlines;
            _area = area;
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _network, _elements, _vlines);
            }
        }

        public NetworkRemoveElementsCommand(
            LadderNetworkViewModel network, 
            IEnumerable<BaseViewModel> elements,
            NetworkChangeElementArea area = null)
        {
            _network = network;
            _elements = elements;
            _vlines = new VerticalLineViewModel[] { };
            _area = area;
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _network, _elements, _vlines);
            }
        }

        public void Execute()
        {
            _network.RemoveElement(_elements);
            _network.RemoveVerticalLine(_vlines);
            //_network.INVModel.Setup(_network);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            _network.ReplaceElement(_elements);
            _network.ReplaceVerticalLine(_vlines);
            if (_area != null)
            {
                _area.Select(_network);
            }
        }
    }
}
