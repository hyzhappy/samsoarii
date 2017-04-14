using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages
{
    public class HelpDocFrame :Frame
    {
        private IPageItem _item;
        public string TabHeader
        {
            get
            {
                return _item.TabHeader;
            }
        }
        public int PageIndex
        {
            get
            {
                return _item.PageIndex;
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
        public HelpDocFrame(IPageItem item)
        {
            _item = item;
            Navigate(_item);
        }
    }
}
