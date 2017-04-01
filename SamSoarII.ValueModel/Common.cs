using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public enum ValueChangedType
    {
        Clear,
        Remove,
        Add,
        Update,
    }
    public class ValueCommentChangedEventArgs : EventArgs
    {
        public ValueChangedType Type { get; set; }
        public string ValueString { get; set; }
        public string Comment { get; set; }
        public ValueCommentChangedEventArgs(ValueChangedType type, string valuestring, string comment)
        {
            Type = type;
            ValueString = valuestring;
            Comment = comment;
        }
    }
    public delegate void ValueCommentChangedEventHandler(ValueCommentChangedEventArgs e);
    public class ValueAliasChangedEventArgs : EventArgs
    {
        public ValueChangedType Type { get; set; }
        public string ValueString { get; set; }
        public string Alias { get; set; }
        public ValueAliasChangedEventArgs(ValueChangedType type, string valuestring, string alias)
        {
            Type = type;
            ValueString = valuestring;
            Alias = alias;
        }
    }
    public delegate void ValueAliasChangedEventHandler(ValueAliasChangedEventArgs e);
    public class VariableChangedEventArgs : EventArgs
    {
        public ValueChangedType ChangedType { get; set; }
        public IVariableValue Variable { get; set; }
        public VariableChangedEventArgs(ValueChangedType type, IVariableValue variable)
        {
            ChangedType = type;
            Variable = variable;
        }
    }

    public delegate void VariableChangedEventHandler(VariableChangedEventArgs e);
}
