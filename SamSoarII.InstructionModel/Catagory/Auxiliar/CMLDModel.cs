using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class CMLDModel : BaseModel
    {
        public override string InstructionName => "CMLD";

        public DoubleWordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }

        public CMLDModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }

        public CMLDModel(DoubleWordValue _InputValue, DoubleWordValue _OutputValue)
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
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMLD'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue = (DoubleWordValue)value; break;
                case 1: OutputValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMLD'", id));
            }
        }
    }
}
