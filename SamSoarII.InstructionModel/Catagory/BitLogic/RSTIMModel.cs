﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class RSTIMModel : BaseModel
    {
        public override string InstructionName => "RSTIM";

        public BitValue Value { get; set; }
        public WordValue Count { get; set; }
        public RSTIMModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }
        public RSTIMModel(BitValue value, WordValue count)
        {
            Value = value;
            Count = count;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
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
                case 0: return Value;
                case 1: return Count;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: Value = (BitValue)value; break;
                case 1: Count = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
