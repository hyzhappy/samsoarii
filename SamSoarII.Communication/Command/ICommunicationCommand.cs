using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public interface ICommunicationCommand
    {
        byte[] RetData { get; set; }
        bool IsSuccess { get; set; }
        byte[] GetBytes();
    }
}
