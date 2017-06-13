using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.UI.Breakpoint
{
    public enum BackTraceType
    {
        External, Diagram, FuncBlock
    };


    public class BackTraceElement
    {
        public BackTraceType Type { get; set; }
        public object Brpo { get; set; }

        public BackTraceElement(BackTraceType _type, object _brpo = null)
        {
            Type = _type; Brpo = _brpo;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case BackTraceType.External:
                    return "<外部代码...>";
                case BackTraceType.Diagram:
                case BackTraceType.FuncBlock:
                    return Brpo != null ? Brpo.ToString() : "null";
                default:
                    return "null";
            }
        }
    }
}
