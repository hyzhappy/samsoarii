using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.AppMain.UI;

namespace SamSoarII.AppMain.LadderCommand
{
    public class LadderDiagramRemoveNetworksCommand : IUndoableCommand
    {
        private LadderDiagramViewModel _ladderDiagram;
        private NetworkChangeElementArea _area;

        // 保证网络排序
        private SortedSet<LadderNetworkViewModel> _removedNetworks;

        private int _index;
        public LadderDiagramRemoveNetworksCommand(
            LadderDiagramViewModel ld, IEnumerable<LadderNetworkViewModel> removedNets, int index,
            NetworkChangeElementArea area = null)
        {
            _ladderDiagram = ld;
            _removedNetworks = new SortedSet<LadderNetworkViewModel>(removedNets);
            _index = index;
            _area = area;
            if (_area == null)
            {
                _area = NetworkChangeElementArea.Create(
                    _ladderDiagram, _removedNetworks);
            }
        }

        public void Execute()
        {
            //保证网络号的重排不会影响屏蔽号
            _ladderDiagram.SetNumberAsync(true);
            foreach(var net in _removedNetworks)
            {
                _ladderDiagram.RemoveNetwork(net);
            }
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.ClearModelMessageByNetwork(_removedNetworks);
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
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.UpdateModelMessageByNetwork();
            if (_area != null)
            {
                LadderNetworkViewModel lnvmodel = _removedNetworks.First();
                _area.Select(lnvmodel);
            }
        }
    }
}
