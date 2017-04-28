using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages
{
    public interface IPageItem
    {
        int PageIndex { get; }
        string TabHeader { get; }
    }
}
