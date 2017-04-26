using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using PLCElementModel.PLCElementModel.Shift;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class SHRBViewModel : OutputRectBaseViewModel
    {
        private SHRBModel model;
        public BitValue SourceValue
        {
            get
            {
                return model.SourceValue;
            }
            set
            {
                model.SourceValue = value;
                MiddleTextBlock1.Text = string.Format("S:{0}", model.SourceValue.ValueShowString);
            }
        }
        public BitValue DestinationValue
        {
            get
            {
                return model.DestinationValue;
            }
            set
            {
                model.DestinationValue = value;
                MiddleTextBlock2.Text = string.Format("D:{0}", model.DestinationValue.ValueShowString);
            }
        }
        public WordValue SourceLength
        {
            get
            {
                return model.SourceLength;
            }
            set
            {
                model.SourceLength = value;
                MiddleTextBlock3.Text = string.Format("SLen:{0}", model.SourceLength.ValueShowString);
            }
        }
        public WordValue DestinationLength
        {
            get
            {
                return model.DestinationLength;
            }
            set
            {
                model.DestinationLength = value;
                MiddleTextBlock4.Text = string.Format("DLen:{0}", model.DestinationLength.ValueShowString);
            }
        }
        public override BaseModel Model
        {
            get
            {
                return model;
            }
            protected set
            {
                model = value as SHRBModel;
                SourceValue = model.SourceValue;
                DestinationValue = model.DestinationValue;
                SourceLength = model.SourceLength;
                DestinationLength = model.DestinationLength;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock() };
        public override string InstructionName
        {
            get
            {
                return "SHRB";
            }
        }
        public SHRBViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new SHRBModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
            CommentArea.Children.Add(_commentTextBlocks[3]);
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = DestinationValue.ValueString;
            var oldvaluestring3 = SourceLength.ValueString;
            var oldvaluestring4 = DestinationLength.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyBitRegex1 }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyBitRegex2 }) && ValueParser.CheckValueString(valueStrings[4], new Regex[] { ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[6], new Regex[] { ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex }))
            {
                SourceValue = ValueParser.ParseBitValue(valueStrings[0], contextDevice);
                DestinationValue = ValueParser.ParseBitValue(valueStrings[2], contextDevice);
                SourceLength = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                DestinationLength = ValueParser.ParseWordValue(valueStrings[6], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DestinationValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, SourceLength.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring4, DestinationLength.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(SourceLength, valueStrings[5]);
                ValueCommentManager.UpdateComment(DestinationLength, valueStrings[7]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }

        public override BaseViewModel Clone()
        {
            return new SHRBViewModel();
        }
        private static int CatalogID { get { return 1205; } }
        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            result.Add(SourceLength.ValueString);
            result.Add(DestinationLength.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = BitValue.Null;
            }
            try
            {
                SourceLength = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                SourceLength = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseBitValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = BitValue.Null;
            }
            try
            {
                DestinationLength = ValueParser.ParseWordValue(valueStrings[3]);
            }
            catch (ValueParseException exception)
            {
                DestinationLength = WordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(4);
            dialog.Title = InstructionName;
            dialog.ShowLine1("S", SourceValue);
            dialog.ShowLine3("D", DestinationValue);
            dialog.ShowLine5("SLen", SourceLength);
            dialog.ShowLine7("DLen", DestinationLength);
            return dialog;
        }

        public override void UpdateCommentContent()
        {
            if (SourceValue != BitValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", SourceValue.ValueString, SourceValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (DestinationValue != BitValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", DestinationValue.ValueString, DestinationValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
            if (SourceLength != WordValue.Null)
            {
                _commentTextBlocks[2].Text = string.Format("{0}:{1}", SourceLength.ValueString, SourceLength.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = string.Empty;
            }
            if (DestinationLength != WordValue.Null)
            {
                _commentTextBlocks[3].Text = string.Format("{0}:{1}", DestinationLength.ValueString, DestinationLength.Comment);
            }
            else
            {
                _commentTextBlocks[3].Text = string.Empty;
            }
        }
    }
}
