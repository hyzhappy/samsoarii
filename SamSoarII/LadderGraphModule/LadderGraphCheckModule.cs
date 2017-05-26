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
    public class ErrorMessage
    {
        public ErrorType Error { get; set; }
        public IEnumerable<LadderNetworkViewModel> RefNetworks { get; set; } = new List<LadderNetworkViewModel>();
        public ErrorMessage(ErrorType Error, IEnumerable<LadderNetworkViewModel> RefNetworks)
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
    public class LadderGraphCheckModule
    {
        public static ErrorMessage Execute(LadderDiagramViewModel ladderDiagram)
        {
            ErrorType error = ErrorType.None;
            foreach (var network in ladderDiagram.GetNetworks().Where(x => { return !x.IsMasked; }))
            {
                error = CheckNetwork(network);
                if (error != ErrorType.None)
                {
                    List<LadderNetworkViewModel> templist = new List<LadderNetworkViewModel>();
                    templist.Add(network);
                    return new ErrorMessage(error,templist);
                }
                else
                {
                    network.InitializeLadderLogicModules();
                    LadderGraphRelocationModule.Execute(network);
                }
            }
            if (!CheckProgramControlInstructions(ladderDiagram))
            {
                error = ErrorType.InstPair;
            }
            return new ErrorMessage(error,null);
        }
        private static ErrorType CheckNetwork(LadderNetworkViewModel ladderNetwork)
        {
            ladderNetwork.PreCompile();
            if (ladderNetwork.LadderElements.Values.Count == 0 && ladderNetwork.LadderVerticalLines.Values.Count == 0)
            {
                return ErrorType.Empty;
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
            if (!CheckSpecialModel(ladderNetwork))
            {
                return ErrorType.Special;
            }
            return ErrorType.None;
        }
        private static bool IsLadderGraphShort(LadderNetworkViewModel ladderNetwork)
        {
            ladderNetwork.ClearSearchedFlag();
            var rootElements = ladderNetwork.LadderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(rootElements);
            while (tempQueue.Count > 0)
            {
                var ele = tempQueue.Dequeue();
                if (!ele.IsSearched)
                {
                    ele.IsSearched = true;
                    if (ele.Type != ElementType.Null && (!CheckLadderGraphShort(ladderNetwork,ele)))
                    {
                        return true;
                    }
                }
                foreach (var item in ele.NextElements)
                {
                    tempQueue.Enqueue(item);
                }
            }
            //return !(IsAllLinkedToRoot(ladderNetwork) && CheckSpecialModel(ladderNetwork) && CheckSelfLoop(ladderNetwork) && CheckHybridLink(ladderNetwork) && CheckElements(ladderNetwork));
            return false;
        }
        //短路检测
        private static bool CheckLadderGraphShort(LadderNetworkViewModel ladderNetwork,BaseViewModel checkmodel)
        {
            List<BaseViewModel> eles = checkmodel.NextElements;
            if (eles.Count == 1)
            {
                return true;
            }
            if (eles.Exists(x => { return x.Type == ElementType.Null; }) && eles.Count > 1)
            {
                ladderNetwork.ErrorModels.Clear();
                ladderNetwork.ErrorModels.Add(eles.First());//add error element
                return false;
            }
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(eles);
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
        private static bool CheckSelfLoop(LadderNetworkViewModel ladderNetwork)
        {
            var notHLines = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine; });
            //var needCheckElements = notHLines.Where(x => { return !(x.NextElemnets.Any(y => { return y.Type == ElementType.Null; })); });
            IEnumerable<BaseViewModel> hLinesOfNeedCheckElement;
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
        private static bool CheckHLines(LadderNetworkViewModel ladderNetwork,IEnumerable<BaseViewModel> hLines)
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
        private static IEnumerable<BaseViewModel> GetHLines(BaseViewModel model, LadderNetworkViewModel ladderNetwork)
        {
            var hLines = ladderNetwork.LadderElements.Values.Where(x => { return (x.Type == ElementType.HLine); });
            var tempList = new List<BaseViewModel>();
            foreach (var hLine in hLines)
            {
                if (IsRelativeToModel(ladderNetwork, model, hLine))
                {
                    tempList.Add(hLine);
                }
            }
            return tempList;
        }
        private static bool IsRelativeToModel(LadderNetworkViewModel ladderNetwork, BaseViewModel model, BaseViewModel hLine)
        {
            var relativePoints = GetRightRelativePoint(hLine.X, hLine.Y);
            //判断元素hLine是否与model相连
            return CheckRelativePoints(ladderNetwork, relativePoints, model, Direction.Right);
        }
        private static bool CheckRelativePoints(LadderNetworkViewModel ladderNetwork, IEnumerable<IntPoint> relativePoints, BaseViewModel model, Direction direction)
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
            BaseViewModel element;
            if (ladderNetwork.LadderElements.TryGetValue(p1, out element))
            {
                if (element == model)
                {
                    return true;
                }
                if (element.Type == ElementType.HLine)
                {
                    right = CheckRelativePoints(ladderNetwork,GetRightRelativePoint(p1.X, p1.Y), model, Direction.Right);
                }
            }
            VerticalLineViewModel verticalLine;
            if ((direction == Direction.Up) || (direction == Direction.Right))
            {
                if (ladderNetwork.LadderVerticalLines.TryGetValue(p2, out verticalLine))
                {
                    up = CheckRelativePoints(ladderNetwork,GetUpRelativePoint(p2.X, p2.Y), model, Direction.Up);
                }
            }
            if ((direction == Direction.Down) || (direction == Direction.Right))
            {
                if (ladderNetwork.LadderVerticalLines.TryGetValue(p3, out verticalLine))
                {
                    down = CheckRelativePoints(ladderNetwork,GetDownRelativePoint(p3.X, p3.Y), model, Direction.Down);
                }
            }
            //结果必须是三个方向的并
            return right || up || down;
        }
        //混联模块检测
        private static bool CheckHybridLink(LadderNetworkViewModel ladderNetwork)
        {
            //得到有多条支路的元素集合
            var needCheckElements = ladderNetwork.LadderElements.Values.Where(x => { return x.NextElements.Count > 1; });
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
                            var tempHashSet = new HashSet<BaseViewModel>(item1.SubElements);
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
                        var tempList = new List<BaseViewModel>(ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine; }));
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
        private static bool CheckSpecialModel(LadderNetworkViewModel ladderNetwork)
        {
            var allElements = ladderNetwork.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine; });
            var allSpecialModels = ladderNetwork.LadderElements.Values.Where(x => { return x.Type == ElementType.Special; });
            foreach (var specialmodel in allSpecialModels)
            {
                //定义特殊模块的所有子元素及其自身为一个结果集
                var subElementsOfSpecialModel = specialmodel.SubElements;
                var tempList = new List<BaseViewModel>(allElements);
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
        private static List<BaseViewModel> GetSubElements(BaseViewModel model)
        {
            if (model.IsSearched)
            {
                return model.SubElements;
            }
            List<BaseViewModel> result = new List<BaseViewModel>();
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
        //private static bool IsAllLinkedToRoot(LadderNetworkViewModel ladderNetwork)
        //{
        //    var rootElements = ladderNetwork.LadderElements.Values.Where(x => { return x.Type == ElementType.Output; });
        //    var tempList = new List<BaseViewModel>();
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
        //private static List<BaseViewModel> GetRootLinkedEles(LadderNetworkViewModel ladderNetwork,BaseViewModel rootElement)
        //{
        //    ladderNetwork.ClearSearchedFlag();
        //    List<BaseViewModel> tempList = new List<BaseViewModel>();
        //    Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(rootElement.NextElements);
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
        private static bool CheckElements(LadderNetworkViewModel ladderNetwork)
        {
            var tempElements = ladderNetwork.LadderElements.Values.ToList();
            for (int i = 0; i <= ladderNetwork.GetMaxY(); i++)
            {
                var tempList = tempElements.Where(x => { return x.Y == i; }).ToList();
                if (tempList.Count != 0 && tempList.Where(x => { return x.Type == ElementType.Output || x.X == 0; }).Count() == 0)
                {
                    tempList = tempElements.Where(x => { return x.Y == i && x.Type == ElementType.HLine; }).ToList();
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
        private static bool CheckHElement(LadderNetworkViewModel ladderNetwork,BaseViewModel model)
        {
            IntPoint p1 = new IntPoint();
            IntPoint p2 = new IntPoint();
            p1.X = model.X - 1;
            p1.Y = model.Y;
            p2.X = model.X - 1;
            p2.Y = model.Y - 1;
            if (ladderNetwork.LadderVerticalLines.ContainsKey(p1) && !ladderNetwork.LadderVerticalLines.ContainsKey(p2) && !ladderNetwork.LadderElements.ContainsKey(p1))
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
        private static bool IsLadderGraphOpen(LadderNetworkViewModel ladderNetwork)
        {
            ladderNetwork.ClearSearchedFlag();
            //首先检查元素的基本性质
            if (!Assert(ladderNetwork))
            {
                return true;
            }
            var rootElements = ladderNetwork.GetElements().Where(x => { return x.Type == ElementType.Output; });
            //检查上并联错误
            int MinY = rootElements.First().Y;
            foreach (var ele in rootElements)
            {
                if (ele.Y < MinY)
                {
                    MinY = ele.Y;
                }
            }
            var tempQueue = new Queue<BaseViewModel>(rootElements);
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
        private static bool Assert(LadderNetworkViewModel ladderNetwork)
        {
            return ladderNetwork.GetElements().All(x => 
            {
                if (!x.Assert())
                {
                    ladderNetwork.ErrorModels.Clear();
                    ladderNetwork.ErrorModels.Add(x);
                }
                return x.Assert();
            }) && CheckVerticalLines(ladderNetwork,ladderNetwork.GetVerticalLines());
        }
        private static bool CheckVerticalLines(LadderNetworkViewModel ladderNetwork,IEnumerable<VerticalLineViewModel> VLines)
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
        private static bool CheckProgramControlInstructions(LadderDiagramViewModel ladderDiagram)
        {
            List<BaseViewModel> eles = GetProgramControlViewModels(ladderDiagram);
            List<BaseViewModel> eles_for = eles.Where(x => { return x.GetType() == typeof(FORViewModel); }).ToList();
            List<BaseViewModel> eles_next = eles.Where(x => { return x.GetType() == typeof(NEXTViewModel); }).ToList();
            List<BaseViewModel> eles_jmp = eles.Where(x => { return x.GetType() == typeof(JMPViewModel); }).ToList();
            List<BaseViewModel> eles_lbl = eles.Where(x => { return x.GetType() == typeof(LBLViewModel); }).ToList();
            if (eles_for.Count != eles_next.Count || eles_jmp.Count != eles_lbl.Count)
            {
                return false;
            }
            else
            {
                foreach (var ele_jmp in eles_jmp)
                {
                    string lblindex = (ele_jmp.Model as JMPModel).LBLIndex.ToString();
                    if (!eles_lbl.Exists(x => { return (x.Model as LBLModel).LBLIndex.ToString() == lblindex; }))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        private static List<BaseViewModel> GetProgramControlViewModels(LadderDiagramViewModel ladderDiagram)
        {
            List<BaseViewModel> eles = new List<BaseViewModel>();
            foreach (var network in ladderDiagram.GetNetworks().Where(x => { return !x.IsMasked; }))
            {
                foreach (var model in network.GetElements().Where(x => { return x.GetType() == typeof(FORViewModel) || x.GetType() == typeof(NEXTViewModel) || x.GetType() == typeof(JMPViewModel) || x.GetType() == typeof(LBLViewModel); }))
                {
                    eles.Add(model);
                }
            }
            return eles;
        }
    }
}
