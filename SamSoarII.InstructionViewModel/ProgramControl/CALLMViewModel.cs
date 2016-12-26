using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.UserInterface;

namespace SamSoarII.InstructionViewModel
{
    public class CALLMViewModel : OutputRectBaseViewModel
    {
        private CALLMModel _model;

        public override BaseModel Model
        {
            get
            {
                throw new NotImplementedException();
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }

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

        public override void ParseValue(List<string> valueStrings)
        {
            throw new NotImplementedException();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            throw new NotImplementedException();
        }
    }
}
