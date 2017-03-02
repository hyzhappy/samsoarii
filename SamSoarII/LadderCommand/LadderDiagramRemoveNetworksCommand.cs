using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
namespace SamSoarII.AppMain.LadderCommand
{
    public class LadderDiagramRemoveNetworksCommand : IUndoableCommand
    {
        private LadderDiagramViewModel _ladderDiagram;

        // 保证网络排序
        private SortedSet<LadderNetworkViewModel> _removedNetworks;

        private int _index;
        public LadderDiagramRemoveNetworksCommand(LadderDiagramViewModel ld, IEnumerable<LadderNetworkViewModel> removedNets, int index)
        {
            _ladderDiagram = ld;
            _removedNetworks = new SortedSet<LadderNetworkViewModel>(removedNets);
            _index = index;
        }

        public void Execute()
        {
            foreach(var net in _removedNetworks)
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
        }
    }
}
