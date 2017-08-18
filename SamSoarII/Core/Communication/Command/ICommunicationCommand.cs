using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
{
    public class AddrSegment
    {
        #region Numbers
        public byte Type { get; set; }
        public byte Length { get; set; }
        public byte AddrLow { get; set; }
        public byte AddrHigh { get; set; }
        public uint Addr
        {
            get
            {
                return (((uint)AddrHigh) << 8) | AddrLow;
            }
        }
        public int DataAddr { get; set; }
        #endregion
        public MonitorElement Model { get; set; }
        public AddrSegment() { }
    }

    public class IntraSegment
    {
        #region Numbers

        public AddrSegment Base { get; private set; }

        public AddrSegment Intra { get; private set; }

        #endregion

        public IntraSegment(AddrSegment _base, AddrSegment _intra)
        {
            Base = _base;
            Intra = _intra;
        }
    }
    
    public interface ICommunicationCommand
    {
        byte[] RetData { get; set; }
        bool IsComplete { get; set; }
        bool IsSuccess { get; set; }
        byte[] GetBytes();
        int RecvDataLen { get; set; }
        FGs_ERR_CODE ErrorCode { get; set; }
    }


}
