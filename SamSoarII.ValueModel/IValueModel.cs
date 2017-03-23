using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public interface IValueModel
    {
        LadderValueType Type { get; }
        string ValueString { get; }
        string ValueShowString { get; }
        string Comment { get; set; }
        bool IsVariable { get; }
        string GetValue();
    }
}
