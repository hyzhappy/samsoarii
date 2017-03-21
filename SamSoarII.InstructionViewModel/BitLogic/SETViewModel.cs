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
        private TextBlock _commentTextBlock1 = new TextBlock();
        private TextBlock _commentTextBlock2 = new TextBlock();
        public override string InstructionName { get { return "SET"; } }
        public SETViewModel()
        {
            Model = new SETModel();
            CenterTextBlock.Text = "S";
            CommentArea.Children.Add(_commentTextBlock1);
            CommentArea.Children.Add(_commentTextBlock2);
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
            Value = ValueParser.ParseBitValue(valueStrings[0]);
            ValueCommentManager.UpdateComment(Value, valueStrings[1]);
            Count = ValueParser.ParseWordValue(valueStrings[2]);
            ValueCommentManager.UpdateComment(Count, valueStrings[3]);
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ValueString);
            result.Add(Count.ValueString);
            return result;
        }

        public override void UpdateCommentContent()
        {
            if (Value != BitValue.Null)
            {
                _commentTextBlock1.Text = string.Format("{0}:{1}", Value.ValueString, Value.Comment);
            }
            else
            {
                _commentTextBlock1.Text = string.Empty;
            }
            if(Count != WordValue.Null)
            {
                _commentTextBlock2.Text = string.Format("{0}:{1}", Count.ValueString, Count.Comment);
            }
            else
            {
                _commentTextBlock2.Text = string.Empty;
            }
        }
    }
}
