﻿using SamSoarII.Core.Models;
using SamSoarII.Global;
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
    /// describe:梯形图重摆模块
    /// </summary>
    public class LadderGraphRelocationModule
    {
        public enum DirectionStatus
        {
            Up_Dec,
            Up_Inc,
            Down_Dec,
            Down_Inc,
            None
        }
        public static void Execute(LadderNetworkModel ladderNetwork)
        {
            foreach (var KVPair in ladderNetwork.LadderLogicModules.OrderBy(x => { return x.Key; }))
            {
                HorizontalScan(KVPair.Value);
                VerticalScan(KVPair.Value);
            }
            RemoveEmptyLines(ladderNetwork);
        }
        /// <summary>
        /// describe:网络横向扫描
        /// </summary>
        /// <param name="ladderLogicModule">代表扫描的最小单元</param>
        private static void HorizontalScan(LadderLogicModule ladderLogicModule)
        {
            InitializeCountLevel(ladderLogicModule);
            if (ladderLogicModule.LadderVerticalLines.Count == 0)
            {
                //TODO movement
                Movement(ladderLogicModule);
            }
            //针对每一层级的VLine进行移动
            for (int level = 1; level < GlobalSetting.LadderXCapacity; level++)
            {
                var tempVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.CountLevel == level; }).ToList();
                if (tempVLines.Count() != 0)
                {
                    Movement(ladderLogicModule, tempVLines.OrderBy(v => { return v.Y; }).ToList());
                }
            }
            MoveResidueEle(ladderLogicModule);
        }
        /// <summary>
        /// describe:网络纵向扫描
        /// </summary>
        /// <param name="ladderLogicModule">代表扫描的最小单元</param>
        private static void VerticalScan(LadderLogicModule ladderLogicModule)
        {
            PreScan(ladderLogicModule);
            int emptyLineCnt = 0;
            int line = ladderLogicModule.endY + 1;
            for (int i = ladderLogicModule.startY; i < line; i++)
            {
                var tempList = ladderLogicModule.LadderElements.Where(x => { return x.Y == i; }).ToList();
                if (tempList.Count() == 0 || tempList.All(x => { return x.Type == LadderUnitModel.Types.HLINE; }))
                {
                    if (tempList.Count() != 0)
                    {
                        for (int j = 0; j < tempList.Count(); j++)
                        {
                            ladderLogicModule.RemoveElement(tempList[j].X, tempList[j].Y);
                        }
                    }
                    var VLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y <= i - 1 && x.Y >= i - 1 - emptyLineCnt; }).ToList();
                    for (int j = 0; j < VLines.Count(); j++)
                    {
                        ladderLogicModule.RemoveVerticalLine(VLines[j].X, VLines[j].Y);
                    }
                    emptyLineCnt++;
                }
                else
                {
                    MoveHorizontalLineEle(ladderLogicModule, emptyLineCnt, tempList);
                }
            }
            line = ladderLogicModule.endY + 1;
            for (int i = ladderLogicModule.startY; i < line; i++)
            {
                var VLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == i; }).OrderBy(x => { return x.X; }).ToList();
                MoveLadderBlocks(ladderLogicModule, VLines);
            }
        }
        private static void MoveHorizontalLineEle(LadderLogicModule ladderLogicModule, int index, List<LadderUnitModel> models)
        {
            LadderUnitModel model;
            LadderUnitModel VLine;
            int line = models[0].Y;
            if (index != 0)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    model = models[i];
                    ladderLogicModule.RemoveElement(model.X,model.Y);
                    model.Y -= index;
                    ladderLogicModule.ReplaceElement(model);
                }
                //对每一行，只保留其上一行的VLine(保持图的基本连通性)，并一起移动
                var VLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == line - 1; }).ToList();
                for (int i = 0; i < VLines.Count(); i++)
                {
                    VLine = VLines[i];
                    ladderLogicModule.RemoveVerticalLine(VLine.X,VLine.Y);
                    VLine.Y -= index;
                    ladderLogicModule.ReplaceVerticalLine(VLine);
                }
            }
        }
        private static void MoveLadderBlocks(LadderLogicModule ladderLogicModule, List<LadderUnitModel> VLines)
        {
            int indexY;
            List<LadderUnitModel> block;
            for (int i = 0; i < VLines.Count; i++)
            {
                if (i == 0)
                {
                    indexY = 1 + VLines[i].Y;
                    block = ladderLogicModule.LadderElements.Where(x => { return x.Y == indexY && x.X <= VLines[i].X; }).ToList();
                    while (block.Count() == 0)
                    {
                        if (ladderLogicModule.LadderVerticalLines.Exists(x => {return x.X == VLines[i].X && x.Y == indexY; }))
                        {
                            indexY++;
                            block = ladderLogicModule.LadderElements.Where(x => { return x.Y == indexY && x.X <= VLines[i].X; }).ToList();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (block.Count() > 0 && indexY > VLines[i].Y + 1)
                    {
                        MoveLadderBlock(ladderLogicModule, block, VLines[i].Y + 1);
                    }
                }
                if (i == VLines.Count - 1)
                {
                    indexY = 1 + VLines[i].Y;
                    block = ladderLogicModule.LadderElements.Where(x => { return x.Y == indexY && x.X > VLines[i].X; }).ToList();
                    while (block.Count() == 0)
                    {
                        if (ladderLogicModule.LadderVerticalLines.Exists(x => {return x.X == VLines[i].X && x.Y == indexY; }))
                        {
                            indexY++;
                            block = ladderLogicModule.LadderElements.Where(x => { return x.Y == indexY && x.X > VLines[i].X; }).ToList();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (block.Count() > 0 && indexY > VLines[i].Y + 1)
                    {
                        MoveLadderBlock(ladderLogicModule, block, VLines[i].Y + 1);
                    }
                }
                if (i < VLines.Count - 1)
                {
                    indexY = 1 + VLines[i].Y;
                    block = ladderLogicModule.LadderElements.Where(x => { return x.Y == indexY && x.X > VLines[i].X && x.X <= VLines[i + 1].X; }).ToList();
                    while (block.Count() == 0)
                    {
                        if (ladderLogicModule.LadderVerticalLines.Exists(x => { return x.X == VLines[i].X && x.Y == indexY; }) && ladderLogicModule.LadderVerticalLines.Exists(x => { return x.X == VLines[i + 1].X && x.Y == indexY; }))
                        {
                            indexY++;
                            block = ladderLogicModule.LadderElements.Where(x => { return x.Y == indexY && x.X > VLines[i].X && x.X <= VLines[i + 1].X; }).ToList();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (block.Count() > 0 && indexY > VLines[i].Y + 1)
                    {
                        MoveLadderBlock(ladderLogicModule, block, VLines[i].Y + 1);
                    }
                }
            }
        }
        private static void MoveLadderBlock(LadderLogicModule ladderLogicModule, IEnumerable<LadderUnitModel> block,int desY)
        {
            block = block.OrderBy(x => { return x.X; });
            int indexY = block.First().Y;
            int startX = block.First().X;
            int endX = block.Last().X;
            foreach (var ele in block)
            {
                ladderLogicModule.RemoveElement(ele.X,ele.Y);
                ele.Y = desY;
                ladderLogicModule.ReplaceElement(ele);
            }
            var tempVlines = new List<LadderUnitModel>(ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == indexY && x.X >= startX && x.X < endX; }));
            if (tempVlines.Count() > 0)
            {
                foreach (var VLine in tempVlines)
                {
                    for (int i = 1; i <= indexY - desY; i++)
                    {
                        ladderLogicModule.ReplaceVerticalLine(new LadderUnitModel(null, LadderUnitModel.Types.VLINE) { X = VLine.X, Y = VLine.Y - i });
                    }
                }
            }
            RemoveVLines(ladderLogicModule, desY, startX - 1, indexY - 1);
            RemoveVLines(ladderLogicModule, desY, endX, indexY - 1);
        }
        private static void RemoveVLines(LadderLogicModule ladderLogicModule, int desY,int x,int y)
        {
            IntPoint p = new IntPoint();
            LadderUnitModel vline;
            p.X = x;
            p.Y = y;
            while (ladderLogicModule.LadderVerticalLines.Exists(a => { return a.X == p.X && a.Y == p.Y; }))
            {
                vline = ladderLogicModule.LadderVerticalLines.Where(a => { return a.X == p.X && a.Y == p.Y; }).First();
                if (!ladderLogicModule.Parent.CheckVerticalLine(vline))
                {
                    ladderLogicModule.RemoveVerticalLine(vline.X,vline.Y);
                }
                p.Y--;
                if (p.Y < desY)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// describe:使网络中的逻辑单元之间不保留任何空行
        /// </summary>
        /// <param name="ladderLogicModule"></param>
        private static void PreScan(LadderLogicModule ladderLogicModule)
        {
            int minY = ladderLogicModule.startY;
            int key = ladderLogicModule.Parent.GetKeyByLadderLogicModule(ladderLogicModule);
            if (key == 0)
            {
                if (minY > 0)
                {
                    MoveAllElements(ladderLogicModule, minY);
                }
            }
            else
            {
                int upEndY = ladderLogicModule.Parent.GetLadderLogicModuleByKey(key - 1).endY;
                if (minY - upEndY > 1)
                {
                    MoveAllElements(ladderLogicModule, minY - upEndY - 1);
                }
            }
        }
        private static void MoveAllElements(LadderLogicModule ladderLogicModule, int index)
        {
            var allElements = new List<LadderUnitModel>(ladderLogicModule.LadderElements);
            allElements.AddRange(ladderLogicModule.LadderVerticalLines);
            foreach (var ele in allElements.OrderBy(x => { return x.Y; }))
            {
                if (ele.Shape == LadderUnitModel.Shapes.VLine)
                {
                    ladderLogicModule.RemoveVerticalLine(ele.X,ele.Y);
                    ele.Y -= index;
                    ladderLogicModule.ReplaceVerticalLine(ele as LadderUnitModel);
                }
                else
                {
                    ladderLogicModule.RemoveElement(ele.X,ele.Y);
                    ele.Y -= index;
                    ladderLogicModule.ReplaceElement(ele);
                }
            }
        }
        //移动每行最大层级VLine之后的元素
        private static void MoveResidueEle(LadderLogicModule ladderLogicModule)
        {
            for (int i = ladderLogicModule.startY; i <= ladderLogicModule.endY; i++)
            {
                var VLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == i - 1 || x.Y == i; }).OrderBy(x => { return x.CountLevel; });
                if (VLines.Count() > 0)
                {
                    var VLine = VLines.Last();
                    var tempList = ladderLogicModule.LadderElements.Where(
                        x => { return x.Shape != LadderUnitModel.Shapes.Output 
                                    && x.Shape != LadderUnitModel.Shapes.OutputRect
                                    && x.Shape != LadderUnitModel.Shapes.HLine 
                                    && x.Y == i && x.X > VLine.X; }).OrderBy(x => { return x.X; }).ToList();
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].X != j + VLine.X + 1)
                        {
                            LadderUnitModel HLine = new LadderUnitModel(null, LadderUnitModel.Types.HLINE);
                            HLine.X = tempList[j].X;
                            HLine.Y = tempList[j].Y;
                            var oldele = ladderLogicModule.ReplaceElement(HLine);
                            oldele.X = j + VLine.X + 1;
                            ladderLogicModule.ReplaceElement(oldele);
                        }
                    }
                }
            }
        }
        private static void Movement(LadderLogicModule ladderLogicModule)
        {
            var tempList = ladderLogicModule.LadderElements.Where(x => 
            { return x.Shape != LadderUnitModel.Shapes.Output 
                  && x.Shape != LadderUnitModel.Shapes.OutputRect
                  && x.Shape != LadderUnitModel.Shapes.HLine; }).OrderBy(x => { return x.X; }).ToList();
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].X != i)
                {
                    LadderUnitModel HLine = new LadderUnitModel(null, LadderUnitModel.Types.HLINE);
                    HLine.X = tempList[i].X;
                    HLine.Y = tempList[i].Y;
                    var oldele = ladderLogicModule.ReplaceElement(HLine);
                    oldele.X = i;
                    ladderLogicModule.ReplaceElement(oldele);
                }
            }
        }
        private static void Movement(LadderLogicModule ladderLogicModule, List<LadderUnitModel> VLines)
        {
            //移动之前先移动此层级和上一层级之间的元素
            MoveElements(ladderLogicModule, VLines[0].CountLevel);
            
            MoveVerticalLines(ladderLogicModule, VLines);
        }
        //检查VLine周边元素的分布
        private static DirectionStatus CheckVLine(LadderLogicModule ladderLogicModule, LadderUnitModel VLine)
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
            if (!ladderLogicModule.LadderElements.Exists(x => { return x.X == p1.X && x.Y == p1.Y; }) && ladderLogicModule.LadderElements.Exists(x => { return x.X == p2.X && x.Y == p2.Y; }))
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
            if (ladderLogicModule.LadderElements.Exists(x => { return x.X == p1.X && x.Y == p1.Y; }) && !ladderLogicModule.LadderElements.Exists(x => { return x.X == p2.X && x.Y == p2.Y; }))
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
            if (!ladderLogicModule.LadderElements.Exists(x => { return x.X == p1.X && x.Y == p1.Y; }) && ladderLogicModule.LadderElements.Exists(x => { return x.X == p2.X && x.Y == p2.Y; }))
            {
                return DirectionStatus.Down_Inc;
            }
            /*
             * Down_d:
             * direction:         <--
                                
                               |   -->        |
                     __________|          ____|
             */
            if (ladderLogicModule.LadderElements.Exists(x => { return x.X == p1.X && x.Y == p1.Y; }) && !ladderLogicModule.LadderElements.Exists(x => { return x.X == p2.X && x.Y == p2.Y; }))
            {
                return DirectionStatus.Down_Dec;
            }
            return DirectionStatus.None;
        }
        private static void MoveVerticalLines(LadderLogicModule ladderLogicModule, List<LadderUnitModel> VLines)
        {
            int cnt = 0;
            Dictionary<int, int> tempdic = new Dictionary<int, int>();
            for (int j = 0; j < VLines.Count(); j++)
            {
                cnt = GetCount(ladderLogicModule, VLines[j]);
                if(j > 0 && (cnt == 0 || IsLinked(VLines[j - 1],VLines[j])))
                    cnt = tempdic[j - 1];
                tempdic.Add(j, cnt);
                MoveVerticalLine(ladderLogicModule, VLines[j], cnt);
            }
        }
        private static bool IsLinked(LadderUnitModel vline1, LadderUnitModel vline2)
        {
            return (vline1.Y == vline2.Y - 1) && (vline1.X == vline2.X);
        }
        private static void MoveVerticalLine(LadderLogicModule ladderLogicModule, LadderUnitModel vLine, int cnt)
        {
            if (vLine.X != cnt - 1)
            {
                IntPoint point = new IntPoint();
                //大于cnt - 1则表示向前移，小于则向后移
                if (vLine.X > cnt - 1)
                {
                    //检查VLine周围元素的分布关系，判断是否在移动时需要添加或减少HLine
                    DirectionStatus status = CheckVLine(ladderLogicModule, vLine);
                    if (status == DirectionStatus.Down_Inc || status == DirectionStatus.Up_Inc)
                    {
                        if (status == DirectionStatus.Down_Inc)
                        {
                            point.Y = vLine.Y + 1;
                        }
                        else if (status == DirectionStatus.Up_Inc)
                        {
                            point.Y = vLine.Y;
                        }
                        for (int k = cnt; k <= vLine.X; k++)
                        {
                            point.X = k;
                            if (!ladderLogicModule.LadderElements.Exists(x => { return x.X == point.X && x.Y == point.Y; }))
                            {
                                LadderUnitModel HLine = new LadderUnitModel(null, LadderUnitModel.Types.HLINE);
                                HLine.X = point.X;
                                HLine.Y = point.Y;
                                ladderLogicModule.ReplaceElement(HLine);
                            }
                        }
                    }
                    if (status == DirectionStatus.Down_Dec || status == DirectionStatus.Up_Dec)
                    {
                        if (status == DirectionStatus.Down_Dec)
                        {
                            point.Y = vLine.Y + 1;
                        }
                        else
                        {
                            point.Y = vLine.Y;
                        }
                        for (int k = cnt; k <= vLine.X; k++)
                        {
                            point.X = k;
                            ladderLogicModule.RemoveElement(point.X, point.Y);
                        }
                    }
                }
                else
                {
                    for (int k = vLine.X + 1; k <= cnt - 1; k++)
                    {
                        point.X = k;
                        point.Y = vLine.Y + 1;
                        if (!ladderLogicModule.LadderElements.Exists(x => { return x.X == point.X && x.Y == point.Y; }))
                        {
                            LadderUnitModel HLine = new LadderUnitModel(null, LadderUnitModel.Types.HLINE);
                            HLine.X = point.X;
                            HLine.Y = point.Y;
                            ladderLogicModule.ReplaceElement(HLine);
                        }
                    }
                }
                ladderLogicModule.RemoveVerticalLine(vLine.X, vLine.Y);
                vLine.X = cnt - 1;
                ladderLogicModule.ReplaceVerticalLine(vLine);
            }
        }
        //移动相应层级之前的元素
        private static void MoveElements(LadderLogicModule ladderLogicModule, int countlevel)
        {
            for (int i = ladderLogicModule.startY; i <= ladderLogicModule.endY; i++)
            {
                var VLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.CountLevel == countlevel && x.Y == i - 1; }).OrderBy(x => { return x.CountLevel; }).ToList();
                if (VLines.Count == 0)
                {
                    VLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.CountLevel == countlevel && x.Y == i; }).OrderBy(x => { return x.CountLevel; }).ToList();
                }
                if (VLines.Count != 0)
                {
                    var VLine = VLines.First();
                    var tempVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.CountLevel < countlevel && (x.Y == i || x.Y == i - 1); }).OrderBy(x => { return x.CountLevel; }).ToList();
                    //若此层级VLine之前没有前一层级的VLine，则直接移动元素
                    if (tempVLines.Count == 0)
                    {
                        var tempList = ladderLogicModule.LadderElements.Where(x => 
                            { return x.Shape != LadderUnitModel.Shapes.Output 
                                  && x.Shape != LadderUnitModel.Shapes.OutputRect
                                  && x.Shape != LadderUnitModel.Shapes.HLine
                                  && x.Y == i && x.X <= VLine.X; }).OrderBy(x => { return x.X; }).ToList();
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            if (tempList[j].X != j)
                            {
                                LadderUnitModel HLine = new LadderUnitModel(null, LadderUnitModel.Types.HLINE);
                                HLine.X = tempList[j].X;
                                HLine.Y = tempList[j].Y;
                                var oldele = ladderLogicModule.ReplaceElement(HLine);
                                oldele.X = j;
                                ladderLogicModule.ReplaceElement(oldele);
                            }
                        }
                    }//否则，元素在两个层级VLine之间移动
                    else
                    {
                        var tempVLine = tempVLines.Last();
                        var tempList = ladderLogicModule.LadderElements.Where(x => 
                            { return x.Shape != LadderUnitModel.Shapes.Output
                                  && x.Shape != LadderUnitModel.Shapes.OutputRect
                                  && x.Shape != LadderUnitModel.Shapes.HLine
                                  && x.Y == i && x.X <= VLine.X && x.X > tempVLine.X; }).OrderBy(x => { return x.X; }).ToList();
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            if (tempList[j].X != j + tempVLine.X + 1)
                            {
                                LadderUnitModel HLine = new LadderUnitModel(null, LadderUnitModel.Types.HLINE);
                                HLine.X = tempList[j].X;
                                HLine.Y = tempList[j].Y;
                                var oldele = ladderLogicModule.ReplaceElement(HLine);
                                oldele.X = j + tempVLine.X + 1;
                                ladderLogicModule.ReplaceElement(oldele);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// describe:进行梯形图扫描前的核心操作,对逻辑单元中的VLine进行分层。
        /// </summary>
        /// <param name="ladderLogicModule"></param>
        private static void InitializeCountLevel(LadderLogicModule ladderLogicModule)
        {
            var tempElements = ladderLogicModule.LadderElements.Where(x => { return x.Type != LadderUnitModel.Types.HLINE; }).ToList();
            for (int i = 0; i < GlobalSetting.LadderXCapacity - 1; i++)
            {
                var tempVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.X == i; }).OrderBy(x => { return x.Y; });//进行层级分配时，是从上到下扫描
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
                        var upVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == VLine.Y - 1 && x.X <= VLine.X; });
                        var downVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X; });
                        var eqVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == VLine.Y && x.X < VLine.X; });
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
                                downVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == VLine.Y + 1 + cnt && x.X <= VLine.X; });
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
                                    if (ladderLogicModule.LadderElements.Where(x => { return x.Y == downVLine.Y && x.X > downVLine.X && x.X <= VLine.X; }).Count() == VLine.X - downVLine.X)
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
                        downVLines = ladderLogicModule.LadderVerticalLines.Where(x => { return x.Y == VLine.Y + cnt && x.X <= VLine.X; });
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
        private static int GetCount(LadderLogicModule ladderLogicModule, LadderUnitModel VLine)
        {
            int cnt = 0, tempCnt = 0;
            var tempEle = ladderLogicModule.LadderElements.Where(x =>
                { return x.Shape != LadderUnitModel.Shapes.Output && x.Shape != LadderUnitModel.Shapes.OutputRect && x.Shape != LadderUnitModel.Shapes.HLine; });
            var templist = ladderLogicModule.LadderVerticalLines.Where(x => { return x.CountLevel < VLine.CountLevel; });
            cnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X; }).Count();
            tempCnt = tempEle.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X; }).Count();
            cnt = Math.Max(tempCnt, cnt);
            //前一层级的VLine有三个方向，上，同一层，下
            var tempList1 = templist.Where(x => { return x.Y == VLine.Y - 1; });
            var tempList2 = templist.Where(x => { return x.Y == VLine.Y; });
            var tempList3 = templist.Where(x => { return x.Y == VLine.Y + 1; });
            bool isSuccess = false;
            if (tempList1.Count() != 0)
            {
                var tempVLine1 = tempList1.OrderBy(x => { return x.CountLevel; }).Last();
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X && x.X > tempVLine1.X; }).Count();
                if (tempCnt > 0)
                {
                    cnt = Math.Max(tempCnt + tempVLine1.X + 1, cnt);
                    isSuccess = true;
                }
            }
            if (tempList2.Count() != 0)
            {
                var tempVLine2 = tempList2.OrderBy(x => { return x.CountLevel; }).Last();
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X && x.X > tempVLine2.X; }).Count();
                if (tempCnt > 0)
                {
                    cnt = Math.Max(tempCnt + tempVLine2.X + 1, cnt);
                    isSuccess = true;
                }
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X && x.X > tempVLine2.X; }).Count();
                if (tempCnt > 0)
                {
                    cnt = Math.Max(tempCnt + tempVLine2.X + 1, cnt);
                    isSuccess = true;
                }
            }
            if (tempList3.Count() != 0)
            {
                var tempVLine3 = tempList3.OrderBy(x => { return x.CountLevel; }).Last();
                tempCnt = tempEle.Where(x => { return x.Y == VLine.Y + 1 && x.X <= VLine.X && x.X > tempVLine3.X; }).Count();
                if (tempCnt > 0)
                {
                    cnt = Math.Max(tempCnt + tempVLine3.X + 1, cnt);
                    isSuccess = true;
                }
            }
            if (cnt == 0 && VLine.Y == ladderLogicModule.startY)
            {
                cnt = tempEle.Where(x => { return x.Y == VLine.Y && x.X <= VLine.X; }).Count();
            }
            if (!isSuccess && templist.Count() > 0) cnt = 0;
            return cnt;
        }
        private static void RemoveEmptyLines(LadderNetworkModel ladderNetwork)
        {
            if(ladderNetwork.RowCount != ladderNetwork.GetMaxY() + 1)
                ladderNetwork.RowCount = ladderNetwork.GetMaxY() + 1;
        }
    }
}
