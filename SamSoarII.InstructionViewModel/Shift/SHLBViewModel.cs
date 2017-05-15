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
    public class SHLBViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "SHLB";
            }
        }

        public override BaseModel Model
        {
            get
            {
                return this.model;
            }

            protected set
            {
                if (value is SHLBModel)
                {
                    this.model = (SHLBModel)(value);
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new SHLBViewModel();
        }

        public override int GetCatalogID()
        {
            return 1210;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            result.Add(CountValue.ValueString);
            result.Add(MoveValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(SourceValue);
            result.Add(CountValue);
            result.Add(DestinationValue);
            result.Add(MoveValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch (ValueParseException e)
            {
                SourceValue = BitValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseBitValue(valueStrings[1]);
            }
            catch (ValueParseException e)
            {
                DestinationValue = BitValue.Null;
            }
            try
            {
                CountValue = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException e)
            {
                CountValue = WordValue.Null;
            }
            try
            {
                MoveValue = ValueParser.ParseWordValue(valueStrings[3]);
            }
            catch (ValueParseException e)
            {
                MoveValue = WordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(4);
            dialog.Title = InstructionName;
            dialog.ShowLine1("SRC", SourceValue);
            dialog.ShowLine3("DST", DestinationValue);
            dialog.ShowLine5("N1", CountValue);
            dialog.ShowLine7("N2", MoveValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = DestinationValue.ValueString;
            var oldvaluestring3 = CountValue.ValueString;
            var oldvaluestring4 = MoveValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0],
                new Regex[] { ValueParser.VerifyBitRegex1});
            bool check2 = ValueParser.CheckValueString(valueStrings[2],
                new Regex[] { ValueParser.VerifyBitRegex2 });
            bool check3 = ValueParser.CheckValueString(valueStrings[4],
                new Regex[] { ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex});
            bool check4 = ValueParser.CheckValueString(valueStrings[6],
                new Regex[] { ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2 && check3 && check4)
            {
                SourceValue = ValueParser.ParseBitValue(valueStrings[0], contextDevice);
                DestinationValue = ValueParser.ParseBitValue(valueStrings[2], contextDevice);
                CountValue = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                MoveValue = ValueParser.ParseWordValue(valueStrings[6], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DestinationValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, CountValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring4, MoveValue.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(CountValue, valueStrings[5]);
                ValueCommentManager.UpdateComment(MoveValue, valueStrings[7]);
            }
        }

        public override void UpdateCommentContent()
        {
            if (SourceValue != BitValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}",
                    SourceValue.ValueString,
                    SourceValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (DestinationValue != BitValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}",
                    DestinationValue.ValueString,
                    DestinationValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (CountValue != WordValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0:s}:{1:s}",
                    CountValue.ValueString,
                    CountValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
            if (MoveValue != WordValue.Null)
            {
                _commentTextBlocks[3].Text = String.Format("{0:s}:{1:s}",
                    MoveValue.ValueString,
                    MoveValue.Comment);
            }
            else
            { 
                _commentTextBlocks[3].Text = String.Empty;
            }

        }

        private SHLBModel model;

        public BitValue SourceValue
        {
            get { return this.model?.SourceValue; }
            set
            {
                if (this.model != null)
                {
                    this.model.SourceValue = value;
                    MiddleTextBlock1.Text = String.Format("SRC:{0:s}", value.ValueString);
                }
            }
        }
        
        public BitValue DestinationValue
        {
            get { return this.model?.DestinationValue; }
            set
            {
                if (this.model != null)
                {
                    this.model.DestinationValue = value;
                    BottomTextBlock.Text = String.Format("DST:{0:s}", value.ValueString);
                }
            }
        }

        public WordValue CountValue
        {
            get { return this.model?.CountValue; }
            set
            {
                if (this.model != null)
                {
                    this.model.CountValue = value;
                    MiddleTextBlock2.Text = String.Format("N1:{0:s}", value.ValueString);
                }
            }
        }

        public WordValue MoveValue
        {
            get { return this.model?.MoveValue; }
            set
            {
                if (this.model != null)
                {
                    this.model.MoveValue = value;
                    MiddleTextBlock3.Text = String.Format("N2:{0:s}", value.ValueString);
                }
            }
        }

        private TextBlock[] _commentTextBlocks = new TextBlock[]
        {
            new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock()
        };

        public SHLBViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new SHLBModel();
            foreach (TextBlock tblock in _commentTextBlocks)
            {
                CommentArea.Children.Add(tblock);
            }
        }

    }
}
