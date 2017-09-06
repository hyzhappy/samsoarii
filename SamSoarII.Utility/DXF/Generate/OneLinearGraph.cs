using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility.DXF
{
    /// <summary>
    /// 能一笔画的图
    /// </summary>
    public class OneLinearGraph
    {
        private DXFGraph parent;
        public DXFGraph Parent { get { return parent; } }
        
        /// <summary>
        /// 一笔画路径起点
        /// </summary>
        public DXFVertex StartP { get { return startP; } }
        private DXFVertex startP;
        /// <summary>
        /// 一笔画路径终点
        /// </summary>
        public DXFVertex EndP { get { return endP; } }
        private DXFVertex endP;
        /// <summary>
        /// 一笔画路径
        /// </summary>
        public List<DXFEdge> Path { get { return path; } }
        private List<DXFEdge> path;
        public OneLinearGraph(DXFGraph parent, List<DXFVertex> subGraph)
        {
            this.parent = parent;
            path = Convert(subGraph);
        }
        private List<DXFEdge> Convert(List<DXFVertex> subGraph)
        {
            List<DXFEdge> path = new List<DXFEdge>();
            if (subGraph.Count == 1)
            {
                startP = subGraph.First();
                endP = startP;
                foreach (var edge in parent.Graph[startP])
                {
                    if(edge.Start.CompareTo(edge.End) == 0)
                    {
                        if(!path.Contains(edge))
                            path.Add(edge);
                    }
                }
            }
            else if (subGraph.Count == 2)
            {
                startP = subGraph.First();
                endP = subGraph.Last();
                //起点和终点可能还包括自环的元素，因此需要都加上
                List<DXFEdge> edges = parent.Graph[startP];
                edges.AddRange(parent.Graph[endP]);
                foreach (var edge in edges)
                {
                    if (edge.Start.CompareTo(endP) == 0 && edge.End.CompareTo(startP) == 0)
                        edge.Flip();//保证图的有向性
                    if (!path.Contains(edge))
                        path.Add(edge);
                }
            }
            else
            {
                Stack<DXFVertex> tempStack = new Stack<DXFVertex>();
                //首先收集图中所有度数为奇数的节点
                foreach (var vertex in subGraph)
                {
                    if (parent.Degree(vertex) % 2 == 1) tempStack.Push(vertex);
                }
                //当存在度数为奇数的点时，容易证明奇数点必定成对出现，同理，由欧拉某定理可知奇数点为2时，此图可以一笔画，起点终点分别是这两个奇点
                if (tempStack.Count > 0)
                {
                    //奇点大于2时，我们需要添加虚线去连接两个奇点，使其能一笔画
                    DXFVertex vertex1;
                    DXFVertex vertex2;
                    while (tempStack.Count > 2)
                    {
                        vertex1 = tempStack.Pop();
                        vertex2 = tempStack.Pop();
                        //添加虚线
                        parent.AddEdge(new DXFEdge(vertex1, vertex2, parent.Model));
                    }
                    startP = tempStack.Pop();
                    endP = tempStack.Pop();
                }
                else
                {
                    startP = subGraph.First();
                    endP = startP;
                }
                GeneratePath(ref path);
            }
            return path;
        }

        private void GeneratePath(ref List<DXFEdge> path)
        {
            //从起点开始搜索
            DPSearchEdge(startP, ref path);
        }

        private void DPSearchEdge(DXFVertex p, ref List<DXFEdge> path)
        {
            List<DXFEdge> edges = parent.Graph[p];
            List<DXFEdge> selfEdges = edges.Where(edge => { return edge.Start.CompareTo(edge.End) == 0; }).ToList();
            //首先添加此节点的所有自环（自环是不需要继续搜索的）
            foreach (var edge in selfEdges)
            {
                if (!edge.IsSreached)
                {
                    edge.IsSreached = true;
                    path.Add(edge);
                }
            }
            //再遍历其他边
            foreach (var edge in edges.Except(selfEdges))
            {
                if (!edge.IsSreached)
                {
                    edge.IsSreached = true;
                    if (edge.Start.CompareTo(p) != 0)
                        edge.Flip();//保持图的有向性
                    path.Add(edge);
                    //继续搜索
                    DPSearchEdge(edge.End, ref path);
                }
            }
        }
    }
}
