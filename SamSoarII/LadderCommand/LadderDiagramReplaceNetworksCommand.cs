﻿using SamSoarII.AppMain.Project;
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
        }
        public void Execute()
        {
            _ladderDiagram.AddNetwork(_replacedNetworks, _index);
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
            else if (_replacedNetworks.Count() > 0)
            {
                // 将梯形图光标移到新生成的行的头部
                LadderNetworkViewModel lnvmodel = _replacedNetworks.First();
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
            _ladderDiagram.IDVModel.Setup(_ladderDiagram);
            _ladderDiagram.ClearModelMessageByNetwork(_replacedNetworks);
            _ladderDiagram.UpdateModelMessageByNetwork();
            if (_oldarea != null)
            {
                LadderNetworkViewModel lnvmodel = _removedNetworks.First();
                _oldarea.Select(lnvmodel);
            }
            else if (_removedNetworks.Count() > 0)
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
            
        }
    }
}
