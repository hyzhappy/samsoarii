using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstModel
{
    public class CALLModel : BaseModel
    {
        public string FunctionName { get; set; }
        public CALLModel()
        {
            FunctionName = string.Empty;
        }
        public CALLModel(string functionName)
        {
            FunctionName = functionName;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}();\r\n}}\r\n", ImportVaribleName, FunctionName);
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
                case 0: return new StringValue(FunctionName);
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: FunctionName = value.ValueString; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
