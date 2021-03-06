﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows.Shapes;
using System.Windows.Media;
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    public class INVViewModel : SpecialBaseViewModel
    {
        private INVModel _model;
        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                _model = value as INVModel;
            }
        }
        public override string InstructionName { get { return "INV"; } }
        public INVViewModel()
        {
            Model = new INVModel();
            var line = new Line();
            line.X1 = 75;
            line.Y1 = 25;
            line.X2 = 25;
            line.Y2 = 75;
            line.StrokeThickness = 4;
            line.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line);
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            // Nothing to do
            return null;
        }

        public override BaseViewModel Clone()
        {
            return new INVViewModel();
        }

        public static int CatalogID { get { return 208; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }


        public override void ParseValue(IList<string> valueStrings)
        {
            // Nothing to do
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
            return new List<IValueModel>();
        }
    }
}
