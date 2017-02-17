using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;


namespace SamSoarII.InstructionViewModel
{
    public class LDWGViewModel : InputBaseViewModel
    {
        private LDWGModel _model;
        private WordValue Value1
        {
            get
            {
                return _model.Value1;
            }
            set
            {
                _model.Value1 = value;
                ValueTextBlock.Text = _model.Value1.ToShowString();
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
                Value2TextBlock.Text = _model.Value2.ToShowString();
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
                _model = value as LDWGModel;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }
        public override string InstructionName { get { return "LDWG"; } }
        public LDWGViewModel()
        {
            CenterTextBlock.Text = "W>";
            Model = new LDWGModel();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine3("W1");
            dialog.ShowLine5("W2");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> temp = new List<string>();
                    temp.Add(dialog.ValueString3);
                    temp.Add(dialog.ValueString5);
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

        public override BaseViewModel Clone()
        {
            return new LDWGViewModel();
        }

        public static int CatalogID { get { return 304; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(List<string> valueStrings)
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
            result.Add(Value1.ToString());
            result.Add(Value2.ToString());
            return result;
        }
    }
}
