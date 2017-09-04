using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Utility.DXF
{
    public class DXFGraph
    {
        public SortedDictionary<DXFVertex, List<DXFEdge>> Graph { get; set; }

        private List<DXFEdge> edges;
        public List<DXFEdge> Edges { get { return edges; } }

        private DXFVertex startP = new DXFVertex(new Point(0,0));
        public DXFVertex StartP { get { return startP; } }
        public DXFGraph()
        {
            Graph = new SortedDictionary<DXFVertex, List<DXFEdge>>();
        }

        public void Add(DXFEdge edge)
        {
            if (Graph.ContainsKey(edge.Start)) Graph[edge.Start].Add(edge);
            else Graph.Add(edge.Start, new List<DXFEdge>() { edge });

            if (Graph.ContainsKey(edge.End)) Graph[edge.End].Add(edge);
            else Graph.Add(edge.End, new List<DXFEdge>() { edge });
        }

        //将非连通图转换为一幅连通图
        public void Convert()
        {
            edges = new List<DXFEdge>();
            //判断图是否存在起点，不存在则添加进去
            if (!Graph.ContainsKey(startP))
                Graph.Add(startP, new List<DXFEdge>());
            //按排序的顺序遍历节点,判断是否添加虚线
            List<DXFEdge> tempEdges;
            for (int i = 0; i < Graph.Keys.Count; i++)
            {
                tempEdges = Graph[Graph.Keys.ElementAt(i)];

            }
        }
    }
}
