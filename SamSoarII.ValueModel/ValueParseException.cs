using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class ValueParseException : Exception
    {
        public override string Message
        {
            get
            {
                return "Value Parse Exception";
            }
        }
    }
}
