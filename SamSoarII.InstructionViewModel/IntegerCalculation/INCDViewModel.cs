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
    public class INCDViewModel : OutputRectBaseViewModel
    {
        private INCDModel _model;
        public DoubleWordValue InputValue
        {
            get
            {
                return _model.InputValue;
            }
            set
            {
                _model.InputValue = value;
                MiddleTextBlock1.Text = _model.InputValue.ValueShowString;
            }
        }
        public DoubleWordValue OutputValue
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
                _model = value as INCDModel;
                InputValue = _model.InputValue;
                OutputValue = _model.OutputValue;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock()};
        public override string InstructionName { get { return "INCD"; } }
        public INCDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new INCDModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

        public override BaseViewModel Clone()
        {
            return new INCDViewModel();
        }

        private static int CatalogID { get { return 811; } }

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

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue = DoubleWordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = DoubleWordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("In",InputValue);
            dialog.ShowLine5("Out",OutputValue);
            return dialog;
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = InputValue.ValueString;
            var oldvaluestring2 = OutputValue.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyDoubleWordRegex1, ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyDoubleWordRegex1 }))
            {
                InputValue = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                OutputValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
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
            if (InputValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", InputValue.ValueString, InputValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (OutputValue != DoubleWordValue.Null)
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
