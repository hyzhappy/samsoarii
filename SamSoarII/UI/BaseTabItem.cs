using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.AppMain.UI
{
    public abstract class BaseTabItem : TabItem
    {
        public abstract string TabName { get; }
        public BaseTabItem()
        {
            
        }
        public abstract void OnItemSelected();
        public abstract void ClearElements();
    }
}
