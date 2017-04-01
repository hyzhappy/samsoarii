using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SamSoarII.Project;
using SamSoarII.InstructionViewModel;
using SamSoarII.Utility;
namespace SamSoarII.AppMain.UI
{
    public class LadderCanvas : Canvas
    {
        public SelectRect _selectRect = new SelectRect();
        public LadderDiagramModel LadderContext = null;
        public LadderCanvas()
        {
            Background = Brushes.Transparent;
            Children.Add(_selectRect);
            MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
            Focusable = true;
            // event
            PreviewKeyDown += MainCanvas_KeyDown;
            MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
        }
        
        private void MainCanvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int x = _selectRect.X;
            int y = _selectRect.Y;
            switch (e.Key)
            {
                case Key.F1:
                    var viewmodel1 = new LDViewModel() { X = x, Y = y };
                    AddElement(viewmodel1);
                    break;
                case Key.F2:
                    var viewmodel2 = new OUTViewModel() { X = x, Y = y };
                    AddElement(viewmodel2);
                    break;
                case Key.F3:
                    var vline = new VerticalLineViewModel() { X = x, Y = y };
                    AddVerticalLine(vline);
                    break;
                case Key.F4:
                    var hline = new HorizontalLineViewModel() { X = x, Y = y };
                    AddElement(hline);
                    break;
                case Key.F5:
                    Compile();
                    break;
                default:
                    break;
            }
        }

        public void LoadLadderDiagram(LadderDiagramModel ladmodel)
        {
            LadderContext = ladmodel;
            Children.Clear();
            foreach (var model in ladmodel.GetVerticalLines())
            {
                Children.Add(model);
            }
            foreach (var model in ladmodel.GetElements())
            {
                Children.Add(model);
            }
            Children.Add(_selectRect);
            _selectRect.Visibility = Visibility.Visible;
        }

        private void MainCanvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IntPoint p = Calc(e.GetPosition(this));
            _selectRect.X = p.X;
            _selectRect.Y = p.Y;
            Keyboard.Focus(this);
        }

        private IntPoint Calc(Point p)
        {
            IntPoint result = new IntPoint();
            result.X = ((int)p.X) / 300;
            result.Y = ((int)p.Y) / 300;
            return result;
        }

        public void AddElement(int index)
        {
            var viewmodel = InstructionViewModelPrototype.Clone(index);
            viewmodel.X = _selectRect.X;
            viewmodel.Y = _selectRect.Y;
            AddElement(viewmodel);
        }

        public void AddVerticalLine()
        {
            var vline = new VerticalLineViewModel();
            vline.X = _selectRect.X;
            vline.Y = _selectRect.Y;
            AddVerticalLine(vline);
        }

        private void AddElement(BaseViewModel viewmodel)
        {
            var old = LadderContext.AddElement(viewmodel);
            if (old != null)
            {
                Children.Remove(old);
            }
            Children.Add(viewmodel);
        }

        private void AddVerticalLine(VerticalLineViewModel vline)
        {
            if (LadderContext.AddVerticalLine(vline))
            {
                Children.Add(vline);
            }
        }

        private void Compile()
        {
            LadderContext.PreCompile();
            var graph = LadderContext.ConvertToGraph();
            graph.Convert();
            var tree = graph.ConvertToTree();
            tree.TreeName = "network1";
            MessageBox.Show(LadderContext.Assert().ToString());
            MessageBox.Show(tree.GenerateCode());
        }

        public void ClearElements()
        {
            Children.Clear();
        }
    }
}
