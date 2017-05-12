using SamSoarII.PLCDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class ArgumentValue : IValueModel
    {
        public string ArgumentName { get; private set; } = String.Empty;

        public string ArgumentType { get; private set; } = String.Empty;

        public IValueModel Argument { get; private set; } = WordValue.Null;
        
        public string Comment
        {
            get
            {
                return Argument.Comment;
            }

            set
            {
                Argument.Comment = value;
            }
        }

        public bool IsVariable
        {
            get
            {
                return Argument.IsVariable;
            }
        }

        public LadderValueType Type
        {
            get
            {
                return Argument.Type;
            }
        }

        public virtual string ValueShowString
        {
            get
            {
                return Argument.ValueShowString;
            }
        }

        public virtual string ValueString
        {
            get
            {
                return Argument.ValueString;
            }
        }

        public string GetValue()
        {
            return Argument.GetValue();
        }

        public override string ToString()
        {
            return String.Format("{0:s} {1:s} {2:s}",
                 ArgumentName, ArgumentType, Argument.ValueString);
        }

        public ArgumentValue(string _argname, string _argtype, IValueModel _argument)
        {
            ArgumentName = _argname;
            ArgumentType = _argtype;
            Argument = _argument;
        }

        static public ArgumentValue Create(string argname, string argtype, string name, PLCDevice.Device device)
        {
            IValueModel argument = null;
            switch (argtype)
            {
                case "BIT*":
                    if (!ValueParser.VerifyBitRegex3.Match(name).Success)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器名称错误！位寄存器只允许Y/M/S", argname));
                    }
                    try
                    {
                        argument = ValueParser.ParseBitValue(name, device);
                    }
                    catch (ValueParseException)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器超出允许的范围！", argname));

                    }
                    break;
                case "WORD*":
                    if (!ValueParser.VerifyWordRegex2.Match(name).Success)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器名称错误！单字寄存器只允许D/TV/CV/AO", argname));
                    }
                    try
                    {
                        argument = ValueParser.ParseWordValue(name, device);
                    }
                    catch (ValueParseException)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器超出允许的范围！", argname));

                    }
                    break;
                case "DWORD*":
                    if (!ValueParser.VerifyDoubleWordRegex1.Match(name).Success)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器名称错误！双字寄存器只允许D/CV", argname));
                    }
                    try
                    {
                        argument = ValueParser.ParseDoubleWordValue(name, device);
                    }
                    catch (ValueParseException)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器超出允许的范围！", argname));

                    }
                    break;
                case "FLOAT*":
                    if (!ValueParser.VerifyFloatRegex.Match(name).Success)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器名称错误！浮点寄存器只允许D", argname));
                    }
                    try
                    {
                        argument = ValueParser.ParseFloatValue(name, device);
                    }
                    catch (ValueParseException)
                    {
                        throw new ValueParseException(String.Format("{0:s}的寄存器超出允许的范围！", argname));

                    }
                    break;
            }
            return new ArgumentValue(argname, argtype, argument);
        }

        static public ArgumentValue Parse(string text)
        {
            string[] texts = text.Split(' ');
            if (texts.Length != 3)
            {
                return ArgumentValue.Null;
            }
            else
            {
                return Create(texts[0], texts[1], texts[2], PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            }
        }
        
        public class NullArgumentValue : ArgumentValue
        {
            public NullArgumentValue() : base(String.Empty, String.Empty, WordValue.Null) { }

            public override string ValueShowString
            {
                get
                {
                    return String.Empty;
                }
            }

            public override string ValueString
            {
                get
                {
                    return String.Empty;
                }
            }
        }

        static public ArgumentValue Null
        {
            get { return new NullArgumentValue(); }
        }
        

    }
    
}
