using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using System.Windows;
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class SETViewModel : OutputBaseViewModel 
    {
        private SETModel _model = new SETModel();

        public BitValue Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                _model.Value = value;
                ValueTextBlock.Text = _model.Value.ValueShowString;
            }
        }

        public WordValue Count
        {
            get
            {
                return _model.Count;
            }
            set
            {
                _model.Count = value;
                CountTextBlock.Text = _model.Count.ValueShowString;
            }
        }

        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                _model = value as SETModel;
                Value = _model.Value;
                Count = _model.Count;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "SET"; } }
        public SETViewModel()
        {
            Model = new SETModel();
            CenterTextBlock.Text = "S";
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("Bit", Value);
            dialog.ShowLine5("Bit", Count);
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new SETViewModel();
        }

        public static int CatalogID { get { return 211; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Value = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch
            {
                Value = BitValue.Null;
            }
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch
            {
                Count = WordValue.Null;
            }
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = Value.ValueString;
            var oldvaluestring2 = Count.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyBitRegex2 }) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyIntKHValueRegex }))
            {
                Value = ValueParser.ParseBitValue(valueStrings[0], contextDevice);
                Count = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, Value.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, Count.ValueString);
                ValueCommentManager.UpdateComment(Value, valueStrings[1]);
                ValueCommentManager.UpdateComment(Count, valueStrings[3]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ValueString);
            result.Add(Count.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(Value);
            result.Add(Count);
            return result;
        }
        public override void UpdateCommentContent()
        {
            if (Value != BitValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", Value.ValueString, Value.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if(Count != WordValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", Count.ValueString, Count.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
        }
    }
}
