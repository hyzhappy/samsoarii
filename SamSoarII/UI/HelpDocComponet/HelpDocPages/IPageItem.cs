using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages
{
    public interface IPageItem
    {
        int PageIndex { get; }
        string TabHeader { get; }
    }
}
