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
    public class ANDDViewModel : OutputRectBaseViewModel
    {
        private ANDDModel _model;
        private DoubleWordValue InputValue1
        {
            get
            {
                return _model.InputValue1;
            }
            set
            {
                _model.InputValue1 = value;
                MiddleTextBlock1.Text = _model.InputValue1.ToShowString();
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
                MiddleTextBlock2.Text = _model.InputValue2.ToShowString();
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
                BottomTextBlock.Text = _model.OutputValue.ToShowString();
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
                _model = value as ANDDModel;
                InputValue1 = _model.InputValue1;
                InputValue2 = _model.InputValue2;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "ANDD"; } }
        public ANDDViewModel()
        {
            TopTextBlock.Text = "ANDD";
            Model = new ANDDModel();
        }

        public override BaseViewModel Clone()
        {
            return new ANDDViewModel();
        }

        private static int CatalogID { get { return 503; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue1.ToString());
            result.Add(InputValue2.ToString());
            result.Add(OutputValue.ToString());
            return result;
        }

        public override void ParseValue(List<string> valueStrings)
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

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "ANDD";
            dialog.ShowLine2("In1");
            dialog.ShowLine4("In2");
            dialog.ShowLine6("Out");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> temp = new List<string>();
                    temp.Add(dialog.ValueString2);
                    temp.Add(dialog.ValueString4);
                    temp.Add(dialog.ValueString6);
                    ParseValue(temp);
                    dialog.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            };
            dialog.ShowDialog();
        }
    }
}
