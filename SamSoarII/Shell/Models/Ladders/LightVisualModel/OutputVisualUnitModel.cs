using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Models
{
    public class OutputVisualUnitModel : BaseVisualUnitModel, IResource
    {
        public override IResource Create(params object[] args)
        {
            return new OutputVisualUnitModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            //TODO Render
            RenderAll();
            recreating = false;
        }

        public OutputVisualUnitModel(LadderUnitModel _core)
        {
            if (_core != null)
            {
                Recreate(_core);
                //添加形状
                visuals.Add(VisualType.Shape, new LadderDrawingVisual[1]);
                //添加仿真及监视时的画刷(第一个为至ON时的画刷，第二个为断点画刷)
                visuals.Add(VisualType.Brush, new LadderDrawingVisual[2]);
                switch (_core.Type)
                {
                    case LadderUnitModel.Types.OUT:
                    case LadderUnitModel.Types.OUTIM:
                        //对于以上类型只有一个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[1]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[1]);
                        break;
                    case LadderUnitModel.Types.SET:
                    case LadderUnitModel.Types.SETIM:
                    case LadderUnitModel.Types.RST:
                    case LadderUnitModel.Types.RSTIM:
                        //对于以上类型有两个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[2]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[2]);
                        break;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            AllResourceManager.Dispose(this);
        }
    }
}
