using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderGraphModule
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
        public LadderNetworkViewModel Parent { get; set; }
        public List<BaseViewModel> LadderElements { get; set; }
        public List<VerticalLineViewModel> LadderVerticalLines { get; set; }
        public LadderLogicModule(LadderNetworkViewModel parent, List<BaseViewModel> ladderElements, List<VerticalLineViewModel> ladderVerticalLines)
        {
            Parent = parent;
            LadderElements = ladderElements;
            LadderVerticalLines = ladderVerticalLines;
        }
        public BaseViewModel ReplaceElement(BaseViewModel model)
        {
            LadderElements.Add(model);
            var movedele = Parent.ReplaceEle(model);
            LadderElements.Remove(movedele);
            return movedele;
        }
        public void RemoveElement(int _x,int _y)
        {
            BaseViewModel removedmodel = null;
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
                Parent.RemoveEle(_x, _y);
            }
        }
        public void ReplaceVerticalLine(VerticalLineViewModel vline)
        {
            LadderVerticalLines.Add(vline);
            Parent.ReplaceVLine(vline);
        }
        public void RemoveVerticalLine(int _x, int _y)
        {
            VerticalLineViewModel removedvline = null;
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
                Parent.RemoveVLine(_x, _y);
            }
        }
    }
}
