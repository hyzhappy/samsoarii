using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SamSoarII.PLCDevice;

namespace SamSoarII.ValueModel
{
    public class ValueParseException : Exception
    {
        private string _message;
        public ValueParseException(string message)
        {
            _message = message;
        }
        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class ValueParser
    {
        private static Regex BitRegex = new Regex(@"^(X|Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex WordRegex = new Regex(@"^(D|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex DoubleWordRegex = new Regex(@"^(D|CV)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex FloatRegex = new Regex(@"^(D)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex VarWordRegex = new Regex(@"^(V|Z)([0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex IntKValueRegex = new Regex(@"^K([-+]?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex IntHValueRegex = new Regex(@"^H([0-9A-F]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex FloatKValueRegex = new Regex(@"^K([-+]?([0-9]*[.])?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex VariableRegex = new Regex(@"^{([0-9a-zA-Z]+)}$", RegexOptions.Compiled);
        public static bool IsVariablePattern(string valueString)
        {
            return VariableRegex.IsMatch(valueString);
        }
        public static IValueModel ParseValue(string valueString, LadderValueType type, Device contextDevice)
        {
            switch(type)
            {
                case LadderValueType.Bool:
                    return ParseBitValue(valueString, contextDevice);
                case LadderValueType.DoubleWord:
                    return ParseDoubleWordValue(valueString, contextDevice);
                case LadderValueType.Float:
                    return ParseFloatValue(valueString, contextDevice);
                case LadderValueType.Word:
                    return ParseWordValue(valueString, contextDevice);
                default:
                    throw new ValueParseException("Unexpected input");
            }
        }
        public static IValueModel ParseValue(string valueString, LadderValueType type)
        {
            switch (type)
            {
                case LadderValueType.Bool:
                    return ParseBitValue(valueString);
                case LadderValueType.DoubleWord:
                    return ParseDoubleWordValue(valueString);
                case LadderValueType.Float:
                    return ParseFloatValue(valueString);
                case LadderValueType.Word:
                    return ParseWordValue(valueString);
                default:
                    throw new ValueParseException("Unexpected input");
            }
        }
        public static BitValue ParseBitValue(string valueString)
        {
            return ParseBitValue(valueString, Device.DefaultDevice);
        }
        public static WordValue ParseWordValue(string valueString)
        {
            return ParseWordValue(valueString, Device.DefaultDevice);
        }
        public static DoubleWordValue ParseDoubleWordValue(string valueString)
        {
            return ParseDoubleWordValue(valueString, Device.DefaultDevice);
        }
        public static FloatValue ParseFloatValue(string valueString)
        {
            return ParseFloatValue(valueString, Device.DefaultDevice);
        }
        public static BitValue ParseBitValue(string valueString, Device contextDevice)
        {
            Match match = BitRegex.Match(valueString);
            if (match.Success)
            {
                uint index = uint.Parse(match.Groups[2].Value);
                WordValue offset = null;
                if(match.Groups[4].Success)
                {
                    if (match.Groups[4].Value.ToUpper() == "V")
                    {
                        uint vindex = uint.Parse(match.Groups[5].Value);
                        if (contextDevice.VRange.AssertValue(vindex))
                        {
                            offset = new VWordValue(vindex);
                        }
                        else
                        {
                            throw new ValueParseException(string.Format("Current PLC Device do not support V{0} address", vindex));
                        }
                    }
                    else
                    {
                        if (match.Groups[4].Value.ToUpper() == "Z")
                        {
                            uint zindex = uint.Parse(match.Groups[5].Value);
                            if (contextDevice.ZRange.AssertValue(zindex))
                            {
                                offset = new ZWordValue(zindex);
                            }
                            else
                            {
                                throw new ValueParseException(string.Format("Current PLC Device do not support Z{0} address", zindex));
                            }
                        }
                    }
                }
                if(match.Groups[1].Value.ToUpper() == "X")
                {
                    if(contextDevice.XRange.AssertValue(index))
                    {
                        return new XBitValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support X{0} address", index));
                    }              
                }
                if (match.Groups[1].Value.ToUpper() == "Y")
                {
                    if (contextDevice.YRange.AssertValue(index))
                    {
                        return new YBitValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support Y{0} address", index));
                    }
                }
                if (match.Groups[1].Value.ToUpper() == "M")
                {
                    if (contextDevice.MRange.AssertValue(index))
                    {
                        return new MBitValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support M{0} address", index));
                    }
                }
                if (match.Groups[1].Value.ToUpper() == "C")
                {
                    if (contextDevice.YRange.AssertValue(index))
                    {
                        return new CBitValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support C{0} address", index));
                    }
                }
                if (match.Groups[1].Value.ToUpper() == "T")
                {
                    if (contextDevice.TRange.AssertValue(index))
                    {
                        return new TBitValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support T{0} address", index));
                    }
                }
                if (match.Groups[1].Value.ToUpper() == "S")
                {
                    if (contextDevice.SRange.AssertValue(index))
                    {
                        return new SBitValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support S{0} address", index));
                    }
                }
            }
            else
            {
                // 变量
                Match match2 = VariableRegex.Match(valueString);
                if(match2.Success)
                {
                    var name = match2.Groups[1].Value;
                    try
                    {
                        var variable = VariableManager.GetVariableByName(name);
                        VariableBitValue bitvalue = variable as VariableBitValue;
                        if (bitvalue != null)
                        {
                            return bitvalue;
                        }
                    }
                    catch
                    {
                        throw new ValueParseException(string.Format("No Bit Value found for variable {0}", name));
                    }
                    throw new ValueParseException(string.Format("Variable {0} is not a Bit Value", name));
                }
            }
            throw new ValueParseException("Unexpected input");
        }

        public static WordValue ParseWordValue(string valueString, Device contextDevice)
        {
            Match match1 = WordRegex.Match(valueString);
            if (match1.Success)
            {
                uint index = uint.Parse(match1.Groups[2].Value);
                WordValue offset = null;
                if (match1.Groups[4].Success)
                {
                    if (match1.Groups[4].Value.ToUpper() == "V")
                    {
                        uint vindex = uint.Parse(match1.Groups[5].Value);
                        if (contextDevice.VRange.AssertValue(vindex))
                        {
                            offset = new VWordValue(vindex);
                        }
                        else
                        {
                            throw new ValueParseException(string.Format("Current PLC Device do not support V{0} address", vindex));
                        }
                    }
                    else
                    {
                        if (match1.Groups[4].Value.ToUpper() == "Z")
                        {
                            uint zindex = uint.Parse(match1.Groups[5].Value);
                            if (contextDevice.ZRange.AssertValue(zindex))
                            {
                                offset = new ZWordValue(zindex);
                            }
                            else
                            {
                                throw new ValueParseException(string.Format("Current PLC Device do not support Z{0} address", zindex));
                            }
                        }
                    }
                }
                if (match1.Groups[1].Value.ToUpper() == "D")
                {
                    if (contextDevice.DRange.AssertValue(index))
                    {
                        return new DWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support D{0} address", index));
                    }
                }
                if (match1.Groups[1].Value.ToUpper() == "CV")
                {
                    if (contextDevice.CVRange.AssertValue(index))
                    {
                        return new CVWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support CV{0} address", index));
                    }
                }
                if (match1.Groups[1].Value.ToUpper() == "TV")
                {
                    if (contextDevice.TVRange.AssertValue(index))
                    {
                        return new TVWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support TV{0} address", index));
                    }
                }
                if (match1.Groups[1].Value.ToUpper() == "AI")
                {
                    if (contextDevice.AIRange.AssertValue(index))
                    {
                        return new AIWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support AI{0} address", index));
                    }
                }
                if (match1.Groups[1].Value.ToUpper() == "AO")
                {
                    if (contextDevice.AORange.AssertValue(index))
                    {
                        return new AOWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support AO{0} address", index));
                    }
                }
            }
            else
            {
                Match match2 = IntKValueRegex.Match(valueString);
                if (match2.Success)
                {
                    short kvalue = short.Parse(match2.Groups[1].Value);
                    return new KWordValue(kvalue);
                }
                else
                {
                    var match3 = IntHValueRegex.Match(valueString);
                    if(match3.Success)
                    {
                        short hvalue = short.Parse(match3.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                        return new HWordValue(hvalue);
                    }
                    else
                    {
                        Match match4 = VarWordRegex.Match(valueString);
                        if(match4.Success)
                        {
                            uint index = uint.Parse(match4.Groups[2].Value);
                            if(match4.Groups[1].Value.ToUpper() == "V")
                            {
                                if(contextDevice.VRange.AssertValue(index))
                                {
                                    return new VWordValue(index);
                                }
                                else
                                {
                                    throw new ValueParseException(string.Format("Current PLC Device do not support V{0} address", index));
                                }
                            }
                            else
                            {
                                if (match4.Groups[1].Value.ToUpper() == "Z")
                                {
                                    if (contextDevice.ZRange.AssertValue(index))
                                    {
                                        return new ZWordValue(index);
                                    }
                                    else
                                    {
                                        throw new ValueParseException(string.Format("Current PLC Device do not support Z{0} address", index));
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 变量
                            Match match5 = VariableRegex.Match(valueString);
                            if (match5.Success)
                            {
                                var name = match5.Groups[1].Value;
                                try
                                {
                                    var variable = VariableManager.GetVariableByName(name);
                                    var wordvalue = variable as VariableWordValue;
                                    if (wordvalue != null)
                                    {
                                        return wordvalue;
                                    }
                                }
                                catch
                                {
                                    throw new ValueParseException(string.Format("No Bit Value found for variable {0}", name));
                                }
                                throw new ValueParseException(string.Format("Variable {0} is not a Word Value", name));
                            }               
                        }
                    }
                }
            }
            throw new ValueParseException("Unexpected input");
        }

        public static DoubleWordValue ParseDoubleWordValue(string valueString, Device contextDevice)
        {
            Match match1 = DoubleWordRegex.Match(valueString);
            if (match1.Success)
            {
                uint index = uint.Parse(match1.Groups[2].Value);
                WordValue offset = null;
                if (match1.Groups[4].Success)
                {
                    if (match1.Groups[4].Value.ToUpper() == "V")
                    {
                        uint vindex = uint.Parse(match1.Groups[5].Value);
                        if (contextDevice.VRange.AssertValue(vindex))
                        {
                            offset = new VWordValue(vindex);
                        }
                        else
                        {
                            throw new ValueParseException(string.Format("Current PLC Device do not support V{0} address", vindex));
                        }
                    }
                    else
                    {
                        if (match1.Groups[4].Value.ToUpper() == "Z")
                        {
                            uint zindex = uint.Parse(match1.Groups[5].Value);
                            if (contextDevice.ZRange.AssertValue(zindex))
                            {
                                offset = new ZWordValue(zindex);
                            }
                            else
                            {
                                throw new ValueParseException(string.Format("Current PLC Device do not support Z{0} address", zindex));
                            }
                        }
                    }
                }
                if(match1.Groups[1].Value.ToUpper() == "D")
                {
                    if(contextDevice.DRange.AssertValue(index))
                    {
                        return new DDoubleWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support D{0} address", index));
                    }
                }
                if(match1.Groups[1].Value.ToUpper() == "CV")
                {
                    if (contextDevice.CVRange.AssertValue(index))
                    {
                        return new CV32DoubleWordValue(index, offset);
                    }
                    else
                    {
                        throw new ValueParseException(string.Format("Current PLC Device do not support CV32{0} address", index));
                    }
                }
            }
            else
            {
                Match match2 = IntKValueRegex.Match(valueString);
                if (match2.Success)
                {
                    return new KDoubleWordValue(int.Parse(match2.Groups[1].Value));
                }
                else
                {
                    var match3 = IntHValueRegex.Match(valueString);
                    if (match3.Success)
                    {
                        return new HDoubleWordValue(int.Parse(match2.Groups[1].Value, System.Globalization.NumberStyles.HexNumber));
                    }
                    else
                    {
                        // 变量
                        Match match4 = VariableRegex.Match(valueString);
                        if (match4.Success)
                        {
                            var name = match4.Groups[1].Value;
                            try
                            {
                                var variable = VariableManager.GetVariableByName(name);
                                var doublewordvalue = variable as VariableDoubleWordValue;
                                if (doublewordvalue != null)
                                {
                                    return doublewordvalue;
                                }
                            }
                            catch
                            {
                                throw new ValueParseException(string.Format("No DoubleWord Value found for variable {0}", name));
                            }
                            throw new ValueParseException(string.Format("Variable {0} is not a DoubleWord Value", name));
                        }
                    }
                }
            }
            throw new ValueParseException("Unexpected input");
        }

        public static FloatValue ParseFloatValue(string valueString, Device contextDevice)
        {
            Match match1 = FloatRegex.Match(valueString);
            if (match1.Success)
            {
                uint index = uint.Parse(match1.Groups[2].Value);
                WordValue offset = null;
                if (match1.Groups[4].Success)
                {
                    if (match1.Groups[4].Value.ToUpper() == "V")
                    {
                        uint vindex = uint.Parse(match1.Groups[5].Value);
                        if(contextDevice.VRange.AssertValue(vindex))
                        {
                            offset = new VWordValue(vindex);
                        }
                        else
                        {
                            throw new ValueParseException(string.Format("Current PLC Device do not support V{0} address", vindex));
                        }
                    }
                    else
                    {            
                        if (match1.Groups[4].Value.ToUpper() == "Z")
                        {
                            uint zindex = uint.Parse(match1.Groups[5].Value);
                            if (contextDevice.ZRange.AssertValue(zindex))
                            {
                                offset = new ZWordValue(zindex);
                            }
                            else
                            {
                                throw new ValueParseException(string.Format("Current PLC Device do not support Z{0} address", zindex));
                            }
                        }
                    }
                }
                if(contextDevice.DRange.AssertValue(index))
                {
                    return new DFloatValue(index, offset);
                }
                else
                {
                    throw new ValueParseException(string.Format("Current PLC Device do not support D{0} address", index));
                }
            }
            else
            {
                Match match2 = FloatKValueRegex.Match(valueString);
                if (match2.Success)
                {
                    return new KFloatValue(float.Parse(match2.Groups[1].Value));
                }
                else
                {
                    // 变量
                    Match match3 = VariableRegex.Match(valueString);
                    if (match3.Success)
                    {
                        var name = match3.Groups[1].Value;
                        try
                        {
                            var variable = VariableManager.GetVariableByName(name);
                            var floatvalue = variable as VariableFloatValue;
                            if (floatvalue != null)
                            {
                                return floatvalue;
                            }
                        }
                        catch
                        {
                            throw new ValueParseException(string.Format("No Float Value found for variable {0}", name));
                        }
                        throw new ValueParseException(string.Format("Variable {0} is not a Float Value", name));
                    }
                }
            }
            throw new ValueParseException("Unexpected input");
        }

        public static IVariableValue CreateVariableValue(string name, string mappedValuestring, LadderValueType type, string comment)
        {

            switch (type)
            {
                case LadderValueType.Bool:
                    return CreateVariableBitValue(name, mappedValuestring, comment);
                case LadderValueType.DoubleWord:
                    return CreateVariableBitValue(name, mappedValuestring, comment);
                case LadderValueType.Float:
                    return CreateVariableBitValue(name, mappedValuestring, comment);
                case LadderValueType.Word:
                    return CreateVariableBitValue(name, mappedValuestring, comment);
            }      
            throw new ValueParseException("Unexpected input");
        }
        public static VariableBitValue CreateVariableBitValue(string name, string mappedValuestring, string comment)
        {
            // 匿名 
            if (mappedValuestring == string.Empty)
            {
                VariableBitValue variable = new VariableBitValue(name, comment);
                return variable;
            }
            else
            {
                // 非匿名
                var value = ParseBitValue(mappedValuestring, PLCDevice.Device.DefaultDevice);
                if(value.IsVariable)
                {
                    throw new ValueParseException("Can not map variable to variable");
                }
                else
                {
                    VariableBitValue variable = new VariableBitValue(name, value, comment);
                    return variable;
                }
            }
        }
        public static VariableWordValue CreateVariableWordValue(string name, string mappedValuestring, string comment)
        {
            // 匿名 
            if (mappedValuestring == string.Empty)
            {
                VariableWordValue variable = new VariableWordValue(name, comment);
                return variable;
            }
            else
            {
                // 非匿名
                var value = ParseWordValue(mappedValuestring, PLCDevice.Device.DefaultDevice);
                if (value.IsVariable)
                {
                    throw new ValueParseException("Can not map variable to variable");
                }
                else
                {
                    VariableWordValue variable = new VariableWordValue(name, value, comment);
                    return variable;
                }
            }
        }
        public static VariableFloatValue CreateVariableFloatValue(string name, string mappedValuestring, string comment)
        {
            // 匿名 
            if (mappedValuestring == string.Empty)
            {
                VariableFloatValue variable = new VariableFloatValue(name, comment);
                return variable;
            }
            else
            {
                // 非匿名
                var value = ParseFloatValue(mappedValuestring, PLCDevice.Device.DefaultDevice);
                if (value.IsVariable)
                {
                    throw new ValueParseException("Can not map variable to variable");
                }
                else
                {
                    VariableFloatValue variable = new VariableFloatValue(name, value, comment);
                    return variable;
                }
            }
        }
        public static VariableDoubleWordValue CreateVariableDoubleWordValue(string name, string mappedValuestring, string comment)
        {
            // 匿名 
            if (mappedValuestring == string.Empty)
            {
                VariableDoubleWordValue variable = new VariableDoubleWordValue(name, comment);
                return variable;
            }
            else
            {
                // 非匿名
                var value = ParseDoubleWordValue(mappedValuestring, PLCDevice.Device.DefaultDevice);
                if (value.IsVariable)
                {
                    throw new ValueParseException("Can not map variable to variable");
                }
                else
                {
                    VariableDoubleWordValue variable = new VariableDoubleWordValue(name, value, comment);
                    return variable;
                }
            }
        }
    }
}
