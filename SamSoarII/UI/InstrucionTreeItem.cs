using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.AppMain.UI
{
    public class InstrucionTreeItem : TreeViewItem
    {
        public int Index { get; set; }
        public InstrucionTreeItem(int index)
        {
            Index = index;
        }

    }
}
