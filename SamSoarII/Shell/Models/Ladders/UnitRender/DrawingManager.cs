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
            context.PushOpacity(model.Opacity);
            switch (model.Core.Shape)
            {
                case Core.Models.LadderUnitModel.Shapes.Input:
                    break;
                case Core.Models.LadderUnitModel.Shapes.Output:
                    break;
                case Core.Models.LadderUnitModel.Shapes.OutputRect:
                    break;
                case Core.Models.LadderUnitModel.Shapes.Special:
                    break;
                case Core.Models.LadderUnitModel.Shapes.HLine:
                    DrawingHLine(context, model);
                    break;
                case Core.Models.LadderUnitModel.Shapes.VLine:
                    DrawingVLine(context, model);
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

        private static void DrawingHLine(DrawingContext context, BaseVisualUnitModel core)
        {
            int span = core.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            Point point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 1.0 / 3 * Global.GlobalSetting.LadderHeightUnit);
            Point point2 = new Point((core.X + 1) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 1.0 / 3 * Global.GlobalSetting.LadderHeightUnit);
            context.DrawLine(LinePen,point1,point2);
        }
        private static void DrawingVLine(DrawingContext context, BaseVisualUnitModel core)
        {
            int span = core.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            Point point1 = new Point((core.X + 1) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 1.0 / 3 * Global.GlobalSetting.LadderHeightUnit);
            Point point2 = new Point((core.X + 1) * Global.GlobalSetting.LadderWidthUnit, (core.Y + 1) * span + 1.0 / 3 * Global.GlobalSetting.LadderHeightUnit);
            context.DrawLine(LinePen, point1, point2);
        }
    }
}
