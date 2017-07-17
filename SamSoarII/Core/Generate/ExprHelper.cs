using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public abstract class ExprHelper
    {
        /// <summary>
        /// 终点坐标
        /// </summary>
        public class TPoint
        {
            public bool enable;
            public LadderChartNode PLCUnit;
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
        public const int FLAG_HASOR = 0x01;
        /// <summary>
        /// 是否已经检验过没有&&了
        /// </summary>
        public const int FLAG_HASAND = 0x02;
        /// <summary>
        /// 是否要将所得的值和前面进行或运算
        /// </summary>
        public const int FLAG_CALOR = 0x04;
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
        static public string Merge(List<string> exprs, int left, int right, ref bool hasor, int padding = 0)
        {
            hasor = false;
            bool _hasor = false;
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
                for (i = left + 1; i <= right; i++)
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
            StringBuilder expr = new StringBuilder(String.Empty);
            // CASE 1 : 若未找到新的公共前缀
            if (old_padding >= padding)
            {
                // 表达式集合用分歧点分为左右两部分，分治处理后合并
                expr.Append(Merge(exprs, left, difpoint - 1, ref _hasor, padding));
                expr.Append("||");
                expr.Append(Merge(exprs, difpoint, right, ref _hasor, padding));
                hasor = true;
                return expr.ToString();
            }
            // CASE 2 : 找到新的公共前缀时，按照分配律，剩下的部分用括号括起来
            expr.Append(exprs[left].Substring(old_padding, padding - old_padding + 1));
            expr.Append("(");
            expr.Append(Merge(exprs, left, difpoint - 1, ref _hasor, padding + 1));
            expr.Append("||");
            expr.Append(Merge(exprs, difpoint, right, ref _hasor, padding + 1));
            expr.Append(")");
            return expr.ToString();
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
        static public List<PLCInstruction> GenInst(List<LadderGraphVertex> _terminates)
        {
            List<Terminate> terminates = new List<Terminate>();     // 将要处理包含关系，最后的结构放在这里
            Terminate term = new Terminate();                       // 前后包含关系放在一个单元内
            TPoint tpoint = new TPoint();                           // 这里构造第一个点 
            tpoint.enable = true;
            tpoint.PLCUnit = _terminates[0].BackEdges[0].Prototype;
            tpoint.ExprIndex = _terminates[0].Expr.Length - 1;
            term.TPoints.Add(tpoint);
            for (int i = 1; i < _terminates.Count; i++)
            {
                LadderGraphVertex lgv1 = _terminates[i - 1];
                LadderGraphVertex lgv2 = _terminates[i];
                tpoint = new TPoint();
                tpoint.enable = true;
                tpoint.PLCUnit = lgv2.BackEdges[0].Prototype;
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
            term.Expr = _terminates.Last<LadderGraphVertex>().Expr;
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
        static private void _GenInst(List<PLCInstruction> insts, List<Terminate> terminates, int left, int right, int padding = 0, int flag = 0)
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
             && insts.Count() > 0 && !insts.Last().Type.Equals("LBL") && !insts.Last().Type.Equals("NEXT"))
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
        static private void _GenInst(List<PLCInstruction> insts, string expr, int start, int end, int flag = 0)
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
            if ((flag & FLAG_CALOR) != 0)
                profix = "OR";
            // 前面要与运算，所以用与前缀
            if ((flag & FLAG_CALAND) != 0)
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
                if (expr[start + 1] == 'i' && expr[start + 2] == 'm')
                {
                    InstHelper.AddInst(insts, profix + "IIM " + expr.Substring(start + 3, end - start - 2), id);
                    return;
                }
                // 一般的非符号( !M0 )
                InstHelper.AddInst(insts, profix + "I " + expr.Substring(start + 1, end - start), id);
                return;
            }
            // 识别上升沿符号（ueM0）
            if (expr[start] == 'u' && expr[start + 1] == 'e')
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
            if (expr[start] == 'd' && expr[start + 1] == 'e')
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
            if (expr[start] == 'i' && expr[start + 1] == 'm')
            {
                InstHelper.AddInst(insts, profix + "IM " + expr.Substring(start + 2, end - start - 1), id);
                return;
            }
            // 比较表达式的长度都不小于6
            if (end - start > 4)
            {
                if (profix.Equals("AND"))
                    profix = "A";
                // 找到比较符的位置
                int op = start + 2;
                while (expr[op] != '=' && expr[op] != '<' && expr[op] != '>') op++;
                // 识别比较符前的数据类型
                switch (expr[op - 1])
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
                if (expr[op] == '<' && expr[op + 1] == '>')
                {
                    InstHelper.AddInst(insts, profix + "NE " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 2, end - op - 1), id);
                    return;
                }
                // 小等比较（M0w<=M1）
                if (expr[op] == '<' && expr[op + 1] == '=')
                {
                    InstHelper.AddInst(insts, profix + "LE " + expr.Substring(start, op - 1 - start) + " " + expr.Substring(op + 2, end - op - 1), id);
                    return;
                }
                // 大等比较（M0w>=M1）
                if (expr[op] == '>' && expr[op + 1] == '=')
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
        private static string _InstToExpr(int left, int right, int stackTop = 1)
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
    }
}
