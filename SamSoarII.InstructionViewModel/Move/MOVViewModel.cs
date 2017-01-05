using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.UserInterface;
using SamSoarII.InstructionModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
using System.Windows;


namespace SamSoarII.InstructionViewModel
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
                MiddleTextBlock2.Text = string.Format("S : {0}", _model.SourceValue.ToShowString());
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
                BottomTextBlock.Text = string.Format("D : {0}", _model.DestinationValue.ToShowString());
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
        public MOVViewModel()
        {
            TopTextBlock.Text = "MOV";
            Model = new MOVModel();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "MOV";
            dialog.ShowLine3("S");
            dialog.ShowLine5("D");
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
            return new MOVViewModel();
        }

        public static int CatalogID { get { return 600; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(List<string> valueStrings)
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
            result.Add(SourceValue.ToString());
            result.Add(DestinationValue.ToString());
            return result;
        }
    }
}
