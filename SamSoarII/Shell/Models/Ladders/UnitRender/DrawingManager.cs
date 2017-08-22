using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public class DrawingManager
    {
        public static Pen LinePen = new Pen(Brushes.Black,4);
        public static void DrawingUnit(DrawingContext context, IViewModel core)
        {
            BaseVisualUnitModel model = core as BaseVisualUnitModel;
            int span = model.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            context.PushOpacity(model.Opacity);
            switch (model.Core.Shape)
            {
                case Core.Models.LadderUnitModel.Shapes.Input:
                    DrawingBaseInput(context, model);
                    DrawingInput(context, model);
                    break;
                case Core.Models.LadderUnitModel.Shapes.Output:
                    DrawingBaseOutput(context, model);
                    break;
                case Core.Models.LadderUnitModel.Shapes.OutputRect:
                    DrawingBaseOutputRect(context, model);
                    break;
                case Core.Models.LadderUnitModel.Shapes.Special:
                    DrawingHLine(context, model, span);
                    DrawingSpecial(context, model, span);
                    break;
                case Core.Models.LadderUnitModel.Shapes.HLine:
                    DrawingHLine(context, model, span);
                    break;
                case Core.Models.LadderUnitModel.Shapes.VLine:
                    DrawingVLine(context, model, span);
                    break;
            }
        }
        public static void DrawingBaseInput(DrawingContext context, BaseVisualUnitModel core)
        {

        }
        public static void DrawingInput(DrawingContext context, BaseVisualUnitModel core)
        {

        }
        public static void DrawingBaseOutput(DrawingContext context, BaseVisualUnitModel core)
        {

        }
        public static void DrawingBaseOutputRect(DrawingContext context, BaseVisualUnitModel core)
        {

        }
        public static void DrawingSpecial(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            Point point1;
            Point point2;
            switch (core.Core.Type)
            {
                case Core.Models.LadderUnitModel.Types.MEP:
                    point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 50);
                    point2 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 150);
                    context.DrawLine(LinePen, point1, point2);
                    point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 50);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 75);
                    context.DrawLine(LinePen, point1, point2);
                    point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 50);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 75);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case Core.Models.LadderUnitModel.Types.MEF:
                    point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 50);
                    point2 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 150);
                    context.DrawLine(LinePen, point1, point2);
                    point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 150);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 125);
                    context.DrawLine(LinePen, point1, point2);
                    point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 150);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 125);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case Core.Models.LadderUnitModel.Types.INV:
                    point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 75);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 125);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                default:
                    break;
            }
        }
        public static void DrawingUnitProperty(DrawingContext context, IViewModel core)
        {

        }

        public static void DrawingUnitCommnet(DrawingContext context, IViewModel core)
        {

        }

        public static void DrawingBrop(DrawingContext context, IViewModel core)
        {

        }

        private static void DrawingHLine(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            Point point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit, core.Y * span + Global.GlobalSetting.LadderHeightUnit / 3);
            Point point2 = new Point((core.X + 1) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + Global.GlobalSetting.LadderHeightUnit / 3);
            context.DrawLine(LinePen,point1,point2);
        }
        private static void DrawingVLine(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            Point point1 = new Point((core.X + 1) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + Global.GlobalSetting.LadderHeightUnit / 3);
            Point point2 = new Point((core.X + 1) * Global.GlobalSetting.LadderWidthUnit, (core.Y + 1) * span + Global.GlobalSetting.LadderHeightUnit / 3);
            context.DrawLine(LinePen, point1, point2);
        }
    }
}
