using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel.Auxiliar;
using System.Windows.Controls;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel.Auxiliar
{
    public class SMOVViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "SMOV";
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
                if (value is SMOVModel)
                {
                    this.model = (SMOVModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except SMOVModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new SMOVViewModel();
        }

        public override int GetCatalogID()
        {
            return 1816;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(SourceStart.ValueString);
            result.Add(SourceCount.ValueString);
            result.Add(DestinationValue.ValueString);
            result.Add(DestinationStart.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(SourceValue);
            result.Add(SourceStart);
            result.Add(SourceCount);
            result.Add(DestinationValue);
            result.Add(DestinationStart);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {   
            try
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                SourceValue = WordValue.Null;
            }
            try
            {
                SourceStart = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                SourceStart = WordValue.Null;
            }
            try
            {
                SourceCount = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                SourceCount = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseWordValue(valueStrings[3]);
            }
            catch (ValueParseException)
            {
                DestinationValue = WordValue.Null;
            }
            try
            {
                DestinationStart = ValueParser.ParseWordValue(valueStrings[4]);
            }
            catch (ValueParseException)
            {
                DestinationStart = WordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(5);
            dialog.Title = InstructionName;
            dialog.ShowLine1("SV:", SourceValue);
            dialog.ShowLine2("SS:", SourceStart);
            dialog.ShowLine4("SC:", SourceCount);
            dialog.ShowLine6("DV:", DestinationValue);
            dialog.ShowLine7("DS:", DestinationStart);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = SourceStart.ValueString;
            var oldvaluestring3 = SourceCount.ValueString;
            var oldvaluestring4 = DestinationValue.ValueString;
            var oldvaluestring5 = DestinationStart.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex1 });
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyIntKHValueRegex });
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyIntKHValueRegex });
            bool check4 = ValueParser.CheckValueString(valueStrings[6], new Regex[] {
                ValueParser.VerifyWordRegex1 });
            bool check5 = ValueParser.CheckValueString(valueStrings[8], new Regex[] {
                ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2 && check3 && check4 && check5)
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                SourceStart = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                SourceCount = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                DestinationValue = ValueParser.ParseWordValue(valueStrings[6], contextDevice);
                DestinationStart = ValueParser.ParseWordValue(valueStrings[8], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, SourceStart.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, SourceCount.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring4, DestinationValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring5, DestinationStart.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(SourceStart, valueStrings[3]);
                ValueCommentManager.UpdateComment(SourceCount, valueStrings[5]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[7]);
                ValueCommentManager.UpdateComment(DestinationStart, valueStrings[9]);
            }
            else if (!check1)
            {
                throw new ValueParseException("SV格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("SS格式错误！");
            }
            else if (!check3)
            {
                throw new ValueParseException("SC格式错误！");
            }
            else if (!check4)
            {
                throw new ValueParseException("DV格式错误！");
            }
            else if (!check5)
            {
                throw new ValueParseException("DS格式错误！");
            }
        }

        public override void UpdateCommentContent()
        {
            if (SourceValue != WordValue.Null)
            {
                _commentTextBlock[0].Text = String.Format("{0:s}:{1:s}", SourceValue.ValueString, SourceValue.Comment);
            }
            else
            {
                _commentTextBlock[0].Text = String.Empty;
            }
            if (SourceStart != WordValue.Null)
            {
                _commentTextBlock[1].Text = String.Format("{0:s}:{1:s}", SourceStart.ValueString, SourceStart.Comment);
            }
            else
            {
                _commentTextBlock[1].Text = String.Empty;
            }
            if (SourceCount != WordValue.Null)
            {
                _commentTextBlock[2].Text = String.Format("{0:s}:{1:s}", SourceCount.ValueString, SourceCount.Comment);
            }
            else
            {
                _commentTextBlock[2].Text = String.Empty;
            }
            if (DestinationValue != WordValue.Null)
            {
                _commentTextBlock[0].Text = String.Format("{0:s}:{1:s}", DestinationValue.ValueString, DestinationValue.Comment);
            }
            else
            {
                _commentTextBlock[0].Text = String.Empty;
            }
            if (DestinationStart != WordValue.Null)
            {
                _commentTextBlock[1].Text = String.Format("{0:s}:{1:s}", DestinationStart.ValueString, DestinationStart.Comment);
            }
            else
            {
                _commentTextBlock[1].Text = String.Empty;
            }
        }

        private SMOVModel model;

        private TextBlock[] _commentTextBlock = new TextBlock[] {
            new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock() };

        public WordValue SourceValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.SoruceValue;
            }
            set
            {
                if (model == null) return;
                model.SoruceValue = value;
                MiddleTextBlock1.Text = String.Format("SV:{0:s}", value.ValueShowString);
            }
        }
        
        public WordValue SourceStart
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.SourceStart;
            }
            set
            {
                if (model == null) return;
                model.SourceStart = value;
                MiddleTextBlock2.Text = String.Format("SS:{0:s}", value.ValueShowString);
            }
        }

        public WordValue SourceCount
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.SourceCount;
            }
            set
            {
                if (model == null) return;
                model.SourceCount = value;
                MiddleTextBlock3.Text = String.Format("SC:{0:s}", value.ValueShowString);
            }
        }

        public WordValue DestinationValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.DestinationValue;
            }
            set
            {
                if (model == null) return;
                model.DestinationValue = value;
                BottomTextBlock.Text = String.Format("DV:{0:s}", value.ValueShowString);
            }
        }

        public WordValue DestinationStart
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.DestinationStart;
            }
            set
            {
                if (model == null) return;
                model.DestinationStart = value;
                MiddleTextBlock4.Text = String.Format("TS:{0:s}", value.ValueShowString);
            }
        }

        public SMOVViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new SMOVModel();
            CommentArea.Children.Add(_commentTextBlock[0]);
            CommentArea.Children.Add(_commentTextBlock[1]);
            CommentArea.Children.Add(_commentTextBlock[2]);
            CommentArea.Children.Add(_commentTextBlock[3]);
            CommentArea.Children.Add(_commentTextBlock[4]);
        }

    }
}
