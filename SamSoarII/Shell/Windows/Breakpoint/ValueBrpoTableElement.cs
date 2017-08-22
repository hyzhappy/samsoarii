using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;

using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Windows
{
    public class ValueBrpoTableElement : INotifyPropertyChanged
    {
        public ValueBrpoTableElement(ValueBrpoElement _core)
        {
            Core = _core;
            leftvalue = "";
            rightvalue = "";
            operation = "";
            valuetype = "";
        }

        public void Dispose()
        {
            Core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ValueBrpoElement core;
        public ValueBrpoElement Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                ValueBrpoElement _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsActive":
                    PropertyChanged(this, new PropertyChangedEventArgs("ActiveInfo"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ActiveBrush"));
                    break;
                default:
                    LoadFromCore();
                    break;
            }
        }
        
        public string ActiveInfo { get { return core == null || !core.IsValid ? "非法" : core.IsActive ? "启用" : "禁用"; } }
        public Brush ActiveBrush { get { return core != null && core.IsValid && core.IsActive ? Brushes.Red : Brushes.Gray; } }

        private string leftvalue;
        public string LeftValue
        {
            get { return this.leftvalue; }
            set { this.leftvalue = value; PropertyChanged(this, new PropertyChangedEventArgs("LeftValue")); }
        }

        private string rightvalue;
        public string RightValue
        {
            get { return this.rightvalue; }
            set { this.rightvalue = value; PropertyChanged(this, new PropertyChangedEventArgs("RightValue")); }
        }

        private string operation;
        public string Operation
        {
            get { return this.operation; }
            set { this.operation = value; PropertyChanged(this, new PropertyChangedEventArgs("Operation")); }
        }

        static private string[] BitOperations = { "上升沿", "下降沿", "变化", "＝", "≠"};
        static private string[] WordOperations = { "变化", "＝", "≠", "＞", "≥", "＜", "≤"};
        public IEnumerable<string> Operations
        {
            get
            {
                if (core != null)
                {
                    if (core.Type == ValueModel.Types.BOOL)
                        return BitOperations;
                    else
                        return WordOperations;
                }
                else
                {
                    return new string[] { };
                }
            }
        }

        private string valuetype;
        public string ValueType
        {
            get { return this.valuetype; }
            set { this.valuetype = value; PropertyChanged(this, new PropertyChangedEventArgs("ValueType")); }
        }

        static private string[] BitTypes = { "BOOL" };
        static private string[] WordTypes = { "WORD", "DWORD", "FLOAT" };
        public IEnumerable<string> ValueTypes
        {
            get
            {
                if (core != null)
                {
                    if (core.Type == ValueModel.Types.BOOL)
                        return BitTypes;
                    else
                        return WordTypes;
                }
                else
                {
                    return new string[] { };
                }
            }
        }

        #endregion

        public void LoadFromCore()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("Operations"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTypes"));
            PropertyChanged(this, new PropertyChangedEventArgs("ActiveInfo"));
            PropertyChanged(this, new PropertyChangedEventArgs("ActiveBrush"));
            if (core != null && core.IsValid)
            {
                LeftValue = core.LValue.Text.Equals("???") ? "" : core.LValue.Text;
                RightValue = core.RValue.Text.Equals("???") ? "" : core.RValue.Text;
                ValueType = ValueModel.NameOfTypes[(int)(core.Type)];
                if (ValueType.Equals("BOOL"))
                {
                    if (RightValue.Equals("K0")) RightValue = "OFF";
                    if (RightValue.Equals("K1")) RightValue = "ON";
                }
                switch (core.Oper)
                {
                    case ValueBrpoElement.Operators.UPEDGE: Operation = "上升沿"; break;
                    case ValueBrpoElement.Operators.DOWNEDGE: Operation = "下降沿"; break;
                    case ValueBrpoElement.Operators.CHANGED: Operation = "变化"; break;
                    case ValueBrpoElement.Operators.EQUAL: Operation = "＝"; break;
                    case ValueBrpoElement.Operators.NOTEQUAL: Operation = "≠"; break;
                    case ValueBrpoElement.Operators.LESS: Operation = "＜"; break;
                    case ValueBrpoElement.Operators.NOTLESS: Operation = "≥"; break;
                    case ValueBrpoElement.Operators.MORE: Operation = "＞"; break;
                    case ValueBrpoElement.Operators.NOTMORE: Operation = "≤"; break;
                }
            }
        }

        static private string[] AllValueTypes = {"BOOL", "WORD", "DWORD", "FLOAT"};
        public void SaveToCore()
        {
            try
            {
                core.Parse(
                    LeftValue.Length > 0 ? LeftValue : "???",
                    RightValue.Length > 0 ? RightValue : "???",
                    Operation,
                    ValueModel.TypeOfNames[ValueType]);
            }
            catch (Exception)
            {
                foreach (string _valuetype in AllValueTypes)
                {
                    try
                    {
                        core.Parse(
                            LeftValue.Length > 0 ? LeftValue : "???",
                            RightValue.Length > 0 ? RightValue : "???",
                            Operation,
                            ValueModel.TypeOfNames[_valuetype]);
                        break;
                    }
                    catch (ValueParseException)
                    {
                    }
                }
            }
        }
    }
}
