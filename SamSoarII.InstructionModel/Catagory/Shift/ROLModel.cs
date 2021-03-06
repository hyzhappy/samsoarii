﻿using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class ROLModel : BaseModel
    {
        public override string InstructionName => "ROL";
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public ROLModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            Count = WordValue.Null;
        }
        public ROLModel(WordValue sourceValue, WordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nint16_t count = {3} % 16;\r\nif(count == 0)\r\n{{\r\n{2} = {1};\r\n}}\r\nelse if(count > 0)\r\n{{\r\nint16_t temp1,temp2;\r\ntemp1 = ({1} << count) >> count;\r\ntemp2 = (({1} - temp1) >> (16 - count)) & (int16_t)(pow(2,count) - 1);\r\n{2} = (temp1 << count) + temp2;\r\n}}\r\nif({2} == 0)\r\n{{\r\nMBit[8167] = 1;\r\n}}\r\n}}\r\n",ImportVaribleName,SourceValue.GetValue(),DestinationValue.GetValue(),Count.GetValue());
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
                case 0: return SourceValue;
                case 1: return DestinationValue;
                case 2: return Count;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: SourceValue = (WordValue)value; break;
                case 1: DestinationValue = (WordValue)value; break;
                case 2: Count = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
