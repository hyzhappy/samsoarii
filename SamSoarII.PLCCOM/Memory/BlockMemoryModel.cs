using SamSoarII.PLCCOM.USB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCCOM.Memory
{
    abstract public class BlockMemoryModel
    {
        private ObservableCollection<UnitMemoryModel> units
            = new ObservableCollection<UnitMemoryModel>();

        public IEnumerable<UnitMemoryModel> Units
        {
            get { return this.units; }
        }
       
        public int OffsetStart { get; private set; } = -0x3fffffff;

        public int OffsetEnd { get; private set; } = 0x3fffffff;

        public virtual void Add(UnitMemoryModel unit)
        {
            if (!units.Contains(unit))
            {
                units.Add(unit);
                OffsetStart = Math.Min(OffsetStart, unit.Offset);
                OffsetEnd = Math.Min(OffsetEnd, unit.Offset + unit.Length);
            }
        }

        public virtual void Remove(UnitMemoryModel unit)
        {
            if (units.Contains(unit))
            {
                units.Remove(unit);
            }
        }

        public abstract void Update(USBModel usbmodel);

        public abstract void Set(USBModel usbmodel, int _offsetstart, int _offsetend);

        static public BlockMemoryModel Merge(BlockMemoryModel b1, BlockMemoryModel b2)
        {
            BlockMemoryModel result = null;
            if (b1 is BlockMemoryModel_8 && b2 is BlockMemoryModel_8)
            {
                result = new BlockMemoryModel_8();
            }
            if (b1 is BlockMemoryModel_16 && b2 is BlockMemoryModel_16)
            {
                result = new BlockMemoryModel_16();
            }
            if (b1 is BlockMemoryModel_32 && b2 is BlockMemoryModel_32)
            {
                result = new BlockMemoryModel_32();
            }
            if (b1 is BlockMemoryModel_64 && b2 is BlockMemoryModel_64)
            {
                result = new BlockMemoryModel_64();
            }
            if (b1 is BlockMemoryModel_32f && b2 is BlockMemoryModel_32f)
            {
                result = new BlockMemoryModel_32f();
            }
            if (b1 is BlockMemoryModel_64f && b2 is BlockMemoryModel_64f)
            {
                result = new BlockMemoryModel_64f();
            }
            if (result == null)
            {
                throw new ArgumentException("Cannot merge blocks.");
            }
            foreach (UnitMemoryModel unit in b1.Units)
            {
                result.Add(unit);
            }
            foreach (UnitMemoryModel unit in b2.Units)
            {
                result.Add(unit);
            }
            return result;
        }

        static public BlockMemoryModel Create(UnitMemoryModel unit)
        {
            if (unit is UnitMemoryModel_8)
            {
                return new BlockMemoryModel_8();
            }
            if (unit is UnitMemoryModel_16)
            {
                return new BlockMemoryModel_16();
            }
            if (unit is UnitMemoryModel_32)
            {
                return new BlockMemoryModel_32();
            }
            if (unit is UnitMemoryModel_64)
            {
                return new BlockMemoryModel_64();
            }
            if (unit is UnitMemoryModel_32f)
            {
                return new BlockMemoryModel_32f();
            }
            if (unit is UnitMemoryModel_64f)
            {
                return new BlockMemoryModel_64f();
            }
            throw new ArgumentException("Cannot create a new BlockMemoryModel");
        }
    }

    public class BlockMemoryModel_8 : BlockMemoryModel
    {
        public override void Add(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_8))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_8");
            }
            base.Add(unit);
        }

        public override void Remove(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_8))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_8");
            }
            base.Remove(unit);
        }

        public override void Update(USBModel usbmodel)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (OffsetEnd - OffsetStart) / ufirst.Length;
            char[] input = new char[length];
            
            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);    
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            int ret = USBModel.USB_Read(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
            foreach (UnitMemoryModel unit in Units)
            {
                unit.Value = input[(unit.Offset - OffsetStart) / unit.Length];
            }
        }

        public override void Set(USBModel usbmodel, int _offsetstart, int _offsetend)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (_offsetend - _offsetstart) / ufirst.Length;
            char[] input = new char[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }
            
            foreach (UnitMemoryModel unit in Units)
            {
                if (unit.Offset >= _offsetstart && unit.Offset < _offsetend)
                {
                    unit.Value = input[(unit.Offset - _offsetstart) / unit.Length];
                }
            }
            int ret = USBModel.USB_Write(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
        }
    }

    public class BlockMemoryModel_16 : BlockMemoryModel
    {
        public override void Add(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_16))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_16");
            }
            base.Add(unit);
        }

        public override void Remove(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_16))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_16");
            }
            base.Remove(unit);
        }
        
        public override void Update(USBModel usbmodel)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (OffsetEnd - OffsetStart) / ufirst.Length;
            UInt16[] input = new UInt16[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            int ret = USBModel.USB_Read16(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
            foreach (UnitMemoryModel unit in Units)
            {
                unit.Value = (Int16)input[(unit.Offset - OffsetStart) / unit.Length];
            }
        }

        public override void Set(USBModel usbmodel, int _offsetstart, int _offsetend)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (_offsetend - _offsetstart) / ufirst.Length;
            UInt16[] input = new UInt16[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            foreach (UnitMemoryModel unit in Units)
            {
                if (unit.Offset >= _offsetstart && unit.Offset < _offsetend)
                {
                    unit.Value = (Int16)input[(unit.Offset - _offsetstart) / unit.Length];
                }
            }
            int ret = USBModel.USB_Write16(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
        }
    }

    public class BlockMemoryModel_32 : BlockMemoryModel
    {

        public override void Add(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_32))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_32");
            }
            base.Add(unit);
        }

        public override void Remove(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_32))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_32");
            }
            base.Remove(unit);
        }

        public override void Update(USBModel usbmodel)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (OffsetEnd - OffsetStart) / ufirst.Length;
            UInt32[] input = new UInt32[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            int ret = USBModel.USB_Read32(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
            foreach (UnitMemoryModel unit in Units)
            {
                unit.Value = (Int64)input[(unit.Offset - OffsetStart) / unit.Length];
            }
        }

        public override void Set(USBModel usbmodel, int _offsetstart, int _offsetend)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (_offsetend - _offsetstart) / ufirst.Length;
            UInt32[] input = new UInt32[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            foreach (UnitMemoryModel unit in Units)
            {
                if (unit.Offset >= _offsetstart && unit.Offset < _offsetend)
                {
                    unit.Value = (Int32)input[(unit.Offset - _offsetstart) / unit.Length];
                }
            }
            int ret = USBModel.USB_Write32(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
        }
    }

    public class BlockMemoryModel_64 : BlockMemoryModel
    {
        public override void Add(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_64))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_64");
            }
            base.Add(unit);
        }

        public override void Remove(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_64))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_64");
            }
            base.Remove(unit);
        }

        public override void Update(USBModel usbmodel)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (OffsetEnd - OffsetStart) / ufirst.Length;
            UInt64[] input = new UInt64[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            int ret = USBModel.USB_Read64(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
            foreach (UnitMemoryModel unit in Units)
            {
                unit.Value = (Int64)input[(unit.Offset - OffsetStart) / unit.Length];
            }
        }

        public override void Set(USBModel usbmodel, int _offsetstart, int _offsetend)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (_offsetend - _offsetstart) / ufirst.Length;
            UInt64[] input = new UInt64[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            foreach (UnitMemoryModel unit in Units)
            {
                if (unit.Offset >= _offsetstart && unit.Offset < _offsetend)
                {
                    unit.Value = (Int64)input[(unit.Offset - _offsetstart) / unit.Length];
                }
            }
            int ret = USBModel.USB_Write64(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
        }
    }

    public class BlockMemoryModel_32f : BlockMemoryModel
    {

        public override void Add(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_32f))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_32f");
            }
            base.Add(unit);
        }

        public override void Remove(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_32f))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_32f");
            }
            base.Remove(unit);
        }

        public override void Update(USBModel usbmodel)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (OffsetEnd - OffsetStart) / ufirst.Length;
            float[] input = new float[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            int ret = USBModel.USB_Read32f(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
            foreach (UnitMemoryModel unit in Units)
            {
                unit.Value = input[(unit.Offset - OffsetStart) / unit.Length];
            }
        }

        public override void Set(USBModel usbmodel, int _offsetstart, int _offsetend)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (_offsetend - _offsetstart) / ufirst.Length;
            float[] input = new float[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            foreach (UnitMemoryModel unit in Units)
            {
                if (unit.Offset >= _offsetstart && unit.Offset < _offsetend)
                {
                    unit.Value = input[(unit.Offset - _offsetstart) / unit.Length];
                }
            }
            int ret = USBModel.USB_Write32f(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
        }
    }

    public class BlockMemoryModel_64f : BlockMemoryModel
    {
        public override void Add(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_64f))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_64f");
            }
            base.Add(unit);
        }

        public override void Remove(UnitMemoryModel unit)
        {
            if (!(unit is UnitMemoryModel_64f))
            {
                throw new ArgumentException("argument should be UnitMemoryModel_64f");
            }
            base.Remove(unit);
        }

        public override void Update(USBModel usbmodel)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (OffsetEnd - OffsetStart) / ufirst.Length;
            double[] input = new double[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            int ret = USBModel.USB_Read64f(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
            foreach (UnitMemoryModel unit in Units)
            {
                unit.Value = input[(unit.Offset - OffsetStart) / unit.Length];
            }
        }

        public override void Set(USBModel usbmodel, int _offsetstart, int _offsetend)
        {
            UnitMemoryModel ufirst = Units.FirstOrDefault();
            if (ufirst == null)
            {
                return;
            }

            string name = String.Empty;
            int length = (_offsetend - _offsetstart) / ufirst.Length;
            double[] input = new double[length];

            if (ufirst.VZName.Length > 0)
            {
                name = String.Format("{0:s}{1:d}{2:s}{3:d}",
                    ufirst.Basename, OffsetStart, ufirst.VZName, ufirst.VZOffset);
            }
            else
            {
                name = String.Format("{0:s}{1:d}",
                    ufirst.Basename, OffsetStart);
            }

            foreach (UnitMemoryModel unit in Units)
            {
                if (unit.Offset >= _offsetstart && unit.Offset < _offsetend)
                {
                    unit.Value = input[(unit.Offset - _offsetstart) / unit.Length];
                }
            }
            int ret = USBModel.USB_Write64f(name, length, input);
            if (ret != 0)
            {
                throw new USBException(ret);
            }
        }
    }

}
