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
        }

        public void Execute()
        {
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
            _ladderDiagram.AddNetwork(_removedNetworks, _index);
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.UpdateModelMessageByNetwork();
            if (_removedNetworks.Count() > 0)
            {
                // 将梯形图光标移到新生成的行的头部
                LadderNetworkViewModel lnvmodel = _removedNetworks.First();
                LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
                ldvmodel.SelectionRect.X = 0;
                ldvmodel.SelectionRect.Y = 0;
                ldvmodel.ProjectModel.IFacade.NavigateToNetwork(
                    new NavigateToNetworkEventArgs(
                        lnvmodel.NetworkNumber,
                        ldvmodel.ProgramName,
                        ldvmodel.SelectionRect.X,
                        ldvmodel.SelectionRect.Y));
            }
            else if (_area != null)
            {
                LadderNetworkViewModel lnvmodel = _removedNetworks.First();
                _area.Select(lnvmodel);
            }
        }
    }
}
