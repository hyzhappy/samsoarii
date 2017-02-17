using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
using SamSoarII.InstructionModel;
using System.Windows.Shapes;
using System.Windows.Media;
using SamSoarII.UserInterface;
namespace SamSoarII.InstructionViewModel
{
    public class MEPViewModel : SpecialBaseViewModel
    {
        private MEPModel _model = new MEPModel();
        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                _model = value as MEPModel;
            }
        }
        public override string InstructionName { get { return "MEP"; } }
        public MEPViewModel()
        {
            Model = new MEPModel();

            var line1 = new Line();
            line1.X1 = 50;
            line1.X2 = 50;
            line1.Y1 = 0;
            line1.Y2 = 100;
            line1.Stroke = Brushes.Black;
            line1.StrokeThickness = 4;
            CenterCanvas.Children.Add(line1);
            var line2 = new Line();
            line2.X1 = 50;
            line2.X2 = 25;
            line2.Y1 = 0;
            line2.Y2 = 25;
            line2.Stroke = Brushes.Black;
            line2.StrokeThickness = 4;
            CenterCanvas.Children.Add(line2);
            var line3 = new Line();
            line3.X1 = 50;
            line3.X2 = 75;
            line3.Y1 = 0;
            line3.Y2 = 25;
            line3.Stroke = Brushes.Black;
            line3.StrokeThickness = 4;
            CenterCanvas.Children.Add(line3);
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            // Nothing to do
        }

        public override BaseViewModel Clone()
        {
            return new MEPViewModel();
        }

        public static int CatalogID { get { return 206; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            // Nothing to do
        }

        public override IEnumerable<string> GetValueString()
        {
            return new List<string>();
        }
    }
}
