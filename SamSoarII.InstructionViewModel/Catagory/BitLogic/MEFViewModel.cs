using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
using SamSoarII.LadderInstModel;
using System.Windows.Shapes;
using System.Windows.Media;
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    public class MEFViewModel : SpecialBaseViewModel
    {
        private MEFModel _model = new MEFModel();
        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                _model = value as MEFModel;     
            }
        }
        public override string InstructionName { get { return "MEF"; } }
        public MEFViewModel()
        {
            Model = new MEFModel();

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
            line2.Y1 = 100;
            line2.Y2 = 75;
            line2.Stroke = Brushes.Black;
            line2.StrokeThickness = 4;
            CenterCanvas.Children.Add(line2);
            var line3 = new Line();
            line3.X1 = 50;
            line3.X2 = 75;
            line3.Y1 = 100;
            line3.Y2 = 75;
            line3.Stroke = Brushes.Black;
            line3.StrokeThickness = 4;
            CenterCanvas.Children.Add(line3);
        }

        public override BaseViewModel Clone()
        {
            return new MEFViewModel();
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            //Nothing to do
            return null;
        }

        public static int CatalogID { get { return 207; } }


        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            //Nothing to do
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            //Nothing to do
        }

        public override IEnumerable<string> GetValueString()
        {
            return new List<string>();
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            return result;
        }
    }
}
