using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using System.Windows;

using SamSoarII.UserInterface;
namespace SamSoarII.InstructionViewModel
{
    public class SETIMViewModel : OutputBaseViewModel
    {
        private SETIMModel _model = new SETIMModel();

        public BitValue Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                _model.Value = value;
                ValueTextBlock.Text = _model.Value.ToShowString();
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
                CountTextBlock.Text = _model.Count.ToShowString();
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
                _model = value as SETIMModel;
                Value = _model.Value;
                Count = _model.Count;
            }
        }
        public SETIMViewModel()
        {
            Model = new SETIMModel();
            //ValueTextBlock.Text = _model.Value.ToString();
            //CountTextBlock.Text = _model.Count.ToString();
            CenterTextBlock.Text = "SI";
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "SETIM";
            dialog.ShowLine4("Bit");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> valuelist = new List<string>();
                    valuelist.Add(dialog.ValueString4);
                    ParseValue(valuelist);
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
            return new SETIMViewModel();
        }

        public static int CatalogID { get { return 212; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                Value = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value = BitValue.Null;
            }
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Count = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ToString());
            result.Add(Count.ToString());
            return result;
        }
    }
}
