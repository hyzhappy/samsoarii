using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Communication
{
    public class MBUSModel : BaseModel
    {
        public override string InstructionName => "MBUS";

        public WordValue COMPort { get; set; }
        public string Table { get; set; }
        public WordValue Message { get; set; }

        public MBUSModel()
        {
            COMPort = WordValue.Null;
            Table = String.Empty;
            Message = WordValue.Null;
        }

        public MBUSModel(WordValue _COMPort, string _Table, WordValue _Message)
        {
            COMPort = _COMPort;
            Table = _Table;
            Message = _Message;
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
                case 1: return new StringValue(Table);
                case 2: return Message;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: COMPort = (WordValue)value; break;
                case 1: Table = ((StringValue)value).ValueString; break;
                case 2: Message = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
