using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;

namespace SamSoarII.Shell.Models
{ 
    public enum VisualType
    {
        Shape,
        Property,
        Comment,
        BrpoBrush,
        OnOffBrush
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
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () 
            {
                using (DrawingContext context = RenderOpen())
                {
                    switch (type)
                    {
                        case VisualType.Shape:
                            DrawingManager.DrawingUnitShape(context, core);
                            break;
                        case VisualType.Property:
                            DrawingManager.DrawingUnitProperty(context, core);
                            break;
                        case VisualType.Comment:
                            DrawingManager.DrawingUnitCommnet(context, core);
                            break;
                        case VisualType.BrpoBrush:
                            DrawingManager.DrawingUnitBrpoBrush(context, core);
                            break;
                        case VisualType.OnOffBrush:
                            DrawingManager.DrawingUnitOnOffBrush(context, core);
                            break;
                    }
                }
            });
        }
        public void Dispose()
        {

        }
        #region Core
        private IViewModel core;
        public IViewModel Core { get { return core; } }
        #endregion
    }
}
