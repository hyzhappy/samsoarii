﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class ALTPModel : BaseModel
    {
        public override string InstructionName => "ALTP";

        public BitValue Value { get; set; }
        public ALTPModel()
        {
            Value = BitValue.Null;
        }
        public ALTPModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
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
                case 0: return Value;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: Value = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
