using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public abstract class InstHelper
    {
        /// <summary>
        /// 根据指令文本构造指令结构，并加入列表中
        /// </summary>
        /// <param name="insts">指令列表</param>
        /// <param name="inst">指令文本</param>
        static public void AddInst(List<PLCInstruction> insts, string text, int id = -1)
        {
            PLCInstruction inst = new PLCInstruction(text);
            inst.PrototypeID = id;
            insts.Add(inst);
        }
        /// <summary>
        /// 将指令转换为表达式
        /// </summary>
        /// <param name="inst">指令结构</param>
        /// <returns>表达式</returns>
        static public string ToExpr(PLCInstruction inst)
        {
            throw new NotImplementedException();
        }
        /// <summary>已经使用的全局变量数量</summary>
        static private int globalCount = 0;
        /// <summary>当前栈顶</summary>
        static private int stackTop = 0;
        /// <summary>辅助栈栈顶</summary>
        static private int mstackTop = 0;
        /// <summary>所有计数器的预设值sv</summary>
        static private string[] ctsv = new string[256];
        /// <summary>栈中元素与前一元素的合并方式（INV，MEP，MEF）</summary>
        static private string[] stackcalcs = new string[256];
        /// <summary>STL状态</summary>
        static private string stlvalue = null;
        /// <summary>表示PLC的NETWORK channel的类型结构</summary>
        public class PLCInstNetwork
        {
            public string Name;
            public int ID;
            public PLCInstruction[] Insts;
            public PLCInstNetwork(string name, PLCInstruction[] insts)
            {
                Name = name;
                Insts = insts;
            }
        }
        /// <summary>
        /// 将给定的PLC代码转换为仿真程序
        /// </summary>
        /// <param name="sw">文件输出流</param>
        /// <param name="networks">PLC代码的NETWORK集</param>
        static public void InstToSimuCode(StreamWriter sw, PLCInstNetwork[] networks)
        {
            sw.Write("#include <stdint.h>\r\n");
            sw.Write("#include \"simulib.h\"\r\n");
            sw.Write("#include \"simuf.h\"\r\n");
            sw.Write("#include \"simuc.h\"\r\n\r\n");
            _InstToCCode(sw, networks, true);
        }
        /// <summary>
        /// 将给定的PLC代码转换为下位程序
        /// </summary>
        /// <param name="sw">文件输出流</param>
        /// <param name="networks">PLC代码的NETWORK集</param>
        static public void InstToDownCode(StreamWriter sw, PLCInstNetwork[] networks)
        {
            sw.Write("#include <stdint.h>\r\n");
            sw.Write("#include \"downlib.h\"\n");
            sw.Write("#include \"downf.h\"\r\n");
            sw.Write("#include \"downc.h\"\n");
            _InstToCCode(sw, networks, false);
        }
        /// <summary>
        /// 将给定的PLC代码转换为C语言代码并输出到文件
        /// </summary>
        /// <param name="sw">文件输出流</param>
        /// <param name="networks">PLC代码的NETWORK集</param>
        static private void _InstToCCode(StreamWriter sw, PLCInstNetwork[] networks, bool simumode = false)
        {
            // 构建指令结构的列表
            List<PLCInstruction> insts = new List<PLCInstruction>();
            // NETWORK合并
            PLCInstNetwork net1 = null;
            PLCInstNetwork net2 = null;
            for (int i = 0; i < networks.Length; i++)
            {
                net2 = networks[i];
                // 当前网络的组名和前一个不同，则设为函数名的声明
                if (net1 != null && !net1.Name.Equals(net2.Name))
                {
                    insts.Add(new PLCInstruction("FUNC " + net2.Name));
                }
                // 将NETWORK的代码合并
                foreach (PLCInstruction inst in net2.Insts)
                {
                    insts.Add(inst);
                }
                // 每个NETWORK结束后弹出最后的栈内容
                //if (net2.Insts.Count() > 1)
                //    insts.Add(new PLCInstruction("POP"));
                net1 = net2;
            }
            stackTop = 0;
            mstackTop = 0;
            globalCount = 0;
            int stackTotal = 0;
            int mstackTotal = 0;
            int globalTotal = 0;
            Stack<PLCInstruction> stackinsts = new Stack<PLCInstruction>();
            for (int i = 0; i < ctsv.Length; i++) ctsv[i] = "0";
            foreach (PLCInstruction inst in insts)
            {
                // 计算普通栈和辅助栈的栈顶
                if (inst[0].Length > 1 && inst[0].Substring(0, 2) == "LD")
                {
                    stackTop++;
                    stackinsts.Push(inst);
                }
                if (inst[0].Equals("ANDB") || inst[0].Equals("ORB") || inst[0].Equals("POP"))
                {
                    stackTop--;
                    PLCInstruction topinst = stackinsts.Pop();
                    topinst.StackCalc = inst[0];
                }
                if (inst[0].Equals("MPS"))
                    mstackTop++;
                if (inst[0].Equals("MPP"))
                    mstackTop--;
                // 记录两个栈到达的最大高度
                if (stackTop > stackTotal)
                    stackTotal = stackTop;
                if (mstackTop > mstackTotal)
                    mstackTotal = mstackTop;
                // 统计所有需要全局变量的指令的总数
                switch (inst[0])
                {
                    case "LDP":
                    case "LDF":
                    case "ANDP":
                    case "ANDF":
                    case "ORP":
                    case "ORF":
                    case "ALTP":
                    case "CTU":
                    case "CTD":
                    case "CTUD":
                    case "INV":
                        globalTotal++;
                        break;
                    case "TON":
                    case "TOF":
                    case "TONR":
                        if (simumode) globalTotal += 4;
                        break;
                    case "FOR":
                        globalTotal += 2;
                        break;
                    case "MEP":
                    case "MEF":
                        globalTotal += 2;
                        break;
                }
                // 找到所有计数器的预设值
                if (inst[0].Length >= 2 && inst[0].Substring(0, 2).Equals("CT"))
                    ctsv[int.Parse(inst[4])] = inst[2];
            }
            // 建立C代码的全局环境
            //sw.Write("#include \"lib.h\"\n");
            //sw.Write("#include \"main.h\"\n\n");
            //sw.Write("static uint16_t _stack[256];\n");         // 数据栈
            //sw.Write("static uint16_t _stacktop;\n");           // 数据栈的栈顶
            //sw.Write("static uint16_t _mstack[256];\n");        // 辅助栈
            //sw.Write("static uint16_t _mstacktop;\n");          // 辅助栈的栈顶
            user_id = 0;
            sw.Write("static int8_t _global[{0:d}];\n", globalTotal); // 全局变量
            // 仿真模式下计算当前信号
            if (simumode) sw.Write("static int8_t _signal;\n");
            // 先声明所有的子函数
            foreach (PLCInstruction inst in insts)
            {
                if (inst[0].Equals("FUNC"))
                    sw.Write("void {0:s}();\n", inst[1]);
            }
            // 建立扫描的主函数
            bool ismain = true;
            sw.Write("void RunLadder()\n{\n");
            if (simumode) sw.Write("_itr_invoke();\n");
            //if (!simumode) sw.Write("if (MBit[8150]) InitUserRegisters();\n");
            // STL取消标志
            sw.Write("int8_t _stlrst;\n");
            // 建立局部的栈和辅助栈
            for (int i = 1; i <= stackTotal; i++)
                sw.Write("int8_t _stack_{0:d};\n", i);
            for (int i = 1; i <= mstackTotal; i++)
                sw.Write("int8_t _mstack_{0:d};\n", i);
            // 生成PLC对应的内容
            // 初始化栈顶
            stackTop = 0;
            mstackTop = 0;
            foreach (PLCInstruction inst in insts)
            {
                switch (inst[0])
                {
                    // 函数头部
                    case "FUNC":
                        if (simumode && !ismain)
                            sw.Write("callleave();\n");
                        sw.Write("}\n\n");
                        ismain = false;
                        sw.Write("void {0:s}()", inst[1]);
                        sw.Write("{\n");
                        if (simumode)
                            sw.Write("callinto();\n");
                        // STL取消标志
                        sw.Write("int8_t _stlrst;\n");
                        // 建立局部的栈和辅助栈
                        for (int i = 1; i <= stackTotal; i++)
                            sw.Write("int8_t _stack_{0:d};\n", i);
                        for (int i = 1; i <= mstackTotal; i++)
                            sw.Write("int8_t _mstack_{0:d};\n", i);
                        // 初始化栈顶
                        stackTop = 0;
                        mstackTop = 0;
                        break;
                    default:
                        InstToCCode(sw, inst, simumode);
                        break;
                }
            }
            if (simumode && !ismain)
                sw.Write("callleave();\n");
            sw.Write("}\n");
        }
        /// <summary>
        /// 将给定的指令结构转换为对应的C语言代码
        /// </summary>
        /// <param name="sw">输出的C文件</param>
        /// <param name="inst">指令结构</param>
        /// <param name="simumode">是否是仿真模式</param>
        static private int user_id = 0;
        static public void InstToCCode(StreamWriter sw, PLCInstruction inst, bool simumode = false)
        {
            // 断点
            int bp = 0;
            // 输出类指令的条件
            string cond = String.Format("_stack_{0:d}", stackTop);
            if (stlvalue != null) cond = String.Format("({0:s}&&{1:s})", cond, stlvalue);
            // 如果是仿真模式
            if (simumode)
            {
                // 断点循环
                if (inst.ProtoType != null)
                {
                    bp = inst.ProtoType.Breakpoint.Address;
                    sw.Write("bpcycle({0});\n", bp);
                }
                // 需要由写入使能作为条件
                if (inst.EnBit != null && inst.EnBit.Length > 0)
                {
                    sw.Write("if (!({0:s})) \n{{\n", inst.EnBit);
                }
            }
            // 当前指令为LD类指令，记录栈内当前位置的合并方式
            if (inst.StackCalc != null)
                stackcalcs[stackTop] = inst.StackCalc;
            // 第一次判断指令类型
            switch (inst[0])
            {
                // STL状态切换
                case "STL":
                    stlvalue = inst[1];
                    sw.Write("_stlrst = 0;\n");
                    break;
                case "STLE":
                    sw.Write("if (_stlrst) {0:s} = 0;\n", stlvalue);
                    stlvalue = null;
                    break;
                // 一般的入栈和逻算
                case "LD":
                    sw.Write("_stack_{0:d} = {1:s};\n", ++stackTop, inst[1]);
                    break;
                case "LDIM":
                    if (!simumode && inst[1][0] == 'X')
                        sw.Write("_stack_{0:d} = ScanIm_X({1:s});\n", ++stackTop, inst[2]);
                    else
                        sw.Write("_stack_{0:d} = {1:s};\n", ++stackTop, inst[1]);
                    break;
                case "AND":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s});\n", stackTop, inst[1]);
                    break;
                case "ANDIM":
                    if (!simumode && inst[1][0] == 'X')
                        sw.Write("_stack_{0:d} = (_stack_{0:d}&&ScanIm_X({1:s}));\n", stackTop, inst[2]);
                    else
                        sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s});\n", stackTop, inst[1]);
                    break;
                case "OR":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s});\n", stackTop, inst[1]);
                    break;
                case "ORIM":
                    if (!simumode && inst[1][0] == 'X')
                        sw.Write("_stack_{0:d} = (_stack_{0:d}||ScanIm_X({1:s}));\n", stackTop, inst[2]);
                    else
                        sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s});\n", stackTop, inst[1]);
                    break;
                case "LDI":
                    sw.Write("_stack_{0:d} = !{1:s};\n", ++stackTop, inst[1]);
                    break;
                case "LDIIM":
                    if (!simumode && inst[1][0] == 'X')
                        sw.Write("_stack_{0:d} = !ScanIm_X({1:s});\n", ++stackTop, inst[2]);
                    else
                        sw.Write("_stack_{0:d} = !{1:s};\n", ++stackTop, inst[1]);
                    break;
                case "ANDI":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&!{1:s});\n", stackTop, inst[1]);
                    break;
                case "ANDIIM":
                    if (!simumode && inst[1][0] == 'X')
                        sw.Write("_stack_{0:d} = (_stack_{0:d}&&!ScanIm_X({1:s}));\n", stackTop, inst[2]);
                    else
                        sw.Write("_stack_{0:d} = (_stack_{0:d}&&!{1:s});\n", stackTop, inst[1]);
                    break;
                case "ORI":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||!{1:s});\n", stackTop, inst[1]);
                    break;
                case "ORIIM":
                    if (!simumode && inst[1][0] == 'X')
                        sw.Write("_stack_{0:d} = (_stack_{0:d}||!ScanIm_X({1:s}));\n", stackTop, inst[2]);
                    else
                        sw.Write("_stack_{0:d} = (_stack_{0:d}||!{1:s});\n", stackTop, inst[1]);
                    break;
                // 上升沿和下降沿
                /*
                 * 这里需要将上一次扫描的当前值记录下来，保存到全局变量中
                 * 上次扫描的值为0，而这次扫描的值为1，代表上升沿的跳变
                 * 上次扫描的值为1，而这次扫描的值为0，代表下降沿的跳变
                 */
                case "LDP":
                    sw.Write("_stack_{0:d} = (_global[{1:d}]==0&&{2:s}==1);\n", ++stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "LDF":
                    sw.Write("_stack_{0:d} = (_global[{1:d}]==1&&{2:s}==0);\n", ++stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ANDP":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&_global[{1:d}]==0&&{2:s}==1);\n", stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ANDF":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&_global[{1:d}]==1&&{2:s}==0);\n", stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ORP":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||_global[{1:d}]==0&&{2:s}==1);\n", stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ORF":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||_global[{1:d}]==1&&{2:s}==0);\n", stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "INV":
                    _CalcSignal(sw);
                    sw.Write("_stack_{0:d} = _global[{1:d}]^1;\n", stackTop, globalCount - 1);
                    break;
                case "MEP":
                    _CalcSignal(sw);
                    sw.Write("_stack_{0:d} = (_global[{1:d}]==0&&_global[{2:d}]==1);\n", stackTop, globalCount, globalCount - 1);
                    sw.Write("_global[{0:d}] = _global[{1:d}];\n", globalCount, globalCount - 1);
                    globalCount++;
                    break;
                case "MEF":
                    _CalcSignal(sw);
                    sw.Write("_stack_{0:d} = (_global[{1:d}]==1&&_global[{2:d}]==0);\n", stackTop, globalCount, globalCount - 1);
                    sw.Write("_global[{0:d}] = _global[{1:d}];\n", globalCount, globalCount - 1);
                    globalCount++;
                    break;
                // 出栈
                case "POP": stackTop--; break;
                // 栈合并
                case "ANDB":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&_stack_{1:d});\n", stackTop - 1, stackTop);
                    stackTop--;
                    break;
                case "ORB":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||_stack_{1:d});\n", stackTop - 1, stackTop);
                    stackTop--;
                    break;
                // 比较两个数是否相等
                case "LDWEQ":
                case "LDDEQ":
                case "LDFEQ":
                    sw.Write("_stack_{0:d} = ({1:s}=={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWEQ":
                case "ADEQ":
                case "AFEQ":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s}=={2:s});\n", stackTop, inst[1], inst[2]); break;
                case "ORWEQ":
                case "ORDEQ":
                case "ORFEQ":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s}=={2:s});\n", stackTop, inst[1], inst[2]); break;
                // 比较两个数是否不相等
                case "LDWNE":
                case "LDDNE":
                case "LDFNE":
                    sw.Write("_stack_{0:d} = ({1:s}!={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWNE":
                case "ADNE":
                case "AFNE":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s}!={2:s});\n", stackTop, inst[1], inst[2]); break;
                case "ORWNE":
                case "ORDNE":
                case "ORFNE":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s}!={2:s});\n", stackTop, inst[1], inst[2]); break;
                // 比较前数是否大等后数
                case "LDWGE":
                case "LDDGE":
                case "LDFGE":
                    sw.Write("_stack_{0:d} = ({1:s}>={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWGE":
                case "ADGE":
                case "AFGE":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s}>={2:s});\n", stackTop, inst[1], inst[2]); break;
                case "ORWGE":
                case "ORDGE":
                case "ORFGE":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s}>={2:s});\n", stackTop, inst[1], inst[2]); break;
                // 比较前数是否小等后数
                case "LDWLE":
                case "LDDLE":
                case "LDFLE":
                    sw.Write("_stack_{0:d} = ({1:s}<={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWLE":
                case "ADLE":
                case "AFLE":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s}<={2:s});\n", stackTop, inst[1], inst[2]); break;
                case "ORWLE":
                case "ORDLE":
                case "ORFLE":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s}<={2:s});\n", stackTop, inst[1], inst[2]); break;
                // 比较前数是否大于后数
                case "LDWG":
                case "LDDG":
                case "LDFG":
                    sw.Write("_stack_{0:d} = ({1:s}>{2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWG":
                case "ADG":
                case "AFG":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s}>{2:s});\n", stackTop, inst[1], inst[2]); break;
                case "ORWG":
                case "ORDG":
                case "ORFG":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s}>{2:s});\n", stackTop, inst[1], inst[2]); break;
                // 比较前数是否小于后数
                case "LDWL":
                case "LDDL":
                case "LDFL":
                    sw.Write("_stack_{0:d} = ({1:s}<{2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWL":
                case "ADL":
                case "AFL":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}&&{1:s}<{2:s});\n", stackTop, inst[1], inst[2]); break;
                case "ORWL":
                case "ORDL":
                case "ORFL":
                    sw.Write("_stack_{0:d} = (_stack_{0:d}||{1:s}<{2:s});\n", stackTop, inst[1], inst[2]); break;
                // 输出线圈
                /*
                 * 将当前栈顶的值赋值给线圈
                 */
                case "OUT":
                    sw.Write("{0:s} = {1:s};\n", inst[1], cond);
                    break;
                case "WBOUT":
                case "WBOUTIM":
                    if (simumode)
                        sw.Write("{0:s} &{2:s}, 1, {1:s});", inst[1], cond, inst.EnBit);
                    else
                        sw.Write("{0:s} 1, {1:s})", inst[1], cond);
                    break;
                case "OUTIM":
                    if (!simumode && inst[1][0] == 'Y')
                        sw.Write("OutputIm_Y({0:s}, {1:s});\n", inst[2], cond);
                    else
                        sw.Write("{0:s} = {1:s};\n", inst[1], cond);
                    break;
                // 置位和复位
                /*
                 * 需要用if来判断栈顶是否为1
                 */
                case "SET":
                case "SETIM":
                    if (simumode)
                        sw.Write("if ({0:s}) {{_bitset(&{1:s}, &{3:s}, {2:s});\n",
                            cond, inst[1], inst[2], inst.EnBit);
                    else if (inst[0].Equals("SETIM") && inst[1][0] == 'Y')
                        sw.Write("if ({0:s}) {{_imyset({1:s}, {2:s});\n",
                            cond, inst[3], inst[2]);
                    else
                        sw.Write("if ({0:s}) {{_bitset(&{1:s}, {2:s});\n",
                            cond, inst[1], inst[2]);
                    /*
                     * 在STL状态下，执行对状态的SET指令后，需要在扫描周期后将当前状态复位
                     */
                    if (stlvalue != null && inst[1][0] == 'S')
                        sw.Write("_stlrst = 1;\n");
                    sw.Write("}\n");
                    break;
                case "WBSET":
                case "WBSETIM":
                    if (simumode)
                        sw.Write("if ({0:s}) {1:s} &{3:s}, {2:s}, 1);", cond, inst[1], inst[2], inst.EnBit);
                    else
                        sw.Write("if ({0:s}) {1:s} {2:s}, 1);", cond, inst[1], inst[2]);
                    break;
                case "RST":
                case "RSTIM":
                    if (simumode)
                        sw.Write("if ({0:s}) {{\n_bitrst(&{1:s}, &{3:s}, {2:s});\n",
                            cond, inst[1], inst[2], inst.EnBit);
                    else if (inst[0].Equals("RSTIM") && inst[1][0] == 'Y')
                        sw.Write("if ({0:s}) {{\n_imyrst({1:s}, {2:s});\n",
                            cond, inst[3], inst[2]);
                    else
                        sw.Write("if ({0:s}) {{\n_bitrst(&{1:s}, {2:s});\n",
                            cond, inst[1], inst[2]);
                    /*
                     * 注意如果复位的是计数器或者计时器位，那么相应的值也要跟着复原
                     * 考虑到向下计数器(CTD)复原时需要载入预设值
                     * 所以每个计数器预设值都要存起来便于访问
                     * 预设值需要在外部先初始化
                     */
                    if (inst[1][0] == 'C')
                    {
                        int begin = int.Parse(inst[3]);
                        int end = begin + int.Parse(inst[4]);
                        for (int i = begin; i < end; i++)
                            if (i >= 235 && !simumode)
                                sw.Write("reset_counter({0:d});\n", i);
                            else
                                sw.Write("CVWord[{0:d}] = {1:s};\n", i, ctsv[i]);
                    }
                    if (inst[1][0] == 'T' && !simumode)
                    {
                        int begin = int.Parse(inst[3]);
                        int end = begin + int.Parse(inst[4]);
                        for (int i = begin; i < end; i++)
                            sw.Write("reset_timer({0:d});\n", i);
                    }
                    sw.Write("}\n");
                    break;
                case "WBRST":
                case "WBRSTIM":
                    if (simumode)
                        sw.Write("if ({0:s}) {1:s} &{3:s}, {2:s}, 0);", cond, inst[1], inst[2], inst.EnBit);
                    else
                        sw.Write("if ({0:s}) {1:s} {2:s}, 0);", cond, inst[1], inst[2]);
                    break;
                // 交替
                case "ALT": sw.Write("if ({0:s}) {1:s}=({1:s} ? 0 : 1);\n", cond, inst[1]); break;
                // 上升沿交替
                case "ALTP":
                    sw.Write("if ({3:s} && _global[{0:d}]==0 && _stack_{1:d}==1) {2:s}=({2:s} ? 0 : 1);\n", globalCount, stackTop, inst[1], cond);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 当栈顶为1时运行的计时器
                case "TON":
                    if (simumode)
                    {
                        sw.Write("_ton({0:s}, {1:s}, {2:s}, (int32_t*)(&_global[{3:d}]));\n",
                            cond, inst[1], inst[2], globalCount);
                        globalCount += 4;
                    }
                    else
                        sw.Write("CI_TON({0:s}, {1:s}, {2:s});\n",
                            cond, inst[1], inst[2]);
                    break;
                // 当栈顶为0时运行的计时器
                case "TOF":
                    if (simumode)
                    {
                        sw.Write("_ton(!{0:s}, {1:s}, {2:s},  (int32_t*)(&_global[{3:d}]));\n",
                            cond, inst[1], inst[2], globalCount);
                        globalCount += 4;
                    }
                    else
                        sw.Write("CI_TOF({0:s}, {1:s}, {2:s});\n",
                            cond, inst[1], inst[2]);
                    break;
                // 当栈顶为1时运行，为0时保留当前计时的计时器
                case "TONR":
                    if (simumode)
                    {
                        sw.Write("_tonr({0:s}, {1:s}, {2:s},  (int32_t*)(&_global[{3:d}]));\n",
                            cond, inst[1], inst[2], globalCount);
                        globalCount += 4;
                    }
                    else
                        sw.Write("CI_TONR({0:s}, {1:s}, {2:s});\n",
                            cond, inst[1], inst[2]);
                    break;
                // 向上计数器，每次栈顶上升跳变时加1
                // 当计数到达目标后计数开关设为1
                case "CTU":
                    sw.Write("if ({3:s} && _global[{0:d}]==0 && _stack_{1:d}==1 && !{2:s})\n", globalCount, stackTop, inst[3], cond);
                    sw.Write("if (++{0:s}>={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 向下计数器
                case "CTD":
                    sw.Write("if ({3:s} && _global[{0:d}]==0 && _stack_{1:d}==1 && !{2:s})\n", globalCount, stackTop, inst[3], cond);
                    sw.Write("if (--{0:s}<={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 向上向下计数器，当当前计数小于目标则加1，大于目标则减1
                case "CTUD":
                    sw.Write("if ({3:s} && _global[{0:d}]==0 && _stack_{1:d}==1 && !{2:s})\n", globalCount, stackTop, inst[3], cond);
                    sw.Write("if (({0:s}<{1:s}?++{0:s}:--{0:s})=={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // FOR循环指令，和c的for循环保持一致
                /*
                 * 之前需要保证FOR和NEXT之间的NETWORK已经合并到了一起
                 * 这样才能符合c语言的括号逻辑
                 * 不难发现，这里的for后面多了一个左括号，这是专门为了后面的NEXT指令准备的
                 */
                case "FOR":
                    sw.Write("if ({0:s}) \n", cond);
                    string iter = String.Format("(*((uint16_t*)(&_global[{0:d}])))", globalCount);
                    globalCount += 2;
                    sw.Write("for ({0:s}=0;{0:s}<{1:s};{0:s}++) {{\n", iter, inst[1]);
                    break;
                // NEXT指令，结束前面的FOR循环
                case "NEXT":
                    sw.Write("}\n");
                    break;
                // JMP指令，跳转到指定的标签处
                /*
                 * 这里和FOR一样，要保证跳转到的标签在同一函数中
                 * 因为这里跳转是用c语言对应的goto功能来实现的
                 */
                case "JMP":
                    sw.Write("if ({0:s}) goto LABEL_{1:s};\n", cond, inst[1]);
                    break;
                // LBL指令，设置跳转标签
                case "LBL":
                    sw.Write("LABEL_{0:s} : \n", inst[1]);
                    break;
                // 辅助栈操作
                case "MPS": sw.Write("_mstack_{0:d} = _stack_{1:d};\n", ++mstackTop, stackTop); break;
                case "MRD": sw.Write("_stack_{0:d} = _mstack_{1:d};\n", stackTop, mstackTop); break;
                case "MPP": sw.Write("_stack_{0:d} = _mstack_{1:d};\n", stackTop, mstackTop--); break;
                // 可能会调用到下位接口的指令
                case "PLSF":
                case "DPLSF":
                    if (!simumode)
                        sw.Write("CI_DPLSF((uint8_t)({0:s}),(uint32_t)({1:s}),{2:s}, {3:d});\n",
                            cond, inst[1], inst[2], user_id);
                    break;
                case "PWM":
                case "DPWM":
                    if (!simumode)
                        sw.Write("CI_DPWM((uint8_t)({0:s}),(uint32_t)({1:s}),(uint32_t)({2:s}),{3:s},{4:d});\n",
                            cond, inst[1], inst[2], inst[3], user_id);
                    break;
                case "PLSY":
                case "DPLSY":
                    if (!simumode)
                        sw.Write("CI_DPLSY((uint8_t)({0:s}),(uint32_t)({1:s}),(uint32_t)({2:s}),{3:s},{4:d});\n",
                            cond, inst[1], inst[2], inst[3], user_id);
                    break;
                case "PLSR":
                    if (!simumode)
                        sw.Write("CI_DPLSR((uint8_t)({0:s}), (uint32_t*)(&{1:s}), (uint32_t)({2:s}), {3:s}, {4:d});\n",
                            cond, inst[1], inst[2], inst[3], user_id);
                    break;
                case "DPLSR":
                    if (!simumode)
                        sw.Write("CI_DPLSR((uint8_t)({0:s}), &{1:s}, {2:s}, {3:s}, {4:d});\n",
                            cond, inst[1], inst[2], inst[3], user_id);
                    break;
                case "PLSRD":
                    if (!simumode)
                        sw.Write("CI_DPLSRD((uint8_t)({0:s}), (uint32_t*)(&{1:s}), (uint32_t)({2:s}), {3:s}, {4:s}, {5:d});\n",
                            cond, inst[1], inst[2], inst[3], inst[4], user_id);
                    break;
                case "DPLSRD":
                    if (!simumode)
                        sw.Write("CI_DPLSRD((uint8_t)({0:s}), &{1:s}, {2:s}, {3:s}, {4:s}, {5:d});\n",
                            cond, inst[1], inst[2], inst[3], inst[4], user_id);
                    break;
                case "HCNT":
                    if (!simumode)
                        sw.Write("CI_HCNT((uint8_t)({0:s}), {1:s}, {2:s});\n",
                             cond, inst[1], inst[2]);
                    break;
                case "PLSNEXT":
                    if (!simumode)
                        sw.Write("CI_PLSNEXT((uint8_t)({0:s}),{1:s});\n",
                            cond, inst[1]);
                    break;
                case "PLSSTOP":
                    if (!simumode)
                        sw.Write("CI_STOP((uint8_t)({0:s}),{1:s});\n",
                            cond, inst[1]);
                    break;
                case "ZRN":
                case "DZRN":
                    if (!simumode)
                        sw.Write("CI_DZRN((uint8_t)({0:s}),{1:s},{2:s},{3:s},{4:s},{5:d});\n",
                            cond, inst[1], inst[2], inst[3], inst[4], user_id);
                    break;
                case "ZRND":
                case "DZRND":
                    if (!simumode)
                        sw.Write("CI_DZRN((uint8_t)({0:s}),{1:s},{2:s},{3:s},{4:s},{5:s},{6:d});\n",
                            cond, inst[1], inst[2], inst[3], inst[4], inst[5], user_id);
                    break;
                case "PTO":
                    if (!simumode)
                        sw.Write("CI_PTO((uint8_t)({0:s}),&{1:s},{2:s},{3:s},{4:d});\n",
                            cond, inst[1], inst[2], inst[3], user_id);
                    break;
                case "DRVI":
                case "DDRVI":
                    if (!simumode)
                        sw.Write("CI_DDRVI((uint8_t)({0:s}),{1:s},{2:s},{3:s},{4:s},{5:d});\n",
                            cond, inst[2], inst[1], inst[3], inst[4], user_id);
                    break;
                case "DRVA":
                case "DDRVA":
                    if (!simumode)
                        sw.Write("CI_DDRVA((uint8_t)({0:s}),{1:s},{2:s},{3:s},{4:s},{5:d});\n",
                            cond, inst[2], inst[1], inst[3], inst[4], user_id);
                    break;
                case "PLSA":
                case "DPLSA":
                    if (!simumode)
                        sw.Write("CI_DPLSA((uint8_t)({0:s}),&{1:s},{2:s},{3:s},{4:s},{5:d});\n",
                            cond, inst[1], inst[2], inst[3], inst[4], user_id);
                    break;
                // 实时时钟
                case "TRD":
                    if (!simumode)
                        sw.Write("CI_RTC_RDRTC((uint8_t)({0:s}),&{1:s});\n",
                            cond, inst[1]);
                    break;
                case "TWR":
                    if (!simumode)
                        sw.Write("CI_RTC_SETRTC((uint8_t)({0:s}),&{1:s});\n",
                            cond, inst[1]);
                    break;
                case "PID":
                    if (!simumode)
                        sw.Write("CI_PID((uint8_t)({0:s}),{1:s}, {2:s}, &{3:s}, {4:s}, {5:s}, &{6:s});\n",
                            cond, inst[1], inst[2], inst[3], inst[4], inst[5], inst[6]);
                    break;
                case "MBUS":
                    if (!simumode)
                        sw.Write("CI_MODBUS_MASTER((uint8_t)({0:s}), {1:d}, {2:s}, &{3:s});\n",
                            cond, inst[1].Equals("485") ? 1 : 0, inst[2], inst[3]);
                    break;
                case "SEND":
                    if (!simumode)
                        sw.Write("CI_SEND((uint8_t)({0:s}), {1:s}, {2:s}, {3:s});\n",
                            cond, inst[1], inst[2], inst[3]);
                    break;
                case "REV":
                    if (!simumode)
                        sw.Write("CI_REV((uint8_t)({0:s}), {1:s}, {2:s}, &{3:s});\n",
                            cond, inst[1], inst[2], inst[3]);
                    break;
                // 默认的其他情况，一般之前要先判断栈顶
                default:
                    sw.Write("if ({0:s}) {{\n", cond);
                    // 第二回指令判断
                    switch (inst[0])
                    {
                        // 状态转移
                        case "ST": sw.Write("{0:s} = 1;\n", inst[1]); break;
                        // 数据格式的转化指令
                        case "WTOD": sw.Write("{1:s} = _WORD_to_DWORD({0:s});\n", inst[1], inst[2]); break;
                        case "BDWTOD": sw.Write("{1:s} _WORD_to_DWORD({0:s}));\n", inst[1], inst[2]); break;
                        case "DTOW": sw.Write("{1:s} = _DWORD_to_WORD({0:s});\n", inst[1], inst[2]); break;
                        case "BWDTOW": sw.Write("{1:s} _DWORD_to_WORD({0:s}));\n", inst[1], inst[2]); break;
                        case "DTOF": sw.Write("{1:s} = _DWORD_to_FLOAT({0:s});\n", inst[1], inst[2]); break;
                        case "BIN": sw.Write("{1:s} = _BCD_to_WORD({0:s});\n", inst[1], inst[2]); break;
                        case "BWBIN": sw.Write("{1:s} _BCD_to_WORD({0:s}));\n", inst[1], inst[2]); break;
                        case "BCD": sw.Write("{1:s} = _WORD_to_BCD({0:s});\n", inst[1], inst[2]); break;
                        case "BWBCD": sw.Write("{1:s} _WORD_to_BCD({0:s}));\n", inst[1], inst[2]); break;
                        case "ROUND": sw.Write("{1:s} = _FLOAT_to_ROUND({0:s});\n", inst[1], inst[2]); break;
                        case "BDROUND": sw.Write("{1:s} _FLOAT_to_ROUND({0:s}));\n", inst[1], inst[2]); break;
                        case "TRUNC": sw.Write("{1:s} = _FLOAT_to_TRUNC({0:s});\n", inst[1], inst[2]); break;
                        case "BDTRUNC": sw.Write("{1:s} _FLOAT_to_TRUNC({0:s}));\n", inst[1], inst[2]); break;
                        // 位运算指令
                        case "INVW": case "INVD": sw.Write("{1:s} = ~{0:s};\n", inst[1], inst[2]); break;
                        case "BWINVW": case "BDINVD": sw.Write("{1:s}, ~{0:s});\n", inst[1], inst[2]); break;
                        case "ANDW": case "ANDD": sw.Write("{2:s} = {0:s}&{1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "BWANDW": case "BDANDD": sw.Write("{2:s}, {0:s}&{1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "ORW": case "ORD": sw.Write("{2:s} = {0:s}|{1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "BWORW": case "BDORD": sw.Write("{2:s}, {0:s}|{1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "XORW": case "XORD": sw.Write("{2:s} = {0:s}^{1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "BWXORW": case "BDXORD": sw.Write("{2:s}, {0:s}^{1:s});\n", inst[1], inst[2], inst[3]); break;
                        // 寄存器移动指令
                        case "MOV": case "MOVD": case "MOVF": sw.Write("{1:s} = {0:s};\n", inst[1], inst[2]); break;
                        case "BWMOV": case "BDMOVD": sw.Write("{1:s} {0:s});\n", inst[1], inst[2]); break;
                        case "MVBLK":
                            if (simumode)
                                sw.Write("_mvwblk(&{0:s}, &{1:s}, &{3:s}, {2:s});\n", inst[1], inst[2], inst[3], inst.EnBit);
                            else
                                sw.Write("_mvwblk(&{0:s}, &{1:s}, {2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "MVDBLK":
                            if (simumode)
                                sw.Write("_mvdblk(&{0:s}, &{1:s}, &{3:s}, {2:s});\n", inst[1], inst[2], inst[3], inst.EnBit);
                            else
                                sw.Write("_mvdblk(&{0:s}, &{1:s}, {2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        // 数学运算指令
                        case "ADD": sw.Write("{2:s} = _addw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWADD": sw.Write("{2:s} _addw({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "ADDD": sw.Write("{2:s} = _addd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDADDD": sw.Write("{2:s} _addd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "SUB": sw.Write("{2:s} = _subw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWSUB": sw.Write("{2:s} _subw({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "SUBD": sw.Write("{2:s} = _subd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDSUBD": sw.Write("{2:s} _subd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "MUL": sw.Write("{2:s} = _mulwd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDMUL": sw.Write("{2:s} _mulwd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "MULW": sw.Write("{2:s} = _mulww({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWMULW": sw.Write("{2:s} _mulww({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "MULD": sw.Write("{2:s} = _muldd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDMULD": sw.Write("{2:s} _muldd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "DIV": sw.Write("{2:s} = _divwd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDDIV": sw.Write("{2:s} _divwd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "DIVW": sw.Write("{2:s} = _divww({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWDIVW": sw.Write("{2:s} _divww({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "DIVD": sw.Write("{2:s} = _divdd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDDIVD": sw.Write("{2:s} _divdd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "INC": sw.Write("{1:s} = _incw({0:s});\n", inst[1], inst[2]); break;
                        case "BWINC": sw.Write("{1:s} _incw({0:s}));\n", inst[1], inst[2]); break;
                        case "INCD": sw.Write("{1:s} = _incd({0:s});\n", inst[1], inst[2]); break;
                        case "BDINCD": sw.Write("{1:s} _incd({0:s}));\n", inst[1], inst[2]); break;
                        case "DEC": sw.Write("{1:s} = _decw({0:s});\n", inst[1], inst[2]); break;
                        case "BWDEC": sw.Write("{1:s} _decw({0:s}));\n", inst[1], inst[2]); break;
                        case "DECD": sw.Write("{1:s} = _decd({0:s});\n", inst[1], inst[2]); break;
                        case "BDDECD": sw.Write("{1:s} _decd({0:s}));\n", inst[1], inst[2]); break;
                        // 移位指令
                        case "SHL": sw.Write("{2:s} = _shlw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWSHL": sw.Write("{2:s} _shlw({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "SHLD": sw.Write("{2:s} = _shld({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDSHLD": sw.Write("{2:s} _shld({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "SHR": sw.Write("{2:s} = _shrw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWSHR": sw.Write("{2:s} _shrw({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "SHRD": sw.Write("{2:s} = _shrd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDSHRD": sw.Write("{2:s} _shrd({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "ROL": sw.Write("{2:s} = _rolw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWROL": sw.Write("{2:s} _rolw({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "ROR": sw.Write("{2:s} = _rorw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BWROR": sw.Write("{2:s} _rolw({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "ROLD": sw.Write("{2:s} = _rold({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDROLD": sw.Write("{2:s} _rold({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "RORD": sw.Write("{2:s} = _rord({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "BDRORD": sw.Write("{2:s} _rord({0:s}, {1:s}));\n", inst[1], inst[2], inst[3]); break;
                        case "SHLB":
                            if (inst.ProtoType.Children[0].IsWordBit)
                            {
                                if (simumode)
                                    sw.Write("_shl_wbit_to_bit({0:s}, &{1:s}, &{4:s}, {2:s}, {3:s});\n", inst.ToCParas(1), inst[2], inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_shl_wbit_to_bit({0:s}, &{1:s}, {2:s}, {3:s});\n", inst.ToCParas(1), inst[2], inst[3], inst[4]);
                            }
                            else
                            {
                                if (simumode)
                                    sw.Write("_bitshl(&{0:s}, &{1:s}, &{4:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_bitshl(&{0:s}, &{1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4]);
                            }
                            break;
                        case "WBSHLB":
                            if (inst.ProtoType.Children[0].IsWordBit)
                            {
                                if (simumode)
                                    sw.Write("_shl_wbit_to_wbit({0:s}, {1:s}, &{4:s}, {2:s}, {3:s});\n", inst.ToCParas(1), inst.ToCParas(2), inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_shl_wbit_to_wbit({0:s}, {1:s}, {2:s}, {3:s});\n", inst.ToCParas(1), inst.ToCParas(2), inst[3], inst[4]);
                            }
                            else
                            {
                                if (simumode)
                                    sw.Write("_shl_bit_to_wbit(&{0:s}, {1:s}, &{4:s}, {2:s}, {3:s});\n", inst[1], inst.ToCParas(2), inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_shl_bit_to_wbit(&{0:s}, {1:s}, {2:s}, {3:s});\n", inst[1], inst.ToCParas(2), inst[3], inst[4]);
                            }
                            break;
                        case "SHRB":
                            if (inst.ProtoType.Children[0].IsWordBit)
                            {
                                if (simumode)
                                    sw.Write("_shr_wbit_to_bit({0:s}, &{1:s}, &{4:s}, {2:s}, {3:s});\n", inst.ToCParas(1), inst[2], inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_shr_wbit_to_bit({0:s}, &{1:s}, {2:s}, {3:s});\n", inst.ToCParas(1), inst[2], inst[3], inst[4]);
                            }
                            else
                            {
                                if (simumode)
                                    sw.Write("_bitshr(&{0:s}, &{1:s}, &{4:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_bitshr(&{0:s}, &{1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4]);
                            }
                            break;
                        case "WBSHRB":
                            if (inst.ProtoType.Children[0].IsWordBit)
                            {
                                if (simumode)
                                    sw.Write("_shr_wbit_to_wbit({0:s}, {1:s}, &{4:s}, {2:s}, {3:s});", inst.ToCParas(1), inst.ToCParas(2), inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_shr_wbit_to_wbit({0:s}, {1:s}, {2:s}, {3:s});", inst.ToCParas(1), inst.ToCParas(2), inst[3], inst[4]);
                            }
                            else
                            {
                                if (simumode)
                                    sw.Write("_shr_bit_to_wbit(&{0:s}, {1:s}, &{4:s}, {2:s}, {3:s});", inst[1], inst.ToCParas(2), inst[3], inst[4], inst.EnBit);
                                else
                                    sw.Write("_shr_bit_to_wbit(&{0:s}, {1:s}, {2:s}, {3:s});", inst[1], inst.ToCParas(2), inst[3], inst[4]);
                            }
                            break;
                        // 可能调用下位的浮点运算
                        case "ADDF":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_ADD({0:s},{1:s},{2:s},&{3:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            else
                                sw.Write("{3:s} = _addf({1:s}, {2:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            break;
                        case "SUBF":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_SUB({0:s},{1:s},{2:s},&{3:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            else
                                sw.Write("{3:s} = _subf({1:s}, {2:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            break;
                        case "MULF":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_MUL({0:s},{1:s},{2:s},&{3:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            else
                                sw.Write("{3:s} = _mulf({1:s}, {2:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            break;
                        case "DIVF":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_DIV({0:s},{1:s},{2:s},&{3:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            else
                                sw.Write("{3:s} = _divf({1:s}, {2:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            break;
                        case "SIN":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_SIN({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _sin({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        case "COS":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_COS({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _cos({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        case "TAN":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_TAN({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _tan({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        case "LN":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_LN({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _ln({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        case "EXP":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_EXP({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _exp({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        case "LOG":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_LOG({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _log({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        case "POW":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_POW({0:s},{1:s},{2:s},&{3:s});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            else
                                sw.Write("{3:s} = _pow({1:s}, {2:s}});\n",
                                    cond, inst[1], inst[2], inst[3]);
                            break;
                        case "SQRT":
                            if (!simumode)
                                sw.Write("CI_FLOAT32_SQRT({0:s},{1:s},&{2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("{2:s} = _sqrt({1:s});\n",
                                    cond, inst[1], inst[2]);
                            break;
                        // 辅助功能
                        case "FACT": sw.Write("{1:s} = _fact({0:s});\n", inst[1], inst[2]); break;
                        case "CMP": sw.Write("_cmpw({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]); break;
                        case "WBCMP":
                            if (simumode)
                                sw.Write("_cmpw_wbit({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst.ToCParas(3), inst.EnBit);
                            else
                                sw.Write("_cmpw_wbit({0:s}, {1:s}, {2:s});\n", inst[1], inst[2], inst.ToCParas(3));
                            break;
                        case "CMPD": sw.Write("_cmpd({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]); break;
                        case "WBCMPD":
                            if (simumode)
                                sw.Write("_cmpd_wbit({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst.ToCParas(3), inst.EnBit);
                            else
                                sw.Write("_cmpd_wbit({0:s}, {1:s}, {2:s});\n", inst[1], inst[2], inst.ToCParas(3));
                            break;
                        case "CMPF": sw.Write("_cmpf({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]); break;
                        case "WBCMPF":
                            if (simumode)
                                sw.Write("_cmpf_wbit({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst.ToCParas(3), inst.EnBit);
                            else
                                sw.Write("_cmpf_wbit({0:s}, {1:s}, {2:s});\n", inst[1], inst[2], inst.ToCParas(3));
                            break;
                        case "ZCP": sw.Write("_zcpw({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "WBZCP":
                            if (simumode)
                                sw.Write("_zcpw_wbit({0:s}, {1:s}, {2:s}, {3:s}, &{4:s});\n", inst[1], inst[2], inst[3], inst.ToCParas(4), inst.EnBit);
                            else
                                sw.Write("_zcpw_wbit({0:s}, {1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst.ToCParas(4));
                            break;
                        case "ZCPD": sw.Write("_zcpd({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "WBZCPD":
                            if (simumode)
                                sw.Write("_zcpd_wbit({0:s}, {1:s}, {2:s}, {3:s}, &{4:s});\n", inst[1], inst[2], inst[3], inst.ToCParas(4), inst.EnBit);
                            else
                                sw.Write("_zcpd_wbit({0:s}, {1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst.ToCParas(4));
                            break;
                        case "ZCPF": sw.Write("_zcpf({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "WBZCPF":
                            if (simumode)
                                sw.Write("_zcpf_wbit({0:s}, {1:s}, {2:s}, {3:s}, &{4:s});\n", inst[1], inst[2], inst[3], inst.ToCParas(4), inst.EnBit);
                            else
                                sw.Write("_zcpf_wbit({0:s}, {1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst.ToCParas(4));
                            break;
                        case "NEG": sw.Write("{1:s} = _negw({0:s});\n", inst[1], inst[2]); break;
                        case "BWNEG": sw.Write("{1:s} _negw({0:s}));\n", inst[1], inst[2]); break;
                        case "NEGD": sw.Write("{1:s} = _negd({0:s});\n", inst[1], inst[2]); break;
                        case "XCH": sw.Write("_xchw(&{0:s}, &{1:s});\n", inst[1], inst[2]); break;
                        case "BWXCH":
                            if (inst.ProtoType.Children[0].IsBitWord)
                                sw.Write("_xch_bword_to_word({0:s}, &{1:s}, &{2:s}, &{3:s});\n", inst.ToCParas(1), inst.ToCEnable(1), inst[2], inst.ToCEnable(2));
                            else
                                sw.Write("_xch_bword_to_word({0:s}, &{1:s}, &{2:s}, &{3:s});\n", inst.ToCParas(2), inst.ToCEnable(2), inst[1], inst.ToCEnable(1));
                            break;
                        case "BWBWXCH":
                            sw.Write("_xch_bword_to_bword({0:s}, &{1:s}, {2:s}, &{3:s});\n", inst.ToCParas(1), inst.ToCEnable(1), inst.ToCParas(2), inst.ToCEnable(2));
                            break;
                        case "XCHD": sw.Write("_xchd(&{0:s}, &{1:s});\n", inst[1], inst[2]); break;
                        case "BDXCHD":
                            if (inst.ProtoType.Children[0].IsBitDoubleWord)
                                sw.Write("_xchd_bdword_to_dword({0:s}, &{1:s}, &{2:s}, &{3:s});\n", inst.ToCParas(1), inst.ToCEnable(1), inst[2], inst.ToCEnable(2));
                            else
                                sw.Write("_xchd_bdword_to_dword({0:s}, &{1:s}, &{2:s}, &{3:s});\n", inst.ToCParas(2), inst.ToCEnable(2), inst[1], inst.ToCEnable(1));
                            break;
                        case "BDBDXCHD":
                            sw.Write("_xchd_bdword_to_bdword({0:s}, &{1:s}, {2:s}, &{3:s});\n", inst.ToCParas(1), inst.ToCEnable(1), inst.ToCParas(2), inst.ToCEnable(2));
                            break;
                        case "XCHF": sw.Write("_xchf(&{0:s}, &{1:s});\n", inst[1], inst[2]); break;
                        case "CML": sw.Write("{1:s} = _cmlw({0:s});\n", inst[1], inst[2]); break;
                        case "BWCML": sw.Write("{1:s} _cmlw({0:s}));\n", inst[1], inst[2]); break;
                        case "CMLD": sw.Write("{1:s} = _cmld({0:s});\n", inst[1], inst[2]); break;
                        case "BDCMLD": sw.Write("{1:s} _cmld({0:s}));\n", inst[1], inst[2]); break;
                        case "FMOV":
                            if (simumode)
                                sw.Write("_fmovw({0:s}, &{1:s}, &{3:s}, {2:s});\n", inst[1], inst[2], inst[3], inst.EnBit);
                            else
                                sw.Write("_fmovw({0:s}, &{1:s}, {2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "FMOVD":
                            if (simumode)
                                sw.Write("_fmovd({0:s}, &{1:s}, &{3:s}, {2:s});\n", inst[1], inst[2], inst[3], inst.EnBit);
                            else
                                sw.Write("_fmovd({0:s}, &{1:s}, {2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "SMOV":
                            sw.Write("_smov({0:s}, {1:s}, {2:s}, &{3:s}, {4:s});\n", inst[1], inst[2], inst[3], inst[4], inst[5]);
                            break;
                        // CALL指令，调用子函数
                        case "CALL":
                            sw.Write("{0:s}();\n", inst[1]);
                            break;
                        // CALLM指令，调用用户实现的c语言宏指令，根据参数数量的不同表现为不同的格式
                        case "CALLM":
                            // 无参数的函数
                            if (inst.Count == 2)
                            {
                                sw.Write("{0:s}();", inst[1]);
                            }
                            // 至少存在一个参数
                            else
                            {
                                sw.Write("{0:s}(&{1:s}", inst[1], inst[2]);
                                for (int i = 3; i < inst.Count; i++)
                                {
                                    if (inst[i].Equals(String.Empty)) break;
                                    sw.Write(",&{0:s}", inst[i]);
                                }
                                sw.Write(");\n");
                            }
                            break;
                        // 中断
                        case "ATCH":
                            if (!simumode)
                                sw.Write("CI_INTR_ATCH((uint8_t)({0:s}), {1:s}, {2:s});\n",
                                    cond, inst[1], inst[2]);
                            else
                                sw.Write("_atch({1:s}, {0:s});\n", inst[1], inst[2]);
                            break;
                        case "DTCH":
                            if (!simumode)
                                sw.Write("CI_INTR_DTCH((uint8_t)({0:s}), {1:s});\n",
                                    cond, inst[1]);
                            else
                                sw.Write("_dtch({0:s});\n", inst[1]);
                            break;
                        case "EI":
                            if (!simumode)
                                sw.Write("CI_INTR_ENI((uint8_t)({0:s}));\n", cond);
                            else
                                sw.Write("_ei();\n");
                            break;
                        case "DI":
                            if (!simumode)
                                sw.Write("CI_INTR_DISI((uint8_t)({0:s}));\n", cond);
                            else
                                sw.Write("_di();\n");
                            break;
                        // 实时时钟
                        /*
                        case "TRD":
                            sw.Write("_trd(&{0:s});\n", inst[1]);
                            break;
                        case "TWR":
                            sw.Write("_twr(&{0:s});\n", inst[1]);
                            break;
                        */
                        // 通信
                        case "MBUS":
                            sw.Write("_mbus({0:s}, NULL, 0, &{1:s});\n", inst[1], inst[3]);
                            break;
                        case "SEND":
                            sw.Write("_send({0:s}, {1:s}, {2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "REV":
                            sw.Write("_rev({0:s}, {1:s}, {2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        // 脉冲
                        /*
                        case "PLSF":
                            sw.Write("_plsf({0:s}, &{1:s});\n", inst[1], inst[2]);
                            break;
                        case "DPLSF":
                            sw.Write("_dplsf({0:s}, &{1:s});\n", inst[1], inst[2]);
                            break;
                        case "PWM":
                            sw.Write("_pwm({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "DPWM":
                            sw.Write("_dpwm({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "PLSY":
                            sw.Write("_plsy({0:s}, &{1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "DPLSY":
                            sw.Write("_dplsy({0:s}, &{1:s}, &{2:s};\n", inst[1], inst[2], inst[3]);
                            break;
                        case "PLSR":
                            sw.Write("_plsr(&{0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "DPLSR":
                            sw.Write("_dplsr(&{0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "PLSRD":
                            sw.Write("_plsrd(&{0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "DPLSRD":
                            sw.Write("_dplsrd(&{0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "PLSNEXT":
                            sw.Write("_plsnext(&{0:s});\n", inst[1]);
                            break;
                        case "PLSSTOP":
                            sw.Write("_plsstop(&{0:s});\n", inst[1]);
                            break;
                        case "ZRN":
                            sw.Write("_zrn({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst[3], inst[4]);
                            break;
                        case "DZRN":
                            sw.Write("_dzrn({0:s}, {1:s}, {2:s}, &{3:s});\n", inst[1], inst[2], inst[3], inst[4]);
                            break;
                        case "PTO":
                            sw.Write("_pto(&{0:s}, &{1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "DRVI":
                            sw.Write("_drvi({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "DDRVI":
                            sw.Write("_ddrvi({0:s}, {1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                            break;
                        case "HCNT":
                            sw.Write("_hcnt(&{0:s}, {1:s});\n", inst[1], inst[2]);
                            break;
                        */
                        // 移位操作
                        default: throw new ArgumentException(String.Format("unidentified PLC command : {0:s}", inst[0]));
                    }
                    sw.Write("}\n");
                    // 注意部分脉冲指令当栈顶为0时需要立即停止
                    /*
                    switch (inst.Type)
                    {
                        case "PLSF":
                        case "DPLSF":
                            sw.Write("else\n{{\n_plsstop(&{0:s});\n}}\n", inst[2]);
                            break;
                        case "PWM":
                        case "DPWM":
                        case "PLSY":
                        case "DPLSY":
                        case "PLSR":
                        case "DPLSR":
                        case "PLSRD":
                        case "DPLSRD":
                        case "PTO":
                        case "DRVI":
                        case "DDRVI":
                            sw.Write("else\n{{\n_plsstop(&{0:s});\n}}\n", inst[3]);
                            break;
                        case "ZRN":
                        case "DZRN":
                            sw.Write("else\n{{\n_plsstop(&{0:s});\n}}\n", inst[4]);
                            break;
                    }
                    */
                    break;
            }
            // 如果是仿真模式需要对写入使能条件判断语句结尾
            if (simumode && inst.EnBit != null && inst.EnBit.Length > 0)
            {
                sw.Write("}\n");
            }
            // 进入条件断点的循环
            if (simumode && inst.ProtoType != null && !LadderUnitModel.LabelTypes.Contains(inst.ProtoType.Type))
            {
                _CalcSignal(sw, true);
                sw.Write("cpcycle({0}, _signal);\n", bp);
            }
        }
        /// <summary>
        /// 通过栈记录计算当前信号
        /// </summary>
        /// <param name="sw">文件输出流</param>
        static private void _CalcSignal(StreamWriter sw, bool calccond = false)
        {
            string signvalue = calccond
                ? "_signal"
                : String.Format("_global[{0:d}]", globalCount++);
            sw.Write("{0:s} = _stack_{1:d};\n", signvalue, stackTop);
            for (int i = stackTop; i > 0; i--)
            {
                if (stackcalcs[i - 1] == "POP") break;
                switch (stackcalcs[i - 1])
                {
                    case "ANDB":
                        sw.Write("{0:s} &= _stack_{1:d};\n", signvalue, i - 1);
                        break;
                    case "ORB":
                        //sw.Write("_global[{0:d}] |= _stack_{1:d};\n", globalCount, stackTop - 1);
                        break;
                }
            }
        }

        static public void FuncToCCode(
            StreamWriter sw,
            FuncBlockModel fbmodel,
            string code,
            bool simumode = false)
        {
            if (simumode)
                FuncToCCode(sw, fbmodel.Root, code);
            else
                sw.Write(ReplaceType(code, 0, code.Length));
        }

        static private void FuncToCCode(
            StreamWriter sw,
            FuncBlock fblock,
            string code)
        {
            string text = String.Empty;
            string divi = String.Empty;
            int bp = 0;
            int prev = fblock.IndexStart;
            if (fblock is FuncBlock_Root
             || fblock is FuncBlock_Local)
            {
                if (fblock is FuncBlock_Local) sw.Write("{\n");
                foreach (FuncBlock child in fblock.Childrens)
                {
                    if (prev < child.IndexStart)
                        sw.Write(code.Substring(prev, child.IndexStart - prev));
                    if (child is FuncBlock_Local)
                        FuncToCCode(sw, child, code);
                }
                bp = fblock.BPAddress;
                sw.Write("bpcycle({0});\n", bp);
                if (fblock is FuncBlock_Local)
                    sw.Write("}\n");
            }
            else if (fblock is FuncBlock_ForHeader)
            {
                FuncBlock_ForHeader fblockfh = (FuncBlock_ForHeader)fblock;
                sw.Write("for (");
                if (fblockfh.Start != null)
                {
                    bp = fblockfh.Start.BPAddress;
                    sw.Write("bpcycle({0}),{1}",
                        bp, ReplaceType(code, fblockfh.Start.IndexStart, fblockfh.Start.Length));
                }
                //sw.Write(";");
                if (fblockfh.Cond != null)
                {
                    bp = fblockfh.Cond.BPAddress;
                    sw.Write("bpcycle({0}),{1}",
                        bp, ReplaceType(code, fblockfh.Cond.IndexStart, fblockfh.Cond.Length));
                }
                //sw.Write(";");
                if (fblockfh.Next != null)
                {
                    bp = fblockfh.Next.BPAddress;
                    sw.Write("bpcycle({0}),{1}",
                        bp, ReplaceType(code, fblockfh.Next.IndexStart, fblockfh.Next.Length));
                }
                sw.Write(")");
            }
            else if (fblock is FuncBlock_WhileHeader)
            {
                FuncBlock_WhileHeader fblockwh = (FuncBlock_WhileHeader)fblock;
                sw.Write("while (");
                if (fblockwh.Cond != null)
                {
                    bp = fblockwh.Cond.BPAddress;
                    sw.Write("bpcycle({0}),{1}",
                        bp, ReplaceType(code, fblockwh.Cond.IndexStart, fblockwh.Cond.Length));
                }
                sw.Write(")");
            }
            else
            {
                if (!(fblock is FuncBlock_Root))
                {
                    bp = fblock.BPAddress;
                    sw.Write("bpcycle({0});\n", bp);
                }
                sw.Write("{0}\n", ReplaceType(code, fblock.IndexStart, fblock.Length));
            }
        }

        static private string ReplaceType(string code, int start, int length)
        {
            string text = code.Substring(start, length);
            text = text.Replace("BIT", "_BIT");
            text = text.Replace("WORD", "_WORD");
            text = text.Replace("FLOAT", "_FLOAT");
            return text;
        }
    }
}
