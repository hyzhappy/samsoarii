﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class ANDWModel : BaseModel
    {
        public override string InstructionName => "ANDW";
        public WordValue InputValue1 { get; set; }
        public WordValue InputValue2 { get; set; }
        public WordValue OutputValue { get; set; }
        public ANDWModel()
        {
            InputValue1 = WordValue.Null;
            InputValue2 = WordValue.Null;
            OutputValue = WordValue.Null;
        }
        public ANDWModel(WordValue inputValue1, WordValue inputValue2, WordValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{3} = {1} & {2};\r\n}}\r\n",ImportVaribleName,InputValue1.GetValue(),InputValue2.GetValue(),OutputValue.GetValue());
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
                case 0: return InputValue1;
                case 1: return InputValue2;
                case 2: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue1 = (WordValue)value; break;
                case 1: InputValue2 = (WordValue)value; break;
                case 2: OutputValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
