using SamSoarII.UserInterface;
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
                // (rW, wD)
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
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    break;
                // (rD, wD)
                case "INVD":
                case "MOVD":
                case "INCD":
                case "DECD":
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
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    // T/C + 地址
                    this[3] = _parent.GetVariableUnit(texts[1][0] + texts[1].Substring(2), "BIT");
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
                    this[1] = _parent.GetVariableUnit(texts[1], "WORD");
                    this[2] = _parent.GetVariableUnit(texts[2], "WORD");
                    this[3] = _parent.GetVariableUnit(texts[3], "WORD");
                    this[4] = _parent.GetVariableUnit(texts[4], "WORD");
                    this[5] = _parent.GetVariableUnit(texts[5], "WORD");
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
                MiddleTextBlock1.Text = this[5].ToString();
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
                // (InputValue, OutputValue)
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
                    labels = new string[2];
                    values = new string[2];
                    types = new string[2];
                    labels[0] = String.Format("{0:s}(InputValue)", this[1].Name);
                    labels[1] = String.Format("{0:s}(OutputValue)", this[2].Name);
                    values[0] = String.Empty;
                    values[1] = String.Empty;
                    types[0] = this[1].Type;
                    types[1] = this[2].Type;
                    if (this[1].Islocked)
                        values[0] = this[1].Value.ToString();
                    if (this[2].Islocked)
                        values[1] = this[2].Value.ToString();
                    break;
                // (InputValue1, InputValue2, OutputValue)
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
                    labels = new string[3];
                    values = new string[3];
                    types = new string[3];
                    labels[0] = String.Format("{0:s}(InputValue1)", this[1].Name);
                    labels[1] = String.Format("{0:s}(InputValue2)", this[2].Name);
                    labels[2] = String.Format("{0:s}(OutputValue)", this[3].Name);
                    values[0] = String.Empty;
                    values[1] = String.Empty;
                    values[2] = String.Empty;
                    types[0] = this[1].Type;
                    types[1] = this[2].Type;
                    types[2] = this[3].Type;
                    if (this[1].Islocked)
                        values[0] = this[1].Value.ToString();
                    if (this[2].Islocked)
                        values[1] = this[2].Value.ToString();
                    if (this[3].Islocked)
                        values[2] = this[3].Value.ToString();
                    break;
                // (SourceValue, DestinationValue, Count)
                case "MVBLK":
                case "FMOV":
                case "MVDBLK":
                case "FMOVD":
                    labels = new string[3];
                    values = new string[3];
                    types = new string[3];
                    labels[0] = String.Format("{0:s}(SourceValue)", this[1].Name);
                    labels[1] = String.Format("{0:s}(DestinationValue)", this[2].Name);
                    labels[2] = String.Format("{0:s}(Count)", this[3].Name);
                    values[0] = String.Empty;
                    values[1] = String.Empty;
                    values[2] = String.Empty;
                    types[0] = this[1].Type;
                    types[1] = this[2].Type;
                    types[2] = this[3].Type;
                    if (this[1].Islocked)
                        values[0] = this[1].Value.ToString();
                    if (this[2].Islocked)
                        values[1] = this[2].Value.ToString();
                    if (this[3].Islocked)
                        values[2] = this[3].Value.ToString();
                    break;
                // (rwW, rW, rwB)
                case "TON":
                case "TONR":
                case "TOF":
                case "CTU":
                case "CTD":
                case "CTUD":
                    return;
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
