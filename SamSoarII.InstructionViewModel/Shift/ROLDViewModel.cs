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
    public class ROLDViewModel : OutputRectBaseViewModel
    {
        private ROLDModel _model;
        private DoubleWordValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock2.Text = string.Format("S : {0}", _model.SourceValue.ValueShowString);
            }
        }
        private WordValue Count
        {
            get
            {
                return _model.Count;
            }
            set
            {
                _model.Count = value;
                MiddleTextBlock3.Text = string.Format("N : {0}", _model.Count.ValueShowString);
            }
        }
        private DoubleWordValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                BottomTextBlock.Text = string.Format("D : {0}", _model.DestinationValue.ValueShowString);
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
                _model = value as ROLDModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
                Count = _model.Count;
            }
        }
        public override string InstructionName { get { return "ROLD"; } }
        public ROLDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new ROLDModel();
        }

        public override BaseViewModel Clone()
        {
            return new ROLDViewModel();
        }

        private static int CatalogID { get { return 1207; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(Count.ValueString);
            result.Add(DestinationValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = DoubleWordValue.Null;
            }
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Count = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseDoubleWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = DoubleWordValue.Null;
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
