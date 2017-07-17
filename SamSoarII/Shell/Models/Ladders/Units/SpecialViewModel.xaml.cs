using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// SpecialViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class SpecialViewModel : LadderUnitViewModel, IResource
    {
        #region IResource

        public override IResource Create(params object[] args)
        {
            return new SpecialViewModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            DataContext = this;
            if (Core?.Parent?.View != null)
            {
                LadderMode = Core.Parent.View.LadderMode;
                IsCommentMode = Core.Parent.View.IsCommentMode;
            }
            ReinitializeComponent();
        }

        #endregion

        public SpecialViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            Recreate(_core);
        }

        private void ReinitializeComponent()
        {
            Line line1 = null;
            Line line2 = null;
            Line line3 = null;
            switch (Core.InstName)
            {
                case "MEP":
                    line1 = new Line();
                    line1.X1 = 50;
                    line1.X2 = 50;
                    line1.Y1 = 0;
                    line1.Y2 = 100;
                    line1.Stroke = Brushes.Black;
                    line1.StrokeThickness = 4;
                    CenterCanvas.Children.Add(line1);
                    line2 = new Line();
                    line2.X1 = 50;
                    line2.X2 = 25;
                    line2.Y1 = 0;
                    line2.Y2 = 25;
                    line2.Stroke = Brushes.Black;
                    line2.StrokeThickness = 4;
                    CenterCanvas.Children.Add(line2);
                    line3 = new Line();
                    line3.X1 = 50;
                    line3.X2 = 75;
                    line3.Y1 = 0;
                    line3.Y2 = 25;
                    line3.Stroke = Brushes.Black;
                    line3.StrokeThickness = 4;
                    CenterCanvas.Children.Add(line3);
                    break;
                case "MEF":
                    line1 = new Line();
                    line1.X1 = 50;
                    line1.X2 = 50;
                    line1.Y1 = 0;
                    line1.Y2 = 100;
                    line1.Stroke = Brushes.Black;
                    line1.StrokeThickness = 4;
                    CenterCanvas.Children.Add(line1);
                    line2 = new Line();
                    line2.X1 = 50;
                    line2.X2 = 25;
                    line2.Y1 = 100;
                    line2.Y2 = 75;
                    line2.Stroke = Brushes.Black;
                    line2.StrokeThickness = 4;
                    CenterCanvas.Children.Add(line2);
                    line3 = new Line();
                    line3.X1 = 50;
                    line3.X2 = 75;
                    line3.Y1 = 100;
                    line3.Y2 = 75;
                    line3.Stroke = Brushes.Black;
                    line3.StrokeThickness = 4;
                    CenterCanvas.Children.Add(line3);
                    break;
                case "INV":
                    line1 = new Line();
                    line1.X1 = 75;
                    line1.Y1 = 25;
                    line1.X2 = 25;
                    line1.Y2 = 75;
                    line1.StrokeThickness = 4;
                    line1.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line1);
                    break;
            }
            Update();
        }

        public override void Dispose()
        {
            base.Dispose();
            DataContext = null;
            AllResourceManager.Dispose(this);
        }
    }
}
