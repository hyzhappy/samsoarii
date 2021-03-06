﻿using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class TWRModel : BaseModel
    {
        public override string InstructionName => "TWR";
        public WordValue StartValue { get; set; }
        public TWRModel()
        {
            StartValue = WordValue.Null;
        }

        public TWRModel(WordValue startValue)
        {
            StartValue = startValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\ntime_t timeNow;\r\nstruct tm *timeNow_f;\r\ntime(&timeNow);\r\ntimeNow_f = gmtime(&timeNow);\r\nint16_t *p = &{1};\r\n*p = BCD((int16_t)(timeNow_f -> tm_year + 1900));\r\n*(p + 1) = BCD((int16_t)(timeNow_f -> tm_mon + 1));\r\n*(p + 2) = BCD((int16_t)timeNow_f -> tm_mday);\r\n*(p + 3) = BCD((int16_t)(timeNow_f -> tm_hour + 8));\r\n*(p + 4) = BCD((int16_t)timeNow_f -> tm_min);\r\n*(p + 5) = BCD((int16_t)timeNow_f -> tm_sec);\r\n*(p + 7) = BCD((int16_t)timeNow_f -> tm_wday);\r\n}}\r\n", ImportVaribleName, StartValue.GetValue());
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
                case 0: return StartValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: StartValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
