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
        #endregion
        public MonitorElement Model { get; set; }
        public AddrSegment() { }
        public AddrSegment(MonitorElement model,bool isIntra = false)
        {
            Model = model;
            if (isIntra)
            {
                Type = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), model.IntrasegmentType), (uint)(model.IntrasegmentAddr));
                AddrLow = (byte)model.IntrasegmentAddr;
                AddrHigh = 0;
                Length = 1;
            }
            else
            {
                Type = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), model.AddrType), (uint)(model.StartAddr));
                byte[] startaddr = ValueConverter.GetBytes((ushort)model.StartAddr);
                AddrLow = startaddr[1];
                AddrHigh = startaddr[0];
                Length = (byte)model.ByteCount;
            }
        }
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
        void UpdataValues();
        int RecvDataLen { get; set; }
    }


}
