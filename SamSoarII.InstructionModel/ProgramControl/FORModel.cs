using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class FORModel : BaseModel
    {
        public WordValue Count { get; set; }
        public FORModel()
        {
            Count = WordValue.Null;
        }
        public FORModel(WordValue count)
        {
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nfor(int i = 0;i < {1};i++)\r\n{{\r\n",ImportVaribleName,Count.GetWordValue());
        }
    }
}
