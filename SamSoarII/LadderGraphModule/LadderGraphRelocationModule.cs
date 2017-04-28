using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderGraphModule
{
    public class LadderGraphRelocationModule
    {
        public static void Execute(LadderNetworkViewModel ladderNetwork)
        {
            HorizontalScan(ladderNetwork);
            VerticalScan(ladderNetwork);
            RemoveEmptyLines(ladderNetwork);
        }
        private static void HorizontalScan(LadderNetworkViewModel ladderNetwork)
        {
            InitializeCountLevel(ladderNetwork);
            int MaxLine = ladderNetwork.GetMaxY() + 1;
            if (ladderNetwork.LadderVerticalLines.Values.Count == 0)
            {
                //TODO movement
                Movement(ladderNetwork);
            }
            //针对每一层级的VLine进行移动
            for (int level = 1; level < GlobalSetting.LadderXCapacity; level++)
            {
                var tempVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.CountLevel == level; }).ToList();
                if (tempVLines.Count() != 0)
                {
                    Movement(ladderNetwork,tempVLines);
                }
            }
            MoveResidueEle(ladderNetwork);
        }
        private static void VerticalScan(LadderNetworkViewModel ladderNetwork)
        {
            PreScan(ladderNetwork);
            int emptyLineCnt = 0;
            int line = ladderNetwork.GetMaxY() + 1;
            for (int i = 0; i < line; i++)
            {
                var tempList = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == i; }).ToList();
                if (tempList.Count() == 0 || tempList.All(x => { return x.Type == ElementType.HLine; }))
                {
                    if (tempList.Count() != 0)
                    {
                        for (int j = 0; j < tempList.Count(); j++)
                        {
                            ladderNetwork.RemoveElement(tempList[j]);
                        }
                    }
                    var VLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y <= i - 1 && x.Y >= i - 1 - emptyLineCnt; }).ToList();
                    for (int j = 0; j < VLines.Count(); j++)
                    {
                        ladderNetwork.RemoveVerticalLine(VLines[j]);
                    }
                    emptyLineCnt++;
                }
                else
                {
                    MoveHorizontalLineEle(ladderNetwork,emptyLineCnt, tempList);
                }
            }
            line = ladderNetwork.GetMaxY() + 1;
            for (int i = 0; i < line; i++)
            {
                var VLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == i; }).OrderBy(x => { return x.X; }).ToList();
                MoveLadderBlocks(ladderNetwork,VLines);
            }
        }
        private static void MoveHorizontalLineEle(LadderNetworkViewModel ladderNetwork,int index, List<BaseViewModel> models)
        {
            BaseViewModel model;
            VerticalLineViewModel VLine;
            int line = models[0].Y;
            if (index != 0)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    model = models[i];
                    ladderNetwork.RemoveElement(model);
                    model.Y -= index;
                    ladderNetwork.ReplaceElement(model);
                }
                //对每一行，只保留其上一行的VLine(保持图的基本连通性)，并一起移动
                var VLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == line - 1; }).ToList();
                for (int i = 0; i < VLines.Count(); i++)
                {
                    VLine = VLines[i];
                    ladderNetwork.RemoveVerticalLine(VLine);
                    VLine.Y -= index;
                    ladderNetwork.ReplaceVerticalLine(VLine);
                }
            }
        }
        private static void MoveLadderBlocks(LadderNetworkViewModel ladderNetwork,List<VerticalLineViewModel> VLines)
        {
            int indexY;
            List<BaseViewModel> block;
            for (int i = 0; i < VLines.Count; i++)
            {
                if (i == 0)
                {
                    indexY = 1 + VLines[i].Y;
                    block = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == indexY && x.X <= VLines[i].X; }).ToList();
                    while (block.Count() == 0)
                    {
                        if (ladderNetwork.LadderVerticalLines.Keys.ToList().Exists(x => { return x.X == VLines[i].X && x.Y == indexY; }))
                        {
                            indexY++;
                            block = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == indexY && x.X <= VLines[i].X; }).ToList();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (block.Count() > 0 && indexY > VLines[i].Y + 1)
                    {
                        MoveLadderBlock(ladderNetwork, block, VLines[i].Y + 1);
                    }
                }
                if (i == VLines.Count - 1)
                {
                    indexY = 1 + VLines[i].Y;
                    block = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == indexY && x.X > VLines[i].X; }).ToList();
                    while (block.Count() == 0)
                    {
                        if (ladderNetwork.LadderVerticalLines.Keys.ToList().Exists(x => { return x.X == VLines[i].X && x.Y == indexY; }))
                        {
                            indexY++;
                            block = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == indexY && x.X > VLines[i].X; }).ToList();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (block.Count() > 0 && indexY > VLines[i].Y + 1)
                    {
                        MoveLadderBlock(ladderNetwork, block, VLines[i].Y + 1);
                    }
                }
                if (i < VLines.Count - 1)
                {
                    indexY = 1 + VLines[i].Y;
                    block = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == indexY && x.X > VLines[i].X && x.X <= VLines[i + 1].X; }).ToList();
                    while (block.Count() == 0)
                    {
                        if (ladderNetwork.LadderVerticalLines.Keys.ToList().Exists(x => { return x.X == VLines[i].X && x.Y == indexY; }) && ladderNetwork.LadderVerticalLines.Keys.ToList().Exists(x => { return x.X == VLines[i + 1].X && x.Y == indexY; }))
                        {
                            indexY++;
                            block = ladderNetwork.LadderElements.Values.Where(x => { return x.Y == indexY && x.X > VLines[i].X && x.X <= VLines[i + 1].X; }).ToList();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (block.Count() > 0 && indexY > VLines[i].Y + 1)
                    {
                        MoveLadderBlock(ladderNetwork, block, VLines[i].Y + 1);
                    }
                }
            }
        }
        private static void MoveLadderBlock(LadderNetworkViewModel ladderNetwork,IEnumerable<BaseViewModel> block,int desY)
        {
            block = block.OrderBy(x => { return x.X; });
            int indexY = block.First().Y;
            int startX = block.First().X;
            int endX = block.Last().X;
            foreach (var ele in block)
            {
                ladderNetwork.RemoveElement(ele);
                ele.Y = desY;
                ladderNetwork.ReplaceElement(ele);
            }
            var tempVlines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == indexY && x.X >= startX && x.X < endX; });
            if (tempVlines.Count() > 0)
            {
                foreach (var VLine in tempVlines)
                {
                    for (int i = 1; i <= indexY - desY; i++)
                    {
                        ladderNetwork.ReplaceVerticalLine(new VerticalLineViewModel() { X = VLine.X, Y = VLine.Y - i });
                    }
                }
            }
            RemoveVLines(ladderNetwork, desY, startX - 1, indexY - 1);
            RemoveVLines(ladderNetwork, desY, endX, indexY - 1);
        }
        private static void RemoveVLines(LadderNetworkViewModel ladderNetwork,int desY,int x,int y)
        {
            IntPoint p = new IntPoint();
            VerticalLineViewModel vline;
            p.X = x;
            p.Y = y;
            while (ladderNetwork.LadderVerticalLines.TryGetValue(p, out vline))
            {
                if (!ladderNetwork.CheckVerticalLine(vline))
                {
                    ladderNetwork.RemoveVerticalLine(vline);
                }
                p.Y--;
                if (p.Y < desY)
                {
                    break;
                }
            }
        }
        private static void PreScan(LadderNetworkViewModel ladderNetwork)
        {
            var rootElements = ladderNetwork.LadderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            int minY = rootElements.First().Y;
            foreach (var rootElement in rootElements)
            {
                if (rootElement.Y < minY)
                {
                    minY = rootElement.Y;
                }
            }
            if (minY > 0)
            {
                MoveAllElements(ladderNetwork,minY);
            }
        }
        private static void MoveAllElements(LadderNetworkViewModel ladderNetwork,int index)
        {
            var allElements = new List<BaseViewModel>(ladderNetwork.LadderElements.Values);
            allElements.AddRange(ladderNetwork.LadderVerticalLines.Values);
            foreach (var ele in allElements)
            {
                if (ele.Type == ElementType.VLine)
                {
                    ladderNetwork.RemoveVerticalLine(ele.X,ele.Y);
                    ele.Y -= index;
                    ladderNetwork.ReplaceVerticalLine(ele as VerticalLineViewModel);
                }
                else
                {
                    ladderNetwork.RemoveElement(ele.X,ele.Y);
                    ele.Y -= index;
                    ladderNetwork.ReplaceElement(ele);
                }
            }
        }
        //移动每行最大层级VLine之后的元素
        private static void MoveResidueEle(LadderNetworkViewModel ladderNetwork)
        {
            for (int i = 0; i <= ladderNetwork.GetMaxY(); i++)
            {
                var VLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == i - 1 || x.Y == i; }).OrderBy(x => { return x.CountLevel; });
                if (VLines.Count() > 0)
                {
                    var VLine = VLines.Last();
                    var tempList = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.Output && x.Type != ElementType.HLine && x.Y == i && x.X > VLine.X; }).OrderBy(x => { return x.X; }).ToList();
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].X != j + VLine.X + 1)
                        {
                            HorizontalLineViewModel HLine = new HorizontalLineViewModel();
                            HLine.X = tempList[j].X;
                            HLine.Y = tempList[j].Y;
                            var oldele = ladderNetwork.ReplaceElement(HLine);
                            oldele.X = j + VLine.X + 1;
                            ladderNetwork.ReplaceElement(oldele);
                        }
                    }
                }
            }
        }
        private static void Movement(LadderNetworkViewModel ladderNetwork)
        {
            var tempList = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.Output && x.Type != ElementType.HLine; }).OrderBy(x => { return x.X; }).ToList();
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].X != i)
                {
                    HorizontalLineViewModel HLine = new HorizontalLineViewModel();
                    HLine.X = tempList[i].X;
                    HLine.Y = tempList[i].Y;
                    var oldele = ladderNetwork.ReplaceElement(HLine);
                    oldele.X = i;
                    ladderNetwork.ReplaceElement(oldele);
                }
            }
        }
        private static void Movement(LadderNetworkViewModel ladderNetwork,List<VerticalLineViewModel> VLines)
        {
            //移动之前先移动此层级和上一层级之间的元素
            MoveElements(ladderNetwork,VLines[0].CountLevel);
            //为确保相同层级的VLine的X坐标相同，计算同一层级中前面所需的最大元素间隔数量
            int cnt = GetCount(ladderNetwork,VLines[0]);
            for (int i = 1; i < VLines.Count; i++)
            {
                int temp = GetCount(ladderNetwork,VLines[i]);
                if (cnt < temp)
                {
                    cnt = temp;
                }
            }
            MoveVerticalLines(ladderNetwork,VLines, cnt);
        }
        //检查VLine周边元素的分布
        private static DirectionStatus CheckVLine(LadderNetworkViewModel ladderNetwork,VerticalLineViewModel VLine)
        {
            IntPoint p = new IntPoint();
            IntPoint p1 = new IntPoint();
            IntPoint p2 = new IntPoint();
            p.X = VLine.X;
            p.Y = VLine.Y;
            p1.X = VLine.X;
            p1.Y = VLine.Y;
            p2.X = VLine.X + 1;
            p2.Y = VLine.Y;
            /*
             * Up_p:    
             * direction:         <--     
                     __________            ______________
                    |              -->    |  
                    |                     |
             */
            if (!ladderNetwork.LadderElements.ContainsKey(p1) && ladderNetwork.LadderElements.ContainsKey(p2))
            {
                return DirectionStatus.Up_Inc;
            }
            /*
             * Up_d:
             * direction:         <--
                     __________           ____
                               |   -->        |
                               |              |
             */
            if (ladderNetwork.LadderElements.ContainsKey(p1) && !ladderNetwork.LadderElements.ContainsKey(p2))
            {
                return DirectionStatus.Up_Dec;
            }
            p1.Y += 1;
            p2.Y += 1;
            /*
             * Down_p:
             * direction:         <--    
                                 
                    |              -->    |  
                    |__________           |______________
             */
            if (!ladderNetwork.LadderElements.ContainsKey(p1) && ladderNetwork.LadderElements.ContainsKey(p2))
            {
                return DirectionStatus.Down_Inc;
            }
            /*
             * Down_d:    
             * direction:         <--     
                                
                               |   -->        |    
                     __________|          ____|
             */
            if (ladderNetwork.LadderElements.ContainsKey(p1) && !ladderNetwork.LadderElements.ContainsKey(p2))
            {
                return DirectionStatus.Down_Dec;
            }
            return DirectionStatus.None;
        }
        private static void MoveVerticalLines(LadderNetworkViewModel ladderNetwork,List<VerticalLineViewModel> VLines, int cnt)
        {
            for (int j = 0; j < VLines.Count(); j++)
            {
                var tempVLine = VLines.ElementAt(j);
                if (tempVLine.X != cnt - 1)
                {
                    IntPoint point = new IntPoint();
                    //大于cnt - 1则表示向前移，小于则向后移
                    if (tempVLine.X > cnt - 1)
                    {
                        //检查VLine周围元素的分布关系，判断是否在移动时需要添加或减少HLine
                        DirectionStatus status = CheckVLine(ladderNetwork,tempVLine);
                        if (status == DirectionStatus.Down_Inc || status == DirectionStatus.Up_Inc)
                        {
                            if (status == DirectionStatus.Down_Inc)
                            {
                                point.Y = tempVLine.Y + 1;
                            }
                            else if (status == DirectionStatus.Up_Inc)
                            {
                                point.Y = tempVLine.Y;
                            }
                            for (int k = cnt; k <= tempVLine.X; k++)
                            {
                                point.X = k;
                                if (!ladderNetwork.LadderElements.ContainsKey(point))
                                {
                                    HorizontalLineViewModel HLine = new HorizontalLineViewModel();
                                    HLine.X = point.X;
                                    HLine.Y = point.Y;
                                    ladderNetwork.ReplaceElement(HLine);
                                }
                            }
                        }
                        if (status == DirectionStatus.Down_Dec || status == DirectionStatus.Up_Dec)
                        {
                            if (status == DirectionStatus.Down_Dec)
                            {
                                point.Y = tempVLine.Y + 1;
                            }
                            else
                            {
                                point.Y = tempVLine.Y;
                            }
                            for (int k = cnt; k <= tempVLine.X; k++)
                            {
                                point.X = k;
                                ladderNetwork.RemoveElement(point);
                            }
                        }
                    }
                    else
                    {
                        for (int k = tempVLine.X + 1; k <= cnt - 1; k++)
                        {
                            point.X = k;
                            point.Y = tempVLine.Y + 1;
                            if (!ladderNetwork.LadderElements.ContainsKey(point))
                            {
                                HorizontalLineViewModel HLine = new HorizontalLineViewModel();
                                HLine.X = point.X;
                                HLine.Y = point.Y;
                                ladderNetwork.ReplaceElement(HLine);
                            }
                        }
                    }
                    ladderNetwork.RemoveVerticalLine(tempVLine);
                    tempVLine.X = cnt - 1;
                    ladderNetwork.ReplaceVerticalLine(tempVLine);
                }
            }
        }
        //移动相应层级之前的元素
        private static void MoveElements(LadderNetworkViewModel ladderNetwork,int countlevel)
        {
            for (int i = 0; i <= ladderNetwork.GetMaxY(); i++)
            {
                var VLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.CountLevel == countlevel && x.Y == i - 1; }).OrderBy(x => { return x.CountLevel; }).ToList();
                if (VLines.Count == 0)
                {
                    VLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.CountLevel == countlevel && x.Y == i; }).OrderBy(x => { return x.CountLevel; }).ToList();
                }
                if (VLines.Count != 0)
                {
                    var VLine = VLines.First();
                    var tempVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.CountLevel < countlevel && (x.Y == i || x.Y == i - 1); }).OrderBy(x => { return x.CountLevel; }).ToList();
                    //若此层级VLine之前没有前一层级的VLine，则直接移动元素
                    if (tempVLines.Count == 0)
                    {
                        var tempList = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine && x.Type != ElementType.Output && x.Y == i && x.X <= VLine.X; }).OrderBy(x => { return x.X; }).ToList();
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            if (tempList[j].X != j)
                            {
                                HorizontalLineViewModel HLine = new HorizontalLineViewModel();
                                HLine.X = tempList[j].X;
                                HLine.Y = tempList[j].Y;
                                var oldele = ladderNetwork.ReplaceElement(HLine);
                                oldele.X = j;
                                ladderNetwork.ReplaceElement(oldele);
                            }
                        }
                    }//否则，元素在两个层级VLine之间移动
                    else
                    {
                        var tempVLine = tempVLines.Last();
                        var tempList = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine && x.Type != ElementType.Output && x.Y == i && x.X <= VLine.X && x.X > tempVLine.X; }).OrderBy(x => { return x.X; }).ToList();
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            if (tempList[j].X != j + tempVLine.X + 1)
                            {
                                HorizontalLineViewModel HLine = new HorizontalLineViewModel();
                                HLine.X = tempList[j].X;
                                HLine.Y = tempList[j].Y;
                                var oldele = ladderNetwork.ReplaceElement(HLine);
                                oldele.X = j + tempVLine.X + 1;
                                ladderNetwork.ReplaceElement(oldele);
                            }
                        }
                    }
                }
            }
        }
        //对网络中的VLine进行分层，即若两条VLine之间存在非HLine元素，则后者level是前者中最大的level加一
        private static void InitializeCountLevel(LadderNetworkViewModel ladderNetwork)
        {
            var tempElements = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine; }).ToList();
            for (int i = 0; i < GlobalSetting.LadderXCapacity - 1; i++)
            {
                var tempVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.X == i; }).OrderBy(x => { return x.Y; });//进行层级分配时，是从上到下扫描
                if (i == 0)
                {
                    foreach (var VLine in tempVLines)
                    {
                        VLine.CountLevel = 1;//处在第一列的VLine其层级为1
                    }
                }
                else
                {
                    foreach (var VLine in tempVLines)
                    {
                        int tempCountLevel = 0;
                        int cnt = 1;
                        var upVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y - 1 && x.X <= VLine.X; });
                        var downVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X; });
                        var eqVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y && x.X < VLine.X; });
                        //由于扫描从上至下，故如下情况时，需移动downVLine
                        /* _______________________
                         *                  |_____
                         *                  |
                         * _________________|
                         * ...
                         */
                        if (downVLines.Count() != 0)
                        {
                            var downVLine = downVLines.OrderBy(x => { return x.X; }).Last();
                            var tempVLine = downVLines.OrderBy(x => { return x.CountLevel; }).Last();
                            //在向下移动时，需要记录经过的VLine中的最大层级数
                            if (tempVLine.CountLevel > 0)
                            {
                                if (tempElements.Exists(x => { return x.X > tempVLine.X && x.X <= VLine.X && x.Y == tempVLine.Y; }))
                                {
                                    tempCountLevel = tempVLine.CountLevel + 1;
                                }
                                else
                                {
                                    tempCountLevel = tempVLine.CountLevel;
                                }
                            }
                            while (downVLine.CountLevel == 0 && downVLine.X == VLine.X)
                            {
                                downVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y + 1 + cnt && x.X <= VLine.X; });
                                if (downVLines.Count() != 0)
                                {
                                    downVLine = downVLines.OrderBy(x => { return x.X; }).Last();
                                    tempVLine = downVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                    if (tempVLine.CountLevel > 0)
                                    {
                                        if (tempElements.Exists(x => { return x.X > tempVLine.X && x.X <= VLine.X && x.Y == tempVLine.Y; }))
                                        {
                                            tempCountLevel = Math.Max(tempCountLevel, tempVLine.CountLevel + 1);
                                        }
                                        else
                                        {
                                            tempCountLevel = Math.Max(tempCountLevel, tempVLine.CountLevel);
                                        }
                                    }
                                    /*下面的判断条件为:
                                     * _____________________
                                     * ________________|
                                     * __________|  |__|    ←   只能移动到这
                                     * __________|          ←× 错误
                                     * 
                                     */
                                    if (ladderNetwork.LadderElements.Values.Where(x => { return x.Y == downVLine.Y && x.X > downVLine.X && x.X <= VLine.X; }).Count() == VLine.X - downVLine.X)
                                    {
                                        cnt++;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        downVLines = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y + cnt && x.X <= VLine.X; });
                        if (upVLines.Count() == 0 && downVLines.Count() == 0 && eqVLines.Count() == 0)
                        {
                            VLine.CountLevel = 1;
                        }
                        else
                        {
                            if (upVLines.Count() != 0)
                            {
                                var upVLine = upVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                if (tempElements.Exists(x => { return x.X > upVLine.X && x.X <= VLine.X && x.Y == VLine.Y; }))
                                {
                                    tempCountLevel = Math.Max(tempCountLevel, upVLine.CountLevel + 1);
                                }
                                else
                                {
                                    tempCountLevel = Math.Max(tempCountLevel, upVLine.CountLevel);
                                }
                                if (downVLines.Count() != 0)
                                {
                                    var downVLine = downVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                    if (tempElements.Exists(x => { return x.X > downVLine.X && x.X <= VLine.X && x.Y == downVLine.Y; }))
                                    {
                                        tempCountLevel = Math.Max(tempCountLevel, downVLine.CountLevel + 1);
                                    }
                                    else
                                    {
                                        tempCountLevel = Math.Max(tempCountLevel, downVLine.CountLevel);
                                    }
                                }
                                if (eqVLines.Count() != 0)
                                {
                                    var eqVLine = eqVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                    if (!tempElements.Exists(x => { return x.X > eqVLine.X && x.X <= VLine.X && (x.Y == VLine.Y || x.Y == VLine.Y + 1); }))
                                    {
                                        tempCountLevel = Math.Max(tempCountLevel, eqVLine.CountLevel);
                                    }
                                    else
                                    {
                                        tempCountLevel = Math.Max(tempCountLevel, eqVLine.CountLevel + 1);
                                    }
                                }
                            }
                            else if (downVLines.Count() != 0)
                            {
                                var downVLine = downVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                if (tempElements.Exists(x => { return x.X > downVLine.X && x.X <= VLine.X && x.Y == downVLine.Y; }))
                                {
                                    tempCountLevel = Math.Max(tempCountLevel, downVLine.CountLevel + 1);
                                }
                                else
                                {
                                    tempCountLevel = Math.Max(tempCountLevel, downVLine.CountLevel);
                                }
                                if (eqVLines.Count() != 0)
                                {
                                    var eqVLine = eqVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                    if (!tempElements.Exists(x => { return x.X > eqVLine.X && x.X <= VLine.X && (x.Y == VLine.Y || x.Y == VLine.Y + 1); }))
                                    {
                                        tempCountLevel = Math.Max(tempCountLevel, eqVLine.CountLevel);
                                    }
                                    else
                                    {
                                        tempCountLevel = Math.Max(tempCountLevel, eqVLine.CountLevel + 1);
                                    }
                                }
                                else
                                {
                                    if (downVLine.CountLevel == 0)
                                    {
                                        tempCountLevel = 1;
                                    }
                                }
                            }
                            else
                            {
                                var eqVLine = eqVLines.OrderBy(x => { return x.CountLevel; }).Last();
                                if (!tempElements.Exists(x => { return x.X > eqVLine.X && x.X <= VLine.X && (x.Y == VLine.Y || x.Y == VLine.Y + 1); }))
                                {
                                    tempCountLevel = Math.Max(tempCountLevel, eqVLine.CountLevel);
                                }
                                else
                                {
                                    tempCountLevel = Math.Max(tempCountLevel, eqVLine.CountLevel + 1);
                                }
                            }
                            VLine.CountLevel = tempCountLevel;
                        }
                    }
                }
            }
        }
        private static int GetCount(LadderNetworkViewModel ladderNetwork,VerticalLineViewModel VLine)
        {
            int cnt;
            var tempEle = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.Output && x.Type != ElementType.HLine; });
            //前一层级的VLine有三个方向，上，同一层，下
            var tempList1 = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y - 1 && x.CountLevel < VLine.CountLevel; });
            var tempList2 = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y && x.CountLevel < VLine.CountLevel; });
            var tempList3 = ladderNetwork.LadderVerticalLines.Values.Where(x => { return x.Y == VLine.Y + 1 && x.CountLevel < VLine.CountLevel; });
            cnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X; }).Count();
            int tempCnt = 0;
            tempCnt = tempEle.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X; }).Count();
            cnt = Math.Max(cnt,tempCnt);
            if (tempList1.Count() != 0)
            {
                var tempVLine1 = tempList1.OrderBy(x => { return x.CountLevel; }).Last();
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X && x.X > tempVLine1.X; }).Count() + tempVLine1.X + 1;
                if (cnt < tempCnt)
                {
                    cnt = tempCnt;
                }
            }
            if (tempList2.Count() != 0)
            {
                var tempVLine2 = tempList2.OrderBy(x => { return x.CountLevel; }).Last();
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X && x.X > tempVLine2.X; }).Count() + tempVLine2.X + 1;
                if (cnt < tempCnt)
                {
                    cnt = tempCnt;
                }
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X && x.X > tempVLine2.X; }).Count() + tempVLine2.X + 1;
                if (cnt < tempCnt)
                {
                    cnt = tempCnt;
                }
            }
            if (tempList3.Count() != 0)
            {
                var tempVLine3 = tempList3.OrderBy(x => { return x.CountLevel; }).Last();
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X && x.X > tempVLine3.X; }).Count() + tempVLine3.X + 1;
                if (cnt < tempCnt)
                {
                    cnt = tempCnt;
                }
            }
            return cnt;
        }
        private static void RemoveEmptyLines(LadderNetworkViewModel ladderNetwork)
        {
            ladderNetwork.RowCount = ladderNetwork.GetMaxY() + 1;
        }
    }
}
