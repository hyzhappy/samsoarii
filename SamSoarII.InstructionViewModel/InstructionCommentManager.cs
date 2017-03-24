using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstViewModel
{
    public static class InstructionCommentManager
    {
        private static Dictionary<string, HashSet<BaseViewModel>> _valueRelatedModel = new Dictionary<string, HashSet<BaseViewModel>>();

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
                if (!str.Equals(string.Empty))
                {
                    if (_valueRelatedModel.ContainsKey(str))
                    {
                        _valueRelatedModel[str].Add(viewmodel);
                    }
                    else
                    {
                        _valueRelatedModel.Add(str, new HashSet<BaseViewModel>() { viewmodel });
                        ValueCommentManager.Add(str);
                    }
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
                        {
                            ValueCommentManager.Delete(str);
                            _valueRelatedModel.Remove(str);
                        }
                    }
                }
            }
        }

        public static void ModifyValue(BaseViewModel viewmodel, string oldvalueString, string newvalueString)
        {
            if (oldvalueString != newvalueString)
            {
                if (_valueRelatedModel.ContainsKey(oldvalueString))
                {
                    var hset = _valueRelatedModel[oldvalueString];
                    hset.Remove(viewmodel);
                    if (hset.Count == 0)
                    {
                        ValueCommentManager.Delete(oldvalueString);
                        _valueRelatedModel.Remove(oldvalueString);
                    }
                }
                if (newvalueString != string.Empty)
                {
                    if (_valueRelatedModel.ContainsKey(newvalueString))
                    {
                        var hset = _valueRelatedModel[newvalueString];
                        hset.Add(viewmodel);
                    }
                    else
                    {
                        _valueRelatedModel.Add(newvalueString, new HashSet<BaseViewModel>() { viewmodel });
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

        public static void Clear()
        {
            _valueRelatedModel.Clear();
        }

    }
}
