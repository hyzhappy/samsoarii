using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;
using SamSoarII.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using System.Collections.Specialized;
using System.Windows;

namespace SamSoarII.Shell.Models
{
    public class BrpoLayer : Layer, IDisposable, IWeakEventListener
    {
        public BrpoLayer(FuncBlockModel _core, TextView _textview) : base(_textview, KnownLayer.BreakpointLabel)
        {
            this.core = _core;
            this.textview = _textview;
            items = new ObservableCollection<FuncBlock>();
            core.PropertyChanged += OnCorePropertyChanged;
            core.BreakpointPropertyChanged += OnCoreBrpoPropertyChanged;
            textview.InsertLayer(this, KnownLayer.BreakpointLabel, LayerInsertionPosition.Replace);
            TextViewWeakEventManager.VisualLinesChanged.AddListener(textview, this);
            TextViewWeakEventManager.ScrollOffsetChanged.AddListener(textview, this);
            Initialize(core.Root);
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextViewWeakEventManager.VisualLinesChanged)
                || managerType == typeof(TextViewWeakEventManager.ScrollOffsetChanged))
            {
                InvalidateVisual();
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            core.PropertyChanged -= OnCorePropertyChanged;
            core.BreakpointPropertyChanged -= OnCoreBrpoPropertyChanged;
            this.core = null;
            this.textview = null;
            foreach (FuncBlock item in items.ToArray())
                items.Remove(item);
            items = null;
        }

        #region Number

        private FuncBlockModel core;
        private TextView textview;
        private FuncBlockViewModel view { get { return core?.View; } }
        private TextArea textarea { get { return view?.CodeTextBox.TextArea; } }
        private ObservableCollection<FuncBlock> items;

        #endregion

        private void Initialize(FuncBlock fblock)
        {
            if (fblock.BPEnable) items.Add(fblock);
            foreach (FuncBlock sub in fblock.Childrens)
                Initialize(sub);
        }

        #region Event Handler

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LadderMode":
                    InvalidateVisual();
                    break;
            }
        }

        private void OnCoreBrpoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is FuncBlock)
            {
                FuncBlock fblock = (FuncBlock)sender;
                switch (e.PropertyName)
                {
                    case "BPEnable":
                        if (fblock.BPEnable && !items.Contains(fblock)) items.Add(fblock);
                        if (!fblock.BPEnable && items.Contains(fblock)) items.Remove(fblock);
                        InvalidateVisual();
                        break;
                    case "BPActive":
                        InvalidateVisual();
                        break;
                }
            }
        }
        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (core.LadderMode != LadderModes.Simulate) return;

            BackgroundGeometryBuilder unactiveBuilder = new BackgroundGeometryBuilder();
            unactiveBuilder.AlignToMiddleOfPixels = true;
            unactiveBuilder.ExtendToFullWidthAtLineEnd = true;
            unactiveBuilder.CornerRadius = 1.0;
            foreach (FuncBlock fblock in items.Where(fb => !fb.BPActive))
                unactiveBuilder.AddSegment(textview, new FuncBlockSegment(fblock));
            Geometry unactiveGeometry = unactiveBuilder.CreateGeometry();
            if (unactiveGeometry != null)
                drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Red, 1.0), unactiveGeometry);
            
            BackgroundGeometryBuilder activeBuilder = new BackgroundGeometryBuilder();
            activeBuilder.AlignToMiddleOfPixels = true;
            activeBuilder.ExtendToFullWidthAtLineEnd = true;
            activeBuilder.CornerRadius = 1.0;
            foreach (FuncBlock fblock in items.Where(fb => fb.BPActive))
                activeBuilder.AddSegment(textview, new FuncBlockSegment(fblock));
            Geometry activeGeometry = activeBuilder.CreateGeometry();
            if (activeGeometry != null)
                drawingContext.DrawGeometry(Brushes.Red, new Pen(Brushes.Red, 1.0), activeGeometry);
        }
        
        #endregion

    }
}
