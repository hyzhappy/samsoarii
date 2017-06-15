using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Extend.LogicGraph;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 描绘梯形图的模型
/// </remarks>

namespace SamSoarII.Extend.LadderChartModel
{
    /// <summary>
    /// 方向
    /// </summary>
    enum Direction
    {
        Up, Down, Left, Right
    }
 
    public class LadderChart
    {
        /// <summary>
        /// 内部成员变量
        /// </summary>
        private List<LCNode> nodes;
        private int width;
        private int heigh;
        LCNode leupnode;
        LCNode riupnode;

        public LadderChart()
        {
            nodes = new List<LCNode>();
        }

        public List<LCNode> Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public int Heigh
        {
            get { return this.heigh; }
            set { this.heigh = value; }
        }
        public LCNode LeUpNode
        {
            get { return this.leupnode; }
            set { this.leupnode = value; }
        }
        public LCNode RiUpNode
        {
            get { return this.riupnode; }
            set { this.riupnode = value; }
        }

        /// <summary>
        /// 添加梯形图节点
        /// </summary>
        public bool Insert(LCNode newnode)
        {
            // 节点已经存在的话
            foreach (LCNode node in nodes)
            {
                if (node.X == newnode.X && node.Y == newnode.Y)
                    return false;
            }
            // 链接相邻节点
            foreach (LCNode node in nodes)
            {
                if (node.X == newnode.X-1 && node.Y == newnode.Y)
                {
                    node.Right = newnode;
                    newnode.Left = node;
                }
                if (node.X == newnode.X+1 && node.Y == newnode.Y)
                {
                    node.Left = newnode;
                    newnode.Right = node;
                }
                if (node.X == newnode.X && node.Y == newnode.Y-1)
                {
                    node.Down = newnode;
                    newnode.Up = node;
                }
                if (node.X == newnode.X && node.Y == newnode.Y+1)
                {
                    node.Up = newnode;
                    newnode.Down = node;
                }
                if (node.X == newnode.X+1 && node.Y == newnode.Y-1)
                {
                    newnode.RiUp = node;
                    node.LeDo = newnode;
                }
                if (node.X == newnode.X+1 && node.Y == newnode.Y+1)
                {
                    newnode.RiDo = node;
                    node.LeUp = newnode;
                }
                if (node.X == newnode.X-1 && node.Y == newnode.Y+1)
                {
                    node.RiUp = newnode;
                    newnode.LeDo = node;
                }
                if (node.X == newnode.X-1 && node.Y == newnode.Y-1)
                {
                    node.RiDo = newnode;
                    newnode.LeUp = node;
                }
            }
            nodes.Add(newnode);
            return true;
        }
   
        /// <summary>
        /// 删除梯形图节点
        /// </summary>
        public bool Delete(LCNode delnode)
        {
            bool isDelete = false;
            // 链接解除
            foreach (LCNode node in nodes)
            {
                if (node.X == delnode.X - 1 && node.Y == delnode.Y)
                {
                    node.Right = null;
                    delnode.Left = null;
                }
                if (node.X == delnode.X + 1 && node.Y == delnode.Y)
                {
                    node.Left = null;
                    delnode.Right = null;
                }
                if (node.X == delnode.X && node.Y == delnode.Y - 1)
                {
                    node.Down = null;
                    delnode.Up = null;
                }
                if (node.X == delnode.X && node.Y == delnode.Y + 1)
                {
                    node.Up = null;
                    delnode.Down = null;
                }
                if (node.X == delnode.X - 1 && node.Y == delnode.Y + 1)
                {
                    node.RiUp = null;
                    delnode.LeDo = null;
                }
                if (node.X == delnode.X - 1 && node.Y == delnode.Y - 1)
                {
                    node.RiDo = null;
                    delnode.LeUp = null;
                }
                if (node.X == delnode.X + 1 && node.Y == delnode.Y - 1)
                {
                    node.LeDo = null;
                    delnode.RiUp = null;
                }
                if (node.X == delnode.X + 1 && node.Y == delnode.Y + 1)
                {
                    node.LeUp = null;
                    delnode.RiDo = null;
                }
                if (node.Id == delnode.Id)
                {
                    nodes.Remove(node);
                    isDelete = true;
                }
            }
            return isDelete;
        }
        /// <summary>
        /// 获得id对应的梯形图节点
        /// </summary>
        public LCNode this[int index]
        {
            get { return nodes[index]; }
            set { nodes[index] = value; }
        }
        /// <summary>
        /// 获得节点总数
        /// </summary>
        public int Count
        { get { return nodes.Count; } }

        /// <summary>
        /// 生成逻辑图
        /// </summary>
        public LadderGraph Generate()
        {
            // 初始化元件的ID和相邻线路的信息
            int nodeid = 0;
            foreach (LCNode node in nodes)
            {
                node.Id = nodeid;
                node.LNodeID = 0;
                node.RNodeID = 0;
                nodeid++;
            }
            // 开始生成线路对应的逻辑图节点编号
            int LGVCount = 0;
            foreach (LCNode node in nodes)
            {
                // 线路左查询
                if (node.LNodeID == 0)
                {
                    //node.LNodeID = ++LGVCount;
                    LGVSearch(node, Direction.Left, ++LGVCount);
                }
                // 线路右查询
                if (node.RNodeID == 0 && node.HAccess)
                {
                    //node.RNodeID = ++LGVCount;
                    LGVSearch(node, Direction.Right, ++LGVCount);
                }
            }
            // 初始化逻辑图
            LadderGraph lgraph = new LadderGraph(this, LGVCount);
            // 添加边，每个元件对应逻辑图的边
            foreach (LCNode node in nodes)
            {
                //Console.Write("{0:d} {1:d} {2:d} {3:d} {4:d} {5:d} {6:d}\n", 
                //    node.X, node.Y, node.Type, node.HAccess, node.VAccess, node.LNodeID, node.RNodeID);
                // 添加边时需要排除线路元件
                if (!node.Type.Equals(String.Empty))
                    lgraph.InsertEdge(node, node.LNodeID, node.RNodeID);
                // 设置起点
                if (node.IsStart && node.HAccess)
                    lgraph.SetStart(node.LNodeID);
                // 设置终点
                if (node.IsTerminate && node.HAccess)
                    lgraph.SetTerminate(node.RNodeID);
            }
            return lgraph;
        }
        /// <summary>
        /// 对梯形图中的线路进行合并并标号
        /// </summary>
        private bool LGVSearch(LCNode node, Direction dir, int value)
        {
            if (node == null)
                return false;
            // 线路在元件右
            if (dir == Direction.Right)
            {
                if (node.RNodeID != 0)
                    return false;
                node.RNodeID = value;
                // 该元件水平方向可导通
                if (node.Type.Equals(String.Empty) && node.HAccess)
                    LGVSearch(node, Direction.Left, value);
                // 存在右方相邻的元件
                if (node.Right != null)
                    LGVSearch(node.Right, Direction.Left, value);
                // 存在上方相邻且可以导通的元件
                if (node.RiUp != null && node.RiUp.VAccess)
                {
                    LGVSearch(node.RiUp, Direction.Left, value);
                    if (node.Up != null)
                        LGVSearch(node.Up, Direction.Right, value);
                }
                // 存在下方相邻且可以导通的元件
                if (node.Right != null && node.Right.VAccess)
                {
                    if (node.Down != null)
                        LGVSearch(node.Down, Direction.Right, value);
                    if (node.RiDo != null)
                        LGVSearch(node.RiDo, Direction.Left, value);
                }
            }
            // 线路在元件左
            if (dir == Direction.Left)
            {
                if (node.LNodeID != 0)
                    return false;
                node.LNodeID = value;
                // 该元件水平方向可导通
                if (node.Type.Equals(String.Empty) && node.HAccess)
                    LGVSearch(node, Direction.Right, value);
                // 存在左方相邻的元件
                if (node.Left != null)
                    LGVSearch(node.Left, Direction.Right, value);
                // 存在上方相邻且可以导通的元件
                if (node.Up != null && node.Up.VAccess)
                {
                    LGVSearch(node.Up, Direction.Left, value);
                    if (node.LeUp != null)
                        LGVSearch(node.LeUp, Direction.Right, value);
                }
                // 存在下方相邻且可以导通的元件
                if (node.VAccess)
                {
                    if (node.Down != null)
                        LGVSearch(node.Down, Direction.Left, value);
                    if (node.LeDo != null)
                        LGVSearch(node.LeDo, Direction.Right, value);
                }
            }
            return true;
        }
        /// <summary>
        /// 对梯形图中的线路进行合并并标号
        /// </summary>
        public bool checkOpenCircuit()
        {
            foreach (LCNode lcnode in nodes)
            {
                // 检查左断路的基本条件
                if (lcnode.HAccess && !lcnode.IsStart && (lcnode.Left == null || !lcnode.Left.HAccess))
                    // 符合左断路条件时检查上导通
                    if (lcnode.Up == null || !lcnode.Up.VAccess)
                        // 符合左断路条件时检查下导通
                        if (lcnode.Down == null || !lcnode.VAccess)
                        {
                            //Console.Write("lopen {0:d} {1:d} {2:d} {3:d}\n", lcnode.X, lcnode.Y, lcnode.HAccess, lcnode.VAccess);
                            return true;
                        }
                // 检查右断路的基本条件
                if (lcnode.HAccess && !lcnode.IsTerminate && lcnode.Right == null)
                    // 符合右断路条件时检查右上导通
                    if (lcnode.RiUp == null || !lcnode.RiUp.VAccess)
                    {
                        //Console.Write("ropen {0:d} {1:d} {2:d} {3:d}\n", lcnode.X, lcnode.Y, lcnode.HAccess, lcnode.VAccess);
                        return true;
                    }
                // 检查下断路的基本条件
                if (lcnode.VAccess && lcnode.Down == null && (lcnode.LeDo == null || !lcnode.LeDo.HAccess))
                    // 符合下断路条件时检查左下导通
                    if (lcnode.LeDo == null || !lcnode.LeDo.HAccess)
                    {
                        //Console.Write("dopen {0:d} {1:d} {2:d} {3:d}\n", lcnode.X, lcnode.Y, lcnode.HAccess, lcnode.VAccess);
                        return true;
                    }
            }
            return false;
        }
    }
}
