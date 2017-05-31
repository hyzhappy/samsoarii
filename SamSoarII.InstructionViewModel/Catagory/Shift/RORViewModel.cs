using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class RORViewModel : OutputRectBaseViewModel
    {
        private RORModel _model;
        public WordValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock2.Text = string.Format("IN1:{0}", _model.SourceValue.ValueShowString);
            }
        }
        public WordValue Count
        {
            get
            {
                return _model.Count;
            }
            set
            {
                _model.Count = value;
                MiddleTextBlock3.Text = string.Format("IN2:{0}", _model.Count.ValueShowString);
            }
        }
        public WordValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                BottomTextBlock.Text = string.Format("OUT:{0}", _model.DestinationValue.ValueShowString);
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
                _model = value as RORModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
                Count = _model.Count;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "ROR"; } }

        public RORViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new RORModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }

        public override BaseViewModel Clone()
        {
            return new RORViewModel();
        }

        private static int CatalogID { get { return 1208; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(Count.ValueString);
            result.Add(DestinationValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(SourceValue);
            result.Add(Count);
            result.Add(DestinationValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = WordValue.Null;
            }
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Count = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = WordValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("IN1", SourceValue);
            dialog.ShowLine4("IN2", Count);
            dialog.ShowLine6("OUT", DestinationValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = DestinationValue.ValueString;
            var oldvaluestring3 = Count.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex }) && ValueParser.CheckValueString(valueStrings[4], new Regex[] { ValueParser.VerifyWordRegex2 }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex }))
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                DestinationValue = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                Count = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DestinationValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, Count.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[5]);
                ValueCommentManager.UpdateComment(Count, valueStrings[3]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (SourceValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", SourceValue.ValueString, SourceValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (DestinationValue != WordValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", DestinationValue.ValueString, DestinationValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
            if (Count != WordValue.Null)
            {
                _commentTextBlocks[2].Text = string.Format("{0}:{1}", Count.ValueString, Count.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = string.Empty;
            }
        }
    }
}
