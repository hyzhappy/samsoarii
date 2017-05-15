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
    public class CTDViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "CTD";
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
                if (value is CTDModel)
                {
                    this.model = (CTDModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except CTDModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new CTDViewModel();
        }

        public override int GetCatalogID()
        {
            return 1001;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(CountValue.ValueString);
            result.Add(StartValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(CountValue);
            result.Add(StartValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                CountValue = ValueParser.ParseWordValue(valueStrings[0]);
                StartValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                try
                {
                    CountValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
                    StartValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
                }
                catch (ValueParseException)
                {
                    CountValue = WordValue.Null;
                    StartValue = WordValue.Null;
                }
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("CT:", CountValue);
            dialog.ShowLine5("SV:", StartValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = CountValue.ValueString;
            var oldvaluestring2 = StartValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyDoubleWordRegex3});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2)
            {
                try
                {
                    CountValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                    StartValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                }
                catch (ValueParseException)
                {
                    CountValue = ValueParser.ParseDoubleWordValue(valueStrings[0], contextDevice);
                    StartValue = ValueParser.ParseDoubleWordValue(valueStrings[2], contextDevice);
                }
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, CountValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, StartValue.ValueString);
                ValueCommentManager.UpdateComment(CountValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(StartValue, valueStrings[3]);
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
            if (StartValue != WordValue.Null && StartValue != DoubleWordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", StartValue.ValueString, StartValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
        }

        private CTDModel model;

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


        public IValueModel StartValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.StartValue;
            }
            set
            {
                if (model == null) return;
                model.StartValue = value;
                MiddleTextBlock2.Text = String.Format("SV:{0}", value.ValueShowString);
            }
        }

        public CTDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new CTDModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

    }
}
