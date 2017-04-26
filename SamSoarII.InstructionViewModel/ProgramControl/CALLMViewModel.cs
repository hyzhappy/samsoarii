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
                this._model = (CALLMModel)(value);
                FunctionName = _model.FunctionName;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
                Value3 = _model.Value3;
                Value4 = _model.Value4;
            }
        }

        public string FunctionName
        {
            get
            {
                return _model.FunctionName;
            }
            set
            {
                _model.FunctionName = value;
                BottomTextBlock.Text = _model.FunctionName;
            }
        }

        public string ArgName1 { get; set; } = String.Empty;
        public string ArgName2 { get; set; } = String.Empty;
        public string ArgName3 { get; set; } = String.Empty;
        public string ArgName4 { get; set; } = String.Empty;
        public string ArgType1 { get; set; } = String.Empty;
        public string ArgType2 { get; set; } = String.Empty;
        public string ArgType3 { get; set; } = String.Empty;
        public string ArgType4 { get; set; } = String.Empty;

        public IValueModel Value1
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
                if (ArgName1.Length > 0)
                    MiddleTextBlock1.Text = String.Format("{0:s}:{1:s}", ArgName1, _model.Value1.ValueShowString);
            }
        }

        public IValueModel Value2
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
                if (ArgName2.Length > 0)
                    MiddleTextBlock2.Text = String.Format("{0:s}:{1:s}", ArgName2, _model.Value2.ValueShowString);
            }
        }

        public IValueModel Value3
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
                if (ArgName3.Length > 0)
                    MiddleTextBlock3.Text = String.Format("{0:s}:{1:s}", ArgName3, _model.Value3.ValueShowString);
            }
        }

        public IValueModel Value4
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
                if (ArgName4.Length > 0)
                    MiddleTextBlock4.Text = String.Format("{0:s}:{1:s}", ArgName4, _model.Value4.ValueShowString);
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
            result.Add(FunctionName);
            result.Add(Value1.ValueString);
            result.Add(Value2.ValueString);
            result.Add(Value3.ValueString);
            result.Add(Value4.ValueString);
            return result;
        }

        private IValueModel _ParseValue(string valueString, string name, string type)
        {
            IValueModel result = WordValue.Null;
            if (name.Equals(String.Empty))
                return result;
            try
            {
                switch (type)
                {
                    case "BIT":
                        result = ValueParser.ParseBitValue(valueString);
                        break;
                    case "WORD":
                        result = ValueParser.ParseWordValue(valueString);
                        break;
                    case "DWORD":
                        result = ValueParser.ParseDoubleWordValue(valueString);
                        break;
                    case "FLOAT":
                        result = ValueParser.ParseFloatValue(valueString);
                        break;
                    default:
                        throw new ValueParseException(String.Format("参数{0:s}无法识别的类型{1:s}", name, type));
                }
            }
            catch (ValueParseException vpe)
            {
                MessageBox.Show(vpe.Message);
                return result;
            }
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            FunctionName = valueStrings[0];
            ArgType1 = valueStrings[4];
            ArgName1 = valueStrings[5];
            ArgType2 = valueStrings[8];
            ArgName2 = valueStrings[9];
            ArgType3 = valueStrings[12];
            ArgName3 = valueStrings[13];
            ArgType4 = valueStrings[16];
            ArgName4 = valueStrings[17];
            Value1 = _ParseValue(valueStrings[2], ArgName1, ArgType1);
            Value2 = _ParseValue(valueStrings[6], ArgName2, ArgType1);
            Value3 = _ParseValue(valueStrings[10], ArgName3, ArgType1);
            Value4 = _ParseValue(valueStrings[14], ArgName4, ArgType1);
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            int argcount = 5;
            if (ArgName4.Equals(String.Empty))
                argcount = 4;
            if (ArgName3.Equals(String.Empty))
                argcount = 3;
            if (ArgName2.Equals(String.Empty))
                argcount = 2;
            if (ArgName1.Equals(String.Empty))
                argcount = 1;
            var dialog = new ElementPropertyDialog(argcount, ElementPropertyDialog.INST_CALLM);
            switch (argcount)
            {
                case 1:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FunctionName;
                    break;
                case 2:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FunctionName;
                    dialog.ShowLine2(ArgName1 + ":", Value1);
                    break;
                case 3:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FunctionName;
                    dialog.ShowLine2(ArgName1 + ":", Value1);
                    dialog.ShowLine3(ArgName2 + ":", Value2);
                    break;
                case 4:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FunctionName;
                    dialog.ShowLine2(ArgName1 + ":", Value1);
                    dialog.ShowLine3(ArgName2 + ":", Value2);
                    dialog.ShowLine4(ArgName3 + ":", Value3);
                    break;
                case 5:
                    dialog.ShowLine1("FUNC:");
                    dialog.ValueString1 = FunctionName;
                    dialog.ShowLine2(ArgName1 + ":", Value1);
                    dialog.ShowLine3(ArgName2 + ":", Value2);
                    dialog.ShowLine4(ArgName3 + ":", Value3);
                    dialog.ShowLine5(ArgName4 + ":", Value4);
                    break;
            }
            return dialog;
        }

        private bool _Check(string valueString, string name, string type)
        {
            if (name.Equals(String.Empty))
                return true;
            switch (type)
            {
                case "BIT":
                    return ValueParser.CheckValueString(valueString, new Regex[] {
                        ValueParser.VerifyBitRegex1 });
                case "WORD":
                    return ValueParser.CheckValueString(valueString, new Regex[] {
                        ValueParser.VerifyWordRegex3 });
                case "DWORD":
                    return ValueParser.CheckValueString(valueString, new Regex[] {
                        ValueParser.VerifyDoubleWordRegex1 });
                case "FLOAT":
                    return ValueParser.CheckValueString(valueString, new Regex[] {
                        ValueParser.VerifyFloatRegex });
                default:
                    return false;
            }
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = FunctionName;
            var oldvaluestring2 = Value1.ValueString;
            var oldvaluestring3 = Value2.ValueString;
            var oldvaluestring4 = Value3.ValueString;
            var oldvaluestring5 = Value4.ValueString;

            var newargtype1 = valueStrings[4];
            var newargname1 = valueStrings[5];
            var newargtype2 = valueStrings[8];
            var newargname2 = valueStrings[9];
            var newargtype3 = valueStrings[12];
            var newargname3 = valueStrings[13];
            var newargtype4 = valueStrings[16];
            var newargname4 = valueStrings[17];

            bool check1 = _Check(valueStrings[2], newargname1, newargtype1);
            bool check2 = _Check(valueStrings[6], newargname2, newargtype2);
            bool check3 = _Check(valueStrings[10], newargname3, newargtype3);
            bool check4 = _Check(valueStrings[14], newargname4, newargtype4);
            
            if (check1 && check2 && check3 && check4)
            {
                FunctionName = valueStrings[0];
                ArgName1 = newargname1;
                ArgType1 = newargtype1;
                Value1 = _ParseValue(valueStrings[2], newargname1, newargtype1);
                ArgName2 = newargname2;
                ArgType2 = newargtype2;
                Value2 = _ParseValue(valueStrings[6], newargname2, newargtype2);
                ArgName3 = newargname3;
                ArgType3 = newargtype3;
                Value3 = _ParseValue(valueStrings[10], newargname3, newargtype3);
                ArgName4 = newargname4;
                ArgType4 = newargtype4;
                Value4 = _ParseValue(valueStrings[14], newargname4, newargtype4);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, FunctionName);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, Value1.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, Value1.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring4, FunctionName);
                InstructionCommentManager.ModifyValue(this, oldvaluestring5, FunctionName);
                ValueCommentManager.UpdateComment(Value1, valueStrings[3]);
                ValueCommentManager.UpdateComment(Value2, valueStrings[7]);
                ValueCommentManager.UpdateComment(Value3, valueStrings[11]);
                ValueCommentManager.UpdateComment(Value4, valueStrings[15]);
            }
            else if (!check1)
            {
                throw new ValueParseException(String.Format("输入参数不符合{0:s}的类型{1:s}", newargname1, newargtype1));
            }
            else if (!check2)
            {
                throw new ValueParseException(String.Format("输入参数不符合{0:s}的类型{1:s}", newargname2, newargtype2));
            }
            else if (!check3)
            {
                throw new ValueParseException(String.Format("输入参数不符合{0:s}的类型{1:s}", newargname3, newargtype3));
            }
            else if (!check4)
            {
                throw new ValueParseException(String.Format("输入参数不符合{0:s}的类型{1:s}", newargname4, newargtype4));
            }
        }

        public override void UpdateCommentContent()
        {
            if (Value1 != WordValue.Null)
            {
                _commentTextBlock[1].Text = String.Format("{0:s}:{1:s}", Value1.ValueString, Value1.Comment);
            }
            else
            {
                _commentTextBlock[1].Text = String.Empty;
            }
            if (Value2 != WordValue.Null)
            {
                _commentTextBlock[2].Text = String.Format("{0:s}:{1:s}", Value2.ValueString, Value2.Comment);
            }
            else
            {
                _commentTextBlock[2].Text = String.Empty;
            }
            if (Value3 != WordValue.Null)
            {
                _commentTextBlock[3].Text = String.Format("{0:s}:{1:s}", Value3.ValueString, Value3.Comment);
            }
            else
            {
                _commentTextBlock[3].Text = String.Empty;
            }
            if (Value4 != WordValue.Null)
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
