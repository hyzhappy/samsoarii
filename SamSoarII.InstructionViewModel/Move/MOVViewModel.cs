using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
using System.Windows;


namespace SamSoarII.LadderInstViewModel
{
    public class MOVViewModel : OutputRectBaseViewModel
    {
        private MOVModel _model = new MOVModel();
        private WordValue SourceValue
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
        private WordValue DestinationValue
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
                _model = value as MOVModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
            }
        }
        public override string InstructionName { get { return "MOV"; } }
        public MOVViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MOVModel();
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("S");
            dialog.ShowLine5("D");
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new MOVViewModel();
        }

        public static int CatalogID { get { return 600; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch(ValueParseException exception)
            {
                SourceValue = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            return result;
        }
    }
}
