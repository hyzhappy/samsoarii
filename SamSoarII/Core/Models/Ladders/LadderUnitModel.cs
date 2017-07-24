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
            FOR, NEXT, JMP, LBL, CALL, CALLM,
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
        
        static public string[] NameOfTypes { get; private set; }
        static public Dictionary<string, Types> TypeOfNames { get; private set; }
        static public Shapes[] ShapeOfTypes { get; private set; }
        static public Outlines[] OutlineOfTypes { get; private set; }
        static public ValueFormat[][] Formats { get; private set; }

        static LadderUnitModel()
        {
            NameOfTypes = new string[(int)Types.NULL + 1];
            NameOfTypes[(int)Types.LD] = "LD";
            NameOfTypes[(int)Types.LDI] = "LDI";
            NameOfTypes[(int)Types.LDIM] = "LDIM";
            NameOfTypes[(int)Types.LDIIM] = "LDIIM";
            NameOfTypes[(int)Types.LDP] = "LDP";
            NameOfTypes[(int)Types.LDF] = "LDF";
            NameOfTypes[(int)Types.MEP] = "MEP";
            NameOfTypes[(int)Types.MEF] = "MEF";
            NameOfTypes[(int)Types.INV] = "INV";
            NameOfTypes[(int)Types.OUT] = "OUT";
            NameOfTypes[(int)Types.OUTIM] = "OUTIM";
            NameOfTypes[(int)Types.SET] = "SET";
            NameOfTypes[(int)Types.SETIM] = "SETIM";
            NameOfTypes[(int)Types.RST] = "RST";
            NameOfTypes[(int)Types.RSTIM] = "RSTIM";
            NameOfTypes[(int)Types.ALT] = "ALT";
            NameOfTypes[(int)Types.ALTP] = "ALTP";
            NameOfTypes[(int)Types.LDWEQ] = "LDWEQ";
            NameOfTypes[(int)Types.LDWNE] = "LDWNE";
            NameOfTypes[(int)Types.LDWGE] = "LDWGE";
            NameOfTypes[(int)Types.LDWLE] = "LDWLE";
            NameOfTypes[(int)Types.LDWG] = "LDWG";
            NameOfTypes[(int)Types.LDWL] = "LDWL";
            NameOfTypes[(int)Types.LDDEQ] = "LDDEQ";
            NameOfTypes[(int)Types.LDDNE] = "LDDNE";
            NameOfTypes[(int)Types.LDDGE] = "LDDGE";
            NameOfTypes[(int)Types.LDDLE] = "LDDLE";
            NameOfTypes[(int)Types.LDDG] = "LDDG";
            NameOfTypes[(int)Types.LDDL] = "LDDL";
            NameOfTypes[(int)Types.LDFEQ] = "LDFEQ";
            NameOfTypes[(int)Types.LDFNE] = "LDFNE";
            NameOfTypes[(int)Types.LDFGE] = "LDFGE";
            NameOfTypes[(int)Types.LDFLE] = "LDFLE";
            NameOfTypes[(int)Types.LDFG] = "LDFG";
            NameOfTypes[(int)Types.LDFL] = "LDFL";
            NameOfTypes[(int)Types.WTOD] = "WTOD";
            NameOfTypes[(int)Types.DTOW] = "DTOW";
            NameOfTypes[(int)Types.DTOF] = "DTOF";
            NameOfTypes[(int)Types.BIN] = "BIN";
            NameOfTypes[(int)Types.BCD] = "BCD";
            NameOfTypes[(int)Types.ROUND] = "ROUND";
            NameOfTypes[(int)Types.TRUNC] = "TRUNC";
            NameOfTypes[(int)Types.INVW] = "INVW";
            NameOfTypes[(int)Types.INVD] = "INVD";
            NameOfTypes[(int)Types.ANDW] = "ANDW";
            NameOfTypes[(int)Types.ANDD] = "ANDD";
            NameOfTypes[(int)Types.ORW] = "ORW";
            NameOfTypes[(int)Types.ORD] = "ORD";
            NameOfTypes[(int)Types.XORW] = "XORW";
            NameOfTypes[(int)Types.XORD] = "XORD";
            NameOfTypes[(int)Types.MOVD] = "MOVD";
            NameOfTypes[(int)Types.MOV] = "MOV";
            NameOfTypes[(int)Types.MOVF] = "MOVF";
            NameOfTypes[(int)Types.MVBLK] = "MVBLK";
            NameOfTypes[(int)Types.MVDBLK] = "MVDBLK";
            NameOfTypes[(int)Types.ADDF] = "ADDF";
            NameOfTypes[(int)Types.SUBF] = "SUBF";
            NameOfTypes[(int)Types.MULF] = "MULF";
            NameOfTypes[(int)Types.DIVF] = "DIVF";
            NameOfTypes[(int)Types.SQRT] = "SQRT";
            NameOfTypes[(int)Types.SIN] = "SIN";
            NameOfTypes[(int)Types.COS] = "COS";
            NameOfTypes[(int)Types.TAN] = "TAN";
            NameOfTypes[(int)Types.LN] = "LN";
            NameOfTypes[(int)Types.EXP] = "EXP";
            NameOfTypes[(int)Types.ADD] = "ADD";
            NameOfTypes[(int)Types.ADDD] = "ADDD";
            NameOfTypes[(int)Types.SUB] = "SUB";
            NameOfTypes[(int)Types.SUBD] = "SUBD";
            NameOfTypes[(int)Types.MUL] = "MUL";
            NameOfTypes[(int)Types.MULW] = "MULW";
            NameOfTypes[(int)Types.MULD] = "MULD";
            NameOfTypes[(int)Types.DIV] = "DIV";
            NameOfTypes[(int)Types.DIVW] = "DIVW";
            NameOfTypes[(int)Types.DIVD] = "DIVD";
            NameOfTypes[(int)Types.INC] = "INC";
            NameOfTypes[(int)Types.INCD] = "INCD";
            NameOfTypes[(int)Types.DEC] = "DEC";
            NameOfTypes[(int)Types.DECD] = "DECD";
            NameOfTypes[(int)Types.TON] = "TON";
            NameOfTypes[(int)Types.TOF] = "TOF";
            NameOfTypes[(int)Types.TONR] = "TONR";
            NameOfTypes[(int)Types.CTU] = "CTU";
            NameOfTypes[(int)Types.CTD] = "CTD";
            NameOfTypes[(int)Types.CTUD] = "CTUD";
            NameOfTypes[(int)Types.FOR] = "FOR";
            NameOfTypes[(int)Types.NEXT] = "NEXT";
            NameOfTypes[(int)Types.JMP] = "JMP";
            NameOfTypes[(int)Types.LBL] = "LBL";
            NameOfTypes[(int)Types.CALL] = "CALL";
            NameOfTypes[(int)Types.CALLM] = "CALLM";
            NameOfTypes[(int)Types.SHL] = "SHL";
            NameOfTypes[(int)Types.SHLD] = "SHLD";
            NameOfTypes[(int)Types.SHR] = "SHR";
            NameOfTypes[(int)Types.SHRD] = "SHRD";
            NameOfTypes[(int)Types.ROL] = "ROL";
            NameOfTypes[(int)Types.ROLD] = "ROLD";
            NameOfTypes[(int)Types.ROR] = "ROR";
            NameOfTypes[(int)Types.RORD] = "RORD";
            NameOfTypes[(int)Types.SHLB] = "SHLB";
            NameOfTypes[(int)Types.SHRB] = "SHRB";
            NameOfTypes[(int)Types.ATCH] = "ATCH";
            NameOfTypes[(int)Types.DTCH] = "DTCH";
            NameOfTypes[(int)Types.EI] = "EI";
            NameOfTypes[(int)Types.DI] = "DI";
            NameOfTypes[(int)Types.TRD] = "TRD";
            NameOfTypes[(int)Types.TWR] = "TWR";
            NameOfTypes[(int)Types.MBUS] = "MBUS";
            NameOfTypes[(int)Types.SEND] = "SEND";
            NameOfTypes[(int)Types.REV] = "REV";
            NameOfTypes[(int)Types.PLSF] = "PLSF";
            NameOfTypes[(int)Types.DPLSF] = "DPLSF";
            NameOfTypes[(int)Types.PWM] = "PWM";
            NameOfTypes[(int)Types.DPWM] = "DPWM";
            NameOfTypes[(int)Types.PLSY] = "PLSY";
            NameOfTypes[(int)Types.DPLSY] = "DPLSY";
            NameOfTypes[(int)Types.PLSR] = "PLSR";
            NameOfTypes[(int)Types.DPLSR] = "DPLSR";
            NameOfTypes[(int)Types.PLSRD] = "PLSRD";
            NameOfTypes[(int)Types.DPLSRD] = "DPLSRD";
            NameOfTypes[(int)Types.PLSNEXT] = "PLSNEXT";
            NameOfTypes[(int)Types.PLSSTOP] = "PLSSTOP";
            NameOfTypes[(int)Types.ZRN] = "ZRN";
            NameOfTypes[(int)Types.DZRN] = "DZRN";
            NameOfTypes[(int)Types.PTO] = "PTO";
            NameOfTypes[(int)Types.DRVI] = "DRVI";
            NameOfTypes[(int)Types.DDRVI] = "DDRVI";
            NameOfTypes[(int)Types.HCNT] = "HCNT";
            NameOfTypes[(int)Types.LOG] = "LOG";
            NameOfTypes[(int)Types.POW] = "POW";
            NameOfTypes[(int)Types.FACT] = "FACT";
            NameOfTypes[(int)Types.CMP] = "CMP";
            NameOfTypes[(int)Types.CMPD] = "CMPD";
            NameOfTypes[(int)Types.CMPF] = "CMPF";
            NameOfTypes[(int)Types.ZCP] = "ZCP";
            NameOfTypes[(int)Types.ZCPD] = "ZCPD";
            NameOfTypes[(int)Types.ZCPF] = "ZCPF";
            NameOfTypes[(int)Types.NEG] = "NEG";
            NameOfTypes[(int)Types.NEGD] = "NEGD";
            NameOfTypes[(int)Types.XCH] = "XCH";
            NameOfTypes[(int)Types.XCHD] = "XCHD";
            NameOfTypes[(int)Types.XCHF] = "XCHF";
            NameOfTypes[(int)Types.CML] = "CML";
            NameOfTypes[(int)Types.CMLD] = "CMLD";
            NameOfTypes[(int)Types.SMOV] = "SMOV";
            NameOfTypes[(int)Types.FMOV] = "FMOV";
            NameOfTypes[(int)Types.FMOVD] = "FMOVD";
            NameOfTypes[(int)Types.VLINE] = "VLINE";
            NameOfTypes[(int)Types.HLINE] = "HLINE";
            NameOfTypes[(int)Types.NULL] = "NULL";

            TypeOfNames = new Dictionary<string, Types>();
            for (int i = 0; i < NameOfTypes.Length; i++)
            {
                TypeOfNames[NameOfTypes[i]] = (Types)i;
            }

            ShapeOfTypes = new Shapes[(int)Types.NULL + 1];
            ShapeOfTypes[(int)Types.LD] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDI] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDIM] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDIIM] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDP] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDF] = Shapes.Input;
            ShapeOfTypes[(int)Types.MEP] = Shapes.Special;
            ShapeOfTypes[(int)Types.MEF] = Shapes.Special;
            ShapeOfTypes[(int)Types.INV] = Shapes.Special;
            ShapeOfTypes[(int)Types.OUT] = Shapes.Output;
            ShapeOfTypes[(int)Types.OUTIM] = Shapes.Output;
            ShapeOfTypes[(int)Types.SET] = Shapes.Output;
            ShapeOfTypes[(int)Types.SETIM] = Shapes.Output;
            ShapeOfTypes[(int)Types.RST] = Shapes.Output;
            ShapeOfTypes[(int)Types.RSTIM] = Shapes.Output;
            ShapeOfTypes[(int)Types.ALT] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ALTP] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.LDWEQ] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDWNE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDWGE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDWLE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDWG] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDWL] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDDEQ] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDDNE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDDGE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDDLE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDDG] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDDL] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDFEQ] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDFNE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDFGE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDFLE] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDFG] = Shapes.Input;
            ShapeOfTypes[(int)Types.LDFL] = Shapes.Input;
            ShapeOfTypes[(int)Types.WTOD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DTOW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DTOF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.BIN] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.BCD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ROUND] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TRUNC] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.INVW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.INVD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ANDW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ANDD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ORW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ORD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.XORW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.XORD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MOVD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MOV] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MOVF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MVBLK] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MVDBLK] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ADDF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SUBF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MULF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DIVF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SQRT] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SIN] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.COS] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TAN] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.LN] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.EXP] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ADD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ADDD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SUB] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SUBD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MUL] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MULW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MULD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DIV] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DIVW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DIVD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.INC] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.INCD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DEC] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DECD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TON] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TOF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TONR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CTU] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CTD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CTUD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.FOR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.NEXT] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.JMP] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.LBL] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CALL] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CALLM] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SHL] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SHLD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SHR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SHRD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ROL] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ROLD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ROR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.RORD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SHLB] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SHRB] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ATCH] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DTCH] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.EI] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DI] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TRD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.TWR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.MBUS] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SEND] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.REV] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PLSF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DPLSF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PWM] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DPWM] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PLSY] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DPLSY] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PLSR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DPLSR] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PLSRD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DPLSRD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PLSNEXT] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PLSSTOP] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ZRN] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DZRN] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.PTO] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DRVI] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.DDRVI] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.HCNT] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.LOG] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.POW] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.FACT] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CMP] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CMPD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CMPF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ZCP] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ZCPD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.ZCPF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.NEG] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.NEGD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.XCH] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.XCHD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.XCHF] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CML] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.CMLD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.SMOV] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.FMOV] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.FMOVD] = Shapes.OutputRect;
            ShapeOfTypes[(int)Types.VLINE] = Shapes.VLine;
            ShapeOfTypes[(int)Types.HLINE] = Shapes.HLine;
            ShapeOfTypes[(int)Types.NULL] = Shapes.Null;

            OutlineOfTypes = new Outlines[(int)Types.NULL + 1];
            OutlineOfTypes[(int)Types.LD] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.LDI] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.LDIM] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.LDIIM] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.LDP] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.LDF] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.MEP] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.MEF] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.INV] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.OUT] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.OUTIM] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.SET] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.SETIM] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.RST] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.RSTIM] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.ALT] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.ALTP] = Outlines.BitOperation;
            OutlineOfTypes[(int)Types.LDWEQ] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDWNE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDWGE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDWLE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDWG] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDWL] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDDEQ] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDDNE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDDGE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDDLE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDDG] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDDL] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDFEQ] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDFNE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDFGE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDFLE] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDFG] = Outlines.Compare;
            OutlineOfTypes[(int)Types.LDFL] = Outlines.Compare;
            OutlineOfTypes[(int)Types.WTOD] = Outlines.Convert;
            OutlineOfTypes[(int)Types.DTOW] = Outlines.Convert;
            OutlineOfTypes[(int)Types.DTOF] = Outlines.Convert;
            OutlineOfTypes[(int)Types.BIN] = Outlines.Convert;
            OutlineOfTypes[(int)Types.BCD] = Outlines.Convert;
            OutlineOfTypes[(int)Types.ROUND] = Outlines.Convert;
            OutlineOfTypes[(int)Types.TRUNC] = Outlines.Convert;
            OutlineOfTypes[(int)Types.INVW] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.INVD] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.ANDW] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.ANDD] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.ORW] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.ORD] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.XORW] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.XORD] = Outlines.LogicOperation;
            OutlineOfTypes[(int)Types.MOVD] = Outlines.Move;
            OutlineOfTypes[(int)Types.MOV] = Outlines.Move;
            OutlineOfTypes[(int)Types.MOVF] = Outlines.Move;
            OutlineOfTypes[(int)Types.MVBLK] = Outlines.Move;
            OutlineOfTypes[(int)Types.MVDBLK] = Outlines.Move;
            OutlineOfTypes[(int)Types.ADDF] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.SUBF] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.MULF] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.DIVF] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.SQRT] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.SIN] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.COS] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.TAN] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.LN] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.EXP] = Outlines.FloatCalculation;
            OutlineOfTypes[(int)Types.ADD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.ADDD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.SUB] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.SUBD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.MUL] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.MULW] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.MULD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.DIV] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.DIVW] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.DIVD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.INC] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.INCD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.DEC] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.DECD] = Outlines.IntegerCalculation;
            OutlineOfTypes[(int)Types.TON] = Outlines.Timer;
            OutlineOfTypes[(int)Types.TOF] = Outlines.Timer;
            OutlineOfTypes[(int)Types.TONR] = Outlines.Timer;
            OutlineOfTypes[(int)Types.CTU] = Outlines.Counter;
            OutlineOfTypes[(int)Types.CTD] = Outlines.Counter;
            OutlineOfTypes[(int)Types.CTUD] = Outlines.Counter;
            OutlineOfTypes[(int)Types.FOR] = Outlines.ProgramControl;
            OutlineOfTypes[(int)Types.NEXT] = Outlines.ProgramControl;
            OutlineOfTypes[(int)Types.JMP] = Outlines.ProgramControl;
            OutlineOfTypes[(int)Types.LBL] = Outlines.ProgramControl;
            OutlineOfTypes[(int)Types.CALL] = Outlines.ProgramControl;
            OutlineOfTypes[(int)Types.CALLM] = Outlines.ProgramControl;
            OutlineOfTypes[(int)Types.SHL] = Outlines.Shift;
            OutlineOfTypes[(int)Types.SHLD] = Outlines.Shift;
            OutlineOfTypes[(int)Types.SHR] = Outlines.Shift;
            OutlineOfTypes[(int)Types.SHRD] = Outlines.Shift;
            OutlineOfTypes[(int)Types.ROL] = Outlines.Shift;
            OutlineOfTypes[(int)Types.ROLD] = Outlines.Shift;
            OutlineOfTypes[(int)Types.ROR] = Outlines.Shift;
            OutlineOfTypes[(int)Types.RORD] = Outlines.Shift;
            OutlineOfTypes[(int)Types.SHLB] = Outlines.Shift;
            OutlineOfTypes[(int)Types.SHRB] = Outlines.Shift;
            OutlineOfTypes[(int)Types.ATCH] = Outlines.Interrupt;
            OutlineOfTypes[(int)Types.DTCH] = Outlines.Interrupt;
            OutlineOfTypes[(int)Types.EI] = Outlines.Interrupt;
            OutlineOfTypes[(int)Types.DI] = Outlines.Interrupt;
            OutlineOfTypes[(int)Types.TRD] = Outlines.RealTime;
            OutlineOfTypes[(int)Types.TWR] = Outlines.RealTime;
            OutlineOfTypes[(int)Types.MBUS] = Outlines.Communication;
            OutlineOfTypes[(int)Types.SEND] = Outlines.Communication;
            OutlineOfTypes[(int)Types.REV] = Outlines.Communication;
            OutlineOfTypes[(int)Types.PLSF] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DPLSF] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PWM] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DPWM] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PLSY] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DPLSY] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PLSR] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DPLSR] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PLSRD] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DPLSRD] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PLSNEXT] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PLSSTOP] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.ZRN] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DZRN] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.PTO] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DRVI] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.DDRVI] = Outlines.Pulse;
            OutlineOfTypes[(int)Types.HCNT] = Outlines.HighCount;
            OutlineOfTypes[(int)Types.LOG] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.POW] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.FACT] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.CMP] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.CMPD] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.CMPF] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.ZCP] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.ZCPD] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.ZCPF] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.NEG] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.NEGD] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.XCH] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.XCHD] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.XCHF] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.CML] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.CMLD] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.SMOV] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.FMOV] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.FMOVD] = Outlines.Auxiliar;
            OutlineOfTypes[(int)Types.VLINE] = Outlines.NULL;
            OutlineOfTypes[(int)Types.HLINE] = Outlines.NULL;
            OutlineOfTypes[(int)Types.NULL] = Outlines.NULL;

            Formats = new ValueFormat[(int)Types.NULL + 1][];
            Formats[(int)Types.LD] =
            Formats[(int)Types.LDI] =
            Formats[(int)Types.LDIM] =
            Formats[(int)Types.LDIIM] =
            Formats[(int)Types.LDP] =
            Formats[(int)Types.LDF] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.BOOL, true, false, 0, new Regex[]{ ValueModel.VerifyBitRegex1}) };
            Formats[(int)Types.MEP] =
            Formats[(int)Types.MEF] =
            Formats[(int)Types.INV] =
            Formats[(int)Types.NEXT] =
            Formats[(int)Types.EI] =
            Formats[(int)Types.DI] =
            Formats[(int)Types.HLINE] =
            Formats[(int)Types.VLINE] =
            Formats[(int)Types.NULL] =
                new ValueFormat[] { };
            Formats[(int)Types.OUT] =
            Formats[(int)Types.OUTIM] =
            Formats[(int)Types.ALT] =
            Formats[(int)Types.ALTP] =
                new ValueFormat[] {
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, 0, new Regex[]{ ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.SET] =
            Formats[(int)Types.SETIM] =
            Formats[(int)Types.RST] =
            Formats[(int)Types.RSTIM] =
                new ValueFormat[] {
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, 0, new Regex[]{ ValueModel.VerifyBitRegex3}),
                    new ValueFormat("CT", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.LDWEQ] =
            Formats[(int)Types.LDWNE] =
            Formats[(int)Types.LDWLE] =
            Formats[(int)Types.LDWGE] =
            Formats[(int)Types.LDWL] =
            Formats[(int)Types.LDWG] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.LDDEQ] =
            Formats[(int)Types.LDDNE] =
            Formats[(int)Types.LDDLE] =
            Formats[(int)Types.LDDGE] =
            Formats[(int)Types.LDDL] =
            Formats[(int)Types.LDDG] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.LDFEQ] =
            Formats[(int)Types.LDFNE] =
            Formats[(int)Types.LDFLE] =
            Formats[(int)Types.LDFGE] =
            Formats[(int)Types.LDFL] =
            Formats[(int)Types.LDFG] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}) };
            Formats[(int)Types.WTOD] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2}) };
            Formats[(int)Types.DTOW] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex2}) };
            Formats[(int)Types.DTOF] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.FLOAT, false, true, -1, new Regex[] { ValueModel.VerifyFloatRegex}) };
            Formats[(int)Types.BIN] =
            Formats[(int)Types.BCD] =
            Formats[(int)Types.INVW] =
            Formats[(int)Types.MOV] =
            Formats[(int)Types.INC] =
            Formats[(int)Types.DEC] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex2}) };
            Formats[(int)Types.INVD] =
            Formats[(int)Types.MOVD] =
            Formats[(int)Types.INCD] =
            Formats[(int)Types.DECD] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2}) };
            Formats[(int)Types.MOVF] =
            Formats[(int)Types.EXP] =
            Formats[(int)Types.LN] =
            Formats[(int)Types.LOG] =
            Formats[(int)Types.SQRT] =
            Formats[(int)Types.SIN] =
            Formats[(int)Types.COS] =
            Formats[(int)Types.TAN] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.FLOAT, true, false, 0, new Regex[] {ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.FLOAT, false, true, -1, new Regex[] { ValueModel.VerifyFloatRegex}) };
            Formats[(int)Types.MVBLK] =
                new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("D", ValueModel.Types.WORD, false, true, 1, new Regex[] { ValueModel.VerifyWordRegex2}),
                    new ValueFormat("N", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.MVDBLK] =
                new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("D", ValueModel.Types.DWORD, false, true, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("N", ValueModel.Types.DWORD, true, false, 2, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.TRUNC] =
            Formats[(int)Types.ROUND] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.FLOAT, true, false, 0, new Regex[] {ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] {ValueModel.VerifyDoubleWordRegex2}) };
            Formats[(int)Types.ANDW] =
            Formats[(int)Types.ORW] =
            Formats[(int)Types.XORW] =
            Formats[(int)Types.ADD] =
            Formats[(int)Types.SUB] =
            Formats[(int)Types.MULW] =
            Formats[(int)Types.DIVW] =
            Formats[(int)Types.SHL] =
            Formats[(int)Types.SHR] =
            Formats[(int)Types.ROL] =
            Formats[(int)Types.ROR] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex2}) };
            Formats[(int)Types.ANDD] =
            Formats[(int)Types.ORD] =
            Formats[(int)Types.XORD] =
            Formats[(int)Types.ADDD] =
            Formats[(int)Types.SUBD] =
            Formats[(int)Types.MULD] =
            Formats[(int)Types.DIVD] =
            Formats[(int)Types.SHLD] =
            Formats[(int)Types.SHRD] =
            Formats[(int)Types.ROLD] =
            Formats[(int)Types.RORD] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2}) };
            Formats[(int)Types.ADDF] =
            Formats[(int)Types.SUBF] =
            Formats[(int)Types.MULF] =
            Formats[(int)Types.DIVF] =
            Formats[(int)Types.POW] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.FLOAT, false, true, -1, new Regex[] { ValueModel.VerifyFloatRegex}) };
            Formats[(int)Types.MUL] =
            Formats[(int)Types.DIV] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex2}) };
            Formats[(int)Types.TON] =
            Formats[(int)Types.TOF] =
            Formats[(int)Types.TONR] =
                new ValueFormat[] {
                    new ValueFormat("T", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex4}),
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.CTU] =
            Formats[(int)Types.CTD] =
            Formats[(int)Types.CTUD] =
            Formats[(int)Types.HCNT] =
                new ValueFormat[] {
                    new ValueFormat("C", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex3}),
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.FOR] =
                new ValueFormat[] {
                    new ValueFormat("CNT", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.JMP] =
            Formats[(int)Types.LBL] =
                new ValueFormat[] {
                    new ValueFormat("LBL", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.CALL] =
                new ValueFormat[] {
                    new ValueFormat("FUNC", ValueModel.Types.STRING, true, false, 0, new Regex[] { ValueModel.AnyNameRegex }) };
            Formats[(int)Types.CALLM] =
                new ValueFormat[] {
                    new ValueFormat("SUB", ValueModel.Types.STRING, true, false, 0, new Regex[] { ValueModel.FuncNameRegex }) };
            Formats[(int)Types.SHLB] =
            Formats[(int)Types.SHRB] =
                new ValueFormat[] {
                    new ValueFormat("S", ValueModel.Types.BOOL, true, false, 0, new Regex[] { ValueModel.VerifyBitRegex1 }),
                    new ValueFormat("D", ValueModel.Types.BOOL, false, true, 1, new Regex[] { ValueModel.VerifyBitRegex2 }),
                    new ValueFormat("N1", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex,  }),
                    new ValueFormat("N2", ValueModel.Types.WORD, true, false, 3, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex,  }), };
            Formats[(int)Types.ATCH] =
                new ValueFormat[] {
                    new ValueFormat("INT", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex,  }),
                    new ValueFormat("EVENT", ValueModel.Types.STRING, true, false, 1, new Regex[] {ValueModel.AnyNameRegex}) };
            Formats[(int)Types.DTCH] =
                new ValueFormat[] {
                    new ValueFormat("INT", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex,  }) };
            Formats[(int)Types.TRD] =
                new ValueFormat[] {
                    new ValueFormat("T", ValueModel.Types.WORD, false, true, 0, new Regex[] {ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.TWR] =
                new ValueFormat[] {
                    new ValueFormat("T", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.MBUS] =
                new ValueFormat[] {
                    new ValueFormat("PORT", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}),
                    new ValueFormat("TABLE", ValueModel.Types.STRING, true, false, 1, new Regex[] {ValueModel.AnyNameRegex}),
                    new ValueFormat("WR", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3}) };
            Formats[(int)Types.SEND] =
                new ValueFormat[] {
                    new ValueFormat("COM", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}),
                    new ValueFormat("ADDR", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("LEN", ValueModel.Types.WORD, true, false, 2, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }) };
            Formats[(int)Types.REV] =
                new ValueFormat[] {
                    new ValueFormat("COM", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}),
                    new ValueFormat("ADDR", ValueModel.Types.WORD, false, true, 1, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("LEN", ValueModel.Types.WORD, true, false, 2, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex }) };
            Formats[(int)Types.PLSF] =
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DPLSF] =
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PLSY] =
            Formats[(int)Types.DRVI] =
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("P", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DPLSY] =
            Formats[(int)Types.DDRVI] =
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("P", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PWM] =
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("DC", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DPWM] =
                new ValueFormat[] {
                    new ValueFormat("F", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("DC", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PLSR] =
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("V", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DPLSR] =
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("V", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PLSRD] =
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("V", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DPLSRD] =
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("V", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PLSNEXT] =
            Formats[(int)Types.PLSSTOP] =
                new ValueFormat[] {
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.ZRN] =
                new ValueFormat[] {
                    new ValueFormat("DV", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("CV", ValueModel.Types.WORD, true, false, 1, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("SIG", ValueModel.Types.BOOL, true, false, 2, new Regex[] {ValueModel.VerifyBitRegex5}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.DZRN] =
                new ValueFormat[] {
                    new ValueFormat("DV", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyDoubleWordRegex2}),
                    new ValueFormat("CV", ValueModel.Types.DWORD, true, false, 1, new Regex[] {ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("SIG", ValueModel.Types.BOOL, true, false, 2, new Regex[] {ValueModel.VerifyBitRegex5}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.PTO] =
                new ValueFormat[] {
                    new ValueFormat("D", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -2, new Regex[] {ValueModel.VerifyBitRegex4}),
                    new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, -1, new Regex[] {ValueModel.VerifyBitRegex4}) };
            Formats[(int)Types.FACT] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex1}) };
            Formats[(int)Types.CMP] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.CMPD] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.CMPF] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.ZCP] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN3", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.ZCPD] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.DWORD, true, false, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN3", ValueModel.Types.DWORD, true, false, 2, new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.ZCPF] =
                new ValueFormat[] {
                    new ValueFormat("IN1", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN2", ValueModel.Types.FLOAT, true, false, 1, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("IN3", ValueModel.Types.FLOAT, true, false, 2, new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex}),
                    new ValueFormat("OUT", ValueModel.Types.BOOL, false, true, -1, new Regex[] { ValueModel.VerifyBitRegex3}) };
            Formats[(int)Types.XCH] =
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("R", ValueModel.Types.WORD, true, true, 1, new Regex[] { ValueModel.VerifyWordRegex1}) };
            Formats[(int)Types.XCHD] =
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.DWORD, true, true, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1}),
                    new ValueFormat("R", ValueModel.Types.DWORD, true, true, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex1}) };
            Formats[(int)Types.XCHF] =
                new ValueFormat[] {
                    new ValueFormat("L", ValueModel.Types.FLOAT, true, true, 0, new Regex[] { ValueModel.VerifyFloatRegex}),
                    new ValueFormat("R", ValueModel.Types.FLOAT, true, true, 1, new Regex[] { ValueModel.VerifyFloatRegex}) };
            Formats[(int)Types.NEG] =
            Formats[(int)Types.CML] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.WORD, false, true, -1, new Regex[] { ValueModel.VerifyWordRegex1}) };
            Formats[(int)Types.NEGD] =
            Formats[(int)Types.CMLD] =
                new ValueFormat[] {
                    new ValueFormat("IN", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("OUT", ValueModel.Types.DWORD, false, true, -1, new Regex[] { ValueModel.VerifyDoubleWordRegex1}) };
            Formats[(int)Types.SMOV] =
                new ValueFormat[] {
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("SS", ValueModel.Types.WORD, true, false, 1, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("SC", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("DV", ValueModel.Types.WORD, false, true, -2, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("DS", ValueModel.Types.WORD, true, false, -1, new Regex[] { ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.FMOV] =
                new ValueFormat[] {
                    new ValueFormat("SV", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("TV", ValueModel.Types.WORD, false, true, 1, new Regex[] { ValueModel.VerifyWordRegex1}),
                    new ValueFormat("CNT", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            Formats[(int)Types.FMOVD] =
                new ValueFormat[] {
                    new ValueFormat("SV", ValueModel.Types.DWORD, true, false, 0, new Regex[] { ValueModel.VerifyDoubleWordRegex1, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }),
                    new ValueFormat("TV", ValueModel.Types.DWORD, false, true, 1, new Regex[] { ValueModel.VerifyDoubleWordRegex1}),
                    new ValueFormat("CNT", ValueModel.Types.WORD, true, false, 2, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex, ValueModel.VerifyIntHValueRegex, }) };
            
        }
        
        #endregion

        public LadderUnitModel(LadderNetworkModel _parent, Types _type)
        {
            type = _type;
            ValueFormat[] formats = Formats[(int)type];
            children = new ValueModel[formats.Length];
            for (int i = 0; i < children.Length; i++)
                children[i] = new ValueModel(this, formats[i]);
            Parent = _parent;
        }

        public LadderUnitModel(LadderNetworkModel _parent, FuncModel func)
        {
            type = Types.CALLM;
            children = new ValueModel[] { new ValueModel(this, Formats[(int)Types.CALLM][0])};
            children[0].Text = func.Name;
            children = children.Concat(func.GetValueModels(this)).ToArray();
            Parent = _parent;
        }

        public LadderUnitModel(LadderNetworkModel _parent, ModbusModel modbus)
        {
            type = Types.MBUS;
            ValueFormat[] formats = Formats[(int)type];
            children = new ValueModel[formats.Length];
            for (int i = 0; i < children.Length; i++)
                children[i] = new ValueModel(this, formats[i]);
            children[1].Text = modbus.Name;
            Parent = _parent;
        }

        public LadderUnitModel(LadderNetworkModel _parent, XElement xele)
        {
            Load(xele);
            Parent = _parent;
        }

        public void Dispose()
        {
            foreach (ValueModel vmodel in children)
            {
                vmodel.Dispose();
            }
            if (View != null) View.Dispose();
            Parent = null;
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
        public Types Type
        {
            get { return this.type; }
        }
        public Shapes Shape
        {
            get { return ShapeOfTypes[(int)type]; }
        }
        public string InstName
        {
            get { return NameOfTypes[(int)type]; }
        }
        public string[] InstArgs
        {
            get
            {
                return children.Select((c) => { return c.Text; }).ToArray();
            }
            set
            {
                Parse(value);
            }
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
        
        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get { return this.laddermode; }
            set { this.laddermode = value; PropertyChanged(this, new PropertyChangedEventArgs("LadderMode")); }
        }

        #region Breakpoint

        private int bpAddress;
        public int BPAddress
        {
            get { return this.bpAddress; }
            set { this.bpAddress = value; }
        }

        private BreakpointCursor bpCursor;
        public BreakpointCursor BPCursor
        {
            get { return this.bpCursor; }
            set { this.bpCursor = value; PropertyChanged(this, new PropertyChangedEventArgs("BPCursor")); }
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
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
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

        #endregion

        #region Load & Save

        public void Load(XElement xele)
        {
            type = LadderUnitModel.TypeOfNames[xele.Attribute("Type").Value];
            x = int.Parse(xele.Attribute("X").Value);
            y = int.Parse(xele.Attribute("Y").Value);
            ValueFormat[] formats = Formats[(int)type];
            IEnumerable<XElement> xele_vs = xele.Elements("Value");
            children = new ValueModel[xele_vs.Count()];
            int i = 0;
            foreach (XElement xele_v in xele_vs)
            {
                children[i] = new ValueModel(this, formats[i]);
                children[i++].Text = xele_v.Value;
            }
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Type", LadderUnitModel.NameOfTypes[(int)type]);
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
                    String.Format(App.CultureIsZH_CH() ? "找不到函数{0:s}" : "Cannot found function {0:s}", _args[0]), Formats[(int)Types.CALLM][0]);
                if (updatevmg && ValueManager != null) ValueManager.Remove(this);
                children = new ValueModel[1];
                children[0] = new ValueModel(this, Formats[(int)Types.CALLM][0]);
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
                    for (int j = 0; j < i; j++)
                        children[j].Text = oldtexts[j];
                    if (updatevmg && ValueManager != null) ValueManager.Add(this);
                    throw new ValueParseException(e.Message,
                        (e is ValueParseException) ? ((ValueParseException)e).Format : null);
                }
            }
            if (updatevmg && ValueManager != null) ValueManager.Add(this);
            Invoke(LadderUnitAction.UPDATE);
        }

        public void Parse(LadderUnitModel.Types _type, string[] _args)
        {
            if (ShapeOfTypes[(int)_type] != ShapeOfTypes[(int)type])
                throw new ValueParseException("元件的新类型和原来不匹配！", null);
            type = _type;
            Parse(_args);
        }

        public void Parse(FuncModel func, string[] _args)
        {
            if (ValueManager != null) ValueManager.Remove(this);
            children = new ValueModel[] { new ValueModel(this, Formats[(int)Types.CALLM][0]) };
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
                    return NextElements.All(x => { return (x.Type == Types.NULL) | (x.Shape == Shapes.Special) | (x.Shape == Shapes.Input); }) & NextElements.Count > 0;
                case Shapes.Output:
                case Shapes.OutputRect:
                    return NextElements.All(x => { return (x.Shape == Shapes.Input) | (x.Shape == Shapes.Special); }) & NextElements.Count > 0;
                case Shapes.Special:
                    return NextElements.All(x => { return x.Shape == Shapes.Special || x.Shape == Shapes.Input; }) & NextElements.Count > 0;
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
        }

        #endregion
    }
}
