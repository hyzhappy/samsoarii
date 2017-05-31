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
    public class TRDViewModel : OutputRectBaseViewModel
    {
        private TRDModel _model;
        public WordValue StartValue
        {
            get
            {
                return _model.StartValue;
            }
            set
            {
                _model.StartValue = value;
                BottomTextBlock.Text = string.Format("T : {0}", _model.StartValue.ValueShowString);
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
                _model = value as TRDModel;
                StartValue = _model.StartValue;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock()};
        public override string InstructionName { get { return "TRD"; } }
        public TRDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new TRDModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
        }

        public override BaseViewModel Clone()
        {
            return new TRDViewModel();
        }

        private static int CatalogID { get { return 1400; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(StartValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(StartValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                StartValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch(ValueParseException exception)
            {
                StartValue = WordValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("T",StartValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring = StartValue.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyWordRegex3 }))
            {
                StartValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring, StartValue.ValueString);
                ValueCommentManager.UpdateComment(StartValue, valueStrings[1]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (StartValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", StartValue.ValueString, StartValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
        }
    }
}
