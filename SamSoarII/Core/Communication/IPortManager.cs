using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public interface IPortManager
    {
        int Start();
        int Abort();
        int Read(ICommunicationCommand cmd);
        int Write(ICommunicationCommand cmd);
        Exception Ex { get; }
    }
}
