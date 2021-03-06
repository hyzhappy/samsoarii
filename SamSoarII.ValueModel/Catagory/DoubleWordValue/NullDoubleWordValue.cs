﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class NullDoubleWordValue : DoubleWordValue
    {

        public override string ValueShowString
        {
            get
            {
                return "???";
            }
        }

        public override string ValueString
        {
            get
            {
                return string.Empty;
            }
        }

        public override string GetValue()
        {
            return string.Empty;
        }
    }
}
