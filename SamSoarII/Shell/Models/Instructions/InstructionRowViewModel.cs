using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public class InstructionRowViewModel : DrawingVisual, IResource
    {
        #region IResource

        private int resourceid;
        public int ResourceID
        {
            get { return this.resourceid; }
            set { this.resourceid = value; }
        }

        public IResource Create(params object[] args)
        {
            return new InstructionRowViewModel((PLCOriginInst)args[0]);
        }

        private static double[] xstarts = { 40, 40 + 41, 40 + 121, 40 + 201, 40 + 281, 40 + 361, 40 + 441, 40 + 521 };
        private static double[] widths = { 38, 78, 78, 78, 78, 78, 78, 78, 600 };
        public void Recreate(params object[] args)
        {
            Core = (PLCOriginInst)args[0];
            Update();
        }

        #endregion

        public InstructionRowViewModel(PLCOriginInst _core)
        {
            Recreate(_core);
        }

        public void Dispose()
        {
            Core = null;
            AllResourceManager.Dispose(this);
            Update();
        }

        #region Number

        private PLCOriginInst core;
        public PLCOriginInst Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                PLCOriginInst _core = core;
                this.core = null;
                if (_core != null && _core.View != null) _core.View = null;
                this.core = value;
                if (core != null && core.View != this) core.View = this;
            }
        }

        private InstructionRowCanvas cvparent;
        public InstructionRowCanvas CVParent
        {
            get
            {
                return this.cvparent;
            }
            set
            {
                if (cvparent == value) return;
                if (cvparent != null) cvparent.Remove(this);
                this.cvparent = value;
                if (cvparent != null) cvparent.Add(this);
            }
        }
        
        #endregion

        public void Update()
        {
            using (DrawingContext context = RenderOpen())
            {
                if (core?.Inst == null) return;
                LadderUnitModel unit = core.Inst.ProtoType;
                Brush background;
                Brush foreground;
                string text;
                for (int i = 0; i < 7; i++)
                {
                    background = (((i + core.ID) & 1) == 0) ? Brushes.AliceBlue : Brushes.LightCyan;
                    foreground = unit != null ? Brushes.Black : Brushes.Gray;
                    text = i == 0 ? (core.ID + 1).ToString() : core[i - 1];
                    DrawingText(context, text, xstarts[i], widths[i], background, foreground);
                }
                if (Core.Parent.IsCommentMode)
                {
                    StringBuilder tbtext = new StringBuilder("");
                    if (unit != null && unit.ValueManager != null)
                    {
                        tbtext.Append("// ");
                        foreach (ValueModel value in unit.AllChildren)
                            tbtext.Append(String.Format("{0:s}:{1:s}, ", value.Text, value.Comment));
                    }
                    DrawingText(context, tbtext.ToString(), xstarts[7], widths[7], Brushes.Transparent, Brushes.Green);
                }
            }
        }

        #region Drawing

        static private FontFamily fontfamily = new FontFamily("Courier New");
        static private int fontsize = 14;
        private void DrawingText(DrawingContext context, string text, double x, double width, Brush background, Brush foreground)
        {
            Typeface face = new Typeface(fontfamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, fontsize, foreground);
            double y = core.Parent.CanvasTop + 26 + core.ID * 20;
            //context.DrawRectangle(background, null, new Rect(x, y, width, 18));
            context.DrawText(formattedText, new Point(x, y));
        }

        #endregion
    }
}
