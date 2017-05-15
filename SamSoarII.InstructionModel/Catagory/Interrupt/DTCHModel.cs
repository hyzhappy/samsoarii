using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Interrupt
{
    public class DTCHModel : BaseModel
    {
        public WordValue IDValue { get; set; }

        public DTCHModel()
        {
            IDValue = WordValue.Null;
        }

        public DTCHModel(WordValue _IDValue)
        {
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
                return 1;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return IDValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: IDValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
