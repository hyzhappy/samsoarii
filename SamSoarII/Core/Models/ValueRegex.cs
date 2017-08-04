using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSoarII.Core.Models
{
    public class ValueRegex : Regex
    {
        public ValueRegex(string _pattern, IEnumerable<string> _supports) 
            : base(_pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
        {
            supports = _supports.ToArray();
        }

        #region Number

        private string[] supports;
        public IList<string> Supports { get { return this.supports; } }

        #endregion
    }
    
}
