using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public class LightCanvas : Canvas
    {
        private List<Visual> visuals = new List<Visual>();

        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        public bool Contains(Visual visual)
        {
            return visual != null && visuals.Contains(visual);
        }

        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this,point);
            return hitResult.VisualHit as DrawingVisual;
        }
    }
}
