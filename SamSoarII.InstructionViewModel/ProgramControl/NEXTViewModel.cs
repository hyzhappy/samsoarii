using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.UserInterface;

namespace SamSoarII.InstructionViewModel
{
    public class NEXTViewModel : OutputRectBaseViewModel
    {
        private NEXTModel _model;
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

        public override void ParseValue(List<string> valueStrings)
        {
            // Nothing to do
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            // Nothing to do
        }
    }
}
