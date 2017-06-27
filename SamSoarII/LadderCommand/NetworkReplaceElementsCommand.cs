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
    public class NetworkReplaceElementsCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;
        private IEnumerable<BaseViewModel> _elements;
        private IEnumerable<VerticalLineViewModel> _vlines;
        private IEnumerable<BaseViewModel> _oldelements;
        private IEnumerable<VerticalLineViewModel> _oldvlines;
        private NetworkChangeElementArea _area;
        private NetworkChangeElementArea _oldarea;

        public NetworkReplaceElementsCommand(
            LadderNetworkViewModel network,
            IEnumerable<BaseViewModel> elements, IEnumerable<VerticalLineViewModel> vlines,
            IEnumerable<BaseViewModel> oldelements, IEnumerable<VerticalLineViewModel> oldvlines,
            NetworkChangeElementArea area = null,
            NetworkChangeElementArea oldarea = null)
        {
            _network = network;
            _elements = elements;
            _vlines = vlines;
            _oldelements = oldelements;
            _oldvlines = oldvlines;
            _area = area;
            _oldarea = oldarea;
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _network, _elements, _vlines);
            }
            if (_oldarea == null)
            {
                _oldarea = NetworkChangeElementArea.Create(
                    _network, _oldelements, _oldvlines);
            }
        }

        public NetworkReplaceElementsCommand(
            LadderNetworkViewModel network,
            IEnumerable<BaseViewModel> elements,
            IEnumerable<BaseViewModel> oldelements,
            NetworkChangeElementArea area = null,
            NetworkChangeElementArea oldarea = null)
        {
            _network = network;
            _elements = elements;
            _oldelements = oldelements;
            _vlines = new VerticalLineViewModel[] { };
            _oldvlines = new VerticalLineViewModel[] { };
            _area = area;
            _oldarea = oldarea;
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _network, _elements, _vlines);
            }
            if (_oldarea == null)
            {
                _oldarea = NetworkChangeElementArea.Create(
                    _network, _oldelements, _oldvlines);
            }
        }
        
        public IEnumerable<BaseViewModel> OldElements
        { get { return _oldelements; } }

        public IEnumerable<BaseViewModel> NewElements
        { get { return _elements; } }

        public virtual void Execute()
        {
            _network.RemoveElement(_oldelements);
            _network.RemoveVerticalLine(_oldvlines);
            _network.ReplaceElement(_elements);
            _network.ReplaceVerticalLine(_vlines);
            if (_area != null)
            {
                _area.Select(_network);
            }
        }

        public virtual void Redo()
        {
            Execute();
        }

        public virtual void Undo()
        {
            _network.RemoveElement(_elements);
            _network.RemoveVerticalLine(_vlines);
            _network.ReplaceElement(_oldelements);
            _network.ReplaceVerticalLine(_oldvlines);
            if (_oldarea != null)
            {
                _oldarea.Select(_network);
            }
        }
    }
    
}
