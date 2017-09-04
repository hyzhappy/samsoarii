using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SamSoarII.Utility.DXF
{
    public interface IVertex : IComparable<IVertex>
    {
        Point P { get;}
    }
}
