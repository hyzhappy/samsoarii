﻿using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class JMPModel : BaseModel
    {
        public override string InstructionName => "JMP";
        public WordValue LBLIndex { get; set; }
        public JMPModel()
        {
            LBLIndex = WordValue.Null;
        }
        public JMPModel(WordValue lblindex)
        {
            LBLIndex = lblindex;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n goto {1};\r\n}}\r\n", ImportVaribleName, LBLIndex.ToString());
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
                case 0: return LBLIndex;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: LBLIndex = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
