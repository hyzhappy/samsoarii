using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class LadderDiagramExchangeNetworkCommand : IUndoableCommand
    {
        private LadderDiagramViewModel _ladderDiagram;
        private LadderNetworkViewModel _sourceNetwork;
        private LadderNetworkViewModel _desNetwork;
        public LadderDiagramExchangeNetworkCommand(LadderDiagramViewModel LD,LadderNetworkViewModel source,LadderNetworkViewModel des)
        {
            _ladderDiagram = LD;
            _sourceNetwork = source;
            _desNetwork = des;
        }
        public void Execute()
        {
            _ladderDiagram.SetNumberAsync(false);
            if (_sourceNetwork.NetworkNumber == _desNetwork.NetworkNumber - 1)
            {
                _ladderDiagram.RemoveNetwork(_sourceNetwork);
                _ladderDiagram.AddNetwork(_sourceNetwork,_sourceNetwork.NetworkNumber + 1);
            }
            else if(_sourceNetwork.NetworkNumber == _desNetwork.NetworkNumber + 1)
            {
                _ladderDiagram.RemoveNetwork(_sourceNetwork);
                _ladderDiagram.AddNetwork(_sourceNetwork, _sourceNetwork.NetworkNumber - 1);
            }
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.UpdateModelMessageByNetwork();
        }
        public void Redo()
        {
            Execute();
        }
        public void Undo()
        {
            Execute();
        }
    }
}
