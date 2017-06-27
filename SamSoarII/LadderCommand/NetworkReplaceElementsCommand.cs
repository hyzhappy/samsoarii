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
        private HashSet<BaseViewModel> _elements;
        private HashSet<VerticalLineViewModel> _vlines;
        private HashSet<BaseViewModel> _oldelements;
        private HashSet<VerticalLineViewModel> _oldvlines;
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
            _elements = new HashSet<BaseViewModel>(elements);
            _vlines = new HashSet<VerticalLineViewModel>(vlines);
            _oldelements = new HashSet<BaseViewModel>(oldelements);
            _oldvlines = new HashSet<VerticalLineViewModel>(oldvlines);
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
            _elements = new HashSet<BaseViewModel>(elements);
            _oldelements = new HashSet<BaseViewModel>(oldelements);
            _vlines = new HashSet<VerticalLineViewModel>();
            _oldvlines = new HashSet<VerticalLineViewModel>();
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

        public BaseViewModel PopOldElement()
        {
            BaseViewModel bvmodel = _oldelements.FirstOrDefault();
            if (bvmodel != null)
            {
                _oldelements.Remove(bvmodel);
            }
            return bvmodel;
        }
        
        public BaseViewModel PopNewElement()
        {
            BaseViewModel bvmodel = _elements.FirstOrDefault();
            if (bvmodel != null)
            {
                _elements.Remove(bvmodel);
            }
            return bvmodel;
        }

        public virtual void Execute()
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
            if (_area != null)
            {
                _area.Select(_network);
            }
            InstructionCommentManager.RaiseMappedMessageChangedEvent();
        }

        public virtual void Redo()
        {
            Execute();
        }

        public virtual void Undo()
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
            InstructionCommentManager.RaiseMappedMessageChangedEvent();
            if (_oldarea != null)
            {
                _oldarea.Select(_network);
            }
        }
    }
    
}
