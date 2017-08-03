using SamSoarII.Core.Generate;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SamSoarII.Core.Simulate;

namespace SamSoarII.Core.Models
{
    public enum LadderUnitAction { ADD, REMOVE, MOVE, UPDATE};

    public class LadderUnitChangedEventException : Exception
    {
        public LadderUnitAction Action { get; private set; }
        public LadderUnitChangedEventException(LadderUnitAction _action, string _msg) : base(_msg) { Action = _action; }
    }
    
    public class LadderUnitChangedEventArgs : EventArgs
    {
        public LadderUnitAction Action { get; private set; }
        public LadderUnitChangedEventArgs(LadderUnitAction _action) { Action = _action; }
    }

    public delegate void LadderUnitChangedEventHandler(LadderUnitModel sender, LadderUnitChangedEventArgs e);

    public class LadderUnitModel : IModel
    {
        #region Resources
        
        public enum Outlines
        {
            BitOperation, Compare, Convert,
            LogicOperation, Move,
            FloatCalculation, IntegerCalculation,
            Timer, Counter, ProgramControl,
            Shift, Interrupt,
            RealTime, Communication,
            Pulse, HighCount, Auxiliar,
            NULL
        }
        public enum Types
        {
            LD, LDI, LDIM, LDIIM, LDP, LDF, MEP, MEF, INV,
            OUT, OUTIM, SET, SETIM, RST, RSTIM, ALT, ALTP, 
            LDWEQ, LDWNE, LDWGE, LDWLE, LDWG, LDWL,
            LDDEQ, LDDNE, LDDGE, LDDLE, LDDG, LDDL,
            LDFEQ, LDFNE, LDFGE, LDFLE, LDFG, LDFL,
            WTOD, DTOW, DTOF, BIN, BCD, ROUND, TRUNC, 
            INVW, INVD, ANDW, ANDD, ORW, ORD, XORW, XORD,
            MOVD, MOV, MOVF, MVBLK, MVDBLK,
            ADDF, SUBF, MULF, DIVF, SQRT, SIN, COS, TAN, LN, EXP,
            ADD, ADDD, SUB, SUBD, MUL, MULW, MULD, DIV, DIVW, DIVD, INC, INCD, DEC, DECD,
            TON, TOF, TONR,
            CTU, CTD, CTUD,
            FOR, NEXT, JMP, LBL, CALL, CALLM, STL, STLE, ST,
            SHL, SHLD, SHR, SHRD, ROL, ROLD, ROR, RORD, SHLB, SHRB,
            ATCH, DTCH, EI, DI,
            TRD, TWR,
            MBUS, SEND, REV,
            PLSF, DPLSF, PWM, DPWM, PLSY, DPLSY, PLSR, DPLSR, PLSRD, DPLSRD, PLSNEXT, PLSSTOP, ZRN, DZRN, PTO, DRVI, DDRVI,
            HCNT,
            LOG, POW, FACT, CMP, CMPD, CMPF, ZCP, ZCPD, ZCPF, NEG, NEGD, XCH, XCHD, XCHF, CML, CMLD, SMOV, FMOV, FMOVD,
            VLINE, HLINE, NULL
        }
        public enum Shapes
        {
            Input, Output, OutputRect, Special, HLine, VLine, Null
        }

        static readonly public LadderUnitFormat[] Formats;
        static readonly public Dictionary<string, Types> TypeOfNames;
        static readonly public Types[] LabelTypes;

        static LadderUnitModel()
        {
            Formats = new LadderUnitFormat[(int)Types.NULL + 1];
            ValueFormat[] vformats = null;
            vformats = new ValueFormat[] {
                new ValueFormat("IN", ValueModel.Types.BOOL, true, false, 0, new Regex[]{ ValueModel.VerifyBitRegex1, ValueModel.WordBitRegex}) };

            Formats[(int)Types.LD] = new LadderUnitFormat(200, "LD", Types.LD, Outlines.BitOperation, Shapes.Input,
                Properties.Resources.MainWindow_Normally_Open_Contact,
                "当位等于1时，常开触点接通（ON），否则断开（OFF）。",
                "Access it (ON) when the value of bit is 1, otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.LDI] = new LadderUnitFormat(201, "LDI", Types.LDI, Outlines.BitOperation, Shapes.Input,
                Properties.Resources.MainWindow_Immediately_Open,
                "当实际输入点（位）是1时，立即常开触点接通（ON），否则断开（OFF）。",
                "Immediately access it (ON) when the value of bit is 1, otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.LDIM] = new LadderUnitFormat(202, "LDIM", Types.LDIM, Outlines.BitOperation, Shapes.Input,
                Properties.Resources.MainWindow_Normally_Closed,
                "当位等于0时，常闭触点接通（ON），否则断开（OFF）。",
                "Access it (ON) when the value of bit is 0, otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.LDIIM] = new LadderUnitFormat(203, "LDIIM", Types.LDIIM, Outlines.BitOperation, Shapes.Input,
                Properties.Resources.MainWindow_Immediately_Close,
                "当实际输入点（位）是0时，立即常闭触点接通（ON），否则断开（OFF）。",
                "Immediately access it (ON) when the value of bit is 0, otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.LDP] = new LadderUnitFormat(204, "LDP", Types.LDP, Outlines.BitOperation, Shapes.Input,
                Properties.Resources.MainWindow_Rising_Edge_Pulse,
                "触点允许一次扫描中每次执行“0”到“1”（OFF → ON）转换时，将堆栈顶值设为1；否则，将其设为0；",
                "Access it (ON) when encounter the rising edge (OFF → ON), otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.LDF] = new LadderUnitFormat(205, "LDF", Types.LDF, Outlines.BitOperation, Shapes.Input,
                Properties.Resources.MainWindow_Falling_Edge_Pulse,
                "触点允许一次扫描中每次执行“1”到“0”（ON → OFF）转换时，将堆栈顶值设为1；否则，将其设为0；",
                "Access it (ON) when encounter the trailing edge (ON → OFF), otherwise open it (OFF).",
                vformats);
            vformats = new ValueFormat[] { };
            Formats[(int)Types.MEP] = new LadderUnitFormat(206, "MEP", Types.MEP, Outlines.BitOperation, Shapes.Special,
                Properties.Resources.MainWindow_Rising_Edge_Of_Result,
                "在至MEP指令为止的运算结果的上升沿时（OFF→ ON）变为ON（导通状态）；",
                "Access it (ON) when the leftside result encounter the rising edge (OFF → ON), otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.MEF] = new LadderUnitFormat(207, "MEF", Types.MEF, Outlines.BitOperation, Shapes.Special,
                Properties.Resources.MainWindow_Falling_Edge_Of_Result,
                "在至MEF指令为止的运算结果的下降沿时（ON → OFF）变为ON（导通状态）；",
                "Access it (ON) when the leftside result encounter the trailing edge (ON → OFF), otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.INV] = new LadderUnitFormat(208, "INV", Types.INV, Outlines.BitOperation, Shapes.Special,
                Properties.Resources.MainWindow_Reversed_Result,
                "将至INV之前的运算结果取反后作为输出的使能。",
                "Access it (ON) when the leftside result is 0, otherwise open it (OFF).",
                vformats);
            Formats[(int)Types.NEXT] = new LadderUnitFormat(1101, "NEXT", Types.NEXT, Outlines.ProgramControl, Shapes.OutputRect,
                Properties.Resources.NEXT_Inst,
                "始终与此条NEXT指令之前的最近一次的FOR指令配套，结束FOR循环。",
                "It always corrosponds with FOR instruction nearest to it, and terminate the FOR cycle",
                vformats);
            Formats[(int)Types.EI] = new LadderUnitFormat(1302, "EI", Types.EI, Outlines.Interrupt, Shapes.OutputRect,
                Properties.Resources.EI_Inst,
                "指令全局性启用所有附加中断事件进程。要执行中断子程序，必须先开启全局中断。",
                "Enable the interruption and allow to run the interruption routines",
                vformats);
            Formats[(int)Types.DI] = new LadderUnitFormat(1303, "DI", Types.DI, Outlines.Interrupt, Shapes.OutputRect,
                Properties.Resources.DI_Inst,
                "指令全局性禁止所有中断事件进程。",
                "Disable to invoke the interruption event.",
                vformats);
            Formats[(int)Types.HLINE] = new LadderUnitFormat(1, "HLINE", Types.HLINE, Outlines.NULL, Shapes.HLine,
                "", "", "", vformats);
            Formats[(int)Types.VLINE] = new LadderUnitFormat(2, "VLINE", Types.VLINE, Outlines.NULL, Shapes.VLine,
                "", "", "", vformats);
            Formats[(int)Types.NULL] = new LadderUnitFormat(0, "NULL", Types.NULL, Outlines.NULL, Shapes.Null,
                "", "", "", vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, 0, new Regex[]{ ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) };
            Formats[(int)Types.OUT] = new LadderUnitFormat(209, "OUT", Types.OUT, Outlines.BitOperation, Shapes.Output,
                Properties.Resources.MainWindow_Output_Coil,
                "线圈前方条件持续导通时，线圈才会输出；",
                "Output this coil when the condition is continuously true.",
                vformats);
            Formats[(int)Types.OUTIM] = new LadderUnitFormat(210, "OUTIM", Types.OUTIM, Outlines.BitOperation, Shapes.Output,
                Properties.Resources.MainWindow_Immediately_Output_Coil,
                "指令将输出位的新数值立即写入输出寄存器，并输出。",
                "Immediately output this coil when the condition is continuously true.",
                vformats);
            Formats[(int)Types.ALT] = new LadderUnitFormat(215, "ALT", Types.ALT, Outlines.BitOperation, Shapes.OutputRect,
                Properties.Resources.Alternating_Output,
                "当堆栈顶值为1时，指令让目标软元件的状态在1（ON）和0（OFF）之间连续交替转换。",
                "Alterminately changed the targeted register between 1(ON) and 0(OFF) when the conditon is true.",
                vformats);
            Formats[(int)Types.ALTP] = new LadderUnitFormat(216, "ALTP", Types.ALTP, Outlines.BitOperation, Shapes.OutputRect,
                Properties.Resources.Pulse_Alternation,
                "当执行到此条指令之前的结果发生了上升沿（OFF -- ON）变化时，目标软元件的状态在1（ON）和0（OFF）之间交替转换。",
                "Alterminately changed the targeted register between 1(ON) and 0(OFF) when the conditon encounter the rising edge.（OFF -- ON）",
                vformats);
            vformats = new ValueFormat[] {
                new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, 0, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}),
                new ValueFormat("CT", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}) };
            Formats[(int)Types.SET] = new LadderUnitFormat(211, "SET", Types.SET, Outlines.BitOperation, Shapes.Output,
                Properties.Resources.Set_Coil,
                "指令置位从（地址位）开始的（长度）个线圈，最大点数根据操作数类型设定。",
                "Set a range of coils to ON, the start and count of these coils can be defined.",
                vformats);
            Formats[(int)Types.SETIM] = new LadderUnitFormat(212, "SETIM", Types.SETIM, Outlines.BitOperation, Shapes.Output,
                Properties.Resources.Set_Immediately,
                "指令立即置位从（地址位）开始的（长度）个线圈，最大点数根据操作数类型设定。",
                "Immediately set a range of coils to ON, the start and count of these coils can be defined.",
                vformats);
            Formats[(int)Types.RST] = new LadderUnitFormat(213, "RST", Types.RST, Outlines.BitOperation, Shapes.Output,
                Properties.Resources.Reset_Coil,
                "指令复位从（地址位）开始的（长度）个线圈，最大点数根据操作数类型设定。",
                "Reset a range of coils to OFF, the start and count of these coils can be defined.",
                vformats);
            Formats[(int)Types.RSTIM] = new LadderUnitFormat(214, "RSTIM", Types.RSTIM, Outlines.BitOperation, Shapes.Output,
                Properties.Resources.Reset_Immediately,
                "指令立即复位从（地址位）开始的（长度）个线圈，最大点数根据操作数类型设定。",
                "Immediately set a range of coils to ON, the start and count of these coils can be defined.",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) };
            Formats[(int)Types.LDWEQ] = new LadderUnitFormat(300, "LDWEQ", Types.LDWEQ, Outlines.Compare, Shapes.Input,
                Properties.Resources.Word_Equal,
                "比较IN1是否等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 16-bit word IN1 and IN2, access it (ON) when equal.",
                vformats);
            Formats[(int)Types.LDWNE] = new LadderUnitFormat(301, "LDWNE", Types.LDWNE, Outlines.Compare, Shapes.Input,
                Properties.Resources.Word_Not_Equal,
                "比较IN1是否不等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 16-bit word IN1 and IN2, access it (ON) when not equal.",
                vformats);
            Formats[(int)Types.LDWLE] = new LadderUnitFormat(302, "LDWLE", Types.LDWLE, Outlines.Compare, Shapes.Input,
                Properties.Resources.Word_Not_More,
                "比较IN1是否小于等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 16-bit word IN1 and IN2, access it (ON) when IN1 is not more than IN2.",
                vformats);
            Formats[(int)Types.LDWGE] = new LadderUnitFormat(303, "LDWGE", Types.LDWGE, Outlines.Compare, Shapes.Input,
                Properties.Resources.Word_Not_Less,
                "比较IN1是否小于等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 16-bit word IN1 and IN2, access it (ON) when IN1 is not less than IN2.",
                vformats);
            Formats[(int)Types.LDWL] = new LadderUnitFormat(304, "LDWL", Types.LDWL, Outlines.Compare, Shapes.Input,
                Properties.Resources.Word_Less,
                "比较IN1是否小于IN2，如果比较结果为真实，输出打开。",
                "Compare the 16-bit word IN1 and IN2, access it (ON) when IN1 is less than IN2.",
                vformats);
            Formats[(int)Types.LDWG] = new LadderUnitFormat(305, "LDWG", Types.LDWG, Outlines.Compare, Shapes.Input,
                Properties.Resources.Word_More,
                "比较IN1是否大于IN2，如果比较结果为真实，输出打开。",
                "Compare the 16-bit word IN1 and IN2, access it (ON) when IN1 is move than IN2.",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}) };
            Formats[(int)Types.LDDEQ] = new LadderUnitFormat(306, "LDDEQ", Types.LDDEQ, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Equal,
                "比较IN1是否等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when equal.",
                vformats);
            Formats[(int)Types.LDDNE] = new LadderUnitFormat(307, "LDDNE", Types.LDDNE, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Not_Equal,
                "比较IN1是否不等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when not equal.",
                vformats);
            Formats[(int)Types.LDDLE] = new LadderUnitFormat(308, "LDDLE", Types.LDDLE, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Not_More,
                "比较IN1是否小于等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is not more than IN2.",
                vformats);
            Formats[(int)Types.LDDGE] = new LadderUnitFormat(309, "LDDGE", Types.LDDGE, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Not_Less,
                "比较IN1是否小于等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is not less than IN2.",
                vformats);
            Formats[(int)Types.LDDL] = new LadderUnitFormat(310, "LDDL", Types.LDDL, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Less,
                "比较IN1是否小于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is less than IN2.",
                vformats);
            Formats[(int)Types.LDDG] = new LadderUnitFormat(311, "LDDG", Types.LDDG, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_More,
                "比较IN1是否大于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is move than IN2.",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}) };
            Formats[(int)Types.LDFEQ] = new LadderUnitFormat(312, "LDFEQ", Types.LDFEQ, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Equal,
                "比较IN1是否等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when equal.",
                vformats);
            Formats[(int)Types.LDFNE] = new LadderUnitFormat(313, "LDFNE", Types.LDFNE, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Not_Equal,
                "比较IN1是否不等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when not equal.",
                vformats);
            Formats[(int)Types.LDFLE] = new LadderUnitFormat(314, "LDFLE", Types.LDFLE, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Not_More,
                "比较IN1是否小于等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is not more than IN2.",
                vformats);
            Formats[(int)Types.LDFGE] = new LadderUnitFormat(315, "LDFGE", Types.LDFGE, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Not_Less,
                "比较IN1是否小于等于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is not less than IN2.",
                vformats);
            Formats[(int)Types.LDFL] = new LadderUnitFormat(316, "LDFL", Types.LDFL, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_Less,
                "比较IN1是否小于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is less than IN2.",
                vformats);
            Formats[(int)Types.LDFG] = new LadderUnitFormat(317, "LDFG", Types.LDFG, Outlines.Compare, Shapes.Input,
                Properties.Resources.DWord_More,
                "比较IN1是否大于IN2，如果比较结果为真实，输出打开。",
                "Compare the 32-bit double word IN1 and IN2, access it (ON) when IN1 is move than IN2.",
                vformats);
            Formats[(int)Types.WTOD] = new LadderUnitFormat(400, "WTOD", Types.WTOD, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.Word_To_DWord,
                "将单字的IN转为双字的OUT。",
                "Convert the 16-bit word IN to 32-bit double word OUT",
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}) });
            Formats[(int)Types.DTOW] = new LadderUnitFormat(401, "DTOW", Types.DTOW, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.DWord_To_Word,
                "将双字的IN转为单字的OUT。",
                "Convert the 32-bit double word IN to 16-bit word OUT",
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex2, ValueModel.BitWordRegex}) });
            Formats[(int)Types.DTOF] = new LadderUnitFormat(402, "DTOF", Types.DTOF, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.DWord_To_Float,
                "将双字的IN转为浮点的OUT。",
                "Convert the 32-bit double word IN to 32-bit float OUT",
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.FLOAT, false, true, -1, new Regex[] { ValueModel.VerifyFloatRegex}) });
            vformats = new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex2, ValueModel.BitWordRegex}) };
            Formats[(int)Types.BIN] = new LadderUnitFormat(403, "BIN", Types.BIN, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.BCD_Code_To_Integer,
                "将BCD码IN转为单字的OUT。",
                "Convert the BCD code IN to 16-bit word OUT",
                vformats);
            Formats[(int)Types.BCD] = new LadderUnitFormat(404, "BCD", Types.BCD, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.Integer_To_BCD_Code,
                "将单字的IN转为BCD码OUT。",
                "Convert the 16-bit word IN to BCD code OUT",
                vformats);
            Formats[(int)Types.INVW] = new LadderUnitFormat(500, "INVW", Types.INVW, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.Word_Reverse,
                "对IN执行取反操作，并将结果载入输出到OUT。",
                "Reverse the 16-bit word IN and assign to OUT.",
                vformats);
            Formats[(int)Types.MOV] = new LadderUnitFormat(600, "MOV", Types.MOV, Outlines.Move, Shapes.OutputRect,
                Properties.Resources.Move_Word,
                "将单字IN移动到单字OUT中，不改变IN的值。",
                "Move the 16-bit word IN to OUT, keep the value of IN",
                vformats);
            Formats[(int)Types.INC] = new LadderUnitFormat(810, "INC", Types.INC, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Word_Add_One,
                "将单字IN加一（IN + 1），产生的整数结果保存到OUT中。",
                "Increase one to 16-bit word IN (IN + 1) and assign to OUT.",
                vformats);
            Formats[(int)Types.DEC] = new LadderUnitFormat(812, "DEC", Types.DEC, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Word_Minus_One,
                "将单字IN减一（IN - 1），产生的整数结果保存到OUT中。",
                "Decrease one to 16-bit word IN (IN - 1) and assign to OUT",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}) };
            Formats[(int)Types.INVD] = new LadderUnitFormat(501, "INVD", Types.INVD, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.DWord_Reverse,
                "对IN执行取反操作，并将结果载入输出到OUT。",
                "Reverse the 32-bit double word IN and assign to OUT.",
                vformats);
            Formats[(int)Types.MOVD] = new LadderUnitFormat(601, "MOVD", Types.MOVD, Outlines.Move, Shapes.OutputRect,
                Properties.Resources.Move_DWord,
                "将双字IN移动到双字OUT中，不改变IN的值。",
                "Move the 32-bit double word IN to OUT, keep the value of IN",
                vformats);
            Formats[(int)Types.INCD] = new LadderUnitFormat(811, "INCD", Types.INCD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.DWord_Add_One,
                "将双字IN加一（IN + 1），产生的整数结果保存到OUT中。",
                "Increase one to 32-bit double word IN (IN + 1) and assign to OUT.",
                vformats);
            Formats[(int)Types.DECD] = new LadderUnitFormat(812, "DECD", Types.DECD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.DWord_Minus_One,
                "将双字IN减一（IN - 1），产生的整数结果保存到OUT中。",
                "Decrease one to 32-bit double word IN (IN - 1) and assign to OUT",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.FLOAT, true, false, 0, new Regex[] {ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.FLOAT, false, true, -1, new Regex[] { ValueModel.VerifyFloatRegex}) };
            Formats[(int)Types.MOVF] = new LadderUnitFormat(602, "MOVF", Types.MOVF, Outlines.Move, Shapes.OutputRect,
                Properties.Resources.Move_Float,
                "将浮点IN移动到双字OUT中，不改变IN的值。",
                "Move the 32-bit float IN to OUT, keep the value of IN",
                vformats);
            Formats[(int)Types.EXP] = new LadderUnitFormat(709, "EXP", Types.EXP, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.EXP_Operation,
                "求出浮点数IN的自然指数，产生的实数结果保存到OUT。",
                "Calculate the natural exponential of 32-bit float IN, and assign the result to OUT.",
                vformats);
            Formats[(int)Types.LN] = new LadderUnitFormat(708, "LN", Types.LN, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.LN_Operation,
                "求出浮点数IN的自然对数，产生的实数结果保存到OUT。",
                "Calculate the natural logarithm of 32-bit float IN, and assign the result to OUT.",
                vformats);
            Formats[(int)Types.LOG] = new LadderUnitFormat(1800, "LOG", Types.LOG, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.LOG_Inst,
                "对寄存器（IN）的浮点数值进行以10为底的对数运算，将浮点数结果传送到寄存器（OUT）。",
                "Calculate the denary logarithm of 32-bit float IN, and assign the result to OUT.",
                vformats);
            Formats[(int)Types.SQRT] = new LadderUnitFormat(704, "SQRT", Types.SQRT, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Sqrt_Operation,
                "将浮点数IN取平方根，产生的实数结果保存到OUT。",
                "Calculate the square root of 32-bit float IN, and assign the result to OUT.",
                vformats);
            Formats[(int)Types.SIN] = new LadderUnitFormat(705, "SIN", Types.SIN, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Sin_Operation,
                "将作为角度的浮点数IN进行正弦运算，产生的实数结果保存到OUT。",
                "Calculate the sine of 32-bit float IN, and assign the result to OUT",
                vformats);
            Formats[(int)Types.COS] = new LadderUnitFormat(706, "COS", Types.COS, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Cos_Operation,
                "将作为角度的浮点数IN进行正弦运算，产生的实数结果保存到OUT。",
                "Calculate the cosine of 32-bit float IN, and assign the result to OUT",
                vformats);
            Formats[(int)Types.TAN] = new LadderUnitFormat(707, "TAN", Types.TAN, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Tan_Operation,
                "将作为角度的浮点数IN进行正弦运算，产生的实数结果保存到OUT。",
                "Calculate the cosine of 32-bit float IN, and assign the result to OUT",
                vformats);
            Formats[(int)Types.MVBLK] = new LadderUnitFormat(603, "MVBLK", Types.MVBLK, Outlines.Move, Shapes.OutputRect,
                Properties.Resources.Move_Blocks_Word,
                "将（长度）个单字从IN开始的一段连续的寄存器移动到从OUT开始的一段寄存器中，长度的范围是（1 - 1024）",
                "Move a range of 16-bit word register started from IN, to another range started from OUT, the maximum length is 1024.",
                new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.BitWordRegex}),
                    new ValueFormat("D", ValueModel.Types.WORD, false, true, 1, new Regex[] { ValueModel.VerifyWordRegex2, ValueModel.BitWordRegex}),
                    new ValueFormat("N", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) });
            Formats[(int)Types.MVDBLK] = new LadderUnitFormat(604, "MVDBLK", Types.MVDBLK, Outlines.Move, Shapes.OutputRect,
                Properties.Resources.Move_Blocks_DWord,
                "将（长度）个双字从IN开始的一段连续的寄存器移动到从OUT开始的一段寄存器中，长度的范围是（1-1024）",
                "Move a range of 32-bit double word register started from IN, to another range started from OUT, the maximum length is 1024.",
                new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("D", ValueModel.Types.DWORD, false, true, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("N", ValueModel.Types.DWORD, true, false, 2, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitDoubleWordRegex })});
            vformats = new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.FLOAT, true, false, 0, new Regex[] {ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}) };
            Formats[(int)Types.TRUNC] = new LadderUnitFormat(406, "TRUNC", Types.TRUNC, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.Trunc,
                "将浮点的IN截位取整转为双字的OUT。",
                "Truncation rounding the 32-bit float IN and assign to OUT.",
                vformats);
            Formats[(int)Types.ROUND] = new LadderUnitFormat(405, "ROUND", Types.ROUND, Outlines.Convert, Shapes.OutputRect,
                Properties.Resources.Round,
                "将浮点的IN四舍五入转为双字的OUT。",
                "Round-up the 32-bit float IN and assign to OUT.",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex2, ValueModel.BitWordRegex}) };
            Formats[(int)Types.ANDW] = new LadderUnitFormat(502, "ANDW", Types.ANDW, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.Word_And,
                "对单字IN1和IN2执行与操作，并将结果载入输出到OUT。",
                "对单字IN1和IN2执行与操作，并将结果载入输出到OUT。",
                vformats);
            Formats[(int)Types.ORW] = new LadderUnitFormat(504, "ORW", Types.ORW, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.Word_Or,
                "对单字IN1和IN2执行或操作，并将结果载入输出到OUT。",
                "对单字IN1和IN2执行或操作，并将结果载入输出到OUT。",
                vformats);
            Formats[(int)Types.XORW] = new LadderUnitFormat(506, "XORW", Types.XORW, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.Word_XOR,
                "对单字IN1和IN2执行异或操作，并将结果载入输出到OUT。",
                "对单字IN1和IN2执行异或操作，并将结果载入输出到OUT。",
                vformats);
            Formats[(int)Types.ADD] = new LadderUnitFormat(800, "ADD", Types.ADD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Word_And,
                "将单字IN1和单字IN2相加（IN1+IN2），产生的整数结果保存到OUT。",
                "将单字IN1和单字IN2相加（IN1+IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.SUB] = new LadderUnitFormat(802, "SUB", Types.SUB, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Word_Minus,
                "将单字IN1和单字IN2相减（IN1-IN2），产生的整数结果保存到OUT。",
                "将单字IN1和单字IN2相减（IN1-IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.MULW] = new LadderUnitFormat(805, "MULW", Types.MULW, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Word_Multiply,
                "将单字IN1和单字IN2相乘（IN1*IN2），产生的整数结果保存到OUT。",
                "将单字IN1和单字IN2相乘（IN1*IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.DIVW] = new LadderUnitFormat(808, "DIVW", Types.DIVW, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Word_Divide,
                "将单字IN1和单字IN2相除（IN1/IN2），产生的整数结果保存到OUT。",
                "将单字IN1和单字IN2相除（IN1/IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.SHL] = new LadderUnitFormat(1200, "SHL", Types.SHL, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.SHL_Inst,
                "指令将单字（IN1）数值向左移动（IN2）位，并将结果载入单字（OUT）。移位指令对每个移出位补0。",
                "指令将单字（IN1）数值向左移动（IN2）位，并将结果载入单字（OUT）。移位指令对每个移出位补0。",
                vformats);
            Formats[(int)Types.SHR] = new LadderUnitFormat(1203, "SHR", Types.SHR, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.SHR_Inst,
                "指令将单字（IN1）数值向右移动（IN2）位，并将结果载入单字（OUT）。移位指令对每个移出位补0。",
                "指令将单字（IN1）数值向右移动（IN2）位，并将结果载入单字（OUT）。移位指令对每个移出位补0。",
                vformats);
            Formats[(int)Types.ROL] = new LadderUnitFormat(1206, "ROL", Types.ROL, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.ROL_Inst,
                "指令将单字（IN1）数值向左移动（IN2）位，并将结果载入单字（OUT）。移出位按原次序补到最右侧。",
                "指令将单字（IN1）数值向左移动（IN2）位，并将结果载入单字（OUT）。移出位按原次序补到最右侧。",
                vformats);
            Formats[(int)Types.ROR] = new LadderUnitFormat(1208, "ROR", Types.ROR, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.RORD_Inst,
                "指令将单字（IN1）数值向右移动（IN2）位，并将结果载入单字（OUT）。移出位按原次序补到最左侧。",
                "指令将单字（IN1）数值向右移动（IN2）位，并将结果载入单字（OUT）。移出位按原次序补到最左侧。",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}) };
            Formats[(int)Types.ANDD] = new LadderUnitFormat(503, "ANDD", Types.ANDD, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.DWord_And,
                "对双字IN1和IN2执行与操作，并将结果载入输出到OUT。",
                "对双字IN1和IN2执行与操作，并将结果载入输出到OUT。",
                vformats);
            Formats[(int)Types.ORD] = new LadderUnitFormat(505, "ORD", Types.ORD, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.DWord_Or,
                "对双字IN1和IN2执行或操作，并将结果载入输出到OUT。",
                "对双字IN1和IN2执行或操作，并将结果载入输出到OUT。",
                vformats);
            Formats[(int)Types.XORD] = new LadderUnitFormat(507, "XORD", Types.XORD, Outlines.LogicOperation, Shapes.OutputRect,
                Properties.Resources.DWord_XOR,
                "对双字IN1和IN2执行异或操作，并将结果载入输出到OUT。",
                "对双字IN1和IN2执行异或操作，并将结果载入输出到OUT。",
                vformats);
            Formats[(int)Types.ADDD] = new LadderUnitFormat(801, "ADDD", Types.ADDD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.DWord_Add,
                "将双字IN1和双字IN2相加（IN1+IN2），产生的整数结果保存到OUT。",
                "将双字IN1和双字IN2相加（IN1+IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.SUBD] = new LadderUnitFormat(803, "SUBD", Types.SUBD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.DWord_Minus,
                "将双字IN1和双字IN2相减（IN1-IN2），产生的整数结果保存到OUT。",
                "将双字IN1和双字IN2相减（IN1-IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.MULD] = new LadderUnitFormat(806, "MULD", Types.MULD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.DWord_Multiply,
                "将双字IN1和双字IN2相减（IN1*IN2），产生的整数结果保存到OUT。",
                "将双字IN1和双字IN2相减（IN1*IN2），产生的整数结果保存到OUT。",
                vformats);
            Formats[(int)Types.DIVD] = new LadderUnitFormat(808, "DIVD", Types.DIVD, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.DWord_Divide,
                "将双字IN1和双字IN2相除（IN1/IN2），产生的双字商保存到双字OUT中。",
                "将双字IN1和双字IN2相除（IN1/IN2），产生的双字商保存到双字OUT中。",
                vformats);
            Formats[(int)Types.SHLD] = new LadderUnitFormat(1201, "SHLD", Types.SHLD, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.SHLD_Inst,
                "指令将双字（IN1）数值向左移动（IN2）位，并将结果载入双字（OUT）。移位指令对每个移出位补0。",
                "指令将双字（IN1）数值向左移动（IN2）位，并将结果载入双字（OUT）。移位指令对每个移出位补0。",
                vformats);
            Formats[(int)Types.SHRD] = new LadderUnitFormat(1204, "SHRD", Types.SHRD, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.SHLD_Inst,
                "指令将双字（IN1）数值向右移动（IN2）位，并将结果载入双字（OUT）。移位指令对每个移出位补0。",
                "指令将双字（IN1）数值向右移动（IN2）位，并将结果载入双字（OUT）。移位指令对每个移出位补0。",
                vformats);
            Formats[(int)Types.ROLD] = new LadderUnitFormat(1207, "ROLD", Types.ROLD, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.ROLD_Inst,
                "指令将单字（IN1）数值向左移动（IN2）位，并将结果载入单字（OUT）。移出位按原次序补到最右侧。",
                "指令将单字（IN1）数值向左移动（IN2）位，并将结果载入单字（OUT）。移出位按原次序补到最右侧。",
                vformats);
            Formats[(int)Types.RORD] = new LadderUnitFormat(1209, "RORD", Types.RORD, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.RORD_Inst,
                "指令将双字（IN1）数值向左移动（IN2）位，并将结果载入双字（OUT）。移出位按原次序补到最左侧。",
                "指令将双字（IN1）数值向左移动（IN2）位，并将结果载入双字（OUT）。移出位按原次序补到最左侧。",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.FLOAT, false, true, -1, new Regex[] { ValueModel.VerifyFloatRegex}) };
            Formats[(int)Types.ADDF] = new LadderUnitFormat(700, "ADDF", Types.ADDF, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Float_Add,
                "将浮点数IN1和浮点数IN2相加（IN1+IN2），产生的实数结果保存到OUT。",
                "将浮点数IN1和浮点数IN2相加（IN1+IN2），产生的实数结果保存到OUT。",
                vformats);
            Formats[(int)Types.SUBF] = new LadderUnitFormat(701, "SUBF", Types.SUBF, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Float_Minus,
                "将浮点数IN1和浮点数IN2相减（IN1-IN2），产生的实数结果保存到OUT。",
                "将浮点数IN1和浮点数IN2相减（IN1-IN2），产生的实数结果保存到OUT。",
                vformats);
            Formats[(int)Types.MULF] = new LadderUnitFormat(702, "MULF", Types.MULF, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Float_Multiply,
                "将浮点数IN1和浮点数IN2相乘（IN1*IN2），产生的实数结果保存到OUT。",
                "将浮点数IN1和浮点数IN2相乘（IN1*IN2），产生的实数结果保存到OUT。",
                vformats);
            Formats[(int)Types.DIVF] = new LadderUnitFormat(703, "DIVF", Types.DIVF, Outlines.FloatCalculation, Shapes.OutputRect,
                Properties.Resources.Float_Divide,
                "将浮点数IN1和浮点数IN2相除（IN1/IN2），产生的实数结果保存到OUT。",
                "将浮点数IN1和浮点数IN2相除（IN1/IN2），产生的实数结果保存到OUT。",
                vformats);
            Formats[(int)Types.POW] = new LadderUnitFormat(1801, "POW", Types.POW, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.POW_Inst,
                "以IN1的浮点数值为底，IN2的浮点数值为指数进行幂运算，将结果传送到OUT。",
                "以IN1的浮点数值为底，IN2的浮点数值为指数进行幂运算，将结果传送到OUT。",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.BitDoubleWordRegex}) };
            Formats[(int)Types.MUL] = new LadderUnitFormat(804, "MUL", Types.MUL, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Multiply,
                "将单字IN1和单字IN2相乘（IN1*IN2），产生的双字结果保存到双字OUT。",
                "将单字IN1和单字IN2相乘（IN1*IN2），产生的双字结果保存到双字OUT。",
                vformats);
            Formats[(int)Types.DIV] = new LadderUnitFormat(807, "DIV", Types.DIV, Outlines.IntegerCalculation, Shapes.OutputRect,
                Properties.Resources.Divide,
                "将单字IN1和单字IN2相除（IN1/IN2），产生商数和余数保存到双字OUT中。低位存商，高位存余。",
                "将单字IN1和单字IN2相除（IN1/IN2），产生商数和余数保存到双字OUT中。低位存商，高位存余。",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("T", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex4}),
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) };
            Formats[(int)Types.TON] = new LadderUnitFormat(900, "TON", Types.TON, Outlines.Timer, Shapes.OutputRect,
                Properties.Resources.TON_Inst,
                "TON指令在启用输入端为（闭合）时，开始计时。\r\n" +
                "当前值（TVx）大于或等于预设时间（SV）时，定时器位（Tx）为（闭合），且定时器停止计时；\r\n" +
                "启用输入端为（断开）时，接通延时定时器自动复位，即定时器位（Tx）为OFF，当前值（TVx）为0；\r\n",
                "TON指令在启用输入端为（闭合）时，开始计时。\r\n" +
                "当前值（TVx）大于或等于预设时间（SV）时，定时器位（Tx）为（闭合），且定时器停止计时；\r\n" +
                "启用输入端为（断开）时，接通延时定时器自动复位，即定时器位（Tx）为OFF，当前值（TVx）为0；\r\n",
                vformats);
            Formats[(int)Types.TOF] = new LadderUnitFormat(901, "TOF", Types.TOF, Outlines.Timer, Shapes.OutputRect,
                Properties.Resources.TOF_Inst, 
                "用于在输入关闭后，延迟固定的一段时间再关闭输出。\r\n" + 
                "启用输入打开时，定时器位立即打开，当前值被设为0。输入关闭时，定时器继续计时，直到消逝的时间达到预设时间。\r\n" +
                "达到预设值后，定时器位关闭，当前值停止计时。如果输入关闭的时间短于预设数值，则定时器位仍保持在打开状态。\r\n" +
                "TOF指令必须遇到从（闭合）至（断开）的转换才开始计时。\r\n",
                "用于在输入关闭后，延迟固定的一段时间再关闭输出。\r\n" +
                "启用输入打开时，定时器位立即打开，当前值被设为0。输入关闭时，定时器继续计时，直到消逝的时间达到预设时间。\r\n" +
                "达到预设值后，定时器位关闭，当前值停止计时。如果输入关闭的时间短于预设数值，则定时器位仍保持在打开状态。\r\n" +
                "TOF指令必须遇到从（闭合）至（断开）的转换才开始计时。\r\n",
                vformats);
            Formats[(int)Types.TONR] = new LadderUnitFormat(902, "TONR", Types.TONR, Outlines.Timer, Shapes.OutputRect,
                Properties.Resources.TONR_Inst,
                "指令在启用输入为（闭合）时，开始计时。当前值（TVxxx）大于或等于预设时间（SV）时，计时位（Txxx）为（闭合）。\r\n" +
                "当输入为（断开）时，保持保留性延迟定时器当前值。您可使用保留性接通延时定时器为多个输入（闭合）阶段累计时间。\r\n",
                "指令在启用输入为（闭合）时，开始计时。当前值（TVxxx）大于或等于预设时间（SV）时，计时位（Txxx）为（闭合）。\r\n" +
                "当输入为（断开）时，保持保留性延迟定时器当前值。您可使用保留性接通延时定时器为多个输入（闭合）阶段累计时间。\r\n",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("C", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex3}),
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) };
            Formats[(int)Types.CTU] = new LadderUnitFormat(1000, "CTU", Types.CTU, Outlines.Counter, Shapes.OutputRect,
                Properties.Resources.CTU_Inst,
                "每次向上计数输入能流从关闭向打开转换时，向上计数（CTU）指令从当前值向上计数。\r\n" +
                "当前值（CVxxx）大于或等于预设值（SV）时，计数器位（Cxxx）打开。\r\n" +
                "复原（RST）输入打开或执行复原指令时，计数器被复原。\r\n" +
                "达到最大值（32767或2147483647）时，计数器停止计数。\r\n" +
                "CV0-CV199为单字计数器(CV)，CV200-CV234为双字计数器(CV32)，CV235-CV255为双字高速计数器。\r\n",
                "每次向上计数输入能流从关闭向打开转换时，向上计数（CTU）指令从当前值向上计数。\r\n" +
                "当前值（CVxxx）大于或等于预设值（SV）时，计数器位（Cxxx）打开。\r\n" +
                "复原（RST）输入打开或执行复原指令时，计数器被复原。\r\n" +
                "达到最大值（32767或2147483647）时，计数器停止计数。\r\n" +
                "CV0-CV199为单字计数器(CV)，CV200-CV234为双字计数器(CV32)，CV235-CV255为双字高速计数器。\r\n",
                vformats);
            Formats[(int)Types.CTD] = new LadderUnitFormat(1001, "CTD", Types.CTD, Outlines.Counter, Shapes.OutputRect,
                Properties.Resources.CTD_Inst,
                "每次向下计数输入能流从关闭向打开转换时，向下计数（CTD）指令从当前值向下计数。\r\n" +
                "当前值CVxxx等于0时，计数器位（Cxxx）打开。\r\n" +
                "载入输入（LD）打开时，计数器复原计数器位（Cxxx）并用预设值（SV）载入当前值。\r\n" +
                "达到零时，计数器位Cxxx打开。向下计数器继续向下计数到最小值（16位-32768，,32位-2147483648），停止计数。\r\n" +
                "CV0-CV199为单字计数器(CV)，CV200-CV234为双字计数器(CV32)，CV235-CV255为双字高速计数器。\r\n",
                "每次向下计数输入能流从关闭向打开转换时，向下计数（CTD）指令从当前值向下计数。\r\n" +
                "当前值CVxxx等于0时，计数器位（Cxxx）打开。\r\n" +
                "载入输入（LD）打开时，计数器复原计数器位（Cxxx）并用预设值（SV）载入当前值。\r\n" +
                "达到零时，计数器位Cxxx打开。向下计数器继续向下计数到最小值（16位-32768，,32位-2147483648），停止计数。\r\n" +
                "CV0-CV199为单字计数器(CV)，CV200-CV234为双字计数器(CV32)，CV235-CV255为双字高速计数器。\r\n",
                vformats);
            Formats[(int)Types.CTUD] = new LadderUnitFormat(1002, "CTUD", Types.CTUD, Outlines.Counter, Shapes.OutputRect,
                Properties.Resources.CTUD_Inst,
                "每次计数输入能流从关闭向打开转换时，计数器（CTUD）指令判断当前值与设定值的大小。\r\n" +
                "当前值（CVxxx）大于（小于）预设值（SV）时，计数器做向下（向上）计数，当前值（CVxxx）等于预设值（SV）时，计数器（Cxxx）置为1，当前值（CVxxx）不动作。\r\n" +
                "复原（RST）输入打开或执行复原指令时，计数器被复原（复原计数器（Cxxx）时，建议使用RST指令进行）。\r\n" +
                "CV0-CV199为单字计数器(CV)，CV200-CV234为双字计数器(CV32)，CV235-CV255为双字高速计数器。\r\n",
                "每次计数输入能流从关闭向打开转换时，计数器（CTUD）指令判断当前值与设定值的大小。\r\n" +
                "当前值（CVxxx）大于（小于）预设值（SV）时，计数器做向下（向上）计数，当前值（CVxxx）等于预设值（SV）时，计数器（Cxxx）置为1，当前值（CVxxx）不动作。\r\n" +
                "复原（RST）输入打开或执行复原指令时，计数器被复原（复原计数器（Cxxx）时，建议使用RST指令进行）。\r\n" +
                "CV0-CV199为单字计数器(CV)，CV200-CV234为双字计数器(CV32)，CV235-CV255为双字高速计数器。\r\n",
                vformats);
            Formats[(int)Types.HCNT] = new LadderUnitFormat(1700, "HCNT", Types.HCNT, Outlines.HighCount, Shapes.OutputRect,
                Properties.Resources.Inst_HighCount,
                "每次输入能流(X0-X7)从关闭向打开转换时，高速计数（HCNT）指令从当前值计数。\r\n" +
                "当前值（CVxxx）到达预设值（SV）时，计数器位（Cxxx）打开。\r\n" +
                "不同的高速计数器(CV235-CV255)对不同的输入能流(X0-X7)有不同的计数策略（向上，向下，保留），具体请查阅帮助文档。\r\n",
                "每次输入能流(X0-X7)从关闭向打开转换时，高速计数（HCNT）指令从当前值计数。\r\n" +
                "当前值（CVxxx）到达预设值（SV）时，计数器位（Cxxx）打开。\r\n" +
                "不同的高速计数器(CV235-CV255)对不同的输入能流(X0-X7)有不同的计数策略（向上，向下，保留），具体请查阅帮助文档。\r\n",
                vformats);
            Formats[(int)Types.FOR] = new LadderUnitFormat(1100, "FOR", Types.FOR, Outlines.ProgramControl, Shapes.OutputRect,
                Properties.Resources.FOR_Inst,
                "如果指令前条件导通，则执行此条指令。此条指令必须和NEXT指令配合使用，FOR和NEXT指令允许了一个程序的区域指定。\r\n" +
                "在这个区域内，包括了一系列指令，它们将会被重复执行（循环次数）次。\r\n",
                "如果指令前条件导通，则执行此条指令。此条指令必须和NEXT指令配合使用，FOR和NEXT指令允许了一个程序的区域指定。\r\n" +
                "在这个区域内，包括了一系列指令，它们将会被重复执行（循环次数）次。\r\n",
                new ValueFormat[] {
                    new ValueFormat("CNT", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) });
            vformats = new ValueFormat[] {
                    new ValueFormat("LBL", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}) };
            Formats[(int)Types.JMP] = new LadderUnitFormat(1102, "JMP", Types.JMP, Outlines.ProgramControl, Shapes.OutputRect,
                Properties.Resources.JMP_Inst,
                "堆栈顶为1时，跳接至标签（JMP）指令对程序中的指定标签（LBL）执行分支操作。",
                "堆栈顶为1时，跳接至标签（JMP）指令对程序中的指定标签（LBL）执行分支操作。",
                vformats);
            Formats[(int)Types.LBL] = new LadderUnitFormat(1103, "LBL", Types.LBL, Outlines.ProgramControl, Shapes.OutputRect,
                Properties.Resources.LBL_Inst,
                "与JMP指令配合使用，指示跳转位置。",
                "与JMP指令配合使用，指示跳转位置。",
                vformats);
            Formats[(int)Types.CALL] = new LadderUnitFormat(1104, "CALL", Types.CALL, Outlines.ProgramControl, Shapes.OutputRect,
                Properties.Resources.CALL_Inst,
                "当CALL指令有效时，通过被调用的指针，它将强迫程序运行与子程序ID号相同的子程序，执行完被调用子程序，会自动返回到当前CALL指令的下一条指令继续执行；\r\n" +
                "当CALL指令无效时，程序会自动跳过此条CALL指令，执行下一条指令。\r\n",
                "当CALL指令有效时，通过被调用的指针，它将强迫程序运行与子程序ID号相同的子程序，执行完被调用子程序，会自动返回到当前CALL指令的下一条指令继续执行；\r\n" +
                "当CALL指令无效时，程序会自动跳过此条CALL指令，执行下一条指令。\r\n",
                new ValueFormat[] {
                    new ValueFormat("FUNC", ValueModel.Types.STRING, true, false, 0, new Regex[] { ValueModel.AnyNameRegex }) });
            Formats[(int)Types.CALLM] = new LadderUnitFormat(1105, "CALLM", Types.CALLM, Outlines.ProgramControl, Shapes.OutputRect,
                Properties.Resources.CALLM_Inst,
                "堆栈顶为1时，会调用用户在函数功能块内实现的函数。参数最多可以设定四个，在C函数中以基地址指针的形式表示。",
                "堆栈顶为1时，会调用用户在函数功能块内实现的函数。参数最多可以设定四个，在C函数中以基地址指针的形式表示。",
                new ValueFormat[] {
                    new ValueFormat("SUB", ValueModel.Types.STRING, true, false, 0, new Regex[] { ValueModel.FuncNameRegex }) });
            Formats[(int)Types.STL] = new LadderUnitFormat(1106, "STL", Types.STL, Outlines.ProgramControl, Shapes.OutputRect,
                "",
                "STL与STLE必须一一配对使用，STL表示一个状态的开始，STLE表示一个状态的结束。",
                "STL与STLE必须一一配对使用，STL表示一个状态的开始，STLE表示一个状态的结束。",
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.BOOL, true, false, -1, new Regex[] {ValueModel.VerifyBitRegex6 }) });
            Formats[(int)Types.STLE] = new LadderUnitFormat(1107, "STLE", Types.STLE, Outlines.ProgramControl, Shapes.OutputRect,
                "",
                "STL与STLE必须一一配对使用，STL表示一个状态的开始，STLE表示一个状态的结束。",
                "STL与STLE必须一一配对使用，STL表示一个状态的开始，STLE表示一个状态的结束。",
                new ValueFormat[] { });
            Formats[(int)Types.ST] = new LadderUnitFormat(1108, "ST", Types.ST, Outlines.ProgramControl, Shapes.OutputRect,
                "",
                "当使用ST指令当状态转移条件成立时，下一个待转移状态被置为ON，但当前STL段中的状态不会被复位OFF，ST指令一般在程序需要同时运行多个状态程序时使用；",
                "当使用ST指令当状态转移条件成立时，下一个待转移状态被置为ON，但当前STL段中的状态不会被复位OFF，ST指令一般在程序需要同时运行多个状态程序时使用；",
                new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.BOOL, true, false, -1, new Regex[] {ValueModel.VerifyBitRegex6 }) });
            vformats = new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.BOOL, true, false, 0, new Regex[] { ValueModel.VerifyBitRegex1, ValueModel.WordBitRegex}),
                    new ValueFormat("D", ValueModel.Types.BOOL, false, true, 1, new Regex[] { ValueModel.VerifyBitRegex2, ValueModel.WordBitRegex}),
                    new ValueFormat("N1", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("N2", ValueModel.Types.WORD, true, false, 3, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}), };
            Formats[(int)Types.SHLB] = new LadderUnitFormat(1202, "SHLB", Types.SHLB, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.SHLB_Inst,
                "指令将起始地址为（S）长度为（IN4）的源位序列依次从起始地址为（D）长度为（IN3）的目的位序列的起始地址移入，目的位序列的值依次向后转移，直至移出。",
                "指令将起始地址为（S）长度为（IN4）的源位序列依次从起始地址为（D）长度为（IN3）的目的位序列的起始地址移入，目的位序列的值依次向后转移，直至移出。",
                vformats);
            Formats[(int)Types.SHRB] = new LadderUnitFormat(1205, "SHRB", Types.SHRB, Outlines.Shift, Shapes.OutputRect,
                Properties.Resources.SHRB_Inst,
                "指令将起始地址为（S）长度为（IN4）的源位序列依次从起始地址为（D）长度为（IN3）的目的位序列的尾部移入，目的位序列的值依次向前转移，直至移出。",
                "指令将起始地址为（S）长度为（IN4）的源位序列依次从起始地址为（D）长度为（IN3）的目的位序列的尾部移入，目的位序列的值依次向前转移，直至移出。",
                vformats);
            Formats[(int)Types.ATCH] = new LadderUnitFormat(1300, "ATCH", Types.ATCH, Outlines.Interrupt, Shapes.OutputRect,
                Properties.Resources.ATCH_Inst,
                "指令将中断事件（EVENT）与子程序号码（INT）相关联，并启用中断事件。",
                "指令将中断事件（EVENT）与子程序号码（INT）相关联，并启用中断事件。",
                new ValueFormat[] {
                    new ValueFormat("INT", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}),
                    new ValueFormat("EVENT", ValueModel.Types.STRING, true, false, 1, new Regex[] {ValueModel.AnyNameRegex}) });
            Formats[(int)Types.DTCH] = new LadderUnitFormat(1301, "DTCH", Types.DTCH, Outlines.Interrupt, Shapes.OutputRect,
                Properties.Resources.DTCH_Inst,
                "指令将解除中断事件（EVENT）与中断子程序号码（INT）之间的关联，并禁用中断事件。",
                "指令将解除中断事件（EVENT）与中断子程序号码（INT）之间的关联，并禁用中断事件。",
                new ValueFormat[] {
                    new ValueFormat("INT", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex,  }) });
            Formats[(int)Types.TRD] = new LadderUnitFormat(1400, "TRD", Types.TRD, Outlines.RealTime, Shapes.OutputRect,
                Properties.Resources.TRD_Inst,
                "指令从硬件时钟读取当前时间和日期，并将其载入以IN1起始的8个连续D的时间缓冲区。其中，年份用2字节表示。",
                "指令从硬件时钟读取当前时间和日期，并将其载入以IN1起始的8个连续D的时间缓冲区。其中，年份用2字节表示。",
                new ValueFormat[] {
                    new ValueFormat("T", ValueModel.Types.WORD, false, true, 0, new Regex[] {ValueModel.VerifyWordRegex3}) });
            Formats[(int)Types.TWR] = new LadderUnitFormat(1401, "TWR", Types.TWR, Outlines.RealTime, Shapes.OutputRect,
                Properties.Resources.TWR_Inst,
                "指令将当前时间和日期写入IN1指定的在8个D的时间缓冲区开始的硬件时钟。其中，年份用2字节表示。",
                "指令将当前时间和日期写入IN1指定的在8个D的时间缓冲区开始的硬件时钟。其中，年份用2字节表示。",
                new ValueFormat[] {
                    new ValueFormat("T", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}) });
            Formats[(int)Types.MBUS] = new LadderUnitFormat(1500, "MBUS", Types.MBUS, Outlines.Communication, Shapes.OutputRect,
                Properties.Resources.MBUS_Inst,
                "调用Modbus主站通信指令。\r\n" +
                "COM_ID ：通信口选择，可选择COM0（232）或COM1（485）；\r\n" +
                "Table  ：Modbus主站表格ID，可在工程管理窗口中添加所需的Modbus主站表格；\r\n" +
                "WR     ：Dn、Dn+1，其中Dn用来存储通信信息代码(见帮助文档)，Dn+1用来存储当前主站表格的Modbus命令编号。\r\n",
                "调用Modbus主站通信指令。\r\n" +
                "COM_ID ：通信口选择，可选择COM0（232）或COM1（485）；\r\n" +
                "Table  ：Modbus主站表格ID，可在工程管理窗口中添加所需的Modbus主站表格；\r\n" +
                "WR     ：Dn、Dn+1，其中Dn用来存储通信信息代码(见帮助文档)，Dn+1用来存储当前主站表格的Modbus命令编号。\r\n",
                new ValueFormat[] {
                    new ValueFormat("PORT", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}),
                    new ValueFormat("TABLE", ValueModel.Types.STRING, true, false, 1, new Regex[] {ValueModel.AnyNameRegex}),
                    new ValueFormat("WR", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3}) });
            Formats[(int)Types.SEND] = new LadderUnitFormat(1501, "SEND", Types.SEND, Outlines.Communication, Shapes.OutputRect,
                Properties.Resources.SEND_Inst,
                "调用自由口通讯发送指令。\r\n" +
                "COM_ID（通信口选择）: 可选择COM0（232）或COM1（485）；\r\n" +
                "Addr（起始地址）    ：该参数地址是以Dn为起始地址的一片区域，该操作数仅能选择D数据寄存器；\r\n" +
                "Len（数据长度）     ：设置待发送数据的长度，该操作数可选择K/H/D，对于不同型号PLC均有其相应的发送最大限制；\r\n",
                "调用自由口通讯发送指令。\r\n" +
                "COM_ID（通信口选择）: 可选择COM0（232）或COM1（485）；\r\n" +
                "Addr（起始地址）    ：该参数地址是以Dn为起始地址的一片区域，该操作数仅能选择D数据寄存器；\r\n" +
                "Len（数据长度）     ：设置待发送数据的长度，该操作数可选择K/H/D，对于不同型号PLC均有其相应的发送最大限制；\r\n",
                new ValueFormat[] {
                    new ValueFormat("COM", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}),
                    new ValueFormat("ADDR", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("LEN", ValueModel.Types.WORD, true, false, 2, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }) });
            Formats[(int)Types.REV] = new LadderUnitFormat(1502, "REV", Types.REV, Outlines.Communication, Shapes.OutputRect,
                Properties.Resources.REV_Inst,
                "调用自由口通讯接收指令。\r\n" +
                "COM_ID（通信口选择）: 可选择COM0（232）或COM1（485）；\r\n" +
                "Addr（起始地址）    ：将接收到的数据存放在以Dn为起始地址的一段区域，该操作数仅能选择D数据寄存器；\r\n" +
                "Len（接收长度）     ：该参数无需设置，在数据接收过程中会自动返回实际接收到的数据长度，并存放在所设的D寄存器中，对于不同型号PLC均有其相应的接收最大限制；\r\n",
                "调用自由口通讯接收指令。\r\n" +
                "COM_ID（通信口选择）: 可选择COM0（232）或COM1（485）；\r\n" +
                "Addr（起始地址）    ：将接收到的数据存放在以Dn为起始地址的一段区域，该操作数仅能选择D数据寄存器；\r\n" +
                "Len（接收长度）     ：该参数无需设置，在数据接收过程中会自动返回实际接收到的数据长度，并存放在所设的D寄存器中，对于不同型号PLC均有其相应的接收最大限制；\r\n",
                new ValueFormat[] {
                    new ValueFormat("COM", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}),
                    new ValueFormat("ADDR", ValueModel.Types.WORD, false, true, 1, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("LEN", ValueModel.Types.WORD, true, false, 2, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }) });
            Formats[(int)Types.PLSF] = new LadderUnitFormat(1600, "PLSF", Types.PLSF, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PLSF_Inst,
                "当栈顶为1时，往输出位输出指定频率的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DPLSF] = new LadderUnitFormat(1601, "DPLSF", Types.DPLSF, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DPLSF_Inst,
                "当栈顶为1时，往输出位输出指定频率的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.PLSY] = new LadderUnitFormat(1604, "PLSY", Types.PLSY, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PLSY_Inst,
                "当栈顶为1时，往输出位输出指定频率和脉冲数的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率和脉冲数的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("P", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DRVI] = new LadderUnitFormat(1617, "DRVI", Types.DRVI, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DRVI_Inst,
                "当栈顶为1时，输出一段给定频率和脉冲数的脉冲信号。",
                "当栈顶为1时，输出一段给定频率和脉冲数的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("P", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DPLSY] = new LadderUnitFormat(1605, "DPLSY", Types.DPLSY, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DPLSY_Inst,
                "当栈顶为1时，往输出位输出指定频率和脉冲数的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率和脉冲数的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("P", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DDRVI] = new LadderUnitFormat(1618, "DDRVI", Types.DDRVI, Outlines.Pulse, Shapes.OutputRect,                
                Properties.Resources.DDRVI_Inst,
                "当栈顶为1时，输出一段给定频率和脉冲数的脉冲信号。",
                "当栈顶为1时，输出一段给定频率和脉冲数的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("P", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}) });
            vformats = new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("DC", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DPLSY] = new LadderUnitFormat(1605, "DPLSY", Types.DPLSY, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DPLSY_Inst,
                "当栈顶为1时，往输出位输出指定频率和脉冲数的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率和脉冲数的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("PN", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.PWM] = new LadderUnitFormat(1602, "PWM", Types.PWM, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PWM_Inst,
                "当栈顶为1时，往输出位输出指定频率和占空比的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率和占空比的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("DC", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DPWM] = new LadderUnitFormat(1603, "DPWM", Types.DPWM, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DPWM_Inst,
                "当栈顶为1时，往输出位输出指定频率和占空比的脉冲信号。",
                "当栈顶为1时，往输出位输出指定频率和占空比的脉冲信号。",
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("DC", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.PLSR] = new LadderUnitFormat(1606, "PLSR", Types.PLSR, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PLSR_Inst,
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D2n）和脉冲数（D2n+1)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D1表示第一段的频率，D2表示第二段的脉冲数，D3表示第二段的频率。以此类推。\r\n",
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D2n）和脉冲数（D2n+1)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D1表示第一段的频率，D2表示第二段的脉冲数，D3表示第二段的频率。以此类推。\r\n",
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("V", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DPLSR] = new LadderUnitFormat(1607, "DPLSR", Types.DPLSR, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DPLSR_Inst,
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D4n）和脉冲数（D4n+2)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D2表示第一段的频率，D4表示第二段的脉冲数，D6表示第二段的频率。以此类推。\r\n",
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D4n）和脉冲数（D4n+2)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D2表示第一段的频率，D4表示第二段的脉冲数，D6表示第二段的频率。以此类推。\r\n",
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("V", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.PLSRD] = new LadderUnitFormat(1608, "PLSRD", Types.PLSRD, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PLSRD_Inst,
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D2n）和脉冲数（D2n+1)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D1表示第一段的频率，D2表示第二段的脉冲数，D3表示第二段的频率。以此类推。\r\n",
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D2n）和脉冲数（D2n+1)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D1表示第一段的频率，D2表示第二段的脉冲数，D3表示第二段的频率。以此类推。\r\n",
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("V", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DPLSRD] = new LadderUnitFormat(1609, "DPLSRD", Types.DPLSRD, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DPLSRD_Inst,
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D4n）和脉冲数（D4n+2)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D2表示第一段的频率，D4表示第二段的脉冲数，D6表示第二段的频率。以此类推。\r\n",
                "当栈顶为1时，输出分段变频段间频率匀速渐变的脉冲信号。\r\n" +
                "段内按照给定频率（D4n）和脉冲数（D4n+2)产生一段脉冲，段之间的频率按给定时间（T)内直线变化。\r\n" +
                "若给定D0为基地址，则D0表示第一段的脉冲数，D2表示第一段的频率，D4表示第二段的脉冲数，D6表示第二段的频率。以此类推。\r\n",
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("V", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            vformats = new ValueFormat[] {
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PLSNEXT] = new LadderUnitFormat(1612, "PLSNEXT", Types.PLSNEXT, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PLSNEXT_Inst,
                "在脉冲输出到达当前段最高频率，并且PLSNEXT指向下一段非结束段，在此频率下稳定输出时，如果M0由OFF—>ON，则以加减速时间进入下一段的脉冲输出。",
                "在脉冲输出到达当前段最高频率，并且PLSNEXT指向下一段非结束段，在此频率下稳定输出时，如果M0由OFF—>ON，则以加减速时间进入下一段的脉冲输出。",
                vformats);
            Formats[(int)Types.PLSSTOP] = new LadderUnitFormat(1613, "PLSSTOP", Types.PLSSTOP, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PLSSTOP_Inst,
                "当导通条件由OFF—>ON，Y脉冲输出立即停止输出脉冲。",
                "当导通条件由OFF—>ON，Y脉冲输出立即停止输出脉冲。",
                vformats);
            Formats[(int)Types.ZRN] = new LadderUnitFormat(1614, "ZRN", Types.ZRN, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.ZRN_Inst,
                "脉冲先以爬行速度的速度前进，当近点信号置位的时候，立即从爬行速度以设定的时间减速到回归速度，当近点信号复位时，停止脉冲发送。",
                "脉冲先以爬行速度的速度前进，当近点信号置位的时候，立即从爬行速度以设定的时间减速到回归速度，当近点信号复位时，停止脉冲发送。",
                new ValueFormat[] {
                    new ValueFormat("DV", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("CV", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("SIG", ValueModel.Types.BOOL, true, false, 2, new Regex[] {ValueModel.VerifyBitRegex5}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.DZRN] = new LadderUnitFormat(1615, "DZRN", Types.DZRN, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.DZRN_Inst,
                "脉冲先以爬行速度的速度前进，当近点信号置位的时候，立即从爬行速度以设定的时间减速到回归速度，当近点信号复位时，停止脉冲发送。",
                "脉冲先以爬行速度的速度前进，当近点信号置位的时候，立即从爬行速度以设定的时间减速到回归速度，当近点信号复位时，停止脉冲发送。",
                new ValueFormat[] {
                    new ValueFormat("DV", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("CV", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("SIG", ValueModel.Types.BOOL, true, false, 2, new Regex[] {ValueModel.VerifyBitRegex5}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.PTO] = new LadderUnitFormat(1616, "PTO", Types.PTO, Outlines.Pulse, Shapes.OutputRect,
                Properties.Resources.PTO_Inst,
                "当栈顶为1时，输出分段变频段内频率匀速渐变的脉冲信号。\r\n" +
                "段内从起始频率D2n到终止频率D2n+1之间直线变化，段之间直接相接。\r\n" +
                "若给定D0为基地址，则D0表示第一段的起始频率，D1表示第一段的终止频率，D2表示第二段的起始频率，D3表示第二段的终止频率。以此类推。\r\n",
                "当栈顶为1时，输出分段变频段内频率匀速渐变的脉冲信号。\r\n" +
                "段内从起始频率D2n到终止频率D2n+1之间直线变化，段之间直接相接。\r\n" +
                "若给定D0为基地址，则D0表示第一段的起始频率，D1表示第一段的终止频率，D2表示第二段的起始频率，D3表示第二段的终止频率。以此类推。\r\n",
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}) });
            Formats[(int)Types.FACT] = new LadderUnitFormat(1802, "FACT", Types.FACT, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.FACT_Inst,
                "计算寄存器（IN）值的阶乘，N的最大值为12，将结果传送到寄存器（OUT）。",
                "计算寄存器（IN）值的阶乘，N的最大值为12，将结果传送到寄存器（OUT）。",
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex1}) });
            Formats[(int)Types.CMP] = new LadderUnitFormat(1803, "CMP", Types.CMP, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.CMP_Inst,
                "比较IN1和IN2的大小，比较结果发送到OUT。\r\n" +
                "若OUT为Y000，当IN1 > IN2时Y000为‘1’。当IN1 == IN2时Y001为‘1’。当IN1 < IN2时Y002为‘1’。\r\n",
                "比较IN1和IN2的大小，比较结果发送到OUT。\r\n" +
                "若OUT为Y000，当IN1 > IN2时Y000为‘1’。当IN1 == IN2时Y001为‘1’。当IN1 < IN2时Y002为‘1’。\r\n",
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) });
            Formats[(int)Types.CMPD] = new LadderUnitFormat(1804, "CMPD", Types.CMPD, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.CMPD_Inst,
                "比较IN1和IN2的大小，比较结果发送到OUT。\r\n" +
                "若OUT为Y000，当IN1 > IN2时Y000为‘1’。当IN1 == IN2时Y001为‘1’。当IN1 < IN2时Y002为‘1’。\r\n",
                "比较IN1和IN2的大小，比较结果发送到OUT。\r\n" +
                "若OUT为Y000，当IN1 > IN2时Y000为‘1’。当IN1 == IN2时Y001为‘1’。当IN1 < IN2时Y002为‘1’。\r\n",
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) });
            Formats[(int)Types.CMPF] = new LadderUnitFormat(1805, "CMPF", Types.CMPF, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.CMPF_Inst,
                "比较IN1和IN2的大小，比较结果发送到OUT。\r\n" +
                "若OUT为Y000，当IN1 > IN2时Y000为‘1’。当IN1 == IN2时Y001为‘1’。当IN1 < IN2时Y002为‘1’。\r\n",
                "比较IN1和IN2的大小，比较结果发送到OUT。\r\n" +
                "若OUT为Y000，当IN1 > IN2时Y000为‘1’。当IN1 == IN2时Y001为‘1’。当IN1 < IN2时Y002为‘1’。\r\n",
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) });
            Formats[(int)Types.ZCP] = new LadderUnitFormat(1806, "ZCP", Types.ZCP, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.ZCP_Inst,
                "比较值 IN 与下限 LOW 及上限 HIGH 作比较，其比较结果在 OUT 作表示。\r\n" +
                "若OUT为Y000，当IN < LOW时Y000为‘1’。当LOW <= IN <= HIGH 时Y001为‘1’。当HIGH < IN时Y002为‘1’。\r\n",
                "比较值 IN 与下限 LOW 及上限 HIGH 作比较，其比较结果在 OUT 作表示。\r\n" +
                "若OUT为Y000，当IN < LOW时Y000为‘1’。当LOW <= IN <= HIGH 时Y001为‘1’。当HIGH < IN时Y002为‘1’。\r\n",
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("IN3", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) });
            Formats[(int)Types.ZCPD] = new LadderUnitFormat(1807, "ZCPD", Types.ZCPD, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.ZCPD_Inst,
                "比较值 IN 与下限 LOW 及上限 HIGH 作比较，其比较结果在 OUT 作表示。\r\n" +
                "若OUT为Y000，当IN < LOW时Y000为‘1’。当LOW <= IN <= HIGH 时Y001为‘1’。当HIGH < IN时Y002为‘1’。\r\n",
                "比较值 IN 与下限 LOW 及上限 HIGH 作比较，其比较结果在 OUT 作表示。\r\n" +
                "若OUT为Y000，当IN < LOW时Y000为‘1’。当LOW <= IN <= HIGH 时Y001为‘1’。当HIGH < IN时Y002为‘1’。\r\n",
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("IN3", ValueModel.Types.DWORD, true, false, 2, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) });
            Formats[(int)Types.ZCPF] = new LadderUnitFormat(1808, "ZCPF", Types.ZCPF, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.ZCPF_Inst,
                "比较值 IN 与下限 LOW 及上限 HIGH 作比较，其比较结果在 OUT 作表示。\r\n" +
                "若OUT为Y000，当IN < LOW时Y000为‘1’。当LOW <= IN <= HIGH 时Y001为‘1’。当HIGH < IN时Y002为‘1’。\r\n",
                "比较值 IN 与下限 LOW 及上限 HIGH 作比较，其比较结果在 OUT 作表示。\r\n" +
                "若OUT为Y000，当IN < LOW时Y000为‘1’。当LOW <= IN <= HIGH 时Y001为‘1’。当HIGH < IN时Y002为‘1’。\r\n",
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN3", ValueModel.Types.FLOAT, true, false, 2, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3, ValueModel.WordBitRegex}) });
            Formats[(int)Types.XCH] = new LadderUnitFormat(1811, "XCH", Types.XCH, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.XCH_Inst,
                "交换IN1和IN2的值。",
                "交换IN1和IN2的值。",
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.BitWordRegex}),
                    new ValueFormat("R", ValueModel.Types.WORD, true, true, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.BitWordRegex}) });
            Formats[(int)Types.XCHD] = new LadderUnitFormat(1812, "XCHD", Types.XCHD, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.XCHD_Inst,
                "交换IN1和IN2的值。",
                "交换IN1和IN2的值。",
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.DWORD, true, true, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("R", ValueModel.Types.DWORD, true, true, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.BitDoubleWordRegex}) });
            Formats[(int)Types.XCHF] = new LadderUnitFormat(1813, "XCHF", Types.XCHF, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.XCHF_Inst,
                "交换IN1和IN2的值。",
                "交换IN1和IN2的值。",
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.FLOAT, true, true, 0, new Regex[] { ValueModel.VerifyFloatRegex}),
                    new ValueFormat("R", ValueModel.Types.FLOAT, true, true, 1, new Regex[] { ValueModel.VerifyFloatRegex}) });
            vformats = new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.BitWordRegex}) };
            Formats[(int)Types.NEG] = new LadderUnitFormat(1809, "NEG", Types.NEG, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.NEG_Inst,
                "对寄存器（IN）的值求补后传送到寄存器（OUT）。",
                "对寄存器（IN）的值求补后传送到寄存器（OUT）。",
                vformats);
            Formats[(int)Types.CML] = new LadderUnitFormat(1814, "CML", Types.CML, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.CML_Inst,
                "将源操作数（IN）中的数据逐位取反后传送到OUT寄存器。",
                "将源操作数（IN）中的数据逐位取反后传送到OUT寄存器。",
                vformats);
            vformats = new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitDoubleWordRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.BitDoubleWordRegex}) };
            Formats[(int)Types.NEGD] = new LadderUnitFormat(1810, "NEGD", Types.NEGD, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.NEGD_Inst,
                "对寄存器（IN）的值求补后传送到寄存器（OUT）。",
                "对寄存器（IN）的值求补后传送到寄存器（OUT）。",
                vformats);
            Formats[(int)Types.CMLD] = new LadderUnitFormat(1815, "CMLD", Types.CMLD, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.CMLD_Inst,
                "将源操作数（IN）中的数据逐位取反后传送到OUT寄存器。",
                "将源操作数（IN）中的数据逐位取反后传送到OUT寄存器。",
                vformats);
            Formats[(int)Types.SMOV] = new LadderUnitFormat(1816, "SMOV", Types.SMOV, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.SMOV_Inst,
                "指令将首先将二进制的源数据（D1）转换成  BCD  码，然后将BCD码移位传送，实现数据的分配、组合。\r\n" +
                "源数据 BCD码右起从第 4 位开始的 2 位移送到目标 D2的第 3 位和第 2 位，而 D2的第 4 和第 1 两位  BCD  码不变。\r\n" +
                "然后，目标D2 中的 BCD码自动转换成二进制数，即为  D2 的内容。BCD  码值超过9999 时出错。\r\n",
                "指令将首先将二进制的源数据（D1）转换成  BCD  码，然后将BCD码移位传送，实现数据的分配、组合。\r\n" +
                "源数据 BCD码右起从第 4 位开始的 2 位移送到目标 D2的第 3 位和第 2 位，而 D2的第 4 和第 1 两位  BCD  码不变。\r\n" +
                "然后，目标D2 中的 BCD码自动转换成二进制数，即为  D2 的内容。BCD  码值超过9999 时出错。\r\n",
                new ValueFormat[] {
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("SS", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}),
                    new ValueFormat("SC", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}),
                    new ValueFormat("DV", ValueModel.Types.WORD, false, true, -2, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("DS", ValueModel.Types.WORD, true, false, -1, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex}) });
            Formats[(int)Types.FMOV] = new LadderUnitFormat(1817, "FMOV", Types.FMOV, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.FMOV_Inst,
                "将源元件（SRC）中的数据传送到指定目标（DST）开始的 N  个目标元件中，这  N 个元件中的数据完全相同。",
                "将源元件（SRC）中的数据传送到指定目标（DST）开始的 N  个目标元件中，这  N 个元件中的数据完全相同。",
                new ValueFormat[] {
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("TV", ValueModel.Types.WORD, false, true, 1, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("CNT", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) });
            Formats[(int)Types.FMOVD] = new LadderUnitFormat(1818, "FMOVD", Types.FMOVD, Outlines.Auxiliar, Shapes.OutputRect,
                Properties.Resources.FMOVD_Inst,
                "将源元件（SRC）中的数据传送到指定目标（DST）开始的 N  个目标元件中，这  N 个元件中的数据完全相同。",
                "将源元件（SRC）中的数据传送到指定目标（DST）开始的 N  个目标元件中，这  N 个元件中的数据完全相同。",
                new ValueFormat[] {
                    new ValueFormat("SV", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("TV", ValueModel.Types.DWORD, false, true, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex1}),
                    new ValueFormat("CNT", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, ValueModel.BitWordRegex}) });
            LabelTypes = new Types[] { Types.LBL, Types.NEXT, Types.STL, Types.STLE };
            TypeOfNames = new Dictionary<string, Types>();
            for (int i = 0; i < Formats.Length; i++)
                TypeOfNames[Formats[i].Name] = (Types)i;
        }
        
        #endregion

        public LadderUnitModel(LadderNetworkModel _parent, Types _type)
        {
            type = _type;
            IList<ValueFormat> formats = Format.Formats;
            children = new ValueModel[formats.Count];
            for (int i = 0; i < children.Length; i++)
                children[i] = new ValueModel(this, formats[i]);
            InitializeStructure(_parent);
        }

        public LadderUnitModel(LadderNetworkModel _parent, FuncModel func)
        {
            type = Types.CALLM;
            children = new ValueModel[] { new ValueModel(this, Format.Formats[0])};
            children[0].Text = func.Name;
            children = children.Concat(func.GetValueModels(this)).ToArray();
            InitializeStructure(_parent);
        }

        public LadderUnitModel(LadderNetworkModel _parent, ModbusModel modbus)
        {
            type = Types.MBUS;
            IList<ValueFormat> formats = Format.Formats;
            children = new ValueModel[formats.Count];
            for (int i = 0; i < children.Length; i++)
                children[i] = new ValueModel(this, formats[i]);
            children[1].Text = modbus.Name;
            InitializeStructure(_parent);
        }

        public LadderUnitModel(LadderNetworkModel _parent, XElement xele)
        {
            children = new ValueModel[] { };
            Parent = _parent;
            try { Load(xele); }
            catch (ValueParseException) { }
            finally { InitializeStructure(_parent); }
        }

        private void InitializeStructure(LadderNetworkModel _parent)
        {
            Parent = _parent;
            if (Shape == Shapes.Input      || Shape == Shapes.Output 
             || Shape == Shapes.OutputRect || Shape == Shapes.Special)
                Breakpoint = new LadderBrpoModel(this);
        }

        public void Dispose()
        {
            foreach (ValueModel vmodel in children)
            {
                vmodel.Dispose();
            }
            if (View != null) View.Dispose();
            if (Breakpoint != null) Breakpoint.Dispose();
            Parent = null;
            Breakpoint = null;
            oldparent = null;
            children = null;
        }

        public override string ToString()
        {
            LadderDiagramModel diagram = Parent?.Parent;
            LadderNetworkModel network = Parent;
            StringBuilder ret = new StringBuilder();
            ret.Append(String.Format("(Diagram={0:s})", diagram != null ? diagram.Name : "null"));
            ret.Append(String.Format("(Network={0:s})", network != null ? network.ID.ToString() : "null"));
            ret.Append(String.Format("(X={0:d},Y={1:d})", X, Y));
            ret.Append(String.Format("({0:s}", InstName));
            for (int i = 0; i < children.Length; i++)
            {
                ret.AppendFormat(" {0:s}", children[i].Text);
            }
            ret.Append(")");
            return ret.ToString();
        }

        public string ToInstString()
        {
            StringBuilder ret = new StringBuilder(InstName);
            for (int i = 0; i < children.Length; i++)
            {
                ret.AppendFormat(" {0:s}", children[i].Text);
            }
            return ret.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderNetworkModel oldparent;
        public LadderNetworkModel OldParent
        {
            get { return this.oldparent; }
        }

        private LadderNetworkModel parent;
        public LadderNetworkModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return; 
                if (ValueManager != null) ValueManager.Remove(this);
                this.oldparent = parent;
                this.parent = value;
                if (ValueManager != null) ValueManager.Add(this);
            }
        }

        public ProjectModel Project { get { return parent?.Parent?.Parent; } }
        public InteractionFacade IFParent { get { return Project?.Parent; } }
        IModel IModel.Parent { get { return Parent; } }
        public ValueManager ValueManager
        {
            get { return parent?.Parent?.Parent?.Parent.MNGValue; }
        }

        public bool IsUsed { get { return parent != null && !parent.IsMasked; } }

        private ValueModel[] children;
        public IList<ValueModel> Children
        {
            get { return this.children; }
        }
        public IEnumerable<ValueModel> UniqueChildren
        {
            get
            {
                for (int i = 0; i < children.Count(); i++)
                {
                    if (children[i].Store?.Parent == null) continue;
                    ValueModel fit = children.First(v => ValueManager[v] == ValueManager[children[i]]);
                    if (fit == children[i]) yield return children[i];   
                }
            }
        }
        
        private int x;
        public int X
        {
            get { return this.x; }
            set { this.x = value; PropertyChanged(this, new PropertyChangedEventArgs("X")); }
        }
        
        private int y;
        public int Y
        {
            get { return this.y; }
            set { this.y = value; PropertyChanged(this, new PropertyChangedEventArgs("Y")); }
        }

        private Types type;
        public Types Type { get { return this.type; } }
        public LadderUnitFormat Format { get { return Formats[(int)Type]; } }
        public Shapes Shape { get { return Format.Shape; } }
        public string InstName { get { return Format.Name; } }
        public string[] InstArgs
        {
            get { return children.Select((c) => { return c.Text; }).ToArray(); }
            set { Parse(value); }
        }

        private PLCInstruction inst;
        public PLCInstruction Inst
        {
            get
            {
                return this.inst;
            }
            set
            {
                if (inst == value) return;
                PLCInstruction _inst = inst;
                this.inst = null;
                if (_inst != null && _inst.ProtoType != null)
                    _inst.ProtoType = null;
                this.inst = value;
                if (inst != null && inst.ProtoType != this)
                    inst.ProtoType = this;
            }
        }
        
        #region Breakpoint

        private LadderBrpoModel breakpoint;
        public LadderBrpoModel Breakpoint
        {
            get
            {
                return this.breakpoint;
            }
            set
            {
                if (breakpoint == value) return;
                LadderBrpoModel _breakpoint = breakpoint;
                this.breakpoint = null;
                if (_breakpoint != null)
                {
                    _breakpoint.PropertyChanged -= OnBreakpointPropertyChanged;
                    if (_breakpoint.Parent != null) _breakpoint.Parent = null;
                }
                this.breakpoint = value;
                if (breakpoint != null)
                {
                    breakpoint.PropertyChanged += OnBreakpointPropertyChanged;
                    if (breakpoint.Parent != this) breakpoint.Parent = this;
                }
            }
        }
        private void OnBreakpointPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnable": PropertyChanged(this, new PropertyChangedEventArgs("BPEnable")); break;
                case "IsActive": PropertyChanged(this, new PropertyChangedEventArgs("BPActive")); break;
                case "Cursor": PropertyChanged(this, new PropertyChangedEventArgs("BPCursor")); break;
            }
        }

        public bool BPEnable
        {
            get { return breakpoint != null && breakpoint.IsEnable; }
            set { if (breakpoint != null) breakpoint.IsEnable = value; }
        }

        public bool BPActive
        {
            get { return breakpoint != null && breakpoint.IsActive; }
            set { if (breakpoint != null) breakpoint.IsActive = value; }
        }

        public BreakpointCursor BPCursor
        {
            get { return breakpoint?.Cursor; }
        }
        
        #endregion

        #endregion

        #region Shell

        private LadderUnitViewModel view;
        public LadderUnitViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                LadderUnitViewModel _view = view;
                this.view = null;
                if (_view != null)
                {
                    foreach (ValueStore vstore in children.Where(vm => vm.Store?.Parent != null).Select(vm => vm.Store))
                        vstore.VisualRefNum--;
                    if (_view.Core != null) _view.Core = null;
                }
                this.view = value;
                if (view != null)
                {
                    foreach (ValueStore vstore in children.Where(vm => vm.Store?.Parent != null).Select(vm => vm.Store))
                        vstore.VisualRefNum++;
                    if (view.Core != this) view.Core = this;
                }
            }
        }
        IViewModel IModel.View
        {
            get { return view; }
            set { View = (LadderUnitViewModel)value; }
        }
        
        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        public event PropertyChangedEventHandler ViewPropertyChanged = delegate { };

        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get { return this.laddermode; }
            set { this.laddermode = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("LadderMode")); }
        }
        
        private bool iscommentmode;
        public bool IsCommentMode
        {
            get { return this.iscommentmode; }
            set { this.iscommentmode = value; ViewPropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode")); }
        }

        #endregion

        #region Load & Save

        public void Load(XElement xele)
        {
            type = LadderUnitModel.TypeOfNames[xele.Attribute("Type").Value];
            x = int.Parse(xele.Attribute("X").Value);
            y = int.Parse(xele.Attribute("Y").Value);
            IList<ValueFormat> formats = Format.Formats;
            children = new ValueModel[formats.Count];
            for (int i = 0; i < children.Length; i++)
                children[i] = new ValueModel(this, formats[i]);
            IEnumerable<XElement> xele_vs = xele.Elements("Value");
            Parse(xele_vs.Select(x => x.Value).ToArray());
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Type", InstName);
            xele.SetAttributeValue("X", x);
            xele.SetAttributeValue("Y", y);
            foreach (ValueModel vmodel in children)
            {
                XElement xele_v = new XElement("Value");
                xele_v.Value = vmodel.Text;
                xele.Add(xele_v);
            }
        }

        #endregion

        #region Parse

        public void Parse(string[] _args, bool updatevmg = true)
        {
            ValueModel[] _children = null;
            if (type == Types.CALLM)
            {
                _children = children;
                FuncModel func = Project.Funcs.Where(f => f.Name.Equals(_args[0])).FirstOrDefault();
                if (func == null) throw new ValueParseException(
                    String.Format(App.CultureIsZH_CH() ? "找不到函数{0:s}" : "Cannot found function {0:s}", _args[0]), Format.Formats[0]);
                if (updatevmg && ValueManager != null) ValueManager.Remove(this);
                children = new ValueModel[1];
                children[0] = new ValueModel(this, Format.Formats[0]);
                children[0].Text = _args[0];
                children = children.Concat(func.GetValueModels(this)).ToArray();
                if (_args.Length != children.Length)
                {
                    children[0].Dispose();
                    children = _children;
                    if (updatevmg && ValueManager != null) ValueManager.Add(this);
                    throw new ValueParseException("输入的参数数量不相符！", null);
                }
                try
                {
                    for (int i = 1; i < children.Length; i++)
                        children[i].Text = _args[i];
                }
                catch (Exception e)
                {
                    foreach (ValueModel vmodel in children)
                        if (vmodel != null) vmodel.Dispose();
                    children = _children;
                    if (updatevmg && ValueManager != null) ValueManager.Add(this);
                    throw new ValueParseException(e.Message,
                        (e is ValueParseException) ? ((ValueParseException)e).Format : null);
                }
                if (updatevmg && ValueManager != null) ValueManager.Add(this);
                Invoke(LadderUnitAction.UPDATE);
                return;
            }
            if (_args.Length != children.Length)
            {
                throw new ValueParseException("输入的参数数量不相符！", null);
            }
            string[] oldtexts = new string[children.Length];
            if (updatevmg && ValueManager != null) ValueManager.Remove(this);
            for (int i = 0; i < children.Length; i++)
            {
                try
                {
                    oldtexts[i] = children[i].Text;
                    children[i].Text = _args[i];
                }
                catch (Exception e)
                {
                    for (int j = 0; j < children.Length; j++)
                        if (oldtexts[j] != null)
                            children[j].Text = oldtexts[j];
                    if (updatevmg && ValueManager != null) ValueManager.Add(this);
                    throw new ValueParseException(e.Message,
                        (e is ValueParseException) ? ((ValueParseException)e).Format : null);
                }
            }
            if (updatevmg && ValueManager != null) ValueManager.Add(this);
            Invoke(LadderUnitAction.UPDATE);
        }
        
        public void Parse(FuncModel func, string[] _args)
        {
            if (ValueManager != null) ValueManager.Remove(this);
            type = Types.CALLM;
            children = new ValueModel[] { new ValueModel(this, Format.Formats[0]) };
            children[0].Text = func.Name;
            children = children.Concat(func.GetValueModels(this)).ToArray();
            try
            {
                Parse(_args);
            }
            catch (ValueParseException e)
            {
                throw e;
            }
            finally
            {
                if (ValueManager != null) ValueManager.Add(this);
            }
        }

        #endregion

        #region For LadderCheckModule

        public List<LadderUnitModel> NextElements = new List<LadderUnitModel>();
        public List<LadderUnitModel> SubElements = new List<LadderUnitModel>();
        public bool IsSearched { get; set; }
        public int CountLevel { get; set; }

        public virtual bool Assert()
        {
            switch (Shape)
            {
                case Shapes.HLine:
                case Shapes.Input:
                    return NextElements.All(x => { return x.Type == Types.NULL || x.Shape == Shapes.Special || x.Shape == Shapes.Input; }) && NextElements.Count > 0;
                case Shapes.Output:
                case Shapes.OutputRect:
                    if (LabelTypes.Contains(Type)) return true;
                    return NextElements.All(x => { return x.Shape == Shapes.Input || x.Shape == Shapes.Special; }) && NextElements.Count > 0;
                case Shapes.Special:
                    return NextElements.All(x => { return x.Shape == Shapes.Special || x.Shape == Shapes.Input; }) && NextElements.Count > 0;
                default:
                    return false;
            }
        }
        #endregion

        #region Event Handler

        public event LadderUnitChangedEventHandler Changed = delegate { };

        public void Invoke(LadderUnitAction action)
        {
            Changed(this, new LadderUnitChangedEventArgs(action));
            parent?.InvokeByChildren(this, action);
        }

        #endregion
    }
}
