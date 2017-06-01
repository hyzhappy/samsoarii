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

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementValueModifyPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ElementValueModifyPanel : UserControl
    {
        public ElementValueModifyPanel()
        {
            InitializeComponent();
        }

        private string varname = String.Empty;
        public string VarName
        {
            get
            {
                return this.varname;
            }
            set
            {
                this.varname = value;
                TB_Name.Text = String.Format("{0:s} = ", varname);
                switch (varname[0])
                {
                    case 'X': case 'Y':
                        InitializeForce();
                        break;
                    case 'S': case 'M':
                        InitializeBit();
                        break;
                    case 'C': case 'T':
                        if (varname[1] == 'V')
                            InitializeWord();
                        else
                            InitializeBit();
                        break;
                    default:
                        InitializeWord();
                        break;
                }
                CB_Type.SelectedIndex = 0;
            }
        }

        private string vartype = String.Empty;
        public string VarType
        {
            get
            {
                return this.vartype;
            }
            set
            {
                this.vartype = value;
                int id = CB_Type.Items.IndexOf(vartype);
                if (id >= 0) CB_Type.SelectedIndex = id;
            }
        }

        public string Value
        {
            get
            {
                return TB_Value.Text;
            }
            set
            {
                TB_Value.Text = value;
            }
        }
        
        private void InitializeForce()
        {
            TB_Value.IsReadOnly = true;
            CB_Type.Items.Add("BOOL");
            BT_FON.Visibility = BT_FOFF.Visibility = BT_CF.Visibility = BT_CFA.Visibility
                = Visibility.Visible;
        }
        private void InitializeBit()
        {
            TB_Value.IsReadOnly = true;
            CB_Type.Items.Add("BOOL");
            BT_WON.Visibility = BT_WOFF.Visibility
                = Visibility.Visible;
        }
        private void InitializeWord()
        {
            CB_Type.Items.Add("WORD");
            CB_Type.Items.Add("UWORD");
            CB_Type.Items.Add("DWORD");
            CB_Type.Items.Add("UDWORD");
            CB_Type.Items.Add("BCD");
            CB_Type.Items.Add("FLOAT");
            BT_Write.Visibility = Visibility.Visible;
        }
        
        public event ElementValueModifyEventHandler ValueModify = delegate { };
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (TB_Value.Background == Brushes.Red)
            {
                return;
            }
            if (sender == BT_FON)
            {
                ValueModify(this, new ElementValueModifyEventArgs(
                    VarName, VarType, "ON", ElementValueModifyEventType.ForceON));
            }
            if (sender == BT_FOFF)
            {
                ValueModify(this, new ElementValueModifyEventArgs(
                    VarName, VarType, "OFF", ElementValueModifyEventType.ForceOFF));
            }
            if (sender == BT_CF)
            {
                ValueModify(this, new ElementValueModifyEventArgs(
                    VarName, VarType, String.Empty, ElementValueModifyEventType.ForceCancel));
            }
            if (sender == BT_CFA)
            {
                ValueModify(this, new ElementValueModifyEventArgs
                    (VarName, VarType, String.Empty, ElementValueModifyEventType.AllCancel));
            }
            if (sender == BT_WON)
            {
                ValueModify(this, new ElementValueModifyEventArgs(
                    VarName, VarType, "ON", ElementValueModifyEventType.WriteON));
            }
            if (sender == BT_WOFF)
            {
                ValueModify(this, new ElementValueModifyEventArgs(
                    VarName, VarType, "OFF", ElementValueModifyEventType.WriteOFF));
            }
            if (sender == BT_Write)
            {
                ValueModify(this, new ElementValueModifyEventArgs(
                    VarName, VarType, TB_Value.Text, ElementValueModifyEventType.Write));
            }
        }

        private void CB_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vartype = e.AddedItems[0].ToString();
            CheckValue();
        }
        private void TB_Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValue();
        }
        private void CheckValue()
        {
            try
            {
                switch (CB_Type.Text)
                {
                    case "BIT":
                        switch (TB_Value.Text)
                        {
                            case "ON":
                            case "OFF":
                                break;
                            default:
                                TB_Value.Background = Brushes.Red;
                                return;
                        }
                        break;
                    case "WORD":
                        Int16.Parse(TB_Value.Text);
                        break;
                    case "UWORD":
                        UInt16.Parse(TB_Value.Text);
                        break;
                    case "DWORD":
                        Int32.Parse(TB_Value.Text);
                        break;
                    case "UDWORD":
                        UInt32.Parse(TB_Value.Text);
                        break;
                    case "BCD":
                        if (TB_Value.Text.Length > 4)
                        {
                            TB_Value.Background = Brushes.Red;
                            return;
                        }
                        Int16.Parse(TB_Value.Text);
                        break;
                    case "FLOAT":
                        float.Parse(TB_Value.Text);
                        break;
                }
                TB_Value.Background = Brushes.White;
            }
            catch (Exception)
            {
                TB_Value.Background = Brushes.Red;
            }
        }
    }

    public delegate void ElementValueModifyEventHandler(object sender, ElementValueModifyEventArgs e);
    public enum ElementValueModifyEventType
    {
        ForceON, ForceOFF, ForceCancel, AllCancel, WriteON, WriteOFF, Write
    }
    public class ElementValueModifyEventArgs : EventArgs
    {
        public string VarName { get; private set; }
        public string VarType { get; private set; }
        public string Value { get; private set; }
        public ElementValueModifyEventType Type { get; private set; }
        public ElementValueModifyEventArgs(
            string _varname, string _vartype, string _value,
            ElementValueModifyEventType _type)
        {
            VarName = _varname;
            VarType = _vartype;
            Value = _value;
            Type = _type;
        }
    }
}
