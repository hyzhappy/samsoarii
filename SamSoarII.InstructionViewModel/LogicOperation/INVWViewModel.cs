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
    public class INVWViewModel : OutputRectBaseViewModel
    {
        private INVWModel _model;
        private WordValue InputValue
        {
            get
            {
                return _model.InputValue;
            }
            set
            {
                _model.InputValue = value;
                MiddleTextBlock1.Text = _model.InputValue.ToShowString();
            }
        }
        private WordValue OutputValue
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
                _model = value as INVWModel;
                InputValue = _model.InputValue;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "INVW"; } }
        public INVWViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new INVWModel();
        }

        public override BaseViewModel Clone()
        {
            return new INVWViewModel();
        }

        private static int CatalogID { get { return 500; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue.ToString());
            result.Add(OutputValue.ToString());
            return result;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue = WordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = WordValue.Null;
            }
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
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
