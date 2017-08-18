using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ClassName : LadderChartNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 描绘梯形图的模型
/// </remarks>

namespace SamSoarII.Core.Generate
{
    public class LadderChart : IDisposable
    {
        #region Resources
        
        /// <summary> /// 方向 </summary>
        private enum Direction
        {
            Up, Down, Left, Right
        }
        #endregion
        
        public LadderChart()
        {
            nodes = new List<LadderChartNode>();
            nodedict = new GridDictionary<LadderChartNode>(12);
        }

        public LadderChart(LadderNetworkModel lnmodel)
        {
            nodes = new List<LadderChartNode>();
            nodedict = new GridDictionary<LadderChartNode>(12);
            // 按照位置坐标来排序
            Comparison<LadderUnitModel> sortByPosition = delegate (LadderUnitModel v1, LadderUnitModel v2)
            {
                int v1_X = v1.X;
                int v1_Y = v1.Y;
                int v2_X = v2.X;
                int v2_Y = v2.Y;
                if (v1.Type == LadderUnitModel.Types.VLINE)
                    v1_X++;
                if (v2.Type == LadderUnitModel.Types.VLINE)
                    v2_X++;
                if (v1_Y < v2_Y || v1_Y == v2_Y && v1_X < v2_X)
                    return -1;
                if (v1_Y > v2_Y || v1_Y == v2_Y && v1_X > v2_X)
                    return 1;
                if (v1.Type != LadderUnitModel.Types.VLINE
                 && v2.Type == LadderUnitModel.Types.VLINE)
                    return -1;
                if (v1.Type == LadderUnitModel.Types.VLINE
                 && v2.Type != LadderUnitModel.Types.VLINE)
                    return 1;
                return 0;
            };
            List<LadderUnitModel> units = lnmodel.Children.Concat(lnmodel.VLines).ToList();
            units.Sort(sortByPosition);

            LadderChartNode current = null;
            foreach (LadderUnitModel unit in units)
            {
                int unitX = unit.X;
                int unitY = unit.Y;
                if (unit.Type == LadderUnitModel.Types.VLINE) unitX++;
                if (current == null || current.X != unitX || current.Y != unitY)
                {
                    current = new LadderChartNode(unit);
                    Insert(current);
                }
                else if (unit.Type == LadderUnitModel.Types.VLINE)
                {
                    current.VAccess = true;
                }
            }
        }

        public void Dispose()
        {
            foreach (LadderChartNode node in nodes)
            {
                node.Dispose();
            }
            nodes.Clear();
            nodes = null;
            nodedict.Clear();
            nodedict = null;
        }

        /// <summary>
        /// 内部成员变量
        /// </summary>
        private List<LadderChartNode> nodes;
        private GridDictionary<LadderChartNode> nodedict;
        private int width;
        private int height;
        
        public List<LadderChartNode> Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        /// <summary>
        /// 添加梯形图节点
        /// </summary>
        public bool Insert(LadderChartNode newnode)
        {
            LadderChartNode node = null;
            // 节点已经存在的话
            node = nodedict[newnode.X, newnode.Y];
            if (node != null)
                return false;
            // 链接相邻节点
            node = nodedict[newnode.X - 1, newnode.Y];
            if (node != default(LadderChartNode))
            {
                node.Right = newnode;
                newnode.Left = node;
            }
            node = nodedict[newnode.X + 1, newnode.Y];
            if (node != default(LadderChartNode))
            {
                node.Left = newnode;
                newnode.Right = node;
            }
            node = nodedict[newnode.X, newnode.Y - 1];
            if (node != default(LadderChartNode))
            {
                node.Down = newnode;
                newnode.Up = node;
            }
            node = nodedict[newnode.X, newnode.Y + 1];
            if (node != default(LadderChartNode))
            {
                node.Up = newnode;
                newnode.Down = node;
            }
            node = nodedict[newnode.X + 1, newnode.Y + 1];
            if (node != default(LadderChartNode))
            {
                node.LeUp = newnode;
                newnode.RiDo = node;
            }
            node = nodedict[newnode.X + 1, newnode.Y - 1];
            if (node != default(LadderChartNode))
            {
                node.LeDo = newnode;
                newnode.RiUp = node;
            }
            node = nodedict[newnode.X - 1, newnode.Y - 1];
            if (node != default(LadderChartNode))
            {
                node.RiDo = newnode;
                newnode.LeUp = node;
            }
            node = nodedict[newnode.X - 1, newnode.Y + 1];
            if (node != default(LadderChartNode))
            {
                node.RiUp = newnode;
                newnode.LeDo = node;
            }
            nodes.Add(newnode);
            nodedict[newnode.X, newnode.Y] = newnode;
            return true;
        }

        /// <summary>
        /// 删除梯形图节点
        /// </summary>
        public bool Delete(LadderChartNode delnode)
        {
            // 不存在
            if (nodedict[delnode.X, delnode.Y] != delnode)
                return false;
            // 链接解除
            if (delnode.Left != null)
                delnode.Left.Right = null;
            if (delnode.Right != null)
                delnode.Right.Left = null;
            if (delnode.Up != null)
                delnode.Up.Down = null;
            if (delnode.Down != null)
                delnode.Down.Up = null;
            if (delnode.RiDo != null)
                delnode.RiDo.LeUp = null;
            if (delnode.RiUp != null)
                delnode.RiUp.LeDo = null;
            if (delnode.LeDo != null)
                delnode.LeDo.RiUp = null;
            if (delnode.LeUp != null)
                delnode.LeUp.RiDo = null;
            nodes.Remove(delnode);
            nodedict[delnode.X, delnode.Y] = default(LadderChartNode);
            return true;
        }
        /// <summary>
        /// 获得id对应的梯形图节点
        /// </summary>
        public LadderChartNode this[int index]
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
        public LadderGraph ToGraph()
        {
            // 初始化元件的ID和相邻线路的信息
            int nodeid = 0;
            foreach (LadderChartNode node in nodes)
            {
                node.ID = nodeid;
                node.LNodeID = 0;
                node.RNodeID = 0;
                nodeid++;
            }
            // 开始生成线路对应的逻辑图节点编号
            int LGVCount = 0;
            foreach (LadderChartNode node in nodes)
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
            foreach (LadderChartNode node in nodes)
            {
                //Console.Write("{0:d} {1:d} {2:d} {3:d} {4:d} {5:d} {6:d}\n", 
                //    node.X, node.Y, node.Type, node.HAccess, node.VAccess, node.LNodeID, node.RNodeID);
                // 添加边时需要排除线路元件
                if (!node.Type.Equals("HLINE") && !node.Type.Equals("VLINE"))
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
        private bool LGVSearch(LadderChartNode node, Direction dir, int value)
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
                if (node.Type.Equals("HLINE") && node.HAccess)
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
                if (node.Type.Equals("HLINE") && node.HAccess)
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
        /// 检查梯形图是否存在开路，并对梯形图中的线路进行合并并标号
        /// </summary>
        public bool CheckOpenCircuit()
        {
            // 整个梯形图为空
            if (nodes.Count() == 0) return false;
            // 不存在起点
            if (nodes.Where(n => n.IsStart).Count() == 0) return true;
            // 不存在终点
            if (nodes.Where(n => n.IsTerminate).Count() == 0) return true;
            foreach (LadderChartNode node in nodes)
            {
                // 检查左断路的基本条件
                if (node.HAccess && !node.IsStart && (node.Left == null || !node.Left.HAccess))
                    // 符合左断路条件时检查上导通
                    if (node.Up == null || !node.Up.VAccess)
                        // 符合左断路条件时检查下导通
                        if (node.Down == null || !node.VAccess)
                            return true;
                // 检查右断路的基本条件
                if (node.HAccess && !node.IsTerminate && node.Right == null)
                    // 符合右断路条件时检查右上导通
                    if (node.RiUp == null || !node.RiUp.VAccess)
                        return true;
                // 检查下断路的基本条件
                if (node.VAccess && node.Down == null && (node.LeDo == null || !node.LeDo.HAccess))
                    // 符合下断路条件时检查左下导通
                    if (node.LeDo == null || !node.LeDo.HAccess)
                        return true;
            }
            return false;
        }
    }
}
