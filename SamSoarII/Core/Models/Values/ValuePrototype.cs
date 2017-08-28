using SamSoarII.Utility;
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
            switch (_bas)
            {
                case Bases.X: case Bases.Y:
                    text = String.Format("{0:s}{1:d}", ValueModel.NameOfBases[(int)bas], ValueConverter.IntToDex(ofs));
                    break;
                default:
                    text = String.Format("{0:s}{1:d}", ValueModel.NameOfBases[(int)bas], ofs);
                    break;
            }
        }
    }
}
