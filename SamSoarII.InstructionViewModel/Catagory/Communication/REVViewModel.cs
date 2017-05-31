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
using SamSoarII.LadderInstModel.Communication;

namespace SamSoarII.LadderInstViewModel
{
    public class REVViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "REV";
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
                if (value is REVModel)
                {
                    this.model = (REVModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except REVModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new REVViewModel();
        }

        public override int GetCatalogID()
        {
            return 1502;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(COMPort.ValueString);
            result.Add(BaseValue.ValueString);
            result.Add(CountValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(COMPort);
            result.Add(BaseValue);
            result.Add(CountValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                COMPort = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                COMPort = WordValue.Null;
            }
            try
            {
                BaseValue = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                BaseValue = WordValue.Null;
            }
            try
            {
                CountValue = ValueParser.ParseWordValue(valueStrings[4]);
            }
            catch (ValueParseException)
            {
                CountValue = WordValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("COM:", COMPort);
            dialog.ShowLine4("ADDR:", BaseValue);
            dialog.ShowLine6("LEN:", CountValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = COMPort.ValueString;
            var oldvaluestring2 = BaseValue.ValueString;
            var oldvaluestring3 = CountValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyIntKHValueRegex });
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex3 });
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex });
            if (check1 && check2 && check3)
            {
                COMPort = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                BaseValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                CountValue = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, COMPort.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, BaseValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, CountValue.ValueString);
                ValueCommentManager.UpdateComment(COMPort, valueStrings[1]);
                ValueCommentManager.UpdateComment(BaseValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(CountValue, valueStrings[5]);
            }
            else if (!check1)
            {
                throw new ValueParseException("COM格式错误！");
            }
            else if (!check2)
            {
                throw new ValueParseException("ADDR格式错误！");
            }
            else if (!check3)
            {
                throw new ValueParseException("LEN格式错误！");
            }
        }
        
        public override void UpdateCommentContent()
        {
            if (COMPort != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", COMPort.ValueString, COMPort.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (BaseValue != WordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0:s}:{1:s}", BaseValue.ValueString, BaseValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (CountValue != WordValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0:s}:{1:s}", CountValue.ValueString, CountValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
        }

        private REVModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] {new TextBlock(), new TextBlock(), new TextBlock()};

        public WordValue COMPort
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.COMPort;
            }
            set
            {
                if (model == null) return;
                model.COMPort = value;
                MiddleTextBlock1.Text = value.ValueShowString;
            }
        }

        public WordValue BaseValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.BaseValue;
            }
            set
            {
                if (model == null) return;
                model.BaseValue = value;
                MiddleTextBlock2.Text = value.ValueShowString;
            }
        }

        public WordValue CountValue
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
                MiddleTextBlock3.Text = value.ValueShowString;
            }
        }

        public REVViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new REVModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }
        
    }
}
