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
    public class PTOViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "PTO";
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
                if (value is PTOModel)
                {
                    this.model = (PTOModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except PTOModel");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new PTOViewModel();
        }

        public override int GetCatalogID()
        {
            return 1615;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(ArgumentValue.ValueString);
            result.Add(OutputValue1.ValueString);
            result.Add(OutputValue2.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(ArgumentValue);
            result.Add(OutputValue1);
            result.Add(OutputValue2);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                ArgumentValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                ArgumentValue = WordValue.Null;
            }
            try
            {
                OutputValue1 = ValueParser.ParseBitValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                OutputValue1 = BitValue.Null;
            }
            try
            {
                OutputValue2 = ValueParser.ParseBitValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                OutputValue2 = BitValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.ShowLine2("D:", ArgumentValue);
            dialog.ShowLine4("OUT1:", OutputValue1);
            dialog.ShowLine6("OUT2:", OutputValue2);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = ArgumentValue.ValueString;
            var oldvaluestring2 = OutputValue1.ValueString;
            var oldvaluestring3 = OutputValue2.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex3});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyBitRegex4});
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyBitRegex4});
            if (check1 && check2 && check3) 
            {
                ArgumentValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                OutputValue1 = ValueParser.ParseBitValue(valueStrings[4], contextDevice);
                OutputValue2 = ValueParser.ParseBitValue(valueStrings[6], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, ArgumentValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, OutputValue1.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, OutputValue2.ValueString);
                ValueCommentManager.UpdateComment(ArgumentValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(OutputValue1, valueStrings[3]);
                ValueCommentManager.UpdateComment(OutputValue2, valueStrings[5]);
            }
            else if (!check1)
            {
                throw new ValueParseException("D格式非法！");
            }
            else if (!check2)
            {
                throw new ValueParseException("OUT1格式非法！");
            }
            else if (!check3)
            {
                throw new ValueParseException("OUT2格式非法！");
            }
        }

        public override void UpdateCommentContent()
        {
            if (ArgumentValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0}:{1}", ArgumentValue.ValueString, ArgumentValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (OutputValue1 != BitValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0}:{1}", OutputValue1.ValueString, OutputValue1.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (OutputValue2 != BitValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0}:{1}", OutputValue2.ValueString, OutputValue2.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
        }

        private PTOModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock()};

        public WordValue ArgumentValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.ArgumentValue;
            }
            set
            {
                if (model == null) return;
                model.ArgumentValue = value;
                MiddleTextBlock1.Text = String.Format("D:{0:s}", value.ValueShowString);
            }
        }
        
        public BitValue OutputValue1
        {
            get
            {
                if (model == null) return BitValue.Null;
                return model.OutputValue1;
            }
            set
            {
                if (model == null) return;
                model.OutputValue1 = value;
                BottomTextBlock.Text = String.Format("OUT1:{0:s}", value.ValueShowString);
            }
        }

        public BitValue OutputValue2
        {
            get
            {
                if (model == null) return BitValue.Null;
                return model.OutputValue2;
            }
            set
            {
                if (model == null) return;
                model.OutputValue2 = value;
                BottomTextBlock2.Text = String.Format("OUT2:{0:s}", value.ValueShowString);
            }
        }

        public PTOViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new PTOModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }

    }
}
