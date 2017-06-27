using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
using System.Threading;

namespace SamSoarII.LadderInstViewModel
{
    public enum MappedMessageChangedType
    {
        Add,
        AddFirst,
        Remove,
        RemoveLast,
        Clear,
        Refresh
    }
    public class MappedMessageChangedEventArgs : EventArgs
    {
        public MappedMessageChangedType Type { get; set; }
        public string ValueString { get; set; }
        public BaseViewModel MappedValueModel { get; set; }
        public MappedMessageChangedEventArgs(MappedMessageChangedType type,string valueString ,BaseViewModel mappedValueModel)
        {
            Type = type;
            ValueString = valueString;
            MappedValueModel = mappedValueModel;
        }
    }
    public delegate void MappedMessageChangedEventHandler(MappedMessageChangedEventArgs e);
    public static class InstructionCommentManager
    {
        public static event MappedMessageChangedEventHandler MappedMessageChanged = delegate { };
        private static Dictionary<string, HashSet<BaseViewModel>> _valueRelatedModel = new Dictionary<string, HashSet<BaseViewModel>>();
        public static Dictionary<string, HashSet<BaseViewModel>> ValueRelatedModel
        {
            get
            {
                return _valueRelatedModel;
            }
        }
        public static void RaiseMappedMessageChangedEvent()
        {
            MappedMessageChanged.Invoke(new MappedMessageChangedEventArgs(MappedMessageChangedType.Refresh,string.Empty,null));
        }
        static InstructionCommentManager()
        {
            ValueCommentManager.ValueCommentChanged += ValueCommentManager_ValueCommentChanged;
        }
        private static void ValueCommentManager_ValueCommentChanged(ValueCommentChangedEventArgs e)
        {
            UpdateCommentContent(e.ValueString);
        }

        public static void Register(BaseViewModel viewmodel)
        {
            foreach (var str in viewmodel.GetValueString())
            {
                if (!str.Equals(string.Empty) && viewmodel.NetWorkNum != -1)
                {
                    if (_valueRelatedModel.ContainsKey(str))
                        _valueRelatedModel[str].Add(viewmodel);
                    else
                        _valueRelatedModel.Add(str, new HashSet<BaseViewModel>() { viewmodel });
                }
            }
        }

        public static void Unregister(BaseViewModel viewmodel)
        {
            foreach (var str in viewmodel.GetValueString())
            {
                if (!str.Equals(string.Empty))
                {
                    if (_valueRelatedModel.ContainsKey(str))
                    {
                        var hset = _valueRelatedModel[str];
                        hset.Remove(viewmodel);
                        if (hset.Count == 0)
                            _valueRelatedModel.Remove(str);
                    }
                }
            }
        }

        public static void ModifyValue(BaseViewModel viewmodel, string oldvalueString, string newvalueString)
        {
            if (oldvalueString != newvalueString && viewmodel.NetWorkNum != -1)
            {
                if (_valueRelatedModel.ContainsKey(oldvalueString))
                {
                    var hset = _valueRelatedModel[oldvalueString];
                    hset.Remove(viewmodel);
                    if (hset.Count == 0)
                    {
                        _valueRelatedModel.Remove(oldvalueString);
                        MappedMessageChanged.Invoke(new MappedMessageChangedEventArgs(MappedMessageChangedType.RemoveLast,oldvalueString, viewmodel));
                    }
                    else
                        MappedMessageChanged.Invoke(new MappedMessageChangedEventArgs(MappedMessageChangedType.Remove, oldvalueString, viewmodel));
                }
                if (newvalueString != string.Empty)
                {
                    if (_valueRelatedModel.ContainsKey(newvalueString))
                    {
                        var hset = _valueRelatedModel[newvalueString];
                        hset.Add(viewmodel);
                        MappedMessageChanged.Invoke(new MappedMessageChangedEventArgs(MappedMessageChangedType.Add, newvalueString, viewmodel));
                    }
                    else
                    {
                        _valueRelatedModel.Add(newvalueString, new HashSet<BaseViewModel>() { viewmodel });
                        MappedMessageChanged.Invoke(new MappedMessageChangedEventArgs(MappedMessageChangedType.AddFirst, newvalueString, viewmodel));
                    }
                }
            }
        }

        public static void UpdateCommentContent(IValueModel value)
        {
            UpdateCommentContent(value.ValueString);
        }

        public static void UpdateCommentContent(string valueString)
        {
            if (_valueRelatedModel.ContainsKey(valueString))
            {
                foreach (var model in _valueRelatedModel[valueString])
                {
                    model.UpdateCommentContent();
                }
            }
        }

        public static void UpdateAllComment()
        {
            var set = new HashSet<BaseViewModel>();
            foreach (var hset in _valueRelatedModel.Values)
            {
                set.UnionWith(hset);
            }
            foreach (var model in set)
            {
                model.UpdateCommentContent();
            }
        }
        public static bool ContainValueString(string valueString)
        {
            return _valueRelatedModel.ContainsKey(valueString);
        }
        public static void Clear()
        {
            _valueRelatedModel.Clear();
            MappedMessageChanged(new MappedMessageChangedEventArgs(MappedMessageChangedType.Clear, String.Empty, null));
        }

    }
}
