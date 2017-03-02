using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using SamSoarII.UserInterface;
namespace SamSoarII.LadderInstViewModel
{
    public class LDIMViewModel : InputBaseViewModel
    {
        private LDIMModel _model;
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
                _model = value as LDIMModel;
                Value = _model.Value;
            }
        }
        public override string InstructionName { get { return "LDIM"; } }
        public LDIMViewModel()
        {
            Model = new LDIMModel();
            // Draw shapes
            Line line = new Line();
            line.X1 = 50;
            line.X2 = 50;
            line.Y1 = 0;
            line.Y2 = 100;
            line.StrokeThickness = 4;
            line.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line);
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
            return new LDIMViewModel();
        }

        public static int CatalogID { get { return 202; } }

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
