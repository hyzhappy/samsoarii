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
    public class MVBLKViewModel : OutputRectBaseViewModel
    {
        private MVBLKModel _model = new MVBLKModel();
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
                MiddleTextBlock3.Text = string.Format("D : {0}", _model.DestinationValue.ValueShowString);
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
                BottomTextBlock.Text = string.Format("N : {0}", _model.Count.ValueShowString);
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
                _model = value as MVBLKModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
                Count = _model.Count;
            }
        }
        public override string InstructionName { get { return "MVBLK"; } }
        public MVBLKViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MVBLKModel();
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("S");
            dialog.ShowLine4("D");
            dialog.ShowLine6("N");
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new MVBLKViewModel();
        }

        public static int CatalogID { get { return 603; } }

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
            catch (ValueParseException exception)
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
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                Count = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            result.Add(Count.ValueString);
            return result;
        }
    }
}
