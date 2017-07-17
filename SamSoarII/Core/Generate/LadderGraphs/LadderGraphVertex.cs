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
/// 逻辑图中的节点
/// </remarks>
/// 
namespace SamSoarII.Core.Generate
{
    public class LadderGraphVertex : IDisposable
    {
        public LadderGraphVertex(int _id)
        {
            id = _id;
            backedges = new List<LadderGraphEdge>();
            nextedges = new List<LadderGraphEdge>();
            expr = null;
            FlagCount = 4;
        }

        public void Dispose()
        {
            backedges.Clear();
            backedges = null;
            nextedges.Clear();
            nextedges = null;
            expr = null;
        }

        #region Numbers

        private int id;
        public int ID { get { return this.id; } }

        private bool isstart;
        public bool IsStart {
            get { return this.isstart; }
            set { this.isstart = value; }
        }

        private bool isterminate;
        public bool IsTerminate {
            get { return this.isterminate; }
            set { this.isterminate = value; }
        }

        private List<LadderGraphEdge> backedges;
        public IList<LadderGraphEdge> BackEdges { get { return this.backedges; } }

        private List<LadderGraphEdge> nextedges;
        public IList<LadderGraphEdge> NextEdges { get { return this.nextedges; } }

        private string expr;
        public string Expr
        {
            get { return this.expr; }
            set { this.expr = value; }
        }
        
        private int[] flags;
        public int this[int index]
        {
            get
            {
                return this.flags[index];
            }
            set
            {
                this.flags[index] = value;
            }
        }
        public int FlagCount
        {
            get { return this.flags.Length; }
            set { this.flags = new int[value]; }
        }

        #endregion

        /// <summary>
        /// 添加一条相邻的边，有可能是前向或者后向
        /// </summary>
        public void InsertEdge(LadderGraphEdge e)
        {
            if (e.Source.ID == id)
                nextedges.Add(e);
            if (e.Destination.ID == id)
                backedges.Add(e);
        }

        /// <summary>
        /// 生成该节点对应的表达式
        /// </summary>
        public void GenExpr()
        {
            if (expr != null) return;
            // 若为起点则表达式为1
            if (this.IsStart)
            {
                this.Expr = "1";
                return;
            }
            // 生成所有子项表达式
            List<String> subexprs = new List<String>();
            foreach (LadderGraphEdge lge in backedges)
            {
                LadderGraphVertex pv = lge.Source;
                // 若前向边指向的节点未访问（表达式为"null"），则生成表达式
                if (pv.Expr == null) pv.GenExpr();
                string uexpr = lge.Prototype.Expr;
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
                Comparison<LadderGraphEdge> sortByExpr = delegate (LadderGraphEdge v1, LadderGraphEdge v2)
                {
                    return v1.Expr.CompareTo(v2.Expr);
                };
                backedges.Sort(sortByExpr);
                foreach (LadderGraphEdge lge in BackEdges)
                {
                    subexprs.Add(lge.Expr);
                }
                // 表达式合并
                bool hasor = false;
                expr = ExprHelper.Merge(subexprs, 0, subexprs.Count - 1, ref hasor);
                if (hasor) expr = "(" + expr + ")";
            }
            // 如果存在前向边
            else if (BackEdges.Count > 0)
            {
                expr = BackEdges[0].Expr;
            }
            else
            {
                expr = "1";
            }
        }
    }
}
