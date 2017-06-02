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
            _network.INVModel.Setup(_network);
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
            _network.INVModel.Setup(_network);
            if (_oldelements.Count() + _oldvlines.Count() == 1)
            {
                // 将梯形图光标移到新生成的单个元件
                BaseViewModel bvmodel = _oldelements.Count() == 1
                    ? _oldelements.First() : _oldvlines.First();
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
            else if (_oldarea != null)
            {
                _oldarea.Select(_network);
            }
        }
    }
    
}
