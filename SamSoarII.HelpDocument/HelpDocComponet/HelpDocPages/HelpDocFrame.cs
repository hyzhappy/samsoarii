using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages
{
    public class HelpDocFrame : Frame
    {
        private IPageItem _page;
        public IPageItem Page
        {
            get
            {
                return _page;
            }
        }
        public string TabHeader
        {
            get
            {
                return _page.TabHeader;
            }
        }
        public int PageIndex
        {
            get
            {
                return _page.PageIndex;
            }
        }
        private bool _isUsed = false;
        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            set
            {
                _isUsed = value;
            }
        }
        public HelpDocFrame(IPageItem page)
        {
            _page = page;
            Navigate(_page);
        }
    }
}
