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
        public override int ParaCount
        {
            get
            {
                return 2;
            }
        }
        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 1: return new StringValue(FuncName);
                case 0: return IDValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 1: FuncName = value.ValueString; break;
                case 0: IDValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
