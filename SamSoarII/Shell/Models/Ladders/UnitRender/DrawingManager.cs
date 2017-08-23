using SamSoarII.Core.Models;
using SamSoarII.Shell.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public enum FontType
    {
        Comment,
        Property,
        Normal
    }
    public class DrawingManager
    {
        public static FontFamily CourierNew = new FontFamily("Courier New");
        public static FontFamily Arial = new FontFamily("Arial");
        public static Pen LinePen = new Pen(Brushes.Black, 4);
        public static Pen Transparent = new Pen(Brushes.Transparent, 0);

        static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE" };
        static string[] BIT_1_SHOWS = { "1", "ON", "TRUE" };

        public static void DrawingUnitShape(DrawingContext context, IViewModel core)
        {
            BaseVisualUnitModel model = core as BaseVisualUnitModel;
            int span = model.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            context.PushOpacity(model.Opacity);
            switch (model.Core.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    DrawingBaseInput(context, model, span);
                    DrawingInput(context, model, span);
                    break;
                case LadderUnitModel.Shapes.Output:
                    DrawingBaseOutput(context, model, span);
                    DrawingOutput(context, model, span);
                    break;
                case LadderUnitModel.Shapes.OutputRect:
                    DrawingOutputRect(context, model, span);
                    break;
                case LadderUnitModel.Shapes.Special:
                    DrawingHLine(context, model, span);
                    DrawingSpecial(context, model, span);
                    break;
                case LadderUnitModel.Shapes.HLine:
                    DrawingHLine(context, model, span);
                    break;
                case LadderUnitModel.Shapes.VLine:
                    DrawingVLine(context, model, span);
                    break;
            }
        }
        public static void DrawingUnitBrpoBrush(DrawingContext context, IViewModel core)
        {
            BaseVisualUnitModel model = core as BaseVisualUnitModel;
            int span = model.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            if (model.Core.LadderMode == LadderModes.Simulate)
            {
                context.PushOpacity(0.7);
                context.DrawRectangle((model.Core.BPCursor != null ? Brushes.Yellow : Brushes.Transparent), Transparent, new Rect(new Point(model.X * Global.GlobalSetting.LadderWidthUnit, model.Y * span), new Size(300, 290)));
            }
        }
        public static void DrawingUnitOnOffBrush(DrawingContext context, IViewModel core)
        {
            BaseVisualUnitModel model = core as BaseVisualUnitModel;
            int span = model.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            switch (model.Core.LadderMode)
            {
                case LadderModes.Simulate:
                case LadderModes.Monitor:
                    if ((model.Core.LadderMode == LadderModes.Monitor && model.MNGComu.IsActive)
                        || (model.Core.LadderMode == LadderModes.Simulate && model.MNGSimu.IsActive))
                    {
                        bool value = false;
                        Int32 w1 = 0, w2 = 0;
                        Int64 d1 = 0, d2 = 0;
                        double f1 = 0, f2 = 0;
                        try
                        {
                            switch (model.Core.Type)
                            {
                                case LadderUnitModel.Types.LD:
                                case LadderUnitModel.Types.LDIM:
                                    value = BIT_1_SHOWS.Contains(model.Core.Children[0].Value.ToString());
                                    break;
                                case LadderUnitModel.Types.LDI:
                                case LadderUnitModel.Types.LDIIM:
                                    value = BIT_0_SHOWS.Contains(model.Core.Children[0].Value.ToString());
                                    break;
                                case LadderUnitModel.Types.LDWEQ:
                                case LadderUnitModel.Types.LDWNE:
                                case LadderUnitModel.Types.LDWGE:
                                case LadderUnitModel.Types.LDWLE:
                                case LadderUnitModel.Types.LDWG:
                                case LadderUnitModel.Types.LDWL:
                                    w1 = short.Parse(model.Core.Children[0].Value.ToString());
                                    w2 = short.Parse(model.Core.Children[1].Value.ToString());
                                    break;
                                case LadderUnitModel.Types.LDDEQ:
                                case LadderUnitModel.Types.LDDNE:
                                case LadderUnitModel.Types.LDDGE:
                                case LadderUnitModel.Types.LDDLE:
                                case LadderUnitModel.Types.LDDG:
                                case LadderUnitModel.Types.LDDL:
                                    d1 = int.Parse(model.Core.Children[0].Value.ToString());
                                    d2 = int.Parse(model.Core.Children[1].Value.ToString());
                                    break;
                                case LadderUnitModel.Types.LDFEQ:
                                case LadderUnitModel.Types.LDFNE:
                                case LadderUnitModel.Types.LDFGE:
                                case LadderUnitModel.Types.LDFLE:
                                case LadderUnitModel.Types.LDFG:
                                case LadderUnitModel.Types.LDFL:
                                    f1 = float.Parse(model.Core.Children[0].Value.ToString());
                                    f2 = float.Parse(model.Core.Children[1].Value.ToString());
                                    break;
                            }
                            switch (model.Core.InstName)
                            {
                                case "LDWEQ": value = (w1 == w2); break;
                                case "LDWNE": value = (w1 != w2); break;
                                case "LDWLE": value = (w1 <= w2); break;
                                case "LDWGE": value = (w1 >= w2); break;
                                case "LDWL": value = (w1 < w2); break;
                                case "LDWG": value = (w1 > w2); break;
                                case "LDDEQ": value = (d1 == d2); break;
                                case "LDDNE": value = (d1 != d2); break;
                                case "LDDLE": value = (d1 <= d2); break;
                                case "LDDGE": value = (d1 >= d2); break;
                                case "LDDL": value = (d1 < d2); break;
                                case "LDDG": value = (d1 > d2); break;
                                case "LDFEQ": value = (f1 == f2); break;
                                case "LDFNE": value = (f1 != f2); break;
                                case "LDFLE": value = (f1 <= f2); break;
                                case "LDFGE": value = (f1 >= f2); break;
                                case "LDFL": value = (f1 < f2); break;
                                case "LDFG": value = (f1 > f2); break;
                            }
                        }
                        catch (Exception)
                        {
                            context.DrawRectangle(Brushes.Red, Transparent, new Rect(new Point(model.X * Global.GlobalSetting.LadderWidthUnit + 100, model.Y * span + 50), new Size(100, 100)));
                            return;
                        }
                        context.DrawRectangle(value ? Brushes.Green : Brushes.Transparent, Transparent, new Rect(new Point(model.X * Global.GlobalSetting.LadderWidthUnit + 100, model.Y * span + 50), new Size(100, 100)));
                    }
                    break;
            }
        }

        public static void DrawingUnitProperty(DrawingContext context, IViewModel core)
        {
            BaseVisualUnitModel model = core as BaseVisualUnitModel;
            int span = model.IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            context.PushOpacity(model.Opacity);
            switch (model.Core.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    DrawingInputProperty(context, model, span);
                    break;
                case LadderUnitModel.Shapes.Output:
                    DrawingOutputProperty(context, model, span);
                    break;
                case LadderUnitModel.Shapes.OutputRect:
                    DrawingOutputRectProperty(context, model, span);
                    break;
            }
        }
        public static void DrawingInputProperty(DrawingContext context, BaseVisualUnitModel core, int span)
        {
            string text1 = string.Empty, text2 = string.Empty;
            switch (core.Core.Type)
            {
                case LadderUnitModel.Types.LD:
                case LadderUnitModel.Types.LDI:
                case LadderUnitModel.Types.LDIM:
                case LadderUnitModel.Types.LDIIM:
                case LadderUnitModel.Types.LDP:
                case LadderUnitModel.Types.LDF:
                    if(core.Core.IsUsed)
                    {
                        switch (core.Core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Core.Children[0].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGSimu.IsAlive ? "???" : core.Core.Children[0].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGComu.IsAlive ? "???" : core.Core.Children[0].Value);
                                break;
                            default:
                                break;
                        }
                        DrawingText(context, core, new Point(0, 0), span, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontType.Property);
                    }
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
                    if (core.Core.IsUsed)
                    {
                        switch (core.Core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Core.Children[0].Text;
                                text2 = core.Core.Children[1].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGSimu.IsAlive ? "???" : core.Core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Core.Children[1].Text,
                                        !core.MNGSimu.IsAlive ? "???" : core.Core.Children[1].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGComu.IsAlive ? "???" : core.Core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Core.Children[1].Text,
                                        !core.MNGComu.IsAlive ? "???" : core.Core.Children[1].Value);
                                break;
                        }
                        DrawingText(context, core, new Point(0, 0), span, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontType.Property);
                        DrawingText(context, core, new Point(0, 150), span, text2, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontType.Property);
                    }
                    break;
            }
        }
        public static void DrawingOutputProperty(DrawingContext context, BaseVisualUnitModel core, int span)
        {
            string text1 = string.Empty, text2 = string.Empty;
            switch (core.Core.Type)
            {
                case LadderUnitModel.Types.OUT:
                case LadderUnitModel.Types.OUTIM:
                    if (core.Core.IsUsed)
                    {
                        switch (core.Core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Core.Children[0].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGSimu.IsAlive ? "???" : core.Core.Children[0].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGComu.IsAlive ? "???" : core.Core.Children[0].Value);
                                break;
                        }
                        DrawingText(context, core, new Point(0, 0), span, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontType.Property);
                    }
                    break;
                case LadderUnitModel.Types.SET:
                case LadderUnitModel.Types.SETIM:
                case LadderUnitModel.Types.RST:
                case LadderUnitModel.Types.RSTIM:
                    if (core.Core.IsUsed)
                    {
                        switch (core.Core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Core.Children[0].Text;
                                text2 = core.Core.Children[1].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGSimu.IsAlive ? "???" : core.Core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Core.Children[1].Text,
                                        !core.MNGSimu.IsAlive ? "???" : core.Core.Children[1].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Core.Children[0].Text,
                                        !core.MNGComu.IsAlive ? "???" : core.Core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Core.Children[1].Text,
                                        !core.MNGComu.IsAlive ? "???" : core.Core.Children[1].Value);
                                break;
                        }
                        DrawingText(context, core, new Point(0, 0), span, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontType.Property);
                        DrawingText(context, core, new Point(0, 150), span, text2, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontType.Property);
                    }
                    break;
            }
            switch (core.Core.LadderMode)
            {
                case LadderModes.Simulate:
                case LadderModes.Monitor:
                    if ((core.Core.LadderMode == LadderModes.Monitor && core.MNGComu.IsActive) 
                        || (core.Core.LadderMode == LadderModes.Simulate && core.MNGSimu.IsActive))
                    {
                        bool value = BIT_1_SHOWS.Contains(core.Core.Children[0].Value.ToString());
                        context.DrawRectangle(value ? Brushes.Green : Brushes.Transparent, Transparent, new Rect(new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 100, core.Y * span + 50), new Size(100, 100)));
                    }
                    break;
            }
        }
        public static void DrawingOutputRectProperty(DrawingContext context, BaseVisualUnitModel core, int span)
        {
            switch (core.Core.Type)
            {
                case LadderUnitModel.Types.TON:
                case LadderUnitModel.Types.TONR:
                case LadderUnitModel.Types.TOF:
                    if (core.Core.Children[0].Base == ValueModel.Bases.TV)
                    {
                        int offset = core.Core.Children[0].Offset;
                        if (offset >= 0 && offset < 200)
                            DrawingText(context, core, new Point(25, 250), span, "100 ms", Arial, FontWeights.Normal, 25);
                        else if (offset >= 200 && offset < 250)
                            DrawingText(context, core, new Point(25, 250), span, "10 ms", Arial, FontWeights.Normal, 25);
                        else
                            DrawingText(context, core, new Point(25, 250), span, "1 ms", Arial, FontWeights.Normal, 25);
                    }
                    break;
            }
            if (core.Core.IsUsed)
            {
                ValueModel vmodel;
                ValueFormat vformat;
                switch (core.Core.LadderMode)
                {
                    case LadderModes.Edit:
                        for (int i = 0; i < core.Core.Children.Count; i++)
                        {
                            vmodel = core.Core.Children[i];
                            vformat = vmodel.Format;
                            string text = string.Format("{0:s}:{1:s}",
                                vformat.Name, vmodel.Text);
                            if (vformat.Position >= 0)
                                DrawingText(context, core, new Point(25, 120 + 30 * vformat.Position - (core.Core.Type == LadderUnitModel.Types.PID ? 20 : 0)), span, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontType.Property);
                            else
                                DrawingText(context, core, new Point(25, 250 + 30 * (vformat.Position + 1)), span, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontType.Property);
                        }
                        break;
                    case LadderModes.Simulate:
                        for (int i = 0; i < core.Core.Children.Count; i++)
                        {
                            vmodel = core.Core.Children[i];
                            vformat = vmodel.Format;
                            string text = string.Format("{0:s} = {1}",
                                vmodel.Text, !core.MNGSimu.IsActive ? "???" : vmodel.Value);
                            if (vformat.Position >= 0)
                                DrawingText(context, core, new Point(25, 120 + 30 * vformat.Position - (core.Core.Type == LadderUnitModel.Types.PID ? 20 : 0)), span, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontType.Property);
                            else
                                DrawingText(context, core, new Point(25, 250 + 30 * (vformat.Position + 1)), span, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontType.Property);
                        }
                        break;
                    case LadderModes.Monitor:
                        for (int i = 0; i < core.Core.Children.Count; i++)
                        {
                            vmodel = core.Core.Children[i];
                            vformat = vmodel.Format;
                            string text = string.Format("{0:s} = {1}",
                                vmodel.Text, !core.MNGComu.IsActive ? "???" : vmodel.Value);
                            if (vformat.Position >= 0)
                                DrawingText(context, core, new Point(25, 120 + 30 * vformat.Position - (core.Core.Type == LadderUnitModel.Types.PID ? 20 : 0)), span, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontType.Property);
                            else
                                DrawingText(context, core, new Point(25, 250 + 30 * (vformat.Position + 1)), span, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontType.Property);
                        }
                        break;
                }
            }
        }

        public static void DrawingUnitCommnet(DrawingContext context, IViewModel core)
        {
            BaseVisualUnitModel model = core as BaseVisualUnitModel;
            int span = Global.GlobalSetting.LadderCommentModeHeightUnit;
            if (model.IsCommentMode)
            {
                string comment = string.Empty;
                int id = 0;
                for (int i = 0; i < model.Core.Children.Count; i++)
                {
                    ValueModel vmodel = model.Core.Children[i];
                    if (vmodel.Base != ValueModel.Bases.K && vmodel.Base != ValueModel.Bases.H)
                    {
                        comment = string.Format("{0:s}:{1:s}",
                                        vmodel.Text, vmodel.Comment);
                        DrawingText(context, model, new Point(10, 300 + (FontManager.GetComment().FontSize + 4) * id), span, comment, null, FontWeights.Normal, 0, 0, TextAlignment.Left, FontType.Comment);
                        id++;
                    }
                }
            }
        }
        public static void DrawingBaseInput(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            Point point1;
            Point point2;
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 100);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 100, core.Y * span + 100);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 200, core.Y * span + 100);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 300, core.Y * span + 100);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 100, core.Y * span + 50);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 100, core.Y * span + 150);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 200, core.Y * span + 50);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 200, core.Y * span + 150);
            context.DrawLine(LinePen, point1, point2);
        }
        public static void DrawingInput(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            switch (core.Core.InstName)
            {
                case "LDWEQ":
                    DrawingText(context,core,new Point(100,70),span,"W==", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDWNE":
                    DrawingText(context, core, new Point(100, 70), span, "W<>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDWGE":
                    DrawingText(context, core, new Point(100, 70), span, "W>=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDWLE":
                    DrawingText(context, core, new Point(100, 70), span, "W<=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDWG":
                    DrawingText(context, core, new Point(100, 70), span, "W>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDWL":
                    DrawingText(context, core, new Point(100, 70), span, "W<", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDDEQ":
                    DrawingText(context, core, new Point(100, 70), span, "D==", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDDNE":
                    DrawingText(context, core, new Point(100, 70), span, "D<>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDDGE":
                    DrawingText(context, core, new Point(100, 70), span, "D>=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDDLE":
                    DrawingText(context, core, new Point(100, 70), span, "D<=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDDG":
                    DrawingText(context, core, new Point(100, 70), span, "D>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDDL":
                    DrawingText(context, core, new Point(100, 70), span, "D<", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDFEQ":
                    DrawingText(context, core, new Point(100, 70), span, "F==", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDFNE":
                    DrawingText(context, core, new Point(100, 70), span, "F<>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDFGE":
                    DrawingText(context, core, new Point(100, 70), span, "F>=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDFLE":
                    DrawingText(context, core, new Point(100, 70), span, "F<=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case "LDFG":
                    DrawingText(context, core, new Point(100, 70), span, "F>", CourierNew, FontWeights.Heavy, 36,100, TextAlignment.Center);
                    break;
                case "LDFL":
                    DrawingText(context, core, new Point(100, 70), span, "F<", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
            }
            Point point1;
            Point point2;
            switch (core.Core.InstName)
            {
                case "LDI":
                    point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 50);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 150);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case "LDIM":
                    point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 50);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 150);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case "LDIIM":
                    point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 50);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 150);
                    context.DrawLine(LinePen, point1, point2);
                    point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 50);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 150);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case "LDP":
                    DrawingArrow(context, core, span, true);
                    break;
                case "LDF":
                    DrawingArrow(context, core, span, false);
                    break;
                default:
                    break;
            }
        }
        public static void DrawingBaseOutput(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            Point point1;
            Point point2;
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 100);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 72, core.Y * span + 100);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 228, core.Y * span + 100);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 300, core.Y * span + 100);
            context.DrawLine(LinePen, point1, point2);
            
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(GenerateBracket(true, core, span));
            geometry.Figures.Add(GenerateBracket(false, core, span));

            context.DrawGeometry(Brushes.Black, LinePen, geometry);
        }
        public static void DrawingOutput(DrawingContext context, BaseVisualUnitModel core, int span)
        {
            switch (core.Core.InstName)
            {
                case "OUTIM":
                    DrawingText(context, core, new Point(100, 70), span, "I", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case "RST":
                    DrawingText(context, core, new Point(100, 70), span, "R", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case "RSTIM":
                    DrawingText(context, core, new Point(100, 70), span, "RI", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case "SET":
                    DrawingText(context, core, new Point(100, 70), span, "S", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case "SETIM":
                    DrawingText(context, core, new Point(100, 70), span, "SI", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
            }
        }
        public static void DrawingOutputRect(DrawingContext context, BaseVisualUnitModel core, int span)
        {
            Point point1;
            Point point2;
            point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 100);
            point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 20, core.Y * span + 100);
            context.DrawLine(LinePen, point1, point2);
            context.DrawRectangle(Brushes.Transparent,LinePen,new Rect(new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 20, core.Y * span + 14.5),new Size(274,274)));

            if(core.Core.Type == LadderUnitModel.Types.PID)
                DrawingText(context, core, new Point(35, 65), span, "EN", Arial, FontWeights.Normal, 25);
            else DrawingText(context, core, new Point(35, 85), span, "EN", Arial, FontWeights.Normal, 25);

            DrawingText(context, core, new Point(25, 18), span, core.Core.InstName, Arial, FontWeights.Normal, 30);
        }
        public static void DrawingSpecial(DrawingContext context, BaseVisualUnitModel core,int span)
        {
            Point point1;
            Point point2;
            switch (core.Core.Type)
            {
                case LadderUnitModel.Types.MEP:
                    DrawingArrow(context, core, span, true);
                    break;
                case LadderUnitModel.Types.MEF:
                    DrawingArrow(context, core, span, false);
                    break;
                case LadderUnitModel.Types.INV:
                    point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 75);
                    point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 125);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                default:
                    break;
            }
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

        private static void DrawingText(DrawingContext context, BaseVisualUnitModel core, Point relativePoint , int span, string text,FontFamily fontFamily, FontWeight fontWeight , double emSize,double containerWidth = 100, TextAlignment alignment = TextAlignment.Left, FontType type = FontType.Normal)
        {
            Typeface face;
            FontData data;
            FormattedText formattedText;
            switch (type)
            {
                case FontType.Comment:
                    data = FontManager.GetComment();
                    face = new Typeface(data.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                    formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, data.FontSize, data.FontColor);
                    formattedText.MaxTextWidth = 290;
                    formattedText.MaxLineCount = 1;
                    formattedText.Trimming = TextTrimming.CharacterEllipsis;
                    break;
                case FontType.Property:
                    data = FontManager.GetLadder();
                    face = new Typeface(data.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                    formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, data.FontSize, data.FontColor);
                    break;
                case FontType.Normal:
                default:
                    face = new Typeface(fontFamily, FontStyles.Normal, fontWeight, FontStretches.Normal);
                    formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, emSize, Brushes.Black);
                    break;
            }
            double x, y;
            x = core.X * Global.GlobalSetting.LadderWidthUnit + relativePoint.X;
            y = core.Y * span + relativePoint.Y;
            if(alignment == TextAlignment.Center)
                x += (containerWidth - formattedText.Width) / 2;
            context.DrawText(formattedText, new Point(x,y));
        }

        private static PathFigure GenerateBracket(bool isLeft, BaseVisualUnitModel core,int span)
        {
            Point start;
            Point end;
            start = isLeft ? new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 100, core.Y * span + 50) : new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 200, core.Y * span + 50);
            end = isLeft ? new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 100, core.Y * span + 150) : new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 200, core.Y * span + 150);
            PathFigure figure = new PathFigure();
            figure.StartPoint = start;
            figure.IsClosed = false;
            figure.IsFilled = false;
            figure.Segments.Add(isLeft ? new ArcSegment(end, new Size(60, 60), 0, false, SweepDirection.Counterclockwise, true) : new ArcSegment(end, new Size(60, 60), 0, false, SweepDirection.Clockwise, true));
            return figure;
        }

        private static void DrawingArrow(DrawingContext context, BaseVisualUnitModel core, int span, bool isUp)
        {
            Point point1;
            Point point2;
            point1 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 50);
            point2 = new Point((core.X + 0.5) * Global.GlobalSetting.LadderWidthUnit, core.Y * span + 150);
            context.DrawLine(LinePen, point1, point2);
            if (isUp)
            {
                point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 50);
                point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 75);
                context.DrawLine(LinePen, point1, point2);
                point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 50);
                point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 75);
                context.DrawLine(LinePen, point1, point2);
            }
            else
            {
                point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 150);
                point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 125, core.Y * span + 125);
                context.DrawLine(LinePen, point1, point2);
                point1 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 150, core.Y * span + 150);
                point2 = new Point(core.X * Global.GlobalSetting.LadderWidthUnit + 175, core.Y * span + 125);
                context.DrawLine(LinePen, point1, point2);
            }
        }
    }
}
