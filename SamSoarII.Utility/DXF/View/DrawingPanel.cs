using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DrawingPanel : Panel
    {
        private List<Visual> elements = new List<Visual>();
        protected override int VisualChildrenCount { get { return elements.Count(); } }
        protected override Visual GetVisualChild(int index)
        {
            return elements[index];
        }
        
        public void Add(Visual unit)
        {
            if (!elements.Contains(unit))
            {
                elements.Add(unit);
                AddVisualChild(unit);
                AddLogicalChild(unit);
            }
        }

        public void Remove(Visual unit)
        {
            if (elements.Contains(unit))
            {
                elements.Remove(unit);
                RemoveVisualChild(unit);
                RemoveLogicalChild(unit);
            }
        }
    }
}
