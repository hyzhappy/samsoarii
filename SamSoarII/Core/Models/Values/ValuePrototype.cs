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
                    text = _ofs > 0 ? "" : "0";
                    while (_ofs > 0)
                    {
                        int _crt = _ofs & 7;
                        text = String.Format("{0:d}{1:s}", _crt, text);
                        _ofs >>= 3;
                    }
                    text = String.Format("{0:s}{1:s}", ValueModel.NameOfBases[(int)bas], text);
                    break;
                default:
                    text = String.Format("{0:s}{1:d}", ValueModel.NameOfBases[(int)bas], ofs);
                    break;
            }
        }
    }
}
