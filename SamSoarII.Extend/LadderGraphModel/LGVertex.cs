using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Extend.LadderChartModel;
using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.Utility;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 逻辑图中的节点
/// </remarks>

namespace SamSoarII.Extend.LogicGraph
{
    public class LGVertex
    {
        /// <summary>
        /// 内部成员变量
        /// </summary>
        private bool enable;

        private int id;
        private bool isstart;
        private bool isterminate;
        private int flag1, flag2, flag3, flag4;

        private List<LGEdge> nedges;
        private List<LGEdge> pedges;
        private string expr;

        public LGVertex(int _id)
        {
            this.Enable = true;
            this.Id = _id;
            this.nedges = new List<LGEdge>();
            this.pedges = new List<LGEdge>();
            this.expr = "null";
        }        

        /// <summary>
        /// 该节点是否能使用
        /// </summary>
        public bool Enable
        {
            get { return this.enable; }
            set { this.enable = value; }
        }

        /// <summary>
        /// 该节点的标号
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// 该节点是否为起点
        /// </summary>
        public bool IsStart
        {
            get { return this.isstart; }
            set { this.isstart = value; }
        }

        /// <summary>
        /// 该节点是否为终点
        /// </summary>
        public bool IsTerminate
        {
            get { return this.isterminate; }
            set { this.isterminate = value; }
        }

        /// <summary>
        /// 根据下标返回或设置相应的值
        /// 0：指令类型
        /// 1,2,3,4：指令的操作数和信息
        /// </summary>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return 0;
                    case 1: return this.flag1;
                    case 2: return this.flag2;
                    case 3: return this.flag3;
                    case 4: return this.flag4;
                    default: return 0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: break;
                    case 1: this.flag1 = value; break;
                    case 2: this.flag2 = value; break;
                    case 3: this.flag3 = value; break;
                    case 4: this.flag4 = value; break;
                }
            }
        }
        /// <summary>
        /// 该节点对应的逻辑表达式
        /// </summary>
        public string Expr
        {
            get { return this.expr; }
            set { this.expr = value; }
        }
        /// <summary>
        /// 该节点的前向边集合
        /// </summary>
        public List<LGEdge> Edges
        {
            get { return this.nedges; }
        }
        /// <summary>
        /// 该节点的后向边集合
        /// </summary>
        public List<LGEdge> BackEdges
        {
            get { return this.pedges; }
        }
        /// <summary>
        /// 添加一条相邻的边，有可能是前向或者后向
        /// </summary>
        public void InsertEdge(LGEdge e)
        {
            if (e.Source.Id == this.Id)
                nedges.Add(e);
            if (e.Destination.Id == this.Id)
                pedges.Add(e);
        }

        /// <summary>
        /// 生成该节点对应的表达式
        /// </summary>
        public void GenExpr()
        {
            if (!this.Expr.Equals("null")) return;
            // 若为起点则表达式为1
            if (this.IsStart)
            {
                this.Expr = "1"; return;
            }
            // 生成所有子项表达式
            List<String> subexprs = new List<String>();
            foreach (LGEdge lge in BackEdges)
            {
                LGVertex pv = lge.Source;
                // 若前向边指向的节点未访问（表达式为"null"），则生成表达式
                if (pv.Expr.Equals("null"))
                    pv.GenExpr();
                string uexpr = lge.PLCInfo.Expr;
                // 若与运算两边的表达式其中为“1”，则忽略这个“1”
                if (uexpr.Equals("1"))
                    lge.Expr = pv.Expr;
                else if (pv.Expr.Equals("1"))
                    lge.Expr = uexpr;
                else
                    lge.Expr = String.Format("{0:s}&&{1:s}", pv.Expr, uexpr);
            }
            // 如果有多个表达式需要或运算
            if (BackEdges.Count > 1)
            {
                // 按照ASCII码来排序，为后续合并工作做准备
                Comparison<LGEdge> sortByExpr = delegate (LGEdge v1, LGEdge v2)
                {
                    return v1.Expr.CompareTo(v2.Expr);
                };
                BackEdges.Sort(sortByExpr);
                foreach (LGEdge lge in BackEdges)
                {
                    subexprs.Add(lge.Expr);
                }
                // 表达式合并
                bool hasor = false;
                Expr = ExprHelper.Merge(subexprs, 0, subexprs.Count - 1, ref hasor);
                if (hasor) Expr = "(" + Expr + ")";
            }
            // 如果存在前向边
            else if (BackEdges.Count > 0)
            {
                this.Expr = BackEdges[0].Expr;
            }
            else
            {
                this.Expr = "1";
            }
        }
    }
}
