using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;

namespace SamSoarII.LadderInstViewModel
{
    public class CALLMViewModel : OutputRectBaseViewModel
    {
        private CALLMModel _model;

        public override BaseModel Model
        {
            get
            {
                return this._model;
            }
            protected set
            {
                if (value is CALLMModel)
                this._model = (CALLMModel)(value);
            }
        }
        public override string InstructionName { get { return "CALLM"; } }
        public override BaseViewModel Clone()
        {
            throw new NotImplementedException();
        }

        public override int GetCatalogID()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetValueString()
        {
            throw new NotImplementedException();
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            throw new NotImplementedException();
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            throw new NotImplementedException();
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            base.AcceptNewValues(valueStrings, contextDevice);
        }
    }
}
