using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages
{
    public delegate void NavigateToPageEventHandler(NavigateToPageEventArgs e);
    public class NavigateToPageEventArgs : EventArgs
    {
        private int _pageIndex;
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }
        public NavigateToPageEventArgs(int pageIndex)
        {
            PageIndex = pageIndex;
        }
    }
}
