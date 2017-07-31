using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSoarII.Core.Generate
{
    public class PLCInstruction
    {
        /// <summary> 私有成员 </summary>
        protected LadderUnitModel prototype;
        protected int prototypeid;
        protected string text;
        protected string type;
        //protected string[] oargs;
        protected string[] args;
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
        public LadderUnitModel ProtoType
        {
            get
            {
                return this.prototype;
            }
            set
            {
                if (prototype == value) return;
                LadderUnitModel _prototype = prototype;
                this.prototype = null;
                if (_prototype != null && _prototype.Inst != null)
                    _prototype.Inst = null;
                this.prototype = value;
                if (prototype != null && prototype.Inst != this)
                    prototype.Inst = this;
            }
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
        /// 用于显示的指令
        /// </summary>
        private PLCOriginInst origin;
        public PLCOriginInst Origin
        {
            get
            {
                return this.origin;
            }
            set
            {
                if (origin == value) return;
                PLCOriginInst _origin = origin;
                this.origin = null;
                if (_origin != null && _origin.Inst != null)
                    _origin.Inst = null;
                this.origin = value;
                if (origin != null && origin.Inst != this)
                    origin.Inst = this;
            }
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
                this.text = value.ToUpper();
                args = text.Split(' ');
                this.type = args[0];
                this.enbit = "";
                //oargs = args.ToArray();
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
                    case "LD":
                    case "AND":
                    case "OR":
                    case "LDI":
                    case "ANDI":
                    case "ORI":
                    case "LDP":
                    case "ANDP":
                    case "ORP":
                    case "LDF":
                    case "ANDF":
                    case "ORF":
                        args[1] = ToCStyle(args[1], "r", "BIT");
                        break;
                    case "LDIM":
                    case "ANDIM":
                    case "ORIM":
                    case "LDIIM":
                    case "ANDIIM":
                    case "ORIIM":
                        args = new string[] { args[0], args[1], "" };
                        args[1] = ToCStyle(args[1], "r", "BIT");
                        args[2] = ToCIndex(args[1], "r", "BIT");
                        break;
                    // (wB)
                    case "OUT":
                    case "ALT":
                    case "ALTP":
                    case "PLSNEXT":
                    case "PLSSTOP":
                        args[1] = ToCStyle(args[1], "w", "BIT");
                        break;
                    case "OUTIM":
                        args = new string[] { args[0], args[1], "" };
                        args[1] = ToCStyle(args[1], "w", "BIT");
                        args[2] = ToCIndex(args[1], "w", "BIT");
                        break;
                    // (rW)
                    case "DTCH":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        break;
                    // (rwW)
                    case "TRD":
                    case "TWR":
                        args[1] = ToCStyle(args[1], "rw", "WORD");
                        break;
                    // (wB, rW)
                    case "SET":
                    case "RST":
                        args[1] = ToCStyle(args[1], "w", "BIT");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        break;
                    case "SETIM":
                    case "RSTIM":
                        args = new string[] { args[0], args[1], args[2], "" };
                        args[1] = ToCStyle(args[1], "w", "BIT");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCIndex(args[1], "w", "BIT");
                        break;
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
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        break;
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
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        break;
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
                        args[1] = ToCStyle(args[1], "r", "FLOAT");
                        args[2] = ToCStyle(args[2], "r", "FLOAT");
                        break;
                    // (rW, wD)
                    case "WTOD":
                    case "FACT":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rD, wF)
                    case "DTOW":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "w", "WORD");
                        break;
                    // (rD, wF)
                    case "DTOF":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "w", "FLOAT");
                        break;
                    // (rD, wD)
                    /*
                    case "BIN": case "BCD":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "w", "DWORD");
                        break;
                    */
                    // (rF, wD)
                    case "ROUND":
                    case "TRUNC":
                        args[1] = ToCStyle(args[1], "r", "FLOAT");
                        args[2] = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rW, wW)
                    case "BIN":
                    case "BCD":
                    case "INVW":
                    case "MOV":
                    case "INC":
                    case "DEC":
                    case "NEG":
                    case "CML":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "w", "WORD");
                        break;
                    // (rD, wD)
                    case "INVD":
                    case "MOVD":
                    case "INCD":
                    case "DECD":
                    case "NEGD":
                    case "CMLD":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "w", "DWORD");
                        break;
                    // (rF, wF)
                    case "MOVF":
                    case "SQRT":
                    case "SIN":
                    case "COS":
                    case "TAN":
                    case "LN":
                    case "EXP":
                    case "LOG":
                        args[1] = ToCStyle(args[1], "r", "FLOAT");
                        args[2] = ToCStyle(args[2], "w", "FLOAT");
                        break;
                    // (rwW, rwW)
                    case "XCH":
                        args[1] = ToCStyle(args[1], "rw", "WORD");
                        args[2] = ToCStyle(args[2], "rw", "WORD");
                        break;
                    // (rwD, rwD)
                    case "XCHD":
                        args[1] = ToCStyle(args[1], "rw", "DWORD");
                        args[2] = ToCStyle(args[2], "rw", "DWORD");
                        break;
                    // (rwF, rwF)
                    case "XCHF":
                        args[1] = ToCStyle(args[1], "rw", "FLOAT");
                        args[2] = ToCStyle(args[2], "rw", "FLOAT");
                        break;
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
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCStyle(args[3], "w", "WORD");
                        break;
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
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        args[3] = ToCStyle(args[3], "w", "DWORD");
                        break;
                    // (rW, rW, wD)
                    case "MUL":
                    case "DIV":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCStyle(args[3], "w", "DWORD");
                        break;
                    // (rF, rF, wF)
                    case "ADDF":
                    case "SUBF":
                    case "MULF":
                    case "DIVF":
                    case "POW":
                        args[1] = ToCStyle(args[1], "r", "FLOAT");
                        args[2] = ToCStyle(args[2], "r", "FLOAT");
                        args[3] = ToCStyle(args[3], "w", "FLOAT");
                        break;
                    // (rW, wW, rW)
                    case "MVBLK":
                    case "FMOV":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "w", "WORD");
                        args[3] = ToCStyle(args[3], "r", "WORD");
                        break;
                    // (rD, wD, rD)
                    case "MVDBLK":
                    case "FMOVD":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "w", "DWORD");
                        args[3] = ToCStyle(args[3], "r", "DWORD");
                        break;
                    // (rW, rW, wB)
                    case "CMP":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCStyle(args[3], "w", "BIT");
                        break;
                    // (rD, rD, wB)
                    case "CMPD":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        args[3] = ToCStyle(args[3], "w", "BIT");
                        break;
                    // (rF, rF, wB)
                    case "CMPF":
                        args[1] = ToCStyle(args[1], "r", "FLOAT");
                        args[2] = ToCStyle(args[2], "r", "FLOAT");
                        args[3] = ToCStyle(args[3], "w", "BIT");
                        break;
                    // (rW, rW, rW, wB)
                    case "ZCP":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCStyle(args[3], "r", "WORD");
                        args[4] = ToCStyle(args[4], "w", "BIT");
                        break;
                    // (rD, rD, rD, wB)
                    case "ZCPD":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        args[3] = ToCStyle(args[3], "r", "DWORD");
                        args[4] = ToCStyle(args[4], "w", "BIT");
                        break;
                    // (rF, rF, rF, wB)
                    case "ZCPF":
                        args[1] = ToCStyle(args[1], "r", "FLOAT");
                        args[2] = ToCStyle(args[2], "r", "FLOAT");
                        args[3] = ToCStyle(args[3], "r", "FLOAT");
                        args[4] = ToCStyle(args[4], "w", "BIT");
                        break;
                    // (rB, wB, rW, rW)
                    case "SHLB":
                    case "SHRB":
                        args[1] = ToCStyle(args[1], "r", "BIT");
                        args[2] = ToCStyle(args[2], "w", "BIT");
                        args[3] = ToCStyle(args[3], "r", "WORD");
                        args[4] = ToCStyle(args[4], "r", "WORD");
                        break;
                    // (rwW, rW, rwB)
                    case "TON":
                    case "TONR":
                    case "TOF":
                    case "HCNT":
                        args[1] = ToCIndex(args[1], "rw", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        break;
                    case "CTU":
                    case "CTD":
                    case "CTUD":
                        args[1] = ToCStyle(args[1], "rw", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        // T/C + 地址
                        args[3] = ToCStyle(args[1][0] + args[1].Substring(2), "rw", "BIT");
                        break;
                    // (rW)
                    case "FOR":
                    case "JMP":
                    case "LBL":
                    case "CALL":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        break;
                    // ()
                    case "NEXT":
                    case "EI":
                    case "DI":
                        break;
                    // (rS, rwW, rwB)
                    /*
                     * 调用c程序比较特殊，因为要指定c程序的名称和参数类型
                     * 所以第一个参数为名称，剩下的为c程序的参数
                     */
                    case "CALLM":
                        args[1] = args[1];
                        for (int i = 2; i < args.Length; i++)
                            args[i] = ToCStylePointer(args[i]);
                        break;
                    // (rW, rS)
                    case "ATCH":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = args[2];
                        break;
                    // (rW, rW, rW, wW, rW)
                    case "SMOV":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCStyle(args[3], "r", "WORD");
                        args[4] = ToCStyle(args[4], "w", "WORD");
                        args[5] = ToCStyle(args[5], "r", "WORD");
                        break;
                    // (rS)
                    case "FUNC":
                        args[1] = args[1];
                        break;
                    // (rW, rS, rwW)
                    case "MBUS":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = args[2];
                        args[3] = ToCStyle(args[3], "rw", "WORD");
                        break;
                    // (rW, rW, rW)
                    case "SEND":
                    case "REV":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCStyle(args[3], "r", "WORD");
                        break;
                    // (rW, wB)
                    case "PLSF":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCIndex(args[2], "w", "BIT");
                        break;
                    // (rD, wB)
                    case "DPLSF":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCIndex(args[2], "w", "BIT");
                        break;
                    // (rW, rW, wB)
                    case "PWM":
                    case "PLSY":
                    case "PLSR":
                    case "ZRN":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        break;
                    case "DRVI":
                    case "DRVA":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        args[4] = ToCIndex(args[4], "w", "BIT");
                        break;
                    // (rD, rD, wB)
                    case "DPWM":
                    case "DPLSY":
                    case "DPLSR":
                    case "DZRN":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        break;
                    case "DDRVI":
                    case "DDRVA":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        args[4] = ToCIndex(args[4], "w", "BIT");
                        break;
                    // (rW, rW, wB, wB)
                    case "PLSRD":
                    case "PLSA":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCStyle(args[2], "r", "WORD");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        args[4] = ToCIndex(args[4], "w", "BIT");
                        break;
                    // (rD, rD, wB, wB)
                    case "DPLSRD":
                    case "DPLSA":
                        args[1] = ToCStyle(args[1], "r", "DWORD");
                        args[2] = ToCStyle(args[2], "r", "DWORD");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        args[4] = ToCIndex(args[4], "w", "BIT");
                        break;
                    // (rW, wB, wB)
                    case "PTO":
                        args[1] = ToCStyle(args[1], "r", "WORD");
                        args[2] = ToCIndex(args[2], "w", "BIT");
                        args[3] = ToCIndex(args[3], "w", "BIT");
                        break;
                    case "STL":
                        args[1] = ToCStyle(args[1], "r", "BIT");
                        break;
                    case "ST":
                        args[1] = ToCStyle(args[1], "w", "BIT");
                        break;
                }
                /*
                 * 注意如果是复位(RST)了计数器的位(C)的话
                 * 会影响对应标号的计数器值
                 * 所以标号需要记录到另外的参数
                 */
                if (Type.Length > 2 && Type.Substring(0, 3).Equals("RST") && args[1][0] == 'C')
                {
                    args = args.Concat(new string[]{ args[1].Substring(1)}).ToArray();
                }
                if (Type.Length > 2 && Type.Substring(0, 2).Equals("CT"))
                {
                    args = args.Concat(new string[] { args[1].Substring(2) }).ToArray();
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
        /// 指令参数数量
        /// </summary>
        public int Count
        {
            get { return args.Count(); }
        }
        /// <summary>
        /// 指令参数（0为类型，1-5分别对应五个参数）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string this[int id]
        {
            get { return args[id]; }
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
            string name = null;
            int addrn = 0;
            string addr = null;
            if (m1.Success)
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
                case "X":
                case "Y":
                case "M":
                case "C":
                case "T":
                case "S":
                    switch (ctype)
                    {
                        case "BIT": return String.Format("{0:s}Bit[{1:s}]", name, addr);
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                case "D":
                case "TV":
                case "AI":
                case "AO":
                case "V":
                case "Z":
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
                            return String.Format("{0:s}DoubleWords[{1:s}]", name, addr);
                        case "FLOAT":
                            hasconvert = true;
                            return String.Format("(*(({2:s}*)({0:s}DoubleWord+{1:s})))", name, addr, wordsize <= 16 ? "float" : "double");
                        default: throw new ArgumentException(String.Format("Invalid variable {0:s} for type {1:s}", name, ctype));
                    }
                case "K":
                case "F":
                    if (mode.Equals("r"))
                        return addr.ToString();
                    else
                        throw new ArgumentException(String.Format("{0:s} cannot be wrote.\n", var));
                case "H":
                    if (mode.Equals("r"))
                        return "0x" + addr.ToString();
                    else
                        throw new ArgumentException(String.Format("{0:s} cannot be wrote.\n", var));
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
           return new PLCOriginInst(this, text);
        }
    }
}
