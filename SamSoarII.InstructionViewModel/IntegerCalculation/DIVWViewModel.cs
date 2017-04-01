using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

using System.Windows;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class DIVWViewModel : OutputRectBaseViewModel
    {
        private DIVWModel _model;
        private WordValue InputValue1
        {
            get
            {
                return _model.InputValue1;
            }
            set
            {
                _model.InputValue1 = value;
                MiddleTextBlock1.Text = _model.InputValue1.ValueShowString;
            }
        }
        private WordValue InputValue2
        {
            get
            {
                return _model.InputValue2;
            }
            set
            {
                _model.InputValue2 = value;
                MiddleTextBlock2.Text = _model.InputValue2.ValueShowString;
            }
        }
        private WordValue OutputValue
        {
            get
            {
                return _model.OutputValue;
            }
            set
            {
                _model.OutputValue = value;
                BottomTextBlock.Text = _model.OutputValue.ValueShowString;
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
                _model = value as DIVWModel;
                InputValue1 = _model.InputValue1;
                InputValue2 = _model.InputValue2;
                OutputValue = _model.OutputValue;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "DIVW"; } }
        public DIVWViewModel()
        {
            TopTextBlock.Text = InstructionName;
            _model = new DIVWModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }


        public override BaseViewModel Clone()
        {
            return new DIVWViewModel();
        }

        private static int CatalogID { get { return 808; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue1.ValueString);
            result.Add(InputValue2.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue1 = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue1 = WordValue.Null;
            }
            try
            {
                InputValue2 = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                InputValue2 = WordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = WordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("In1", InputValue1);
            dialog.ShowLine4("In2", InputValue2);
            dialog.ShowLine6("Out", OutputValue);
            return dialog;
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = InputValue1.ValueString;
            var oldvaluestring2 = InputValue2.ValueString;
            var oldvaluestring3 = OutputValue.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[4], new Regex[] { ValueParser.VerifyWordRegex1 }))
            {
                InputValue1 = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                InputValue2 = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                OutputValue = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, InputValue1.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, InputValue2.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(InputValue1, valueStrings[1]);
                ValueCommentManager.UpdateComment(InputValue2, valueStrings[3]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[5]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (InputValue1 != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", InputValue1.ValueString, InputValue1.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (InputValue2 != WordValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", InputValue2.ValueString, InputValue2.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
            if (OutputValue != WordValue.Null)
            {
                _commentTextBlocks[2].Text = string.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = string.Empty;
            }
        }
    }
}
