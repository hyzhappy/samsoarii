using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;


namespace SamSoarII.LadderInstViewModel
{
    public class TRDViewModel : OutputRectBaseViewModel
    {
        private TRDModel _model;
        private WordValue StartValue
        {
            get
            {
                return _model.StartValue;
            }
            set
            {
                _model.StartValue = value;
                BottomTextBlock.Text = string.Format("T : {0}", _model.StartValue.ValueShowString);
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
                _model = value as TRDModel;
                StartValue = _model.StartValue;
            }
        }
        public override string InstructionName { get { return "TRD"; } }
        public TRDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new TRDModel();
        }

        public override BaseViewModel Clone()
        {
            return new TRDViewModel();
        }

        private static int CatalogID { get { return 1400; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(StartValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                StartValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch(ValueParseException exception)
            {
                StartValue = WordValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("T");
            return dialog;
        }
    }
}
