using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class LDDEQViewModel : InputBaseViewModel
    {
        private LDDEQModel _model;
        public DoubleWordValue Value1
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
        public DoubleWordValue Value2
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
                _model = value as LDDEQModel;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }
        private TextBlock[] _commentTextBlocks = { new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "LDDEQ"; } }
        public LDDEQViewModel()
        {
            CenterTextBlock.Text = "D==";
            Model = new LDDEQModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

        public override BaseViewModel Clone()
        {
            return new LDDEQViewModel();
        }

        private static int CatalogID { get { return 306; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Value1 = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value1 = DoubleWordValue.Null;
            }
            try
            {
                Value2 = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Value2 = DoubleWordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value1.ValueString);
            result.Add(Value2.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(Value1);
            result.Add(Value2);
            return result;
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("DW1",Value1);
            dialog.ShowLine5("DW2",Value2);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = Value1.ValueString;
            var oldvaluestring2 = Value2.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyDoubleWordRegex1,ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyDoubleWordRegex1, ValueParser.VerifyIntKHValueRegex }))
            {
                Value1 = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                Value2 = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
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
            if (Value1 != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", Value1.ValueString, Value1.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (Value2 != DoubleWordValue.Null)
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
