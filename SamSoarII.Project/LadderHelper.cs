﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.PLCCompiler;
using SamSoarII.InstructionViewModel;
namespace SamSoarII.Project
{
    public class LadderHelper
    {
        public static ExpGraphNode CreateNodeByElement(BaseViewModel viewmodel)
        {
            switch(viewmodel.Type)
            {
                case ElementType.Output:
                    return new ExpGraphOutputENode(viewmodel.Model);
                case ElementType.Input:
                    return new ExpGraphInputJNode(viewmodel.Model);
                case ElementType.Special:
                    return new ExpGraphSpecialNode(viewmodel.Model);
                default:
                    throw new NotImplementedException();
            }
        }
        public static ExpGraph ConvertGraph(IEnumerable<BaseViewModel> viewModels)
        {
            ExpGraph graph = new ExpGraph();
            Dictionary<BaseViewModel, ExpGraphNode> _viewmodel_node_map = new Dictionary<BaseViewModel, ExpGraphNode>();
            foreach (var viewmodel in viewModels.Where(x => { return x.Type == ElementType.Output; }))
            {
                var rnode = ConvertGraphNode(viewmodel, _viewmodel_node_map);
                graph.AddRootNode(rnode);
            }
            foreach(var gnode in _viewmodel_node_map.Values)
            {
                graph.AddGenNode(gnode);
            }
            return graph;
        }

        private static ExpGraphNode ConvertGraphNode(BaseViewModel viewmodel, Dictionary<BaseViewModel, ExpGraphNode> _viewmodel_node_map)
        {
            ExpGraphNode node;
            if(_viewmodel_node_map.TryGetValue(viewmodel, out node))
            {
                return node;
            }
            else
            {
                node = CreateNodeByElement(viewmodel);
                if(viewmodel.Type != ElementType.Output)
                    _viewmodel_node_map.Add(viewmodel, node);
                if (viewmodel.NextElemnets.Count == 0)
                {           
                    return node;
                }
                else
                {
                    foreach(var _next_viewmodel in viewmodel.NextElemnets)
                    {
                        if(_next_viewmodel.Type != ElementType.Null)
                        {
                            var _n_node = ConvertGraphNode(_next_viewmodel, _viewmodel_node_map);
                            node.AddNextNode(_n_node);
                        }
                    }
                    return node;
                }
            }
        }
    }
}
