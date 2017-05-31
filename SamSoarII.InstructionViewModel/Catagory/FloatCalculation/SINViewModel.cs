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
    public class SINViewModel : OutputRectBaseViewModel
    {
        private SINModel _model;
        public FloatValue InputValue
        {
            get
            {
                return _model.InputValue;
            }
            set
            {
                _model.InputValue = value;
                MiddleTextBlock1.Text = String.Format("IN:{0:s}", _model.InputValue.ValueShowString);
            }
        }
        public FloatValue OutputValue
        {
            get
            {
                return _model.OutputValue;
            }
            set
            {
                _model.OutputValue = value;
                BottomTextBlock.Text = String.Format("OUT:{0:s}", _model.OutputValue.ValueShowString);
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
                _model = value as SINModel;
                InputValue = _model.InputValue;
                OutputValue = _model.OutputValue;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "SIN"; } }
        public SINViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new SINModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

        public override BaseViewModel Clone()
        {
            return new SINViewModel();
        }

        private static int CatalogID { get { return 705; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(InputValue);
            result.Add(OutputValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue = FloatValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseFloatValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = FloatValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("In",InputValue);
            dialog.ShowLine5("Out",OutputValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = InputValue.ValueString;
            var oldvaluestring2 = OutputValue.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyFloatRegex, ValueParser.VerifyFloatKValueRegex }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyFloatRegex }))
            {
                InputValue = ValueParser.ParseFloatValue(valueStrings[0], contextDevice);
                OutputValue = ValueParser.ParseFloatValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, InputValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(InputValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[3]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (InputValue != FloatValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", InputValue.ValueString, InputValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (OutputValue != FloatValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
        }
    }
}
