using SamSoarII.Core.Communication;
using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
using SamSoarII.Global;
using SamSoarII.Shell.Managers;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SamSoarII.Shell.Models
{
    public class LadderUnitViewModel : DrawingVisual, IViewModel, IResource
    {
        #region IResource

        private int resourceid;
        public int ResourceID
        {
            get { return this.resourceid; }
            set { this.resourceid = value; }
        }

        public IResource Create(params object[] args)
        {
            return new LadderUnitViewModel((LadderUnitModel)args[0]);    
        }
        
        public virtual void Recreate(params object[] args)
        {
            Core = (LadderUnitModel)args[0];
            oldladdermode = Core.LadderMode;
            if (LadderMode != LadderModes.Edit)
            {
                if (LadderMode == LadderModes.Simulate)
                {
                    MNGSimu.Started += OnSimulateStarted;
                    MNGSimu.Aborted += OnSimulateAborted;
                }
                if (LadderMode == LadderModes.Monitor)
                {
                    MNGComu.Started += OnMonitorStarted;
                    MNGComu.Aborted += OnMonitorAborted;
                }
                foreach (ValueModel vmodel in Core.Children)
                    if (vmodel.Store != null)
                        vmodel.Store.PropertyChanged += OnValueStorePropertyChanged;
            }
        }
        
        #endregion

        public LadderUnitViewModel(LadderUnitModel _core)
        {
            Core = _core;
        }

        public virtual void Dispose()
        {
            if (LadderMode != LadderModes.Edit)
            {
                if (LadderMode == LadderModes.Simulate)
                {
                    MNGSimu.Started -= OnSimulateStarted;
                    MNGSimu.Aborted -= OnSimulateAborted;
                }
                if (LadderMode == LadderModes.Monitor)
                {
                    MNGComu.Started -= OnMonitorStarted;
                    MNGComu.Aborted -= OnMonitorAborted;
                }
                foreach (ValueModel vmodel in Core.Children)
                    if (vmodel.Store != null)
                        vmodel.Store.PropertyChanged -= OnValueStorePropertyChanged;
            }
            if (brpoview != null)
            {
                brpoview.Dispose();
                brpoview = null;
            }
            Core = null;
            AllResourceManager.Dispose(this);
        }

        static public LadderUnitViewModel Create(LadderUnitModel _core)
        {
            return AllResourceManager.CreateUnit(_core);
        }
        
        #region Core

        private LadderUnitModel core;
        public virtual LadderUnitModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                LadderUnitModel _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ViewPropertyChanged -= OnCorePropertyChanged;
                    _core.Changed -= OnCoreChanged;
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.ViewPropertyChanged += OnCorePropertyChanged;
                    core.Changed += OnCoreChanged;
                    if (core.View != this) core.View = this;
                }
                Visibility = core != null;
            }
        }
        public SimulateManager MNGSimu { get { return Core.IFParent.MNGSimu; } }
        public CommunicationManager MNGComu { get { return Core.IFParent.MNGComu; } }
        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (LadderUnitModel)value; }
        }
        protected virtual void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "X":
                case "Y":
                case "IsUsed":
                case "IsCommentMode": Update(); break;
                case "LadderMode":
                    if (oldladdermode != LadderModes.Edit)
                    {
                        if (oldladdermode == LadderModes.Simulate)
                        {
                            MNGSimu.Started -= OnSimulateStarted;
                            MNGSimu.Aborted -= OnSimulateAborted;
                        }
                        if (oldladdermode == LadderModes.Monitor)
                        {
                            MNGComu.Started -= OnMonitorStarted;
                            MNGComu.Aborted -= OnMonitorAborted;
                        }
                        foreach (ValueModel vmodel in Core.Children)
                            if (vmodel.Store != null)
                                vmodel.Store.PropertyChanged -= OnValueStorePropertyChanged;
                    }
                    if (LadderMode != LadderModes.Edit)
                    {
                        if (LadderMode == LadderModes.Simulate)
                        {
                            MNGSimu.Started += OnSimulateStarted;
                            MNGSimu.Aborted += OnSimulateAborted;
                        }
                        if (LadderMode == LadderModes.Monitor)
                        {
                            MNGComu.Started += OnMonitorStarted;
                            MNGComu.Aborted += OnMonitorAborted;
                        }
                        foreach (ValueModel vmodel in Core.Children)
                            if (vmodel.Store != null)
                                vmodel.Store.PropertyChanged += OnValueStorePropertyChanged;
                    }
                    Update();
                    oldladdermode = LadderMode;
                    break;
                case "BPEnable": case "BPCursor":
                    Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate () { Update(); });
                    break;
            }
        }
        private void OnCoreChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            if (Core != sender) return;
            switch (e.Action)
            {
                case LadderUnitAction.ADD:
                    //if (ViewParent != null)
                    //    ViewParent.LadderCanvas.Children.Add(this);
                    //Update();
                    break;
                case LadderUnitAction.REMOVE:
                    //if (ViewParent != null)
                    //    ViewParent.LadderCanvas.Children.Remove(this);
                    break;
                case LadderUnitAction.MOVE:
                    //Update(UPDATE_TOP | UPDATE_LEFT);
                    break;
                case LadderUnitAction.UPDATE:
                    Update();
                    break;
            }
        }

        public int X { get { return core.X; } }

        public int Y { get { return core.Y; } }
        
        #endregion

        #region Shell
        
        public LadderNetworkViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }
        
        private LadderCanvas cvparent;
        public LadderCanvas CVParent
        {
            get
            {
                return this.cvparent;
            }
            set
            {
                if (cvparent == value) return;
                if (cvparent != null) cvparent.Remove(this);
                this.cvparent = value;
                if (cvparent != null) cvparent.Add(this);
            }
        }

        private LadderModes oldladdermode;
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }

        public int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        public int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }
        public double ActualX { get { return core.X * WidthUnit; } }
        public double ActualY { get { return core.Y * HeightUnit + (core.Parent != null ? core.Parent.UnitBaseTop : 0); } }

        private bool visibility;
        public bool Visibility
        {
            get { return this.visibility; }
            set { this.visibility = value; Update(); }
        }

        private LadderBrpoViewModel brpoview;
        public LadderBrpoViewModel BrpoView { get { return this.brpoview; } }

        #region Update

        public void Update()
        {
            using (DrawingContext context = RenderOpen())
            {
                if (!visibility) return;
                context.PushOpacity(core.IsUsed ? 1.0 : 0.3);
                UpdateBreakpoint(context);
                UpdateFramework(context);
                UpdatePropertyText(context);
                UpdateCommentText(context);
            }
        }
        private void UpdateFramework(DrawingContext context)
        {
            switch (Core.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    DrawingBaseInput(context);
                    DrawingInput(context);
                    break;
                case LadderUnitModel.Shapes.Output:
                    DrawingBaseOutput(context);
                    DrawingOutput(context);
                    break;
                case LadderUnitModel.Shapes.OutputRect:
                    DrawingOutputRect(context);
                    break;
                case LadderUnitModel.Shapes.Special:
                    DrawingHLine(context);
                    DrawingSpecial(context);
                    break;
                case LadderUnitModel.Shapes.HLine:
                    DrawingHLine(context);
                    break;
                case LadderUnitModel.Shapes.VLine:
                    DrawingVLine(context);
                    break;
            }
        }

        private void UpdatePropertyText(DrawingContext context)
        {
            switch (Core.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    DrawingInputProperty(context);
                    break;
                case LadderUnitModel.Shapes.Output:
                    DrawingOutputProperty(context);
                    break;
                case LadderUnitModel.Shapes.OutputRect:
                    DrawingOutputRectProperty(context);
                    break;
            }
        }

        private void UpdateCommentText(DrawingContext context)
        {
            if (IsCommentMode)
            {
                string comment = string.Empty;
                int id = 0;
                for (int i = 0; i < Core.Children.Count; i++)
                {
                    ValueModel vmodel = Core.Children[i];
                    if (vmodel.Base != ValueModel.Bases.K && vmodel.Base != ValueModel.Bases.H)
                    {
                        comment = string.Format("{0:s}:{1:s}", vmodel.Text, vmodel.Comment);
                        DrawingText(context, new Point(10, 300 + (FontManager.GetComment().FontSize + 4) * id), HeightUnit, comment, null, FontWeights.Normal, 0, 0, TextAlignment.Left, FontTypes.Comment);
                        id++;
                    }
                }
            }
        }

        private void UpdateBreakpoint(DrawingContext context)
        {
            if ((LadderMode == LadderModes.Simulate && core.BPEnable) && brpoview == null)
            {
                brpoview = AllResourceManager.CreateBrpo(core.Breakpoint);
                if (brpoview.Parent != ViewParent.ViewParent.MainCanvas)
                {
                    if (brpoview.Parent is Canvas)
                        ((Canvas)(brpoview.Parent)).Children.Remove(brpoview);
                    ViewParent.ViewParent.MainCanvas.Children.Add(brpoview);
                }
            }
            if (!(LadderMode == LadderModes.Simulate && core.BPEnable) && brpoview != null)
            {
                brpoview.Dispose();
                brpoview = null;
            }
            if (brpoview != null)
            {
                Canvas.SetLeft(brpoview, ActualX);
                Canvas.SetTop(brpoview, ActualY);
            }
            if (core.BPCursor != null)
                context.DrawRectangle(Brushes.Yellow, null, new Rect(ActualX, ActualY, WidthUnit, HeightUnit));
        }
        
        #endregion

        #region Drawing

        private static FontFamily CourierNew = new FontFamily("Courier New");
        private static FontFamily Arial = new FontFamily("Arial");
        private static Pen LinePen = new Pen(Brushes.Black, 4);
        private static Pen Transparent = new Pen(Brushes.Transparent, 0);
        private static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE" };
        private static string[] BIT_1_SHOWS = { "1", "ON", "TRUE" };

        #region Framework

        public void DrawingBaseInput(DrawingContext context)
        {
            Point point1;
            Point point2;
            point1 = new Point(ActualX, ActualY + 100);
            point2 = new Point(ActualX + 100, ActualY + 100);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(ActualX + 200, ActualY + 100);
            point2 = new Point(ActualX + 300, ActualY + 100);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(ActualX + 100, ActualY + 50);
            point2 = new Point(ActualX + 100, ActualY + 150);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(ActualX + 200, ActualY + 50);
            point2 = new Point(ActualX + 200, ActualY + 150);
            context.DrawLine(LinePen, point1, point2);
        }
        public void DrawingInput(DrawingContext context)
        {
            switch (Core.Type)
            {
                case LadderUnitModel.Types.LDWEQ:
                    DrawingText(context, new Point(100, 70), HeightUnit, "W==", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDWNE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "W<>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDWGE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "W>=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDWLE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "W<=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDWG:
                    DrawingText(context, new Point(100, 70), HeightUnit, "W>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDWL:
                    DrawingText(context, new Point(100, 70), HeightUnit, "W<", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDDEQ:
                    DrawingText(context, new Point(100, 70), HeightUnit, "D==", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDDNE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "D<>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDDGE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "D>=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDDLE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "D<=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDDG:
                    DrawingText(context, new Point(100, 70), HeightUnit, "D>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDDL:
                    DrawingText(context, new Point(100, 70), HeightUnit, "D<", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDFEQ:
                    DrawingText(context, new Point(100, 70), HeightUnit, "F==", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDFNE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "F<>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDFGE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "F>=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDFLE:
                    DrawingText(context, new Point(100, 70), HeightUnit, "F<=", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDFG:
                    DrawingText(context, new Point(100, 70), HeightUnit, "F>", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.LDFL:
                    DrawingText(context, new Point(100, 70), HeightUnit, "F<", CourierNew, FontWeights.Heavy, 36, 100, TextAlignment.Center);
                    break;
            }
            Point point1;
            Point point2;
            switch (core.Type)
            {
                case LadderUnitModel.Types.LDI:
                    point1 = new Point(ActualX + 175, ActualY + 50);
                    point2 = new Point(ActualX + 125, ActualY + 150);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case LadderUnitModel.Types.LDIM:
                    point1 = new Point(ActualX + 150, ActualY + 50);
                    point2 = new Point(ActualX + 150, ActualY + 150);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case LadderUnitModel.Types.LDIIM:
                    point1 = new Point(ActualX + 150, ActualY + 50);
                    point2 = new Point(ActualX + 150, ActualY + 150);
                    context.DrawLine(LinePen, point1, point2);
                    point1 = new Point(ActualX + 175, ActualY + 50);
                    point2 = new Point(ActualX + 125, ActualY + 150);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                case LadderUnitModel.Types.LDP:
                    DrawingArrow(context, true);
                    break;
                case LadderUnitModel.Types.LDF:
                    DrawingArrow(context, false);
                    break;
                default:
                    break;
            }
        }
        public void DrawingBaseOutput(DrawingContext context)
        {
            Point point1;
            Point point2;
            point1 = new Point(ActualX, ActualY + 100);
            point2 = new Point(ActualX + 72, ActualY + 100);
            context.DrawLine(LinePen, point1, point2);
            point1 = new Point(ActualX + 228, ActualY + 100);
            point2 = new Point(ActualX + 300, ActualY + 100);
            context.DrawLine(LinePen, point1, point2);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(GenerateBracket(true));
            geometry.Figures.Add(GenerateBracket(false));

            context.DrawGeometry(Brushes.Black, LinePen, geometry);
        }
        public void DrawingOutput(DrawingContext context)
        {
            switch (core.Type)
            {
                case LadderUnitModel.Types.OUTIM:
                    DrawingText(context, new Point(100, 70), HeightUnit, "I", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.RST:
                    DrawingText(context, new Point(100, 70), HeightUnit, "R", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.RSTIM:
                    DrawingText(context, new Point(100, 70), HeightUnit, "RI", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.SET:
                    DrawingText(context, new Point(100, 70), HeightUnit, "S", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
                case LadderUnitModel.Types.SETIM:
                    DrawingText(context, new Point(100, 70), HeightUnit, "SI", CourierNew, FontWeights.Heavy, 48, 100, TextAlignment.Center);
                    break;
            }
        }
        public void DrawingOutputRect(DrawingContext context)
        {
            Point point1;
            Point point2;
            point1 = new Point(ActualX, ActualY + 100);
            point2 = new Point(ActualX + 20, ActualY + 100);
            context.DrawLine(LinePen, point1, point2);
            context.DrawRectangle(Brushes.Transparent, LinePen, new Rect(new Point(ActualX + 20, ActualY + 14.5), new Size(274, 274)));

            if (core.Type == LadderUnitModel.Types.PID)
                DrawingText(context, new Point(35, 65), HeightUnit, "EN", Arial, FontWeights.Normal, 25);
            else
                DrawingText(context, new Point(35, 85), HeightUnit, "EN", Arial, FontWeights.Normal, 25);

            DrawingText(context, new Point(25, 18), HeightUnit, core.InstName, Arial, FontWeights.Normal, 30);
        }
        public void DrawingSpecial(DrawingContext context)
        {
            Point point1;
            Point point2;
            switch (core.Type)
            {
                case LadderUnitModel.Types.MEP:
                    DrawingArrow(context, true);
                    break;
                case LadderUnitModel.Types.MEF:
                    DrawingArrow(context, false);
                    break;
                case LadderUnitModel.Types.INV:
                    point1 = new Point(ActualX + 175, ActualY + 75);
                    point2 = new Point(ActualX + 125, ActualY + 125);
                    context.DrawLine(LinePen, point1, point2);
                    break;
                default:
                    break;
            }
        }
        private void DrawingHLine(DrawingContext context)
        {
            Point point1 = new Point(ActualX, ActualY + GlobalSetting.LadderHeightUnit / 3);
            Point point2 = new Point(ActualX + WidthUnit, ActualY + Global.GlobalSetting.LadderHeightUnit / 3);
            context.DrawLine(LinePen, point1, point2);
        }
        private void DrawingVLine(DrawingContext context)
        {
            Point point1 = new Point(ActualX + WidthUnit, ActualY + GlobalSetting.LadderHeightUnit / 3);
            Point point2 = new Point(ActualX + WidthUnit, ActualY + HeightUnit + GlobalSetting.LadderHeightUnit / 3);
            context.DrawLine(LinePen, point1, point2);
        }
        private PathFigure GenerateBracket(bool isLeft)
        {
            Point start;
            Point end;
            start = isLeft ? new Point(ActualX + 100, ActualY + 50) : new Point(ActualX + 200, ActualY + 50);
            end = isLeft ? new Point(ActualX + 100, ActualY + 150) : new Point(ActualX + 200, ActualY + 150);
            PathFigure figure = new PathFigure();
            figure.StartPoint = start;
            figure.IsClosed = false;
            figure.IsFilled = false;
            figure.Segments.Add(isLeft ? new ArcSegment(end, new Size(60, 60), 0, false, SweepDirection.Counterclockwise, true) : new ArcSegment(end, new Size(60, 60), 0, false, SweepDirection.Clockwise, true));
            return figure;
        }

        private void DrawingArrow(DrawingContext context, bool isUp)
        {
            Point point1;
            Point point2;
            point1 = new Point(ActualX + WidthUnit * 0.5, ActualY + 50);
            point2 = new Point(ActualX + WidthUnit * 0.5, ActualY + 150);
            context.DrawLine(LinePen, point1, point2);
            if (isUp)
            {
                point1 = new Point(ActualX + 150, ActualY + 50);
                point2 = new Point(ActualX + 125, ActualY + 75);
                context.DrawLine(LinePen, point1, point2);
                point1 = new Point(ActualX + 150, ActualY + 50);
                point2 = new Point(ActualX + 175, ActualY + 75);
                context.DrawLine(LinePen, point1, point2);
            }
            else
            {
                point1 = new Point(ActualX + 150, ActualY + 150);
                point2 = new Point(ActualX + 125, ActualY + 125);
                context.DrawLine(LinePen, point1, point2);
                point1 = new Point(ActualX + 150, ActualY + 150);
                point2 = new Point(ActualX + 175, ActualY + 125);
                context.DrawLine(LinePen, point1, point2);
            }
        }
        
        public enum FontTypes { Normal, Property, Comment };
        private void DrawingText(DrawingContext context, Point relativePoint, int span, string text, FontFamily fontFamily, FontWeight fontWeight, double emSize, double containerWidth = 100, TextAlignment alignment = TextAlignment.Left, FontTypes type = FontTypes.Normal)
        {
            Typeface face;
            FontData data;
            FormattedText formattedText;
            switch (type)
            {
                case FontTypes.Comment:
                    data = FontManager.GetComment();
                    face = new Typeface(data.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                    formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, data.FontSize, data.FontColor);
                    formattedText.MaxTextWidth = 290;
                    formattedText.MaxLineCount = 1;
                    formattedText.Trimming = TextTrimming.CharacterEllipsis;
                    break;
                case FontTypes.Property:
                    data = FontManager.GetLadder();
                    face = new Typeface(data.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                    formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, data.FontSize, data.FontColor);
                    break;
                case FontTypes.Normal:
                default:
                    face = new Typeface(fontFamily, FontStyles.Normal, fontWeight, FontStretches.Normal);
                    formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, emSize, Brushes.Black);
                    break;
            }
            double x, y;
            x = ActualX + relativePoint.X;
            y = ActualY + relativePoint.Y;
            if (alignment == TextAlignment.Center)
                x += (containerWidth - formattedText.Width) / 2;
            context.DrawText(formattedText, new Point(x, y));
        }

        #endregion

        #region Property
        
        public void DrawingInputProperty(DrawingContext context)
        {
            string text1 = string.Empty, text2 = string.Empty;
            switch (core.Type)
            {
                case LadderUnitModel.Types.LD:
                case LadderUnitModel.Types.LDI:
                case LadderUnitModel.Types.LDIM:
                case LadderUnitModel.Types.LDIIM:
                case LadderUnitModel.Types.LDP:
                case LadderUnitModel.Types.LDF:
                    if (core.IsUsed)
                    {
                        switch (core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Children[0].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGSimu.IsAlive ? "???" : core.Children[0].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGComu.IsAlive ? "???" : core.Children[0].Value);
                                break;
                            default:
                                break;
                        }
                        DrawingText(context, new Point(0, 0),HeightUnit, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontTypes.Property);
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
                    if (core.IsUsed)
                    {
                        switch (core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Children[0].Text;
                                text2 = core.Children[1].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGSimu.IsAlive ? "???" : core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Children[1].Text,
                                        !MNGSimu.IsAlive ? "???" : core.Children[1].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGComu.IsAlive ? "???" : core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Children[1].Text,
                                        !MNGComu.IsAlive ? "???" : core.Children[1].Value);
                                break;
                        }
                        DrawingText(context, new Point(0, 0), HeightUnit, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontTypes.Property);
                        DrawingText(context, new Point(0, 150), HeightUnit, text2, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontTypes.Property);
                    }
                    break;
            }
            switch (core.LadderMode)
            {
                case LadderModes.Simulate:
                case LadderModes.Monitor:
                    bool value = false;
                    Int32 w1 = 0, w2 = 0;
                    Int64 d1 = 0, d2 = 0;
                    double f1 = 0, f2 = 0;
                    try
                    {
                        switch (core.Type)
                        {
                            case LadderUnitModel.Types.LD:
                            case LadderUnitModel.Types.LDIM:
                                value = BIT_1_SHOWS.Contains(core.Children[0].Value.ToString());
                                break;
                            case LadderUnitModel.Types.LDI:
                            case LadderUnitModel.Types.LDIIM:
                                value = BIT_0_SHOWS.Contains(core.Children[0].Value.ToString());
                                break;
                            case LadderUnitModel.Types.LDWEQ:
                            case LadderUnitModel.Types.LDWNE:
                            case LadderUnitModel.Types.LDWGE:
                            case LadderUnitModel.Types.LDWLE:
                            case LadderUnitModel.Types.LDWG:
                            case LadderUnitModel.Types.LDWL:
                                w1 = short.Parse(core.Children[0].Value.ToString());
                                w2 = short.Parse(core.Children[1].Value.ToString());
                                break;
                            case LadderUnitModel.Types.LDDEQ:
                            case LadderUnitModel.Types.LDDNE:
                            case LadderUnitModel.Types.LDDGE:
                            case LadderUnitModel.Types.LDDLE:
                            case LadderUnitModel.Types.LDDG:
                            case LadderUnitModel.Types.LDDL:
                                d1 = int.Parse(core.Children[0].Value.ToString());
                                d2 = int.Parse(core.Children[1].Value.ToString());
                                break;
                            case LadderUnitModel.Types.LDFEQ:
                            case LadderUnitModel.Types.LDFNE:
                            case LadderUnitModel.Types.LDFGE:
                            case LadderUnitModel.Types.LDFLE:
                            case LadderUnitModel.Types.LDFG:
                            case LadderUnitModel.Types.LDFL:
                                f1 = float.Parse(core.Children[0].Value.ToString());
                                f2 = float.Parse(core.Children[1].Value.ToString());
                                break;
                        }
                        switch (core.InstName)
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
                        context.DrawRectangle(Brushes.Red, Transparent, new Rect(new Point(ActualX + 100, ActualY + 50), new Size(100, 100)));
                        return;
                    }
                    context.DrawRectangle(value ? Brushes.Green : Brushes.Transparent, Transparent, new Rect(new Point(ActualX + 100, ActualY + 50), new Size(100, 100)));
                    break;
            }
        }
        public void DrawingOutputProperty(DrawingContext context)
        {
            string text1 = string.Empty, text2 = string.Empty;
            switch (core.Type)
            {
                case LadderUnitModel.Types.OUT:
                case LadderUnitModel.Types.OUTIM:
                    if (core.IsUsed)
                    {
                        switch (core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Children[0].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGSimu.IsAlive ? "???" : core.Children[0].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGComu.IsAlive ? "???" : core.Children[0].Value);
                                break;
                        }
                        DrawingText(context, new Point(0, 0), HeightUnit, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontTypes.Property);
                    }
                    break;
                case LadderUnitModel.Types.SET:
                case LadderUnitModel.Types.SETIM:
                case LadderUnitModel.Types.RST:
                case LadderUnitModel.Types.RSTIM:
                    if (core.IsUsed)
                    {
                        switch (core.LadderMode)
                        {
                            case LadderModes.Edit:
                                text1 = core.Children[0].Text;
                                text2 = core.Children[1].Text;
                                break;
                            case LadderModes.Simulate:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGSimu.IsAlive ? "???" : core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Children[1].Text,
                                        !MNGSimu.IsAlive ? "???" : core.Children[1].Value);
                                break;
                            case LadderModes.Monitor:
                                text1 = string.Format("{0:s} = {1}",
                                        core.Children[0].Text,
                                        !MNGComu.IsAlive ? "???" : core.Children[0].Value);
                                text2 = string.Format("{0:s} = {1}",
                                        core.Children[1].Text,
                                        !MNGComu.IsAlive ? "???" : core.Children[1].Value);
                                break;
                        }
                        DrawingText(context, new Point(0, 0), HeightUnit, text1, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontTypes.Property);
                        DrawingText(context, new Point(0, 150), HeightUnit, text2, null, FontWeights.Heavy, 0, 300, TextAlignment.Center, FontTypes.Property);
                    }
                    break;
            }
            switch (core.LadderMode)
            {
                case LadderModes.Simulate:
                case LadderModes.Monitor:
                    bool value = BIT_1_SHOWS.Contains(core.Children[0].Value.ToString());
                    context.DrawRectangle(value ? Brushes.Green : Brushes.Transparent, Transparent, new Rect(new Point(ActualX + 100, ActualY + 50), new Size(100, 100)));
                    break;
            }
        }
        public void DrawingOutputRectProperty(DrawingContext context)
        {
            switch (core.Type)
            {
                case LadderUnitModel.Types.TON:
                case LadderUnitModel.Types.TONR:
                case LadderUnitModel.Types.TOF:
                    if (core.Children[0].Base == ValueModel.Bases.TV)
                    {
                        int offset = core.Children[0].Offset;
                        if (offset >= 0 && offset < 200)
                            DrawingText(context, new Point(25, 250), HeightUnit, "100 ms", Arial, FontWeights.Normal, 25);
                        else if (offset >= 200 && offset < 250)
                            DrawingText(context, new Point(25, 250), HeightUnit, "10 ms", Arial, FontWeights.Normal, 25);
                        else
                            DrawingText(context, new Point(25, 250), HeightUnit, "1 ms", Arial, FontWeights.Normal, 25);
                    }
                    break;
            }
            if (core.IsUsed)
            {
                ValueModel vmodel;
                ValueFormat vformat;
                switch (core.LadderMode)
                {
                    case LadderModes.Edit:
                        for (int i = 0; i < core.Children.Count; i++)
                        {
                            vmodel = core.Children[i];
                            vformat = vmodel.Format;
                            string text = string.Format("{0:s}:{1:s}",
                                vformat.Name, vmodel.Text);
                            if (vformat.Position >= 0)
                                DrawingText(context, new Point(25, 120 + 30 * vformat.Position - (core.Type == LadderUnitModel.Types.PID ? 20 : 0)), HeightUnit, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontTypes.Property);
                            else
                                DrawingText(context, new Point(25, 250 + 30 * (vformat.Position + 1)), HeightUnit, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontTypes.Property);
                        }
                        break;
                    case LadderModes.Simulate:
                        for (int i = 0; i < core.Children.Count; i++)
                        {
                            vmodel = core.Children[i];
                            vformat = vmodel.Format;
                            string text = string.Format("{0:s} = {1}",
                                vmodel.Text, !MNGSimu.IsActive ? "???" : vmodel.Value);
                            if (vformat.Position >= 0)
                                DrawingText(context, new Point(25, 120 + 30 * vformat.Position - (core.Type == LadderUnitModel.Types.PID ? 20 : 0)), HeightUnit, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontTypes.Property);
                            else
                                DrawingText(context, new Point(25, 250 + 30 * (vformat.Position + 1)), HeightUnit, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontTypes.Property);
                        }
                        break;
                    case LadderModes.Monitor:
                        for (int i = 0; i < core.Children.Count; i++)
                        {
                            vmodel = core.Children[i];
                            vformat = vmodel.Format;
                            string text = string.Format("{0:s} = {1}",
                                vmodel.Text, !MNGComu.IsActive ? "???" : vmodel.Value);
                            if (vformat.Position >= 0)
                                DrawingText(context, new Point(25, 120 + 30 * vformat.Position - (core.Type == LadderUnitModel.Types.PID ? 20 : 0)), HeightUnit, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontTypes.Property);
                            else
                                DrawingText(context, new Point(25, 250 + 30 * (vformat.Position + 1)), HeightUnit, text, null, FontWeights.Heavy, 0, 0, TextAlignment.Left, FontTypes.Property);
                        }
                        break;
                }
            }
        }

        #endregion
        
        #endregion

        #endregion

        #region Event Handler

        private void OnValueStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate () { Update(); });
        }
        
        private void OnSimulateStarted(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate () { Update(); });
        }

        private void OnSimulateAborted(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate () { Update(); });
        }

        private void OnMonitorStarted(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate () { Update(); });
        }

        private void OnMonitorAborted(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate () { Update(); });
        }

        #endregion
    }
}
