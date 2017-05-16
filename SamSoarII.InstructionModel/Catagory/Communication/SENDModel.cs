using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Communication
{
    public class SENDModel : BaseModel
    {
        public WordValue COMPort { get; set; }
        public WordValue BaseValue { get; set; }
        public WordValue CountValue { get; set; }

        public SENDModel()
        {
            COMPort = WordValue.Null;
            BaseValue = WordValue.Null;
            CountValue = WordValue.Null;
        }

        public SENDModel(WordValue _COMPort, WordValue _Base, WordValue _Count)
        {
            COMPort = _COMPort;
            BaseValue = _Base;
            CountValue = _Count;
        }
        
        public override string GenerateCode()
        {
            return String.Empty;
        }
        public override int ParaCount
        {
            get
            {
                return 3;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return COMPort;
                case 1: return BaseValue;
                case 2: return CountValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: COMPort = (WordValue)value; break;
                case 1: BaseValue = (WordValue)value; break;
                case 2: CountValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
