using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public static class ValueCommentManager
    {
        private static Dictionary<string, string> _valueCommentDict;

        public static event ValueCommentChangedEventHandler ValueCommentChanged = delegate { };

        public static void Initialize()
        {
            _valueCommentDict = new Dictionary<string, string>();
        }
        public static void Clear()
        {
            _valueCommentDict.Clear();
            ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Clear, string.Empty, string.Empty));
        }

        public static bool ContainValue(IValueModel value)
        {
            return ContainValue(value.ValueString);
        }

        public static bool ContainValue(string valueString)
        {
            if (valueString != string.Empty && _valueCommentDict.ContainsKey(valueString))
            {
                return true;
            }
            return false;
        }
        public static void UpdateComment(string valueString, string comment)
        {
            if (ContainValue(valueString))
            {
                if (comment != string.Empty)
                {
                    _valueCommentDict[valueString] = comment;
                    ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Update, valueString, comment));
                }
                else
                {
                    _valueCommentDict.Remove(valueString);
                    ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Remove, valueString, comment));
                }
            }
            else
            {
                if (valueString != string.Empty && comment != string.Empty)
                {
                    _valueCommentDict.Add(valueString,comment);
                    ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Add, valueString, comment));
                }
            }
        }
        public static void Delete(string valueString)
        {
            DeleteValueString(valueString);
        }
        public static void DeleteValueString(string valueString)
        {
            if (_valueCommentDict.ContainsKey(valueString))
            {
                _valueCommentDict.Remove(valueString);
                ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Remove, valueString, string.Empty));
            }
        }
        public static void UpdateComment(IValueModel value, string comment)
        {
            UpdateComment(value.ValueBaseString, comment);
        }

        public static string GetComment(IValueModel value)
        {
            return GetComment(value.ValueBaseString);
        }

        public static string GetComment(string valueString)
        {
            if (_valueCommentDict.ContainsKey(valueString))
            {
                return _valueCommentDict[valueString];
            }
            return string.Empty;
        }
        
        public static IDictionary<string, string> ValueCommentDict { get { return _valueCommentDict; } }
    }
}
