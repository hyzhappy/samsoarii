using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Interrupt
{
    public class ATCHModel : BaseModel
    {
        public string FuncName { get; set; }

        public WordValue IDValue { get; set; }

        public ATCHModel()
        {
            FuncName = String.Empty;
            IDValue = WordValue.Null;
        }

        public ATCHModel(string _FuncName, WordValue _IDValue)
        {
            FuncName = _FuncName;
            IDValue = _IDValue;
        }
        
        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}
