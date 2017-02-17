using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice
{
    public abstract class BaseDevice
    {
        protected DeviceArchitecture Arch { get; set; }
    }
}
