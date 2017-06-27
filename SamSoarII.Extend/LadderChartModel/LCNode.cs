using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstViewModel;

/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 梯形图的元件节点的信息
/// </remarks>

namespace SamSoarII.Extend.LadderChartModel
{
    public class LCNode : IPosition
    {
        /// <summary>
        /// 内部成员变量
        /// </summary>
        private BaseViewModel prototype; 

        private bool enable;

        private int id;
        private string type;
        private bool isstart;
        private bool isterminate;
        private string flag1, flag2, flag3, flag4, flag5;

        private int x;
        private int y;
        private LCNode up, down, left, right, riup, rido, leup, ledo;
        private bool horizontial_edge;
        private bool vertical_edge;

        //private int unodeid;
        //private int dnodeid;
        private int lnodeid;
        private int rnodeid;

        /// <summary>
        /// 初始化
        /// </summary>
        public LCNode(int _id)
        {
            this.enable = true;
            this.id = _id;
        }
        /// <summary>
        /// 原型
        /// </summary>
        public BaseViewModel Prototype
        {
            get { return this.prototype; }
            set { this.prototype = value; }
        }
        /// <summary>
        /// 这个图节点是否被占用
        /// </summary>
        public bool Enable
        {
            get { return this.enable;}
            set { this.enable = value; }
        }
        
        /// <summary>
        /// 节点的代号
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// 这个节点的PLC指令的类型
        /// 格式为0xAABB，A为指令种类的编号，B为指令详细功能的编号
        /// </summary>
        public string Type
        {
            get { return this.type;}
            set { this.type = value; }
        }

        /// <summary>
        /// 是否为起始节点
        /// </summary>
        public bool IsStart
        {
            get { return this.isstart; }
            set { this.isstart = value; }
        }

        /// <summary>
        /// 是否为终点节点
        /// </summary>
        public bool IsTerminate
        {
            get { return this.isterminate;}
            set { this.isterminate = value;}
        }

        /// <summary>
        /// 根据下标返回或设置相应的值
        /// 0：指令类型
        /// 1,2,3,4：指令的操作数和信息
        /// </summary>
        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.type;
                    case 1: return this.flag1;
                    case 2: return this.flag2;
                    case 3: return this.flag3;
                    case 4: return this.flag4;
                    case 5: return this.flag5;
                    default:return String.Empty;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: this.type = value; break;
                    case 1: this.flag1 = value;break;
                    case 2: this.flag2 = value;break;
                    case 3: this.flag3 = value;break;
                    case 4: this.flag4 = value;break;
                    case 5: this.flag5 = value;break;
                }
            }
        }

        /// <summary>
        /// 节点在梯形图中的坐标
        /// </summary>
        public int X
        {
            get { return this.x;}
            set
            {
                this.x = value;
                this.isstart = (this.x == 0);
                this.isterminate = (this.x == 11);   
            }
        }
        public int Y
        {
            get { return this.y;}
            set { this.y = value; }
        }

        /// <summary>
        /// 指向相邻节点的通道
        /// </summary>
        public LCNode Left
        {
            get { return this.left; }
            set { this.left = value; }
        }
        public LCNode Right
        {
            get { return this.right; }
            set { this.right = value; }
        }
        public LCNode Up
        {
            get { return this.up; }
            set { this.up = value; }
        }
        public LCNode Down
        {
            get { return this.down; }
            set { this.down = value; }
        }
        public LCNode RiUp
        {
            get { return this.riup; }
            set { this.riup = value; }
        }
        public LCNode RiDo
        {
            get { return this.rido; }
            set { this.rido = value; }
        }
        public LCNode LeUp
        {
            get { return this.leup; }
            set { this.leup = value; }
        }
        public LCNode LeDo
        {
            get { return this.ledo; }
            set { this.ledo = value; }
        }

        /// <summary>
        /// 这个节点是否左联通和下联通
        /// HAccess为true时，代表这个节点左右联通
        /// VAccess为true时，代表这个节点向下有边连接
        /// </summary>
        public bool HAccess
        {
            get { return this.horizontial_edge; }
            set { this.horizontial_edge = value; }
        }
        public bool VAccess
        {
            get { return this.vertical_edge; }
            set { this.vertical_edge = value; }
        }

        /// <summary>
        /// 当梯形图转换为逻辑图后，这个元件相邻的线路所对应的图中的节点
        /// </summary>        
        public int LNodeID
        {
            get { return this.lnodeid; }
            set { this.lnodeid = value; }
        }
        public int RNodeID
        {
            get { return this.rnodeid; }
            set { this.rnodeid = value; }
        }
        /// <summary>
        /// 该元件的表达式模式（限逻辑元件）
        /// </summary>        
        public string Expr
        {
            get
            {
                switch (Type)
                {
                    case "LD": return String.Format("[{0:d}]{1:s}", Id, flag1);
                    case "LDI": return String.Format("[{0:d}]!{1:s}", Id, flag1);
                    case "LDIM": return String.Format("[{0:d}]im{1:s}", Id, flag1);
                    case "LDIIM": return String.Format("[{0:d}]!im{1:s}", Id, flag1);
                    case "LDP": return String.Format("[{0:d}]ue{1:s}", Id, flag1);
                    case "LDF": return String.Format("[{0:d}]de{1:s}", Id, flag1);
                    case "MEP": return String.Format("[{0:d}]ue", Id);
                    case "MEF": return String.Format("[{0:d}]de", Id);
                    case "INV": return String.Format("[{0:d}]!", Id);
                    case "LDWEQ": return String.Format("[{0:d}]{1:s}w={2:s}", Id, flag1, flag2);
                    case "LDDEQ": return String.Format("[{0:d}]{1:s}d={2:s}", Id, flag1, flag2);
                    case "LDFEQ": return String.Format("[{0:d}]{1:s}f={2:s}", Id, flag1, flag2);
                    case "LDWNE": return String.Format("[{0:d}]{1:s}w<>{2:s}", Id, flag1, flag2);
                    case "LDDNE": return String.Format("[{0:d}]{1:s}d<>{2:s}", Id, flag1, flag2);
                    case "LDFNE": return String.Format("[{0:d}]{1:s}f<>{2:s}", Id, flag1, flag2);
                    case "LDWGE": return String.Format("[{0:d}]{1:s}w>={2:s}", Id, flag1, flag2);
                    case "LDDGE": return String.Format("[{0:d}]{1:s}d>={2:s}", Id, flag1, flag2);
                    case "LDFGE": return String.Format("[{0:d}]{1:s}f>={2:s}", Id, flag1, flag2);
                    case "LDWLE": return String.Format("[{0:d}]{1:s}w<={2:s}", Id, flag1, flag2);
                    case "LDDLE": return String.Format("[{0:d}]{1:s}d<={2:s}", Id, flag1, flag2);
                    case "LDFLE": return String.Format("[{0:d}]{1:s}f<={2:s}", Id, flag1, flag2);
                    case "LDWG": return String.Format("[{0:d}]{1:s}w>{2:s}", Id, flag1, flag2);
                    case "LDDG": return String.Format("[{0:d}]{1:s}d>{2:s}", Id, flag1, flag2);
                    case "LDFG": return String.Format("[{0:d}]{1:s}f>{2:s}", Id, flag1, flag2);
                    case "LDWL": return String.Format("[{0:d}]{1:s}w<{2:s}", Id, flag1, flag2);
                    case "LDDL": return String.Format("[{0:d}]{1:s}d<{2:s}", Id, flag1, flag2);
                    case "LDFL": return String.Format("[{0:d}]{1:s}f<{2:s}", Id, flag1, flag2);
                    default: return String.Format("[{0:d}]1", Id);
                }
            }
        }
        /// <summary>
        /// 元件用PLC指令格式显示
        /// </summary>
        /// <param name="profix">指令前缀(/A/OR)</param>
        /// <returns></returns>
        public string ToShowString(string profix = "")
        {
            string text = "";
            switch (type)
            {
                case "LD": text = profix + " " + flag1; break;
                case "LDI": text = profix + "I " + flag1; break;
                case "LDIM": text = profix + "IM " + flag1; break;
                case "LDIIM": text = profix + "IIM " + flag1; break;
                case "LDP": text = profix + "P " + flag1; break;
                case "LDF": text = profix + "F " + flag1; break;
                case "MEP": text = "MEP"; break;
                case "MEF": text = "MEF"; break;
                case "INV": text = "INV"; break;
                case "OUT": text = "OUT " + flag1; break;
                case "OUTIM": text = "OUTIM " + flag1; break;
                case "SET": text = "SET " + flag1 + " " + flag2; break;
                case "SETIM": text = "SETIM " + flag1 + " " + flag2; break;
                case "RST": text = "RST " + flag1 + " " + flag2; break;
                case "RSTIM": text = "RSTIM " + flag1 + " " + flag2; break;
                case "ALT": text = "ALT " + flag1; break;
                case "ALTP": text = "ALTP " + flag1; break;
                case "LDWEQ": text = profix + "WEQ " + flag1 + " " + flag2; break;
                case "LDDEQ": text = profix + "DEQ " + flag1 + " " + flag2; break;
                case "LDFEQ": text = profix + "FEQ " + flag1 + " " + flag2; break;
                case "LDWNE": text = profix + "WNE " + flag1 + " " + flag2; break;
                case "LDDNE": text = profix + "DNE " + flag1 + " " + flag2; break;
                case "LDFNE": text = profix + "FNE " + flag1 + " " + flag2; break;
                case "LDWLE": text = profix + "WLE " + flag1 + " " + flag2; break;
                case "LDDLE": text = profix + "DLE " + flag1 + " " + flag2; break;
                case "LDFLE": text = profix + "FLE " + flag1 + " " + flag2; break;
                case "LDWGE": text = profix + "WGE " + flag1 + " " + flag2; break;
                case "LDDGE": text = profix + "DGE " + flag1 + " " + flag2; break;
                case "LDFGE": text = profix + "FGE " + flag1 + " " + flag2; break;
                case "LDWL": text = profix + "WL " + flag1 + " " + flag2; break;
                case "LDDL": text = profix + "DL " + flag1 + " " + flag2; break;
                case "LDFL": text = profix + "FL " + flag1 + " " + flag2; break;
                case "LDWG": text = profix + "WG " + flag1 + " " + flag2; break;
                case "LDDG": text = profix + "DG " + flag1 + " " + flag2; break;
                case "LDFG": text = profix + "FG " + flag1 + " " + flag2; break;
                case "WTOD": text = "WTOD " + flag1 + " " + flag2; break;
                case "DTOW": text = "DTOW " + flag1 + " " + flag2; break;
                case "DTOF": text = "DTOF " + flag1 + " " + flag2; break;
                case "BIN": text = "BIN " + flag1 + " " + flag2; break;
                case "BCD": text = "BCD " + flag1 + " " + flag2; break;
                case "ROUND": text = "ROUND " + flag1; break;
                case "TURNC": text = "TURNC " + flag1; break;
                case "INVW": text = "INVW " + flag1 + " " + flag2; break;
                case "INVD": text = "INVD " + flag1 + " " + flag2; break;
                case "ANDW": text = "ANDW " + flag1 + " " + flag2 + " " + flag3; break;
                case "ANDD": text = "ANDD " + flag1 + " " + flag2 + " " + flag3; break;
                case "ORW": text = "ORW " + flag1 + " " + flag2 + " " + flag3; break;
                case "ORD": text = "ORD " + flag1 + " " + flag2 + " " + flag3; break;
                case "XORW": text = "XORW " + flag1 + " " + flag2 + " " + flag3; break;
                case "XORD": text = "XORD " + flag1 + " " + flag2 + " " + flag3; break;
                case "MOV": text = "MOV " + flag1 + " " + flag2; break;
                case "MOVD": text = "MOVD " + flag1 + " " + flag2; break;
                case "MOVF": text = "MOVF " + flag1 + " " + flag2; break;
                case "MVBLK": text = "MVBLK " + flag1 + " " + flag2 + " " + flag3; break;
                case "MVDBLK": text = "MVDBLK " + flag1 + " " + flag2 + " " + flag3; break;
                case "ADDF": text = "ADDF " + flag1 + " " + flag2 + " " + flag3; break;
                case "SUBF": text = "SUBF " + flag1 + " " + flag2 + " " + flag3; break;
                case "MULF": text = "MULF " + flag1 + " " + flag2 + " " + flag3; break;
                case "DIVF": text = "DIVF " + flag1 + " " + flag2 + " " + flag3; break;
                case "SQRT": text = "SQRT " + flag1 + " " + flag2; break;
                case "SIN": text = "SIN " + flag1 + " " + flag2; break;
                case "COS": text = "COS " + flag1 + " " + flag2; break;
                case "TAN": text = "TAN " + flag1 + " " + flag2; break;
                case "LN": text = "LN " + flag1 + " " + flag2; break;
                case "EXP": text = "EXP " + flag1 + " " + flag2; break;
                case "ADD": text = "ADD " + flag1 + " " + flag2 + " " + flag3; break;
                case "ADDD": text = "ADDD " + flag1 + " " + flag2 + " " + flag3; break;
                case "SUB": text = "SUB " + flag1 + " " + flag2 + " " + flag3; break;
                case "SUBD": text = "SUBD " + flag1 + " " + flag2 + " " + flag3; break;
                case "MUL": text = "MUL " + flag1 + " " + flag2 + " " + flag3; break;
                case "MULD": text = "MULD " + flag1 + " " + flag2 + " " + flag3; break;
                case "MULW": text = "MULW " + flag1 + " " + flag2 + " " + flag3; break;
                case "DIV": text = "DIV " + flag1 + " " + flag2 + " " + flag3; break;
                case "DIVD": text = "DIVD " + flag1 + " " + flag2 + " " + flag3; break;
                case "DIVW": text = "DIVW " + flag1 + " " + flag2 + " " + flag3; break;
                case "INC": text = "INC " + flag1 + " " + flag2; break;
                case "INCD": text = "INCD " + flag1 + " " + flag2; break;
                case "DEC": text = "DEC " + flag1 + " " + flag2; break;
                case "DECD": text = "DECD " + flag1 + " " + flag2; break;
                case "TON": text = "TON " + flag1 + " " + flag2; break;
                case "TONR": text = "TONR " + flag1 + " " + flag2; break;
                case "TOF": text = "TOF " + flag1 + " " + flag2; break;
                case "CTU": text = "CTU " + flag1 + " " + flag2; break;
                case "CTUD": text = "CTUD " + flag1 + " " + flag2; break;
                case "CTD": text = "CTD " + flag1 + " " + flag2; break;
                case "FOR": text = "FOR " + flag1; break;
                case "NEXT": text = "NEXT"; break;
                case "JMP": text = "JMP " + flag1; break;
                case "LBL": text = "LBL " + flag1; break;
                case "CALL": text = "CALL " + flag1; break;
                case "CALLM": text = "CALLM " + flag1 + " " + flag2 + " " + flag3 + " " + flag4 + " " + flag5; break;
                case "SHL": text = "SHL " + flag1 + " " + flag2 + " " + flag3; break;
                case "SHLD": text = "SHLD " + flag1 + " " + flag2 + " " + flag3; break;
                case "SHR": text = "SHR " + flag1 + " " + flag2 + " " + flag3; break;
                case "SHRD": text = "SHRD " + flag1 + " " + flag2 + " " + flag3; break;
                case "SHLB": text = "SHLB " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "SHRB": text = "SHRB " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "ROL": text = "ROL " + flag1 + " " + flag2 + " " + flag3; break;
                case "ROLD": text = "ROLD " + flag1 + " " + flag2 + " " + flag3; break;
                case "ROR": text = "ROR " + flag1 + " " + flag2 + " " + flag3; break;
                case "RORD": text = "RORD " + flag1 + " " + flag2 + " " + flag3; break;
                case "ATCH": text = "ATCH " + flag1 + " " + flag2; break;
                case "DECH": text = "DECH " + flag1 + " " + flag2; break;
                case "EI": text = "EI "; break;
                case "DI": text = "DI "; break;
                case "TRD": text = "TRD " + flag1; break;
                case "TWR": text = "TWR " + flag1; break;
                case "MBUS": text = "MBUS " + flag1 + " " + flag2 + " " + flag3; break;
                case "SEND": text = "SEND " + flag1 + " " + flag2 + " " + flag3; break;
                case "REV": text = "REV " + flag1 + " " + flag2 + " " + flag3; break;
                case "PLSF": text = "PLSF " + flag1 + " " + flag2; break;
                case "DPLSF": text = "DPLSF " + flag1 + " " + flag2; break;
                case "PWM": text = "PWM " + flag1 + " " + flag2 + " " + flag3; break;
                case "DPWM": text = "DPWM " + flag1 + " " + flag2 + " " + flag3; break;
                case "PLSY": text = "PLSY " + flag1 + " " + flag2 + " " + flag3; break;
                case "DPLSY": text = "DPLSY " + flag1 + " " + flag2 + " " + flag3; break;
                case "PLSR": text = "PLSR " + flag1 + " " + flag2 + " " + flag3; break;
                case "DPLSR": text = "DPLSR " + flag1 + " " + flag2 + " " + flag3; break;
                case "PLSRD": text = "PLSRD " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "DPLSRD": text = "DPLSRD " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "PLSNEXT": text = "PLSNEXT " + flag1; break;
                case "PLSSTOP": text = "PLSSTOP " + flag1; break;
                case "ZRN": text = "ZRN " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "DZRN": text = "DZRN " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "PTO": text = "PTO " + flag1 + " " + flag2 + " " + flag3; break;
                case "DRVI": text = "DRVI " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "DDRVI": text = "DRVI " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "HCNT": text = "DRVI " + flag1 + " " + flag2; break;
                case "LOG": text = "LOG " + flag1 + " " + flag2; break;
                case "POW": text = "POW " + flag1 + " " + flag2 + " " + flag3; break;
                case "FACT": text = "FACT " + flag1 + " " + flag2; break;
                case "CMP": text = "CMP " + flag1 + " " + flag2 + " " + flag3; break;
                case "CMPD": text = "CMPD " + flag1 + " " + flag2 + " " + flag3; break;
                case "CMPF": text = "CMPF " + flag1 + " " + flag2 + " " + flag3; break;
                case "ZCP": text = "ZCP " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "ZCPD": text = "ZCPD " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "ZCPF": text = "ZCPF " + flag1 + " " + flag2 + " " + flag3 + " " + flag4; break;
                case "NEG": text = "NEG " + flag1 + " " + flag2; break;
                case "NEGD": text = "NEGD " + flag1 + " " + flag2; break;
                case "XCH": text = "XCH " + flag1 + " " + flag2; break;
                case "XCHD": text = "XCHD " + flag1 + " " + flag2; break;
                case "XCHF": text = "XCHF " + flag1 + " " + flag2; break;
                case "CML": text = "CML " + flag1 + " " + flag2; break;
                case "CMLD": text = "CMLD " + flag1 + " " + flag2; break;
                case "SMOV": text = "SMOV " + flag1 + " " + flag2 + " " + flag3 + " " + flag4 + " " + flag5; break;
                case "FMOV": text = "FMOV " + flag1 + " " + flag2 + " " + flag3; break;
                case "FMOVD": text = "FMOVD " + flag1 + " " + flag2 + " " + flag3; break;
                default: text = ""; break;
            }
            return text;
        }

        /// <summary>
        /// 生成该元件对应的指令
        /// </summary> 
        public void GenInst(List<PLCInstruction> insts, int flag=0)
        {
            string profix = "LD";
            if ((flag & 0x04) != 0)
                profix = "A";
            if ((flag & 0x08) != 0)
                profix = "OR";
            PLCInstruction inst = new PLCInstruction(ToShowString(profix));
            inst.PrototypeID = Id;
            inst.ProtoType = Prototype;
            insts.Add(inst);
        }

        public override string ToString()
        {
            return String.Format("({0:d},{1:d},{3},{4}){2:s}", X, Y, Type, VAccess, HAccess);
        }

    }

}
