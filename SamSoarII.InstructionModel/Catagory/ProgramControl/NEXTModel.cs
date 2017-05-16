using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstModel
{
    public class NEXTModel : BaseModel
    {
        public override string GenerateCode()
        {
            return string.Format("}}\r\n}}\r\n");
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
