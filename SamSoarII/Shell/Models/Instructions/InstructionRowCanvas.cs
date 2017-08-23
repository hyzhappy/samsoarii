using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public class InstructionRowCanvas : Canvas
    {
        public InstructionRowCanvas() : base()
        {
            rows = new List<InstructionRowViewModel>();
        }

        #region Number

        private List<InstructionRowViewModel> rows;
        public IList<InstructionRowViewModel> Rows { get { return this.rows; } }
        protected override int VisualChildrenCount { get { return rows.Count(); } }
        protected override Visual GetVisualChild(int index)
        {
            return rows[index];
        }

        #endregion

        public void Add(InstructionRowViewModel row)
        {
            if (!rows.Contains(row))
            {
                rows.Add(row);
                AddVisualChild(row);
                AddLogicalChild(row);
            }
        }

        public void Remove(InstructionRowViewModel row)
        {
            if (rows.Contains(row))
            {
                rows.Remove(row);
                RemoveVisualChild(row);
                RemoveLogicalChild(row);
            }
        }

    }
}
