using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Simulation.Shell.ViewModel
{
    /// <summary>
    /// SimuViewOutRecModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewOutRecModel : SimuViewBaseModel
    {
        public SimuViewOutRecModel(SimulateModel parent) : base(parent)
        {
            InitializeComponent();
        }

        public override void Setup(string text)
        {
            string[] texts = text.Split(' ');
            Inst = texts[0];
            
            switch (Inst)
            {
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this._args2 = _parent.GetVariableUnit(texts[2], "FLOAT");
                    break;
                // (rW, wD)
                case "WTOD":
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
                    break;
                // (rD, wF)
                case "DTOW":
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    break;
                // (rD, wF)
                case "DTOF":
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "FLOAT");
                    break;
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
                    break;
                // (rW, wW)
                case "BIN":
                case "BCD":
                case "INVW":
                case "MOV":
                case "INC":
                case "DEC":
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    break;
                // (rD, wD)
                case "INVD":
                case "MOVD":
                case "INCD":
                case "DECD":
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
                    break;
                // (rF, wF)
                case "MOVF":
                case "SQRT":
                case "SIN":
                case "COS":
                case "TAN":
                case "LN":
                case "EXP":
                    this._args1 = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this._args2 = _parent.GetVariableUnit(texts[2], "FLOAT");
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    this._args3 = _parent.GetVariableUnit(texts[3], "WORD");
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
                    this._args3 = _parent.GetVariableUnit(texts[3], "DWORD");
                    break;
                // (rW, rW, wD)
                case "MUL":
                case "DIV":
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    this._args3 = _parent.GetVariableUnit(texts[3], "DWORD");
                    break;
                // (rF, rF, wF)
                case "ADDF":
                case "SUBF":
                case "MULF":
                case "DIVF":
                    this._args1 = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this._args2 = _parent.GetVariableUnit(texts[2], "FLOAT");
                    this._args3 = _parent.GetVariableUnit(texts[3], "FLOAT");
                    break;
                // (rW, wW, rW)
                case "MVBLK":
                case "FMOV":
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    this._args3 = _parent.GetVariableUnit(texts[3], "WORD");
                    break;
                // (rD, wD, rD)
                case "MVDBLK":
                case "FMOVD":
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
                    this._args3 = _parent.GetVariableUnit(texts[3], "DWORD");
                    break;
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
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    // T/C + 地址
                    this._args3 = _parent.GetVariableUnit(texts[1][0] + texts[1].Substring(2), "BIT");
                    break;
                // (rW)
                case "FOR":
                case "JMP":
                case "LBL":
                case "CALL":
                    //this.flag1 = ToCStyle(args[1], "r", "WORD");
                    break;
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
                    /*
                    this.flag1 = args[1];
                    this.flag2 = ToCStyle(args[2], "rw", "WORD");
                    this.flag3 = ToCStyle(args[3], "rw", "BIT");
                    */
                    break;
                // (rW, rW, rW, wW, rW)
                case "SMOV":
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    this._args3 = _parent.GetVariableUnit(texts[3], "WORD");
                    this._args4 = _parent.GetVariableUnit(texts[4], "WORD");
                    this._args5 = _parent.GetVariableUnit(texts[5], "WORD");
                    break;
                // (rS)
                case "FUNC":
                    //this.flag1 = args[1];
                    break;
            }
            Update();
        }

        public override void Update()
        {
            TopTextBlock.Text = Inst;
            if (_args1 != null)
            {
                MiddleTextBlock1.Text = _args1.ToString();
            }
            if (_args2 != null)
            {
                MiddleTextBlock2.Text = _args2.ToString();
            }
            if (_args3 != null)
            {
                MiddleTextBlock3.Text = _args3.ToString();
            }
            if (_args4 != null)
            {
                MiddleTextBlock4.Text = _args4.ToString();
            }
            if (_args5 != null)
            {
                MiddleTextBlock1.Text = _args5.ToString();
            }
            
        }
    }
}
