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
    public class SHLBViewModel : BaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "SHLB";
            }
        }

        public override bool IsCommentMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsMonitorMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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

        public override int X
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Y
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            throw new NotImplementedException();
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

        public override void ParseValue(IList<string> valueStrings)
        {
            throw new NotImplementedException();
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            throw new NotImplementedException();
        }

        public override void UpdateCommentContent()
        {
            throw new NotImplementedException();
        }
    }
}
