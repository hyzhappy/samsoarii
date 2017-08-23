using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public class LadderCanvas : Canvas
    {
        public LadderCanvas() : base()
        {
            units = new List<LadderUnitViewModel>();
        }

        #region Number
        
        private List<LadderUnitViewModel> units;
        public IList<LadderUnitViewModel> Units { get { return this.units; } }
        protected override int VisualChildrenCount { get { return units.Count(); } }
        protected override Visual GetVisualChild(int index)
        {
            return units[index];
        }

        #endregion

        public void Add(LadderUnitViewModel unit)
        {
            if (!units.Contains(unit))
            {
                units.Add(unit);
                AddVisualChild(unit);
                AddLogicalChild(unit);
            }
        }

        public void Remove(LadderUnitViewModel unit)
        {
            if (units.Contains(unit))
            {
                units.Remove(unit);
                RemoveVisualChild(unit);
                RemoveLogicalChild(unit);
            }
        }
        
    }
}
