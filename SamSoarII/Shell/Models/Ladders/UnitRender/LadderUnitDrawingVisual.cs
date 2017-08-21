using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{ 
    public enum VisualType
    {
        Unit,
        Property,
        Comment,
        SelectRect,
        SelectArea
    }
    public class LadderUnitDrawingVisual : DrawingVisual, IRenderModel
    {
        private VisualType type;
        public VisualType Type { get { return type; } }
        public LadderUnitDrawingVisual(IViewModel core, VisualType type)
        {
            this.core = core;
            this.type = type;
        }
        public void Render()
        {
            using (DrawingContext context = RenderOpen())
            {
                switch (type)
                {
                    case VisualType.Unit:
                        DrawingManager.DrawingUnit(context, core);
                        break;
                    case VisualType.Property:
                        DrawingManager.DrawingUnitProperty(context, core);
                        break;
                    case VisualType.Comment:
                        DrawingManager.DrawingUnitCommnet(context, core);
                        break;
                    case VisualType.SelectRect:
                        DrawingManager.DrawingSelectRect(context, core);
                        break;
                    case VisualType.SelectArea:
                        DrawingManager.DrawingSelectArea(context, core);
                        break;
                }
            }
        }

        public void Dispose()
        {
            core = null;
        }
        #region Core
        private IViewModel core;
        public IViewModel Core { get { return core; } }
        #endregion
    }
}
