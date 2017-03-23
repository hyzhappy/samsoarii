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
    public class MULDViewModel : OutputRectBaseViewModel
    {
        private MULDModel _model;
        private DoubleWordValue InputValue1
        {
            get
            {
                return _model.InputValue1;
            }
            set
            {
                _model.InputValue1 = value;
                MiddleTextBlock1.Text = _model.InputValue1.ValueShowString;
            }
        }
        private DoubleWordValue InputValue2
        {
            get
            {
                return _model.InputValue2;
            }
            set
            {
                _model.InputValue2 = value;
                MiddleTextBlock2.Text = _model.InputValue2.ValueShowString;
            }
        }
        private DoubleWordValue OutputValue
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
                _model = value as MULDModel;
                InputValue1 = _model.InputValue1;
                InputValue2 = _model.InputValue2;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "MULD"; } }
        public MULDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            _model = new MULDModel();
        }


        public override BaseViewModel Clone()
        {
            return new MULDViewModel();
        }

        private static int CatalogID { get { return 806; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue1.ValueString);
            result.Add(InputValue2.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                InputValue1 = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue1 = DoubleWordValue.Null;
            }
            try
            {
                InputValue2 = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                InputValue2 = DoubleWordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseDoubleWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = DoubleWordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("In1");
            dialog.ShowLine4("In2");
            dialog.ShowLine6("Out");
            return dialog;
        }
    }
}
