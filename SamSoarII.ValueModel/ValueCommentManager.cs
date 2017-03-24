using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public static class ValueCommentManager
    {
        private static Dictionary<string, Tuple<string,bool,bool>> _valueCommentDict;

        public static event ValueCommentChangedEventHandler ValueCommentChanged = delegate { };

        public static void Initialize()
        {
            _valueCommentDict = new Dictionary<string, Tuple<string, bool, bool>>();
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

        public static void UpdateComment(string valueString, string comment,bool isRelativeToVariable,bool isRelativeToValueModel)
        {
            if (valueString != string.Empty)
            {
                if (ContainValue(valueString))
                {
                    if (comment == string.Empty && (!ValueParser.IsVariablePattern(valueString)))
                    {
                        string tempstr = _valueCommentDict[valueString].Item1;
                        if (_valueCommentDict[valueString].Item2)
                        {
                            if (!_valueCommentDict[valueString].Item3)
                            {
                                _valueCommentDict[valueString] = new Tuple<string, bool, bool>(tempstr,true,isRelativeToValueModel);
                                if (isRelativeToValueModel)
                                {
                                    ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Add, valueString, tempstr));
                                }
                            }
                        }
                        else
                        {
                            if (!isRelativeToVariable)
                            {
                                _valueCommentDict[valueString] = new Tuple<string, bool, bool>(comment, isRelativeToVariable, true);
                                ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Update, valueString, comment));
                            }
                        }
                    }
                    else
                    {
                        if (_valueCommentDict[valueString].Item3)
                        {
                            _valueCommentDict[valueString] = new Tuple<string, bool, bool>(comment,isRelativeToVariable,true);
                            ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Update, valueString, comment));
                        }
                        else
                        {
                            _valueCommentDict[valueString] = new Tuple<string, bool, bool>(comment,true,isRelativeToValueModel);
                            if (isRelativeToValueModel)
                            {
                                ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Update, valueString, comment));
                            }
                        }
                    }
                }
                else
                {
                    _valueCommentDict[valueString] = new Tuple<string, bool, bool>(comment,isRelativeToVariable,isRelativeToValueModel);
                    if (isRelativeToValueModel)
                    {
                        ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Add, valueString, comment));
                    }
                }
            }
        }
        public static void Add(string valueString)
        {
            if (ContainValue(valueString))
            {
                string tempstr = _valueCommentDict[valueString].Item1;
                _valueCommentDict[valueString] = new Tuple<string, bool, bool>(tempstr,true,true);
                ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Add, valueString, tempstr));
            }
            else
            {
                _valueCommentDict[valueString] = new Tuple<string, bool, bool>(string.Empty, false, true);
                ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Add, valueString, string.Empty));
            }
        }
        public static void Delete(string valueString)
        {
            string tempstr = _valueCommentDict[valueString].Item1;
            if (!_valueCommentDict[valueString].Item2)
            {
                DeleteValueString(valueString);
            }
            else
            {
                _valueCommentDict[valueString] = new Tuple<string, bool, bool>(tempstr,true,false);
            }
            ValueCommentChanged.Invoke(new ValueCommentChangedEventArgs(ValueChangedType.Remove, valueString, string.Empty));
        }
        public static void DeleteValueString(string valueString)
        {
            _valueCommentDict.Remove(valueString);
        }
        public static bool CheckValueString(string valueString)
        {
            return _valueCommentDict[valueString].Item2 && !_valueCommentDict[valueString].Item3;
        }
        public static void UpdateComment(IValueModel value, string comment)
        {
            UpdateComment(value.ValueString, comment,value is IVariableValue,!(value is IVariableValue));
        }

        public static string GetComment(IValueModel value)
        {
            return GetComment(value.ValueString);
        }

        public static string GetComment(string valueString)
        {
            if (_valueCommentDict.ContainsKey(valueString))
            {
                return _valueCommentDict[valueString].Item1;
            }
            return string.Empty;
        }
        
        public static IDictionary<string, Tuple<string, bool, bool>> ValueCommentDict { get { return _valueCommentDict; } }
    }
}
