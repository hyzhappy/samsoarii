﻿using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class TONRModel : BaseModel
    {
        public override string InstructionName => "TONR";
        public WordValue TimerValue { get; set; }
        public WordValue EndValue { get; set; }

        public TONRModel()
        {
            TimerValue = WordValue.Null;
            EndValue = WordValue.Null;
        }

        public TONRModel(WordValue _TimerValue, WordValue _EndValue)
        {
            TimerValue = _TimerValue;
            EndValue = _EndValue;
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
                case 0: return TimerValue;
                case 1: return EndValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: TimerValue = (WordValue)value; break;
                case 1: EndValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
