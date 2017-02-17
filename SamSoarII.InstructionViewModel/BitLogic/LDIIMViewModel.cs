using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.InstructionModel;
using SamSoarII.ValueModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using SamSoarII.UserInterface;
namespace SamSoarII.InstructionViewModel
{
    public class LDIIMViewModel : InputBaseViewModel
    {
        private LDIIMModel _model;
        private BitValue Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                _model.Value = value;
                ValueTextBlock.Text = _model.Value.ToShowString();
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
                _model = value as LDIIMModel;
                Value = _model.Value;
            }
        }
        public override string InstructionName { get { return "LDIIM"; } }
        public LDIIMViewModel()
        {
            Model = new LDIIMModel();
            // Draw shapes
            Line line1 = new Line();
            line1.X1 = 75;
            line1.X2 = 25;
            line1.Y1 = 0;
            line1.Y2 = 100;
            line1.StrokeThickness = 4;
            line1.Stroke = Brushes.Black;

            Line line2 = new Line();
            line2.X1 = 50;
            line2.X2 = 50;
            line2.Y1 = 0;
            line2.Y2 = 100;
            line2.StrokeThickness = 4;
            line2.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line1);
            CenterCanvas.Children.Add(line2);
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine4("Bit");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> valuelist = new List<string>();
                    valuelist.Add(dialog.ValueString4);
                    ParseValue(valuelist);
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
            return new LDIIMViewModel();
        }

        public static int CatalogID { get { return 203; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }
 
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                Value = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value = BitValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ToString());
            return result;
        }
    }
}
