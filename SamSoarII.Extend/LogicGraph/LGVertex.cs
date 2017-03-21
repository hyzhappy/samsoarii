﻿using System;
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
            if (!this.Expr.Equals("null"))
                return;
            //Console.Write("expr begin {0:d}, {1:s}\n", Id, Expr);
            // 若为起点则表达式为1
            if (this.IsStart)
            {
                this.Expr = "1";
                //Console.Write("expr end {0:d}, {1:s}\n", Id, Expr);
                return;
            }
            // 生成所有子项表达式
            List<String> subexprs = new List<String>();
            foreach (LGEdge lge in pedges)
            {
                LGVertex pv = lge.Source;
                // 若前向边指向的节点未访问（表达式为"null"），则生成表达式
                if (pv.Expr.Equals("null"))
                    pv.GenExpr();
                string uexpr = lge.PLCInfo.Expr;
                // 若与运算两边的表达式其中为“1”，则忽略这个“1”
                if (uexpr.Equals("1"))
                    subexprs.Add(pv.Expr);
                else if (pv.Expr.Equals("1"))
                    subexprs.Add(uexpr);
                else
                    subexprs.Add(String.Format("{0:s}&&{1:s}", pv.Expr, uexpr));
            }
            // 如果有多个表达式需要或运算
            if (subexprs.Count > 1)
            {
                // 排序，为后续合并工作做准备
                subexprs.Sort();
                // 表达式合并
                this.Expr = "(" + ExprHelper.Merge(subexprs, 0, subexprs.Count - 1) + ")";
            } 
            else
            {
                this.Expr = subexprs[0];
            }
            //Console.Write("expr end {0:d}, {1:s}\n", Id, Expr);
        }
        /// <summary>
        /// 生成从该节点出发到达所有终点的PLC指令
        /// </summary>
        public void GenInst(List<PLCInstruction> insts, int flags=0)
        {
            // 到达终点可跳出
            if (this.IsTerminate)
            {
                return;
            }
            // 结果节点不影响栈内容，不需要开销辅助栈
            // 找到所有指向的结果节点，忽略掉辅助栈的操作
            // 剩下的节点数量若不大于一，表示不存在分支，也不需要辅助栈来维护
            int ncount = 0;
            for (int i = 0; i < nedges.Count; i++)
            {
                LGEdge lge = nedges[i];
                if (lge.Destination.IsTerminate)
                    continue;
                ncount++;
            }
            // 当前节点存在分支，则需要辅助栈来维护
            if (ncount > 1)
            {
                // 当前值压入辅助栈
                InstHelper.AddInst(insts, "MPS");
                for (int i = 0; i < nedges.Count; i++)
                {
                    LGEdge lge = nedges[i];
                    // 执行当前边对应的PLC指令
                    lge.PLCInfo.GenInst(insts, flags | 0x04);
                    // 生成子树的指令
                    lge.Destination.GenInst(insts, flags);
                    // 结果节点忽略辅助栈
                    if (lge.Destination.IsTerminate)
                        continue;
                    // 还原现场，需要从辅助栈中取出暂存的值
                    if (--ncount > 1)
                        InstHelper.AddInst(insts, "MRD");
                    // 若暂存值不会再用了，从辅助栈中弹出
                    if (ncount == 1)
                        InstHelper.AddInst(insts, "MPP");
                }
            }
            else
            {
                // 同上，但不需要辅助栈的支持
                // 但是需要先处理结果节点
                for (int i = 0; i < nedges.Count; i++)
                {
                    LGEdge lge = nedges[i];
                    if (!lge.Destination.IsTerminate)
                        continue;
                    lge.PLCInfo.GenInst(insts, flags | 0x04);
                    lge.Destination.GenInst(insts, flags);
                }
                // 然后再处理非结果节点
                for (int i = 0; i < nedges.Count; i++)
                {
                    LGEdge lge = nedges[i];
                    if (lge.Destination.IsTerminate)
                        continue;
                    lge.PLCInfo.GenInst(insts, flags | 0x04);
                    lge.Destination.GenInst(insts, flags);
                }
            }
        }
    }
}
