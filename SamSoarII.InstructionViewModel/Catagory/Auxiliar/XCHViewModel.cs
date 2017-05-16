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
    public class XCHViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "XCH";
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
                if (value is XCHModel)
                {
                    this.model = (XCHModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except XCHModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new XCHViewModel();
        }

        public override int GetCatalogID()
        {
            return 1811;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(LeftValue.ValueString);
            result.Add(RightValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(LeftValue);
            result.Add(RightValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                LeftValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                LeftValue = WordValue.Null;
            }
            try
            {
                RightValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                RightValue = WordValue.Null;
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
                ValueParser.VerifyWordRegex1});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex1});
            if (check1 && check2)
            {
                LeftValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                RightValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
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
            if (LeftValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", LeftValue.ValueString, LeftValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }

            if (RightValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", RightValue.ValueString, RightValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
        }

        private XCHModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] {new TextBlock(), new TextBlock()};

        public WordValue LeftValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.LeftValue;
            }
            set
            {
                if (model == null) return;
                model.LeftValue = value;
                MiddleTextBlock1.Text = String.Format("L:{0:s}", value.ValueShowString);
            }
        }

        public WordValue RightValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.RightValue;
            }
            set
            {
                if (model == null) return;
                model.RightValue = value;
                MiddleTextBlock2.Text = String.Format("R:{0:s}", value.ValueShowString);
            }
        }
        

        public XCHViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new XCHModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }
        
    }
}
