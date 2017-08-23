using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Models
{
    public class OutputRectVisualUnitModel : BaseVisualUnitModel, IResource
    {
        public override IResource Create(params object[] args)
        {
            return new OutputRectVisualUnitModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            //TODO Render
            RenderAll();
            recreating = false;
        }

        public OutputRectVisualUnitModel(LadderUnitModel _core)
        {
            if (_core != null)
            {
                Recreate(_core);
                //添加形状
                visuals.Add(VisualType.Shape, new LadderDrawingVisual[1]);
                //添加仿真及监视时的画刷(只有断点画刷)
                visuals.Add(VisualType.Brush, new LadderDrawingVisual[1]);
                switch (_core.Type)
                {
                    case LadderUnitModel.Types.HMIBLOCK:
                    case LadderUnitModel.Types.PAUSE:
                        //尚未启用
                        break;
                    case LadderUnitModel.Types.NEXT:
                    case LadderUnitModel.Types.STLE:
                    case LadderUnitModel.Types.EI:
                    case LadderUnitModel.Types.DI:
                        //对于以上类型没有元件及注释
                        break;
                    case LadderUnitModel.Types.ALT:
                    case LadderUnitModel.Types.ALTP:
                    case LadderUnitModel.Types.FOR:
                    case LadderUnitModel.Types.JMP:
                    case LadderUnitModel.Types.LBL:
                    case LadderUnitModel.Types.CALL:
                    case LadderUnitModel.Types.STL:
                    case LadderUnitModel.Types.ST:
                    case LadderUnitModel.Types.DTCH:
                    case LadderUnitModel.Types.TRD:
                    case LadderUnitModel.Types.TWR:
                    case LadderUnitModel.Types.PLSNEXT:
                    case LadderUnitModel.Types.PLSSTOP:
                        //对于以上类型只有一个元件及注释(常量没有注释，但此处依旧添加，便于拓展)
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[1]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[1]);
                        break;
                    case LadderUnitModel.Types.WTOD:
                    case LadderUnitModel.Types.DTOW:
                    case LadderUnitModel.Types.DTOF:
                    case LadderUnitModel.Types.BIN:
                    case LadderUnitModel.Types.BCD:
                    case LadderUnitModel.Types.ROUND:
                    case LadderUnitModel.Types.TRUNC:
                    case LadderUnitModel.Types.INVW:
                    case LadderUnitModel.Types.INVD:
                    case LadderUnitModel.Types.MOVD:
                    case LadderUnitModel.Types.MOV:
                    case LadderUnitModel.Types.MOVF:
                    case LadderUnitModel.Types.SQRT:
                    case LadderUnitModel.Types.SIN:
                    case LadderUnitModel.Types.COS:
                    case LadderUnitModel.Types.TAN:
                    case LadderUnitModel.Types.LN:
                    case LadderUnitModel.Types.EXP:
                    case LadderUnitModel.Types.INC:
                    case LadderUnitModel.Types.INCD:
                    case LadderUnitModel.Types.DEC:
                    case LadderUnitModel.Types.DECD:
                    case LadderUnitModel.Types.TON:
                    case LadderUnitModel.Types.TOF:
                    case LadderUnitModel.Types.TONR:
                    case LadderUnitModel.Types.CTU:
                    case LadderUnitModel.Types.CTD:
                    case LadderUnitModel.Types.CTUD:
                    case LadderUnitModel.Types.ATCH:
                    case LadderUnitModel.Types.PLSF:
                    case LadderUnitModel.Types.DPLSF:
                    case LadderUnitModel.Types.BLOCK:
                    case LadderUnitModel.Types.HCNT:
                    case LadderUnitModel.Types.LOG:
                    case LadderUnitModel.Types.FACT:
                    case LadderUnitModel.Types.NEG:
                    case LadderUnitModel.Types.NEGD:
                    case LadderUnitModel.Types.XCH:
                    case LadderUnitModel.Types.XCHD:
                    case LadderUnitModel.Types.XCHF:
                    case LadderUnitModel.Types.CML:
                    case LadderUnitModel.Types.CMLD:
                        //对于以上类型有两个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[2]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[2]);
                        break;
                    case LadderUnitModel.Types.ANDW:
                    case LadderUnitModel.Types.ANDD:
                    case LadderUnitModel.Types.ORW:
                    case LadderUnitModel.Types.ORD:
                    case LadderUnitModel.Types.XORW:
                    case LadderUnitModel.Types.XORD:
                    case LadderUnitModel.Types.MVBLK:
                    case LadderUnitModel.Types.MVDBLK:
                    case LadderUnitModel.Types.ADDF:
                    case LadderUnitModel.Types.SUBF:
                    case LadderUnitModel.Types.MULF:
                    case LadderUnitModel.Types.DIVF:
                    case LadderUnitModel.Types.ADD:
                    case LadderUnitModel.Types.ADDD:
                    case LadderUnitModel.Types.SUB:
                    case LadderUnitModel.Types.SUBD:
                    case LadderUnitModel.Types.MUL:
                    case LadderUnitModel.Types.MULW:
                    case LadderUnitModel.Types.MULD:
                    case LadderUnitModel.Types.DIV:
                    case LadderUnitModel.Types.DIVW:
                    case LadderUnitModel.Types.DIVD:
                    case LadderUnitModel.Types.SHL:
                    case LadderUnitModel.Types.SHLD:
                    case LadderUnitModel.Types.SHR:
                    case LadderUnitModel.Types.SHRD:
                    case LadderUnitModel.Types.ROL:
                    case LadderUnitModel.Types.ROLD:
                    case LadderUnitModel.Types.ROR:
                    case LadderUnitModel.Types.RORD:
                    case LadderUnitModel.Types.MBUS:
                    case LadderUnitModel.Types.SEND:
                    case LadderUnitModel.Types.REV:
                    case LadderUnitModel.Types.PWM:
                    case LadderUnitModel.Types.DPWM:
                    case LadderUnitModel.Types.PLSY:
                    case LadderUnitModel.Types.DPLSY:
                    case LadderUnitModel.Types.PTO:
                    case LadderUnitModel.Types.TBL:
                    case LadderUnitModel.Types.POLYLINEF:
                    case LadderUnitModel.Types.POLYLINEI:
                    case LadderUnitModel.Types.LINEF:
                    case LadderUnitModel.Types.LINEI:
                    case LadderUnitModel.Types.ARCF:
                    case LadderUnitModel.Types.ARCI:
                    case LadderUnitModel.Types.POW:
                    case LadderUnitModel.Types.CMP:
                    case LadderUnitModel.Types.CMPD:
                    case LadderUnitModel.Types.CMPF:
                    case LadderUnitModel.Types.FMOV:
                    case LadderUnitModel.Types.FMOVD:
                        //对于以上类型有三个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[2]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[2]);
                        break;
                    case LadderUnitModel.Types.SHLB:
                    case LadderUnitModel.Types.SHRB:
                    case LadderUnitModel.Types.PLSR:
                    case LadderUnitModel.Types.DPLSR:
                    case LadderUnitModel.Types.ZRN:
                    case LadderUnitModel.Types.DZRN:
                    case LadderUnitModel.Types.DRVI:
                    case LadderUnitModel.Types.DDRVI:
                    case LadderUnitModel.Types.DRVA:
                    case LadderUnitModel.Types.DDRVA:
                    case LadderUnitModel.Types.EHCNT:
                    case LadderUnitModel.Types.ZCP:
                    case LadderUnitModel.Types.ZCPD:
                    case LadderUnitModel.Types.ZCPF:
                        //对于以上类型有四个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[4]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[4]);
                        break;
                    case LadderUnitModel.Types.CALLM:
                    case LadderUnitModel.Types.PLSRD:
                    case LadderUnitModel.Types.DPLSRD:
                    case LadderUnitModel.Types.PLSA:
                    case LadderUnitModel.Types.DPLSA:
                    case LadderUnitModel.Types.DZRND:
                    case LadderUnitModel.Types.SMOV:
                        //对于以上类型有五个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[5]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[5]);
                        break;
                    case LadderUnitModel.Types.PID:
                        //对于以上类型有六个元件及注释
                        visuals.Add(VisualType.Comment, new LadderDrawingVisual[6]);
                        visuals.Add(VisualType.Property, new LadderDrawingVisual[6]);
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
