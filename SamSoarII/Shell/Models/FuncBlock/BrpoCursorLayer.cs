using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
using System.Windows.Threading;
using System.Threading;
using System.Windows;

namespace SamSoarII.Shell.Models
{
    public class BrpoCursorLayer : Layer, IDisposable, IWeakEventListener
    {
        #region Resources

        #endregion

        public BrpoCursorLayer(FuncBlockModel _core, TextView _textview) : base(_textview, KnownLayer.BreakpointCursor)
        {
            this.core = _core;
            this.textview = _textview;
            cursor.PropertyChanged += OnCursorPropertyChanged;
            textview.InsertLayer(this, KnownLayer.BreakpointCursor, LayerInsertionPosition.Replace);
            TextViewWeakEventManager.VisualLinesChanged.AddListener(textview, this);
            TextViewWeakEventManager.ScrollOffsetChanged.AddListener(textview, this);
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
            cursor.PropertyChanged -= OnCursorPropertyChanged;
            this.core = null;
            this.textview = null;
        }

        #region Number

        private FuncBlockModel core;
        private TextView textview;
        private BreakpointCursor cursor { get { return core.Parent?.Parent.MNGSimu.Viewer.Cursor; } }

        #endregion

        #region Event Handler
        
        private void OnCursorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate () { InvalidateVisual(); });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (core.LadderMode != LadderModes.Simulate) return;

            if (cursor.Current is FuncBrpoModel)
            {
                FuncBrpoModel fbrpo = (FuncBrpoModel)(cursor.Current);
                if (fbrpo.Parent.Model == core)
                {
                    BackgroundGeometryBuilder cursorBuilder = new BackgroundGeometryBuilder();
                    cursorBuilder.AlignToMiddleOfPixels = true;
                    cursorBuilder.ExtendToFullWidthAtLineEnd = true;
                    cursorBuilder.CornerRadius = 1.0;
                    cursorBuilder.AddSegment(textview, new FuncBlockSegment(fbrpo.Parent));
                    Geometry cursorGeometry = cursorBuilder.CreateGeometry();
                    if (cursorGeometry != null)
                        drawingContext.DrawGeometry(Brushes.Yellow, new Pen(Brushes.Yellow, 1.0), cursorGeometry);
                }
            }
        }

        #endregion
    }
}
