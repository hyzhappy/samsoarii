﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstModel
{
    public class RSTModel : BaseModel
    {
        public BitValue Value { get; set; }
        public WordValue Count { get; set; }
        public RSTModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }
        public RSTModel(BitValue value, WordValue count)
        {
            Value = value;
            Count = count;
        }
        public override string GenerateCode()
        {
            if (Value.GetType() == typeof(TBitValue))
            {
                Match match = Regex.Match(Value.GetBitValue(), "[0-9]+", RegexOptions.IgnoreCase);
                uint index = uint.Parse(match.Value);
                return string.Format("if({0})\r\n{{\r\nplc_bool* p = &{2};\r\nfor(int i = 0; i < {1}; i++)\r\n{{\r\n*p = 0;\r\nTV[{3}] = 0;\r\np++;\r\n}}\r\n}}\r\n", ImportVaribleName, Count.GetWordValue(), Value.GetBitValue(), index);
            }
            else if(Value.GetType() == typeof(CBitValue))
            {
                return null;
            }
            else
            {
                return string.Format("if({0})\r\n{{\r\nplc_bool* p = &{2};\r\nfor(int i = 0; i < {1}; i++)\r\n{{\r\n*p = 0;\r\np++;\r\n}}\r\n}}\r\n", ImportVaribleName, Count.GetWordValue(), Value.GetBitValue());
            }
        }
    }
}
