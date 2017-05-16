using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class LOGModel : BaseModel
    {
        public override string InstructionName => "LOG";

        public FloatValue InputValue { get; set; }
        public FloatValue OutputValue { get; set; }
        
        public LOGModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }

        public LOGModel(FloatValue _InputValue, FloatValue _OutputValue)
        {
            InputValue = _InputValue;
            OutputValue = _OutputValue;
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
                case 0: return InputValue;
                case 1: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'LOG'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue = (FloatValue)value; break;
                case 1: OutputValue = (FloatValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'LOG'", id));
            }
        }
    }
}
