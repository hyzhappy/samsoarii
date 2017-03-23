using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;

namespace SamSoarII.LadderInstViewModel
{
    public class TONViewModel : OutputRectBaseViewModel
    {
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
        public override string InstructionName { get { return "TON"; } }

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
    }
}
