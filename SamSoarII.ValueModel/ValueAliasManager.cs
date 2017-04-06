using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class ValueAliasManager
    {
        private static Dictionary<string, string> _valueAliasDict;
        public static event ValueAliasChangedEventHandler ValueAliasChanged = delegate { };
        public static void Initialize()
        {
            _valueAliasDict = new Dictionary<string, string>();
        }
        public static void Clear()
        {
            _valueAliasDict.Clear();
            ValueAliasChanged.Invoke(new ValueAliasChangedEventArgs(ValueChangedType.Clear, string.Empty, string.Empty));
        }
        public static bool ContainValue(string valueString)
        {
            if (valueString != string.Empty && _valueAliasDict.ContainsKey(valueString))
            {
                return true;
            }
            return false;
        }
        public static void UpdateAlias(string valueString, string alias)
        {
            if (ContainValue(valueString))
            {
                if (alias != string.Empty)
                {
                    _valueAliasDict[valueString] = alias;
                    ValueAliasChanged.Invoke(new ValueAliasChangedEventArgs(ValueChangedType.Update, valueString, alias));
                }
                else
                {
                    _valueAliasDict.Remove(valueString);
                    ValueAliasChanged.Invoke(new ValueAliasChangedEventArgs(ValueChangedType.Remove, valueString, alias));
                }
            }
            else
            {
                if (valueString != string.Empty && alias != string.Empty)
                {
                    _valueAliasDict.Add(valueString, alias);
                    ValueAliasChanged.Invoke(new ValueAliasChangedEventArgs(ValueChangedType.Add, valueString, alias));
                }
            }
        }
        public static void Delete(string valueString)
        {
            DeleteValueString(valueString);
        }
        public static void DeleteValueString(string valueString)
        {
            if (_valueAliasDict.ContainsKey(valueString))
            {
                _valueAliasDict.Remove(valueString);
                ValueAliasChanged.Invoke(new ValueAliasChangedEventArgs(ValueChangedType.Remove, valueString, string.Empty));
            }
        }
        public static void UpdateAlias(IValueModel value, string comment)
        {
            UpdateAlias(value.ValueString, comment);
        }
        public static bool CheckAlias(string valueString,string alias)
        {
            return !ValueAliasDict.Any(x => { return x.Key != valueString && x.Value == alias; });
        }
        public static IDictionary<string, string> ValueAliasDict { get { return _valueAliasDict; } }
    }
}
