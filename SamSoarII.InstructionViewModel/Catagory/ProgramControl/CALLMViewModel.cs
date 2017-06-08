using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class CALLMViewModel : OutputRectBaseViewModel
    {
        private CALLMModel _model;

        public override BaseModel Model
        {
            get
            {
                return this._model;
            }
            protected set
            {
                if (value is CALLMModel)
                    this._model = (CALLMModel)(value);
            }
        }

        public string FuncName
        {
            get
            {
                if (_model == null) return null;
                return _model.FunctionName;
            }
            set
            {
                if (_model == null) return;
                _model.FunctionName = value;
                MiddleTextBlock1.Text = value;
            }
        }

        public string FuncComment { get; set; }
            = String.Empty;
        
        public ArgumentValue Value1
        {
            get
            {
                if (_model == null) return null;
                return _model.Value1;
            }
            set
            {
                if (_model == null) return;
                _model.Value1 = value;
                if (value != ArgumentValue.Null 
                 && value.ArgumentName.Length > 0)
                    MiddleTextBlock2.Text = String.Format("{0:s}:{1:s}", value.ArgumentName, value.ValueShowString);
                else
                    MiddleTextBlock2.Text = String.Empty;
            }
        }

        public ArgumentValue Value2
        {
            get
            {
                if (_model == null) return null;
                return _model.Value2;
            }
            set
            {
                if (_model == null) return;
                _model.Value2 = value;
                if (value != ArgumentValue.Null
                 && value.ArgumentName.Length > 0)
                    MiddleTextBlock3.Text = String.Format("{0:s}:{1:s}", value.ArgumentName, value.ValueShowString);
                else
                    MiddleTextBlock3.Text = String.Empty;
            }
        }

        public ArgumentValue Value3
        {
            get
            {
                if (_model == null) return null;
                return _model.Value3;
            }
            set
            {
                if (_model == null) return;
                _model.Value3 = value;
                if (value != ArgumentValue.Null
                 && value.ArgumentName.Length > 0)
                    MiddleTextBlock4.Text = String.Format("{0:s}:{1:s}", value.ArgumentName, value.ValueShowString);
                else
                    MiddleTextBlock4.Text = String.Empty;
            }
        }

        public ArgumentValue Value4
        {
            get
            {
                if (_model == null) return null;
                return _model.Value4;
            }
            set
            {
                if (_model == null) return;
                _model.Value4 = value;
                if (value != ArgumentValue.Null
                 && value.ArgumentName.Length > 0)
                    MiddleTextBlock5.Text = String.Format("{0:s}:{1:s}", value.ArgumentName, value.ValueShowString);
                else
                    MiddleTextBlock5.Text = String.Empty;
            }
        }
        
        private TextBlock[] _commentTextBlock = new TextBlock[]
            {new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock()};

        public override string InstructionName { get { return "CALLM"; } }
        public override BaseViewModel Clone()
        {
            return new CALLMViewModel();
        }

        public override int GetCatalogID()
        {
            return 1105;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(FuncName);
            result.Add(Value1.ToString());
            result.Add(Value2.ToString());
            result.Add(Value3.ToString());
            result.Add(Value4.ToString());
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            FuncName = valueStrings[0];
            string[] args = null;
            ArgumentValue[] values = new ArgumentValue[4];
            for (int i = 0; i < valueStrings.Count-1; i++)
            {
                try
                {
                    values[i] = ArgumentValue.Parse(valueStrings[i+1]);
                }
                catch (ValueParseException)
                {
                    args = valueStrings[i+1].Split(' ');
                    switch (args[1])
                    {
                        case "BIT*":
                            values[i] = new ArgumentValue(args[0], args[1], BitValue.Null);
                            break;
                        case "WORD*":
                            values[i] = new ArgumentValue(args[0], args[1], WordValue.Null);
                            break;
                        case "DWORD*":
                            values[i] = new ArgumentValue(args[0], args[1], DWordValue.Null);
                            break;
                        case "FLOAT*":
                            values[i] = new ArgumentValue(args[0], args[1], FloatValue.Null);
                            break;
                        default:
                            values[i] = ArgumentValue.Null;
                            break;
                    }
                }
            }
            Value1 = values[0];
            Value2 = values[1];
            Value3 = values[2];
            Value4 = values[3];
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            int argcount = 5;
            if (Value4 == ArgumentValue.Null)
                argcount = 4;
            if (Value3 == ArgumentValue.Null)
                argcount = 3;
            if (Value2 == ArgumentValue.Null)
                argcount = 2;
            if (Value1 == ArgumentValue.Null)
                argcount = 1;
            var dialog = new ElementPropertyDialog(argcount, ElementPropertyDialog.INST_CALLM);
            switch (argcount)
            {
                case 1:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FuncName;
                    break;
                case 2:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FuncName;
                    dialog.ShowLine2(Value1.ArgumentName + ":", Value1);
                    break;
                case 3:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FuncName;
                    dialog.ShowLine2(Value1.ArgumentName + ":", Value1);
                    dialog.ShowLine3(Value2.ArgumentName + ":", Value2);
                    break;
                case 4:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FuncName;
                    dialog.ShowLine2(Value1.ArgumentName + ":", Value1);
                    dialog.ShowLine3(Value2.ArgumentName + ":", Value2);
                    dialog.ShowLine4(Value3.ArgumentName + ":", Value3);
                    break;
                case 5:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FuncName;
                    dialog.ShowLine2(Value1.ArgumentName + ":", Value1);
                    dialog.ShowLine3(Value2.ArgumentName + ":", Value2);
                    dialog.ShowLine4(Value3.ArgumentName + ":", Value3);
                    dialog.ShowLine5(Value4.ArgumentName + ":", Value4);
                    break;
            }
            return dialog;
        }
        */
        public void AcceptNewValues(string _funcname, string _funccomment, ArgumentValue[] _values)
        {
            var oldvaluestring1 = FuncName;
            var oldvaluestring2 = Value1.ValueString;
            var oldvaluestring3 = Value2.ValueString;
            var oldvaluestring4 = Value3.ValueString;
            var oldvaluestring5 = Value4.ValueString;
            FuncName = _funcname;
            FuncComment = _funccomment;
            if (_values.Length > 0)
            {
                Value1 = _values[0];
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, Value1.ValueString);
                ValueCommentManager.UpdateComment(Value1, Value1.Comment);
            }
            else
            {
                Value1 = ArgumentValue.Null;
            }
            if (_values.Length > 1)
            {
                Value2 = _values[1];
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, Value2.ValueString);
                ValueCommentManager.UpdateComment(Value2, Value2.Comment);
            }
            else
            {
                Value2 = ArgumentValue.Null;
            }
            if (_values.Length > 2)
            {
                Value3 = _values[2];
                InstructionCommentManager.ModifyValue(this, oldvaluestring4, Value3.ValueString);
                ValueCommentManager.UpdateComment(Value3, Value3.Comment);
            }
            else
            {
                Value3 = ArgumentValue.Null;
            }
            if (_values.Length > 3)
            {
                Value4 = _values[3];
                InstructionCommentManager.ModifyValue(this, oldvaluestring5, Value4.ValueString);
                ValueCommentManager.UpdateComment(Value4, Value4.Comment);
            }
            else
            {
                Value4 = ArgumentValue.Null;
            }
            UpdateCommentContent();
        }

        public override void UpdateCommentContent()
        {
            _commentTextBlock[0].Text = String.Format("{0:s}:{1:s}",
                FuncName, FuncComment);
            if (Value1 != ArgumentValue.Null && Value1.ArgumentName.Length > 0)
            {
                _commentTextBlock[1].Text = String.Format("{0:s}:{1:s}", Value1.ValueString, Value1.Comment);
            }
            else
            {
                _commentTextBlock[1].Text = String.Empty;
            }
            if (Value2 != ArgumentValue.Null && Value2.ArgumentName.Length > 0)
            {
                _commentTextBlock[2].Text = String.Format("{0:s}:{1:s}", Value2.ValueString, Value2.Comment);
            }
            else
            {
                _commentTextBlock[2].Text = String.Empty;
            }
            if (Value3 != ArgumentValue.Null && Value3.ArgumentName.Length > 0)
            {
                _commentTextBlock[3].Text = String.Format("{0:s}:{1:s}", Value3.ValueString, Value3.Comment);
            }
            else
            {
                _commentTextBlock[3].Text = String.Empty;
            }
            if (Value4 != ArgumentValue.Null && Value4.ArgumentName.Length > 0)
            {
                _commentTextBlock[4].Text = String.Format("{0:s}:{1:s}", Value4.ValueString, Value4.Comment);
            }
            else
            {
                _commentTextBlock[4].Text = String.Empty;
            }
        }

        public CALLMViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new CALLMModel();
            CommentArea.Children.Add(_commentTextBlock[0]);
            CommentArea.Children.Add(_commentTextBlock[1]);
            CommentArea.Children.Add(_commentTextBlock[2]);
            CommentArea.Children.Add(_commentTextBlock[3]);
            CommentArea.Children.Add(_commentTextBlock[4]);
        }
    }
}
