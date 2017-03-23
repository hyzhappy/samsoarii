using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using System.Windows;
using SamSoarII.UserInterface;
namespace SamSoarII.LadderInstViewModel
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
                ValueTextBlock.Text = _model.Value.ValueShowString;
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
                CountTextBlock.Text = _model.Count.ValueShowString;
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
        public override string InstructionName { get { return "SETIM"; } }
        public SETIMViewModel()
        {
            Model = new SETIMModel();
            //ValueTextBlock.Text = _model.Value.ValueString;
            //CountTextBlock.Text = _model.Count.ValueString;
            CenterTextBlock.Text = "SI";
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("Bit", Value);
            dialog.ShowLine5("Count", Count);
            return dialog;
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

        public override void ParseValue(IList<string> valueStrings)
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
            result.Add(Value.ValueString);
            result.Add(Count.ValueString);
            return result;
        }
    }
}
