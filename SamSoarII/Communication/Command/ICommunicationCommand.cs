using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class AddrSegment
    {
        #region Numbers
        public byte Type { get; private set; }
        public byte Length { get; private set; }
        public byte AddrLow { get; private set; }
        public byte AddrHigh { get; private set; }
        public uint Addr
        {
            get
            {
                return (((uint)AddrHigh) << 8) | AddrLow;
            }
        }
        #endregion
        public ElementModel Model { get; set; }
        public AddrSegment(ElementModel model,bool isIntra = false)
        {
            Model = model;
            if (isIntra)
            {
                Type = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), model.IntrasegmentType), model.IntrasegmentAddr);
                AddrLow = (byte)model.IntrasegmentAddr;
                AddrHigh = 0;
                Length = 1;
            }
            else
            {
                Type = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), model.AddrType), model.StartAddr);
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
    }


}
