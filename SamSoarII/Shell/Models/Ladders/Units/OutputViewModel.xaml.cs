using SamSoarII.Core.Models;
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
    /// OutputViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class OutputViewModel : LadderUnitViewModel, INotifyPropertyChanged, IResource
    {
        #region IResource
        
        public override IResource Create(params object[] args)
        {
            return new OutputViewModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            ReinitializeComponent();
            DataContext = this;
            recreating = false;
        }

        #endregion

        public OutputViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            comments = new TextBlock[2];
            if (_core != null) Recreate(_core);
        }

        public override void Dispose()
        {
            if (Core.Breakpoint.View != null)
            {
                MainCanvas.Children.Remove(Core.Breakpoint.View);
                Core.Breakpoint.View.Dispose();
            }
            base.Dispose();
            DataContext = null;
            AllResourceManager.Dispose(this);
        }

        private void ReinitializeComponent()
        {
            switch (Core.InstName)
            {
                case "OUT": CenterTextBlock.Text = String.Empty; break;
                case "OUTIM": CenterTextBlock.Text = "I"; break;
                case "RST": CenterTextBlock.Text = "R"; break;
                case "RSTIM": CenterTextBlock.Text = "RI"; break;
                case "SET": CenterTextBlock.Text = "S"; break;
                case "SETIM": CenterTextBlock.Text = "SI"; break;
                default: CenterTextBlock.Text = ""; break;
            }
            ValueTextBlock.Visibility = Core.Children.Count >= 1
                ? Visibility.Visible : Visibility.Hidden;
            CountTextBlock.Visibility = Core.Children.Count >= 2
                ? Visibility.Visible : Visibility.Hidden;
            for (int i = 0; i < comments.Length; i++)
            {
                if (i < Core.Children.Count)
                    ShowComment(i);
                else
                    HideComment(i);
            }
            Update();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Shell

        private TextBlock[] comments;
        public void ShowComment(int id)
        {
            if (comments[id] == null)
            {
                comments[id] = new TextBlock();
                MainCanvas.Children.Add(comments[id]);
            }
            Canvas.SetTop(comments[id], 300 + (FontManager.GetComment().FontSize + 4) * id);
            Canvas.SetLeft(comments[id], 4);
            comments[id].Visibility = IsCommentMode
                ? Visibility.Visible : Visibility.Hidden;
        }
        public void HideComment(int id)
        {
            if (comments[id] == null) return;
            comments[id].Visibility = Visibility.Hidden;
        }

        public string ValueText
        {
            get
            {
                if (Core.Children.Count >= 1)
                {
                    switch (LadderMode)
                    {
                        case LadderModes.Edit: return Core.Children[0].Text;
                        case LadderModes.Simulate: return String.Format("{0:s} = {1}",
                            Core.Children[0].Text, MNGSimu.IsAlive ? Core.Children[0].Value : "???");
                    }
                }
                return "";
            }
        }
        public string CountText
        {
            get
            {
                if (Core.Children.Count >= 2)
                {
                    switch (LadderMode)
                    {
                        case LadderModes.Edit: return Core.Children[1].Text;
                        case LadderModes.Simulate: return String.Format("{0:s} = {1}",
                            Core.Children[1].Text, MNGSimu.IsAlive ? Core.Children[1].Value : "???");
                    }
                }
                return "";
            }
        }

        protected override void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCorePropertyChanged(sender, e);
            if (e.PropertyName.Equals("IsCommentMode"))
            {
                for (int i = 0; i < Core.Children.Count; i++)
                    if (comments[i] != null)
                        comments[i].Visibility = IsCommentMode ? Visibility.Visible : Visibility.Hidden;
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
                    CountTextBlock.Foreground = fdata.FontColor;
                    CountTextBlock.FontFamily = fdata.FontFamily;
                    CountTextBlock.FontSize = fdata.FontSize;
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
                    for (int i = 0; i < comments.Length; i++)
                    {
                        if (i < Core.Children.Count)
                            ShowComment(i);
                        else
                            HideComment(i);
                    }
                    break;
                case UPDATE_BRPO:
                    if (LadderMode == LadderModes.Edit)
                    {
                        if (Core.Breakpoint.View != null)
                        {
                            MainCanvas.Children.Remove(Core.Breakpoint.View);
                            Core.Breakpoint.View.Dispose();
                        }
                        MainCanvas.Background = Brushes.Transparent;
                        break;
                    }
                    if (Core.BPEnable && Core.Breakpoint.View == null)
                    {
                        Core.Breakpoint.View = AllResourceManager.CreateBrpo(Core.Breakpoint);
                        MainCanvas.Children.Add(Core.Breakpoint.View);
                    }
                    if (!Core.BPEnable && Core.Breakpoint.View != null)
                    {
                        MainCanvas.Children.Remove(Core.Breakpoint.View);
                        Core.Breakpoint.View.Dispose();
                    }
                    MainCanvas.Background = (Core.BPCursor != null ? Brushes.Yellow : Brushes.Transparent);
                    break;
                case UPDATE_PROPERTY:
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (Core == null) return;
                        UpdateCenterCanvas();
                        if (LadderMode == LadderModes.Edit)
                        {
                            if (Core.Children.Count >= 1) ValueTextBlock.Text = Core.Children[0].Text;
                            if (Core.Children.Count >= 2) CountTextBlock.Text = Core.Children[1].Text;
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
                            CountTextBlock.Text = "";
                        }
                        else
                        {
                            if (LadderMode == LadderModes.Simulate)
                            {
                                if (Core.Children.Count >= 1) ValueTextBlock.Text =
                                    String.Format("{0:s} = {1}",
                                        Core.Children[0].Text,
                                        !MNGSimu.IsAlive ? "???" : Core.Children[0].Value);
                                if (Core.Children.Count >= 2) CountTextBlock.Text =
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
                                if (Core.Children.Count >= 2) CountTextBlock.Text =
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
            bool value = BIT_1_SHOWS.Contains(Core.Children[0].Value.ToString());
            CenterCanvas.Background = value ? Brushes.Green : Brushes.Transparent;
        }

        #endregion
    }
}
