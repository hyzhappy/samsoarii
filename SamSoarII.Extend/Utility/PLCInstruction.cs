﻿using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// 一个单字(WORD)所占的空间
        /// </summary>
        private int wordsize = 16;
        /// <summary>
        /// 一个单字(WORD)所占的空间
        /// </summary>
        public int WordSize
        {
            get
            {
                return this.wordsize;
            }
            set
            {
                this.wordsize = value;
                if (hasconvert) Text = Text;
            }
        }
        /// <summary>
        /// 是否存在转换操作
        /// </summary>
        private bool hasconvert = false;
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
        /// 指令文本
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set
            {
                hasconvert = false;
                // 解析给定的文本，生成类型和四个参数
                this.text = value;
                string[] args = text.Split(' ');
                this.type = args[0];
                this.enbit = "";
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
                    case "PLSNEXT": case "PLSSTOP":
                        this.flag1 = ToCStyle(args[1], "w", "BIT");
                        break;
                    // (rW)
                    case "DTCH":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        break;
                    // (rwW)
                    case "TRD": case "TWR":
                        this.flag1 = ToCStyle(args[1], "rw", "WORD");
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
                    case "WTOD": case "FACT":
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
                    case "ROUND": case "TRUNC":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rW, wW)
                    case "BIN": case "BCD":
                    case "INVW": case "MOV":
                    case "INC": case "DEC":
                    case "NEG": case "CML":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "w", "WORD");
                        break;
                    // (rD, wD)
                    case "INVD": case "MOVD":
                    case "INCD": case "DECD":
                    case "NEGD": case "CMLD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rF, wF)
                    case "MOVF":
                    case "SQRT": case "SIN": case "COS": case "TAN": case "LN": case "EXP":
                    case "LOG":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "w", "FLOAT");
                        break;
                    // (rwW, rwW)
                    case "XCH":
                        this.flag1 = ToCStyle(args[1], "rw", "WORD");
                        this.flag2 = ToCStyle(args[2], "rw", "WORD");
                        break;
                    // (rwD, rwD)
                    case "XCHD":
                        this.flag1 = ToCStyle(args[1], "rw", "DWORD");
                        this.flag2 = ToCStyle(args[2], "rw", "DWORD");
                        break;
                    // (rwF, rwF)
                    case "XCHF":
                        this.flag1 = ToCStyle(args[1], "rw", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "rw", "FLOAT");
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
                    case "ADDF": case "SUBF": case "MULF": case "DIVF": case "POW":
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
                    // (rW, rW, wB)
                    case "CMP":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCStyle(args[3], "w", "BIT");
                        break;
                    // (rD, rD, wB)
                    case "CMPD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        this.flag3 = ToCStyle(args[3], "w", "BIT");
                        break;
                    // (rF, rF, wB)
                    case "CMPF":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "r", "FLOAT");
                        this.flag3 = ToCStyle(args[3], "w", "BIT");
                        break;
                    // (rW, rW, rW, wB)
                    case "ZCP":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCStyle(args[3], "r", "WORD");
                        this.flag4 = ToCStyle(args[4], "w", "BIT");
                        break;
                    // (rD, rD, rD, wB)
                    case "ZCPD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        this.flag3 = ToCStyle(args[3], "r", "DWORD");
                        this.flag4 = ToCStyle(args[4], "w", "BIT");
                        break;
                    // (rF, rF, rF, wB)
                    case "ZCPF":
                        this.flag1 = ToCStyle(args[1], "r", "FLOAT");
                        this.flag2 = ToCStyle(args[2], "r", "FLOAT");
                        this.flag3 = ToCStyle(args[3], "r", "FLOAT");
                        this.flag4 = ToCStyle(args[4], "w", "BIT");
                        break;
                    // (rB, wB, rW, rW)
                    case "SHLB": case "SHRB":
                        this.flag1 = ToCStyle(args[1], "r", "BIT");
                        this.flag2 = ToCStyle(args[2], "w", "BIT");
                        this.flag3 = ToCStyle(args[3], "r", "WORD");
                        this.flag4 = ToCStyle(args[4], "r", "WORD");
                        break;
                    // (rwW, rW, rwB)
                    case "TON": case "TONR": case "TOF": case "HCNT":
                        this.flag1 = ToCIndex(args[1], "rw", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        break;
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
                     * 调用c程序比较特殊，因为要指定c程序的名称和参数类型
                     * 所以第一个参数为名称，剩下的为c程序的参数
                     */
                    case "CALLM":
                        this.flag1 = args[1];
                        this.flag2 = ToCStylePointer(args[2]);
                        this.flag3 = ToCStylePointer(args[3]);
                        this.flag4 = ToCStylePointer(args[4]);
                        this.flag5 = ToCStylePointer(args[5]);
                        break;
                    // (rW, rS)
                    case "ATCH":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = args[2];
                        break;
                    // (rW, rW, rW, wW, rW)
                    case "SMOV":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCStyle(args[3], "r", "WORD");
                        this.flag4 = ToCStyle(args[4], "w", "WORD");
                        this.flag5 = ToCStyle(args[5], "r", "WORD");
                        break;
                    // (rS)
                    case "FUNC":
                        this.flag1 = args[1];
                        break;
                    // (rW, rS, rwW)
                    case "MBUS":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = args[2];
                        this.flag3 = ToCStyle(args[3], "rw", "WORD");
                        break;
                    // (rW, rW, rW)
                    case "SEND": case "REV":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCStyle(args[3], "r", "WORD");
                        break;
                    // (rW, wB)
                    case "PLSF":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCIndex(args[2], "w", "BIT");
                        break;
                    // (rD, wB)
                    case "DPLSF":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCIndex(args[2], "w", "BIT");
                        break;
                    // (rW, rW, wB)
                    case "PWM": case "PLSY": case "PLSR": case "ZRN": case "DRVI":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCIndex(args[3], "w", "BIT");
                        break;
                    // (rD, rD, wB)
                    case "DPWM": case "DPLSY": case "DPLSR": case "DZRN": case "DDRVI":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        this.flag3 = ToCIndex(args[3], "w", "BIT");
                        break;
                    // (rW, rW, wB, wB)
                    case "PLSRD":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCStyle(args[2], "r", "WORD");
                        this.flag3 = ToCIndex(args[3], "w", "BIT");
                        this.flag4 = ToCIndex(args[4], "w", "BIT");
                        break;
                    // (rD, rD, wB, wB)
                    case "DPLSRD":
                        this.flag1 = ToCStyle(args[1], "r", "DWORD");
                        this.flag2 = ToCStyle(args[2], "r", "DWORD");
                        this.flag3 = ToCIndex(args[3], "w", "BIT");
                        this.flag4 = ToCIndex(args[4], "w", "BIT");
                        break;
                    // (rW, wB, wB)
                    case "PTO":
                        this.flag1 = ToCStyle(args[1], "r", "WORD");
                        this.flag2 = ToCIndex(args[2], "w", "BIT");
                        this.flag3 = ToCIndex(args[3], "w", "BIT");
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
        private string ToCStyle(string var, string mode = "rw", string ctype = "WORD")
        {
            Match m1 = Regex.Match(var, @"^([a-zA-Z]+)(\d+)$");
            Match m2 = Regex.Match(var, @"^([a-zA-Z]+)(\d+)(V|Z)(\d+)$");
            Match m3 = Regex.Match(var, @"^([KH])(.+)$");
            string name = null;
            int addrn = 0;
            string addr = null;
            if (m3.Success)
            {
                switch (m3.Groups[1].Value)
                {
                    case "K":
                        if (mode.Equals("r"))
                            return m3.Groups[2].Value;
                        else
                            throw new ArgumentException(String.Format("{0:s} cannot be wrote.\n", var));
                    case "H":
                        if (mode.Equals("r"))
                            return "0x" + m3.Groups[2].Value;
                        else
                            throw new ArgumentException(String.Format("{0:s} cannot be wrote.\n", var));
                }
            }
            else if (m1.Success)
            {
                name = m1.Groups[1].Value;
                addrn = int.Parse(m1.Groups[2].Value);
                if (name.Equals("CV") && addrn >= 200)
                {
                    name = "CV32"; addrn -= 200;
                }
                addr = addrn.ToString();
            }
            else if (m2.Success)
            {
                name = m2.Groups[1].Value;
                addrn = int.Parse(m2.Groups[2].Value);
                if (name.Equals("CV") && addrn >= 200)
                {
                    name = "CV32"; addrn -= 200;
                }
                addr = String.Format("{0:d}+{1:s}Word[{2:s}]",
                    addrn, m2.Groups[3].Value, m2.Groups[4].Value);
            }
            else
            {
                return var;
            }
            // 如果该参数可写，需要附带写入使能
            if (mode.Equals("w") || mode.Equals("rw"))
            {
                if (this.enbit.Equals(String.Empty))
                {
                    this.enbit = String.Format("{0:s}Enable[{1:s}]", name, addr);
                }
                else
                {
                    this.enbit += "||" + String.Format("{0:s}Enable[{1:s}]", name, addr);
                }
            }
            switch (name)
            {
                case "X": case "Y": case "M": case "C": case "T": case "S":
                    switch (ctype)
                    {
                        case "BIT": return String.Format("{0:s}Bit[{1:s}]", name, addr);
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                case "D": case "TV": case "AI": case "AO": case "V": case "Z":
                    /*
                     * 若要类型转换需要得到地址，所以要分为三个步骤
                     * 1. 获得这个变量的地址Word+addr
                     * 2. 将这个地址的指针转换成需要的类型的指正(type*)(Word+addr)
                     * 3. 最后再根据转换后的指针来取值*(type*)(Word)
                     */
                     switch (ctype)
                     {
                        case "WORD":
                            return String.Format("{0:s}Word[{1:s}]", name, addr);
                        case "DWORD":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}Word+{1:s})))", name, addr, wordsize <= 16 ? "int32_t" : "int64_t");
                        case "FLOAT":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}Word+{1:s})))", name, addr, wordsize <= 16 ? "float" : "double");
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                     }
                case "CV":
                    switch (ctype)
                    {
                        case "WORD":
                            return String.Format("{0:s}Word[{1:s}]", name, addr);
                        case "DWORD":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}Word+{1:s})))", name, addr, wordsize <= 16 ? "int32_t" : "int64_t");
                        case "FLOAT":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}Word+{1:s})))", name, addr, wordsize <= 16 ? "float" : "double");
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                case "CV32":
                    switch (ctype)
                    {
                        case "WORD":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}DoubleWord+{1:s})))", name, addr, wordsize <= 16 ? "int16_t" : "int32_t");
                        case "DWORD":
                            return String.Format("{0:s}DoubleWord[{1:s}]", name, addr);
                        case "FLOAT":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}DoubleWord+{1:s})))", name, addr, wordsize <= 16 ? "float" : "double");
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                default:
                    return var;
            }
        }
        
        private string ToCIndex(string var, string mode = "rw", string type = "WORD")
        {
            Match m1 = Regex.Match(var, @"^([a-zA-Z]+)(\d+)$");
            Match m2 = Regex.Match(var, @"^([a-zA-Z]+)(\d+)(V|Z)(\d+)$");
            if (m1.Success)
            {
                return m1.Groups[2].Value;
            }
            else if (m2.Success)
            {
                return String.Format("{0}+{1}Word[{2}]",
                    m1.Groups[2].Value, m1.Groups[3].Value, m1.Groups[4].Value);
            }
            else
            {
                return "0";
            }
        }
        /// <summary>
        /// 将变量名称转变为c语言指针
        /// </summary>
        /// <param name="var">变量名称</param>
        /// <returns></returns>
        private string ToCStylePointer(string var)
        {
            Match m1 = Regex.Match(var, @"^([a-zA-Z]+)(\d+)$");
            if (!m1.Success) return var;
            if (var.Equals(String.Empty))
                return String.Empty;
            // 找到最后一个字母
            int i = 0;
            while (i < var.Length && Char.IsLetter(var[i])) i++;
            // 确定前面的类型名称和后面的数值
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            switch (name)
            {
                // 位线圈
                case "X":
                case "Y":
                case "M":
                case "C":
                case "T":
                case "S":
                    return String.Format("&{0:s}Bit[{1:d}]", name, addr);
                // 16位寄存器
                case "D":
                case "TV":
                    return String.Format("&{0:s}Word[{1:d}]", name, addr);
                // 32位寄存器
                case "CV":
                    if (addr < 200)
                    {
                        return String.Format("&{0:s}Word[{1:d}]", name, addr);
                    }
                    else
                    {
                        return String.Format("&{0:s}DoubleWord[{1:d}]", name, addr);
                    }
                default:
                    throw new ArgumentException("{0:s} cannot be converted to pointer.\n", var);
            }
        }
        /// <summary>
        /// 转换成原型指令
        /// </summary>
        /// <returns></returns>
        public PLCOriginInst ToOrigin()
        {
            PLCOriginInst ret = new PLCOriginInst(text);
            ret.ProtoType = ProtoType;
            return ret;
        }
    }
}
