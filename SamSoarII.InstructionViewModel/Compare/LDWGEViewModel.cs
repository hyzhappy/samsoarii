using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
using System.Windows.Controls;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class LDWGEViewModel : InputBaseViewModel
    {
        private LDWGEModel _model;
        public WordValue Value1
        {
            get
            {
                return _model.Value1;
            }
            set
            {
                _model.Value1 = value;
                ValueTextBlock.Text = _model.Value1.ValueShowString;
            }
        }
        public WordValue Value2
        {
            get
            {
                return _model.Value2;
            }
            set
            {
                _model.Value2 = value;
                Value2TextBlock.Text = _model.Value2.ValueShowString;
            }
        }
        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                _model = value as LDWGEModel;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }
        private TextBlock[] _commentTextBlocks = { new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "LDWGE"; } }
        public LDWGEViewModel()
        {
            CenterTextBlock.Text = "W>=";
            Model = new LDWGEModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("W1",Value1);
            dialog.ShowLine5("W2",Value2);
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new LDWGEViewModel();
        }

        public static int CatalogID { get { return 302; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Value1 = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value1 = WordValue.Null;
            }
            try
            {
                Value2 = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Value2 = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value1.ValueString);
            result.Add(Value2.ValueString);
            return result;
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = Value1.ValueString;
            var oldvaluestring2 = Value2.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex }))
            {
                Value1 = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                Value2 = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, Value1.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, Value2.ValueString);
                ValueCommentManager.UpdateComment(Value1, valueStrings[1]);
                ValueCommentManager.UpdateComment(Value2, valueStrings[3]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (Value1 != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", Value1.ValueString, Value1.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (Value2 != WordValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", Value2.ValueString, Value2.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
        }
    }
}
