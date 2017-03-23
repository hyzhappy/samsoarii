using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public interface IVariableValue : IValueModel
    {
        string VarName { get; set; }

        bool IsAnonymous { get; set; }

        IValueModel MappedValue { get; set; }

    }
}
