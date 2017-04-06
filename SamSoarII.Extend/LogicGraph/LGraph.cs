using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.LadderChartModel;
using SamSoarII.Extend.Utility;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 逻辑图模型
/// </remarks>

namespace SamSoarII.Extend.LogicGraph
{
    public class LGraph
    {
        /// <summary>
        /// 内部成员变量
        /// </summary>

        private LadderChart lchart;

        private List<LGVertex> vertexs;
        private List<LGVertex> starts;
        private List<LGVertex> terminates;
        private List<LGEdge> edges;

        /// <summary>
        /// 初始化
        /// </summary>
        public LGraph(LadderChart lchart, int size)
        {
            this.lchart = lchart;
            vertexs = new List<LGVertex>();
            starts = new List<LGVertex>();
            terminates = new List<LGVertex>();
            edges = new List<LGEdge>();
            
            for (int i = 1; i <= size; i++)
                vertexs.Add(new LGVertex(i));
        }
        public LGraph()
        {
            this.lchart = null;
            vertexs = new List<LGVertex>();
            starts = new List<LGVertex>();
            terminates = new List<LGVertex>();
            edges = new List<LGEdge>();
        }
        public LadderChart LChart
        {
            get { return this.lchart; }
            set { this.lchart = value; }
        }
        /// <summary>
        /// 所有点的列表
        /// </summary>
        public List<LGVertex> Vertexs
        {
            get { return this.vertexs; }
            set { this.vertexs = value; }
        }
        /// <summary>
        /// 所有初始点的列表
        /// </summary>
        public List<LGVertex> Starts
        {
            get { return this.starts; }
            set { this.starts = value; }
        }
        /// <summary>
        /// 所有终止点的列表
        /// </summary>
        public List<LGVertex> Terminates
        {
            get { return this.terminates; }
            set { this.terminates = value; }
        }
        /// <summary>
        /// 所有边的列表
        /// </summary>
        public List<LGEdge> Edges
        {
            get { return this.edges; }
            set { this.edges = value; }
        }
        /// <summary>
        /// 添加边
        /// </summary>
        public void InsertEdge(LCNode lcnode, int sid, int did)
        {
            if (sid <= 0 || sid > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            if (did <= 0 || did > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            //Console.Write("edge {0:d} {1:d}\n", sid, did);
            LGEdge e = new LGEdge(lcnode, vertexs[sid-1], vertexs[did-1]);
            vertexs[sid-1].InsertEdge(e);
            vertexs[did-1].InsertEdge(e);
            edges.Add(e);
        }
        /// <summary>
        /// 将制定的点设为起点
        /// </summary>
        public void SetStart(int id)
        {
            if (id <= 0 || id > vertexs.Count)
                throw new ArgumentOutOfRangeException();
            LGVertex lgv = vertexs[id - 1];
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
            LGVertex lgv = vertexs[id - 1];
            if (terminates.Contains(lgv))
                return;
            lgv.IsTerminate = true;
            terminates.Add(lgv);
        }
        /// <summary>
        /// 检查是否断路，可以直接检查梯形图的所有元件
        /// </summary>
        public bool checkOpenCircuit()
        {
            // 直接检查梯形图
            return lchart.checkOpenCircuit();
            /*
            foreach (LGEdge lge in edges)
            {
                LCNode lcnode = lge.PLCInfo;
                // 左断路，右断路，下断路
                if (lcnode.X > 0 && lcnode.Left == null)
                    return true;
                if (lcnode.X < 9 && lcnode.Right == null)
                    return true;
                if (lcnode.HAccess && lcnode.Down == null)
                    return true;
            }
            return false;
            */
        }
        /// <summary>
        /// 检查是否短路，在逻辑图中体现为单向环
        /// </summary>
        public bool checkShortCircuit()
        {
            // 做一次拓扑排序来检查
            // 初始化每个节点的访问标记和后向计数
            foreach (LGVertex lgv in vertexs)
            {
                lgv[4] = 0;
            }
            foreach (LGEdge lge in edges)
            {
                lge.Destination[4]++;
            }
            // 开始拓扑排序，将计数为0的点加入队列中
            Queue<LGVertex> queue = new Queue<LGVertex>();
            foreach (LGVertex lgv in vertexs)
            {
                if (lgv[4] == 0)
                    queue.Enqueue(lgv);
            }
            // 记录已经进行拓扑排序的节点数量
            int solveCount = 0;
            while (queue.Count > 0)
            {
                solveCount++;
                LGVertex lgv = queue.Dequeue();
                //Console.Write("solve : {0:d}\n", lgv.Id);
                foreach (LGEdge lge in lgv.Edges)
                {
                    //Console.Write("next => {0:d} {1:d}\n", lge.Destination.Id, lge.Destination[4]);
                    if ((--lge.Destination[4]) == 0)
                    {
                        queue.Enqueue(lge.Destination);
                    }
                }
            }
            //Console.Write("c1={0:d} c2={1:d}\n", solveCount, vertexs.Count);
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
        /// 所以可以检查所有的环，判断是否都合法
        /// 合法的条件为in(T)-out(S)+sum(in(a)-out(a)) == 0, a为环内不包含原终点的点
        /// in(T)是相对于终点T的出度，即所有能到达T的边的总数
        /// out(S)是相对于起点S的出度，即所有能从S到达该点的边的总数
        /// </detail>
        public bool CheckFusionCircuit()
        {
            // 枚举每个点作为环的原点
            foreach (LGVertex lgv1 in vertexs)
            {
                // 出度为1的点不可能是环的原点
                if (lgv1.Edges.Count <= 1)
                    continue;
                /*
                 *  对逻辑图中的每个点作标记
                 *  标记1为这个点被访问过几次，即原点出发有多少条路径到达该点
                 *  标记2为in(T) - out(S)
                 *  标记3为内部的入出度的差的总和，即sum(in(a) - out(a))
                 *  标记4的用途会解释
                 */
                foreach (LGVertex lgv2 in vertexs)
                    lgv2[1] = lgv2[2] = lgv2[3] = lgv2[4] = 0;
                /*
                 * in(T)和out(S)是无法直接获得的，需要从起点出发访问所有的边
                 * 并且边上的四个标记以位的方式存储这条边是否到达某个节点
                 * 标记1存储编号1-32，标记2存33-64，以此类推
                 * 对于每个相对于S的in(T)，找到标记过的后向边的数量
                 * 对于每个相对于T的out(S)，找出在对应位标记的前向边的数量
                 * 点的标记这里暂时用来统计边的到达情况
                 */
                foreach (LGEdge lge in edges)
                    lge[1] = lge[2] = lge[3] = lge[4];
                _EdgeSearch(lgv1);
                foreach (LGVertex lgv2 in vertexs)
                {
                    // 这里对点标记再次初始化，用于后面的环判断
                    lgv2[1] = lgv2[2] = lgv2[3] = lgv2[4] = 0;
                    // 该点应不为起点
                    if (lgv2.Id == lgv1.Id)
                        continue;
                    // 检查终点的所有后向边，统计in(T)
                    foreach (LGEdge lge in lgv2.BackEdges)
                    {
                        // 若能到达则+1
                        if (!_EdgeEmptyFlag(lge))
                            lgv2[2]++;
                    }
                    // 检查起点的所有前向边，统计out(S)
                    foreach (LGEdge lge in lgv1.Edges)
                    {
                        // 若能到达则-1
                        if (_EdgeGetFlag(lge, lgv2.Id))
                            lgv2[2]--;
                    }
                }
                // 调用内部的判断方法
                //Console.Write("start {0:d}\n", lgv1.Id);
                if (!_CheckFusionCircuit(lgv1, lgv1))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 得到边标记中点标号的对应位
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <param name="id">给定的点标号</param>
        /// <returns>对应的位的值（0或1,1返回true）</returns>
        private bool _EdgeGetFlag(LGEdge lge, int id)
        {
            if (id <= 32) return ((lge[1] >> (id - 1)) & 1) != 0;
            if (id <= 64) return ((lge[2] >> (id - 33)) & 1) != 0;
            if (id <= 96) return ((lge[3] >> (id - 65)) & 1) != 0;
            if (id <= 128) return ((lge[4] >> (id - 97)) & 1) != 0;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 设置边标记中点标号的对应位（设置为1）
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <param name="id">给定的点标号</param>
        /// <returns>对应的标记</returns>
        private int _EdgeSetFlag(LGEdge lge, int id)
        {
            if (id <= 32) return lge[1] |= (1 << (id - 1));
            if (id <= 64) return lge[2] |= (1 << (id - 33));
            if (id <= 96) return lge[3] |= (1 << (id - 65));
            if (id <= 128) return lge[4] |= (1 << (id - 97));
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 判断这个边的所有标记是否为空
        /// </summary>
        /// <param name="lge">给定的边</param>
        /// <returns>是否为空，为空返回true</returns>
        private bool _EdgeEmptyFlag(LGEdge lge)
        {
            return (lge[1] == 0 && lge[2] == 0 && lge[3] == 0 && lge[4] == 0);
        }
        /// <summary>
        /// 判断这个点的所有标记是否为空
        /// </summary>
        /// <param name="lgv">给定的点</param>
        /// <returns>是否为空，为空返回true</returns>
        private bool _VertexEmptyFlag(LGVertex lgv)
        {
            return (lgv[1] == 0 && lgv[2] == 0 && lgv[3] == 0 && lgv[4] == 0);
        }
        /// <summary>
        /// 从当前边开始前向查询，得到到达所有节点的通行情况，保存在标记中
        /// </summary>
        /// <param name="lgv">当前要查询的边</param>
        private void _EdgeSearch(LGVertex lgv)
        {
            foreach (LGEdge lge in lgv.Edges)
            {
                // 指向的点未被访问，标记为空
                if (_VertexEmptyFlag(lge.Destination))
                    _EdgeSearch(lge.Destination);
                // 将边的标记设为指向点的标记
                lge[1] = lge.Destination[1];
                lge[2] = lge.Destination[2];
                lge[3] = lge.Destination[3];
                lge[4] = lge.Destination[4];
                // 能够指向该点，添加通行信息
                _EdgeSetFlag(lge, lge.Destination.Id);
                // 该点统计所有前向边
                lgv[1] |= lge[1];
                lgv[2] |= lge[2];
                lgv[3] |= lge[3];
                lgv[4] |= lge[4];
            }
        }
        /// <summary>
        /// 内部检查是否有不合法的环
        /// </summary>
        /// <param name="lgv"> 当前检查的节点</param>
        /// <param name="lgvs"> 环的原点</param>
        /// <returns> 是否存在非法环</returns>
        private bool _CheckFusionCircuit(LGVertex lgv, LGVertex lgvs)
        {
            // 下一个节点的标记3
            int _lgvn3 = lgv.Id == lgvs.Id ? 0 : lgv[3] + lgv.BackEdges.Count - lgv.Edges.Count;
            //Console.Write("check {0:d} {1:d} {2:d}\n", lgv.Id, lgv[3], _lgvn3);
            // 访问所有前向相邻的节点
            foreach (LGEdge lgen in lgv.Edges)
            {
                LGVertex lgvn = lgen.Destination;
                /*
                 * 若该点未被访问的情况下
                 * 访问该点，作相应的标记
                 */
                if (lgvn[1] == 0)
                {
                    lgvn[1]++;
                    //lgvn[2] = lgvn.BackEdges.Count - lgvs.Edges.Count;
                    lgvn[3] = _lgvn3;
                    //Console.Write("visit1 {0:d} {1:d}\n", lgvn.Id, lgvn[2]);
                    if (!_CheckFusionCircuit(lgvn, lgvs))
                        return false;
                }
                /*
                 * 该点访问过一次的情况下
                 * 两条路径构成了一个环，判断这个环的方法为以下表达式是否成立
                 *     in(T) - out(S) + sum(in(a) - out(a)) == 0
                 * 其中节点lgvn的 flag2 = in(T) - out(S)，
                 *     节点lgvn的 flag3 = sum(a1)
                 *     变量_lgvn3的值为 sum(a2)
                 * 所以需要判断in(T) - out(S) + sum(a1) + sum(a2)是否为0
                 */
                else if (lgvn[1] == 1)
                {
                    lgvn[1]++;
                    //Console.Write("visit2 {0:d} {1:d} {2:d}\n", lgvn.Id, lgvn[2], lgvn[3]);
                    if (lgvn[3] + _lgvn3 + lgvn[2] != 0)
                        return false;
                    lgvn[4] = _lgvn3;
                }
                /*
                 * 该点访问过两次以上的情况下
                 * 由于多条路径会构成多条环，以三条路径为例，会分别构成三条环，对应以下三个表达式
                 * 设C = in(T) - out(S)
                 * C + sum(a1) + sum(a2) == 0
                 * C + sum(a1) + sum(a3) == 0
                 * C + sum(a2) + sum(a3) == 0
                 * 可得sum(a1) == sum(a2) == sum(a3)
                 * 所以这里需要判断所有的sum是否相等，之前访问时用到的sum(a2)需要存在flag4里
                 */
                else
                {
                    lgvn[1]++;
                    //Console.Write("visit3 {0:d} {1:d} {2:d} {3:d}\n", lgvn.Id, lgvn[2], lgvn[3], lgvn[4]);
                    if (_lgvn3 != lgvn[3] || lgvn[3] != lgvn[4])
                        return false;
                }
            }
            return true;
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
            foreach (LGVertex lgv in terminates)
            {
                if (lgv.Expr.Equals("null"))
                    lgv.GenExpr();
            }
            // 按照ASCII码来排序
            Comparison<LGVertex> sortByExpr = delegate (LGVertex v1, LGVertex v2)
            {
                return v1.Expr.CompareTo(v2.Expr);
            };
            terminates.Sort(sortByExpr);
            // 调用方法
            List<PLCInstruction> insts = ExprHelper.GenInst(terminates);
            return insts;
        } 
    }
}
