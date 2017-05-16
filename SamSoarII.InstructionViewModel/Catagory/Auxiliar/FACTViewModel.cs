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
    public class FACTViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "FACT";
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
                if (value is FACTModel)
                {
                    this.model = (FACTModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except FACTModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new FACTViewModel();
        }

        public override int GetCatalogID()
        {
            return 1802;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(InputValue);
            result.Add(OutputValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                InputValue = WordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                OutputValue = DoubleWordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {

            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("IN:", InputValue);
            dialog.ShowLine5("OUT:", OutputValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = InputValue.ValueString;
            var oldvaluestring2 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyDoubleWordRegex1});
            if (check1 && check2)
            {
                InputValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                OutputValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, InputValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(InputValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[3]);
            }
            else if (!check1)
            {
                throw new ValueParseException("IN格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("OUT格式错误！");
            }
        }


        public override void UpdateCommentContent()
        {
            if (InputValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", InputValue.ValueString, InputValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (OutputValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
        }

        private FACTModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] {new TextBlock(), new TextBlock()};

        public WordValue InputValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.InputValue;
            }
            set
            {
                if (model == null) return;
                model.InputValue = value;
                MiddleTextBlock1.Text = String.Format("IN:{0:s}", value.ValueShowString);
            }
        }

        public DoubleWordValue OutputValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.OutputValue;
            }
            set
            {
                if (model == null) return;
                model.OutputValue = value;
                BottomTextBlock.Text = String.Format("OUT:{0:s}", value.ValueShowString);
            }
        }
        

        public FACTViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new FACTModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }
        
    }
}
