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

namespace SamSoarII.LadderInstViewModel
{
    public class TOFViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "TOF";
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
                if (value is TOFModel)
                {
                    this.model = (TOFModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except TOFModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new TOFViewModel();
        }

        public override int GetCatalogID()
        {
            return 902;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(TimerValue.ValueString);
            result.Add(EndValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                TimerValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                TimerValue = WordValue.Null;
            }
            try
            {
                EndValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                EndValue = WordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine3("TV:", TimerValue);
            dialog.ShowLine5("SV:", EndValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = TimerValue.ValueString;
            var oldvaluestring2 = EndValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex4});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex});
            if (check1 && check2)
            {
                TimerValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                EndValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, TimerValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, EndValue.ValueString);
                ValueCommentManager.UpdateComment(TimerValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(EndValue, valueStrings[3]);
            }
            else if (!check1)
            {
                throw new ValueParseException("TV格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("SV格式错误！");
            }
        }


        public override void UpdateCommentContent()
        {
            if (TimerValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", TimerValue.ValueString, TimerValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (EndValue != WordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", EndValue.ValueString, EndValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
        }

        private TOFModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };

        public WordValue TimerValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.TimerValue;
            }
            set
            {
                if (model == null) return;
                model.TimerValue = value;
                MiddleTextBlock1.Text = String.Format("{0:s}", model.TimerValue.ValueShowString);
            }
        }

        public WordValue EndValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.EndValue;
            }
            set
            {
                if (model == null) return;
                model.EndValue = value;
                MiddleTextBlock2.Text = String.Format("SV:{0:s}", model.EndValue.ValueShowString);
            }
        }


        public TOFViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new TOFModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

    }
}
