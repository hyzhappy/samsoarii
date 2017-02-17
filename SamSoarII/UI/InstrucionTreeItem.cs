using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.AppMain.UI
{
    public class InstrucionTreeItem : TreeViewItem
    {
        public int InstructionIndex
        {
            get
            {
                return (int)GetValue(InstructionIndexProperty);
            }
            set
            {
                SetValue(InstructionIndexProperty, value);
            }
        }

        public static readonly DependencyProperty InstructionIndexProperty = DependencyProperty.Register("InstructionIndex", typeof(int), typeof(InstrucionTreeItem));
        public InstrucionTreeItem()
        {
          
        }


    }
}
