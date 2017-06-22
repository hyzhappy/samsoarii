using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class LadderDiagramReplaceNetworksCommand : IUndoableCommand
    {
        private LadderDiagramViewModel _ladderDiagram;

        // 保证网络排序
        private SortedSet<LadderNetworkViewModel> _replacedNetworks;
        private SortedSet<LadderNetworkViewModel> _removedNetworks;
        private NetworkChangeElementArea _oldarea;
        private NetworkChangeElementArea _area;
        private int _index;
        public LadderDiagramReplaceNetworksCommand(
            LadderDiagramViewModel ld, 
            IEnumerable<LadderNetworkViewModel> replacedNets, IEnumerable<LadderNetworkViewModel> removedNets, int index,
            NetworkChangeElementArea area = null, NetworkChangeElementArea oldarea = null)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>(replacedNets);
            _removedNetworks = new SortedSet<LadderNetworkViewModel>(removedNets);
            _index = index;
            _oldarea = oldarea;
            _area = area;
            if (_oldarea == null)
            {
                _oldarea = NetworkChangeElementArea.Create(
                    _ladderDiagram, _removedNetworks);
            }
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _ladderDiagram, _replacedNetworks);
            }
        }

        public LadderDiagramReplaceNetworksCommand(
            LadderDiagramViewModel ld, 
            IEnumerable<LadderNetworkViewModel> replacedNets, int index,
            NetworkChangeElementArea area = null, NetworkChangeElementArea oldarea = null)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>(replacedNets);
            _removedNetworks = new SortedSet<LadderNetworkViewModel>();
            _index = index;
            _oldarea = oldarea;
            _area = area;
            if (_oldarea == null)
            {
                _oldarea = NetworkChangeElementArea.Create(
                    _ladderDiagram, _removedNetworks);
            }
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _ladderDiagram, _replacedNetworks);
            }
        }

        public LadderDiagramReplaceNetworksCommand(
            LadderDiagramViewModel ld, 
            LadderNetworkViewModel net, int index,
            NetworkChangeElementArea area = null, NetworkChangeElementArea oldarea = null)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>() { net };
            _removedNetworks = new SortedSet<LadderNetworkViewModel>();
            _index = index;
            _oldarea = oldarea;
            _area = area;
            if (_oldarea == null)
            {
                _oldarea = NetworkChangeElementArea.Create(
                    _ladderDiagram, _removedNetworks);
            }
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _ladderDiagram, _replacedNetworks);
            }
        }

        public LadderDiagramReplaceNetworksCommand(
            LadderDiagramViewModel ld, 
            LadderNetworkViewModel replacedNet, IEnumerable<LadderNetworkViewModel> removedNets, int index,
            NetworkChangeElementArea area = null, NetworkChangeElementArea oldarea = null)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>() { replacedNet };
            _removedNetworks = new SortedSet<LadderNetworkViewModel>(removedNets);
            _index = index;
            _oldarea = oldarea;
            _area = area;
            if (_oldarea == null)
            {
                _oldarea = NetworkChangeElementArea.Create(
                    _ladderDiagram, _removedNetworks);
            }
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _ladderDiagram, _replacedNetworks);
            }
        }
        public void Execute()
        {
            //保证网络号的重排不会影响屏蔽号
            _ladderDiagram.SetNumberAsync(true);
            _ladderDiagram.AddNetwork(_replacedNetworks, _index,false);
            foreach (var net in _removedNetworks)
            {
                _ladderDiagram.RemoveNetwork(net);
            }
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.ClearModelMessageByNetwork(_removedNetworks);
            _ladderDiagram.UpdateModelMessageByNetwork();
            if (_area != null)
            {
                LadderNetworkViewModel lnvmodel = _replacedNetworks.First();
                _area.Select(lnvmodel);
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            //保证网络号的重排不会影响屏蔽号
            _ladderDiagram.SetNumberAsync(true);
            _ladderDiagram.AddNetwork(_removedNetworks, _index,true);
            foreach (var net in _replacedNetworks)
            {
                _ladderDiagram.RemoveNetwork(net);
            }
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.ClearModelMessageByNetwork(_replacedNetworks);
            _ladderDiagram.UpdateModelMessageByNetwork();
            if (_oldarea != null)
            {
                LadderNetworkViewModel lnvmodel = _removedNetworks.First();
                _oldarea.Select(lnvmodel);
            }
        }
    }
}
