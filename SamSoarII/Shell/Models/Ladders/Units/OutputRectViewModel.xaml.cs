using SamSoarII.Core.Models;
using SamSoarII.Shell.Managers;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
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
using System.ComponentModel;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// OutputRectViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class OutputRectViewModel : LadderUnitViewModel, IResource
    {
        #region IResource

        public override IResource Create(params object[] args)
        {
            return new OutputRectViewModel((LadderUnitModel)args[0]);
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

        public OutputRectViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            middlevalues = new TextBlock[5];
            bottomvalues = new TextBlock[2];
            comments = new TextBlock[6];
            if (_core != null) Recreate(_core);
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

        private void ReinitializeComponent()
        {
            TopTextBlock.Text = Core.InstName;
            for (int i = -2; i < 5; i++)
                HideValue(i);
            for (int i = 0; i < Core.Children.Count; i++)
            {
                ValueModel vmodel = Core.Children[i];
                ValueFormat vformat = vmodel.Format;
                ShowValue(vformat.Position);
            }
            switch (Core.Type)
            {
                case LadderUnitModel.Types.TON:
                case LadderUnitModel.Types.TONR:
                case LadderUnitModel.Types.TOF:
                    ShowValue(-1);
                    bottomvalues[0].Text = "100 ms";
                    break;
            }
            for (int i = 0; i < comments.Length; i++)
            {
                if (i < Core.Children.Count)
                    ShowComment(i);
                else
                    HideComment(i);
            }
            Canvas.SetTop(LB_EN, Core.Type == LadderUnitModel.Types.PID ? 60 : 80);
            Update();
        }

        #region Shell

        private TextBlock[] middlevalues;
        private TextBlock[] bottomvalues;
        public void ShowValue(int id)
        {
            if (id >= 0)
            {
                if (middlevalues[id] == null)
                {
                    middlevalues[id] = new TextBlock();
                    middlevalues[id].TextAlignment = TextAlignment.Left;
                    Canvas.SetLeft(middlevalues[id], 25);
                    mainCanvas.Children.Add(middlevalues[id]);
                }
                Canvas.SetTop(middlevalues[id], 120 + id * 30 - (Core.Type == LadderUnitModel.Types.PID ? 20 : 0));
                middlevalues[id].Visibility = Visibility.Visible;
            }
            else
            {
                id = -id - 1;
                if (bottomvalues[id] == null)
                {
                    bottomvalues[id] = new TextBlock();
                    bottomvalues[id].TextAlignment = TextAlignment.Right;
                    Canvas.SetLeft(bottomvalues[id], 25);
                    Canvas.SetTop(bottomvalues[id], 250 - id * 30);
                    mainCanvas.Children.Add(bottomvalues[id]);
                }
                bottomvalues[id].Visibility = Visibility.Visible;
            }
        }
        public void HideValue(int id)
        {
            if (id >= 0)
            {
                if (middlevalues[id] == null) return;
                middlevalues[id].Visibility = Visibility.Hidden;
            }
            else
            {
                id = -id - 1;
                if (bottomvalues[id] == null) return;
                bottomvalues[id].Visibility = Visibility.Hidden;
            }
        }
        
        private TextBlock[] comments;
        public void ShowComment(int id)
        {
            if (comments[id] == null)
            {
                comments[id] = new TextBlock();
                mainCanvas.Children.Add(comments[id]);
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

        public override void Update(int flags = 255)
        {
            FontData fdata = null;
            base.Update(flags);
            switch (flags)
            {
                case UPDATE_STYLE:
                    fdata = FontManager.GetLadder();
                    TopTextBlock.Foreground = fdata.FontColor;
                    TopTextBlock.FontFamily = fdata.FontFamily;
                    TopTextBlock.FontSize = fdata.FontSize;
                    foreach (TextBlock middle in middlevalues)
                    {
                        if (middle != null)
                        {
                            middle.Foreground = fdata.FontColor;
                            middle.FontFamily = fdata.FontFamily;
                            middle.FontSize = fdata.FontSize;
                        }
                    }
                    foreach (TextBlock bottom in bottomvalues)
                    {
                        if (bottom != null)
                        {
                            bottom.Foreground = fdata.FontColor;
                            bottom.FontFamily = fdata.FontFamily;
                            bottom.FontSize = fdata.FontSize;
                        }
                    }
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
                        if (LadderMode == LadderModes.Edit)
                        {
                            for (int i = 0; i < Core.Children.Count; i++)
                            {
                                ValueModel vmodel = Core.Children[i];
                                ValueFormat vformat = vmodel.Format;
                                ShowValue(vformat.Position);
                                string text = String.Format("{0:s}:{1:s}",
                                    vformat.Name, vmodel.Text);
                                TextBlock textblock = null;
                                if (vformat.Position >= 0)
                                    textblock = middlevalues[vformat.Position];
                                else
                                    textblock = bottomvalues[-vformat.Position - 1];
                                textblock.Text = text;
                                ShowComment(i);
                                comments[i].Text = String.Format("{0:s}:{1:s}",
                                    vmodel.Text, vmodel.Comment);
                            }
                            for (int i = Core.Children.Count; i < 5; i++)
                            {
                                HideValue(i);
                                HideComment(i);
                            }
                        }
                        else if (!Core.IsUsed)
                        {
                            for (int i = 0; i < Core.Children.Count; i++)
                            {
                                ValueModel vmodel = Core.Children[i];
                                ValueFormat vformat = vmodel.Format;
                                if (vformat.Position >= 0)
                                    middlevalues[vformat.Position].Text = "";
                                else
                                    bottomvalues[-vformat.Position - 1].Text = "";
                            }
                        }
                        else
                        {
                            if (LadderMode == LadderModes.Simulate)
                            {
                                for (int i = 0; i < Core.Children.Count; i++)
                                {
                                    ValueModel vmodel = Core.Children[i];
                                    ValueFormat vformat = vmodel.Format;
                                    string text = String.Format("{0:s} = {1}",
                                        vmodel.Text, !MNGSimu.IsAlive ? "???" : vmodel.Value);
                                    if (vformat.Position >= 0)
                                        middlevalues[vformat.Position].Text = text;
                                    else
                                        bottomvalues[-vformat.Position - 1].Text = text;
                                }
                            }
                            else if (LadderMode == LadderModes.Monitor)
                            {
                                for (int i = 0; i < Core.Children.Count; i++)
                                {
                                    ValueModel vmodel = Core.Children[i];
                                    ValueFormat vformat = vmodel.Format;
                                    string text = String.Format("{0:s} = {1}",
                                        vmodel.Text, !MNGComu.IsAlive ? "???" : vmodel.Value);
                                    if (vformat.Position >= 0)
                                        middlevalues[vformat.Position].Text = text;
                                    else
                                        bottomvalues[-vformat.Position - 1].Text = text;
                                }
                            }
                        }
                    });
                    break;
            }
        }

        #endregion

    }
}
