using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using SamSoarII.UserInterface;
using System.Windows;
namespace SamSoarII.LadderInstViewModel
{
    public class RSTViewModel : OutputBaseViewModel
    {
        private RSTModel _model = new RSTModel();

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
                _model = value as RSTModel;
                Value = _model.Value;
                Count = _model.Count;
            }
        }
        public override string InstructionName { get { return "RST"; } }
        public RSTViewModel()
        {
            Model = new RSTModel();
            CenterTextBlock.Text = "R";
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
            return new RSTViewModel();
        }

        public static int CatalogID { get { return 213; } }


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
