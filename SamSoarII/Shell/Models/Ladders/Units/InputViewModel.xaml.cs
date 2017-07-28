using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
using SamSoarII.Shell.Managers;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// InputViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InputViewModel : LadderUnitViewModel, INotifyPropertyChanged, IResource
    {
        #region IResource

        public override IResource Create(params object[] args)
        {
            return new InputViewModel((LadderUnitModel)args[0]);
        }
        
        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            ReinitializeComponent();
            DataContext = this;
            if (Core?.Parent?.View != null)
                IsCommentMode = Core.Parent.View.IsCommentMode;
            recreating = false;
        }

        #endregion

        public InputViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            comments = new TextBlock[2];
            if (_core != null) Recreate(_core);
        }
        
        private void ReinitializeComponent()
        {
            // 开始绘图
            CenterCanvas.Children.Clear();
            Line line = null;
            switch (Core.InstName)
            {
                case "LDWEQ":
                    CenterTextBlock.Text = "W==";
                    break;
                case "LDWNE":
                    CenterTextBlock.Text = "W<>";
                    break;
                case "LDWGE":
                    CenterTextBlock.Text = "W>=";
                    break;
                case "LDWLE":
                    CenterTextBlock.Text = "W<=";
                    break;
                case "LDWG":
                    CenterTextBlock.Text = "W>";
                    break;
                case "LDWL":
                    CenterTextBlock.Text = "W<";
                    break;
                case "LDDEQ":
                    CenterTextBlock.Text = "D==";
                    break;
                case "LDDNE":
                    CenterTextBlock.Text = "D<>";
                    break;
                case "LDDGE":
                    CenterTextBlock.Text = "D>=";
                    break;
                case "LDDLE":
                    CenterTextBlock.Text = "D<=";
                    break;
                case "LDDG":
                    CenterTextBlock.Text = "D>";
                    break;
                case "LDDL":
                    CenterTextBlock.Text = "D<";
                    break;
                case "LDFEQ":
                    CenterTextBlock.Text = "F==";
                    break;
                case "LDFNE":
                    CenterTextBlock.Text = "F<>";
                    break;
                case "LDFGE":
                    CenterTextBlock.Text = "F>=";
                    break;
                case "LDFLE":
                    CenterTextBlock.Text = "F<=";
                    break;
                case "LDFG":
                    CenterTextBlock.Text = "F>";
                    break;
                case "LDFL":
                    CenterTextBlock.Text = "F<";
                    break;
                default:
                    CenterTextBlock.Text = "";
                    break;
            }
            // 画个-[/]-表示【取反】
            switch (Core.InstName)
            {
                case "LDI":
                case "LDIIM":
                    line = new Line();
                    line.X1 = 75;
                    line.X2 = 25;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
                default:
                    break;
            }
            // 画个-[|]-表示【立即】
            switch (Core.InstName)
            {
                case "LDIM":
                case "LDIIM":
                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 50;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
            }
            // 画个-[↑]-表示【上升沿】
            switch (Core.InstName)
            {
                case "LDP":
                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 70;
                    line.Y1 = 0;
                    line.Y2 = 20;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 30;
                    line.Y1 = 0;
                    line.Y2 = 20;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 50;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
                default:
                    break;
            }
            // 画个-[↓]-表示【下降沿】
            switch (Core.InstName)
            {
                case "LDF":
                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 50;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 70;
                    line.Y1 = 100;
                    line.Y2 = 80;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 30;
                    line.Y1 = 100;
                    line.Y2 = 80;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
                default:
                    break;
            }
            ValueTextBlock.Visibility = Core.Children.Count >= 1
                ? Visibility.Visible
                : Visibility.Hidden;
            Value2TextBlock.Visibility = Core.Children.Count >= 2
                ? Visibility.Visible
                : Visibility.Hidden;
            CenterCanvas.Children.Add(CenterTextBlock);
            CommentArea.Children.Clear();
            for (int i = 0; i < Core.Children.Count; i++)
            {
                if (comments[i] == null)
                    comments[i] = new TextBlock();
                CommentArea.Children.Add(comments[i]);
            }
            Update();
        }
        
        public override void Dispose()
        { 
            if (Core.Breakpoint.View != null)
            {
                mainCanvas.Children.Remove(Core.Breakpoint.View);
                Core.Breakpoint.View.Dispose();
            }
            base.Dispose();
            DataContext = null;
            AllResourceManager.Dispose(this);
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Shell
        
        private TextBlock[] comments;

        public override bool IsCommentMode
        {
            set
            {
                base.IsCommentMode = value;
                CommentArea.Visibility = IsCommentMode ? Visibility.Visible : Visibility.Hidden;
            }
        }
        
        public override void Update(int flags = UPDATE_ALL)
        {
            FontData fdata = null;
            base.Update(flags);
            switch (flags)
            {
                case UPDATE_STYLE:
                    fdata = FontManager.GetLadder();
                    ValueTextBlock.Foreground = fdata.FontColor;
                    ValueTextBlock.FontFamily = fdata.FontFamily;
                    ValueTextBlock.FontSize = fdata.FontSize;
                    Value2TextBlock.Foreground = fdata.FontColor;
                    Value2TextBlock.FontFamily = fdata.FontFamily;
                    Value2TextBlock.FontSize = fdata.FontSize;
                    fdata = FontManager.GetComment();
                    foreach (TextBlock comment in comments)
                    {
                        if (comment != null)
                        {
                            comment.Foreground = fdata.FontColor;
                            comment.FontFamily = fdata.FontFamily;
                            comment.FontSize = fdata.FontSize;
                        }
                    }
                    break;
                case UPDATE_BRPO:
                    if (LadderMode == LadderModes.Edit)
                    {
                        if (Core.Breakpoint.View != null)
                        {
                            mainCanvas.Children.Remove(Core.Breakpoint.View);
                            Core.Breakpoint.View.Dispose();
                        }
                        mainCanvas.Background = Brushes.Transparent;
                        break;
                    }
                    if (Core.BPEnable && Core.Breakpoint.View == null)
                    {
                        Core.Breakpoint.View = AllResourceManager.CreateBrpo(Core.Breakpoint);
                        mainCanvas.Children.Add(Core.Breakpoint.View);
                    }
                    if (!Core.BPEnable && Core.Breakpoint.View != null)
                    {
                        mainCanvas.Children.Remove(Core.Breakpoint.View);
                        Core.Breakpoint.View.Dispose();
                    }
                    mainCanvas.Background = (Core.BPCursor != null ? Brushes.Yellow : Brushes.Transparent);
                    break;
                case UPDATE_PROPERTY:
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (Core == null) return;
                        UpdateCenterCanvas();
                        if (LadderMode == LadderModes.Edit)
                        {
                            if (Core.Children.Count >= 1) ValueTextBlock.Text = Core.Children[0].Text;
                            if (Core.Children.Count >= 2) Value2TextBlock.Text = Core.Children[1].Text;
                            for (int i = 0; i < Core.Children.Count; i++)
                            {
                                ValueModel vmodel = Core.Children[i];
                                comments[i].Text = String.Format("{0:s}:{1:s}",
                                    vmodel.Text, vmodel.Comment);
                            }
                        }
                        else if (!Core.IsUsed)
                        {
                            ValueTextBlock.Text = "";
                            Value2TextBlock.Text = "";
                        }
                        else
                        {
                            if (LadderMode == LadderModes.Simulate)
                            {
                                if (Core.Children.Count >= 1) ValueTextBlock.Text =
                                    String.Format("{0:s} = {1}",
                                        Core.Children[0].Text,
                                        !MNGSimu.IsAlive ? "???" : Core.Children[0].Value);
                                if (Core.Children.Count >= 2) Value2TextBlock.Text =
                                    String.Format("{0:s} = {1}",
                                        Core.Children[1].Text,
                                        !MNGSimu.IsAlive ? "???" : Core.Children[1].Value);
                            }
                            else if (LadderMode == LadderModes.Monitor)
                            {
                                if (Core.Children.Count >= 1) ValueTextBlock.Text =
                                    String.Format("{0:s} = {1}",
                                        Core.Children[0].Text,
                                        !MNGComu.IsAlive ? "???" : Core.Children[0].Value);
                                if (Core.Children.Count >= 2) Value2TextBlock.Text =
                                    String.Format("{0:s} = {1}",
                                        Core.Children[1].Text,
                                        !MNGComu.IsAlive ? "???" : Core.Children[1].Value);
                            }
                        }
                    });
                    break;
            }
        }
        
        static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE" };
        static string[] BIT_1_SHOWS = { "1", "ON", "TRUE" };

        private void UpdateCenterCanvas()
        {
            if (LadderMode == LadderModes.Edit ||
               (LadderMode == LadderModes.Simulate && !MNGSimu.IsAlive) ||
               (LadderMode == LadderModes.Monitor && !MNGComu.IsAlive))
            {
                CenterCanvas.Background = Brushes.Transparent;
                return;
            }
            bool value = false;
            Int32 w1 = 0, w2 = 0;
            Int64 d1 = 0, d2 = 0;
            double f1 = 0, f2 = 0;
            try
            {
                switch (Core.InstName)
                {
                    case "LD":
                    case "LDIM":
                        value = BIT_1_SHOWS.Contains(Core.Children[0].Value.ToString());
                        break;
                    case "LDI":
                    case "LDIIM":
                        value = BIT_0_SHOWS.Contains(Core.Children[0].Value.ToString());
                        break;
                    case "LDWEQ":
                    case "LDWNE":
                    case "LDWLE":
                    case "LDWGE":
                    case "LDWL":
                    case "LDWG":
                        w1 = (Int16)(Core.Children[0].Value);
                        w2 = (Int16)(Core.Children[1].Value);
                        break;
                    case "LDDEQ":
                    case "LDDNE":
                    case "LDDLE":
                    case "LDDGE":
                    case "LDDL":
                    case "LDDG":
                        d1 = (Int32)(Core.Children[0].Value);
                        d2 = (Int32)(Core.Children[1].Value);
                        break;
                    case "LDFEQ":
                    case "LDFNE":
                    case "LDFLE":
                    case "LDFGE":
                    case "LDFL":
                    case "LDFG":
                        f1 = (float)(Core.Children[0].Value);
                        f2 = (float)(Core.Children[1].Value);
                        break;
                }
                switch (Core.InstName)
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
                CenterCanvas.Background = Brushes.Red;
                return;
            }
            CenterCanvas.Background = value ? Brushes.Green : Brushes.Transparent;
        }

        #endregion
    }
}
