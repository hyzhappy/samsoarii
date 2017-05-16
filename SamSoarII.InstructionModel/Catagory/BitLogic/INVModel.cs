using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class INVModel : BaseModel
    {
        public override string InstructionName => "INV";

        public INVModel()
        {

        }
        public override string GenerateCode()
        {
            return string.Format("uint32_t {0} = !{1};\r\n", ExportVaribleName, ImportVaribleName);
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
