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
    class XCHFViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "XCHF";
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
                if (value is XCHFModel)
                {
                    this.model = (XCHFModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except XCHFModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new XCHFViewModel();
        }

        public override int GetCatalogID()
        {
            return 1813;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(LeftValue.ValueString);
            result.Add(RightValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                LeftValue = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                LeftValue = FloatValue.Null;
            }
            try
            {
                RightValue = ValueParser.ParseFloatValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                RightValue = FloatValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {

            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("L:", LeftValue);
            dialog.ShowLine5("R:", RightValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = LeftValue.ValueString;
            var oldvaluestring2 = RightValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyFloatRegex});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyFloatRegex});
            if (check1 && check2)
            {
                LeftValue = ValueParser.ParseFloatValue(valueStrings[0], contextDevice);
                RightValue = ValueParser.ParseFloatValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, LeftValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, RightValue.ValueString);
                ValueCommentManager.UpdateComment(LeftValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(RightValue, valueStrings[3]);
            }
            else if (!check1)
            {
                throw new ValueParseException("L格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("R格式错误！");
            }
        }


        public override void UpdateCommentContent()
        {
            if (LeftValue != FloatValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", LeftValue.ValueString, LeftValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }

            if (RightValue != FloatValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", RightValue.ValueString, RightValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
        }

        private XCHFModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] {new TextBlock(), new TextBlock()};

        public FloatValue LeftValue
        {
            get
            {
                if (model == null) return FloatValue.Null;
                return model.LeftValue;
            }
            set
            {
                if (model == null) return;
                model.LeftValue = value;
                MiddleTextBlock1.Text = String.Format("L:{0:s}", value.ValueShowString);
            }
        }

        public FloatValue RightValue
        {
            get
            {
                if (model == null) return FloatValue.Null;
                return model.RightValue;
            }
            set
            {
                if (model == null) return;
                model.RightValue = value;
                MiddleTextBlock2.Text = String.Format("R:{0:s}", value.ValueShowString);
            }
        }
        

        public XCHFViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new XCHFModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }
        
    }
}
