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
    public class MVDBLKViewModel : OutputRectBaseViewModel
    {
        private MVDBLKModel _model = new MVDBLKModel();
        private DoubleWordValue SourceValue
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
        private DoubleWordValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                MiddleTextBlock3.Text = string.Format("D : {0}", _model.DestinationValue.ToShowString());
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
                BottomTextBlock.Text = string.Format("N : {0}", _model.Count.ToShowString());
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
                _model = value as MVDBLKModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
                Count = _model.Count;
            }
        }
        public override string InstructionName { get { return "MVDBLK"; } }
        public MVDBLKViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MVDBLKModel();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine2("S");
            dialog.ShowLine4("D");
            dialog.ShowLine6("N");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> temp = new List<string>();
                    temp.Add(dialog.ValueString2);
                    temp.Add(dialog.ValueString4);
                    temp.Add(dialog.ValueString6);
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
            return new MVDBLKViewModel();
        }

        public static int CatalogID { get { return 604; } }

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
            result.Add(SourceValue.ToString());
            result.Add(DestinationValue.ToString());
            result.Add(Count.ToString());
            return result;
        }
    }
}
