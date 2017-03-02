using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class JMPModel : BaseModel
    {
        public WordValue LBLIndex { get; set; }
        public JMPModel()
        {
            LBLIndex = WordValue.Null;
        }
        public JMPModel(WordValue lblindex)
        {
            LBLIndex = lblindex;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n goto {1};\r\n}}\r\n", ImportVaribleName, LBLIndex.ToString());
        }
    }
}
