using SamSoarII.Core.Models;
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
            middlevalues = new TextBlock[5];
            bottomvalues = new TextBlock[2];
            comments = new TextBlock[5];
            ReinitializeComponent();
            DataContext = this;
            if (Core?.Parent?.View != null)
            {
                LadderMode = Core.Parent.View.LadderMode;
                IsCommentMode = Core.Parent.View.IsCommentMode;
            }
        }
        
        #endregion

        public OutputRectViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            Recreate(_core);
        }

        public override void Dispose()
        {
            base.Dispose();
            DataContext = null;
            AllResourceManager.Dispose(this);
        }

        private void ReinitializeComponent()
        {
            bool[] middleused = { false, false, false, false, false };
            bool[] bottomused = { false, false };
            int maxcount = Core.Children.Count;
            if (Core.Type == LadderUnitModel.Types.CALLM)
            {
                maxcount = 5;
                for (int i = 0; i < maxcount; i++)
                {
                    if (middlevalues[i] == null)
                    {
                        middlevalues[i] = new TextBlock();
                        Canvas.SetLeft(middlevalues[i], 25);
                        Canvas.SetTop(middlevalues[i], 120 + i * 30);
                        middlevalues[i].TextAlignment = TextAlignment.Left;
                        middlevalues[i].Style = (Style)Resources["LadderFontStyle"];
                    }
                    if (!mainCanvas.Children.Contains(middlevalues[i]))
                        mainCanvas.Children.Add(middlevalues[i]);
                }
                for (int i = 0; i < 2; i++)
                {
                    if (bottomvalues[i] != null && mainCanvas.Children.Contains(bottomvalues[i]))
                        mainCanvas.Children.Remove(bottomvalues[i]);
                }
            }
            for (int i = 0; i < Core.Children.Count; i++)
            {
                ValueModel vmodel = Core.Children[i];
                ValueFormat vformat = vmodel.Format;
                int id = 0;
                if (vformat.Position >= 0)
                {
                    id = vformat.Position;
                    middleused[id] = true;
                    if (middlevalues[id] == null)
                    {
                        middlevalues[id] = new TextBlock();
                        Canvas.SetLeft(middlevalues[id], 25);
                        Canvas.SetTop(middlevalues[id], 120 + id * 30);
                        middlevalues[id].TextAlignment = TextAlignment.Left;
                        middlevalues[id].Style = (Style)Resources["LadderFontStyle"];
                    }
                    if (!mainCanvas.Children.Contains(middlevalues[id]))
                        mainCanvas.Children.Add(middlevalues[id]);
                }
                else
                {
                    id = -vformat.Position - 1;
                    bottomused[id] = true; 
                    if (bottomvalues[id] == null)
                    {
                        bottomvalues[id] = new TextBlock();
                        Canvas.SetLeft(bottomvalues[id], 25);
                        Canvas.SetTop(bottomvalues[id], 250 - id * 30);
                        bottomvalues[id].TextAlignment = TextAlignment.Right;
                        bottomvalues[id].Style = (Style)Resources["LadderFontStyle"];
                    }
                    if (!mainCanvas.Children.Contains(bottomvalues[id]))
                        mainCanvas.Children.Add(bottomvalues[id]);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (!middleused[i] && middlevalues[i] != null
                  && mainCanvas.Children.Contains(middlevalues[i]))
                    mainCanvas.Children.Remove(middlevalues[i]);
            }
            for (int i = 0; i < 2; i++)
            {
                if (!bottomused[i] && bottomvalues[i] != null 
                  && mainCanvas.Children.Contains(bottomvalues[i]))
                    mainCanvas.Children.Remove(bottomvalues[i]);
            }
            CommentArea.Children.Clear();
            for (int i = 0; i < maxcount; i++)
            {
                if (comments[i] == null) comments[i] = new TextBlock();
                CommentArea.Children.Add(comments[i]);
            }
            Update();
        }

        #region Shell

        private TextBlock[] middlevalues;
        private TextBlock[] bottomvalues;
        private TextBlock[] comments;

        public override bool IsCommentMode
        {
            set
            {
                base.IsCommentMode = value;
                CommentArea.Visibility = IsCommentMode ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public override void Update(int flags = 255)
        {
            base.Update(flags);
            switch (flags)
            {
                case UPDATE_PROPERTY:
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (LadderMode == LadderModes.Edit)
                        {
                            TopTextBlock.Text = Core.InstName;
                            if (Core.Type == LadderUnitModel.Types.CALLM)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    middlevalues[i].Text = "";
                                    comments[i].Text = "";
                                }
                            }
                            for (int i = 0; i < Core.Children.Count; i++)
                            {
                                ValueModel vmodel = Core.Children[i];
                                ValueFormat vformat = vmodel.Format;
                                string text = String.Format("{0:s}:{1:s}",
                                    vformat.Name, vmodel.Text);
                                if (vformat.Position >= 0)
                                    middlevalues[vformat.Position].Text = text;
                                else
                                    bottomvalues[-vformat.Position - 1].Text = text;
                                comments[i].Text = String.Format("{0:s}:{1:s}",
                                    vmodel.Text, vmodel.Comment);
                            }
                        }
                        else if (LadderMode == LadderModes.Simulate)
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
                    });
                    break;
            }
        }

        #endregion

    }
}
