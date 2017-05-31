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
    public class FMOVViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "FMOV";
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
                if (value is FMOVModel)
                {
                    this.model = (FMOVModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except FMOVModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new FMOVViewModel();
        }

        public override int GetCatalogID()
        {
            return 1817;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            result.Add(CountValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(SourceValue);
            result.Add(DestinationValue);
            result.Add(CountValue);
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
                DestinationValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                DestinationValue = WordValue.Null;
            }
            try
            {
                CountValue = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                CountValue = WordValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {

            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("SV:", SourceValue);
            dialog.ShowLine4("TV:", DestinationValue);
            dialog.ShowLine6("CT:", CountValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = DestinationValue.ValueString;
            var oldvaluestring3 = CountValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex });
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex1});
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2 && check3)
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                DestinationValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                CountValue = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DestinationValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, CountValue.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(CountValue, valueStrings[5]);
            }
            else if (!check1)
            {
                throw new ValueParseException("SV格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("TV格式错误！");
            }
            else if (!check3)
            {
                throw new ValueParseException("CT格式错误！");
            }
        }


        public override void UpdateCommentContent()
        {
            if (SourceValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", SourceValue.ValueString, SourceValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (DestinationValue != WordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", DestinationValue.ValueString, DestinationValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (CountValue != WordValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0:s}:{1:s}", CountValue.ValueString, CountValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
        }

        private FMOVModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] {new TextBlock(), new TextBlock(), new TextBlock()};

        public WordValue SourceValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.SourceValue;
            }
            set
            {
                if (model == null) return;
                model.SourceValue = value;
                MiddleTextBlock1.Text = String.Format("SV:{0:s}", value.ValueShowString);
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
                BottomTextBlock.Text = String.Format("TV:{0:s}", value.ValueShowString);
            }
        }

        public WordValue CountValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.CountValue;
            }
            set
            {
                if (model == null) return;
                model.CountValue = value;
                MiddleTextBlock2.Text = String.Format("CT:{0:s}", value.ValueShowString);
            }
        }

        public FMOVViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new FMOVModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }
        
    }
}
