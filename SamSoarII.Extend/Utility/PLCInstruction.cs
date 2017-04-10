using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/3/2
/// Author : morenan
/// </summary>
/// <remarks>
/// PLC指令的结构
/// </remarks>

namespace SamSoarII.Extend.Utility
{
    public class PLCInstruction
    {
        /// <summary>
        /// 私有成员
        /// </summary>
        protected BaseViewModel prototype = null;
        protected int prototypeid = -1;
        protected string text = String.Empty;
        protected string type = String.Empty;
        protected string flag1 = String.Empty;
        protected string flag2 = String.Empty;
        protected string flag3 = String.Empty;
        protected string flag4 = String.Empty;
        protected string flag5 = String.Empty;
        protected string oflag1 = String.Empty;
        protected string oflag2 = String.Empty;
        protected string oflag3 = String.Empty;
        protected string oflag4 = String.Empty;
        protected string oflag5 = String.Empty;
        protected string enbit = String.Empty;
        protected string stackcalc = String.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text">指令文本</param>
        public PLCInstruction(string text)
        {
            Text = text;
        }
        /// <summary>
        /// 转为字符串格式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }
        /// <summary>
        /// 原型
        /// </summary>
        public BaseViewModel ProtoType
        {
            get { return this.prototype; }
            set { this.prototype = value; }
        }
        /// <summary>
        /// 原型的ID
        /// </summary>
        public int PrototypeID
        {
            get { return this.prototypeid; }
            set { this.prototypeid = value; }
        }
        /// <summary>
        /// 当这个指令把元素加入栈中时（LD类指令），需要知道这个元素是如何与上一个元素计算合并的
        /// 已知的合并方式有三种，分别为ANDB，ORB和POP
        /// 这里用于线路转换指令（INV, MEP, MEF）的当前信号的计算
        /// </summary>
        public string StackCalc
        {
            get { return this.stackcalc; }
            set { this.stackcalc = value; }
        }
        /// <summary>
        /// 判断是否为这条指令的原型
        /// </summary>
        /// <param name="bvmodel">元件的显示模型</param>
        /// <returns></returns>
        public bool IsPrototype(BaseViewModel bvmodel)
        {
            switch (Type)
            {
                // (rB)
                case "LD":
                case "AND":
                case "OR":
                    return bvmodel.InstructionName.Equals("LD");
                case "LDI":
                case "ANDI":
                case "ORI":
                    return bvmodel.InstructionName.Equals("LDI");
                case "LDIM":
                case "ANDIM":
                case "ORIM":
                    return bvmodel.InstructionName.Equals("LDIM");
                case "LDIIM":
                case "ANDIIM":
                case "ORIIM":
                    return bvmodel.InstructionName.Equals("LDIIM");
                case "LDP":
                case "ANDP":
                case "ORP":
                    return bvmodel.InstructionName.Equals("LDP");
                case "LDF":
                case "ANDF":
                case "ORF":
                    return bvmodel.InstructionName.Equals("LDF");
                case "LDWEQ":
                case "AWEQ":
                case "ORWEQ":
                    return bvmodel.InstructionName.Equals("WEQ");
                case "LDDEQ":
                case "ADEQ":
                case "ORDEQ":
                    return bvmodel.InstructionName.Equals("DEQ");
                case "LDFEQ":
                case "AFEQ":
                case "ORFEQ":
                    return bvmodel.InstructionName.Equals("FEQ");
                case "LDWNE":
                case "AWNE":
                case "ORWNE":
                    return bvmodel.InstructionName.Equals("WNE");
                case "LDDNE":
                case "ADNE":
                case "ORDNE":
                    return bvmodel.InstructionName.Equals("DNE");
                case "LDFNE":
                case "AFNE":
                case "ORFNE":
                    return bvmodel.InstructionName.Equals("FNE");
                case "LDWGE":
                case "AWGE":
                case "ORWGE":
                    return bvmodel.InstructionName.Equals("WGE");
                case "LDDGE":
                case "ADGE":
                case "ORDGE":
                    return bvmodel.InstructionName.Equals("DGE");
                case "LDFGE":
                case "AFGE":
                case "ORFGE":
                    return bvmodel.InstructionName.Equals("FGE");
                case "LDWLE":
                case "AWLE":
                case "ORWLE":
                    return bvmodel.InstructionName.Equals("WLE");
                case "LDDLE":
                case "ADLE":
                case "ORDLE":
                    return bvmodel.InstructionName.Equals("DLE");
                case "LDFLE":
                case "AFLE":
                case "ORFLE":
                    return bvmodel.InstructionName.Equals("FLE");
                case "LDWG":
                case "AWG":
                case "ORWG":
                    return bvmodel.InstructionName.Equals("WG");
                case "LDDG":
                case "ADG":
                case "ORDG":
                    return bvmodel.InstructionName.Equals("DG");
                case "LDFG":
                case "AFG":
                case "ORFG":
                    return bvmodel.InstructionName.Equals("FG");
                case "LDWL":
                case "AWL":
                case "ORWL":
                    return bvmodel.InstructionName.Equals("WL");
                case "LDDL":
                case "ADL":
                case "ORDL":
                    return bvmodel.InstructionName.Equals("DL");
                case "LDFL":
                case "AFL":
                case "ORFL":
                    return bvmodel.InstructionName.Equals("FL");
                default:
                    return bvmodel.InstructionName.Equals(Type);
            }
        }
        /// <summary>
        /// 指令文本
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set
            {
                // 解析给定的文本，生成类型和四个参数
                this.text = value;
                string[] args = text.Split(' ');
                this.type = args[0];
                this.enbit = null;
                if (args.Length > 1) this.oflag1 = args[1];
                if (args.Length > 2) this.oflag2 = args[2];
                if (args.Length > 3) this.oflag3 = args[3];
                if (args.Length > 4) this.oflag4 = args[4];
                if (args.Length > 5) this.oflag5 = args[5];
                // 根据指令类型的参数结构来分类，并转化参数的格式
                /*
                 * 参数分为位(B)，字(W)，双字(D)和浮点(F)这四种数据类型
                 * 按照读写权限分为只读(r)，只写(w)和读写(rw)三种
                 * 将指令所有参数的类型和读写的简称括号起来表示
                 * 例如(rB,rW,rwD)表示第一个参数为只读位，第二个为只写字，第三个读写双字
                 * 如果参数包括可写项，则附带支持仿真的写入使能
                 */
                switch (this.type)
                {
                    // (rB)
                    case "LD": case "AND": case "OR":
                    case "LDI": case "ANDI": case "ORI":
                    case "LDIM":case "ANDIM":case "ORIM":
                    case "LDIIM":case "ANDIIM":case "ORIIM":
                    case "LDP": case "ANDP": case "ORP":
                    case "LDF": case "ANDF": case "ORF":
                        this.flag1 = ToCStyle(args[1], "r", "BIT");
                        break;
                    // (wB)
                    case "OUT": case "OUTIM": case "ALT": case "ALTP":
                        this.flag1 = ToCStyle(args[1], "w", "BIT");
                        break;
                    // (rW)
                    case "DTCH":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        break;
                    // (wB, rW)
                    case "SET": case "SETIM": case "RST": case "RSTIM":
                        this.flag1 = ToCStyle(args[1], "w", "BIT");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        break;
                    // (rW, rW)
                    case "LDWEQ":case "LDWNE":case "LDWGE":case "LDWLE":case "LDWG":case "LDWL":
                    case "AWEQ":case "AWNE":case "AWGE":case "AWLE":case "AWG":case "AWL":
                    case "ORWEQ": case "ORWNE":case "ORWGE":case "ORWLE":case "ORWG":case "ORWL":
                    case "ATCH":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        break;
                    // (rD, rD)
                    case "LDDEQ":case "LDDNE":case "LDDGE":case "LDDLE":case "LDDG":case "LDDL":
                    case "ADEQ":case "ADNE":case "ADGE":case "ADLE":case "ADG":case "ADL":
                    case "ORDEQ":case "ORDNE":case "ORDGE":case "ORDLE": case "ORDG":case "ORDL":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        break;
                    // (rF, rF)
                    case "LDFEQ": case "LDFNE":case "LDFGE":case "LDFLE":case "LDFG":case "LDFL":
                    case "AFEQ":case "AFNE":case "AFGE":case "AFLE":case "AFG":case "AFL":
                    case "ORFEQ":case "ORFNE":case "ORFGE":case "ORFLE":case "ORFG":case "ORFL":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "r", "FLOAT");
                        break;
                    // (rW, wD)
                    case "WTOD":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rD, wF)
                    case "DTOW":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "w", "WORD");
                        break;
                    // (rD, wF)
                    case "DTOF":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "w", "FLOAT");
                        break;
                    // (rD, wD)
                    /*
                    case "BIN": case "BCD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        break;
                    */
                    // (rF, wD)
                    case "ROUND": case "TURNC":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rW, wW)
                    case "BIN": case "BCD":
                    case "INVW": case "MOV":
                    case "INC": case "DEC":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "w", "WORD");
                        break;
                    // (rD, wD)
                    case "INVD": case "MOVD":
                    case "INCD": case "DECD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rF, wF)
                    case "MOVF":
                    case "SQRT": case "SIN": case "COS": case "TAN": case "LN": case "EXP":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "w", "FLOAT");
                        break;
                    // (rW, rW, wW)
                    case "ADD": case "SUB": case "MULW": case "DIVW":
                    case "ANDW": case "ORW": case "XORW":
                    case "SHL": case "SHR": case "ROL": case "ROR":　
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCStyle(args[3], "w", "WORD");
                        break;
                    // (rD, rD, wD)
                    case "ADDD": case "SUBD": case "MULD": case "DIVD":
                    case "ANDD": case "ORD": case "XORD":
                    case "SHLD": case "SHRD": case "ROLD": case "RORD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        this.flag3 = ToCStyle(args[3], "w", "DWORD");
                        break;
                    // (rW, rW, wD)
                    case "MUL": case "DIV":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCStyle(args[3], "w", "DWORD");
                        break;
                    // (rF, rF, wF)
                    case "ADDF": case "SUBF": case "MULF": case "DIVF":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "r", "FLOAT");
                        this.flag3 = ToCStyle(args[3], "w", "FLOAT");
                        break;
                    // (rW, wW, rW)
                    case "MVBLK":  case "FMOV":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "w", "WORD");
                        this.flag3 = ToCStyle(args[3], "r", "WORD");
                        break;
                    // (rD, wD, rD)
                    case "MVDBLK": case "FMOVD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        this.flag3 = ToCStyle(args[3], "r", "DWORD");
                        break;
                    // (rwW, rW, rwB)
                    /*
                     * TON, TONR, TOF这三个计时器比较特殊
                     * 首先，TV这个计时寄存器必须是可读可写的
                     * 计时目标是可读的，除此之外，还要有计时开关位T来当第三个参数
                     * 但是参数省略了T，可以通过TV的编号来得到T的编号
                     */
                    case "TON": case "TONR": case "TOF":
                    /*
                     * CTU, CTD, CTUD三个计数器和计数器的结构大致相同
                     * 可放在一块处理
                     */
                    case "CTU": case "CTD": case "CTUD":
                        this.flag1 = ToCStyle(args[1], "rw", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        // T/C + 地址
                        this.flag3 = ToCStyle(args[1][0] + args[1].Substring(2), "rw", "BIT");
                        break;
                    // (rW)
                    case "FOR": case "JMP": case "LBL": case "CALL":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        break;
                    // ()
                    case "NEXT": case "EI": case "DI":
                        break;
                    // (rS, rwW, rwB)
                    /*
                     * 调用c程序比较特殊，因为要指定c程序的名称
                     * 所以第一个参数为名称，剩下两个分别为D参数和M参数
                     */
                    case "CALLM":
                        this.flag1 = args[1];
                        this.flag2 = ToCStyle(args[2], "rw", "WORD");
                        this.flag3 = ToCStyle(args[3], "rw", "BIT");
                        break;
                    // (rW, rW, rW, wW, rW)
                    case "SMOV":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[1], "r", "WORD");
                        this.flag3 = ToCStyle(args[1], "r", "WORD");
                        this.flag4 = ToCStyle(args[1], "w", "WORD");
                        this.flag5 = ToCStyle(args[1], "r", "WORD");
                        break;
                    // (rS)
                    case "FUNC":
                        this.flag1 = args[1];
                        break;
                }
                /*
                 * 注意如果是复位(RST)了计数器的位(C)的话
                 * 会影响对应标号的计数器值
                 * 所以标号需要记录到另外的参数
                 */
                if (Type.Length > 2 && Type.Substring(0, 3).Equals("RST") && this.flag1[0] == 'C')
                {
                    this.flag3 = args[1].Substring(1);
                }
                /*
                 * 
                 */
                if (Type.Length > 2 && Type.Substring(0, 2).Equals("CT"))
                {
                    this.flag4 = args[1].Substring(2);
                }
            }
        }
        
        /// <summary>
        /// 指令类型
        /// </summary>
        public string Type
        {
            get { return this.type; }
        }
        /// <summary>
        /// 指令参数（0为类型，1-4分别对应四个参数）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string this[int id]
        {
            get
            {
                switch (id)
                {
                    case 0: return type;
                    case 1: return flag1;
                    case 2: return flag2;
                    case 3: return flag3;
                    case 4: return flag4;
                    case 5: return flag5;
                    default: return "0";
                }
            }
        }
        /// <summary>
        /// 输出写入使能（仿真模式下使用）
        /// </summary>
        public string EnBit
        {
            get { return this.enbit; }
        }
        /// <summary>
        /// 将常量/变量名称转变为c语言可识别的形式
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="mode">访问变量的权限（r：读，w：写，rw：读写）</param>
        /// <param name="ctype">要转换成的c语言类型</param>
        /// <returns>c语言格式</returns>
        private string ToCStyle(string var, string mode="rw", string ctype="WORD")
        {
            // 找到最后一个字母
            int i = 0;
            while (i < var.Length && Char.IsLetter(var[i])) i++;
            // 确定前面的类型名称和后面的数值
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 如果该参数可写，需要附带写入使能
            if (mode.Equals("w") || mode.Equals("rw"))
            {
                this.enbit = String.Format("{0:s}Enable[{1:d}]", name, addr);
            }
            switch (name)
            {
                // 位线圈
                case "X": case "Y": case "M": case "C": case "T": case "B":
                    switch (ctype)
                    {
                        case "BIT": return String.Format("{0:s}Bit[{1:d}]", name, addr);
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                // 16位寄存器
                case "D": case "CV": case "TV":
                    /*
                     * 若要类型转换需要得到地址，所以要分为三个步骤
                     * 1. 获得这个变量的地址Word+addr
                     * 2. 将这个地址的指针转换成需要的类型的指正(type*)(Word+addr)
                     * 3. 最后再根据转换后的指针来取值*(type*)(Word)
                     */
                     switch (ctype)
                     {
                        case "WORD": return String.Format("{0:s}Word[{1:d}]", name, addr);
                        case "DWORD": return String.Format("(*((uint32_t*)({0:s}Word+{1:d})))", name, addr);
                        case "FLOAT": return String.Format("(*((float*)({0:s}Word+{1:d})))", name, addr);
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                // 32位寄存器
                case "CV32":
                    switch (ctype)
                    {
                        case "WORD": return String.Format("(*((uint16_t*)({0:s}CVDoubleWord+{1:d})))", name, addr);
                        case "DWORD": return String.Format("{0:s}CVDoubleWords[{1:d}]", name, addr);
                        case "FLOAT": return String.Format("(*((float*)({0:s}CVDoubleWord+{1:d})))", name, addr);
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                case "K": case "F":
                    if (mode.Equals("r"))
                        return var.Substring(i);
                    else
                        throw new ArgumentException("{0:s} cannot be wrote.\n", var);
                case "H":
                    if (mode.Equals("r"))
                        return "0x" + var.Substring(i);
                    else
                        throw new ArgumentException("{0:s} cannot be wrote.\n", var);
                default:
                    return var;
            }
        }
        
        public PLCOriginInst ToOrigin()
        {
            PLCOriginInst ret = new PLCOriginInst(text);
            ret.ProtoType = ProtoType;
            return ret;
        }

        public PLCInstruction ReplaceFlag(int id, string flag)
        {
            PLCOriginInst oinst = ToOrigin();
            string _text = String.Empty;
            for (int i = 0; i < 6; i++)
            {
                _text += String.Format("{0:s} ", i == id ? flag : oinst[i]);
            }
            PLCInstruction ret = new PLCInstruction(_text);
            ret.ProtoType = ProtoType;
            return ret;
        }
        
        public static int FlagNumber(string text)
        {
            switch (text)
            {
                // (rB)
                case "LD":
                case "AND":
                case "OR":
                case "LDI":
                case "ANDI":
                case "ORI":
                case "LDIM":
                case "ANDIM":
                case "ORIM":
                case "LDIIM":
                case "ANDIIM":
                case "ORIIM":
                case "LDP":
                case "ANDP":
                case "ORP":
                case "LDF":
                case "ANDF":
                case "ORF":
                    return 1;
                // (wB)
                case "OUT":
                case "OUTIM":
                case "ALT":
                case "ALTP":
                    return 1;
                // (rW)
                case "DTCH":
                    return 1;
                // (wB, rW)
                case "SET":
                case "SETIM":
                case "RST":
                case "RSTIM":
                    return 2;
                // (rW, rW)
                case "LDWEQ":
                case "LDWNE":
                case "LDWGE":
                case "LDWLE":
                case "LDWG":
                case "LDWL":
                case "AWEQ":
                case "AWNE":
                case "AWGE":
                case "AWLE":
                case "AWG":
                case "AWL":
                case "ORWEQ":
                case "ORWNE":
                case "ORWGE":
                case "ORWLE":
                case "ORWG":
                case "ORWL":
                case "ATCH":
                    return 2;
                // (rD, rD)
                case "LDDEQ":
                case "LDDNE":
                case "LDDGE":
                case "LDDLE":
                case "LDDG":
                case "LDDL":
                case "ADEQ":
                case "ADNE":
                case "ADGE":
                case "ADLE":
                case "ADG":
                case "ADL":
                case "ORDEQ":
                case "ORDNE":
                case "ORDGE":
                case "ORDLE":
                case "ORDG":
                case "ORDL":
                    return 2;
                // (rF, rF)
                case "LDFEQ":
                case "LDFNE":
                case "LDFGE":
                case "LDFLE":
                case "LDFG":
                case "LDFL":
                case "AFEQ":
                case "AFNE":
                case "AFGE":
                case "AFLE":
                case "AFG":
                case "AFL":
                case "ORFEQ":
                case "ORFNE":
                case "ORFGE":
                case "ORFLE":
                case "ORFG":
                case "ORFL":
                    return 2;
                // (rW, wD)
                case "WTOD":
                    return 2;
                // (rD, wF)
                case "DTOW":
                    return 2;
                // (rD, wF)
                case "DTOF":
                    return 2;
                // (rD, wD)
                /*
                case "BIN": case "BCD":
                    this.flag1 = ToCStyle(args[1], "r", "DWORD");
                    this.flag2 = ToCStyle(args[2], "w", "DWORD");
                    break;
                */
                // (rF, wD)
                case "ROUND":
                case "TURNC":
                    return 2;
                // (rW, wW)
                case "BIN":
                case "BCD":
                case "INVW":
                case "MOV":
                case "INC":
                case "DEC":
                    return 2;
                // (rD, wD)
                case "INVD":
                case "MOVD":
                case "INCD":
                case "DECD":
                    return 2;
                // (rF, wF)
                case "MOVF":
                case "SQRT":
                case "SIN":
                case "COS":
                case "TAN":
                case "LN":
                case "EXP":
                    return 2;
                // (rW, rW, wW)
                case "ADD":
                case "SUB":
                case "MULW":
                case "DIVW":
                case "ANDW":
                case "ORW":
                case "XORW":
                case "SHL":
                case "SHR":
                case "ROL":
                case "ROR":
                    return 3;
                // (rD, rD, wD)
                case "ADDD":
                case "SUBD":
                case "MULD":
                case "DIVD":
                case "ANDD":
                case "ORD":
                case "XORD":
                case "SHLD":
                case "SHRD":
                case "ROLD":
                case "RORD":
                    return 3;
                // (rW, rW, wD)
                case "MUL":
                case "DIV":
                    return 3;
                // (rF, rF, wF)
                case "ADDF":
                case "SUBF":
                case "MULF":
                case "DIVF":
                    return 3;
                // (rW, wW, rW)
                case "MVBLK":
                case "FMOV":
                    return 3;
                // (rD, wD, rD)
                case "MVDBLK":
                case "FMOVD":
                    return 3;
                // (rwW, rW, rwB)
                /*
                 * TON, TONR, TOF这三个计时器比较特殊
                 * 首先，TV这个计时寄存器必须是可读可写的
                 * 计时目标是可读的，除此之外，还要有计时开关位T来当第三个参数
                 * 但是参数省略了T，可以通过TV的编号来得到T的编号
                 */
                case "TON":
                case "TONR":
                case "TOF":
                /*
                 * CTU, CTD, CTUD三个计数器和计数器的结构大致相同
                 * 可放在一块处理
                 */
                case "CTU":
                case "CTD":
                case "CTUD":
                    return 3;
                // (rW)
                case "FOR":
                case "JMP":
                case "LBL":
                case "CALL":
                    return 1;
                // ()
                case "NEXT":
                case "EI":
                case "DI":
                    break;
                // (rS, rwW, rwB)
                /*
                 * 调用c程序比较特殊，因为要指定c程序的名称
                 * 所以第一个参数为名称，剩下两个分别为D参数和M参数
                 */
                case "CALLM":
                    return 3;
                // (rW, rW, rW, wW, rW)
                case "SMOV":
                    return 5;
                // (rS)
                case "FUNC":
                    return 1;
            }
            return 0;
        }

        public void UpdatePrototype()
        {
            if (ProtoType == null)
                return;
            if ((Type == "LD" || Type == "AND" || Type == "OR") && ProtoType is LDViewModel)
            {
                ((LDViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if ((Type == "LDI" || Type == "ANDI" || Type == "ORI") && ProtoType is LDIViewModel)
            {
                ((LDIViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if ((Type == "LDIM" || Type == "ANDIM" || Type == "ORIM") && ProtoType is LDIMViewModel)
            {
                ((LDIMViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if ((Type == "LDIIM" || Type == "ANDIIM" || Type == "ORIIM") && ProtoType is LDIIMViewModel)
            {
                ((LDIIMViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if ((Type == "LDP" || Type == "ANDP" || Type == "ORP") && ProtoType is LDPViewModel)
            {
                ((LDPViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if ((Type == "LDF" || Type == "ANDF" || Type == "ORF") && ProtoType is LDFViewModel)
            {
                ((LDFViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if (Type == "OUT" && ProtoType is OUTViewModel)
            {
                ((OUTViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if (Type == "OUTIM" && ProtoType is OUTIMViewModel)
            {
                ((OUTIMViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if (Type == "ALTP" && ProtoType is ALTPViewModel)
            {
                ((ALTPViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if (Type == "ALT" && ProtoType is ALTViewModel)
            {
                ((ALTViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
            }
            else if (Type == "SET" && ProtoType is SETViewModel)
            {
                ((SETViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
                ((SETViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "SETIM" && ProtoType is SETIMViewModel)
            {
                ((SETIMViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
                ((SETIMViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "RST" && ProtoType is RSTViewModel)
            {
                ((RSTViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
                ((RSTViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "RSTIM" && ProtoType is RSTIMViewModel)
            {
                ((RSTIMViewModel)(ProtoType)).Value = (BitValue)(CreateValueModel(oflag1, "BIT"));
                ((RSTIMViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "INV" && ProtoType is INVViewModel)
            {

            }
            else if (Type == "MEP" && ProtoType is MEPViewModel)
            {

            }
            else if (Type == "MEF" && ProtoType is MEFViewModel)
            {

            }
            else if ((Type == "LDWEQ" || Type == "AWEQ" || Type == "ORWEQ") && ProtoType is LDWEQViewModel)
            {
                ((LDWEQViewModel)(ProtoType)).Value1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((LDWEQViewModel)(ProtoType)).Value2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if ((Type == "LDFEQ" || Type == "AFEQ" || Type == "ORFEQ") && ProtoType is LDFEQViewModel)
            {
                ((LDFEQViewModel)(ProtoType)).Value1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LDFEQViewModel)(ProtoType)).Value2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if ((Type == "LDDEQ" || Type == "ADEQ" || Type == "ORDEQ") && ProtoType is LDDEQViewModel)
            {
                ((LDDEQViewModel)(ProtoType)).Value1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((LDDEQViewModel)(ProtoType)).Value2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if ((Type == "LDWNE" || Type == "AWNE" || Type == "ORWNE") && ProtoType is LDWNEViewModel)
            {
                ((LDWNEViewModel)(ProtoType)).Value1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((LDWNEViewModel)(ProtoType)).Value2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if ((Type == "LDFNE" || Type == "AFNE" || Type == "ORFNE") && ProtoType is LDFNEViewModel)
            {
                ((LDFNEViewModel)(ProtoType)).Value1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LDFNEViewModel)(ProtoType)).Value2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if ((Type == "LDDNE" || Type == "ADNE" || Type == "ORDNE") && ProtoType is LDDNEViewModel)
            {
                ((LDDNEViewModel)(ProtoType)).Value1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((LDDNEViewModel)(ProtoType)).Value2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if ((Type == "LDWLE" || Type == "AWLE" || Type == "ORWLE") && ProtoType is LDWLEViewModel)
            {
                ((LDWLEViewModel)(ProtoType)).Value1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((LDWLEViewModel)(ProtoType)).Value2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if ((Type == "LDFLE" || Type == "AFLE" || Type == "ORFLE") && ProtoType is LDFLEViewModel)
            {
                ((LDFLEViewModel)(ProtoType)).Value1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LDFLEViewModel)(ProtoType)).Value2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if ((Type == "LDDLE" || Type == "ADLE" || Type == "ORDLE") && ProtoType is LDDLEViewModel)
            {
                ((LDDLEViewModel)(ProtoType)).Value1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((LDDLEViewModel)(ProtoType)).Value2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if ((Type == "LDWGE" || Type == "AWGE" || Type == "ORWGE") && ProtoType is LDWGEViewModel)
            {
                ((LDWGEViewModel)(ProtoType)).Value1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((LDWGEViewModel)(ProtoType)).Value2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if ((Type == "LDFGE" || Type == "AFGE" || Type == "ORFGE") && ProtoType is LDFGEViewModel)
            {
                ((LDFGEViewModel)(ProtoType)).Value1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LDFGEViewModel)(ProtoType)).Value2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if ((Type == "LDDGE" || Type == "ADGE" || Type == "ORDGE") && ProtoType is LDDGEViewModel)
            {
                ((LDDGEViewModel)(ProtoType)).Value1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((LDDGEViewModel)(ProtoType)).Value2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if ((Type == "LDWL" || Type == "AWL" || Type == "ORWL") && ProtoType is LDWLViewModel)
            {
                ((LDWLViewModel)(ProtoType)).Value1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((LDWLViewModel)(ProtoType)).Value2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if ((Type == "LDFL" || Type == "AFL" || Type == "ORFL") && ProtoType is LDFLViewModel)
            {
                ((LDFLViewModel)(ProtoType)).Value1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LDFLViewModel)(ProtoType)).Value2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if ((Type == "LDDL" || Type == "ADL" || Type == "ORDL") && ProtoType is LDDLViewModel)
            {
                ((LDDLViewModel)(ProtoType)).Value1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((LDDLViewModel)(ProtoType)).Value2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if ((Type == "LDWG" || Type == "AWG" || Type == "ORWG") && ProtoType is LDWGViewModel)
            {
                ((LDWGViewModel)(ProtoType)).Value1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((LDWGViewModel)(ProtoType)).Value2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if ((Type == "LDFG" || Type == "AFG" || Type == "ORFG") && ProtoType is LDFGViewModel)
            {
                ((LDFGViewModel)(ProtoType)).Value1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LDFGViewModel)(ProtoType)).Value2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if ((Type == "LDDG" || Type == "ADG" || Type == "ORDG") && ProtoType is LDDGViewModel)
            {
                ((LDDGViewModel)(ProtoType)).Value1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((LDDGViewModel)(ProtoType)).Value2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if (Type == "BCD" && ProtoType is BCDViewModel)
            {
                ((BCDViewModel)(ProtoType)).InputValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((BCDViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "BIN" && ProtoType is BINViewModel)
            {
                ((BINViewModel)(ProtoType)).InputValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((BINViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "DTOF" && ProtoType is DTOFViewModel)
            {
                ((DTOFViewModel)(ProtoType)).InputValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((DTOFViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "DTOW" && ProtoType is DTOWViewModel)
            {
                ((DTOWViewModel)(ProtoType)).InputValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((DTOWViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "ROUND" && ProtoType is ROUNDViewModel)
            {
                ((ROUNDViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((ROUNDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if (Type == "TRUNC" && ProtoType is TRUNCViewModel)
            {
                ((TRUNCViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((TRUNCViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            /*
            else if (Type == "CTD" && ProtoType is CTDViewModel)
            {

            }
            else if (Type == "CTU" && ProtoType is CTUDViewModel)
            {

            }
            else if (Type == "CTUD" && ProtoType is CTUDViewModel)
            {

            }
            */
            else if (Type == "ADDF" && ProtoType is ADDFViewModel)
            {
                ((ADDFViewModel)(ProtoType)).InputValue1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((ADDFViewModel)(ProtoType)).InputValue2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
                ((ADDFViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag3, "FLOAT"));
            }
            else if (Type == "SUBF" && ProtoType is SUBFViewModel)
            {
                ((SUBFViewModel)(ProtoType)).InputValue1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((SUBFViewModel)(ProtoType)).InputValue2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
                ((SUBFViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag3, "FLOAT"));
            }
            else if (Type == "MULF" && ProtoType is MULFViewModel)
            {
                ((MULFViewModel)(ProtoType)).InputValue1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((MULFViewModel)(ProtoType)).InputValue2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
                ((MULFViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag3, "FLOAT"));
            }
            else if (Type == "DIVF" && ProtoType is DIVFViewModel)
            {
                ((DIVFViewModel)(ProtoType)).InputValue1 = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((DIVFViewModel)(ProtoType)).InputValue2 = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
                ((DIVFViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag3, "FLOAT"));
            }
            else if (Type == "SIN" && ProtoType is SINViewModel)
            {
                ((SINViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((SINViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "COS" && ProtoType is COSViewModel)
            {
                ((COSViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((COSViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "TAN" && ProtoType is TANViewModel)
            {
                ((TANViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((TANViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "LN" && ProtoType is LNViewModel)
            {
                ((LNViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((LNViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "EXP" && ProtoType is EXPViewModel)
            {
                ((EXPViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((EXPViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "SQRT" && ProtoType is SQRTViewModel)
            {
                ((SQRTViewModel)(ProtoType)).InputValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((SQRTViewModel)(ProtoType)).OutputValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "ADD" && ProtoType is ADDViewModel)
            {
                ((ADDViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((ADDViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((ADDViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "ADDD" && ProtoType is ADDDViewModel)
            {
                ((ADDDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((ADDDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((ADDDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "SUB" && ProtoType is SUBViewModel)
            {
                ((SUBViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((SUBViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((SUBViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "SUBD" && ProtoType is SUBDViewModel)
            {
                ((SUBDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((SUBDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((SUBDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "MUL" && ProtoType is MULViewModel)
            {
                ((MULViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((MULViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((MULViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "MULD" && ProtoType is MULDViewModel)
            {
                ((MULDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((MULDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((MULDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "MULW" && ProtoType is MULWViewModel)
            {
                ((MULWViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((MULWViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((MULWViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "DIV" && ProtoType is DIVViewModel)
            {
                ((DIVViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((DIVViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((DIVViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "DIVD" && ProtoType is DIVDViewModel)
            {
                ((SUBDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((SUBDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((SUBDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "INC" && ProtoType is INCViewModel)
            {
                ((INCViewModel)(ProtoType)).InputValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((INCViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "INCD" && ProtoType is INCDViewModel)
            {
                ((INCDViewModel)(ProtoType)).InputValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((INCDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if (Type == "DEC" && ProtoType is DECViewModel)
            {
                ((DECViewModel)(ProtoType)).InputValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((DECViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "DECD" && ProtoType is DECDViewModel)
            {
                ((DECDViewModel)(ProtoType)).InputValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((DECDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if (Type == "ANDW" && ProtoType is ANDWViewModel)
            {
                ((ANDWViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((ANDWViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((ANDWViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "ANDD" && ProtoType is ANDDViewModel)
            {
                ((ANDDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((ANDDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((ANDDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "ORW" && ProtoType is ORWViewModel)
            {
                ((ORWViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((ORWViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((ORWViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "ORD" && ProtoType is ORDViewModel)
            {
                ((ORDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((ORDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((ORDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "XORW" && ProtoType is XORWViewModel)
            {
                ((XORWViewModel)(ProtoType)).InputValue1 = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((XORWViewModel)(ProtoType)).InputValue2 = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((XORWViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "XORD" && ProtoType is XORDViewModel)
            {
                ((XORDViewModel)(ProtoType)).InputValue1 = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((XORDViewModel)(ProtoType)).InputValue2 = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((XORDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag3, "DWORD"));
            }
            else if (Type == "INVW" && ProtoType is INVWViewModel)
            {
                ((INVWViewModel)(ProtoType)).InputValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((INVWViewModel)(ProtoType)).OutputValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "INVD" && ProtoType is INVDViewModel)
            {
                ((INVDViewModel)(ProtoType)).InputValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((INVDViewModel)(ProtoType)).OutputValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if (Type == "MOV" && ProtoType is MOVViewModel)
            {
                ((MOVViewModel)(ProtoType)).SourceValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((MOVViewModel)(ProtoType)).DestinationValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
            }
            else if (Type == "MOVD" && ProtoType is MOVDViewModel)
            {
                ((MOVDViewModel)(ProtoType)).SourceValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((MOVDViewModel)(ProtoType)).DestinationValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
            }
            else if (Type == "MOVF" && ProtoType is MOVFViewModel)
            {
                ((MOVFViewModel)(ProtoType)).SourceValue = (FloatValue)(CreateValueModel(oflag1, "FLOAT"));
                ((MOVFViewModel)(ProtoType)).DestinationValue = (FloatValue)(CreateValueModel(oflag2, "FLOAT"));
            }
            else if (Type == "MVBLK" && ProtoType is MVBLKViewModel)
            {
                ((MVBLKViewModel)(ProtoType)).SourceValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((MVBLKViewModel)(ProtoType)).DestinationValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((MVBLKViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "MVDBLK" && ProtoType is MVBLKViewModel)
            {
                ((MVDBLKViewModel)(ProtoType)).SourceValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((MVDBLKViewModel)(ProtoType)).DestinationValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((MVDBLKViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "CALL" && ProtoType is CALLViewModel)
            {
                //((CALLViewModel)(ProtoType)).FunctionName = flag1;
            }
            else if (Type == "CALLM" && ProtoType is CALLViewModel)
            {
                //((CALLMViewModel)(ProtoType)).FunctionName = flag1;
            }
            else if (Type == "FOR" && ProtoType is FORViewModel)
            {
                ((FORViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag1, "WORD"));
            }
            else if (Type == "JMP" && ProtoType is JMPViewModel)
            {
                ((JMPViewModel)(ProtoType)).LBLIndex = (WordValue)(CreateValueModel(oflag1, "WORD"));
            }
            else if (Type == "LBL" && ProtoType is LBLViewModel)
            {
                ((LBLViewModel)(ProtoType)).LBLIndex = (WordValue)(CreateValueModel(oflag1, "WORD"));
            }
            else if (Type == "NEXT" && ProtoType is NEXTViewModel)
            {

            }
            else if (Type == "TRD" && ProtoType is TRDViewModel)
            {
                ((TRDViewModel)(ProtoType)).StartValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
            }
            else if (Type == "TWR" && ProtoType is TWRViewModel)
            {
                ((TWRViewModel)(ProtoType)).StartValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
            }
            else if (Type == "ROL" && ProtoType is ROLViewModel)
            {
                ((ROLViewModel)(ProtoType)).SourceValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((ROLViewModel)(ProtoType)).DestinationValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((ROLViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "ROLD" && ProtoType is ROLDViewModel)
            {
                ((ROLDViewModel)(ProtoType)).SourceValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((ROLDViewModel)(ProtoType)).DestinationValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((ROLDViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "ROR" && ProtoType is RORViewModel)
            {
                ((RORViewModel)(ProtoType)).SourceValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((RORViewModel)(ProtoType)).DestinationValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((RORViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "RORD" && ProtoType is RORDViewModel)
            {
                ((RORDViewModel)(ProtoType)).SourceValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((RORDViewModel)(ProtoType)).DestinationValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((RORDViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "SHL" && ProtoType is SHLViewModel)
            {
                ((SHLViewModel)(ProtoType)).SourceValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((SHLViewModel)(ProtoType)).DestinationValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((SHLViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "SHLD" && ProtoType is SHLDViewModel)
            {
                ((SHLDViewModel)(ProtoType)).SourceValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((SHLDViewModel)(ProtoType)).DestinationValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((SHLDViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "SHR" && ProtoType is SHRViewModel)
            {
                ((SHRViewModel)(ProtoType)).SourceValue = (WordValue)(CreateValueModel(oflag1, "WORD"));
                ((SHRViewModel)(ProtoType)).DestinationValue = (WordValue)(CreateValueModel(oflag2, "WORD"));
                ((SHRViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else if (Type == "SHRD" && ProtoType is SHRDViewModel)
            {
                ((SHRDViewModel)(ProtoType)).SourceValue = (DoubleWordValue)(CreateValueModel(oflag1, "DWORD"));
                ((SHRDViewModel)(ProtoType)).DestinationValue = (DoubleWordValue)(CreateValueModel(oflag2, "DWORD"));
                ((SHRDViewModel)(ProtoType)).Count = (WordValue)(CreateValueModel(oflag3, "WORD"));
            }
            else
            {
                PrototypeOutOfDateException exc = new PrototypeOutOfDateException();
                exc.Prototype_old = ProtoType;
                exc.Prototype_new = LadderInstViewModelPrototype.Clone(Type);
                exc.Prototype_new.X = exc.Prototype_old.X;
                exc.Prototype_new.Y = exc.Prototype_old.Y;
                throw exc;
            }
            //ProtoType.Model = model;
        }

        public IValueModel CreateValueModel(string name, string type)
        {
            switch (type)
            {
                case "BIT": return ValueParser.ParseBitValue(name);
                case "WORD": return ValueParser.ParseWordValue(name);
                case "DWORD": return ValueParser.ParseDoubleWordValue(name);
                case "FLOAT": return ValueParser.ParseFloatValue(name);
                default: return null;
            }
        }
        
    }
    
    public class PrototypeOutOfDateException : Exception
    {
        public BaseViewModel Prototype_old { get; set; }
        public BaseViewModel Prototype_new { get; set; }
    }
}
