using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.LadderChartModel;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 和表达式操作相关的辅助类
/// </remarks>

namespace SamSoarII.Extend.Utility
{
    public class ExprHelper
    {
        /// <summary>
        /// 终点坐标
        /// </summary>
        public class TPoint
        {
            public bool enable;
            public LCNode PLCUnit;
            public int ExprIndex;
        }
        /// <summary>
        /// 终点单元
        /// </summary>
        public class Terminate
        {
            public string Expr;
            public List<TPoint> TPoints;
            public Terminate()
            {
                TPoints = new List<TPoint>();
            }
        }
        /// <summary>
        /// 是否已经检验过没有||了
        /// </summary>
        public const int FLAG_HASOR  = 0x01;
        /// <summary>
        /// 是否已经检验过没有&&了
        /// </summary>
        public const int FLAG_HASAND = 0x02;
        /// <summary>
        /// 是否要将所得的值和前面进行或运算
        /// </summary>
        public const int FLAG_CALOR  = 0x04;
        /// <summary>
        /// 是否要将所得的值和前面进行与运算
        /// </summary>
        public const int FLAG_CALAND = 0x08;
        /// <summary>
        /// 将区间内的多个表达式进行逻辑或合并
        /// </summary>
        /// <param name="exprs">要合并的表达式列表</param>
        /// <param name="left">区间下标</param>
        /// <param name="right">区间上标</param>
        /// <param name="padding">当前检查过的最大公共前缀的坐标</param>
        /// <returns>合并后的表达式</returns>
        /// <detail>
        /// 前提是所有表达式已经排好序，否则算法无法继续进行
        /// 对于给定区间内的所有表达式，前缀标记前面的部分已经处理，需要将后面的部分进行或合并
        /// 找到最大公共前缀后面一位的分歧点，为了优化性能，最好是找靠近中间的
        /// 前面的部分的形式为A = A1 && A2 && A3 ... Am
        /// 后面的部分都不一样，分别设为B1, B2, ... Bn
        /// 根据分配律，最后合并的表达式为expr = A && (B1 || B2 ... || Bn)
        /// </detail>
        static public string Merge(List<string> exprs, int left, int right, int padding = 0)
        {
            int i;
            // 只有一个表达式
            if (left == right)
            {
                return exprs[left].Substring(padding);
            }
            // 扩展公共前缀，并记录离中心最近的分歧点
            bool allequal = true;
            int difpoint = -1; // <= 分歧点
            int old_padding = padding;
            while (allequal)
            {
                for (i = left+1; i <= right; i++)
                {
                    if (exprs[i][padding] != exprs[left][padding])
                    {
                        allequal = false;
                        // 若比当前分歧点更优则替换掉
                        if (Math.Abs(i * 2 - (left + right)) < Math.Abs(difpoint * 2 - (left + right)))
                            difpoint = i;
                    }
                }
                if (allequal == true)
                    padding++;
            }
            // 向前修正前缀末尾为and计算符
            while (padding > old_padding && exprs[left][padding] != '&') padding--;
            //Console.Write("next padding = {0:d}\n", padding);
            string expr = "";
            // CASE 1 : 若未找到新的公共前缀
            if (old_padding >= padding)
            {
                // 表达式集合用分歧点分为左右两部分，分治处理后合并
                expr = Merge(exprs, left, difpoint - 1, padding);
                expr += "||";
                expr += Merge(exprs, difpoint, right, padding);
                return expr;
            }
            // CASE 2 : 找到新的公共前缀时，按照分配律，剩下的部分用括号括起来
            expr = exprs[left].Substring(old_padding, padding-old_padding+1) + "(";
            expr += Merge(exprs, left, difpoint - 1, padding + 1);
            expr += "||";
            expr += Merge(exprs, difpoint, right, padding + 1);
            return expr + ")";
        }
        /// <summary>
        /// 通过逻辑表达式来生成PLC指令
        /// </summary>
        /// <param name="expr">表达式</param>
        /// <returns>最终的PLC指令列表</returns>
        static public List<PLCInstruction> GenInst(string expr)
        {
            List<PLCInstruction> insts = new List<PLCInstruction>();
            _GenInst(insts, expr, 0, expr.Length - 1);
            return insts;
        }
        /// <summary>
        /// 通过区间内给定的逻辑图终点来生成PLC指令
        /// 要求所有终点的表达式已经生成并排序
        /// </summary>
        /// <param name="terminates">终点列表</param>
        /// <returns>最终的PLC指令列表</returns>
        /// <detail> 
        /// 这里需要讲包含关系的表达式集合并到一起
        /// 用特殊的数据结构来表示
        /// </detail>
        static public List<PLCInstruction> GenInst(List<LGVertex> _terminates)
        {
            List<Terminate> terminates = new List<Terminate>();     // 将要处理包含关系，最后的结构放在这里
            Terminate term = new Terminate();                       // 前后包含关系放在一个单元内
            TPoint tpoint = new TPoint();                           // 这里构造第一个点 
            tpoint.enable = true;
            tpoint.PLCUnit = _terminates[0].BackEdges[0].PLCInfo;
            tpoint.ExprIndex = _terminates[0].Expr.Length-1;
            term.TPoints.Add(tpoint);
            for (int i = 1; i < _terminates.Count; i++)
            {
                LGVertex lgv1 = _terminates[i - 1];
                LGVertex lgv2 = _terminates[i];
                tpoint = new TPoint();
                tpoint.enable = true;
                tpoint.PLCUnit = lgv2.BackEdges[0].PLCInfo;
                tpoint.ExprIndex = lgv2.Expr.Length - 1;
                // 每次对相邻的两个表达式，后面的表达式截为相同长度后进行比较，若相等可归为一类
                if (lgv1.Expr.Length <= lgv2.Expr.Length && lgv1.Expr.Equals(lgv2.Expr.Substring(0, lgv1.Expr.Length)))
                {
                    term.TPoints.Add(tpoint);
                }
                else
                // 不为包含关系，上一个单元结束，设置最终表达式并添加到列表结构中
                {
                    term.Expr = lgv1.Expr;
                    terminates.Add(term);
                    term = new Terminate();
                    term.TPoints = new List<TPoint>();
                    term.TPoints.Add(tpoint);
                }
            }
            // 最后一个单元添加到表里
            term.Expr = _terminates.Last<LGVertex>().Expr;
            terminates.Add(term);
            // 调用内部方法
            List<PLCInstruction> insts = new List<PLCInstruction>();
            _GenInst(insts, terminates, 0, terminates.Count - 1);
            return insts;
        }
        /// <summary>
        /// 通过区间内给定的逻辑图终点来生成PLC指令
        /// </summary>
        /// <param name="terminates">终点列表</param>
        /// <param name="left">区间头</param>
        /// <param name="right">区间尾</param>
        /// <param name="padding">访问过的最大前缀</param>
        /// <param name="flag">标记</param>
        /// <detail>
        /// 这↑里↓和表达式合并的想法一样，所以也需要将表达式排序
        /// 不同的是，这里要解决的是辅助栈的分配问题，所以需要找到所有分歧点
        /// 然后根据数量来确定辅助栈的操作
        /// </detail>
        static private void _GenInst(List<PLCInstruction> insts, List<Terminate> terminates, int left, int right, int padding=0, int flag=0)
        {
            int i, j;
            // 只有一个终点时，需要对这个终点单元进行解析
            // 一般来讲，这个单元的表达式被里面的坐标集合分为多段
            // 需要对每一段分别进行计算和执行终点指令
            if (left == right)
            {
                Terminate term = terminates[left];
                // 生成第一段
                _GenInst(insts, term.Expr, padding, term.TPoints[0].ExprIndex, flag);
                if (term.TPoints[0].enable)
                    term.TPoints[0].PLCUnit.GenInst(insts, flag);
                for (i = 1; i < term.TPoints.Count; i++)
                {
                    // 依次生成每一段
                    if (term.TPoints[i - 1].ExprIndex + 2 < term.TPoints[i].ExprIndex)
                        _GenInst(insts, term.Expr, term.TPoints[i - 1].ExprIndex + 3, term.TPoints[i].ExprIndex, flag | FLAG_CALAND);
                    if (term.TPoints[i].enable)
                        term.TPoints[i].PLCUnit.GenInst(insts, flag);
                }
            }
            else
            {
                // 扩展公共前缀
                bool allequal = true;                  // 判断所有表达式在当前位是否相等
                //bool allinside = true;               // 判断当前位是否至少被一个表达式包含
                List<int> difpoints = new List<int>(); // 分歧点队列
                int old_padding = padding;
                int andpos = padding;                  // 这里需要记录最后的与运算符的位置
                /*
                 * 开始查找下一个分歧点
                 * 注意当前检查的位置超过其中一个表达式的长度时，两个表达式比较时视为相等
                */
                while (allequal)
                {
                    for (i = left + 1; i <= right; i++)
                    {
                        string expr1 = terminates[i - 1].Expr;
                        string expr2 = terminates[i].Expr;
                        if (expr1[padding] != expr2[padding])
                        {
                            allequal = false;
                            break;
                        }
                    }
                    if (allequal == true)
                        padding++;
                }
                // 向前修正前缀末尾为and计算符
                while (padding > old_padding && terminates[left].Expr[padding] != '&') padding--;
                // 然后向后查找所有的分歧点
                for (i = left + 1; i <= right; i++)
                {
                    string expr1 = terminates[i - 1].Expr;
                    string expr2 = terminates[i].Expr;
                    /*
                     * 两个表达式都到达末尾的情况下
                     * 由于表达式的末尾都是原型不同的常量1，所以无需比较默认相同
                     */
                    bool isend1 = !expr1.Substring(padding + 1).Contains('&');
                    bool isend2 = !expr2.Substring(padding + 1).Contains('&');
                    if (isend1 && isend2) continue;
                    // 向后查找直到到达and运算符为止
                    for (j = padding + 1; j < expr1.Length && j < expr2.Length; j++)
                    {
                        if (!expr1[j].Equals(expr2[j])) break;
                        if (expr1[j] == '&') break;
                    }
                    /* 若不相同则为分歧点
                     * 当下标超过某个表达式的长度时，不视为分歧点
                     */
                    if (j < expr1.Length && j < expr2.Length && !expr1[j].Equals(expr2[j]))
                    {
                        difpoints.Add(i);
                    }
                }
                // CASE 1 : 若未找到新的公共前缀
                if (old_padding >= padding)
                {
                    // 如果不存在分歧点，直接输出所有结果指令
                    if (difpoints.Count() == 0)
                    {
                        for (i = left; i <= right; i++)
                        {
                            _GenInst(insts, terminates, i, i, padding, flag);
                        }
                    }
                    else
                    {
                        // 根据分歧点将所有终点进行划分，然后分别处理
                        _GenInst(insts, terminates, left, difpoints[0] - 1, padding, flag);
                        for (i = 1; i < difpoints.Count; i++)
                        {
                            _GenInst(insts, terminates, difpoints[i - 1], difpoints[i] - 1, padding, flag);
                        }
                        _GenInst(insts, terminates, difpoints[i - 1], right, padding, flag);
                    }
                    // 分割成子块来处理，所以不需要POP
                    return;
                }
                // CASE 2 : 找到新的公共前缀时，需要将前缀内的结果用辅助栈存起来
                // 根据分歧点将所有终点进行划分，然后分别处理
                // 并根据分歧点的数量来决定辅助栈的操作
                else
                {
                    //_GenInst(insts, terminates[left].Expr, old_padding, padding - 2, flag);
                    /*
                     * 如果更新的这一段前缀中包含其中一些终点的坐标
                     * 如果不考虑这些坐标，相应的终点就会被忽略
                     * 需要分以下步骤来解决这个问题
                     *      1. 将所有坐标升序排序
                     *      2. 将这段前缀按照坐标进行分段，然后每一段算出结果后执行终点指令
                     */
                    // 查询所有符合条件的tpoint
                    List<TPoint> tpoints = new List<TPoint>();
                    for (i = left; i <= right; i++)
                        foreach (TPoint tp in terminates[i].TPoints)
                        {
                            if (tp.ExprIndex >= old_padding && tp.ExprIndex <= padding - 2)
                                tpoints.Add(tp);
                        }
                    // 如果存在
                    if (tpoints.Count > 0)
                    {
                        // 所有找到的坐标进行排序
                        tpoints.Sort();
                        // 计算第一个坐标前面的值，并运行对应的第一个终点指令
                        _GenInst(insts, terminates[left].Expr, old_padding, tpoints[0].ExprIndex, flag);
                        tpoints[0].PLCUnit.GenInst(insts, flag);
                        tpoints[0].enable = false;
                        for (i = 1; i < tpoints.Count; i++)
                        {
                            // 依次计算坐标间隔的表达式的值，并执行终点指令
                            _GenInst(insts, terminates[left].Expr, tpoints[i - 1].ExprIndex + 3, tpoints[i].ExprIndex, flag | FLAG_CALAND);
                            tpoints[i].PLCUnit.GenInst(insts, flag);
                            tpoints[i].enable = false;
                        }
                        // 计算最后一个坐标后到找到的前缀之间的表达式的值
                        _GenInst(insts, terminates[left].Expr, tpoints[i - 1].ExprIndex + 3, padding - 2, flag | FLAG_CALAND);
                    }
                    // 如果没有
                    else
                    {
                        // 直接计算当前前缀的表达式
                        _GenInst(insts, terminates[left].Expr, old_padding, padding - 2, flag);
                    }
                    // 如果不存在分歧点，直接输出所有结果指令
                    if (difpoints.Count() == 0)
                    {
                        for (i = left; i <= right; i++)
                        {
                            _GenInst(insts, terminates, i, i, padding + 1, flag | FLAG_CALAND);
                        }
                    }
                    // 以下为涉及到辅助栈的PLC代码生成
                    else
                    {
                        // 将计算结果暂存进辅助栈
                        InstHelper.AddInst(insts, "MPS");
                        // 生成第一组终点的PLC指令程序
                        _GenInst(insts, terminates, left, difpoints[0] - 1, padding + 1, flag | FLAG_CALAND);
                        for (i = 1; i < difpoints.Count; i++)
                        {
                            // 中间结果从辅助栈中取出来，恢复现场
                            InstHelper.AddInst(insts, "MRD");
                            // 依次生成下一组
                            _GenInst(insts, terminates, difpoints[i - 1], difpoints[i] - 1, padding + 1, flag | FLAG_CALAND);
                        }
                        // 最后一次用到辅助栈，所以直接弹出
                        InstHelper.AddInst(insts, "MPP");
                        // 生成最后一组
                        _GenInst(insts, terminates, difpoints[i - 1], right, padding + 1, flag | FLAG_CALAND);
                    }
                }
            }
            /*
             * 前面没有与运算和或运算的链接，这时需要将栈顶弹出
             * 要注意一些特殊的标志指令，这些指令不涉及到栈的操作
             */
            if ((flag & FLAG_CALAND) == 0 && (flag & FLAG_CALOR) == 0
             && insts.Count() > 0  && !insts.Last().Type.Equals("LBL") && !insts.Last().Type.Equals("NEXT"))
            {
                InstHelper.AddInst(insts, "POP");
            }
        }
        /// <summary>
        /// 内部生成PLC指令的方法
        /// </summary>
        /// <param name="insts">生成的PLC指令的列表</param>
        /// <param name="expr">表达式</param>
        /// <param name="start">表达式的开始位置</param>
        /// <param name="end">表达式的结束位置</param>
        /// <param name="flag">标记</param>
        static private void _GenInst(List<PLCInstruction> insts, string expr, int start, int end, int flag=0)
        {
            if (start > end) return;
            //Console.WriteLine(expr.Substring(start, end - start + 1));
            int bracket = 0;
            // 当前单元的结尾
            int uend = end;
            // CASE 1：查询并处理表达式中的或运算符（优先级最高）
            if ((flag & FLAG_HASOR) == 0)
            {
                bracket = 0;
                while (uend >= start && (bracket > 0 || expr[uend] != '|'))
                {
                    if (expr[uend] == '(') bracket--;
                    if (expr[uend] == ')') bracket++;
                    uend--;
                }
                if (uend >= start && expr[uend] == '|')
                {
                    // 如果前面需要与合并，可以拆掉括号逐一运算
                    if ((flag & FLAG_CALOR) != 0)
                    {
                        _GenInst(insts, expr, start, uend - 2, FLAG_CALOR);
                        _GenInst(insts, expr, uend + 1, end, FLAG_HASOR | FLAG_CALOR);
                    }
                    else
                    {
                        // 先计算或运算符前的部分
                        _GenInst(insts, expr, start, uend - 2);
                        // 将或运算符前的部分和后面进行或运算
                        _GenInst(insts, expr, uend + 1, end, FLAG_HASOR | FLAG_CALOR);
                        // 需要和前面与运算时
                        if ((flag & FLAG_CALAND) != 0)
                        {
                            InstHelper.AddInst(insts, "ANDB");
                        }
                    }
                    return;
                }
            }
            // CASE 2：查询并处理表达式中的与运算符（优先级其次）
            if ((flag & FLAG_HASAND) == 0)
            {
                bracket = 0;
                uend = end;
                while (uend >= start && (bracket > 0 || expr[uend] != '&'))
                {
                    if (expr[uend] == '(') bracket--;
                    if (expr[uend] == ')') bracket++;
                    uend--;
                }
                if (uend >= start && expr[uend] == '&')
                {
                    // 如果前面需要与合并，可以拆掉括号逐一运算
                    if ((flag & FLAG_CALAND) != 0)
                    {
                        _GenInst(insts, expr, start, uend - 2, FLAG_HASOR | FLAG_CALAND);
                        _GenInst(insts, expr, uend + 1, end, FLAG_HASOR | FLAG_HASAND | FLAG_CALAND);
                    }
                    else
                    {
                        // 先计算与运算符前的部分
                        _GenInst(insts, expr, start, uend - 2, FLAG_HASOR);
                        // 将与运算符前的部分和后面进行与运算
                        _GenInst(insts, expr, uend + 1, end, FLAG_HASOR | FLAG_HASAND | FLAG_CALAND);
                        // 需要和前面或运算时
                        if ((flag & FLAG_CALOR) != 0)
                        {
                            InstHelper.AddInst(insts, "ORB");
                        }
                    }
                    return;
                }
            }
            // CASE 3：当前为单一单元
            // 当前表达式被括号包围时拆掉括号
            if (expr[start] == '(' && expr[end] == ')')
            {
                _GenInst(insts, expr, start + 1, end - 1);
                // 前面要或运算，所以要用栈的或合并
                if ((flag & FLAG_CALOR) != 0)
                    InstHelper.AddInst(insts, "ORB");
                // 前面要与运算，所以要用栈的与合并
                if ((flag & FLAG_CALAND) != 0)
                    InstHelper.AddInst(insts, "ANDB");
                return;
            }
            string profix = "LD";
            // 前面要或运算，所以用或前缀
            if ((flag&FLAG_CALOR) != 0)
                profix = "OR";
            // 前面要与运算，所以用与前缀
            if ((flag&FLAG_CALAND) != 0)
                profix = "AND";
            // 对表达式进行解析
            // 识别开始的元件ID标记
            int idend = start + 1;
            while (expr[idend] != ']') idend++;
            int id = int.Parse(expr.Substring(start + 1, idend - start - 1));
            start = idend + 1;
            // 识别开始的非符号
            if (expr[start] == '!')
            {
                // 如果是取反运算( ! )
                if (end == start)
                {
                    InstHelper.AddInst(insts, "INV", id);
                    return;
                }
                // 识别非符号后面的立即符号( !imM0 )
                if (expr[start+1] == 'i' && expr[start+2] == 'm')
                {
                    InstHelper.AddInst(insts, profix + "IIM " + expr.Substring(start + 3, end - start - 2), id);
                    return;
                }
                // 一般的非符号( !M0 )
                InstHelper.AddInst(insts, profix + "I " + expr.Substring(start + 1, end - start), id);
                return;
            }
            // 识别上升沿符号（ueM0）
            if (expr[start] == 'u' && expr[start+1] == 'e')
            {
                // 如果是取上升沿运算( ue )
                if (end == start + 1)
                {
                    InstHelper.AddInst(insts, "MEP", id);
                    return;
                }
                InstHelper.AddInst(insts, profix + "P " + expr.Substring(start + 2, end - start - 1), id);
                return;
            }
            // 识别下降沿符号（deM0）
            if (expr[start] == 'd' && expr[start+1] == 'e')
            {
                // 如果是取下降沿运算( de )
                if (end == start + 1)
                {
                    InstHelper.AddInst(insts, "MEF", id);
                    return;
                }
                InstHelper.AddInst(insts, profix + "F " + expr.Substring(start + 2, end - start - 1), id);
                return;
            }
            // 识别立即符号（imM0）
            if (expr[start] == 'i' && expr[start+1] == 'm')
            {
                InstHelper.AddInst(insts, profix + "IM " + expr.Substring(start + 2, end - start - 1), id);
                return;
            }
            // 比较表达式的长度都不小于6
            if (end-start > 4)
            {
                if (profix.Equals("AND"))
                    profix = "A";
                // 找到比较符的位置
                int op = start + 2;
                while (expr[op] != '=' && expr[op] != '<' && expr[op] != '>') op++;
                // 识别比较符前的数据类型
                switch (expr[op-1])
                {
                    case 'w': profix += 'W'; break;
                    case 'd': profix += 'D'; break;
                    case 'f': profix += 'F'; break;
                }
                // 等比较（M0w=M1）
                if (expr[op] == '=')
                {
                    InstHelper.AddInst(insts, profix + "EQ " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 1, end - op), id);
                    return;
                }
                // 不等比较（M0w<>M1）
                if (expr[op] == '<' && expr[op+1] == '>')
                {
                    InstHelper.AddInst(insts, profix + "NE " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 2, end - op - 1), id);
                    return;
                }
                // 小等比较（M0w<=M1）
                if (expr[op] == '<' && expr[op+1] == '=')
                {
                    InstHelper.AddInst(insts, profix + "LE " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 2, end - op - 1), id);
                    return;
                }
                // 大等比较（M0w>=M1）
                if (expr[op] == '>' && expr[op+1] == '=')
                {
                    InstHelper.AddInst(insts, profix + "GE " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 2, end - op - 1), id);
                    return;
                }
                // 小于比较（M0w<M1）
                if (expr[op] == '<')
                {
                    InstHelper.AddInst(insts, profix + "L " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 1, end - op), id);
                    return;
                }
                // 大于比较（M0w>M1）
                if (expr[op] == '>')
                {
                    InstHelper.AddInst(insts, profix + "G " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 1, end - op), id);
                    return;
                }
            }
            // 单一的位寄存器（M0）
            string _expr = expr.Substring(start, end - start + 1);
            if (_expr.Equals("1")) return;
            InstHelper.AddInst(insts, profix + " " + _expr, id);
        }
        /// <summary>
        /// 暂存在类中的静态的PLC指令列表
        /// </summary>
        private static List<PLCInstruction> insts;
        /// <summary>
        /// 每条指令对应的当前栈的长度
        /// </summary>
        private static int[] instStackCount;
        /// <summary>
        /// 每条指令对应的辅助栈的长度
        /// </summary>
        //private static int[] instMStackCount;
        /// <summary>
        /// 将PLC指令列表指定区间内转换为表达式（未考虑辅助栈）
        /// </summary>
        /// <param name="_insts">PLC指令列表</param>
        /// <param name="left">区间左</param>
        /// <param name="right">区间右</param>
        /// <returns>表达式</returns>
        public static string InstToExpr(List<PLCInstruction> _insts, int left, int right)
        {
            // 暂存起来给内部方法用
            insts = _insts;
            // 建立当前栈长列表
            instStackCount = new int[insts.Count];
            int lastStackCount = 0;
            for (int i = 0; i < insts.Count; i++)
            {
                // 这个指令是LD类的，栈顶加1元素
                if (insts[i].Text.Substring(0, 2).Equals("LD"))
                    instStackCount[i] = lastStackCount + 1;
                // 这个指令是对栈顶两元素进行或合并或者与合并，栈顶减1元素
                else if (insts[i].Text.Substring(0, 3).Equals("ORB") ||
                         insts[i].Text.Substring(0, 3).Equals("ANB"))
                    instStackCount[i] = lastStackCount - 1;
                // 其他指令不涉及到栈的改变
                else
                    instStackCount[i] = lastStackCount;
                lastStackCount = instStackCount[i];
            }
            return _InstToExpr(left, right);
        }
        /// <summary>
        /// 内部调用的表达式转换方法（未考虑辅助栈）
        /// </summary>
        /// <param name="left">区间左</param>
        /// <param name="right">区间右</param>
        /// <param name="stackTop">当前栈顶</param>
        /// <returns>表达式</returns>
        private static string _InstToExpr(int left, int right,int stackTop = 1)
        {
            // 表达式的前缀
            string profix = "";
            // 表达式的后缀
            string suffix = "";
            /* 
             * 将未改变栈长的指令转换成表达式的后缀依次累加，并从区间中删除
             * 例如：LD M0, LD M1, ANB, AND M2, OR M3
             * 后面的AND M2, OR M3没有改变栈的大小转换为后缀&&M2||M3
             * 前面的部分交给之后处理
             * 要注意的是或运算的优先级问题，保证优先级靠后需要将前面的部分加上括号
             * 例如AND M2, OR M3，运算顺序实际上是(expr&&M2)||M3
             * 所以在前缀后面加上'('，后缀前面加上')'表示已经加了括号
             */
            while (right >= left && instStackCount[right] == stackTop)
            {
                suffix = InstHelper.ToExpr(insts[right]);
                // 或运算加括号
                if (insts[right].Text.Substring(0, 2).Equals("OR"))
                {
                    profix = profix + "(";
                    suffix = ")" + suffix;
                }
                right--;
            }
            /*
             * 如果区间为空，分为两种情况：
             * 1. 区间一开始已经为空，说明给定的指令是空的。
             * 2. 检查完没有进栈出栈操作的指令后为空，说明所有指令都没有涉及到改变栈长的操作，全部转换为前缀和后缀。
             */
            if (left > right) return profix + suffix;
            /*
             * 若区间末尾为合并指令ANB,ORB，则之前已经准备好了两个栈元素，进行这一次计算
             * 所以整个指令列表分为三个部分：
             * [计算第一个元素]A[计算第二个元素]B[将栈顶两个元素计算合并]C
             * 可以证明，在A处和C处的栈长是相等的，而在B处的栈长为A处和C处+1
             * 找到A处，将区间分为两部分，左右部分分治处理后合并
             */
            int mid = right++;
            while (instStackCount[mid] > stackTop) mid--;
            string lexpr = _InstToExpr(left, mid, stackTop);
            string rexpr = _InstToExpr(mid + 1, right - 1, stackTop + 1);
            // 栈顶合并运算是或运算时，可以保证优先级最后
            if (insts[right].Text.Substring(0, 3).Equals("ORB"))
                return profix + lexpr + "||" + rexpr + suffix;
            // 与运算需要左右加括号确保优先级
            if (insts[right].Text.Substring(0, 3).Equals("ANB"))
                return profix + "(" + lexpr + ")&&(" + rexpr + ")" + suffix;
            // 不是合并运算？
            throw new ArgumentException();
        }


        static public void InstToCCode(StreamWriter sw, string funcName, List<PLCInstruction> insts)
        {
            sw.Write("\nvoid {0:s}()\n{\n", funcName);
            for (int i = 0; i < insts.Count; i++)
            {
                InstHelper.InstToCCode(sw, insts[i]);
            }
            sw.Write("}\n");
        }
        
        /// <summary>
        /// 将内部的表达式转换为C语言可识别的格式
        /// </summary>
        /// <param name="expr">原表达式</param>
        /// <returns>C表达式</returns>
        /// <detail>
        /// 
        /// </detail>
        static public string ConvertToCStyle(string expr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据给定的表达式还原逻辑图
        /// </summary>
        /// <param name="expr">输入的表达式</param>
        /// <param name="lgvstart">给定逻辑图起点（默认为空则新建一个）</param>
        /// <param name="lgvend">给定逻辑图终点（默认为空则新建一个）</param>
        /// <returns>输出的逻辑图，带有梯度图绑定</returns>
        static public LGraph GenLGraph(string expr)
        {
            lgvtop = 0;
            return _GenLGraph(expr);
        }
        /// <summary>
        /// 当前最大的逻辑图节点编号
        /// </summary>
        static private int lgvtop = 0;
        /// <summary>
        /// 内部的表达式转逻辑图方法
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="lgvstart"></param>
        /// <param name="lgvend"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        static private LGraph _GenLGraph(string expr, LGVertex lgvstart = null, LGVertex lgvend = null, int flag = 0)
        {
            int estart = 0, eend = expr.Length - 1;
            LadderChart lchart = new LadderChart();
            LGraph lgraph = new LGraph();
            lgraph.LChart = lchart;
            // 若起始点未给定则添加
            if (lgvstart == null)
                lgvstart = new LGVertex(++lgvtop);
            // 若结束点未给定则添加
            if (lgvend == null)
                lgvend = new LGVertex(++lgvtop);
            //Console.WriteLine(expr.Substring(start, end - start + 1));
            // 当前表达式被括号包围时拆掉括号
            int bracket = 0;
            while (expr[estart] == '(' && expr[eend] == ')')
            {
                estart++;
                eend--;
            }
            // 当前单元的结尾
            int uend = estart;
            // CASE 1：查询并处理表达式中的或运算符（优先级最高）
            if ((flag & FLAG_HASOR) == 0)
            {
                bracket = 0;
                while (uend <= eend && (bracket > 0 || expr[uend] != '|'))
                {
                    if (expr[uend] == '(') bracket++;
                    if (expr[uend] == ')') bracket--;
                    uend++;
                }
                if (uend <= eend && expr[uend] == '|')
                {
                    // 先生成或运算符前的部分的逻辑图
                    LGraph lg1 = _GenLGraph(expr.Substring(0, uend), lgvstart, lgvend, flag | FLAG_HASOR);
                    // 再生成或运算符后的部分的逻辑图
                    LGraph lg2 = _GenLGraph(expr.Substring(uend + 2), lgvstart, lgvend, flag);
                    // 合并两个逻辑图
                    lgraph.Vertexs = lg1.Vertexs.Union(lg2.Vertexs).ToList();
                    lgraph.Starts = lg1.Starts;
                    lgraph.Terminates = lg1.Terminates;
                    lgraph.Edges = lg1.Edges.Concat(lg2.Edges).ToList();
                    // 合并两个梯形图
                    /*
                     * 因为两个字表达式属于或的关系，所以要在梯形图上体现出来并联的结构
                     * 合并梯形图时，要注意以下几点
                     * 1. 将对应的梯形图上下对接，并且首尾相连
                     * 2. 如果两个梯形图的宽度不一致，另一个要额外向前设立线路，然后再对接
                     */
                    // 两个相邻的梯形图元件
                    LCNode lcn1 = null, lcn2 = null;
                    foreach (LCNode lcn3 in lg2.LChart.Nodes)
                    {
                        // 编号不能相同，所以要加上梯形图1的元件数量，建立新的编号域
                        lcn3.Id += lg1.LChart.Nodes.Count();
                        // 梯形图2放在梯形图1的下面，所以Y坐标要加上梯形图1的高度
                        lcn3.Y += lg1.LChart.Heigh;
                    }
                    // 建立合并后的新的梯形图，先将元件集合并
                    lchart.Nodes = lg1.LChart.Nodes.Union(lg2.LChart.Nodes).ToList();
                    // 宽度取两者最大，高度取两者相加
                    lchart.Width = Math.Max(lg1.LChart.Width, lg2.LChart.Width);
                    lchart.Heigh = lg1.LChart.Heigh + lg2.LChart.Heigh;
                    // 找到梯形图1的左端的最下元件
                    lcn1 = lg1.LChart.LeUpNode;
                    while (lcn1.Down != null)
                        lcn1 = lcn1.Down;
                    /*
                     * 将两个梯形图的左端对接
                     * 需要将上面的部分向下设立新的路线，并到达下面的左端
                     */
                    // 从该元件开始向下铺路
                    lcn1.VAccess = true;
                    for (int y = lcn1.Y + 1; y <= lg1.LChart.Heigh; y++)
                    {
                        // 新建下面的空元件
                        lcn2 = new LCNode(lchart.Nodes.Count() + 1);
                        lcn2.X = lcn1.X;
                        lcn2.Y = y;
                        lcn2.HAccess = false;
                        // 将向下联通设为true，能通到下面
                        lcn2.VAccess = true;
                        lcn1.Down = lcn2;
                        lcn2.Up = lcn1;
                        lchart.Nodes.Add(lcn2);
                        // 完成连接后，当前元件移动到下面
                        lcn1 = lcn2;
                    }
                    // 将铺路后最下面的元件和梯形图2的左上角对接
                    lcn2 = lg2.LChart.LeUpNode;
                    lcn1.Down = lcn2;
                    lcn2.Up = lcn1;
                    /*
                     * 将两个梯形图的右端对接
                     * 需要将上面的部分向下设立新的路线，并到达下面的右端
                     */
                    // 如果梯形图1的宽度小于梯形图2
                    /*
                     * 先将图1的右上角拓宽，延伸到图2一样的宽度
                     * 然后再向下连接，和图2的右上角相连
                     */
                    if (lg1.LChart.Width < lg2.LChart.Width)
                    {
                        lcn1 = lg1.LChart.RiUpNode;
                        // 向右连接
                        for (int x = lg1.LChart.Width + 1; x <= lg2.LChart.Width; x++)
                        {
                            lcn2 = new LCNode(lchart.Nodes.Count() + 1);
                            lcn2.X = x;
                            lcn2.Y = lcn1.Y;
                            lcn2.HAccess = true;
                            lcn2.VAccess = false;
                            lcn1.Right = lcn2;
                            lcn2.Left = lcn1;
                            lchart.Nodes.Add(lcn2);
                            lcn1 = lcn2;
                        }
                        // 设置图1新的右上角
                        lg1.LChart.RiUpNode = lcn1;
                    } 
                    else
                    // 如果梯形图1的宽度大于梯形图2
                    /*
                     * 将图2的右上角进行扩展，与图1的宽度一致
                     * 然后图1向下连接，直到到达图2新扩展的右下角
                     */
                    if (lg1.LChart.Width > lg2.LChart.Width)
                    {
                        lcn1 = lg2.LChart.RiUpNode;
                        // 向右连接
                        for (int x = lg2.LChart.Width + 1; x <= lg1.LChart.Width; x++)
                        {
                            lcn2 = new LCNode(lchart.Nodes.Count() + 1);
                            lcn2.X = x;
                            lcn2.Y = lcn1.Y;
                            lcn2.HAccess = true;
                            lcn2.VAccess = false;
                            lcn1.Right = lcn2;
                            lcn2.Left = lcn1;
                            lchart.Nodes.Add(lcn2);
                            lcn1 = lcn2;
                        }
                        // 设置图2新的右上角
                        lg2.LChart.RiUpNode = lcn1;
                    }
                    /*
                     * 经过上面的调整，这个时候图1和图2的宽度一致
                     * 但还是要考虑两个梯形图最右端的元件，
                     * 要注意元件是左端向下连接的，这样连接的可能会是右端的前一个线路
                     * --[]-----[]-----[]--
                     *              |
                     *              |
                     * --[]-----[]-----[]--
                     * 像这种右端存在非空元件时，会连接前面的部分
                     * 所以要将梯形图1再向右扩展一格，右上角向右再连一条向下的线路
                     * 这样保证连接的是最右端
                     * --[]-----[]-----[]--- [right]
                     *                     |
                     *                     |
                     * --[]-----[]-----[]---
                     */
                    // 如果两个梯形图其中之一的右端元件非空
                    if (!lg1.LChart.RiUpNode.Type.Equals(String.Empty) &&
                        !lg2.LChart.RiUpNode.Type.Equals(String.Empty))
                    {
                        // 向右添加线路
                        lcn1 = lg1.LChart.RiUpNode;
                        lcn2 = new LCNode(lchart.Nodes.Count() + 1);
                        lcn2.X = lcn1.X + 1;
                        lcn2.Y = lcn1.Y;
                        lcn2.HAccess = false;
                        lcn2.VAccess = true;
                        lcn1.Right = lcn2;
                        lcn2.Left = lcn1;
                        lchart.Nodes.Add(lcn2);
                        lg1.LChart.RiUpNode = lcn2;
                    }
                    lcn1 = lg1.LChart.RiUpNode;
                    while (lcn1.Down != null)
                        lcn1 = lcn1.Down;
                    lcn1.VAccess = true;
                    // 向下连接
                    for (int y = lcn1.Y + 1; y <= lg1.LChart.Heigh; y++)
                    {
                        lcn2 = new LCNode(lchart.Nodes.Count() + 1);
                        lcn2.X = lcn1.X;
                        lcn2.Y = y;
                        lcn2.HAccess = false;
                        lcn2.VAccess = true;
                        lcn1.Down = lcn2;
                        lcn2.Up = lcn1;
                        lchart.Nodes.Add(lcn2);
                        lcn1 = lcn2;
                    }
                    // 最下端和图2的右上角连接
                    lcn2 = lg2.LChart.RiUpNode;
                    lcn1.Down = lcn2;
                    lcn2.Up = lcn1;
                    // 设置左上角和右上角
                    lchart.LeUpNode = lg1.LChart.LeUpNode;
                    lchart.RiUpNode = lg1.LChart.RiUpNode;

                    return lgraph;
                }
            }
            // CASE 2：查询并处理表达式中的与运算符（优先级其次）
            if ((flag & FLAG_HASAND) == 0)
            {
                bracket = 0;
                uend = estart;
                while (uend <= eend && (bracket > 0 || expr[uend] != '&'))
                {
                    if (expr[uend] == '(') bracket++;
                    if (expr[uend] == ')') bracket--;
                    uend++;
                }
                if (uend <= eend && expr[uend] == '&')
                {
                    // 添加中间点
                    LGVertex lgvmidium = new LGVertex(++lgvtop);
                    // 先生成或运算符前的部分的逻辑图
                    LGraph lg1 = _GenLGraph(expr.Substring(0, uend), lgvstart, lgvmidium, flag | FLAG_HASOR);
                    // 再生成或运算符后的部分的逻辑图
                    LGraph lg2 = _GenLGraph(expr.Substring(uend + 2), lgvmidium, lgvend, flag);
                    // 合并两个逻辑图
                    lgraph.Vertexs = lg1.Vertexs.Union(lg2.Vertexs).ToList();
                    lgraph.Starts = lg1.Starts;
                    lgraph.Terminates = lg2.Terminates;
                    lgraph.Edges = lg1.Edges.Concat(lg2.Edges).ToList();
                    // 图2移动到图1的右端并合并，所以编号加上图1的总数，X坐标加上图1的宽度
                    foreach (LCNode lcn3 in lg2.LChart.Nodes)
                    {
                        lcn3.Id += lg1.LChart.Nodes.Count();
                        lcn3.X += lg1.LChart.Width;
                    }
                    // 合并梯度图
                    lchart.Nodes = lg1.LChart.Nodes.Union(lg2.LChart.Nodes).ToList();
                    lchart.Width = lg1.LChart.Width + lg2.LChart.Width;
                    lchart.Heigh = Math.Max(lg1.LChart.Width, lg2.LChart.Width);
                    lchart.LeUpNode = lg1.LChart.LeUpNode;
                    lchart.RiUpNode = lg2.LChart.RiUpNode;
                    // 图1和图2首尾相接
                    lg1.LChart.RiUpNode.Right = lg2.LChart.LeUpNode;
                    lg2.LChart.LeUpNode.Left = lg1.LChart.RiUpNode;
                    // 图1需要向右连接
                    lg1.LChart.RiUpNode.HAccess = true;
                    return lgraph;
                }
            }
            // CASE 3：当前为单一单元
            // 新建一个新的逻辑图
            lgraph.Starts.Add(lgvstart);
            lgraph.Terminates.Add(lgvend);
            lgraph.Vertexs.Add(lgvstart);
            lgraph.Vertexs.Add(lgvend);
            LCNode lcn = new LCNode(0);
            LGEdge lge = new LGEdge(lcn, lgvstart, lgvend);
            lgvstart.Edges.Add(lge);
            lgvend.BackEdges.Add(lge);
            lgraph.Edges.Add(lge);
            // 新建一个梯度图
            lchart.Width = lchart.Heigh = 1;
            lchart.Insert(lcn);
            // 对表达式进行解析
            // 函数调用形式表示的功能指令（func(a,b,c)）
            if (expr[eend] == ')')
            {
                // 找到括号的起始位置和终止位置
                int ebstart = estart;
                int ebend = eend;
                while (expr[ebstart] != '(') ebstart++;
                // 得到函数名称和参数集合
                string fname = expr.Substring(estart, ebstart - estart);
                string[] fargs = expr.Substring(ebstart + 1, ebend - ebstart - 1).Split(',');
                // 转换为LadderChart模式
                lcn.Type = fname;
                if (fargs.Length > 1)
                    lcn[1] = fargs[0];
                if (fargs.Length > 2)
                    lcn[2] = fargs[1];
                if (fargs.Length > 3)
                    lcn[3] = fargs[2];
                if (fargs.Length > 4)
                    lcn[4] = fargs[3];
                if (fargs.Length > 5)
                    lcn[5] = fargs[4];
                return lgraph;
            }
            // 识别开始的非符号
            if (expr[estart] == '!')
            {
                // 识别非符号后面的立即符号( !imM0 )
                if (expr[estart + 1] == 'i' && expr[estart + 2] == 'm')
                {
                    lcn.Type = "LDIIM";
                    lcn[1] = expr.Substring(estart + 3, eend - estart - 2);
                    return lgraph;
                }
                // 一般的非符号( !M0 )
                lcn.Type = "LDI";
                lcn[1] = expr.Substring(estart + 1, eend - estart);
                return lgraph;
            }
            // 识别上升沿符号（ueM0）
            if (expr[estart] == 'u' && expr[estart + 1] == 'e')
            {
                lcn.Type = "LDP";
                lcn[1] = expr.Substring(estart + 2, eend - estart - 1);
                return lgraph;
            }
            // 识别下降沿符号（deM0）
            if (expr[estart] == 'd' && expr[estart + 1] == 'e')
            {
                lcn.Type = "LDF";
                lcn[1] = expr.Substring(estart + 2, eend - estart - 1);
                return lgraph;
            }
            // 识别立即符号（imM0）
            if (expr[estart] == 'i' && expr[estart + 1] == 'm')
            {
                lcn.Type = "LDIM";
                lcn[1] = expr.Substring(estart + 2, eend - estart - 1);
                return lgraph;
            }
            // 比较表达式的长度都不小于6
            if (eend - estart > 4)
            {
                // 找到比较符的位置
                int op = estart + 2;
                while (expr[op] != '=' && expr[op] != '<' && expr[op] != '>') op++;
                // 识别比较符前的数据类型
                /*
                int datatype = 0;
                switch (expr[op - 1])
                {
                    case 'w': datatype = 1; break;
                    case 'd': datatype = 2; break;
                    case 'f': datatype = 3; break;
                }
                */
                // 等比较（M0w=M1）
                if (expr[op] == '=')
                {
                    switch (expr[op - 1])
                    {
                        case 'w': lcn.Type = "LDWEQ"; break;
                        case 'd': lcn.Type = "LDDEQ"; break;
                        case 'f': lcn.Type = "LDFEQ"; break;
                    }
                    lcn[1] = expr.Substring(estart, op - 1 - estart);
                    lcn[2] = expr.Substring(op + 1, eend - op);
                    return lgraph;
                }
                // 不等比较（M0w<>M1）
                if (expr[op] == '<' && expr[op + 1] == '>')
                {
                    switch (expr[op - 1])
                    {
                        case 'w': lcn.Type = "LDWNE"; break;
                        case 'd': lcn.Type = "LDDNE"; break;
                        case 'f': lcn.Type = "LDFNE"; break;
                    }
                    lcn[1] = expr.Substring(estart, op - 1 - estart);
                    lcn[2] = expr.Substring(op + 2, eend - op - 1);
                    return lgraph;
                }
                // 小等比较（M0w<=M1）
                if (expr[op] == '<' && expr[op + 1] == '=')
                {
                    if (expr[op] == '<' && expr[op + 1] == '>')
                    {
                        switch (expr[op - 1])
                        {
                            case 'w': lcn.Type = "LDWLE"; break;
                            case 'd': lcn.Type = "LDDLE"; break;
                            case 'f': lcn.Type = "LDFLE"; break;
                        }
                        lcn[1] = expr.Substring(estart, op - 1 - estart);
                        lcn[2] = expr.Substring(op + 2, eend - op - 1);
                        return lgraph;
                    }
                    return lgraph;
                }
                // 大等比较（M0w>=M1）
                if (expr[op] == '>' && expr[op + 1] == '=')
                {
                    if (expr[op] == '<' && expr[op + 1] == '>')
                    {
                        switch (expr[op - 1])
                        {
                            case 'w': lcn.Type = "LDWGE"; break;
                            case 'd': lcn.Type = "LDDGE"; break;
                            case 'f': lcn.Type = "LDFGE"; break;
                        }
                        lcn[1] = expr.Substring(estart, op - 1 - estart);
                        lcn[2] = expr.Substring(op + 2, eend - op - 1);
                        return lgraph;
                    }
                    return lgraph;
                }
                // 小于比较（M0w<M1）
                if (expr[op] == '<')
                {
                    switch (expr[op - 1])
                    {
                        case 'w': lcn.Type = "LDWL"; break;
                        case 'd': lcn.Type = "LDDL"; break;
                        case 'f': lcn.Type = "LDFL"; break;
                    }
                    lcn[1] = expr.Substring(estart, op - 1 - estart);
                    lcn[2] = expr.Substring(op + 1, eend - op);
                    return lgraph;
                }
                // 大于比较（M0w>M1）
                if (expr[op] == '>')
                {
                    switch (expr[op - 1])
                    {
                        case 'w': lcn.Type = "LDWG"; break;
                        case 'd': lcn.Type = "LDDG"; break;
                        case 'f': lcn.Type = "LDFG"; break;
                    }
                    lcn[1] = expr.Substring(estart, op - 1 - estart);
                    lcn[2] = expr.Substring(op + 1, eend - op);
                    return lgraph;
                }
            }
            // 读取位（M0）
            lcn.Type = "LD";
            lcn[1] = expr.Substring(estart, eend - estart + 1);
            return lgraph;
        }
        
    }
}
