using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
namespace SamSoarII.LadderInstViewModel
{
    public class ALTPViewModel : OutputRectBaseViewModel
    {
        private ALTPModel _model;
        private BitValue Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                _model.Value = value;
                BottomTextBlock.Text = _model.Value.ValueShowString;
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
                _model = value as ALTPModel;
                Value = _model.Value;
            }
        }
        public override string InstructionName { get { return "ALTP"; } }
        public ALTPViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new ALTPModel();
        }

        public override BaseViewModel Clone()
        {
            return new ALTPViewModel();
        }

        private int CatalogID { get { return 216; } }
        public override int GetCatalogID()
        {
            return CatalogID;
        }
        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Value = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch
            {
                Value = BitValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("Bit", Value);
            return dialog;
        }

    }
}
