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
using SamSoarII.LadderInstModel.HighCount;

namespace SamSoarII.LadderInstViewModel
{
    public class HCNTViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "HCNT";
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
                if (value is HCNTModel)
                {
                    this.model = (HCNTModel)(value);
                    CountValue = model.CountValue;
                    DefineValue = model.DefineValue;
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except HCNTModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new HCNTViewModel();
        }

        public override int GetCatalogID()
        {
            return 1700;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(CountValue.ValueString);
            result.Add(DefineValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                CountValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
                DefineValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                CountValue = DoubleWordValue.Null;
                DefineValue = DoubleWordValue.Null;
            }      
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("CT:", CountValue);
            dialog.ShowLine5("SV:", DefineValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = CountValue.ValueString;
            var oldvaluestring2 = DefineValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyDoubleWordRegex3});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyDoubleWordRegex1, ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2)
            {
                CountValue = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                DefineValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, CountValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DefineValue.ValueString);
                ValueCommentManager.UpdateComment(CountValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DefineValue, valueStrings[3]);
            }
            else if (!check1)
            {
                throw new ValueParseException("计数器输入错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("预设值输入错误！");
            }

        }
 
        public override void UpdateCommentContent()
        {
            if (CountValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", CountValue.ValueString, CountValue.Comment);    
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (DefineValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", DefineValue.ValueString, DefineValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
        }
        
        private HCNTModel model;
        
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };

        public DoubleWordValue CountValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.CountValue;
            }
            set
            {
                if (model == null) return;
                model.CountValue = value;
                MiddleTextBlock1.Text = String.Format("CT:{0}", value.ValueShowString);
            }
        }


        public DoubleWordValue DefineValue
        {
            get
            {
                if (model == null) return DoubleWordValue.Null;
                return model.DefineValue;
            }
            set
            {
                if (model == null) return;
                model.DefineValue = value;
                BottomTextBlock.Text = String.Format("SV:{0}", value.ValueShowString);
            }
        }

        public HCNTViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new HCNTModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

    }
}
