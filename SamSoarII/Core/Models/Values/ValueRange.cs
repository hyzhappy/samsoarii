using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSoarII.Core.Models
{
    public class ValueRange
    {
        #region Resources

        private static Regex NormalRegex = new Regex(
            @"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+))
            ((V|Z)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+)))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex WordBitRegex = new Regex(
            @"^(D|V|Z)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+))
            (\.)((\[([0-9]+)\.\.([0-9]+)\])|([0-9])+))
            ((V|Z)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+)))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex BitWordRegex = new Regex(
            @"^(K)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+))
            (X|Y|S|M)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+))
            ((V|Z)((\[([0-9]+)\.\.([0-9]+)\])|([0-9]+)))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        public ValueRange(string text, bool _isany = false)
        {
            isany = _isany;
            if (isany) return;
            StringBuilder basetext = new StringBuilder();
            Match m = NormalRegex.Match(text);
            if (m.Success)
            {
                Build(basetext, m.Groups, 1, ref oct);
                if (m.Groups[7].Value.Length > 0)
                    Build(basetext, m.Groups, 8, ref ict);
            }
            else
            {
                m = WordBitRegex.Match(text);
                if (m.Success)
                {
                    Build(basetext, m.Groups, 1, ref oct);
                    Build(basetext, m.Groups, 7, ref fct);
                    if (m.Groups[13].Value.Length > 0)
                        Build(basetext, m.Groups, 14, ref ict);
                }
                else
                {
                    m = BitWordRegex.Match(text);
                    if (m.Success)
                    {
                        Build(basetext, m.Groups, 1, ref fct);
                        Build(basetext, m.Groups, 7, ref oct);
                        if (m.Groups[13].Value.Length > 0)
                            Build(basetext, m.Groups, 14, ref ict);
                    }
                    else
                    {
                        basetext.Append(text);
                        oct = 1; ict = 1;
                    }
                }
            }
            bas = null;
            try
            {
                ValueModel.Analyzer_Bit.Text = basetext.ToString();
                bas = ValueModel.Analyzer_Bit.Clone();
            }
            catch (ValueParseException)
            {
            }
            if (bas == null)
            {
                try
                {
                    ValueModel.Analyzer_Word.Text = basetext.ToString();
                    bas = ValueModel.Analyzer_Word.Clone();
                }
                catch (ValueParseException)
                {
                }
            }
            if (bas == null)
            {
                try
                {
                    ValueModel.Analyzer_DWord.Text = basetext.ToString();
                    bas = ValueModel.Analyzer_DWord.Clone();
                }
                catch (ValueParseException)
                {
                }
            }
            if (bas == null)
            {
                try
                {
                    ValueModel.Analyzer_Float.Text = basetext.ToString();
                    bas = ValueModel.Analyzer_Float.Clone();
                }
                catch (ValueParseException)
                {
                }
            }
            if (bas == null)
            {
                try
                {
                    ValueModel.Analyzer_String.Text = basetext.ToString();
                    bas = ValueModel.Analyzer_String.Clone();
                }
                catch (ValueParseException)
                {
                }
            }
        }
        
        private void Build(StringBuilder basetext, GroupCollection groups, int id, ref int count)
        {
            basetext.Append(groups[id].Value);
            if (groups[id+2].Value.Length > 0)
            {
                basetext.Append(groups[id+3].Value);
                count = int.Parse(groups[id+4].Value) - int.Parse(groups[id+3].Value) + 1;
            }
            else
            {
                basetext.Append(groups[id+5].Value);
                count = 1;
            }
        }

        #region Number

        private bool isany;
        public bool IsAny { get { return this.isany; } }
        public bool IsLegal { get { return isany || bas != null; } }

        private ValueModel bas;
        public ValueModel Base { get { return this.bas; } }

        private int oct;
        public int OffsetCount { get { return this.oct; } }

        private int ict;
        public int IntraCount { get { return this.ict; } }

        private int fct;
        public int FlagCount { get { return this.fct; } }
        

        #endregion

        #region Method

        public bool Contains(ValueModel vmodel)
        {
            bool ret = true;
            //ret &= bas.Type == bas.Type;
            ret &= bas.Base == vmodel.Base;
            ret &= bas.Intra == vmodel.Intra;
            ret &= bas.Intra == ValueModel.Bases.NULL || (bas.IntraOffset <= vmodel.IntraOffset && vmodel.IntraOffset < bas.IntraOffset + ict);
            if (bas.IsWordBit)
            {
                ret &= vmodel.IsWordBit;
                ret &= (bas.Offset >> 4) <= (vmodel.Offset >> 4) && (vmodel.Offset >> 4) < (bas.Offset >> 4) + oct;
                ret &= (bas.Offset & 15) <= (vmodel.Offset & 15) && (vmodel.Offset & 15) < (bas.Offset & 15) + fct;
            }
            else  
            {
                ret &= bas.Offset <= vmodel.Offset && vmodel.Offset < bas.Offset + oct;
                if (bas.IsBitWord || bas.IsBitDoubleWord)
                {
                    ret &= !(bas.IsBitWord ^ vmodel.IsBitWord);
                    ret &= !(bas.IsBitDoubleWord ^ vmodel.IsBitDoubleWord);
                    ret &= bas.Size <= vmodel.Size && vmodel.Size < bas.Size + fct; 
                }
            }
            return ret;
        }
        
        #endregion

    }
}
