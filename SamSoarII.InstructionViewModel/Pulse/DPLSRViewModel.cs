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
    public class DPLSRViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "DPLSR";
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
                if (value is DPLSRModel)
                {
                    this.model = (DPLSRModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except DPLSRModel");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new DPLSRViewModel();
        }

        public override int GetCatalogID()
        {
            return 1607;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(ArgumentValue.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                ArgumentValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                ArgumentValue = DoubleWordValue.Null;
            }
            try
            {
                VelocityValue = ValueParser.ParseDoubleWordValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                VelocityValue = DoubleWordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseBitValue(valueStrings[4]);
            }
            catch (ValueParseException)
            {
                OutputValue = BitValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.ShowLine2("D:", ArgumentValue);
            dialog.ShowLine4("V:", VelocityValue);
            dialog.ShowLine6("OUT:", OutputValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = ArgumentValue.ValueString;
            var oldvaluestring2 = VelocityValue.ValueString;
            var oldvaluestring3 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyDoubleWordRegex2});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyDoubleWordRegex2, ValueParser.VerifyIntKHValueRegex});
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyBitRegex4});
            if (check1 && check2 && check3) 
            {
                ArgumentValue = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                VelocityValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                OutputValue = ValueParser.ParseBitValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, ArgumentValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, VelocityValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(ArgumentValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(VelocityValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[5]);
            }
            else if (!check1)
            {
                throw new ValueParseException("D格式非法！");
            }
            else if (!check2)
            {
                throw new ValueParseException("V格式非法");
            }
            else if (!check3)
            {
                throw new ValueParseException("OUT格式非法");
            }
        }

        public override void UpdateCommentContent()
        {
            if (ArgumentValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0}:{1}", ArgumentValue.ValueString, ArgumentValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (VelocityValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0}:{1}", VelocityValue.ValueString, VelocityValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (OutputValue != BitValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
        }

        private DPLSRModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock() };

        public DoubleWordValue ArgumentValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.ArgumentValue;
            }
            set
            {
                if (model == null) return;
                model.ArgumentValue = value;
                MiddleTextBlock1.Text = String.Format("D:{0:s}", value.ValueShowString);
            }
        }

        public DoubleWordValue VelocityValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.VelocityValue;
            }
            set
            {
                if (model == null) return;
                model.VelocityValue = value;
                MiddleTextBlock2.Text = String.Format("V:{0:s}", value.ValueShowString);
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

        public DPLSRViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new DPLSRModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }

    }
}
