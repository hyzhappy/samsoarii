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
        public INVModel()
        {

        }
        public override string GenerateCode()
        {
            return string.Format("uint32_t {0} = !{1};\r\n", ExportVaribleName, ImportVaribleName);
        }
    }
}
