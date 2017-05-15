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

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// MicroViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MicroViewModel : UserControl
    {
        private BaseViewModel bvmodel;
        public BaseViewModel Prototype
        {
            get { return this.bvmodel; }
        }
        
        public int X
        {
            get { return Grid.GetColumn(this); }
            set { Grid.SetColumn(this, value); }
        }

        public int Y
        {
            get { return Grid.GetRow(this); }
            set { Grid.SetRow(this, value); }
        }

        public MicroViewModel()
        {
            InitializeComponent();
        }
        
        public MicroViewModel(BaseViewModel _bvmodel, bool linkup, bool linkdown)
        {
            InitializeComponent();
            bvmodel = _bvmodel;
            L_LeftUp.Visibility = linkup ? Visibility.Visible : Visibility.Hidden;
            L_LeftDown.Visibility = linkdown ? Visibility.Visible : Visibility.Hidden;
            if (bvmodel is InputBaseViewModel)
            {
                L_Input.Visibility = Visibility.Visible;
                L_Output.Visibility = Visibility.Visible;
                L_Left.Visibility = Visibility.Visible;
                L_Right.Visibility = Visibility.Visible;
                TB_Huge.Visibility = Visibility.Visible;
                switch (bvmodel.InstructionName)
                {
                    case "LD": break;
                    case "LDI": TB_Huge.Text = "/"; break;
                    case "LDIM": TB_Huge.Text = "|"; break;
                    case "LDIIM": TB_Huge.Text = "/|"; break;
                    case "LDP": TB_Huge.Text = "↑"; break;
                    case "LDF": TB_Huge.Text = "↓"; break;
                    case "LDWEQ": case "LDDEQ": case "LDFEQ": TB_Huge.Text = "＝"; break;
                    case "LDWNE": case "LDDNE": case "LDFNE": TB_Huge.Text = "≠"; break;
                    case "LDWLE": case "LDDLE": case "LDFLE": TB_Huge.Text = "≤"; break;
                    case "LDWGE": case "LDDGE": case "LDFGE": TB_Huge.Text = "≥"; break;
                    case "LDWL": case "LDDL": case "LDFL": TB_Huge.Text = "＜"; break;
                    case "LDWG": case "LDDG": case "LDFG": TB_Huge.Text = "＞"; break;
                }
            }
            else if (bvmodel is OutputBaseViewModel)
            {
                L_Input.Visibility = Visibility.Visible;
                P_Left.Visibility = Visibility.Visible;
                P_Right.Visibility = Visibility.Visible;
                TB_Huge.Visibility = Visibility.Visible;
                switch (bvmodel.InstructionName)
                {
                    case "OUT": break;
                    case "OUTIM": TB_Huge.Text = "|"; break;
                    case "RST": TB_Huge.Text = "R"; break;
                    case "RSTIM": TB_Huge.Text = "RI"; break;
                    case "SET": TB_Huge.Text = "S"; break;
                    case "SETIM": TB_Huge.Text = "SI"; break;
                }
            }
            else if (bvmodel is OutputRectBaseViewModel)
            {
                L_Input.Visibility = Visibility.Visible;
                L_Left.Visibility = Visibility.Visible;
                L_Right.Visibility = Visibility.Visible;
                L_Top.Visibility = Visibility.Visible;
                L_Down.Visibility = Visibility.Visible;
                TB_Huge.Visibility = Visibility.Visible;
                TB_Inst.Visibility = Visibility.Visible;
                switch (bvmodel.InstructionName)
                {
                    case "ADD": case "ADDD": case "ADDF": TB_Huge.Text = "＋"; break;
                    case "SUB": case "SUBD": case "SUBF": TB_Huge.Text = "－"; break;
                    case "MUL": case "MULW": case "MULD": case "MULF": TB_Huge.Text = "×"; break;
                    case "DIV": case "DIVW": case "DIVD": case "DIVF": TB_Huge.Text = "÷"; break;
                    case "INC": case "INCD": TB_Huge.Text = "++"; break;
                    case "DEC": case "DECD": TB_Huge.Text = "--"; break;
                    case "TON": case "TONR": case "TOF": TB_Huge.Text = "T"; break;
                    case "CTU": case "CTUD": case "CTD": TB_Huge.Text = "C"; break;
                    case "SHL": case "SHLD": case "SHLB": case "ROL": case "ROLD": TB_Huge.Text = "<<"; break;
                    case "SHR": case "SHRD": case "SHRB": case "ROR": case "RORD": TB_Huge.Text = "<<"; break;
                    case "PLSF": case "DPLSF": case "PWM": case "DPWM": 
                    case "PLSY": case "DPLSY":  case "PLSR": case "DPLSR": 
                    case "PLSRD": case "DPLSRD": case "PLSNEXT": case "PLSSTOP": 
                    case "ZRN": case "DZRN": case "PTO": case "DRVI": case "DDRVI":
                        TB_Huge.Text = "P"; break;
                    case "AND": case "ANDD": TB_Huge.Text = "A"; break;
                    case "OR": case "ORD": TB_Huge.Text = "O"; break;
                    case "XOR": case "XORD": TB_Huge.Text = "X"; break;
                    default: TB_Inst.Text = bvmodel.InstructionName; break;
                }
            }
            else if (bvmodel is SpecialBaseViewModel)
            {
                L_Input.Visibility = Visibility.Visible;
                L_Output.Visibility = Visibility.Visible;
                L_Inner.Visibility = Visibility.Visible;
                TB_Huge.Visibility = Visibility.Visible;
                switch (bvmodel.InstructionName)
                {
                    case "INV": TB_Huge.Text = "/"; break;
                    case "MEP": TB_Huge.Text = "↑"; break;
                    case "MEF": TB_Huge.Text = "↓"; break;
                }
            }
        }
    }
}
