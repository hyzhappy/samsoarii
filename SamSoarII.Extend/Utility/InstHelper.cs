﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        /// <summary> 指令大类：位逻辑（Usage : InstId = 0x01??）</summary>
        public const int BITLOGIC = 0x01;
        /// <summary> 指令大类：比较（Usage : InstId = 0x02??）</summary>
        public const int COMPARE = 0x02;
        /// <summary> 指令大类：转换（Usage : InstId = 0x03??）</summary>
        public const int CONVERT = 0x03;
        /// <summary> 指令大类：逻辑运算（Usage : InstId = 0x04??）</summary>
        public const int LOGICCALC = 0x04;
        /// <summary> 指令大类：传送（Usage : InstId = 0x05??）</summary>
        public const int TRANSFORM = 0x05;
        /// <summary> 指令大类：浮点运算（Usage : InstId = 0x06??）</summary>
        public const int FLOATCALC = 0x06;
        /// <summary> 指令大类：整数运算（Usage : InstId = 0x07??）</summary>
        public const int INTCALC = 0x07;
        /// <summary> 指令大类：定时器（Usage : InstId = 0x08??）</summary>
        public const int TIMER = 0x08;
        /// <summary> 指令大类：计数器（Usage : InstId = 0x09??）</summary>
        public const int COUNTER = 0x09;
        /// <summary> 指令大类：程序控制（Usage : InstId = 0x0A??）</summary>
        public const int PROGCTRL = 0x0A;
        /// <summary> 指令大类：位移指令（Usage : InstId = 0x0B??）</summary>
        public const int BITSHIFT = 0x0B;
        /// <summary> 指令大类：中断指令（Usage : InstId = 0x0C??）</summary>
        public const int INTERRUPT = 0x0C;
        /// <summary> 指令大类：实时时钟（Usage : InstId = 0x0D??）</summary>
        public const int DATE = 0x0D;
        /// <summary> 指令大类：通信指令（Usage : InstId = 0x0E??）</summary>
        public const int COMUNICATE = 0x0E;
        /// <summary> 指令大类：脉冲指令（Usage : InstId = 0x0F??）</summary>
        public const int PULSE = 0x0F;
        /// <summary> 指令大类：高数计数器（Usage : InstId = 0x10??）</summary>
        public const int HIGHCOUNTER = 0x10;
        /// <summary> 指令大类：辅助指令（Usage : InstId = 0x11??）</summary>
        public const int MILITARY = 0x11;

        /// <summary> 指令小类：常开（Usage : InstId = 0x??01）</summary>
        public const int LD = 0x01;
        /// <summary> 指令小类：立即常开（Usage : InstId = 0x??02）</summary>
        public const int LDIM = 0x02;
        /// <summary> 指令小类：常闭（Usage : InstId = 0x??03）</summary>
        public const int LDI = 0x03;
        /// <summary> 指令小类：立即常闭（Usage : InstId = 0x??04）</summary>
        public const int LDIIM = 0x04;
        /// <summary> 指令小类：上升（Usage : InstId = 0x??05）</summary>
        public const int LDP = 0x05;
        /// <summary> 指令小类：下降（Usage : InstId = 0x??06）</summary>
        public const int LDF = 0x06;
        /// <summary> 指令小类：结果上升（Usage : InstId = 0x??07）</summary>
        public const int MEP = 0x07;
        /// <summary> 指令小类：结果下降（Usage : InstId = 0x??08）</summary>
        public const int MEF = 0x08;
        /// <summary> 指令小类：结果取反（Usage : InstId = 0x??09）</summary>
        public const int INV = 0x09;
        /// <summary> 指令小类：输出（Usage : InstId = 0x??0A）</summary>
        public const int OUT = 0x0A;
        /// <summary> 指令小类：立即输出（Usage : InstId = 0x??0B）</summary>
        public const int OUTIM = 0x0B;
        /// <summary> 指令小类：置位（Usage : InstId = 0x??0C）</summary>
        public const int SET = 0x0C;
        /// <summary> 指令小类：立即置位（Usage : InstId = 0x??0D）</summary>
        public const int SETIM = 0x0D;
        /// <summary> 指令小类：复位（Usage : InstId = 0x??0E）</summary>
        public const int RST = 0x0E;
        /// <summary> 指令小类：立即复位（Usage : InstId = 0x??10）</summary>
        public const int RSTIM = 0x10;
        /// <summary> 指令小类：交替（Usage : InstId = 0x??11）</summary>
        public const int ALT = 0x11;
        /// <summary> 指令小类：上升交替（Usage : InstId = 0x??12）</summary>
        public const int ALTP = 0x12;

        /// <summary> 指令小类：单字相等（Usage : InstId = 0x??13）</summary>
        public const int EQW = 0x13;
        /// <summary> 指令小类：单字不等（Usage : InstId = 0x??14）</summary>
        public const int NEW = 0x14;
        /// <summary> 指令小类：单字大等（Usage : InstId = 0x??15）</summary>
        public const int GEW = 0x15;
        /// <summary> 指令小类：单字小等（Usage : InstId = 0x??16）</summary>
        public const int LEW = 0x16;
        /// <summary> 指令小类：单字大于（Usage : InstId = 0x??17）</summary>
        public const int GTW = 0x17;
        /// <summary> 指令小类：单字小于（Usage : InstId = 0x??18）</summary>
        public const int LTW = 0x18;
        /// <summary> 指令小类：双字相等（Usage : InstId = 0x??19）</summary>
        public const int EQD = 0x19;
        /// <summary> 指令小类：双字不等（Usage : InstId = 0x??1A）</summary>
        public const int NED = 0x1A;
        /// <summary> 指令小类：双字大等（Usage : InstId = 0x??1B）</summary>
        public const int GED = 0x1B;
        /// <summary> 指令小类：双字小等（Usage : InstId = 0x??1C）</summary>
        public const int LED = 0x1C;
        /// <summary> 指令小类：双字大于（Usage : InstId = 0x??1D）</summary>
        public const int GTD = 0x1D;
        /// <summary> 指令小类：双字小于（Usage : InstId = 0x??1E）</summary>
        public const int LTD = 0x1E;
        /// <summary> 指令小类：浮点相等（Usage : InstId = 0x??1F）</summary>
        public const int EQF = 0x1F;
        /// <summary> 指令小类：浮点不等（Usage : InstId = 0x??20）</summary>
        public const int NEF = 0x20;
        /// <summary> 指令小类：浮点大等（Usage : InstId = 0x??21）</summary>
        public const int GEF = 0x21;
        /// <summary> 指令小类：浮点小等（Usage : InstId = 0x??22）</summary>
        public const int LEF = 0x22;
        /// <summary> 指令小类：浮点大于（Usage : InstId = 0x??23）</summary>
        public const int GTF = 0x23;
        /// <summary> 指令小类：浮点小于（Usage : InstId = 0x??24）</summary>
        public const int LTF = 0x24;

        /// <summary> 指令小类：单字转双字（Usage : InstId = 0x??25）</summary>
        public const int WTOD = 0x25;
        /// <summary> 指令小类：双字转单字（Usage : InstId = 0x??26）</summary>
        public const int DTOW = 0x26;
        /// <summary> 指令小类：双字转浮点（Usage : InstId = 0x??27）</summary>
        public const int DTOF = 0x27;
        /// <summary> 指令小类：BCD转双字（Usage : InstId = 0x??28）</summary>
        public const int BIN = 0x28;
        /// <summary> 指令小类：双字转BCD（Usage : InstId = 0x??29）</summary>
        public const int BCD = 0x29;
        /// <summary> 指令小类：四舍五入（Usage : InstId = 0x??2A）</summary>
        public const int ROUND = 0x2A;
        /// <summary> 指令小类：截位取整（Usage : InstId = 0x??2B）</summary>
        public const int TURNC = 0x2B;

        /// <summary> 指令小类：单字取反（Usage : InstId = 0x??2C）</summary>
        public const int INVW = 0x2C;
        /// <summary> 指令小类：双字取反（Usage : InstId = 0x??2D）</summary>
        public const int INVD = 0x2D;
        /// <summary> 指令小类：单字AND（Usage : InstId = 0x??2E）</summary>
        public const int ANDW = 0x2E;
        /// <summary> 指令小类：双字AND（Usage : InstId = 0x??30）</summary>
        public const int ANDD = 0x30;
        /// <summary> 指令小类：单字OR（Usage : InstId = 0x??31）</summary>
        public const int ORW = 0x31;
        /// <summary> 指令小类：双字OR（Usage : InstId = 0x??32）</summary>
        public const int ORD = 0x32;
        /// <summary> 指令小类：单字XOR（Usage : InstId = 0x??33）</summary>
        public const int XORW = 0x33;
        /// <summary> 指令小类：双字XOR（Usage : InstId = 0x??34）</summary>
        public const int XORD = 0x34;

        /// <summary> 指令小类：单字移动（Usage : InstId = 0x??35）</summary>
        public const int MOV = 0x35;
        /// <summary> 指令小类：双字移动（Usage : InstId = 0x??36）</summary>
        public const int MOVD = 0x36;
        /// <summary> 指令小类：浮点移动（Usage : InstId = 0x??37）</summary>
        public const int MOVF = 0x37;
        /// <summary> 指令小类：批量字移动（Usage : InstId = 0x??38）</summary>
        public const int MVBLK = 0x38;
        /// <summary> 指令小类：批量双字移动（Usage : InstId = 0x??39）</summary>
        public const int MVDBLK = 0x39;

        /// <summary> 指令小类：浮点加法（Usage : InstId = 0x??3A）</summary>
        public const int ADDF = 0x3A;
        /// <summary> 指令小类：浮点减法（Usage : InstId = 0x??3B）</summary>
        public const int SUBF = 0x3B;
        /// <summary> 指令小类：浮点乘法（Usage : InstId = 0x??3C）</summary>
        public const int MULF = 0x3C;
        /// <summary> 指令小类：浮点除法（Usage : InstId = 0x??3D）</summary>
        public const int DIVF = 0x3D;
        /// <summary> 指令小类：浮点方根（Usage : InstId = 0x??3E）</summary>
        public const int SQRT = 0x3E;
        /// <summary> 指令小类：浮点正弦（Usage : InstId = 0x??3F）</summary>
        public const int SIN = 0x3F;
        /// <summary> 指令小类：浮点余弦（Usage : InstId = 0x??40）</summary>
        public const int COS = 0x40;
        /// <summary> 指令小类：浮点正切（Usage : InstId = 0x??41）</summary>
        public const int TAN = 0x41;
        /// <summary> 指令小类：浮点对数（Usage : InstId = 0x??42）</summary>
        public const int LN = 0x42;
        /// <summary> 指令小类：浮点指数（Usage : InstId = 0x??43）</summary>
        public const int EXP = 0x43;

        /// <summary> 指令小类：单字加法（Usage : InstId = 0x??44）</summary>
        public const int ADD = 0x44;
        /// <summary> 指令小类：双字加法（Usage : InstId = 0x??45）</summary>
        public const int ADDD = 0x45;
        /// <summary> 指令小类：单字减法（Usage : InstId = 0x??46）</summary>
        public const int SUB = 0x46;
        /// <summary> 指令小类：双字减法（Usage : InstId = 0x??47）</summary>
        public const int SUBD = 0x47;
        /// <summary> 指令小类：单字乘法输出双字（Usage : InstId = 0x??48）</summary>
        public const int MUL = 0x48;
        /// <summary> 指令小类：单字乘法输出单字（Usage : InstId = 0x??49）</summary>
        public const int MULW = 0x49;
        /// <summary> 指令小类：双字乘法输出双字（Usage : InstId = 0x??4A）</summary>
        public const int MULD = 0x4A;
        /// <summary> 指令小类：单字乘法输出双字（Usage : InstId = 0x??4B）</summary>
        public const int DIV = 0x4B;
        /// <summary> 指令小类：单字乘法输出单字（Usage : InstId = 0x??4C）</summary>
        public const int DIVW = 0x4C;
        /// <summary> 指令小类：双字乘法输出双字（Usage : InstId = 0x??4D）</summary>
        public const int DIVD = 0x4D;
        /// <summary> 指令小类：单字累加（Usage : InstId = 0x??4F）</summary>
        public const int INC = 0x4F;
        /// <summary> 指令小类：双字累加（Usage : InstId = 0x??50）</summary>
        public const int INCD = 0x50;
        /// <summary> 指令小类：单字累减（Usage : InstId = 0x??51）</summary>
        public const int DEC = 0x51;
        /// <summary> 指令小类：双字累减（Usage : InstId = 0x??52）</summary>
        public const int DECD = 0x52;

        /// <summary> 指令小类：闭合计时（Usage : InstId = 0x??53）</summary>
        public const int TON = 0x53;
        /// <summary> 指令小类：闭合保留计时（Usage : InstId = 0x??54）</summary>
        public const int TONR = 0x54;
        /// <summary> 指令小类：断开计时（Usage : InstId = 0x??55）</summary>
        public const int TOF = 0x55;

        /// <summary> 指令小类：向上计数（Usage : InstId = 0x??56）</summary>
        public const int CTU = 0x56;
        /// <summary> 指令小类：向下计数（Usage : InstId = 0x??57）</summary>
        public const int CTD = 0x57;
        /// <summary> 指令小类：向上向下计数（Usage : InstId = 0x??58）</summary>
        public const int CTUD = 0x58;

        /// <summary> 指令小类：循环（Usage : InstId = 0x??59）</summary>
        public const int FOR = 0x59;
        /// <summary> 指令小类：循环结束（Usage : InstId = 0x??5A）</summary>
        public const int NEXT = 0x5A;
        /// <summary> 指令小类：跳跃（Usage : InstId = 0x??5B）</summary>
        public const int JMP = 0x5B;
        /// <summary> 指令小类：跳跃标记（Usage : InstId = 0x??5C）</summary>
        public const int LBL = 0x5C;
        /// <summary> 指令小类：子程序（Usage : InstId = 0x??5D）</summary>
        public const int CALL = 0x5D;
        /// <summary> 指令小类：宏指令（Usage : InstId = 0x??5E）</summary>
        public const int CALLM = 0x5E;

        /// <summary> 指令小类：单字左移（Usage : InstId = 0x??5F）</summary>
        public const int SHL = 0x5F;
        /// <summary> 指令小类：双字左移（Usage : InstId = 0x??60）</summary>
        public const int SHLD = 0x60;
        /// <summary> 指令小类：单字右移（Usage : InstId = 0x??61）</summary>
        public const int SHR = 0x61;
        /// <summary> 指令小类：双字右移（Usage : InstId = 0x??62）</summary>
        public const int SHRD = 0x62;
        /// <summary> 指令小类：位左移（Usage : InstId = 0x??63）</summary>
        public const int SHLB = 0x63;
        /// <summary> 指令小类：位右移（Usage : InstId = 0x??64）</summary>
        public const int SHRB = 0x64;
        /// <summary> 指令小类：单字循环左移（Usage : InstId = 0x??65）</summary>
        public const int ROL = 0x65;
        /// <summary> 指令小类：双字循环左移（Usage : InstId = 0x??66）</summary>
        public const int ROLD = 0x66;
        /// <summary> 指令小类：单字循环右移（Usage : InstId = 0x??67）</summary>
        public const int ROR = 0x67;
        /// <summary> 指令小类：双字循环右移（Usage : InstId = 0x??68）</summary>
        public const int RORD = 0x68;

        /// <summary> 指令小类：中断设置（Usage : InstId = 0x??69）</summary>
        public const int ATCH = 0x69;
        /// <summary> 指令小类：中断取消（Usage : InstId = 0x??6A）</summary>
        public const int DECH = 0x6A;
        /// <summary> 指令小类：中断开始（Usage : InstId = 0x??6B）</summary>
        public const int EI = 0x6B;
        /// <summary> 指令小类：中断结束（Usage : InstId = 0x??6C）</summary>
        public const int DI = 0x6C;

        /// <summary> 指令小类：实时时钟读取（Usage : InstId = 0x??6D）</summary>
        public const int TRD = 0x6D;
        /// <summary> 指令小类：实时时钟写入（Usage : InstId = 0x??6E）</summary>
        public const int TWR = 0x6E;

        /// <summary> 指令小类：MODBUS通信（Usage : InstId = 0x??6F）</summary>
        public const int MBUS = 0x6F;
        /// <summary> 指令小类：发送数据（Usage : InstId = 0x??70）</summary>
        public const int SEND = 0x70;
        /// <summary> 指令小类：接受数据（Usage : InstId = 0x??71）</summary>
        public const int REV = 0x71;

        /// <summary> 指令小类：可变频率脉冲输出（单字参数）（Usage : InstId = 0x??72）</summary>
        public const int PLSF = 0x72;
        /// <summary> 指令小类：可变频率脉冲输出（双字参数）（Usage : InstId = 0x??73）</summary>
        public const int DPLSF = 0x73;
        /// <summary> 指令小类：脉宽调制（单字参数）（Usage : InstId = 0x??74）</summary>
        public const int PWM = 0x74;
        /// <summary> 指令小类：脉宽调制（双字参数）（Usage : InstId = 0x??75）</summary>
        public const int DPWM = 0x75;
        /// <summary> 指令小类：单端不带加减数脉冲（单字参数）（Usage : InstId = 0x??76）</summary>
        public const int PLSY = 0x76;
        /// <summary> 指令小类：单端不带加减数脉冲（双字参数）（Usage : InstId = 0x??77）</summary>
        public const int DPLSY = 0x77;
        /// <summary> 指令小类：相对位置多段脉冲（单字参数）（Usage : InstId = 0x??78）</summary>
        public const int PLSR = 0x78;
        /// <summary> 指令小类：相对位置多段脉冲（双字参数）（Usage : InstId = 0x??79）</summary>
        public const int DPLSR = 0x79;
        /// <summary> 指令小类：带方向相对位置多段脉冲（单字参数）（Usage : InstId = 0x??7A）</summary>
        public const int PLSRD = 0x7A;
        /// <summary> 指令小类：带方向相对位置多段脉冲（双字参数）（Usage : InstId = 0x??7B）</summary>
        public const int DPLSRD = 0x7B;
        /// <summary> 指令小类：脉冲段切换（Usage : InstId = 0x??7C）</summary>
        public const int PLSNEXT = 0x7C;
        /// <summary> 指令小类：脉冲停止（Usage : InstId = 0x??7D）</summary>
        public const int PLSSTOP = 0x7D;
        /// <summary> 指令小类：回归原点（单字参数）（Usage : InstId = 0x??7E）</summary>
        public const int ZRN = 0x7E;
        /// <summary> 指令小类：回归原点（单字参数）（Usage : InstId = 0x??7F）</summary>
        public const int DZRN = 0x7F;
        /// <summary> 指令小类：相对位置多段脉冲控制（Usage : InstId = 0x??80）</summary>
        public const int PTO = 0x80;
        /// <summary> 指令小类：相对位置单段脉冲（单字参数）（Usage : InstId = 0x??81）</summary>
        public const int DRVI = 0x81;
        /// <summary> 指令小类：相对位置单段脉冲（单字参数）（Usage : InstId = 0x??82）</summary>
        public const int DDRVI = 0x82;

        /// <summary> 指令小类：高速计数器（Usage : InstId = 0x??83）</summary>
        public const int HCNT = 0x83;

        /// <summary> 指令小类：10为底的对数（Usage : InstId = 0x??84）</summary>
        public const int LOG = 0x84;
        /// <summary> 指令小类：幂乘（Usage : InstId = 0x??85）</summary>
        public const int POW = 0x85;
        /// <summary> 指令小类：阶乘（Usage : InstId = 0x??86）</summary>
        public const int FACT = 0x86;
        /// <summary> 指令小类：单字比较（Usage : InstId = 0x??87）</summary>
        public const int CMP = 0x87;
        /// <summary> 指令小类：双字比较（Usage : InstId = 0x??88）</summary>
        public const int CMPD = 0x88;
        /// <summary> 指令小类：浮点比较（Usage : InstId = 0x??89）</summary>
        public const int CMPF = 0x89;
        /// <summary> 指令小类：单字区间比较（Usage : InstId = 0x??8A）</summary>
        public const int ZCP = 0x8A;
        /// <summary> 指令小类：双字区间比较（Usage : InstId = 0x??8B）</summary>
        public const int ZCPD = 0x8B;
        /// <summary> 指令小类：浮点区间比较（Usage : InstId = 0x??8C）</summary>
        public const int ZCPF = 0x8C;
        /// <summary> 指令小类：单字求补（Usage : InstId = 0x??8D）</summary>
        public const int NEG = 0x8D;
        /// <summary> 指令小类：双字求补（Usage : InstId = 0x??8E）</summary>
        public const int NEGD = 0x8E;
        /// <summary> 指令小类：单字交换（Usage : InstId = 0x??8F）</summary>
        public const int XCH = 0x8F;
        /// <summary> 指令小类：双字交换（Usage : InstId = 0x??90）</summary>
        public const int XCHD = 0x90;
        /// <summary> 指令小类：浮点交换（Usage : InstId = 0x??91）</summary>
        public const int XCHF = 0x91;
        /// <summary> 指令小类：单字取反传送（Usage : InstId = 0x??92）</summary>
        public const int CML = 0x92;
        /// <summary> 指令小类：双字取反传送（Usage : InstId = 0x??93）</summary>
        public const int CMLD = 0x93;
        /// <summary> 指令小类：移位传送（Usage : InstId = 0x??94）</summary>
        public const int SMOV = 0x94;
        /// <summary> 指令小类：单字多点传送（Usage : InstId = 0x??95）</summary>
        public const int FMOV = 0x95;
        /// <summary> 指令小类：双字多点传送（Usage : InstId = 0x??96）</summary>
        public const int FMOVD = 0x96;

        /// <summary> X线圈的偏移量</summary>
        public const int ADDRX = 0;
        /// <summary> Y线圈的偏移量</summary>
        public const int ADDRY = 10000;
        /// <summary> 模拟输入寄存器AI的偏移量</summary>
        public const int ADDRAI = 20000;
        /// <summary> 模拟输出寄存器AO的偏移量</summary>
        public const int ADDRAO = 20512;
        /// <summary> 继电器线圈M的偏移量</summary>
        public const int ADDRM = 30000;
        /// <summary> 数据寄存器D的偏移量</summary>
        public const int ADDRD = 40000;
        /// <summary> 计数器的偏移量</summary>
        public const int ADDRCV = 60000;
        /// <summary> 32位计数器的偏移量</summary>
        public const int ADDRCV32 = 60000;
        /// <summary> 计时器的偏移量</summary>
        public const int ADDRTV = 60256;
        /// <summary> 计数器开关的偏移量</summary>
        public const int ADDRC = 60512;
        /// <summary> 计时器开关的偏移量</summary>
        public const int ADDRT = 60768;
        /// <summary> 常数的偏移量（测试用）</summary>
        public const int ADDRK = 70000;
        /// <summary>
        /// 根据指令的文本格式给定ID
        /// </summary>
        /// <param name="inst">文本</param>
        /// <returns>ID</returns>
        static public int InstID(string inst)
        {
            if (inst.Equals("LD")) return 0x01;
            if (inst.Equals("LDIM")) return 0x02;
            if (inst.Equals("LDI")) return 0x03;
            if (inst.Equals("LDIIM")) return 0x04;
            if (inst.Equals("LDP")) return 0x05;
            if (inst.Equals("LDF")) return 0x06;
            if (inst.Equals("MEP")) return 0x07;
            if (inst.Equals("MEF")) return 0x08;
            if (inst.Equals("INV")) return 0x09;
            if (inst.Equals("OUT")) return 0x0A;
            if (inst.Equals("OUTIM")) return 0x0B;
            if (inst.Equals("SET")) return 0x0C;
            if (inst.Equals("SETIM")) return 0x0D;
            if (inst.Equals("RST")) return 0x0E;
            if (inst.Equals("RSTIM")) return 0x10;
            if (inst.Equals("ALT")) return 0x11;
            if (inst.Equals("ALTP")) return 0x12;

            if (inst.Equals("EQW")) return 0x13;
            if (inst.Equals("NEW")) return 0x14;
            if (inst.Equals("GEW")) return 0x15;
            if (inst.Equals("LEW")) return 0x16;
            if (inst.Equals("GTW")) return 0x17;
            if (inst.Equals("LTW")) return 0x18;
            if (inst.Equals("EQD")) return 0x19;
            if (inst.Equals("NED")) return 0x1A;
            if (inst.Equals("GED")) return 0x1B;
            if (inst.Equals("LED")) return 0x1C;
            if (inst.Equals("GTD")) return 0x1D;
            if (inst.Equals("LTD")) return 0x1E;
            if (inst.Equals("EQF")) return 0x1F;
            if (inst.Equals("NEF")) return 0x20;
            if (inst.Equals("GEF")) return 0x21;
            if (inst.Equals("LEF")) return 0x22;
            if (inst.Equals("GTF")) return 0x23;
            if (inst.Equals("LTF")) return 0x24;

            if (inst.Equals("WTOD")) return 0x25;
            if (inst.Equals("DTOW")) return 0x26;
            if (inst.Equals("DTOF")) return 0x27;
            if (inst.Equals("BIN")) return 0x28;
            if (inst.Equals("BCD")) return 0x29;
            if (inst.Equals("ROUND")) return 0x2A;
            if (inst.Equals("TURNC")) return 0x2B;

            if (inst.Equals("INVW")) return 0x2C;
            if (inst.Equals("INVD")) return 0x2D;
            if (inst.Equals("ANDW")) return 0x2E;
            if (inst.Equals("ANDD")) return 0x30;
            if (inst.Equals("ORW")) return 0x31;
            if (inst.Equals("ORD")) return 0x32;
            if (inst.Equals("XORW")) return 0x33;
            if (inst.Equals("XORD")) return 0x34;

            if (inst.Equals("MOV")) return 0x35;
            if (inst.Equals("MOVD")) return 0x36;
            if (inst.Equals("MOVF")) return 0x37;
            if (inst.Equals("MVBLK")) return 0x38;
            if (inst.Equals("MVDBLK")) return 0x39;

            if (inst.Equals("ADDF")) return 0x3A;
            if (inst.Equals("SUBF")) return 0x3B;
            if (inst.Equals("MULF")) return 0x3C;
            if (inst.Equals("DIVF")) return 0x3D;
            if (inst.Equals("SQRT")) return 0x3F;
            if (inst.Equals("SIN")) return 0x3E;
            if (inst.Equals("COS")) return 0x40;
            if (inst.Equals("TAN")) return 0x41;
            if (inst.Equals("LN")) return 0x42;
            if (inst.Equals("EXP")) return 0x43;

            if (inst.Equals("ADD")) return 0x44;
            if (inst.Equals("ADDD")) return 0x45;
            if (inst.Equals("SUB")) return 0x46;
            if (inst.Equals("SUBD")) return 0x47;
            if (inst.Equals("MUL")) return 0x48;
            if (inst.Equals("MULW")) return 0x49;
            if (inst.Equals("MULD")) return 0x4A;
            if (inst.Equals("DIV")) return 0x4B;
            if (inst.Equals("DIVW")) return 0x4C;
            if (inst.Equals("DIVD")) return 0x4D;
            if (inst.Equals("INC")) return 0x4F;
            if (inst.Equals("INCD")) return 0x50;
            if (inst.Equals("DEC")) return 0x51;
            if (inst.Equals("DECD")) return 0x52;

            if (inst.Equals("TON")) return 0x53;
            if (inst.Equals("TONR")) return 0x54;
            if (inst.Equals("TOF")) return 0x55;

            if (inst.Equals("CTU")) return 0x56;
            if (inst.Equals("CTD")) return 0x57;
            if (inst.Equals("CTUD")) return 0x58;

            if (inst.Equals("FOR")) return 0x59;
            if (inst.Equals("NEXT")) return 0x5A;
            if (inst.Equals("JMP")) return 0x5B;
            if (inst.Equals("LBL")) return 0x5C;
            if (inst.Equals("CALL")) return 0x5D;
            if (inst.Equals("CALLM")) return 0x5E;

            if (inst.Equals("SHL")) return 0x5F;
            if (inst.Equals("SHLD")) return 0x60;
            if (inst.Equals("SHR")) return 0x61;
            if (inst.Equals("SHRD")) return 0x62;
            if (inst.Equals("SHLB")) return 0x63;
            if (inst.Equals("SHRB")) return 0x64;
            if (inst.Equals("ROL")) return 0x65;
            if (inst.Equals("ROLD")) return 0x66;
            if (inst.Equals("ROR")) return 0x67;
            if (inst.Equals("RORD")) return 0x68;

            if (inst.Equals("ATCH")) return 0x69;
            if (inst.Equals("DECH")) return 0x6A;
            if (inst.Equals("EI")) return 0x6B;
            if (inst.Equals("DI")) return 0x6C;

            if (inst.Equals("TRD")) return 0x6D;
            if (inst.Equals("TWR")) return 0x6E;

            if (inst.Equals("MBUS")) return 0x6F;
            if (inst.Equals("SEND")) return 0x70;
            if (inst.Equals("REV")) return 0x71;

            if (inst.Equals("PLSF")) return 0x72;
            if (inst.Equals("DPLSF")) return 0x73;
            if (inst.Equals("PWM")) return 0x74;
            if (inst.Equals("DPWM")) return 0x75;
            if (inst.Equals("PLSY")) return 0x76;
            if (inst.Equals("DPLSY")) return 0x77;
            if (inst.Equals("PLSR")) return 0x78;
            if (inst.Equals("DPLSR")) return 0x79;
            if (inst.Equals("PLSRD")) return 0x7A;
            if (inst.Equals("DPLSRD")) return 0x7B;
            if (inst.Equals("PLSNEXT")) return 0x7C;
            if (inst.Equals("PLSSTOP")) return 0x7D;
            if (inst.Equals("ZRN")) return 0x7E;
            if (inst.Equals("DZRN")) return 0x7F;
            if (inst.Equals("PTO")) return 0x80;
            if (inst.Equals("DRVI")) return 0x81;
            if (inst.Equals("DDRVI")) return 0x82;

            if (inst.Equals("HCNT")) return 0x83;

            if (inst.Equals("LOG")) return 0x84;
            if (inst.Equals("POW")) return 0x85;
            if (inst.Equals("FACT")) return 0x86;
            if (inst.Equals("CMP")) return 0x87;
            if (inst.Equals("CMPD")) return 0x88;
            if (inst.Equals("CMPF")) return 0x89;
            if (inst.Equals("ZCP")) return 0x8A;
            if (inst.Equals("ZCPD")) return 0x8B;
            if (inst.Equals("ZCPF")) return 0x8C;
            if (inst.Equals("NEG")) return 0x8D;
            if (inst.Equals("NEGD")) return 0x8E;
            if (inst.Equals("XCH")) return 0x8F;
            if (inst.Equals("XCHD")) return 0x90;
            if (inst.Equals("XCHF")) return 0x91;
            if (inst.Equals("CML")) return 0x92;
            if (inst.Equals("CMLD")) return 0x93;
            if (inst.Equals("SMOV")) return 0x94;
            if (inst.Equals("FMOV")) return 0x95;
            if (inst.Equals("FMOVD")) return 0x96;

            return 0x00;
        }
        /// <summary>
        /// 通过记号来还原寄存器名称
        /// </summary>
        /// <param name="flag">记号</param>
        /// <returns>名称</returns>
        static public string RegName(int flag)
        {
            if (flag >= ADDRK) return "K" + (flag - ADDRK);
            if (flag >= ADDRT) return "T" + (flag - ADDRT);
            if (flag >= ADDRC) return "C" + (flag - ADDRC);
            if (flag >= ADDRTV) return "TV" + (flag - ADDRTV);
            if (flag >= ADDRCV) return "CV" + (flag - ADDRCV);
            if (flag >= ADDRD) return "D" + (flag - ADDRD);
            if (flag >= ADDRM) return "M" + (flag - ADDRM);
            if (flag >= ADDRAO) return "AO" + (flag - ADDRAO);
            if (flag >= ADDRAI) return "AI" + (flag - ADDRAI);
            if (flag >= ADDRY) return "Y" + (flag - ADDRY);
            if (flag >= ADDRX) return "X" + (flag - ADDRX);
            return "0";
        }
        /// <summary>
        /// 通过寄存器名称来生成记号
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>记号</returns>
        static public int RegAddr(string name)
        {
            if (name[0] == 'K') return ADDRK + int.Parse(name.Substring(1));
            if (name[0] == 'T' && name[1] == 'V') return ADDRTV + int.Parse(name.Substring(2));
            if (name[0] == 'C' && name[1] == 'V') return ADDRCV + int.Parse(name.Substring(2));
            if (name[0] == 'A' && name[1] == 'O') return ADDRAO + int.Parse(name.Substring(2));
            if (name[0] == 'A' && name[1] == 'I') return ADDRAI + int.Parse(name.Substring(2));
            if (name[0] == 'T') return ADDRT + int.Parse(name.Substring(1));
            if (name[0] == 'C') return ADDRC + int.Parse(name.Substring(1));
            if (name[0] == 'D') return ADDRD + int.Parse(name.Substring(1));
            if (name[0] == 'M') return ADDRM + int.Parse(name.Substring(1));
            if (name[0] == 'X') return ADDRX + int.Parse(name.Substring(1));
            if (name[0] == 'Y') return ADDRY + int.Parse(name.Substring(1));
            return 0;
        }
        /// <summary>
        /// 根据指令文本构造指令结构，并加入列表中
        /// </summary>
        /// <param name="insts">指令列表</param>
        /// <param name="inst">指令文本</param>
        static public void AddInst(List<PLCInstruction> insts, string inst)
        {
            insts.Add(new PLCInstruction(inst));
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

        static private int mstackTop = 0;
        /// <summary>所有计数器的预设值sv</summary>
        static private int[] ctsv = new int[256];
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
        /// 将给定的PLC代码转换为C语言代码并输出到文件
        /// </summary>
        /// <param name="sw">文件输出流</param>
        /// <param name="networks">PLC代码的NETWORK集</param>
        static public void InstToCCode(StreamWriter sw, PLCInstNetwork[] networks)
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
                if (net2.Insts.Count() > 1)
                    insts.Add(new PLCInstruction("POP"));
                net1 = net2;
            }
            stackTop = 0;
            mstackTop = 0;
            globalCount = 0;
            int stackTotal = 0;
            int mstackTotal = 0;
            int globalTotal = 0;
            foreach (PLCInstruction inst in insts)
            {
                // 计算普通栈和辅助栈的栈顶
                if (inst.Type.Length > 1 && inst.Type.Substring(0, 2) == "LD")
                    stackTop++;
                if (inst.Type.Equals("ANB") || inst.Type.Equals("ORB") || inst.Type.Equals("POP"))
                    stackTop--;
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
                    case "LDP": case "LDF": case "ANDP": case "ANDF": case "ORP": case "ORF": case "MEP": case "MEF":
                    case "ALTP":
                    case "CTU": case "CTD": case "CTUD":
                    case "FOR":
                        globalTotal++;
                        break;   
                    case "TON": case "TOF": case "TONR":
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
            sw.Write("#include \"lib.h\"\n");
            sw.Write("#include \"main.h\"\n\n");
            //sw.Write("static uint16_t _stack[256];\n");         // 数据栈
            //sw.Write("static uint16_t _stacktop;\n");           // 数据栈的栈顶
            //sw.Write("static uint16_t _mstack[256];\n");        // 辅助栈
            //sw.Write("static uint16_t _mstacktop;\n");          // 辅助栈的栈顶
            sw.Write("static uint16_t _global[{0:d}];\n", globalTotal); // 全局变量
            sw.Write("static uint32_t _old_XBits0;\n");         // 记录之前的X0，判断边沿条件
            sw.Write("static uint32_t _old_XBits1;\n");         // 记录之前的X1，判断边沿条件
            sw.Write("static uint16_t _interrupt_enable;\n");   // 中断开关
            sw.Write("static void (*_interrupt[8])();\n\n");    // 每个中断码对应的中断向量
            // 扫描之前先判断中断条件并触发中断
            sw.Write("void _interrupt_proc()\n{\n");
            // 中断开关是否打开
            sw.Write("if (_interrupt_enable) {\n");
            // X000上升沿
            sw.Write("if (_old_XBits0==0 && XBit[0]!=0 && _interrupt[0])\n");
            sw.Write("_interrupt[0]();\n");
            // X000下降沿
            sw.Write("if (_old_XBits0!=0 && XBit[0]==0 && _interrupt[1])\n");
            sw.Write("_interrupt[1]();\n");
            // X000边沿
            sw.Write("if (((_old_XBits0==0 && XBit[0]!=0) || (_old_XBits1!=0 && XBit[0]==0)) && _interrupt[2])\n");
            sw.Write("_interrupt[2]();\n");
            // X001上升沿
            sw.Write("if (_old_XBits1==0 && XBit[1]!=0 && _interrupt[3])\n");
            sw.Write("_interrupt[3]();\n");
            // X001下降沿
            sw.Write("if (_old_XBits1!=0 && XBit[1]==0 && _interrupt[4])\n");
            sw.Write("_interrupt[4]();\n");
            // X001边沿
            sw.Write("if (((_old_XBits1==0 && XBit[1]!=0) || (_old_XBits1!=0 && XBit[1]==0)) && _interrupt[5])\n");
            sw.Write("_interrupt[5]();\n");
            // 中断定时器1
            sw.Write("if (_interrupt_timer_arrive(6) && !_interrupt[6])\n");
            sw.Write("_interrupt[6]();\n");
            // 中断定时器2
            sw.Write("if (_interrupt_timer_arrive(7) && !_interrupt[7])\n");
            sw.Write("_interrupt[7]();\n}\n");
            // 更新记录的X0和X1
            sw.Write("_old_XBits0 = XBit[0];\n");
            sw.Write("_old_XBits1 = XBit[1];\n");
            // 结束中断判断函数
            sw.Write("}\n\n");
            // 先声明所有的子函数
            foreach (PLCInstruction inst in insts)
            {
                if (inst.Type.Equals("FUNC"))
                    sw.Write("void {0:s}();", inst[1]);
            }
            // 建立扫描的主函数
            sw.Write("void RunLadder()\n{\n");
            sw.Write("_interrupt_proc();\n");
            sw.Write("uint16_t temp;\n");
            // 建立局部的栈和辅助栈
            for (int i = 1; i <= stackTotal; i++)
            {
                sw.Write("uint16_t _stack_{0:d};\n", i);
            }
            for (int i = 1; i <= mstackTotal; i++)
            {
                sw.Write("uint16_t _mstack_{0:d};\n", i);
            }
            // 生成PLC对应的内容
            foreach (PLCInstruction inst in insts)
            {
                InstToCCode(sw, inst);
            }
            sw.Write("}\n");
        }
        /// <summary>
        /// 将给定的指令结构转换为对应的C语言代码
        /// </summary>
        /// <param name="sw">输出的C文件</param>
        /// <param name="inst">指令结构</param>
        static public void InstToCCode(StreamWriter sw, PLCInstruction inst)
        {
            // 第一次判断指令类型
            switch (inst.Type)
            {
                // 函数头部
                case "FUNC":
                    sw.Write("}\n\n");
                    sw.Write("void {0:s}()", inst[1]);
                    sw.Write("{\n");
                    break;
                // 一般的入栈和逻算
                case "LD": case "LDIM":     sw.Write("_stack_{0:d} = {1:s};\n",   ++stackTop, inst[1]); break;
                case "AND": case "ANDIM":   sw.Write("_stack_{0:d} &= {1:s};\n",    stackTop, inst[1]); break;
                case "OR": case "ORIM":     sw.Write("_stack_{0:d} |= {1:s};\n",    stackTop, inst[1]); break;
                case "LDI": case "LDIIM":   sw.Write("_stack_{0:d} = !{1:s};\n",  ++stackTop, inst[1]); break;
                case "ANDI": case "ANDIIM": sw.Write("_stack_{0:d} &= !{1:s};\n",   stackTop, inst[1]); break;
                case "ORI": case "ORIIM":   sw.Write("_stack_{0:d} |= !{1:s};\n",   stackTop, inst[1]); break;
                case "INV": sw.Write("_stack_{0:d} ^= 1;\n", stackTop); break;
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
                case "MEP":
                    sw.Write("temp = _stack_{0:d};\n", stackTop);
                    sw.Write("_stack_{0:d} = (_global[{1:d}]==0&&temp==1);\n", stackTop, globalCount);
                    sw.Write("_global[{0:d}] = temp;\n", globalCount++);
                    break;
                case "MEF":
                    sw.Write("temp = _stack_{0:d};\n", stackTop);
                    sw.Write("_stack_{0:d} = (_global[{1:d}]==1&&temp==0);\n", stackTop, globalCount);
                    sw.Write("_global[{0:d}] = temp;\n", globalCount++);
                    break;
                // 出栈
                case "POP": stackTop--; break;
                // 栈合并
                case "ANB":
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
                    sw.Write("_stack_{0:d} &= ({0:s}<{1:s});\n",  stackTop, inst[1], inst[2]); break;
                case "ORWL": case "ORDL": case "ORFL":
                    sw.Write("_stack_{0:d} |= ({0:s}<{1:s});\n",  stackTop, inst[1], inst[2]); break;
                // 输出线圈
                /*
                 * 将当前栈顶的值赋值给线圈
                 */
                case "OUT": sw.Write("{0:s} = _stack_{1:d};\n", inst[1], stackTop); break;
                // 置位和复位
                /*
                 * 需要用if来判断栈顶是否为1
                 */
                case "SET": case "SETIM":
                    sw.Write("if (_stack_{0:d}) _bitset(&{1:s}, {2:s});\n", stackTop, inst[1], inst[2]); break;
                case "RST": case "RSTIM":
                    sw.Write("if (_stack_{0:d}) \n{{\n_bitrst(&{1:s}, {2:s});\n", stackTop, inst[1], inst[2]);
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
                            sw.Write("CVWord[{0:d}] = {1:d}", i, ctsv[i]);
                    }
                    sw.Write("}\n");
                break;
                // 交替
                case "ALT": sw.Write("if (_stack_{0:d}) {1:s}^=1;\n", stackTop, inst[1]); break;
                // 上升沿交替
                case "ALTP":
                    sw.Write("if (_global[{0:d}]==0&&_stack_{1:d}==1) ", globalCount, stackTop);
                    sw.Write("_global[{0:d}] = {1:s}\n", globalCount++, inst[1]);
                    break;
                // 当栈顶为1时运行的计时器
                case "TON":
                    sw.Write("_ton(&{0:s}, &{2:s}, _stack_{4:d}, {1:s}, &_global[{3:d}]);\n", 
                        inst[1], inst[2], inst[3], globalCount, stackTop);
                    globalCount += 2;
                    break;
                // 当栈顶为0时运行的计时器
                case "TOF":
                    sw.Write("_ton(&{0:s}, &{2:s}, !_stack_{4:d}, {1:s}, &_global[{3:d}]);\n", 
                        inst[1], inst[2], inst[3], globalCount, stackTop);
                    globalCount += 2;
                    break;
                // 当栈顶为1时运行，为0时保留当前计时的计时器
                case "TONR":
                    sw.Write("_tonr(&{0:s}, &{2:s}, _stack_{4:d}, {1:s}, &_global[{3:d}]);\n", 
                        inst[1], inst[2], inst[3], globalCount, stackTop);
                    globalCount += 2;
                    break;
                // 向上计数器，每次栈顶上升跳变时加1
                // 当计数到达目标后计数开关设为1
                case "CTU":
                    sw.Write("if (_global[{0:d}]==0&&_stack_{1:d}==1&&!{2:s})\n", globalCount, stackTop, inst[3]);
                    sw.Write("if (++{0:s}>={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 向下计数器
                case "CTD":
                    sw.Write("if (_global[{0:d}]==0&&_stack_{1:d}==1&&!{2:s})\n", globalCount, stackTop, inst[3]);
                    sw.Write("if (--{0:s}<={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
                    sw.Write("_global[{0:d}] = _stack_{1:d};\n", globalCount++, stackTop);
                    break;
                // 向上向下计数器，当当前计数小于目标则加1，大于目标则减1
                case "CTUD":
                    sw.Write("if (_global[{0:d}]==0&&_stack_{1:d}==1&&!{2:s})\n", globalCount, stackTop, inst[3]);
                    sw.Write("if (({0:s}<{1:s}?{0:s}++:{0:s}--)=={1:s}) {2:s} = 1;\n", inst[1], inst[2], inst[3]);
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
                // CALL指令，调用子函数
                case "CALL":
                    sw.Write("if (_stack_{0:d})) \n", stackTop);
                    sw.Write("_SBR_{0:s}();\n", inst[1]);
                    break;
                // CALLM指令，调用用户实现的c语言宏指令
                case "CALLM":
                    sw.Write("if (_stack_{0:d})) \n", stackTop);
                    sw.Write("{0:s}(&{1:s}, &{2:s});\n", inst[1], inst[2], inst[3]);
                    break;
                // 辅助栈操作
                case "MPS": sw.Write("_mstack_{0:d} = _stack_{1:d};\n", ++mstackTop, stackTop); break;
                case "MRD": sw.Write("_stack_{0:d} = _mstack_{1:d};\n", stackTop, mstackTop); break;
                case "MPP": sw.Write("_stack_{0:d} = _mstack_{1:d};\n", stackTop, mstackTop--); break;
                // 默认的其他情况，一般之前要先判断栈顶
                default:
                    sw.Write("if (_stack_{0:d}) {{\n", stackTop);
                    // 第二回指令判断
                    switch (inst.Type)
                    {
                        // 数据格式的转化指令
                        case "WTOD": sw.Write("{1:s} = (uint32_t)({0;s});\n", inst[1], inst[2]); break;
                        case "DTOW": sw.Write("{1:s} = (uint16_t)({0:s});\n", inst[1], inst[2]); break;
                        case "DTOF": sw.Write("{1:s} = (float)({0:s});\n", inst[1], inst[2]); break;
                        case "BIN": sw.Write("{1:s} = _BCD_to_WORD({0:s});\n", inst[1], inst[2]); break;
                        case "BCD": sw.Write("{1:s} = _WORD_to_BCD({0:s});\n", inst[1], inst[2]); break;
                        case "ROUND": sw.Write("*((long*)(&{1:s})) = round(*((double*)(&{0:s}));\n", inst[1], inst[2]); break;
                        case "TURNC": sw.Write("*((long*)(&{1:s})) = turnc(*((double*)(&{0:s}));\n", inst[1], inst[2]); break;
                        // 位运算指令
                        case "INVW": case "INVD": sw.Write("{1:s} = ~{0:s};\n", inst[1], inst[2]);break;
                        case "ANDW": case "ANDD": sw.Write("{2:s} = {0:s}&{1:s}", inst[1], inst[2], inst[3]);break;
                        case "ORW": case "ORD": sw.Write("{2:s} = {0:s}|{1:s}", inst[1], inst[2], inst[3]); break;
                        case "XORW": case "XORD": sw.Write("{2:s} = {0:s}^{1:s}", inst[1], inst[2], inst[3]); break;
                        // 寄存器移动指令
                        case "MOV": case "MOVD": case "MOVF": sw.Write("{1:s} = {0:s};\n", inst[1], inst[2]);break;
                        case "MVBLK": sw.Write("memcpy(&{0:s}, &{1:s}, {2:s}*2);\n", inst[1], inst[2], inst[3]);break;
                        case "MVDBLK": sw.Write("memcpy(&{0:s}, &{1:s}, {2:s}*4);\n", inst[1], inst[2], inst[3]);break;
                        // 数学运算指令
                        case "ADD": case "ADDD": case "ADDF": sw.Write("{2:s} = {0:s} + {1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "SUB": case "SUBD": case "SUBF": sw.Write("{2:s} = {0:s} - {1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "MUL": case "MULD": case "MULF": sw.Write("{2:s} = {0:s} * {1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "DIV": sw.Write("{2:s} = (({0:s}%{1:s})<<16)|(({0:s}/{1:s})&65535);", inst[1], inst[2], inst[3]); break;
                        case "DIVD": case "DIVF": sw.Write("{2:s} = {0:s} / {1:s};\n", inst[1], inst[2], inst[3]); break;
                        case "INC": case "INCD":sw.Write("{1:s} = {0:s} + 1;\n", inst[1], inst[2]); break;
                        case "DEC": case "DECD":sw.Write("{1:s} = {0:s} - 1;\n", inst[1], inst[2]) ;break;
                        case "SIN": sw.Write("{1:s} = sin({0:s});\n", inst[1], inst[2]); break;
                        case "COS": sw.Write("{1:s} = cos({0:s});\n", inst[1], inst[2]); break;
                        case "TAN": sw.Write("{1:s} = tan({0:s});\n", inst[1], inst[2]); break;
                        case "LN": sw.Write("{1:s} = log({0:s});\n", inst[1], inst[2]); break;
                        case "EXP": sw.Write("{1:s} = exp({0:s});\n", inst[1], inst[2]); break;
                        // 移位指令
                        case "SHL": case "SHLD": sw.Write("{2:s} = {0:s} << {1:s};\n", inst[1], inst[2], inst[3]);break;
                        case "SHR": case "SHRD": sw.Write("{2:s} = {0:s} >> {1:s};\n", inst[1], inst[2], inst[3]);break;
                        case "ROL": sw.Write("{2:s} = ({0:s}<<{1:s})|({0:s}>>(16-{1:s});\n", inst[1] ,inst[2], inst[3]); break;
                        case "ROR": sw.Write("{2:s} = ({0:s}>>{1:s})|({0:s}<<(16-{1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "ROLD": sw.Write("{2:s} = ({0:s}<<{1:s})|({0:s}>>(32-{1:s});\n", inst[1], inst[2], inst[3]); break;
                        case "RORD": sw.Write("{2:s} = ({0:s}>>{1:s})|({0:s}<<(32-{1:s});\n", inst[1], inst[2], inst[3]); break;
                        // 线圈组的移位
                        case "SHLB":
                            /*
                             * 这里以线圈组的左移距离
                             * current -> M0M1M2M3M4M5M6...Mn
                             * target  -> X0X1..Xm-1M0M1...Mn-m
                             * 需要以下三个步骤
                             * 1. 将M0M1..Mm-1拷贝到预设的临时缓冲区里
                             * 2. 将拷贝过的数据段再拷贝到MmMm+1...Mn里
                             * 3. 将X0X1...Xm-1拷贝到M0M1...M-1里
                             */
                            sw.Write("_bitcpy((void*)(((int)(&{0:s}))+{1:s}), _buffer, {2:s});\n", inst[2], inst[4], inst[3]);
                            sw.Write("_bitcpy(_buffer, {0:s}, {1:s});\n", inst[2], inst[3]);
                            sw.Write("_bitcpy({0:s}, (void*)(((int)(&{1:s}))+{2:s}-{3:s}), {3:s});", inst[1], inst[2], inst[3], inst[4]);
                        break;
                        case "SHRB":
                            // 右移的步骤和左移一致，但方向相反
                            sw.Write("_bitcpy({0:s}, _buffer, {1:s});\n", inst[2], inst[3]);
                            sw.Write("_bitcpy(_buffer, (void*)(((int)(&{0:s}))+{1:s}), {2:s});\n", inst[2], inst[4], inst[3]);
                            sw.Write("_bitcpy({0:s}, {1:s}, {2:s});", inst[1], inst[2], inst[4]);
                        break;
                        // 辅助功能
                        case "LOG":sw.Write("{1:d} = log({0:d})/log(10);\n", inst[1], inst[2]);break;
                        case "POW":sw.Write("{2:d} = power({0:d}, {1:d});\n", inst[1], inst[2], inst[3]);break;
                        case "FACT":sw.Write("{1:d} = _fact({0:d);\n", inst[1], inst[2]); break;
                        case "CMP":  sw.Write("{2:d} = _cmpw({0:d}, {1:d});\n", inst[1], inst[2], inst[3]);break;
                        case "CMPD": sw.Write("{2:d} = _cmpd({0:d}, {1:d});\n", inst[1], inst[2], inst[3]); break;
                        case "CMPF": sw.Write("{2:d} = _cmpf({0:d}, {1:d});\n", inst[1], inst[2], inst[3]); break;
                        case "ZCP": sw.Write("{3:d} = _zcpw({0:d}, {1:d}, {2:d});\n", inst[1], inst[2], inst[3], inst[4]);break;
                        case "ZCPD": sw.Write("{3:d} = _zcpd({0:d}, {1:d}, {2:d});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "ZCPF": sw.Write("{3:d} = _zcpf({0:d}, {1:d}, {2:d});\n", inst[1], inst[2], inst[3], inst[4]); break;
                        case "NEG": case "NEGD": sw.Write("{1:d} = -{0:d};\n", inst[1], inst[2]); break;
                        case "XCH": sw.Write("_xch(&{0:d}, &{1:d});\n", inst[1], inst[2]);break;
                        case "XCHD": sw.Write("_xchd(&{0:d}, &{1:d});\n", inst[1], inst[2]);break;
                        case "XCHF": sw.Write("_xchf(&{0:d}, &{1:d});\n", inst[1], inst[2]); break;
                        case "CML": case "CMLD": sw.Write("{1:d} = ~{0:d};\n", inst[1], inst[2]);break;
                        // 设置中断
                        case "ATCH":
                            sw.Write("_interrupt[{1:s}] = INTR_{0:s};\n", inst[1], inst[2]);
                            // 初始化中断定时器
                            sw.Write("_interrupt_timer_init({0:s}, DWords[8173], DWords[8174]);\n", inst[2]);
                            break;
                        case "DTCH":
                            sw.Write("_interrupt[{0:s}] = null;\n", inst[1]);
                            
                            sw.Write("_interrupt_timer_cancel({0:s})", inst[1]);
                            break;
                        case "EI": sw.Write("_interrupt_enable = 1;\n");break;
                        case "DI": sw.Write("_interrupt_enable = 0;\n");break;
                        // 移位操作
                        case "SMOV": sw.Write("_smov({0:s}, {1:s}, {2:s}, &{3:s}, {4:s});\n",
                            inst[1], inst[2], inst[3], inst[4], inst[5]);break;
                        case "FMOV": sw.Write("_memsetw(&{1:s}, {0:s}, {2:s});\n", inst[1], inst[2], inst[3]);break;
                        case "FMOVD": sw.Write("_memsetd(&{1:s}, {0:s}, {2:s};\n", inst[1], inst[2], inst[3]);break;
                        default: throw new ArgumentException(String.Format("unidentified PLC command : {0:s}", inst.Type));
                    }
                    sw.Write("}\n");
                    // 注意栈顶为0时重置一般的计时器
                    if (inst.Type.Equals("TON"))
                    {
                        sw.Write("else\n{{\n{0:s}=0;\n}}\n", inst[1]);
                    }
                break; 
            }
        }
        
        static private string _GetBit(string flag)
        {
            string[] flags = flag.Split(' ');
            return String.Format("(({0:s}>>{1:s})&1)", flags[0], flags[1]);
        }
        /// <summary>
        /// 生成设置特定的位的c代码
        /// </summary>
        /// <param name="flag">位所在的整数以及位的位置，中间用空格分开</param>
        /// <param name="value">设定值（0或1）</param>
        /// <returns>c代码</returns>
        static private string _SetBit(string flag, string value)
        {
            string[] flags = flag.Split(' ');
            return String.Format("{0:s}&=(~(1<<{1:s}))|({2:s}<<{1:s})", flags[0], flags[1], value);         
        }
        /// <summary>
        /// 生成异或特定的位的c代码
        /// </summary>
        /// <param name="flag">位所在的整数以及位的位置，中间用空格分开</param>
        /// <param name="value">设定值（0或1）</param>
        /// <returns>c代码</returns>
        static private string _XorBit(string flag)
        {
            string[] flags = flag.Split(' ');
            return String.Format("{0:s}^=1<<{1:s}", flags[0], flags[1]);
        }
    }
}