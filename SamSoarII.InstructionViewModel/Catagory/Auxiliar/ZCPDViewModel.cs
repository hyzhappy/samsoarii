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
    public class ZCPDViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "ZCPD";
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
                if (value is ZCPDModel)
                {
                    this.model = (ZCPDModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except ZCPDModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new ZCPDViewModel();
        }

        public override int GetCatalogID()
        {
            return 1807;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue1.ValueString);
            result.Add(InputValue2.ValueString);
            result.Add(InputValue3.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(InputValue1);
            result.Add(InputValue2);
            result.Add(InputValue3);
            result.Add(OutputValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue1 = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                InputValue1 = DoubleWordValue.Null;
            }
            try
            {
                InputValue2 = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                InputValue2 = DoubleWordValue.Null;
            }
            try
            {
                InputValue3 = ValueParser.ParseDoubleWordValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                InputValue3 = DoubleWordValue.Null;
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
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(4);
            dialog.Title = InstructionName;
            dialog.ShowLine1("IN1:", InputValue1);
            dialog.ShowLine3("IN2:", InputValue2);
            dialog.ShowLine5("IN3:", InputValue3);
            dialog.ShowLine7("OUT:", OutputValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

            var oldvaluestring1 = InputValue1.ValueString;
            var oldvaluestring2 = InputValue2.ValueString;
            var oldvaluestring3 = InputValue3.ValueString;
            var oldvaluestring4 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyDoubleWordRegex1, ValueParser.VerifyIntKHValueRegex });
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyDoubleWordRegex1, ValueParser.VerifyIntKHValueRegex });
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyDoubleWordRegex1, ValueParser.VerifyIntKHValueRegex });
            bool check4 = ValueParser.CheckValueString(valueStrings[6], new Regex[] {
                ValueParser.VerifyBitRegex3 });
            if (check1 && check2 && check3 && check4)
            {
                InputValue1 = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                InputValue2 = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                InputValue3 = ValueParser.ParseDoubleWordValue(valueStrings[4], contextDevice);
                OutputValue = ValueParser.ParseBitValue(valueStrings[6], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, InputValue1.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, InputValue2.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, InputValue3.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring4, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(InputValue1, valueStrings[1]);
                ValueCommentManager.UpdateComment(InputValue2, valueStrings[3]);
                ValueCommentManager.UpdateComment(InputValue3, valueStrings[5]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[7]);
            }
            else if (!check1)
            {
                throw new ValueParseException("IN1格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("IN2格式错误！");
            }
            else if (!check3)
            {
                throw new ValueParseException("IN3格式错误！");
            }
            else if (!check4)
            {
                throw new ValueParseException("OUT格式错误！");
            }
        }


        public override void UpdateCommentContent()
        {
            if (InputValue1 != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", InputValue1.ValueString, InputValue1.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (InputValue2 != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", InputValue2.ValueString, InputValue2.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (InputValue3 != DoubleWordValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0:s}:{1:s}", InputValue2.ValueString, InputValue2.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
            if (OutputValue != BitValue.Null)
            {
                _commentTextBlocks[3].Text = String.Format("{0:s}:{1:s}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[3].Text = String.Empty;
            }
        }

        private ZCPDModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock(), new TextBlock() };
        
        public DoubleWordValue InputValue1
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.InputValue1;
            }
            set
            {
                if (model == null) return;
                model.InputValue1 = value;
                MiddleTextBlock1.Text = String.Format("IN1:{0:s}", value.ValueShowString);
            }
        }

        public DoubleWordValue InputValue2
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.InputValue2;
            }
            set
            {
                if (model == null) return;
                model.InputValue2 = value;
                MiddleTextBlock2.Text = String.Format("IN2:{0:s}", value.ValueShowString);
            }
        }

        public DoubleWordValue InputValue3
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.InputValue3;
            }
            set
            {
                if (model == null) return;
                model.InputValue3 = value;
                MiddleTextBlock3.Text = String.Format("IN3:{0:s}", value.ValueShowString);
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

        public ZCPDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new ZCPDModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
            CommentArea.Children.Add(_commentTextBlocks[3]);
        }

    }
}
