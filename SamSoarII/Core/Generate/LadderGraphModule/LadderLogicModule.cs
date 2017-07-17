using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Generate
{
    /// <summary>
    /// author:zheyuYang
    /// describe:代表梯形图中的最小逻辑单元,不可分割!
    /// </summary>
    public class LadderLogicModule
    {
        public int startY
        {
            get
            {
                return LadderElements.OrderBy(x => { return x.Y; }).First().Y;
            }
        }
        public int endY
        {
            get
            {
                return LadderElements.OrderBy(x => { return x.Y; }).Last().Y;
            }
        }
        public LadderNetworkModel Parent { get; set; }
        public List<LadderUnitModel> LadderElements { get; set; }
        public List<LadderUnitModel> LadderVerticalLines { get; set; }
        public LadderLogicModule(LadderNetworkModel parent, List<LadderUnitModel> ladderElements, List<LadderUnitModel> ladderVerticalLines)
        {
            Parent = parent;
            LadderElements = ladderElements;
            LadderVerticalLines = ladderVerticalLines;
        }
        public LadderUnitModel ReplaceElement(LadderUnitModel model)
        {
            LadderElements.Add(model);
            var movedele = Parent.Children[model.X, model.Y];
            Parent.Add(model);
            LadderElements.Remove(movedele);
            return movedele;
        }
        public void RemoveElement(int _x,int _y)
        {
            LadderUnitModel removedmodel = null;
            foreach (var ele in LadderElements)
            {
                if (ele.X == _x && ele.Y == _y)
                {
                    removedmodel = ele;
                    break;
                }
            }
            if (removedmodel != null)
            {
                LadderElements.Remove(removedmodel);
                Parent.Remove(removedmodel);
            }
        }
        public void ReplaceVerticalLine(LadderUnitModel vline)
        {
            LadderVerticalLines.Add(vline);
            Parent.AddV(vline);
        }
        public void RemoveVerticalLine(int _x, int _y)
        {
            LadderUnitModel removedvline = null;
            foreach (var vline in LadderVerticalLines)
            {
                if (vline.X == _x && vline.Y == _y)
                {
                    removedvline = vline;
                    break;
                }
            }
            if (removedvline != null)
            {
                LadderVerticalLines.Remove(removedvline);
                Parent.RemoveV(removedvline);
            }
        }
    }
}
