using SamSoarII.LadderInstModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel.Counter
{
    public class CTUViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "CTU";
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
                if (value is CTUModel)
                {
                    this.model = (CTUModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except CTUModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new CTUViewModel();
        }

        public override int GetCatalogID()
        {
            return 1000;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(CountValue.ValueString);
            result.Add(EndValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(CountValue);
            result.Add(EndValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                CountValue = ValueParser.ParseWordValue(valueStrings[0]);
                EndValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                try
                {
                    CountValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
                    EndValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
                }
                catch (ValueParseException)
                {
                    CountValue = WordValue.Null;
                    EndValue = WordValue.Null;
                }
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("CT:", CountValue);
            dialog.ShowLine5("SV:", EndValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = CountValue.ValueString;
            var oldvaluestring2 = EndValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyDoubleWordRegex3});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2)
            {
                try
                {
                    CountValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                    EndValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                }
                catch (ValueParseException)
                {
                    CountValue = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                    EndValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                }
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, CountValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, EndValue.ValueString);
                ValueCommentManager.UpdateComment(CountValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(EndValue, valueStrings[3]);
            }
            else if (!check1)
            {
                throw new ValueParseException("计数器输入错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("最大值输入错误！");
            }

        }
 
        public override void UpdateCommentContent()
        {
            if (CountValue != WordValue.Null && CountValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", CountValue.ValueString, CountValue.Comment);    
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (EndValue != WordValue.Null && EndValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", EndValue.ValueString, EndValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
        }
        
        private CTUModel model;
        
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };

        public IValueModel CountValue
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
                MiddleTextBlock1.Text = String.Format("CT:{0}", value.ValueShowString);
            }
        }


        public IValueModel EndValue
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
                MiddleTextBlock2.Text = String.Format("SV:{0}", value.ValueShowString);
            }
        }

        public CTUViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new CTUModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

    }
}
