using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    public class HelpDocTreeItem : TreeViewItem
    {
        public int PageIndex
        {
            get
            {
                return (int)GetValue(PageIndexProperty);
            }
            set
            {
                SetValue(PageIndexProperty, value);
            }
        }

        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(HelpDocTreeItem));
        public HelpDocTreeItem()
        {

        }
    }
}
