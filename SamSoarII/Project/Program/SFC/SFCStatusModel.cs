using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Project.Program.SFC
{
    public class SFCStatusModel
    {
        private LadderDiagramViewModel ldvmodel;
        public LadderDiagramViewModel LDVModel
        {
            get { return this.ldvmodel; }
        }

        public SFCStatusModel(LadderDiagramViewModel _ldvmodel)
        {
            ldvmodel = _ldvmodel;
        }
    }

    public class SFCInvalidStatusModel : SFCStatusModel
    {
        public SFCInvalidStatusModel() : base(null)
        {
        }
    }
}
