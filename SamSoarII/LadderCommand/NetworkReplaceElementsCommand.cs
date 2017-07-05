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
        private int _oldRowCount;

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
            _oldRowCount = _network.RowCount;
            int tempRowCount = ComputeRowCount();
            if (_network.RowCount < tempRowCount) _network.RowCount = tempRowCount;
            _network.RemoveElements(_oldelements);
            _network.RemoveVerticalLines(_oldvlines);
            _network.ReplaceElements(_elements);
            _network.ReplaceVerticalLines(_vlines);
            InstructionCommentManager.RaiseMappedMessageChangedEvent();
            if (_area != null)
            {
                _area.Select(_network);
            }
        }

        private int ComputeRowCount()
        {
            if (_elements.Count() == 0 && _vlines.Count() == 0) return 0;
            if (_elements.Count() == 0) return _vlines.OrderBy(x => { return x.Y; }).Last().Y + 2;
            if (_vlines.Count() == 0) return _elements.OrderBy(x => { return x.Y; }).Last().Y + 1;
            return Math.Max(_elements.OrderBy(x => { return x.Y; }).Last().Y + 1,
                _vlines.OrderBy(x => { return x.Y; }).Last().Y + 2);
        }

        public virtual void Redo()
        {
            Execute();
        }

        public virtual void Undo()
        {
            _network.RowCount = _oldRowCount;
            _network.RemoveElements(_elements);
            _network.RemoveVerticalLines(_vlines);
            _network.ReplaceElements(_oldelements);
            _network.ReplaceVerticalLines(_oldvlines);
            InstructionCommentManager.RaiseMappedMessageChangedEvent();
            _oldarea?.Select(_network);
            if (_oldarea == null) _area?.Select(_network);
        }
    }
}
