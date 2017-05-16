using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstModel.Interrupt
{
    public class DIModel : BaseModel
    {
        public override string InstructionName => "DI";
        public DIModel()
        {
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
        public override int ParaCount
        {
            get
            {
                return 0;
            }
        }
        public override IValueModel GetPara(int id)
        {
            throw new NotImplementedException();
        }
        public override void SetPara(int id, IValueModel value)
        {
            throw new NotImplementedException();
        }
    }
}
