using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{ 
    public enum VisualType
    {
        Unit,
        Property,
        Comment,
        Brop
    }
    public class LadderDrawingVisual : DrawingVisual, IRenderModel
    {
        private VisualType type;
        public VisualType Type { get { return type; } }
        public LadderDrawingVisual(IViewModel core, VisualType type)
        {
            this.core = core;
            this.type = type;
        }
        public void Render()
        {
            if (core == null) return;
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate () 
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
                        case VisualType.Brop:
                            DrawingManager.DrawingBrop(context, core);
                            break;
                    }
                }
            });
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
