using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Models
{
    public class InputVisualUnitModel : BaseVisualUnitModel, IResource
    {
        public override IResource Create(params object[] args)
        {
            return new InputVisualUnitModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            //TODO Render
            RenderAll();
            recreating = false;
        }
        
        public InputVisualUnitModel(LadderUnitModel _core)
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
                    case LadderUnitModel.Types.LD:
                    case LadderUnitModel.Types.LDI:
                    case LadderUnitModel.Types.LDIM:
                    case LadderUnitModel.Types.LDIIM:
                    case LadderUnitModel.Types.LDP:
                    case LadderUnitModel.Types.LDF:
                        //对于以上类型只有一个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[1]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[1]);
                        break;
                    case LadderUnitModel.Types.LDWEQ:
                    case LadderUnitModel.Types.LDWNE:
                    case LadderUnitModel.Types.LDWGE:
                    case LadderUnitModel.Types.LDWLE:
                    case LadderUnitModel.Types.LDWG:
                    case LadderUnitModel.Types.LDWL:
                    case LadderUnitModel.Types.LDDEQ:
                    case LadderUnitModel.Types.LDDNE:
                    case LadderUnitModel.Types.LDDGE:
                    case LadderUnitModel.Types.LDDLE:
                    case LadderUnitModel.Types.LDDG:
                    case LadderUnitModel.Types.LDDL:
                    case LadderUnitModel.Types.LDFEQ:
                    case LadderUnitModel.Types.LDFNE:
                    case LadderUnitModel.Types.LDFGE:
                    case LadderUnitModel.Types.LDFLE:
                    case LadderUnitModel.Types.LDFG:
                    case LadderUnitModel.Types.LDFL:
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
