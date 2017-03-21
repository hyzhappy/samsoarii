using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class LDWNEViewModel : InputBaseViewModel
    {
        private LDWNEModel _model;
        private WordValue Value1
        {
            get
            {
                return _model.Value1;
            }
            set
            {
                _model.Value1 = value;
                ValueTextBlock.Text = _model.Value1.ValueShowString;
            }
        }
        private WordValue Value2
        {
            get
            {
                return _model.Value2;
            }
            set
            {
                _model.Value2 = value;
                Value2TextBlock.Text = _model.Value2.ValueShowString;
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
                _model = value as LDWNEModel;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }
        private TextBlock[] _commentTextBlocks = { new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "LDWNE"; } }
        public LDWNEViewModel()
        {
            CenterTextBlock.Text = "W<>";
            Model = new LDWNEModel();
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("W1");
            dialog.ShowLine5("W2");
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new LDWNEViewModel();
        }

        public static int CatalogID { get { return 301; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Value1 = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value1 = WordValue.Null;
            }
            try
            {
                Value2 = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Value2 = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value1.ValueString);
            result.Add(Value2.ValueString);
            return result;
        }

    }
}
