using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamSoarII.PLCCOM.Memory
{
    abstract public class UnitMemoryModel
    {
        #region Number & Interfaces

        public abstract object Value { get; set; }

        private string name;
        
        public string Name
        {
            get
            {
                return this.name;
            }
            private set
            {
                this.name = value;
                Match m1 = Regex.Match(value, @"([A-Z]+)(\d+)$");
                Match m2 = Regex.Match(value, @"([A-Z]+)(\d+)([VZ])(\d+)$");
                if (m1.Success)
                {
                    Basename = m1.Groups[1].Value;
                    Offset = int.Parse(m1.Groups[2].Value);
                    VZName = String.Empty;
                    VZOffset = 0;
                }
                if (m2.Success)
                {
                    Basename = m2.Groups[1].Value;
                    Offset = int.Parse(m2.Groups[2].Value);
                    VZName = m2.Groups[3].Value;
                    VZOffset = int.Parse(m2.Groups[4].Value);
                }
            }
        }
        
        public string Basename { get; private set; }

        public int Offset { get; private set; }

        public int Length { get; private set; }

        public string VZName { get; private set; }

        public int VZOffset { get; private set; }
        
        #endregion

        public UnitMemoryModel(string _name, int _length)
        {
            Name = _name;
            Length = _length;
        }

        #region Prev & Next

        public abstract UnitMemoryModel Prev();

        public abstract UnitMemoryModel Next();

        #endregion

        static public UnitMemoryModel Create(string name, string type, PLCDevice.Device device)
        {
            Match m1 = Regex.Match(name, @"^D\d+([VZ]\d+)?$");
            Match m2 = Regex.Match(name, @"^CV2\d\d([VZ]\d+)$");
            Match m3 = Regex.Match(name, @"^[XYSMTV]\d+([VZ]\d+)?$");
            Match m4 = Regex.Match(name, @"^(TV|CV|AI|AO|V|Z)\d+([VZ]\d+)?$");
            int size = 1;
            int length = 1;

            if (m1.Success)
            {
                switch (type)
                {
                    case "WORD":
                        size *= 1;
                        break;
                    case "DWORD":
                        size *= 2;
                        length = 2;
                        break;
                    case "FLOAT":
                        size *= 2;
                        length = 2;
                        break;
                    default:
                        throw new ArgumentException(String.Format("Invaild type {0:s} for register {1:s}", type, name));
                }
            }
            else if (m2.Success)
            {
                switch (type)
                {
                    case "DWORD":
                        size *= 2;
                        break;
                    default:
                        throw new ArgumentException(String.Format("Invaild type {0:s} for register {1:s}", type, name));
                }
            }
            else if (m3.Success)
            {
                switch (type)
                {
                    case "BIT":
                        size = 1;
                        break;
                    default:
                        throw new ArgumentException(String.Format("Invaild type {0:s} for register {1:s}", type, name));
                }
            }
            else if (m4.Success)
            {
                switch (type)
                {
                    case "WORD":
                        size *= 1;
                        break;
                    default:
                        throw new ArgumentException(String.Format("Invaild type {0:s} for register {1:s}", type, name));
                }
            }
            else
            {
                throw new ArgumentException(String.Format("Invaild register {0:s}", name));
            }
            if (type.Equals("BIT"))
            {
                return new UnitMemoryModel_8(name, length);
            }
            switch (device.BitNumber)
            {
                case 16:
                    size *= 2;
                    break;
                case 32:
                case 64:
                    size *= 4;
                    break;
            }
            switch (size)
            {
                case 1:
                    if (type.Equals("FLOAT"))
                    {
                        throw new ArgumentException("Unsupported register type : 8-bit float");
                    }
                    else
                    {
                        return new UnitMemoryModel_8(name, length);
                    }
                case 2:
                    if (type.Equals("FLOAT"))
                    {
                        throw new ArgumentException("Unsupported register type : 16-bit float");
                    }
                    else
                    {
                        return new UnitMemoryModel_16(name, length);
                    }
                case 4:
                    if (type.Equals("FLOAT"))
                    {
                        return new UnitMemoryModel_32f(name, length);
                    }
                    else
                    {
                        return new UnitMemoryModel_32(name, length);
                    }
                case 8:
                    if (type.Equals("FLOAT"))
                    {
                        return new UnitMemoryModel_64f(name, length);
                    }
                    else
                    {
                        return new UnitMemoryModel_64(name, length);
                    }
                default:
                    throw new ArgumentException(String.Format("Unsupported register type : {0:d}-bits", size * 8));
            }
        }
    }
    
    public class UnitMemoryModel_8 : UnitMemoryModel
    {
        private char value;
        public override object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (char)value;
            }
        }

        public UnitMemoryModel_8(string _name, int _length) : base(_name, _length) { }
       
        public override UnitMemoryModel Prev()
        {
            if (VZName.Length > 0)
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_8(
                        String.Format("{0:s}{1:d}{2:s}{3:d}",
                            Basename, Offset - Length, VZName, VZOffset),
                        Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_8(
                        String.Format("{0:s}{1:d}",
                            Basename, Offset - Length),
                        Length);
                }
                else
                {
                    return null;
                }
            }
        }

        public override UnitMemoryModel Next()
        {
            if (VZName.Length > 0)
            {
                return new UnitMemoryModel_8(
                    String.Format("{0:s}{1:d}{2:s}{3:d}",
                       Basename, Offset + Length, VZName, VZOffset),
                    Length);
            }
            else
            {
                return new UnitMemoryModel_8(
                    String.Format("{0:s}{1:d}",
                        Basename, Offset + Length),
                    Length);
            }
        }
    }

    public class UnitMemoryModel_16 : UnitMemoryModel
    {
        private Int16 value;
        public override object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (Int16)value;
            }
        }

        public UnitMemoryModel_16(string _name, int _length) : base(_name, _length) { }

        public override UnitMemoryModel Prev()
        {
            if (VZName.Length > 0)
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_16(
                        String.Format("{0:s}{1:d}{2:s}{3:d}",
                            Basename, Offset - Length, VZName, VZOffset),
                        Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_16(
                        String.Format("{0:s}{1:d}",
                            Basename, Offset - Length),
                        Length);
                }
                else
                {
                    return null;
                }
            }
        }

        public override UnitMemoryModel Next()
        {
            if (VZName.Length > 0)
            {
                return new UnitMemoryModel_16(
                    String.Format("{0:s}{1:d}{2:s}{3:d}",
                       Basename, Offset + Length, VZName, VZOffset),
                    Length);
            }
            else
            {
                return new UnitMemoryModel_16(
                    String.Format("{0:s}{1:d}",
                        Basename, Offset + Length),
                    Length);
            }
        }
    }

    public class UnitMemoryModel_32 : UnitMemoryModel
    {
        private Int32 value;
        public override object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (Int32)value;
            }
        }
        
        public UnitMemoryModel_32(string _name, int _length) : base(_name, _length) { }

        public override UnitMemoryModel Prev()
        {
            if (VZName.Length > 0)
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_32(
                        String.Format("{0:s}{1:d}{2:s}{3:d}",
                            Basename, Offset - Length, VZName, VZOffset),
                        Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_32(
                        String.Format("{0:s}{1:d}",
                            Basename, Offset - Length),
                        Length);
                }
                else
                {
                    return null;
                }
            }
        }

        public override UnitMemoryModel Next()
        {
            if (VZName.Length > 0)
            {
                return new UnitMemoryModel_32(
                    String.Format("{0:s}{1:d}{2:s}{3:d}",
                       Basename, Offset + Length, VZName, VZOffset),
                    Length);
            }
            else
            {
                return new UnitMemoryModel_32(
                    String.Format("{0:s}{1:d}",
                        Basename, Offset + Length),
                    Length);
            }
        }
    }

    public class UnitMemoryModel_64 : UnitMemoryModel
    {
        private Int64 value;
        public override object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (Int64)value;
            }
        }

        public UnitMemoryModel_64(string _name, int _length) : base(_name, _length) { }

        public override UnitMemoryModel Prev()
        {
            if (VZName.Length > 0)
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_64(
                        String.Format("{0:s}{1:d}{2:s}{3:d}",
                            Basename, Offset - Length, VZName, VZOffset),
                        Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_64(
                        String.Format("{0:s}{1:d}",
                            Basename, Offset - Length),
                        Length);
                }
                else
                {
                    return null;
                }
            }
        }

        public override UnitMemoryModel Next()
        {
            if (VZName.Length > 0)
            {
                return new UnitMemoryModel_64(
                    String.Format("{0:s}{1:d}{2:s}{3:d}",
                       Basename, Offset + Length, VZName, VZOffset),
                    Length);
            }
            else
            {
                return new UnitMemoryModel_64(
                    String.Format("{0:s}{1:d}",
                        Basename, Offset + Length),
                    Length);
            }
        }
    }

    public class UnitMemoryModel_32f : UnitMemoryModel
    {
        private float value;
        public override object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (float)value;
            }
        }

        public UnitMemoryModel_32f(string _name, int _length) : base(_name, _length) { }

        public override UnitMemoryModel Prev()
        {
            if (VZName.Length > 0)
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_32f(
                        String.Format("{0:s}{1:d}{2:s}{3:d}",
                            Basename, Offset - Length, VZName, VZOffset),
                        Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_32f(
                        String.Format("{0:s}{1:d}",
                            Basename, Offset - Length),
                        Length);
                }
                else
                {
                    return null;
                }
            }
        }

        public override UnitMemoryModel Next()
        {
            if (VZName.Length > 0)
            {
                return new UnitMemoryModel_32f(
                    String.Format("{0:s}{1:d}{2:s}{3:d}",
                       Basename, Offset + Length, VZName, VZOffset),
                    Length);
            }
            else
            {
                return new UnitMemoryModel_32f(
                    String.Format("{0:s}{1:d}",
                        Basename, Offset + Length),
                    Length);
            }
        }
    }

    public class UnitMemoryModel_64f : UnitMemoryModel
    {
        private double value;
        public override object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (double)value;
            }
        }
        public UnitMemoryModel_64f(string _name, int _length) : base(_name, _length) { }

        public override UnitMemoryModel Prev()
        {
            if (VZName.Length > 0)
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_64f(
                        String.Format("{0:s}{1:d}{2:s}{3:d}",
                            Basename, Offset - Length, VZName, VZOffset),
                        Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Offset - Length >= 0)
                {
                    return new UnitMemoryModel_64f(
                        String.Format("{0:s}{1:d}",
                            Basename, Offset - Length),
                        Length);
                }
                else
                {
                    return null;
                }
            }
        }

        public override UnitMemoryModel Next()
        {
            if (VZName.Length > 0)
            {
                return new UnitMemoryModel_64f(
                    String.Format("{0:s}{1:d}{2:s}{3:d}",
                       Basename, Offset + Length, VZName, VZOffset),
                    Length);
            }
            else
            {
                return new UnitMemoryModel_64f(
                    String.Format("{0:s}{1:d}",
                        Basename, Offset + Length),
                    Length);
            }
        }
    }
}
