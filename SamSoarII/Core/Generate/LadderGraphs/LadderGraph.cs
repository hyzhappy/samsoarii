using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 逻辑图模型
/// </remarks>
/// 
namespace SamSoarII.Core.Generate
{
    public class LadderGraph : IDisposable
    {
        /// <summary>
        /// 内部成员变量
        /// </summary>
        private LadderChart lchart;
        private List<LadderGraphVertex> vertexs;
        private List<LadderGraphVertex> starts;
        private List<LadderGraphVertex> terminates;
        private List<LadderGraphEdge> edges;

        /// <summary>
        /// 初始化
        /// </summary>
        public LadderGraph(LadderChart lchart, int size)
        {
            this.lchart = lchart;
            vertexs = new List<LadderGraphVertex>();
            starts = new List<LadderGraphVertex>();
            terminates = new List<LadderGraphVertex>();
            edges = new List<LadderGraphEdge>();

            for (int i = 1; i <= size; i++)
                vertexs.Add(new LadderGraphVertex(i));
        }
        public LadderGraph()
        {
            this.lchart = null;
            vertexs = new List<LadderGraphVertex>();
            starts = new List<LadderGraphVertex>();
            terminates = new List<LadderGraphVertex>();
            edges = new List<LadderGraphEdge>();
        }

        public void Dispose()
        {
            foreach (LadderGraphVertex vertex in vertexs)
            {
                vertex.Dispose();
            }
            foreach (LadderGraphEdge edge in edges)
            {
                edge.Dispose();
            }
            vertexs.Clear();
            vertexs = null;
            edges.Clear();
            edges = null;
            starts.Clear();
            starts = null;
            terminates.Clear();
            terminates = null;
        }

        public LadderChart LChart
        {
            get { return this.lchart; }
            set { this.lchart = value; }
        }
        /// <summary>
        /// 所有点的列表
        /// </summary>
        public List<LadderGraphVertex> Vertexs
        {
            get { return this.vertexs; }
            set { this.vertexs = value; }
        }
        /// <summary>
        /// 所有初始点的列表
        /// </summary>
        public List<LadderGraphVertex> Starts
        {
            get { return this.starts; }
            set { this.starts = value; }
        }
        /// <summary>
        /// 所有终止点的列表
        /// </summary>
        public List<LadderGraphVertex> Terminates
        {
            get { return this.terminates; }
            set { this.terminates = value; }
        }
        /// <summary>
        /// 所有边的列表
        /// </summary>
        public List<LadderGraphEdge> Edges
        {
            get { return this.edges; }
            set { this.edges = value; }
        }
        /// <summary>
        /// 添加边
        /// </summary>
        public void InsertEdge(LadderChartNode lcnode, int sid, int did)
        {
            if (sid <= 0 || sid > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            if (did <= 0 || did > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            //Console.Write("edge {0:d} {1:d}\n", sid, did);
            LadderGraphEdge e = new LadderGraphEdge(lcnode, vertexs[sid - 1], vertexs[did - 1]);
            vertexs[sid - 1].InsertEdge(e);
            vertexs[did - 1].InsertEdge(e);
            edges.Add(e);
        }
        /// <summary>
        /// 将制定的点设为起点
        /// </summary>
        public void SetStart(int id)
        {
            if (id <= 0 || id > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            LadderGraphVertex lgv = vertexs[id - 1];
            if (starts.Contains(lgv))
                return;
            lgv.IsStart = true;
            starts.Add(lgv);
        }
        /// <summary>
        /// 将制定的点设为终点
        /// </summary>
        public void SetTerminate(int id)
        {
            if (id <= 0 || id > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            LadderGraphVertex lgv = vertexs[id - 1];
            if (terminates.Contains(lgv))
                return;
            lgv.IsTerminate = true;
            terminates.Add(lgv);
        }
        /// <summary>
        /// 检查是否断路，可以直接检查梯形图的所有元件
        /// </summary>
        public bool CheckOpenCircuit()
        {
            // 直接检查梯形图
            return lchart.CheckOpenCircuit();
        }
        /// <summary>
        /// 检查是否短路，在逻辑图中体现为单向环
        /// </summary>
        public bool CheckShortCircuit()
        {
            // 做一次拓扑排序来检查
            // 初始化每个节点的访问标记和后向计数
            foreach (LadderGraphVertex lgv in vertexs)
            {
                lgv[0] = 0;
            }
            foreach (LadderGraphEdge lge in edges)
            {
                lge.Destination[0]++;
            }
            // 开始拓扑排序，将计数为0的点加入队列中
            Queue<LadderGraphVertex> queue = new Queue<LadderGraphVertex>();
            foreach (LadderGraphVertex lgv in vertexs)
            {
                if (lgv[0] == 0)
                    queue.Enqueue(lgv);
            }
            // 记录已经进行拓扑排序的节点数量
            int solveCount = 0;
            while (queue.Count > 0)
            {
                solveCount++;
                LadderGraphVertex lgv = queue.Dequeue();
                foreach (LadderGraphEdge lge in lgv.NextEdges)
                {
                    if ((--lge.Destination[0]) == 0)
                    {
                        queue.Enqueue(lge.Destination);
                    }
                }
            }
            // 若存在节点未被排序，则在环中
            if (solveCount < vertexs.Count)
                return true;
            return false;
        }
        /// <summary>
        /// 检查是否混联错误
        /// </summary>
        /// <returns>是否存在混联错误</returns>
        /// <detail>
        /// 当存在入度不等于出度的环时，说明这个环存在分支，即符合混联错误的条件
        /// 入度大于出度时存在向内的分支，入度小于出度时存在向外的分支
        /// 所以可以检查所有的联通分量，判断是否都合法
        /// 合法的条件为in(T)-out(S)+sum(in(a)-out(a)) == 0, a为环内不包含原终点的点
        /// in(T)是相对于终点T的出度，即所有能到达T的边的总数
        /// out(S)是相对于起点S的出度，即所有能从S到达该点的边的总数
        /// </detail>
        public bool CheckFusionCircuit()
        {
            int flagcount = (vertexs.Count() - 1) / 32 + 1;
            // 设置标记长度
            foreach (LadderGraphVertex lgv in vertexs)
                lgv.FlagCount = flagcount;
            foreach (LadderGraphEdge lge in edges)
                lge.FlagCount = flagcount;
            // 枚举每个点作为环的原点
            foreach (LadderGraphVertex lgv1 in vertexs)
            {
                // 出度为1的点不可能是环的原点
                if (lgv1.NextEdges.Count <= 1)
                    continue;
                /*
                 * in(T)和out(S)是无法直接获得的，需要从起点出发访问所有的边
                 * 并且点和边上的四个标记以位的方式存储这条边是否到达某个节点
                 * 标记1存储编号1-32，标记2存33-64，以此类推
                 * 对于每个相对于S的in(T)，找到标记过的后向边的数量
                 * 对于每个相对于T的out(S)，找出在对应位标记的前向边的数量
                 */
                foreach (LadderGraphVertex lgv2 in vertexs)
                    for (int i = 0; i < flagcount; i++) lgv2[i] = 0;
                foreach (LadderGraphEdge lge in edges)
                    for (int i = 0; i < flagcount; i++) lge[i] = 0;
                _EdgeSearch(lgv1);
                LadderGraphEdge flagedge = new LadderGraphEdge(null, null, null);
                flagedge.FlagCount = flagcount;
                foreach (LadderGraphVertex lgv2 in vertexs)
                {
                    // 该点应不为起点
                    if (lgv2.ID == lgv1.ID)
                        continue;
                    int sum = 0, sumin = 0, sumout = 0;
                    for (int i = 0; i < flagcount; i++)
                        flagedge[i] = ~0;
                    // 检查终点的所有后向边，统计in(T)
                    foreach (LadderGraphEdge lge in lgv2.BackEdges)
                    {
                        if (!_EdgeEmptyFlag(lge)) sumin++;
                    }
                    // 检查起点的所有前向边，统计out(S)
                    foreach (LadderGraphEdge lge in lgv1.NextEdges)
                    {
                        if (_EdgeGetFlag(lge, lgv2.ID))
                        {
                            sumout++;
                            for (int i = 0; i < flagcount; i++)
                                flagedge[i] &= lge[i];
                        }
                    }
                    // 检查终点的所有前向边，剔除掉后面的桥点
                    foreach (LadderGraphEdge lge in lgv2.NextEdges)
                    {
                        for (int i = 0; i < flagcount; i++)
                            flagedge[i] &= ~lge[i];
                    }
                    // 若不能构成两条以上的路径则忽略
                    if (sumin < 2 || sumout < 2) continue;
                    // 计算in(T)-out(S)
                    sum = sumin - sumout;
                    // 检查所有能够到达终点的点，统计sum(in(a)-out(a))
                    bool hasbridge = false;
                    foreach (LadderGraphVertex lgv3 in vertexs)
                    {
                        if (lgv3.ID == lgv1.ID) continue;
                        if (lgv3.ID == lgv2.ID) continue;
                        // 该点是连通分量的桥点
                        if (_EdgeGetFlag(flagedge, lgv3.ID))
                        {
                            hasbridge = true;
                            break;
                        }
                        if (_VertexGetFlag(lgv3, lgv2.ID))
                            sum += lgv3.BackEdges.Count() - lgv3.NextEdges.Count();
                    }
                    // 如果不存在桥点，并且不满足等式则混连错误
                    if (!hasbridge && sum != 0) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 得到边标记中点标号的对应位
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <param name="id">给定的点标号</param>
        /// <returns>对应的位的值（0或1,1返回true）</returns>
        private bool _EdgeGetFlag(LadderGraphEdge lge, int id)
        {
            for (int i = 0; i < lge.FlagCount; i++)
                if (id <= ((i + 1) << 5))
                    return ((lge[i] >> (id - ((i << 5) + 1))) & 1) != 0;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 设置边标记中点标号的对应位（设置为1）
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <param name="id">给定的点标号</param>
        /// <returns>对应的标记</returns>
        private int _EdgeSetFlag(LadderGraphEdge lge, int id)
        {
            for (int i = 0; i < lge.FlagCount; i++)
                if (id <= ((i + 1) << 5))
                    return lge[i] |= 1 << (id - ((i << 5) + 1));   
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 判断这个边的所有标记是否为空
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <returns>是否为空，为空返回true</returns>
        private bool _EdgeEmptyFlag(LadderGraphEdge lge)
        {
            for (int i = 0; i < lge.FlagCount; i++)
                if (lge[i] != 0) return false;
            return true;
        }
        /// <summary>
        /// 得到点标记中点标号的对应位
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <param name="id">给定的点标号</param>
        /// <returns>对应的位的值（0或1,1返回true）</returns>
        private bool _VertexGetFlag(LadderGraphVertex lgv, int id)
        {
            for (int i = 0; i < lgv.FlagCount; i++)
                if (id <= ((i + 1) << 5))
                    return ((lgv[i] >> (id - ((i << 5) + 1))) & 1) != 0;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 判断这个点的所有标记是否为空
        /// </summary>
        /// <param name="lgv">给定的点</param>
        /// <returns>是否为空，为空返回true</returns>
        private bool _VertexEmptyFlag(LadderGraphVertex lgv)
        {
            for (int i = 0; i < lgv.FlagCount; i++)
                if (lgv[i] != 0) return false;
            return true;
        }
        /// <summary>
        /// 从当前边开始前向查询，得到到达所有节点的通行情况，保存在标记中
        /// </summary>
        /// <param name="lgv">当前要查询的边</param>
        private void _EdgeSearch(LadderGraphVertex lgv)
        {
            foreach (LadderGraphEdge lge in lgv.NextEdges)
            {
                // 指向的点未被访问，标记为空
                if (_VertexEmptyFlag(lge.Destination))
                    _EdgeSearch(lge.Destination);
                // 将边的标记设为指向点的标记
                for (int i = 0; i < lge.FlagCount; i++)
                    lge[i] = lge.Destination[i];
                // 能够指向该点，添加通行信息
                _EdgeSetFlag(lge, lge.Destination.ID);
                // 该点统计所有前向边
                for (int i = 0; i < lgv.FlagCount; i++)
                    lgv[i] |= lge[i];
            }
        }
        /// <summary>
        /// 生成PLC指令
        /// </summary>
        /// <returns>PLC指令列表</returns>
        public List<PLCInstruction> GenInst()
        {
            // 终点为空的情况下，返回空的指令列表
            if (terminates.Count() == 0)
            {
                return new List<PLCInstruction>();
            }
            // 生成所有终点的表达式
            foreach (LadderGraphVertex lgv in terminates)
            {
                if (lgv.Expr == null) lgv.GenExpr();
            }
            // 按照ASCII码来排序
            Comparison<LadderGraphVertex> sortByExpr = delegate (LadderGraphVertex v1, LadderGraphVertex v2)
            {
                return v1.Expr.CompareTo(v2.Expr);
            };
            terminates.Sort(sortByExpr);
            // 调用方法
            List<PLCInstruction> insts = ExprHelper.GenInst(terminates);
            // 获得指令的原型
            foreach (PLCInstruction inst in insts)
            {
                if (inst.PrototypeID != -1)
                {
                    inst.ProtoType = lchart.Nodes[inst.PrototypeID].Prototype;
                }
            }
            return insts;
        }
    }
}
