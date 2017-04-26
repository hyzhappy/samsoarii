using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice
{
    public class SpecialRegister
    {
        public int ID { get; set; }
        public string Base { get; set; }
        public int Offset { get; set; }
        public string Describe { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
    }
}
