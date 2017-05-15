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
            return string.Format("if({0})\r\n{{\r\nfor(int i = 0;i < {1};i++)\r\n{{\r\n",ImportVaribleName,Count.GetValue());
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
                case 0: return Count;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: Count = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
