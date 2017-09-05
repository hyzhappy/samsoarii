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

        private List<DXFEdge> path;
        public List<DXFEdge> Path { get { return path; } }

        private DXFVertex startP = new DXFVertex(new Point(0,0));
        public DXFVertex StartP { get { return startP; } }
        public DXFGraph()
        {
            Graph = new SortedDictionary<DXFVertex, List<DXFEdge>>();
        }

        public void AddEdge(DXFEdge edge)
        {
            if (Graph.ContainsKey(edge.Start)) Graph[edge.Start].Add(edge);
            else Graph.Add(edge.Start, new List<DXFEdge>() { edge });
            //自环的度数为2，因此这里当起点与终点相同时，仍然添加
            if (Graph.ContainsKey(edge.End)) Graph[edge.End].Add(edge);
            else Graph.Add(edge.End, new List<DXFEdge>() { edge });
        }

        private void initVertex()
        {
            foreach (var vertex in Graph.Keys)
            {
                foreach (var edge in Graph[vertex])
                {
                    if (edge.Start.CompareTo(vertex) == 0 && edge.Start != vertex)
                        edge.Start = vertex;
                    if (edge.End.CompareTo(vertex) == 0 && edge.End != vertex)
                        edge.End = vertex;
                }
            }
        }

        //将非连通图转换为一幅连通图
        public void Convert()
        {
            //初始化路径
            path = new List<DXFEdge>();
            //判断图是否存在起点，不存在则添加进去
            if (!Graph.ContainsKey(startP))
                if (Graph.Count > 0)
                    AddEdge(new DXFEdge(startP, Graph.Keys.First()));//添加一条连接起点和图的虚线
            //DXFVertex是类，因此不同边的定点引用不同，这里将引用设置一致
            initVertex();
            //先遍历所有节点，得到该图的所有连通子图,并转化成一笔画图
            HashSet<DXFVertex> tempPoints = new HashSet<DXFVertex>(Graph.Keys);
            //待添加的一笔画图
            List<OneLinearGraph> graphs = new List<OneLinearGraph>();
            DXFVertex vertex;
            while (tempPoints.Count > 0)
            {
                List<DXFVertex> subGraph = new List<DXFVertex>();
                vertex = tempPoints.First();
                DPSearchVertex(vertex, ref tempPoints, ref subGraph);
                graphs.Add(new OneLinearGraph(this, subGraph));
            }
            //将所有一笔画图连接，转化为一笔画路径
            CombinePath(graphs);
        }
        
        private void DPSearchVertex(DXFVertex startP, ref HashSet<DXFVertex> tempPoints, ref List<DXFVertex> subGraph)
        {
            startP.IsSreached = true;
            tempPoints.Remove(startP);
            subGraph.Add(startP);
            foreach (var p in GetLinkedPoints(startP))
            {
                if (!p.IsSreached) DPSearchVertex(p, ref tempPoints, ref subGraph);
            }
        }

        private List<DXFVertex> GetLinkedPoints(DXFVertex startP)
        {
            List<DXFVertex> vertexes = new List<DXFVertex>();
            List<DXFEdge> edges = Graph[startP];
            foreach (var edge in edges)
            {
                if (edge.Start.CompareTo(edge.End) == 0) continue;
                if (edge.Start.CompareTo(startP) == 0) vertexes.Add(edge.End);
                else vertexes.Add(edge.Start);
            }
            return vertexes;
        }

        private void CombinePath(List<OneLinearGraph> graphs)
        {
            path.AddRange(graphs[0].Path);
            for (int i = 1; i < graphs.Count; i++)
            {
                //将每个一笔画图首位相连,生成一幅一笔画图
                path.Add(new DXFEdge(graphs[i - 1].EndP, graphs[i].StartP));
                path.AddRange(graphs[i].Path);
            }
        }
        
        public int Degree(DXFVertex vertex)
        {
            if (Graph.ContainsKey(vertex))
                return Graph[vertex].Count;
            else return -1;
        }
    }
}
