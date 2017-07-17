using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    public class ValuePrototype : ValueModel 
    {
        public ValuePrototype(Bases _bas, int _ofs) : base(null, null)
        {
            bas = _bas;
            ofs = _ofs;
            text = String.Format("{0:s}{1:d}", ValueModel.NameOfBases[(int)bas], ofs);
        }
    }
}
