using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    public class NullViewModel : BaseViewModel
    {
        public override BaseModel Model
        {
            get
            {
                throw new InvalidOperationException();
            }
            protected set
            {
                throw new InvalidOperationException();
            }
        }
        public override string InstructionName { get { throw new InvalidOperationException(); } }
        public override IPropertyDialog PreparePropertyDialog()
        {
            throw new InvalidOperationException();
        }

        public override BaseViewModel Clone()
        {
            throw new InvalidOperationException();
        }

        public override int GetCatalogID()
        {
            throw new InvalidOperationException();
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            throw new InvalidOperationException();
        }

        public override IEnumerable<string> GetValueString()
        {
            throw new InvalidOperationException();
        }

        public override ElementType Type
        {
            get
            {
                return ElementType.Null; 
            }
        }

        
        public override int X
        {
            get;set;
        }

        public override int Y
        {
            get;set;
        }

        public override bool IsCommentMode
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        public override bool IsMonitorMode
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        public override void UpdateCommentContent()
        {
            // nothing to do
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

        }

    }
}
