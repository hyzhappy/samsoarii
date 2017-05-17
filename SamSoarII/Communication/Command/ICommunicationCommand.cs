using SamSoarII.AppMain.UI.Monitor;
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

        public AddrSegment(byte _type, byte _length, byte _addrLow, byte _addrHigh)
        {
            Type = _type;
            Length = _length;
            AddrLow = _addrLow;
            AddrHigh = _addrHigh;
        }

        public bool Merge(ElementModel emodel)
        {
            //if (emodel.IsIntrasegment) return false;
            int addrdelta = (int)((int)Addr + Length - emodel.StartAddr);
            if (addrdelta > 8) return false;
            int _length = (int)(emodel.StartAddr + emodel.ByteCount - (int)Addr);
            if (_length > 32) return false;
            Length = (byte)_length;
            return true;
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
        bool IsSuccess { get; set; }
        byte[] GetBytes();
    }


}
