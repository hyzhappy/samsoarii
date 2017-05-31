using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class FORViewModel : OutputRectBaseViewModel
    {
        private FORModel _model;
        public WordValue Count
        {
            get
            {
                return _model.Count;
            }
            set
            {
                _model.Count = value;
                BottomTextBlock.Text = string.Format("CNT:{0}", _model.Count.ValueShowString);
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
                _model = value as FORModel;
                Count = _model.Count;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock()};
        public override string InstructionName { get { return "FOR"; } }
        public FORViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new FORModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
        }

        public override BaseViewModel Clone()
        {
            return new FORViewModel();
        }

        private static int CatalogID { get { return 1100; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Count.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(Count);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch(ValueParseException exception)
            {
                Count = WordValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("N",Count);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring = Count.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyWordRegex3,ValueParser.VerifyIntKHValueRegex }))
            {
                Count = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring, Count.ValueString);
                ValueCommentManager.UpdateComment(Count, valueStrings[1]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (Count != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", Count.ValueString, Count.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
        }
    }
}
