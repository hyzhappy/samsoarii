using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    public class ReplaceFormat
    {
        public ReplaceFormat(string text)
        {
            if (text.Length == 0)
            {
                mode = Modes.Error;
                return;
            }
            string[] args = text.Split(' ');
            if (args.Length == 1)
            {
                if (args[0].Equals("*"))
                {
                    mode = Modes.Unit;
                    isanyinst = true;
                }
                else if (LadderUnitModel.Formats.Where(f => f.Name.Equals(args[0])).Count() > 0)
                {
                    mode = Modes.Unit;
                    isanyinst = false;
                    instname = args[0];
                    AppendAnyRanges();
                }
                else if (args[0][0] == '$')
                {
                    try
                    {
                        mode = Modes.Base;
                        ranges = new List<ValueRange> { new ValueRange(args[0].Substring(1)) };
                        if (ranges[0].Base.Type == ValueModel.Types.STRING
                         || ranges[0].Base.IsWordBit
                         || ranges[0].Base.IsBitWord
                         || ranges[0].Base.IsBitDoubleWord)
                        {
                            throw new ValueParseException("cannot be string.", ranges[0].Base.Format);
                        }
                    }
                    catch (ValueParseException)
                    {
                        mode = Modes.Error;
                        ranges = null;
                    }
                }
                else
                {
                    try
                    {
                        mode = Modes.Value;
                        ranges = new List<ValueRange> { new ValueRange(args[0]) };
                    }
                    catch (ValueParseException)
                    {
                        mode = Modes.Error;
                        ranges = null;
                    }
                }
                return;
            }
            if (args[0].Equals("*"))
            {
                isanyinst = true;
            }
            else if (LadderUnitModel.Formats.Where(f => f.Name.Equals(args[0])).Count() > 0)
            {
                isanyinst = false;
                instname = args[0];
            }
            else
            {
                mode = Modes.Error;
                return;
            }
            mode = Modes.Unit;
            ranges = new List<ValueRange>();
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "*":
                        ranges.Add(new ValueRange("*", true));
                        break;
                    case ".": 
                        if (i < args.Length - 1)
                            mode = Modes.Error;
                        else 
                            AppendAnyRanges();
                        return;
                    default:
                        ranges.Add(new ValueRange(args[i]));
                        break;
                }
            }
        }

        #region Number
        
        public enum Modes { Error, Unit, Value, Base };
        private Modes mode;
        public Modes Mode { get { return this.mode; } }

        private bool isanyinst;
        public string instname;

        private List<ValueRange> ranges;
        public IList<ValueRange> Ranges { get { return this.ranges; } }
        public void AppendAnyRanges()
        {
            if (ranges == null) ranges = new List<ValueRange>();
            ranges.AddRange(new ValueRange[5 - ranges.Count()]);
            for (int i = 4; i >= 0; i--)
                if (ranges[i] == null) ranges[i] = new ValueRange("*", true);
        }

        #endregion

        #region Method

        public bool Match(LadderUnitModel unit)
        {
            switch (mode)
            {
                case Modes.Error:
                    return false;
                case Modes.Value:
                    foreach (ValueModel vmodel in unit.Children)
                        if (ranges[0].Contains(vmodel)) return true;
                    return false;
                case Modes.Unit:
                case Modes.Base:
                    if (!isanyinst && !unit.InstName.Equals(instname))
                        return false;
                    for (int i = 0; i < unit.Children.Count; i++)
                        if (i >= ranges.Count() || !ranges[i].Contains(unit.Children[i]))
                            return false;
                    return true;
                default:
                    return false;
            }
        }
        public ReplaceCommand Replace(LadderUnitModel unit, ReplaceFormat origin)
        {
            StringBuilder newargs = new StringBuilder();
            switch (mode)
            {
                case Modes.Error:
                    return null;
                case Modes.Value:
                    newargs.Append(unit.InstName);
                    for (int i = 0; i < unit.Children.Count; i++)
                    {
                        ValueModel value = unit.Children[i];
                        ValueRange oldrange = origin.Ranges[0];
                        ValueRange newrange = Ranges[0];
                        ReplaceValue(newargs, value, oldrange, newrange);
                    }
                    return new ReplaceCommand(unit, newargs.ToString());
                case Modes.Unit:
                case Modes.Base:
                    newargs.Append(isanyinst ? unit.InstName : instname);
                    for (int i = 0; i < unit.Children.Count; i++)
                    {
                        ValueModel value = unit.Children[i];
                        ValueRange oldrange = i <= origin.Ranges.Count ? origin.Ranges[i] : null;
                        ValueRange newrange = i < Ranges.Count ? Ranges[i] : null;
                        ReplaceValue(newargs, value, oldrange, newrange);
                    }
                    return new ReplaceCommand(unit, newargs.ToString());
                default:
                    return null;
            }
        }

        private void ReplaceValue(StringBuilder newargs, ValueModel value, ValueRange oldrange, ValueRange newrange)
        {
            if (newrange != null)
            {
                newargs.Append(" ");
                int offset = 0;
                if (newrange.IsAny)
                    newargs.Append(value.Text);
                else if (oldrange == null || oldrange.IsAny)
                    newargs.Append(newrange.Base.Text);
                else if (!oldrange.Contains(value))
                    newargs.Append(value.Text);
                else if (newrange.Base.IsWordBit)
                {
                    offset = (newrange.Base.Offset >> 4);
                    if (!oldrange.IsAny && value.IsWordBit && oldrange.Base.IsWordBit && newrange.OffsetCount > 1)
                        offset += (value.Offset >> 4) - (oldrange.Base.Offset >> 4);
                    newargs.Append(String.Format("{0:s}{1:d}",
                        ValueModel.NameOfBases[(int)(newrange.Base.Base)], offset));
                    offset = (newrange.Base.Offset & 15);
                    if (!oldrange.IsAny && value.IsWordBit && oldrange.Base.IsWordBit && newrange.FlagCount > 1)
                        offset += (value.Offset & 15) - (oldrange.Base.Offset & 15);
                    offset &= 15;
                    newargs.Append(String.Format(".{1:x}", offset));
                }
                else if (Mode == Modes.Base && value.IsWordBit)
                {
                    offset = newrange.Base.Offset;
                    if (!oldrange.IsAny && newrange.OffsetCount > 1)
                    {
                        if (oldrange.Base.IsWordBit)
                            offset += (value.Offset >> 4) - (oldrange.Base.Offset >> 4);
                        else
                            offset += (value.Offset >> 4) - oldrange.Base.Offset;
                    }
                    newargs.Append(String.Format("{0:s}{1:d}",
                        ValueModel.NameOfBases[(int)(newrange.Base.Base)], offset));
                    offset = value.Offset & 15;
                    newargs.Append(String.Format(".{1:x}", offset));
                }
                else
                {
                    if (newrange.Base.IsBitWord || newrange.Base.IsBitDoubleWord)
                    {
                        offset = newrange.Base.Size;
                        if (!oldrange.IsAny
                         && (value.IsBitWord || value.IsBitDoubleWord)
                         && (oldrange.Base.IsBitWord || oldrange.Base.IsBitDoubleWord)
                         && newrange.FlagCount > 1)
                        {
                            offset += value.Size - oldrange.Base.Size;
                        }
                        newargs.Append(String.Format("K{0:d}", offset));
                    }
                    else if (Mode == Modes.Base && (value.IsBitWord || value.IsBitDoubleWord))
                    {
                        offset = value.Size;
                        newargs.Append(String.Format("K{0:d}", offset));
                    }
                    offset = newrange.Base.Offset;
                    if (!oldrange.IsAny && newrange.OffsetCount > 1)
                        offset += value.Offset - oldrange.Base.Offset;
                    newargs.Append(String.Format("{0:s}{1:d}",
                        ValueModel.NameOfBases[(int)(newrange.Base.Base)], offset));
                }
                if (newrange.Base.Intra != ValueModel.Bases.NULL)
                {
                    offset = newrange.Base.IntraOffset;
                    if (!oldrange.IsAny 
                     && value.Intra == newrange.Base.Intra
                     && oldrange.Base.Intra == newrange.Base.Intra
                     && newrange.IntraCount > 1)
                    {
                        offset += value.IntraOffset - oldrange.Base.IntraOffset;
                    }
                    newargs.Append(String.Format("{0:s}{1:d}",
                        ValueModel.NameOfBases[(int)(newrange.Base.Intra)], offset));
                }
                else if (Mode == Modes.Base && value.Intra != ValueModel.Bases.NULL)
                {
                    offset = value.IntraOffset;
                    if (!oldrange.IsAny
                     && value.Intra == newrange.Base.Intra
                     && oldrange.Base.Intra == newrange.Base.Intra
                     && newrange.IntraCount > 1)
                    {
                        offset += value.IntraOffset - oldrange.Base.IntraOffset;
                    }
                    newargs.Append(String.Format("{0:s}{1:d}",
                        ValueModel.NameOfBases[(int)(newrange.Base.Intra)], offset));
                }
            }
        }
        #endregion

    }
}
