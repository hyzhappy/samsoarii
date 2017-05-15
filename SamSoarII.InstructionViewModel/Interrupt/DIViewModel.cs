﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel.Interrupt;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstViewModel
{
    public class DIViewModel : OutputRectBaseViewModel
    {
        private DIModel _model;
        public override BaseModel Model
        {
            get
            {
                return _model;   
            }
            protected set
            {
                _model = value as DIModel;
            }
        }
        public override string InstructionName { get { return "DI"; } }
        public override BaseViewModel Clone()
        {
            return new DIViewModel();
        }

        private static int CatalogID { get { return 1303; } }

        public override int GetCatalogID()
        {
            return CatalogID;
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
        public override void ParseValue(IList<string> valueStrings)
        {
            // Nothing to do
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            base.AcceptNewValues(valueStrings, contextDevice);
        }
        public override IPropertyDialog PreparePropertyDialog()
        {
            // Nothing to do
            return null;
        }
    }
}
