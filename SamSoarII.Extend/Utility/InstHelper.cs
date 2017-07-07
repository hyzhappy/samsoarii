using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using SamSoarII.Extend.FuncBlockModel;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/3/2
/// Author : morenan
/// </summary>
/// <remarks>
/// 和PLC指令相关的辅助类
/// </remarks>

namespace SamSoarII.Extend.Utility
{
    public class InstHelper
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
        static private int[] ctsv = new int[256];
        /// <summary>栈中元素与前一元素的合并方式（INV，MEP，MEF）</summary>
        static private string[] stackcalcs = new string[256];
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
            foreach (PLCInstNetwork net in networks)
            {
                foreach (PLCInstruction inst in net.Insts)
                {
                    inst.WordSize = 32;
                }
            }
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
            foreach (PLCInstNetwork net in networks)
            {
                foreach (PLCInstruction inst in net.Insts)
                {
                    inst.WordSize = 16;
                }
            }
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
            foreach (PLCInstruction inst in insts)
            {
                // 计算普通栈和辅助栈的栈顶
                if (inst.Type.Length > 1 && inst.Type.Substring(0, 2) == "LD")
                {
                    stackTop++;
                    stackinsts.Push(inst);
                }
                if (inst.Type.Equals("ANDB") || inst.Type.Equals("ORB") || inst.Type.Equals("POP"))
                {
                    stackTop--;
                    PLCInstruction topinst = stackinsts.Pop();
                    topinst.StackCalc = inst.Type;
                }
                if (inst.Type.Equals("MPS"))
                    mstackTop++;
                if (inst.Type.Equals("MPP"))
                    mstackTop--;
                // 记录两个栈到达的最大高度
                if (stackTop > stackTotal)
                    stackTotal = stackTop;
                if (mstackTop > mstackTotal)
                    mstackTotal = mstackTop;
                // 统计所有需要全局变量的指令的总数
                switch (inst.Type)
                {
                    case "LDP": case "LDF": case "ANDP": case "ANDF": case "ORP": case "ORF":
                    case "ALTP":
                    case "CTU": case "CTD": case "CTUD":
                    case "FOR":
                    case "INV":
                    case "TON": case "TOF": case "TONR":
                        globalTotal++;
                        break;
                    case "MEP": case "MEF":
                        globalTotal += 2;
                        break;
                }
                // 找到所有计数器的预设值
                if (inst.Type.Length >= 2 && inst.Type.Substring(0, 2).Equals("CT"))
                {
                    ctsv[int.Parse(inst[4])] = int.Parse(inst[2]);
                }
            }
            // 建立C代码的全局环境
            //sw.Write("#include \"lib.h\"\n");
            //sw.Write("#include \"main.h\"\n\n");
            //sw.Write("static uint16_t _stack[256];\n");         // 数据栈
            //sw.Write("static uint16_t _stacktop;\n");           // 数据栈的栈顶
            //sw.Write("static uint16_t _mstack[256];\n");        // 辅助栈
            //sw.Write("static uint16_t _mstacktop;\n");          // 辅助栈的栈顶
            user_id = 0;
            sw.Write("static int32_t _global[{0:d}];\n", globalTotal); // 全局变量
            if (simumode)
                sw.Write("static int32_t _signal;\n");
            // 先声明所有的子函数
            foreach (PLCInstruction inst in insts)
            {
                if (inst.Type.Equals("FUNC"))
                    sw.Write("void _SBR_{0:s}();\n", inst[1]);
            }
            // 建立扫描的主函数
            sw.Write("void RunLadder()\n{\n");
            if (simumode)
                sw.Write("callinto();\n");
            sw.Write("_itr_invoke();\n");
            // 建立局部的栈和辅助栈
            for (int i = 1; i <= stackTotal; i++)
            {
                sw.Write("uint32_t _stack_{0:d};\n", i);
            }
            for (int i = 1; i <= mstackTotal; i++)
            {
                sw.Write("uint32_t _mstack_{0:d};\n", i);
            }
            // 生成PLC对应的内容
            // 初始化栈顶
            stackTop = 0;
            mstackTop = 0;
            foreach (PLCInstruction inst in insts)
            {
                switch (inst.Type)
                {
                    // 函数头部
                    case "FUNC":
                        if (simumode)
                            sw.Write("callleave();\n");
                        sw.Write("}\n\n");
                        sw.Write("void _SBR_{0:s}()", inst[1]);
                        sw.Write("{\n");
                        if (simumode)
                            sw.Write("callinto();\n");
                        // 建立局部的栈和辅助栈
                        for (int i = 1; i <= stackTotal; i++)
                        {
                            sw.Write("uint16_t _stack_{0:d};\n", i);
                        }
                        for (int i = 1; i <= mstackTotal; i++)
                        {
                            sw.Write("uint16_t _mstack_{0:d};\n", i);
                        }
                        // 初始化栈顶
                        stackTop = 0;
                        mstackTop = 0;
                        break;
                    default:
                        InstToCCode(sw, inst, simumode);
                        break;
                }
            }
            if (simumode)
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
            int bp = 0;
            // 如果是仿真模式
            if (simumode)
            {
                // 断点循环
                if (inst.ProtoType != null)
                {
                    BreakPointManager.Register(inst.ProtoType);
                    bp = inst.ProtoType.BPAddress;
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
            {
                stackcalcs[stackTop] = inst.StackCalc;
            }
            // 第一次判断指令类型
            switch (inst.Type)
            {
                // 一般的入栈和逻算
                case "LD": case "LDIM":     sw.Write("_stack_{0:d} = {1:s};\n",   ++stackTop, inst[1]); break;
                case "AND": case "ANDIM":   sw.Write("_stack_{0:d} &= {1:s};\n",    stackTop, inst[1]); break;
                case "OR": case "ORIM":     sw.Write("_stack_{0:d} |= {1:s};\n",    stackTop, inst[1]); break;
                case "LDI": case "LDIIM":   sw.Write("_stack_{0:d} = !{1:s};\n",  ++stackTop, inst[1]); break;
                case "ANDI": case "ANDIIM": sw.Write("_stack_{0:d} &= !{1:s};\n",   stackTop, inst[1]); break;
                case "ORI": case "ORIIM":   sw.Write("_stack_{0:d} |= !{1:s};\n",   stackTop, inst[1]); break;
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
                    sw.Write("_stack_{0:d} &= (_global[{1:d}]==0&&{2:s}==1);\n",  stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ANDF":
                    sw.Write("_stack_{0:d} &= (_global[{1:d}]==1&&{2:s}==0);\n",  stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ORP":
                    sw.Write("_stack_{0:d} &= (_global[{1:d}]==0&&{2:s}==1);\n",  stackTop, globalCount, inst[1]);
                    sw.Write("_global[{0:d}] = {1:s};\n", globalCount++, inst[1]);
                    break;
                case "ORF":
                    sw.Write("_stack_{0:d} &= (_global[{1:d}]==1&&{2:s}==0);\n",  stackTop, globalCount, inst[1]);
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
                    sw.Write("_stack_{0:d} &= _stack_{1:d};\n", stackTop-1, stackTop);
                    stackTop--;
                    break;
                case "ORB":
                    sw.Write("_stack_{0:d} |= _stack_{1:d};\n", stackTop-1, stackTop);
                    stackTop--;
                    break;
                // 比较两个数是否相等
                case "LDWEQ":case "LDDEQ":case "LDFEQ":
                    sw.Write("_stack_{0:d} = ({1:s}=={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWEQ":case "ADEQ":case "AFEQ":
                    sw.Write("_stack_{0:d} &= ({1:s}=={2:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWEQ":case "ORDEQ":case "ORFEQ":
                    sw.Write("_stack_{0:d} |= ({1:s}=={2:s});\n",  stackTop, inst[1], inst[2]); break;
                // 比较两个数是否不相等
                case "LDWNE":case "LDDNE":case "LDFNE":
                    sw.Write("_stack_{0:d} = ({1:s}!={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWNE":case "ADNE":case "AFNE":
                    sw.Write("_stack_{0:d} &= ({1:s}!={2:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWNE": case "ORDNE":case "ORFNE":
                    sw.Write("_stack_{0:d} |= ({1:s}!={2:s});\n",  stackTop, inst[1], inst[2]); break;
                // 比较前数是否大等后数
                case "LDWGE":case "LDDGE":case "LDFGE":
                    sw.Write("_stack_{0:d} = ({1:s}>={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWGE":case "ADGE":case "AFGE":
                    sw.Write("_stack_{0:d} &= ({1:s}>={2:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWGE":case "ORDGE":case "ORFGE":
                    sw.Write("_stack_{0:d} |= ({1:s}>={2:s});\n",  stackTop, inst[1], inst[2]); break;
                // 比较前数是否小等后数
                case "LDWLE": case "LDDLE":case "LDFLE":
                    sw.Write("_stack_{0:d} = ({1:s}<={2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWLE":case "ADLE": case "AFLE":
                    sw.Write("_stack_{0:d} &= ({1:s}<={2:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWLE":case "ORDLE":case "ORFLE":
                    sw.Write("_stack_{0:d} |= ({1:s}<={2:s});\n",  stackTop, inst[1], inst[2]); break;
                // 比较前数是否大于后数
                case "LDWG": case "LDDG":case "LDFG":
                    sw.Write("_stack_{0:d} = ({1:s}>{2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWG":case "ADG":case "AFG":
                    sw.Write("_stack_{0:d} &= ({1:s}>{2:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWG":case "ORDG":case "ORFG":
                    sw.Write("_stack_{0:d} |= ({1:s}>{2:s});\n",  stackTop, inst[1], inst[2]); break;
                // 比较前数是否小于后数
                case "LDWL":case "LDDL": case "LDFL":
                    sw.Write("_stack_{0:d} = ({1:s}<{2:s});\n", ++stackTop, inst[1], inst[2]); break;
                case "AWL":case "ADL":  case "AFL":
                    sw.Write("_stack_{0:d} &= ({1:s}<{2:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWL": case "ORDL": case "ORFL":
                    sw.Write("_stack_{0:d} |= ({1:s}<{2:s});\n",  stackTop, inst[1], inst[2]); break;
                // 输出线圈
                /*
                 * 将当前栈顶的值赋值给线圈
                 */
                case "OUT": case "OUTIM":
                    sw.Write("{0:s} = _stack_{1:d};\n", inst[1], stackTop); break;
                // 置位和复位
                /*
                 * 需要用if来判断栈顶是否为1
                 */
                case "SET": case "SETIM":
                    if (simumode)
                        sw.Write("if (_stack_{0:d}) _bitset(&{1:s}, &{3:s}, {2:s});\n", 
                            stackTop, inst[1], inst[2], inst.EnBit);
                    else
                        sw.Write("if (_stack_{0:d}) _bitset(&{1:s}, {2:s});\n", 
                            stackTop, inst[1], inst[2]);
                    break;
                case "RST": case "RSTIM":
                    if (simumode)
                        sw.Write("if (_stack_{0:d}) {{\n_bitrst(&{1:s}, &{3:s}, {2:s});\n", 
                            stackTop, inst[1], inst[2], inst.EnBit);
                    else
                        sw.Write("if (_stack_{0:d}) {{\n_bitrst(&{1:s}, {2:s});\n", 
                            stackTop, inst[1], inst[2]);
                    /*
                     * 注意如果复位的是计数器位，那么计数器值也要跟着复原
                     * 考虑到向下计数器(CTD)复原时需要载入预设值
                     * 所以每个计数器预设值都要存起来便于访问
                     * 预设值需要在外部先初始化
                     */
                    if (inst[1][0] == 'C')
                    {
                        int begin = int.Parse(inst[3]);
                        int end = begin + int.Parse(inst[4]);
                        for (int i = begin; i < end; i++)
                            sw.Write("CVWord[{0:d}] = {1:d};", i, ctsv[i]);
                    }
                    sw.Write("}\n");
                    break;
                // 交替
                case "ALT": sw.Write("if (_stack_{0:d}) {1:s}=({1:s} ? 0 : 1);\n", stackTop, inst[1]); break;
                // 上升沿交替
                case "ALTP":
                    sw.Write("if (_global[{0:d}]==0 && _stack_{1:d}==1) {2:s}=({2:s} ? 0 : 1);\n", globalCount, stackTop, inst[1]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 当栈顶为1时运行的计时器
                case "TON":
                    if (simumode)
                        sw.Write("_ton(_stack_{0:d}, {1:s}, {2:s}, &_global[{3:d}]);\n",
                            stackTop, inst[1], inst[2], globalCount);
                    else
                        sw.Write("CI_TON(_stack_{0:d}, {1:s}, {2:s});\n",
                            stackTop, inst[1], inst[2]);
                    globalCount += 1;
                    break;
                // 当栈顶为0时运行的计时器
                case "TOF":
                    if (simumode)
                        sw.Write("_ton(!_stack_{0:d}, {1:s}, {2:s}, &_global[{3:d}]);\n",
                            stackTop, inst[1], inst[2], globalCount);
                    else
                        sw.Write("CI_TON(!_stack_{0:d}, {1:s}, {2:s});\n",
                            stackTop, inst[1], inst[2]);
                    globalCount += 1;
                    break;
                // 当栈顶为1时运行，为0时保留当前计时的计时器
                case "TONR":
                    if (simumode)
                        sw.Write("_tonr(_stack_{0:d}, {1:s}, {2:s}, &_global[{3:d}]);\n",
                            stackTop, inst[1], inst[2], globalCount);
                    else
                        sw.Write("CI_TON(_stack_{0:d}, {1:s}, {2:s});\n",
                            stackTop, inst[1], inst[2]);
                    globalCount += 1;
                    break;
                // 向上计数器，每次栈顶上升跳变时加1
                // 当计数到达目标后计数开关设为1
                case "CTU":
                    sw.Write("if (_global[{0:d}]==0 && _stack_{1:d}==1 && !{2:s})\n", globalCount, stackTop, inst[3]);
                    sw.Write("if (++{0:s}>={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 向下计数器
                case "CTD":
                    sw.Write("if (_global[{0:d}]==0 && _stack_{1:d}==1 && !{2:s})\n", globalCount, stackTop, inst[3]);
                    sw.Write("if (--{0:s}<={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 向上向下计数器，当当前计数小于目标则加1，大于目标则减1
                case "CTUD":
                    sw.Write("if (_global[{0:d}]==0 && _stack_{1:d}==1 && !{2:s})\n", globalCount, stackTop, inst[3]);
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
                    sw.Write("if (_stack_{0:d}) \n", stackTop);
                    sw.Write("for (_global[{0:d}]=0;_global[{0:d}]<{1:s};_global[{0:d}]++) {{\n", globalCount++, inst[1]);
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
                    sw.Write("if (_stack_{0:d}) \n", stackTop);
                    sw.Write("goto LABEL_{0:s};\n", globalCount++, inst[1]);
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
                case "PLSF": case "DPLSF":
                    if (!simumode)
                        sw.Write("CI_DPLSF((uint8_t)(_stack_{0:d}),(uint32_t)({1:s}),{2:s}, {3:d});\n",
                            stackTop, inst[1], inst[2], user_id);
                    break;
                case "PWM": case "DPWM":
                    if (!simumode)
                        sw.Write("CI_DPWM((uint8_t)(_stack_{0:d}),(uint32_t)({1:s}),(uint32_t)({2:s}),{3:s},{4:d});\n",
                            stackTop, inst[1], inst[2], inst[3], user_id);
                    break;
                case "PLSY": case "DPLSY":
                    if (!simumode)
                        sw.Write("CI_DPLSY((uint8_t)(_stack_{0:d}),(uint32_t)({1:s}),(uint32_t)({2:s}),{3:s},{4:d});\n",
                            stackTop, inst[1], inst[2], inst[3], user_id);
                    break;
                case "HCNT":
                    if (!simumode)
                        sw.Write("CI_HCNT((uint8_t)(_stack_{0:d}),{1:s},{2:s});\n",
                             stackTop, inst[1], inst[2]);
                    break;
                // 默认的其他情况，一般之前要先判断栈顶
                default:
                    sw.Write("if (_stack_{0:d}) {{\n", stackTop);
                    // 第二回指令判断
                    switch (inst.Type)
                    {
                        // 数据格式的转化指令
                        case "WTOD": sw.Write("{1:s} = _WORD_to_DWORD({0:s});\n", inst[1], inst[2]); break;
                        case "DTOW": sw.Write("{1:s} = _DWORD_to_WORD({0:s});\n", inst[1], inst[2]); break;
                        case "DTOF": sw.Write("{1:s} = _DWORD_to_FLOAT({0:s});\n", inst[1], inst[2]); break;
                        case "BIN": sw.Write("{1:s} = _BCD_to_WORD({0:s});\n", inst[1], inst[2]); break;
                        case "BCD": sw.Write("{1:s} = _WORD_to_BCD({0:s});\n", inst[1], inst[2]); break;
                        case "ROUND": sw.Write("{1:s} = _FLOAT_to_ROUND({0:s});\n", inst[1], inst[2]); break;
                        case "TRUNC": sw.Write("{1:s} = _FLOAT_to_TRUNC({0:s});\n", inst[1], inst[2]); break;
                        // 位运算指令
                        case "INVW": case "INVD": sw.Write("{1:s} = ~{0:s};\n", inst[1], inst[2]); break;
                        case "ANDW": case "ANDD": sw.Write("{2:s} = {0:s}&{1:s}", inst[1], inst[2], inst[3]); break;
                        case "ORW": case "ORD": sw.Write("{2:s} = {0:s}|{1:s}", inst[1], inst[2], inst[3]); break;
                        case "XORW": case "XORD": sw.Write("{2:s} = {0:s}^{1:s}", inst[1], inst[2], inst[3]); break;
                        // 寄存器移动指令
                        case "MOV": case "MOVD": case "MOVF": sw.Write("{1:s} = {0:s};\n", inst[1], inst[2]);break;
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
                        case "ADDD": sw.Write("{2:s} = _addd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "ADDF": sw.Write("{2:s} = _addf({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SUB": sw.Write("{2:s} = _subw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SUBD": sw.Write("{2:s} = _subd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SUBF": sw.Write("{2:s} = _subf({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "MUL": sw.Write("{2:s} = _mulwd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "MULW": sw.Write("{2:s} = _mulww({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "MULD": sw.Write("{2:s} = _muldd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "MULF": sw.Write("{2:s} = _mulff({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "DIV": sw.Write("{2:s} = _divwd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "DIVW": sw.Write("{2:s} = _divww({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "DIVD": sw.Write("{2:s} = _divdd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "DIVF": sw.Write("{2:s} = _divff({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "INC": sw.Write("{1:s} = _incw({0:s});\n", inst[1], inst[2]); break;
                        case "INCD": sw.Write("{1:s} = _incd({0:s});\n", inst[1], inst[2]); break;
                        case "DEC": sw.Write("{1:s} = _decw({0:s});\n", inst[1], inst[2]); break;
                        case "DECD": sw.Write("{1:s} = _decw({0:s});\n", inst[1], inst[2]); break;
                        case "SIN": sw.Write("{1:s} = _sin({0:s});\n", inst[1], inst[2]); break;
                        case "COS": sw.Write("{1:s} = _cos({0:s});\n", inst[1], inst[2]); break;
                        case "TAN": sw.Write("{1:s} = _tan({0:s});\n", inst[1], inst[2]); break;
                        case "LN": sw.Write("{1:s} = _ln({0:s});\n", inst[1], inst[2]); break;
                        case "EXP": sw.Write("{1:s} = _exp({0:s});\n", inst[1], inst[2]); break;
                        // 移位指令
                        case "SHL": sw.Write("{2:s} = _shlw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SHLD": sw.Write("{2:s} = _shld({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SHR": sw.Write("{2:s} = _shrw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SHRD": sw.Write("{2:s} = _shrd({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "ROL": sw.Write("{2:s} = _rolw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "ROR": sw.Write("{2:s} = _rorw({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "ROLD": sw.Write("{2:s} = _rold({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "RORD": sw.Write("{2:s} = _rord({0:s}, {1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "SHLB":
                            if (simumode)
                                sw.Write("_bitshl(&{0:s}, &{1:s}, &{4:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4], inst.EnBit);
                            else
                                sw.Write("_bitshl(&{0:s}, &{1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4]);
                            break;
                        case "SHRB":
                            if (simumode)
                                sw.Write("_bitshr(&{0:s}, &{1:s}, &{4:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4], inst.EnBit);
                            else
                                sw.Write("_bitshr(&{0:s}, &{1:s}, {2:s}, {3:s});\n", inst[1], inst[2], inst[3], inst[4]);
                            break;
                        // 辅助功能
                        case "LOG":sw.Write("{1:d} = _log({0:d});\n", inst[1], inst[2]); break;
                        case "POW":sw.Write("{2:d} = _pow({0:d}, {1:d});\n", inst[1], inst[2], inst[3]); break;
                        case "FACT":sw.Write("{1:d} = _fact({0:d);\n", inst[1], inst[2]); break;
                        case "CMP":  sw.Write("{2:d} = _cmpw({0:d}, {1:d});\n", inst[1], inst[2], inst[3]); break;
                        case "CMPD": sw.Write("{2:d} = _cmpd({0:d}, {1:d});\n", inst[1], inst[2], inst[3]); break;
                        case "CMPF": sw.Write("{2:d} = _cmpf({0:d}, {1:d});\n", inst[1], inst[2], inst[3]); break;
                        case "ZCP": sw.Write("{3:d} = _zcpw({0:d}, {1:d}, {2:d});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "ZCPD": sw.Write("{3:d} = _zcpd({0:d}, {1:d}, {2:d});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "ZCPF": sw.Write("{3:d} = _zcpf({0:d}, {1:d}, {2:d});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "NEG": sw.Write("{1:d} = _negw({0:d});\n", inst[1], inst[2]); break;
                        case "NEGD": sw.Write("{1:d} = _negd({0:d});\n", inst[1], inst[2]); break;
                        case "XCH": sw.Write("_xch(&{0:d}, &{1:d});\n", inst[1], inst[2]); break;
                        case "XCHD": sw.Write("_xchd(&{0:d}, &{1:d});\n", inst[1], inst[2]); break;
                        case "XCHF": sw.Write("_xchf(&{0:d}, &{1:d});\n", inst[1], inst[2]); break;
                        case "CML": sw.Write("{1:d} = _cmlw({0:d});\n", inst[1], inst[2]); break;
                        case "CMLD": sw.Write("{1:d} = _cmld({0:d});\n", inst[1], inst[2]); break;
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
                        case "SMOV": sw.Write("_smov({0:s}, {1:s}, {2:s}, &{3:s}, {4:s});\n", inst[1], inst[2], inst[3], inst[4], inst[5]); break;
                        // CALL指令，调用子函数
                        case "CALL":
                            sw.Write("_SBR_{0:s}();\n", inst[1]);
                            break;
                        // CALLM指令，调用用户实现的c语言宏指令，根据参数数量的不同表现为不同的格式
                        case "CALLM":
                            // 无参数的函数
                            if (inst[2].Equals(String.Empty))
                            {
                                sw.Write("{0:s}();", inst[1]);
                            }
                            // 至少存在一个参数
                            else
                            {
                                sw.Write("{0:s}({1:s}", inst[1], inst[2]);
                                for (int i = 3; i < 6; i++)
                                {
                                    if (inst[i].Equals(String.Empty)) break;
                                    sw.Write(",{0:s}", inst[i]);
                                }
                                sw.Write(");\n");
                            }
                            break;
                        // 中断
                        case "ATCH":
                            sw.Write("_atch({0:s}, _SBR_{1:s});\n", inst[1], inst[2]);
                            break;
                        case "DTCH":
                            sw.Write("_dtch({0:s}, _SBR_{1:s});\n", inst[1], inst[2]);
                            break;
                        case "EI":
                            sw.Write("_ei();\n");
                            break;
                        case "DI":
                            sw.Write("_di();\n");
                            break;
                        // 实时时钟
                        case "TRD":
                            sw.Write("_trd(&{0:s});\n", inst[1]);
                            break;
                        case "TWR":
                            sw.Write("_twr(&{0:s});\n", inst[1]);
                            break;
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
                        // 移位操作
                        default: throw new ArgumentException(String.Format("unidentified PLC command : {0:s}", inst.Type));
                    }
                    sw.Write("}\n");
                    // 注意栈顶为0时重置一般的计时器
                    if (inst.Type.Equals("TON"))
                    {
                        sw.Write("else\n{{\n{0:s}=0;\n}}\n", inst[1]);
                    }
                    // 注意部分脉冲指令当栈顶为0时需要立即停止
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
                break; 
            }
            // 如果是仿真模式需要对写入使能条件判断语句结尾
            if (simumode && inst.EnBit != null && inst.EnBit.Length > 0)
            {
                sw.Write("}\n");
            }
            // 进入条件断点的循环
            if (simumode && inst.ProtoType != null)
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
                if (stackcalcs[i-1] == "POP") break;
                switch (stackcalcs[i-1])
                {
                    case "ANDB":
                        sw.Write("{0:s} &= _stack_{1:d};\n", signvalue, stackTop - 1);
                        break;
                    case "ORB":
                        //sw.Write("_global[{0:d}] |= _stack_{1:d};\n", globalCount, stackTop - 1);
                        break;
                }
            }
        }

        static public void FuncToCCode(
            StreamWriter sw, 
            FuncBlockModel.FuncBlockModel fbmodel, 
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
                if (fblock is FuncBlock_Local)
                    sw.Write("{\n");
                foreach (FuncBlock child in fblock.Childrens)
                {
                    if (prev < child.IndexStart)
                    {
                        sw.Write(code.Substring(prev, child.IndexStart - prev));
                    }
                    if (child is FuncBlock_Local)
                    {
                        FuncToCCode(sw, child, code);
                    }
                }
                BreakPointManager.Register(fblock);
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
                    BreakPointManager.Register(fblockfh.Start);
                    bp = fblockfh.Start.BPAddress;
                    sw.Write("bpcycle({0}),{1}",
                        bp, ReplaceType(code, fblockfh.Start.IndexStart, fblockfh.Start.Length));
                }
                //sw.Write(";");
                if (fblockfh.Cond != null)
                {
                    BreakPointManager.Register(fblockfh.Cond);
                    bp = fblockfh.Cond.BPAddress;
                    sw.Write("bpcycle({0}),{1}",
                        bp, ReplaceType(code, fblockfh.Cond.IndexStart, fblockfh.Cond.Length));
                }
                //sw.Write(";");
                if (fblockfh.Next != null)
                {
                    BreakPointManager.Register(fblockfh.Next);
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
                    BreakPointManager.Register(fblockwh.Cond);
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
                    BreakPointManager.Register(fblock);
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
