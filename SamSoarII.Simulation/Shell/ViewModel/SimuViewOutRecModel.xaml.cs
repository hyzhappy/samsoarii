using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.UI;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
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
                // ()
                case "NEXT":
                case "EI":
                case "DI":
                    break;
                // (wP)
                case "PLSNEXT":
                case "PLSSTOP":
                    this[1] = _parent.GetVariableUnit(texts[1], "PULSE");
                    break;
                // (rW)
                case "DTCH":
                case "FOR":
                case "JMP":
                case "LBL":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    break;
                // (rW, wD)
                case "FACT":
                case "WTOD":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
                    break;
                // (rD, wF)
                case "DTOW":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    break;
                // (rD, wF)
                case "DTOF":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "FLOAT");
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
                    this[1] = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
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
                case "XCH":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    break;
                // (rD, wD)
                case "INVD":
                case "MOVD":
                case "INCD":
                case "DECD":
                case "NEGD":
                case "CMLD":
                case "XCHD":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
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
                case "XCHF":
                    this[1] = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this[2] = _parent.GetVariableUnit(texts[2], "FLOAT");
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
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
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
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "DWORD");
                    break;
                // (rW, rW, wD)
                case "MUL":
                case "DIV":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "DWORD");
                    break;
                // (rF, rF, wF)
                case "ADDF":
                case "SUBF":
                case "MULF":
                case "DIVF":
                    this[1] = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this[2] = _parent.GetVariableUnit(texts[2], "FLOAT");
                    this[3] = _parent.GetVariableUnit(texts[3], "FLOAT");
                    break;
                // (rW, wW, rW)
                case "MVBLK":
                case "FMOV":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
                    break;
                // (rD, wD, rD)
                case "MVDBLK":
                case "FMOVD":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "DWORD");
                    break;
                // (rW, rW, wB)
                case "CMP":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "BIT");
                    break;
                // (rD, rD, wB)
                case "CMPD":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "BIT");
                    break;
                // (rF, rF, wB)
                case "CMPF":
                    this[1] = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this[2] = _parent.GetVariableUnit(texts[2], "FLOAT");
                    this[3] = _parent.GetVariableUnit(texts[3], "BIT");
                    break;
                // (rW, rW, rW, wB)
                case "ZCP":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
                    this[4] = _parent.GetVariableUnit(texts[4], "BIT");
                    break;
                // (rD, rD, rD, wB)
                case "ZCPD":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "DWORD");
                    this[4] = _parent.GetVariableUnit(texts[4], "BIT");
                    break;
                // (rF, rF, rF, wB)
                case "ZCPF":
                    this[1] = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this[2] = _parent.GetVariableUnit(texts[2], "FLOAT");
                    this[3] = _parent.GetVariableUnit(texts[3], "FLOAT");
                    this[4] = _parent.GetVariableUnit(texts[4], "BIT");
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
                case "HCNT":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    // T/C + 地址
                    //this[3] = _parent.GetVariableUnit(texts[1][0] + texts[1].Substring(2), "BIT");
                    break;
                // (rS, rwW, rwB)
                /*
                 * 调用c程序比较特殊，因为要指定c程序的名称
                 * 所以第一个参数为名称，剩下两个分别为D参数和M参数
                 */
                case "CALLM":
                    this[1] = new SimulateStringUnit(texts[1]);
                    IEnumerable<FuncHeaderModel> fit = SimuParent.AllFuncs.Headers.Where
                    (
                        (FuncHeaderModel _fhmodel) =>
                        {
                            return _fhmodel.Name.Equals(texts[1]);
                        }
                    );
                    if (fit.Count() == 0)
                    {
                        throw new ArgumentException(String.Format("Cannot found function {0:s}", texts[1]));
                    }
                    FuncHeaderModel fhmodel = fit.First();
                    for (int i = 0; i < fhmodel.ArgCount; i++)
                    {
                        string type = fhmodel.GetArgType(i);
                        switch (type)
                        {
                            case "_BIT*":
                                type = "BIT";
                                break;
                            case "_WORD*":
                                type = "WORD";
                                break;
                            case "D_WORD*":
                                type = "DWORD";
                                break;
                            case "_FLOAT*":
                                type = "FLOAT";
                                break;
                        }
                        this[i + 2] = _parent.GetVariableUnit(texts[i + 2], type);
                    }
                    break;
                // (rS)
                case "CALL":
                    this[1] = new SimulateStringUnit(texts[1]);
                    break;
                // (rW, rS)
                case "ATCH":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = new SimulateStringUnit(texts[2]);
                    break;
                // (rW, rW, rW, wW, rW)
                case "SMOV":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
                    this[4] = _parent.GetVariableUnit(texts[4], "WORD");
                    this[5] = _parent.GetVariableUnit(texts[5], "WORD");
                    break;
                // (rW, rS, rwW)
                case "MBUS":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = new SimulateStringUnit(texts[2]);
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
                    break;
                // (rW, rW, rW)
                case "SEND":
                case "REV":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
                    break;
                // (rW, wB)
                case "PLSF":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "PULSE");
                    break;
                // (rD, wB)
                case "DPLSF":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "PULSE");
                    break;
                // (rW, rW, wB)
                case "PWM":
                case "PLSY":
                case "PLSR":
                case "ZRN":
                case "DRVI":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "PULSE");
                    break;
                // (rD, rD, wB)
                case "DPWM":
                case "DPLSY":
                case "DPLSR":
                case "DZRN":
                case "DDRVI":
                    this[1] = _parent.GetVariableUnit(texts[1], "DWORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "DWORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "PULSE");
                    break;
                // (rW, rW, wB, wB)
                case "PLSRD":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "PULSE");
                    this[4] = _parent.GetVariableUnit(texts[4], "PULSE");
                    break;
                // (rD, rD, wB, wB)
                case "DPLSRD":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "PULSE");
                    this[4] = _parent.GetVariableUnit(texts[4], "PULSE");
                    break;
                // (rW, wB, wB)
                case "PTO":
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "PULSE");
                    this[3] = _parent.GetVariableUnit(texts[3], "PULSE");
                    break;

            }
            Update();
        }

        public override void Update()
        {
            Dispatcher.Invoke(_Update);
        }

        private void _Update()
        {
            TopTextBlock.Text = Inst;
            if (this[1] != null)
            {
                MiddleTextBlock1.Text = this[1].ToString();
            }
            if (this[2] != null)
            {
                MiddleTextBlock2.Text = this[2].ToString();
            }
            if (this[3] != null)
            {
                MiddleTextBlock3.Text = this[3].ToString();
            }
            if (this[4] != null)
            {
                MiddleTextBlock4.Text = this[4].ToString();
            }
            if (this[5] != null)
            {
                MiddleTextBlock5.Text = this[5].ToString();
            }
        }
        
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (dialog != null)
            {
                return;
            }
            string[] labels = null;
            string[] values = null;
            string[] types = null;
            switch (Inst)
            {
                // (IN, OUT)
                case "WTOD":
                case "DTOW":
                case "DTOF":
                case "ROUND":
                case "TURNC":
                case "BIN":
                case "BCD":
                case "INVW":
                case "MOV":
                case "INC":
                case "DEC":
                case "INVD":
                case "MOVD":
                case "INCD":
                case "DECD":
                case "MOVF":
                case "SQRT":
                case "SIN":
                case "COS":
                case "TAN":
                case "LN":
                case "EXP":
                case "LOG":
                case "FACT":
                case "CML":
                case "CMLD":
                case "NEG":
                case "NEGD":
                    labels = new string[2];
                    values = new string[2];
                    types = new string[2];
                    labels[0] = "IN";
                    labels[1] = "OUT";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (OUT1, OUT2)
                case "XCH":
                case "XCHD":
                case "XCHF":
                    labels = new string[2];
                    values = new string[2];
                    types = new string[2];
                    labels[0] = "OUT1";
                    labels[1] = "OUT2";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (IN1, IN2, OUT)
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
                case "MUL":
                case "DIV":
                case "ADDF":
                case "SUBF":
                case "MULF":
                case "DIVF":
                case "POW":
                case "CMP":
                case "CMPD":
                case "CMPF":
                    labels = new string[3];
                    values = new string[3];
                    types = new string[3];
                    labels[0] = "IN1";
                    labels[1] = "IN2";
                    labels[2] = "OUT";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (IN, IN1, IN2, OUT)
                case "ZCP":
                case "ZCPD":
                case "ZCPF":
                    labels = new string[4];
                    values = new string[4];
                    types = new string[4];
                    labels[0] = "IN";
                    labels[1] = "IN1";
                    labels[2] = "IN2";
                    labels[3] = "OUT";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (S, D, CT)
                case "MVBLK":
                case "FMOV":
                case "MVDBLK":
                case "FMOVD":
                    labels = new string[3];
                    values = new string[3];
                    types = new string[3];
                    labels[0] = "S";
                    labels[1] = "D";
                    labels[2] = "CT";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (TV, SV)
                case "TON":
                case "TONR":
                case "TOF":
                    labels = new string[2];
                    values = new string[2];
                    types = new string[2];
                    labels[0] = "TV";
                    labels[1] = "SV";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (CV, SV)
                case "CTU":
                case "CTD":
                case "CTUD":
                case "HCNT":
                    labels = new string[2];
                    values = new string[2];
                    types = new string[2];
                    labels[0] = "CV";
                    labels[1] = "TV";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (FEQ, OUT)
                case "PLSF":
                case "DPLSF":
                    labels = new string[2];
                    values = new string[2];
                    types = new string[2];
                    labels[0] = "FEQ";
                    labels[1] = "OUT";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (FEQ, DF, OUT)
                case "PWM":
                case "DPWM":
                    labels = new string[3];
                    values = new string[3];
                    types = new string[3];
                    labels[0] = "FEQ";
                    labels[1] = "DF";
                    labels[2] = "OUT";
                    _SetDialogProperty(labels, values, types);
                    break;
                // (FEQ, PN, OUT)
                case "PLSY":
                case "DPLSY":
                    labels = new string[3];
                    values = new string[3];
                    types = new string[3];
                    labels[0] = "FEQ";
                    labels[1] = "PN";
                    labels[2] = "OUT";
                    _SetDialogProperty(labels, values, types);
                    break;
                default:
                    return;
            }
            dialog = new SimuArgsDialog(labels, values, types);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.EnsureClick += OnDialogEnsureClicked;
            dialog.CancelClick += OnDialogCancelClicked;
            dialog.ShowDialog();
        }
    }
}
