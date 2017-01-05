using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SamSoarII.Project;
namespace SamSoarII.AppMain.UI
{
    public class LadderTabItem : BaseTabItem
    {
        private LadderCanvas _ladderCanvas = new LadderCanvas();

        public override string TabName { get { return Header.ToString(); } }

        public LadderTabItem(LadderDiagramModel model, ScaleTransform scale)
        {
            Content = _ladderCanvas;
            BindingOperations.SetBinding(this, TabItem.HeaderProperty, new Binding() { Source = model, Path = new System.Windows.PropertyPath("Name"), Mode = BindingMode.OneWay });
            _ladderCanvas.LoadLadderDiagram(model);
            _ladderCanvas.LayoutTransform = scale;
            MinWidth = 100;
        }

        public void AddElement(int index)
        {
            _ladderCanvas.AddElement(index);
        }

        public void AddVerticalLine()
        {
            _ladderCanvas.AddVerticalLine();
        }

        private void ItemGetKeyboardFocus()
        {
            _ladderCanvas.Focus();
            Keyboard.Focus(_ladderCanvas);
        }

        public override void ClearElements()
        {
            _ladderCanvas.ClearElements();
        }

        public override void OnItemSelected()
        {
            ItemGetKeyboardFocus();
        }
    }
}
