// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Base class for known layers.
	/// </summary>
	public class Layer : UIElement
	{
        /// <summary> TextView </summary>
		protected readonly TextView textView;
        /// <summary> KnownLayer </summary>
		protected readonly KnownLayer knownLayer;

        /// <summary> Create a new Layer </summary>
        public Layer(TextView textView, KnownLayer knownLayer)
		{
			Debug.Assert(textView != null);
			this.textView = textView;
			this.knownLayer = knownLayer;
			this.Focusable = false;
		}

        /// <summary> HitTest </summary>
        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return null;
		}

        /// <summary> HitTest </summary>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}

        /// <summary> Event : OnRender </summary>
        protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			textView.RenderBackground(drawingContext, knownLayer);
		}
	}
}
