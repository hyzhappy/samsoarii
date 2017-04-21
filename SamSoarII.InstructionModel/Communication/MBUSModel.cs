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
    }
}
