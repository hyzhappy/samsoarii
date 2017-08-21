using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SamSoarII.Core.Models;
using System.ComponentModel;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// PIDDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PIDDialog : Window, IDisposable, INotifyPropertyChanged
    {
        public PIDDialog(InteractionFacade _ifparent, LadderUnitModel _core)
        {
            InitializeComponent();
            ifparent = _ifparent;
            elements = new PIDTempElement[]
            {
                new PIDTempElement(this, 0, "比例增益KP", "输入", "FLOAT"),
                new PIDTempElement(this, 2, "积分时间TI(ms)", "输入", "FLOAT"),
                new PIDTempElement(this, 4, "微分时间TD(ms)", "输入", "FLOAT"),
                new PIDTempElement(this, 6, "PID控制死区", "输入", "FLOAT"),
                new PIDTempElement(this, 8, "采样时间(ms)", "输入", "FLOAT"),
                new PIDTempElement(this, 10, "给定值低限", "输入", "FLOAT"),
                new PIDTempElement(this, 12, "给定值高限", "输入", "FLOAT"),
                new PIDTempElement(this, 14, "方向设置", "输入", "[0 - 1]"),
                new PIDTempElement(this, 15, "自整定状态", "输入/输出", "[0 - 2]"),
                new PIDTempElement(this, 16, "整定采样点数", "输入", "WORD"),
                new PIDTempElement(this, 17, "PID输出量", "输出", "DWORD")
            };
            Core = _core;
            DataContext = this;
        }

        public void Dispose()
        {
            ifparent = null;
            Core = null;
            foreach (PIDTempElement element in elements)
                element.Dispose();
            elements = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private InteractionFacade ifparent;
        public InteractionFacade IFParent { get { return this.ifparent; } }

        private LadderUnitModel core;
        public LadderUnitModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core != null)
                {
                    if (temp != null) temp.Dispose();
                }
                this.core = value;
                if (core != null)
                {
                    temp = core.Children[2].Clone();
                    if (!core.Children[0].Text.Equals("???"))
                        TB_Loop.Text = core.Children[0].Text;
                    if (!core.Children[1].Text.Equals("???"))
                        CB_Mode.SelectedIndex = (int)(core.Children[1].Value);
                    else
                        CB_Mode.SelectedIndex = 0;
                    if (!core.Children[2].Text.Equals("???"))
                        TB_Temp.Text = core.Children[2].Text;
                    if (!core.Children[3].Text.Equals("???"))
                        TB_In.Text = core.Children[3].Text;
                    if (!core.Children[4].Text.Equals("???"))
                        TB_Out.Text = core.Children[4].Text;
                    if (!core.Children[5].Text.Equals("???"))
                        TB_SV.Text = core.Children[5].Text;
                }
            }
        }

        private PIDTempElement[] elements;
        public IList<PIDTempElement> Elements { get { return this.elements; } }

        private ValueModel temp;
        public ValueModel Temp { get { return this.temp; } }

        public IList<string> PropertyStrings
        {
            get
            {
                string[] strings = new string[12];
                strings[0] = TB_Loop.Text;
                strings[2] = String.Format("K{0:d}", CB_Mode.SelectedIndex);
                strings[4] = TB_Temp.Text;
                strings[6] = TB_In.Text;
                strings[8] = TB_Out.Text;
                strings[10] = TB_SV.Text;
                for (int i = 1; i < strings.Length; i += 2)
                    strings[i] = ifparent.MNGValue[strings[i - 1]].Comment;
                return strings;
            }
        }

        #endregion

        #region Event Handler

        private void TB_Temp_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { temp.Text = TB_Temp.Text; } catch (ValueParseException) { }
            foreach (PIDTempElement element in elements)
                element.InvokePropertyChanged("ValueString");
        }

        public event RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Ensure(this, e);
        }

        public event RoutedEventHandler Cancel = delegate { };
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Cancel(this, e);
        }


        #endregion
    }

    public class PIDTempElement : IDisposable, INotifyPropertyChanged
    {
        public PIDTempElement(PIDDialog _parent, int _offset, string _argument, string _vartype, string _range)
        {
            parent = _parent;
            offset = _offset;
            argument = _argument;
            vartype = _vartype;
            range = _range;
        }

        public void Dispose()
        {
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void InvokePropertyChanged(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        #region Number

        private PIDDialog parent;
        public PIDDialog Parent { get { return this.parent; } }

        private int offset;
        public string ValueString
        {
            get
            {
                if (parent.Temp == null) return "";
                if (parent.Temp.Base == ValueModel.Bases.NULL) return "";
                switch (parent.Temp.Intra)
                {
                    case ValueModel.Bases.V:
                    case ValueModel.Bases.Z:
                        return String.Format("{0:s}{1:d}{2:s}{3:d}",
                            ValueModel.NameOfBases[(int)(parent.Temp.Base)],
                            parent.Temp.Offset + offset,
                            ValueModel.NameOfBases[(int)(parent.Temp.Intra)],
                            parent.Temp.IntraOffset);
                    default:
                        return String.Format("{0:s}{1:d}",
                            ValueModel.NameOfBases[(int)(parent.Temp.Base)],
                            parent.Temp.Offset + offset);
                }
            }
        }

        private string argument;
        public string Argument { get { return this.argument; } }

        private string vartype;
        public string VarType { get { return this.vartype; } }

        private string range;
        public string Range { get { return this.range; } }

        #endregion
    }
}
