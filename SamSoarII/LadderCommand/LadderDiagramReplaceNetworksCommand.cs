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
            _ladderDiagram.SetMaskNumber();
            _ladderDiagram.AddNetwork(_replacedNetworks, _index,false);
            _ladderDiagram.RemoveNetworks(_removedNetworks);
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.ClearModelMessageByNetwork(_removedNetworks);
            _ladderDiagram.UpdateModelMessageByNetwork();
            _area = NetworkChangeElementArea.Create(
                _ladderDiagram, _replacedNetworks);
            LadderNetworkViewModel lnvmodel = _replacedNetworks.First();
            _area.Select(lnvmodel);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            _ladderDiagram.AddNetwork(_removedNetworks, _index,true);
            _ladderDiagram.RemoveNetworks(_replacedNetworks);
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.ClearModelMessageByNetwork(_replacedNetworks);
            _ladderDiagram.UpdateModelMessageByNetwork();
            _oldarea = NetworkChangeElementArea.Create(
                _ladderDiagram, _removedNetworks);
            if (_removedNetworks.Count == 0) return;
            LadderNetworkViewModel lnvmodel = _removedNetworks.First();
            _oldarea.Select(lnvmodel);
        }
    }
}
