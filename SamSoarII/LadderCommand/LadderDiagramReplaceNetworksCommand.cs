using SamSoarII.AppMain.Project;
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
        private int _index;
        public LadderDiagramReplaceNetworksCommand(LadderDiagramViewModel ld, IEnumerable<LadderNetworkViewModel> replacedNets, IEnumerable<LadderNetworkViewModel> removedNets, int index)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>(replacedNets);
            _removedNetworks = new SortedSet<LadderNetworkViewModel>(removedNets);
            _index = index;
        }

        public LadderDiagramReplaceNetworksCommand(LadderDiagramViewModel ld, IEnumerable<LadderNetworkViewModel> replacedNets, int index)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>(replacedNets);
            _removedNetworks = new SortedSet<LadderNetworkViewModel>();
            _index = index;
        }

        public LadderDiagramReplaceNetworksCommand(LadderDiagramViewModel ld, LadderNetworkViewModel net, int index)
        {
            _ladderDiagram = ld;
            _replacedNetworks = new SortedSet<LadderNetworkViewModel>() { net };
            _removedNetworks = new SortedSet<LadderNetworkViewModel>();
            _index = index;
        }
        public void Execute()
        {
            _ladderDiagram.AddNetwork(_replacedNetworks, _index);
            foreach (var net in _removedNetworks)
            {
                _ladderDiagram.RemoveNetwork(net);
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            _ladderDiagram.AddNetwork(_removedNetworks, _index);
            foreach (var net in _replacedNetworks)
            {
                _ladderDiagram.RemoveNetwork(net);
            }
        }
    }
}
