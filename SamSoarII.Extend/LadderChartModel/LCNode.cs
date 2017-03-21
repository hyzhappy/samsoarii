using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Extend.Utility;

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
    public class LCNode
    {
        /// <summary>
        /// 内部成员变量
        /// </summary>
        private bool enable;

        private int id;
        private int type;
        private bool isstart;
        private bool isterminate;
        private int flag1, flag2, flag3, flag4, flag5;

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
        public int Type
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
        public int this[int index]
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
                    default:return 0;
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
                this.isterminate = (this.x == 9);   
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
                switch (Type&0xFF)
                {
                    case InstHelper.LD: return InstHelper.RegName(flag1);
                    case InstHelper.LDI: return "!" + InstHelper.RegName(flag1);
                    case InstHelper.LDIM: return "im" + InstHelper.RegName(flag1);
                    case InstHelper.LDIIM: return "!im" + InstHelper.RegName(flag1);
                    case InstHelper.LDP: return "ue" + InstHelper.RegName(flag1);
                    case InstHelper.LDF: return "de" + InstHelper.RegName(flag1);
                    case InstHelper.MEP: return "de";
                    case InstHelper.MEF: return "ue";
                    case InstHelper.INV: return "!";
                    case InstHelper.EQW: return InstHelper.RegName(flag1) + "w=" + InstHelper.RegName(flag2);
                    case InstHelper.EQD: return InstHelper.RegName(flag1) + "d=" + InstHelper.RegName(flag2);
                    case InstHelper.EQF: return InstHelper.RegName(flag1) + "f=" + InstHelper.RegName(flag2);
                    case InstHelper.NEW: return InstHelper.RegName(flag1) + "w<>" + InstHelper.RegName(flag2);
                    case InstHelper.NED: return InstHelper.RegName(flag1) + "d<>" + InstHelper.RegName(flag2);
                    case InstHelper.NEF: return InstHelper.RegName(flag1) + "f<>" + InstHelper.RegName(flag2);
                    case InstHelper.GEW: return InstHelper.RegName(flag1) + "w>=" + InstHelper.RegName(flag2);
                    case InstHelper.GED: return InstHelper.RegName(flag1) + "d>=" + InstHelper.RegName(flag2);
                    case InstHelper.GEF: return InstHelper.RegName(flag1) + "f>=" + InstHelper.RegName(flag2);
                    case InstHelper.LEW: return InstHelper.RegName(flag1) + "w<=" + InstHelper.RegName(flag2);
                    case InstHelper.LED: return InstHelper.RegName(flag1) + "d<=" + InstHelper.RegName(flag2);
                    case InstHelper.LEF: return InstHelper.RegName(flag1) + "f<=" + InstHelper.RegName(flag2);
                    case InstHelper.GTW: return InstHelper.RegName(flag1) + "w>" + InstHelper.RegName(flag2);
                    case InstHelper.GTD: return InstHelper.RegName(flag1) + "d>" + InstHelper.RegName(flag2);
                    case InstHelper.GTF: return InstHelper.RegName(flag1) + "f>" + InstHelper.RegName(flag2);
                    case InstHelper.LTW: return InstHelper.RegName(flag1) + "w<" + InstHelper.RegName(flag2);
                    case InstHelper.LTD: return InstHelper.RegName(flag1) + "d<" + InstHelper.RegName(flag2);
                    case InstHelper.LTF: return InstHelper.RegName(flag1) + "f<" + InstHelper.RegName(flag2);
                    default: return "1";
                }
            }
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
            string text = "";
            switch (type&0xFF)
            {
                case InstHelper.LD: text = profix + " " + InstHelper.RegName(flag1); break;
                case InstHelper.LDI:text = profix + "I " + InstHelper.RegName(flag1); break;
                case InstHelper.LDIM:text = profix + "IM " + InstHelper.RegName(flag1); break;
                case InstHelper.LDIIM:text = profix + "IIM " + InstHelper.RegName(flag1); break;
                case InstHelper.LDP: text = profix + "P " + InstHelper.RegName(flag1); break;
                case InstHelper.LDF: text = profix + "F " + InstHelper.RegName(flag1); break;
                case InstHelper.MEP: text = "MEP"; break;
                case InstHelper.MEF: text = "MEF"; break;
                case InstHelper.INV: text = "INV"; break;
                case InstHelper.OUT: text = "OUT " + InstHelper.RegName(flag1); break;
                case InstHelper.OUTIM: text = "OUTIM " + InstHelper.RegName(flag1); break;
                case InstHelper.SET: text = "SET " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.SETIM: text = "SETIM " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.RST: text = "RST " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.RSTIM: text = "RSTIM " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.ALT: text = "ALT " + InstHelper.RegName(flag1); break;
                case InstHelper.ALTP: text = "ALTP " + InstHelper.RegName(flag1); break;
                case InstHelper.EQW: text = profix + "WEQ " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.EQD: text = profix + "DEQ " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.EQF: text = profix + "FEQ " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.NEW: text = profix + "WNE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.NED: text = profix + "DNE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.NEF: text = profix + "FNE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LEW: text = profix + "WLE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LED: text = profix + "DLE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LEF: text = profix + "FLE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.GEW: text = profix + "WGE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.GED: text = profix + "DGE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.GEF: text = profix + "FGE " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LTW: text = profix + "WL " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LTD: text = profix + "DL " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LTF: text = profix + "FL " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.GTW: text = profix + "WG " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.GTD: text = profix + "DG " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.GTF: text = profix + "FG " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.WTOD: text = profix + "WTOD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.DTOW: text = profix + "DTOW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.DTOF: text = profix + "DTOF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.BIN: text = profix + "BIN " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.BCD: text = profix + "BCD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.ROUND: text = "ROUND " + InstHelper.RegName(flag1); break;
                case InstHelper.TURNC: text = "TURNC " + InstHelper.RegName(flag1); break;
                case InstHelper.INVW: text = "INVW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.INVD: text = "INVD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.ANDW: text = "ANDW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ANDD: text = "ANDD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ORW: text = "ORW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ORD: text = "ORD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.XORW: text = "XORW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.XORD: text = "XORD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.MOV: text = profix + "MOV " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.MOVD: text = profix + "MOVD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.MOVF: text = profix + "MOVF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.MVBLK: text = "MVBLK " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.MVDBLK: text = "MVDBLK " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ADDF: text = "ADDF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SUBF: text = "SUBF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.MULF: text = "MULF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DIVF: text = "DIVF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SQRT: text = "SQRT " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.SIN: text = "SIN " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.COS: text = "COS " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.TAN: text = "TAN " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LN: text = "LN " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.EXP: text = "EXP " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.ADD: text = "ADD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ADDD: text = "ADDD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SUB: text = "SUB " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SUBD: text = "SUBD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.MUL: text = "MUL " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.MULD: text = "MULD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.MULW: text = "MULW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DIV: text = "DIV " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DIVD: text = "DIVD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DIVW: text = "DIVW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.INC: text = "INC " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.INCD: text = "INCD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.DEC: text = "DEC " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.DECD: text = "DECD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.TON: text = "TON " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.TONR: text = "TONR " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.TOF: text = "TOF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.CTU: text = "CTU " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.CTUD: text = "CTUD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.CTD: text = "CTD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.FOR: text = "FOR " + InstHelper.RegName(flag1); break;
                case InstHelper.NEXT: text = "NEXT"; break;
                case InstHelper.JMP: text = "JMP " + InstHelper.RegName(flag1); break;
                case InstHelper.LBL: text = "LBL " + InstHelper.RegName(flag1); break;
                case InstHelper.CALL: text = "CALL " + InstHelper.RegName(flag1); break;
                case InstHelper.CALLM: text = "CALLM " + InstHelper.RegName(flag1); break;
                case InstHelper.SHL: text = "SHL " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SHLD: text = "SHLD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SHR: text = "SHR " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SHRD: text = "SHRD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SHLB: text = "SHLB " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.SHRB: text = "SHRB " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.ROL: text = "ROL " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ROLD: text = "ROLD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ROR: text = "ROR " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.RORD: text = "RORD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ATCH: text = "ATCH " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.DECH: text = "DECH " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.EI: text = "EI "; break;
                case InstHelper.DI: text = "DI "; break;
                case InstHelper.TRD: text = "TRD " + InstHelper.RegName(flag1); break;
                case InstHelper.TWR: text = "TWR " + InstHelper.RegName(flag1); break;
                case InstHelper.MBUS: text = "MBUS " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.SEND: text = "SEND " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.REV: text = "REV " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.PLSF: text = "PLSF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.DPLSF: text = "DPLSF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.PWM: text = "PWM " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DPWM: text = "DPWM " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.PLSY: text = "PLSY " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DPLSY: text = "DPLSY " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.PLSR: text = "PLSR " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DPLSR: text = "DPLSR " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.PLSRD: text = "PLSRD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.DPLSRD: text = "DPLSRD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.PLSNEXT: text = "PLSNEXT " + InstHelper.RegName(flag1); break;
                case InstHelper.PLSSTOP: text = "PLSSTOP " + InstHelper.RegName(flag1); break;
                case InstHelper.ZRN: text = "ZRN " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.DZRN: text = "DZRN " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.PTO: text = "PTO " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.DRVI: text = "DRVI " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.DDRVI: text = "DRVI " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.HCNT: text = "DRVI " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.LOG: text = "LOG " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.POW: text = "POW " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.FACT: text = "FACT " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.CMP: text = "CMP " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.CMPD: text = "CMPD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.CMPF: text = "CMPF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.ZCP: text = "ZCP " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.ZCPD: text = "ZCPD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.ZCPF: text = "ZCPF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4); break;
                case InstHelper.NEG: text = "NEG " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.NEGD: text = "NEGD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.XCH: text = "XCH " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.XCHD: text = "XCHD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.XCHF: text = "XCHF " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.CML: text = "CML " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.CMLD: text = "CMLD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2); break;
                case InstHelper.SMOV: text = "SMOV " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3) + " " + InstHelper.RegName(flag4) + " " + InstHelper.RegName(flag5); break;
                case InstHelper.FMOV: text = "FMOV " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                case InstHelper.FMOVD: text = "FMOVD " + InstHelper.RegName(flag1) + " " + InstHelper.RegName(flag2) + " " + InstHelper.RegName(flag3); break;
                default: text = "";break;    
            }
            InstHelper.AddInst(insts, text);
        }
        
    }

}
