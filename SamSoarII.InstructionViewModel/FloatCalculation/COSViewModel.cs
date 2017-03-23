using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

using System.Windows;

namespace SamSoarII.LadderInstViewModel
{
    public class COSViewModel : OutputRectBaseViewModel
    {
        private COSModel _model;
        public FloatValue InputValue
        {
            get
            {
                return _model.InputValue;
            }
            set
            {
                _model.InputValue = value;
                MiddleTextBlock1.Text = _model.InputValue.ValueShowString;
            }
        }
        public FloatValue OutputValue
        {
            get
            {
                return _model.OutputValue;
            }
            set
            {
                _model.OutputValue = value;
                BottomTextBlock.Text = _model.OutputValue.ValueShowString;
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
                _model = value as COSModel;
                InputValue = _model.InputValue;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "COS"; } }
        public COSViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new COSModel();
        }

        public override BaseViewModel Clone()
        {
            return new COSViewModel();
        }

        private static int CatalogID { get { return 706; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }
        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue = FloatValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseFloatValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = FloatValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("In");
            dialog.ShowLine5("Out");
            return dialog;
        }
    }
}
