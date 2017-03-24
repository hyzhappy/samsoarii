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
    public class MOVFViewModel : OutputRectBaseViewModel
    {
        private MOVFModel _model;
        private FloatValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock1.Text = _model.SourceValue.ValueShowString;
            }
        }
        private FloatValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                BottomTextBlock.Text = _model.DestinationValue.ValueShowString;
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
                _model = value as MOVFModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "MOVF"; } }
        public MOVFViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MOVFModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }
        public override BaseViewModel Clone()
        {
            return new MOVFViewModel();
        }

        public static int CatalogID { get { return 602; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("S",SourceValue);
            dialog.ShowLine5("D",DestinationValue);
            return dialog;
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = DestinationValue.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyFloatRegex, ValueParser.VerifyFloatKValueRegex }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyFloatRegex }))
            {
                SourceValue = ValueParser.ParseFloatValue(valueStrings[0], contextDevice);
                DestinationValue = ValueParser.ParseFloatValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DestinationValue.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[3]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = FloatValue.Null;
            }

            try
            {
                DestinationValue = ValueParser.ParseFloatValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = FloatValue.Null;
            }
        }
        public override void UpdateCommentContent()
        {
            if (SourceValue != FloatValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", SourceValue.ValueString, SourceValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (DestinationValue != FloatValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", DestinationValue.ValueString, DestinationValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
        }
    }
}
