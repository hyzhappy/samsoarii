﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCCompiler;
using SamSoarII.LadderInstViewModel;
using System.Windows.Media;

namespace SamSoarII.AppMain.Project
{
    public class LadderHelper
    {
        public static SolidColorBrush FoldingBrush;
        public static SolidColorBrush SimulateBrush;
        public static SolidColorBrush MonitorBrush;
        public static SolidColorBrush LoadingBrush;
        static LadderHelper()
        {
            Color color = new Color();
            color.A = 255;
            color.R = 60;
            color.G = 58;
            color.B = 58;
            FoldingBrush = new SolidColorBrush(color);
            //#FF0281CE
            color = new Color();
            color.A = 255;
            color.R = 0x02;
            color.G = 0x81;
            color.B = 0xCE;
            MonitorBrush = new SolidColorBrush(color);
            //#FFF46622
            color = new Color();
            color.A = 255;
            color.R = 0xf4;
            color.G = 0x66;
            color.B = 0x22;
            SimulateBrush = new SolidColorBrush(color);
            //#FFA92AD7
            color = new Color();
            color.A = 255;
            color.R = 0xa9;
            color.G = 0x2a;
            color.B = 0xd7;
            LoadingBrush = new SolidColorBrush(color);
        }
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
                    throw new ArgumentException();
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
                if (viewmodel.NextElements.Count == 0)
                {           
                    return node;
                }
                else
                {
                    foreach(var _next_viewmodel in viewmodel.NextElements)
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
