using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Models
{
    public class SpecialVisualUnitModel : BaseVisualUnitModel, IResource
    {
        public override IResource Create(params object[] args)
        {
            return new SpecialVisualUnitModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            //TODO Render
            RenderAll();
            recreating = false;
        }

        public SpecialVisualUnitModel(LadderUnitModel _core)
        {
            if (_core != null)
            {
                Recreate(_core);
                //添加形状
                visuals.Add(VisualType.Shape, new LadderDrawingVisual[1]);
                //添加仿真时的画刷(只有断点画刷)
                visuals.Add(VisualType.Brush, new LadderDrawingVisual[1]);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            AllResourceManager.Dispose(this);
        }
    }
}
