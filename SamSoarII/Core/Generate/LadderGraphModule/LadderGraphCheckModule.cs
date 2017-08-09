using SamSoarII.Core.Models;
using SamSoarII.Global;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Generate
{
    public class ErrorMessage
    {
        public ErrorType Error { get; set; }
        public IEnumerable<LadderNetworkModel> RefNetworks { get; set; } = new List<LadderNetworkModel>();
        public ErrorMessage(ErrorType Error, IEnumerable<LadderNetworkModel> RefNetworks)
        {
            this.Error = Error;
            this.RefNetworks = RefNetworks;
        }
    }
    public enum ErrorType
    {
        Open,
        Short,
        SelfLoop,
        HybridLink,
        InstPair,
        Special,
        Empty,
        None
    }
    /// <summary>
    /// author:zheyuYang
    /// describe:梯形图检查模块
    /// </summary>
    public class LadderGraphCheckModule
    {
        public enum Direction { Up, Down, Left, Right}

        public static ErrorMessage Execute(LadderDiagramModel ladderDiagram)
        {
            ErrorType error = ErrorType.None;
            ladderDiagram.ClearUndoRedoAction();
            foreach (var network in ladderDiagram.Children.Where(x => { return !x.IsMasked; }))
            {
                error = CheckNetwork(network);
                if (error != ErrorType.None)
                {
                    List<LadderNetworkModel> templist = new List<LadderNetworkModel>();
                    templist.Add(network);
                    return new ErrorMessage(error,templist);
                }
                else
                {
                    network.InitializeLadderLogicModules();
                    if (network.View != null && network.View.IsSingleSelected()) network.View.ReleaseSelectRect();
                    LadderGraphRelocationModule.Execute(network);
                }
            }
            return new ErrorMessage(error,null);
        }
        private static ErrorType CheckNetwork(LadderNetworkModel ladderNetwork)
        {
            ladderNetwork.PreCompile();
            if (ladderNetwork.Children.Count() == 0 && ladderNetwork.VLines.Count() == 0)
            {
                return ErrorType.None;
            }
            if (IsLadderGraphOpen(ladderNetwork))
            {
                return ErrorType.Open;
            }
            //if (!IsAllLinkedToRoot(ladderNetwork))
            //{
            //    return ErrorType.Open;
            //}
            if (IsLadderGraphShort(ladderNetwork))
            {
                return ErrorType.Short;
            }
            if (!CheckSelfLoop(ladderNetwork))
            {
                return ErrorType.SelfLoop;
            }
            if (!CheckElements(ladderNetwork))
            {
                return ErrorType.Special;
            }
            if (!CheckHybridLink(ladderNetwork))
            {
                return ErrorType.HybridLink;
            }
            //if (!CheckSpecialModel(ladderNetwork))
            //{
            //    return ErrorType.Special;
            //}
            return ErrorType.None;
        }
        private static bool IsLadderGraphShort(LadderNetworkModel ladderNetwork)
        {
            ladderNetwork.ClearSearchedFlag();
            var rootElements = ladderNetwork.Children.Where(x => x.Shape == LadderUnitModel.Shapes.Output || x.Shape == LadderUnitModel.Shapes.OutputRect);
            Queue<LadderUnitModel> tempQueue = new Queue<LadderUnitModel>(rootElements);
            while (tempQueue.Count > 0)
            {
                var ele = tempQueue.Dequeue();
                if (!ele.IsSearched)
                {
                    ele.IsSearched = true;
                    if (ele.Type != LadderUnitModel.Types.NULL && (!CheckLadderGraphShort(ladderNetwork, ele)))
                    {
                        return true;
                    }
                }
                foreach (var item in ele.NextElements)
                {
                    tempQueue.Enqueue(item);
                }
            }
            return false;
        }
        //短路检测
        private static bool CheckLadderGraphShort(LadderNetworkModel ladderNetwork, LadderUnitModel checkmodel)
        {
            List<LadderUnitModel> eles = checkmodel.NextElements;
            if (eles.Count == 1)
            {
                return true;
            }
            if (eles.Exists(x => { return x.Type == LadderUnitModel.Types.NULL; }) && eles.Count > 1)
            {
                ladderNetwork.ErrorModels.Clear();
                ladderNetwork.ErrorModels.Add(eles.First());//add error element
                return false;
            }
            Queue<LadderUnitModel> tempQueue = new Queue<LadderUnitModel>(eles);
            while (tempQueue.Count > 0)
            {
                var ele = tempQueue.Dequeue();
                if (eles.Intersect(ele.NextElements).Count() > 0)
                {
                    ladderNetwork.ErrorModels.Clear();
                    ladderNetwork.ErrorModels.Add(ele);//add error element
                    return false;
                }
                else
                {
                    foreach (var item in ele.NextElements)
                    {
                        tempQueue.Enqueue(item);
                    }
                }
            }
            return true;
        }
        //自环检测
        private static bool CheckSelfLoop(LadderNetworkModel ladderNetwork)
        {
            var notHLines = ladderNetwork.Children.Where(x => { return x.Type != LadderUnitModel.Types.HLINE; });
            //var needCheckElements = notHLines.Where(x => { return !(x.NextElemnets.Any(y => { return y.Type == ElementType.Null; })); });
            IEnumerable<LadderUnitModel> hLinesOfNeedCheckElement;
            foreach (var needCheckElement in notHLines)
            {
                hLinesOfNeedCheckElement = GetHLines(needCheckElement, ladderNetwork);
                if (!CheckHLines(ladderNetwork,hLinesOfNeedCheckElement))
                {
                    return false;
                }
            }
            return true;
        }
        //判断集合中X坐标相同的直线，其NextElemnets集合是否完全相同
        private static bool CheckHLines(LadderNetworkModel ladderNetwork,IEnumerable<LadderUnitModel> hLines)
        {
            for (int x = 0; x < GlobalSetting.LadderXCapacity; x++)
            {
                var lines = hLines.Where(l => { return l.X == x; });
                if (lines.Count() > 1)
                {
                    for (int j = 0; j < lines.Count(); j++)
                    {
                        for (int k = j + 1; k < lines.Count(); k++)
                        {
                            if (lines.ElementAt(j).NextElements.SequenceEqual(lines.ElementAt(k).NextElements))
                            {
                                ladderNetwork.ErrorModels.Clear();
                                ladderNetwork.ErrorModels.Add(lines.ElementAt(j));//add error element
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        //得到与model直连的所有HLine
        private static IEnumerable<LadderUnitModel> GetHLines(LadderUnitModel model, LadderNetworkModel ladderNetwork)
        {
            var hLines = ladderNetwork.Children.Where(x => { return (x.Type == LadderUnitModel.Types.HLINE); });
            var tempList = new List<LadderUnitModel>();
            foreach (var hLine in hLines)
            {
                if (IsRelativeToModel(ladderNetwork, model, hLine))
                {
                    tempList.Add(hLine);
                }
            }
            return tempList;
        }
        private static bool IsRelativeToModel(LadderNetworkModel ladderNetwork, LadderUnitModel model, LadderUnitModel hLine)
        {
            var relativePoints = GetRightRelativePoint(hLine.X, hLine.Y);
            //判断元素hLine是否与model相连
            return CheckRelativePoints(ladderNetwork, relativePoints, model, Direction.Right);
        }
        private static bool CheckRelativePoints(LadderNetworkModel ladderNetwork, IEnumerable<IntPoint> relativePoints, LadderUnitModel model, Direction direction)
        {
            IntPoint p1 = new IntPoint();//right
            IntPoint p2 = new IntPoint();//up
            IntPoint p3 = new IntPoint();//down
            bool right = false;
            bool up = false;
            bool down = false;
            if (direction == Direction.Right)
            {
                p1 = relativePoints.ElementAt(0);
                p2 = relativePoints.ElementAt(1);
                p3 = relativePoints.ElementAt(2);
            }
            else if (direction == Direction.Up)
            {
                p2 = relativePoints.ElementAt(0);
                p1 = relativePoints.ElementAt(1);
            }
            else
            {
                p3 = relativePoints.ElementAt(0);
                p1 = relativePoints.ElementAt(1);
            }
            LadderUnitModel element = ladderNetwork.Children[p1.X, p1.Y];
            if (element != null)
            {
                if (element == model)
                {
                    return true;
                }
                if (element.Type == LadderUnitModel.Types.HLINE)
                {
                    right = CheckRelativePoints(ladderNetwork,GetRightRelativePoint(p1.X, p1.Y), model, Direction.Right);
                }
            }
            LadderUnitModel verticalLine;
            if ((direction == Direction.Up) || (direction == Direction.Right))
            {
                verticalLine = ladderNetwork.VLines[p2.X, p2.Y];
                if (verticalLine != null)
                {
                    up = CheckRelativePoints(ladderNetwork,GetUpRelativePoint(p2.X, p2.Y), model, Direction.Up);
                }
            }
            if ((direction == Direction.Down) || (direction == Direction.Right))
            {
                verticalLine = ladderNetwork.VLines[p3.X, p3.Y];
                if (verticalLine != null)
                {
                    down = CheckRelativePoints(ladderNetwork,GetDownRelativePoint(p3.X, p3.Y), model, Direction.Down);
                }
            }
            //结果必须是三个方向的并
            return right || up || down;
        }
        //混联模块检测
        private static bool CheckHybridLink(LadderNetworkModel ladderNetwork)
        {
            //得到有多条支路的元素集合
            var needCheckElements = ladderNetwork.Children.Where(x => { return x.NextElements.Count > 1 && x.Type != LadderUnitModel.Types.HLINE; }).ToList();
            foreach (var ele in needCheckElements)
            {
                for (int i = 0; i < ele.NextElements.Count; i++)
                {
                    for (int j = i + 1; j < ele.NextElements.Count; j++)
                    {
                        //取其中任意两条支路并得到其交集
                        var item1 = ele.NextElements.ElementAt(i);
                        var item2 = ele.NextElements.ElementAt(j);
                        var tempPublicEle = item1.SubElements.Intersect(item2.SubElements);
                        int cnt = tempPublicEle.Count();
                        if (cnt == 1)
                        {
                            ladderNetwork.ErrorModels.Clear();
                            ladderNetwork.ErrorModels.Add(tempPublicEle.First());//add error element
                            return false;
                        }
                        //若交集元素大于0说明存在环
                        if (cnt > 0)
                        {
                            //得到环中的所有元素
                            var tempHashSet = new HashSet<LadderUnitModel>(item1.SubElements);
                            tempHashSet.UnionWith(item2.SubElements);
                            foreach (var item in tempPublicEle)
                            {
                                tempHashSet.Remove(item);
                            }
                            //对于环中的任一元素的子元素集合，其与两个支路交集(tempPublicEle)相交的元素个数小于两个支路交集的元素个数
                            foreach (var item in tempHashSet)
                            {
                                if (item.SubElements.Intersect(tempPublicEle).Count() < cnt)
                                {
                                    ladderNetwork.ErrorModels.Clear();
                                    ladderNetwork.ErrorModels.Add(tempPublicEle.First());//add error element
                                    return false;
                                }
                            }
                        }
                        //得到除直线外，且除去ele子元素的集合。
                        var tempList = new List<LadderUnitModel>(ladderNetwork.Children.Where(x => { return x.Type != LadderUnitModel.Types.HLINE; }));
                        foreach (var item in ele.SubElements)
                        {
                            tempList.Remove(item);
                        }
                        //遍历该集合，计算任一元素的子元素集合与ele子元素的集合的交集元素个数
                        foreach (var item in tempList)
                        {
                            int cnt1 = item.SubElements.Intersect(ele.SubElements).Count();
                            if (cnt1 > cnt && cnt1 < ele.SubElements.Count - 1)
                            {
                                ladderNetwork.ErrorModels.Clear();
                                ladderNetwork.ErrorModels.Add(item2);//add error element
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        //特殊模块检测
        private static bool CheckSpecialModel(LadderNetworkModel ladderNetwork)
        {
            var allElements = ladderNetwork.Children.Where(x => { return x.Type != LadderUnitModel.Types.HLINE; });
            var allSpecialModels = ladderNetwork.Children.Where(x => { return x.Shape == LadderUnitModel.Shapes.Special; });
            foreach (var specialmodel in allSpecialModels)
            {
                //定义特殊模块的所有子元素及其自身为一个结果集
                var subElementsOfSpecialModel = specialmodel.SubElements;
                var tempList = new List<LadderUnitModel>(allElements);
                //得到除去结果集的元素集合
                foreach (var ele in subElementsOfSpecialModel)
                {
                    if (tempList.Contains(ele))
                    {
                        tempList.Remove(ele);
                    }
                }
                //得到包含特殊模块的所有父元素集合
                var list = tempList.Where(x => { return x.SubElements.Contains(specialmodel); });
                foreach (var ele in tempList)
                {
                    var subElementsOfEle = ele.SubElements;
                    //结果集之外的任一元素与该结果集相交的元素集合
                    int count = subElementsOfEle.Intersect(subElementsOfSpecialModel).Count();
                    //其数量若大于0且小于整个结果集，且属于特殊模块的父元素的子元素
                    if (count < subElementsOfSpecialModel.Count && count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (item.SubElements.Contains(ele))
                            {
                                ladderNetwork.ErrorModels.Clear();
                                ladderNetwork.ErrorModels.Add(specialmodel);//add error element
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        //得到所有子元素，包括自身和NULL元素。
        private static List<LadderUnitModel> GetSubElements(LadderUnitModel model)
        {
            if (model.IsSearched)
            {
                return model.SubElements;
            }
            List<LadderUnitModel> result = new List<LadderUnitModel>();
            foreach (var ele in model.NextElements)
            {
                var tempList = GetSubElements(ele);
                foreach (var item in tempList)
                {
                    if (!result.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }
            result.Add(model);
            model.SubElements = result;
            model.IsSearched = true;
            return result;
        }
        //检测根元素的非NULL元素集合是否相交
        //private static bool IsAllLinkedToRoot(LadderNetworkModel ladderNetwork)
        //{
        //    var rootElements = ladderNetwork.LadderElements.Values.Where(x => { return x.Type == ElementType.Output; });
        //    var tempList = new List<LadderUnitModel>();
        //    tempList = GetRootLinkedEles(ladderNetwork, rootElements.ElementAt(0));
        //    for (int x = 1; x < rootElements.Count(); x++)
        //    {
        //        tempList = tempList.Intersect(GetRootLinkedEles(ladderNetwork,rootElements.ElementAt(x))).ToList();
        //        if (tempList.Count == 0)
        //        {
        //            ladderNetwork.ErrorModels.Clear();
        //            ladderNetwork.ErrorModels.Add(rootElements.First());//add error element
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        //得到与根元素相关的最后一个非NULL元素集合
        //private static List<LadderUnitModel> GetRootLinkedEles(LadderNetworkModel ladderNetwork,LadderUnitModel rootElement)
        //{
        //    ladderNetwork.ClearSearchedFlag();
        //    List<LadderUnitModel> tempList = new List<LadderUnitModel>();
        //    Queue<LadderUnitModel> tempQueue = new Queue<LadderUnitModel>(rootElement.NextElements);
        //    while (tempQueue.Count > 0)
        //    {
        //        var ele = tempQueue.Dequeue();
        //        if (!ele.IsSearched)
        //        {
        //            ele.IsSearched = true;
        //            if (ele.NextElements.Exists(x => { return x.Type == ElementType.Null; }))
        //            {
        //                tempList.Add(ele);
        //            }
        //            else
        //            {
        //                foreach (var item in ele.NextElements)
        //                {
        //                    tempQueue.Enqueue(item);
        //                }
        //            }
        //        }
        //    }
        //    return tempList;
        //}
        private static bool CheckElements(LadderNetworkModel ladderNetwork)
        {
            var tempElements = ladderNetwork.Children.ToList();
            for (int i = 0; i <= ladderNetwork.RowCount - 1; i++)
            {
                var tempList = tempElements.Where(x => { return x.Y == i; }).ToList();
                if (tempList.Count != 0 && tempList.Where(x => { return x.Shape == LadderUnitModel.Shapes.Output || x.Shape == LadderUnitModel.Shapes.OutputRect || x.X == 0; }).Count() == 0)
                {
                    tempList = tempElements.Where(x => { return x.Y == i && x.Type == LadderUnitModel.Types.HLINE; }).ToList();
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (!CheckHElement(ladderNetwork,tempList[j]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private static bool CheckHElement(LadderNetworkModel ladderNetwork,LadderUnitModel model)
        {
            IntPoint p1 = new IntPoint();
            IntPoint p2 = new IntPoint();
            p1.X = model.X - 1;
            p1.Y = model.Y;
            p2.X = model.X - 1;
            p2.Y = model.Y - 1;
            if (ladderNetwork.VLines[p1.X, p1.Y] != null && 
                ladderNetwork.VLines[p2.X, p2.Y] == null && 
                ladderNetwork.Children[p1.X, p1.Y] == null)
            {
                ladderNetwork.ErrorModels.Clear();
                ladderNetwork.ErrorModels.Add(model);//add error element
                return false;
            }
            else
            {
                return true;
            }
        }
        private static bool IsLadderGraphOpen(LadderNetworkModel ladderNetwork)
        {
            ladderNetwork.ClearSearchedFlag();
            //首先检查元素的基本性质
            if (!Assert(ladderNetwork))
            {
                return true;
            }
            var rootElements = ladderNetwork.Children.Where(x => { return x.Shape == LadderUnitModel.Shapes.Output || x.Shape == LadderUnitModel.Shapes.OutputRect; });
            //检查上并联错误
            int MinY = rootElements.First().Y;
            foreach (var ele in rootElements)
            {
                if (ele.Y < MinY)
                {
                    MinY = ele.Y;
                }
            }
            var tempQueue = new Queue<LadderUnitModel>(rootElements);
            while (tempQueue.Count > 0)
            {
                var item = tempQueue.Dequeue();
                if (!item.IsSearched)
                {
                    item.IsSearched = true;
                    if (item.Y < MinY)
                    {
                        ladderNetwork.ErrorModels.Clear();
                        ladderNetwork.ErrorModels.Add(item);
                        return true;
                    }
                    foreach (var ele in item.NextElements)
                    {
                        tempQueue.Enqueue(ele);
                    }
                }
            }
            return false;
        }
        private static bool Assert(LadderNetworkModel ladderNetwork)
        {
            return ladderNetwork.Children.All(x => 
            {
                if (!x.Assert())
                {
                    ladderNetwork.ErrorModels.Clear();
                    ladderNetwork.ErrorModels.Add(x);
                }
                return x.Assert();
            }) && CheckVerticalLines(ladderNetwork,ladderNetwork.VLines);
        }
        private static bool CheckVerticalLines(LadderNetworkModel ladderNetwork,IEnumerable<LadderUnitModel> VLines)
        {
            foreach (var VLine in VLines)
            {
                if (!ladderNetwork.CheckVerticalLine(VLine))
                {
                    ladderNetwork.ErrorModels.Clear();
                    ladderNetwork.ErrorModels.Add(VLine);
                    return false;
                }
            }
            return true;
        }
        private static IEnumerable<IntPoint> GetUpRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为上,右
            tempList.Add(new IntPoint() { X = x, Y = y - 1 });
            tempList.Add(new IntPoint() { X = x + 1, Y = y });
            return tempList;
        }
        private static IEnumerable<IntPoint> GetDownRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为下，右
            tempList.Add(new IntPoint() { X = x, Y = y + 1 });
            tempList.Add(new IntPoint() { X = x + 1, Y = y + 1 });
            return tempList;
        }
        private static IEnumerable<IntPoint> GetRightRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为右，上，下
            tempList.Add(new IntPoint() { X = x + 1, Y = y });
            tempList.Add(new IntPoint() { X = x, Y = y - 1 });
            tempList.Add(new IntPoint() { X = x, Y = y });
            return tempList;
        }
    }
}
