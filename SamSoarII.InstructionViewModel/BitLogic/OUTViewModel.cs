using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
namespace SamSoarII.LadderInstViewModel
{
    public class OUTViewModel : OutputBaseViewModel
    {
        private OUTModel _model = new OUTModel();
        private BitValue Value
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

        public string ValueComment { get; set; }
        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                _model = value as OUTModel;
                Value = _model.Value;
            }
        }
        public override string InstructionName { get { return "OUT"; } }


        private TextBlock _commentTextBlock1 = new TextBlock();
        public OUTViewModel()
        {
            Model = new OUTModel();
            CommentArea.Children.Add(_commentTextBlock1);
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("Bit", Value);
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new OUTViewModel();
        }

        public static int CatalogID { get { return 209; } }

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
            catch(ValueParseException exception)
            {
                Value = BitValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ValueString);
            return result;
        }
    }
}
