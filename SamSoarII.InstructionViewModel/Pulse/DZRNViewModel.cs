using System;
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
using SamSoarII.LadderInstModel.Pulse;

namespace SamSoarII.LadderInstViewModel.Pulse
{
    public class DZRNViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "DZRN";
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
                if (value is DZRNModel)
                {
                    this.model = (DZRNModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except DZRNModel");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new DZRNViewModel();
        }

        public override int GetCatalogID()
        {
            return 1613;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(BackValue.ValueString);
            result.Add(CrawValue.ValueString);
            result.Add(SignalValue.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                BackValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                BackValue = DoubleWordValue.Null;
            }
            try
            {
                CrawValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                CrawValue = DoubleWordValue.Null;
            }
            try
            {
                SignalValue = ValueParser.ParseBitValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                SignalValue = BitValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseBitValue(valueStrings[3]);
            }
            catch (ValueParseException)
            {
                OutputValue = BitValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(4);
            dialog.ShowLine1("BV:", BackValue);
            dialog.ShowLine3("CV:", CrawValue);
            dialog.ShowLine5("SIGN:", SignalValue);
            dialog.ShowLine7("OUT:", OutputValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = BackValue.ValueString;
            var oldvaluestring2 = CrawValue.ValueString;
            var oldvaluestring3 = SignalValue.ValueString;
            var oldvaluestring4 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex3});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex});
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyBitRegex5});
            bool check4 = ValueParser.CheckValueString(valueStrings[6], new Regex[] {
                ValueParser.VerifyBitRegex4});
            if (check1 && check2 && check3 && check4) 
            {
                BackValue = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                CrawValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                SignalValue = ValueParser.ParseBitValue(valueStrings[4], contextDevice);
                OutputValue = ValueParser.ParseBitValue(valueStrings[6], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, BackValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, CrawValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, SignalValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(BackValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(CrawValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(SignalValue, valueStrings[5]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[7]);
            }
            else if (!check1)
            {
                throw new ValueParseException("BV格式非法！");
            }
            else if (!check2)
            {
                throw new ValueParseException("CV格式非法");
            }
            else if (!check3)
            {
                throw new ValueParseException("SIGN格式非法！");
            }
            else if (!check4)
            {
                throw new ValueParseException("OUT格式非法！");
            }
        }

        public override void UpdateCommentContent()
        {
            if (BackValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0}:{1}", BackValue.ValueString, BackValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (CrawValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0}:{1}", CrawValue.ValueString, CrawValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (SignalValue != BitValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0}:{1}", SignalValue.ValueString, SignalValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
            if (OutputValue != BitValue.Null)
            {
                _commentTextBlocks[3].Text = String.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[3].Text = String.Empty;
            }
        }

        private DZRNModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock()};

        public DoubleWordValue BackValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.BackValue;
            }
            set
            {
                if (model == null) return;
                model.BackValue = value;
                MiddleTextBlock1.Text = String.Format("BV:{0:s}", value.ValueShowString);
            }
        }

        public DoubleWordValue CrawValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.CrawValue;
            }
            set
            {
                if (model == null) return;
                model.CrawValue = value;
                MiddleTextBlock2.Text = String.Format("CV:{0:s}", value.ValueShowString);
            }
        }
         
        public BitValue SignalValue
        {
            get
            {
                if (model == null) return BitValue.Null;
                return model.SignalValue;
            }
            set
            {
                if (model == null) return;
                model.SignalValue = value;
                MiddleTextBlock3.Text = String.Format("SIGN:{0:s}", value.ValueShowString);
            }
        }

        public BitValue OutputValue
        {
            get
            {
                if (model == null) return BitValue.Null;
                return model.OutputValue;
            }
            set
            {
                if (model == null) return;
                model.OutputValue = value;
                BottomTextBlock.Text = String.Format("OUT:{0:s}", value.ValueShowString);
            }
        }

        public DZRNViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new DZRNModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
            CommentArea.Children.Add(_commentTextBlocks[3]);
        }

    }
}
