﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstViewModel;

namespace SamSoarII.Extend.Utility
{
    public class PLCOriginInst : PLCInstruction
    {
        public PLCOriginInst(string text) : base(text)
        {
        }

        public new string this[int id]
        {
            get
            {
                switch (id)
                {
                    case 0: return type;
                    case 1: return oflag1;
                    case 2: return oflag2;
                    case 3: return oflag3;
                    case 4: return oflag4;
                    case 5: return oflag5;
                    default: return String.Empty;
                }
            }
        }

        public const int STATUS_ACCEPT = 0x00;
        public const int STATUS_WARNING = 0x01;
        public const int STATUS_ERROR = 0x02;
        public int Status { get; set; }

        public string Message { get; set; }
       
    }
}
