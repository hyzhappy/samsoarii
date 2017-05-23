using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstViewModel
{
    public class NEXTViewModel : OutputRectBaseViewModel
    {
        private NEXTModel _model = new NEXTModel();
        public override BaseModel Model
        {
            get
            {
                return _model;   
            }
            protected set
            {
                _model = value as NEXTModel;
            }
        }
        public override string InstructionName { get { return "NEXT"; } }
        public override BaseViewModel Clone()
        {
            return new NEXTViewModel();
        }

        private static int CatalogID { get { return 1101; } }

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
        public override bool Assert()
        {
            return NextElements.Where(x => { return x.Type == ElementType.Null; }).Count() == 1 && NextElements.Count == 1;
        }

        
    }
}
