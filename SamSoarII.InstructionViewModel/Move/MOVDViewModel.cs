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
    public class MOVDViewModel : OutputRectBaseViewModel
    {
        private MOVDModel _model;
        private DoubleWordValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock1.Text = _model.SourceValue.ToShowString();
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
                BottomTextBlock.Text = _model.DestinationValue.ToShowString();
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
                _model = value as MOVDModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
            }
        }
        public override string InstructionName { get { return "MOVD"; } }
        public MOVDViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MOVDModel();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
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
            return new MOVDViewModel();
        }

        public static int CatalogID { get { return 601; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(List<string> valueStrings)
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
                DestinationValue = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = DoubleWordValue.Null;
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
