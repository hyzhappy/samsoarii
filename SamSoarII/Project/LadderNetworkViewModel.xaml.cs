using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.PLCCompiler;
using SamSoarII.UserInterface;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// LadderNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderNetworkViewModel : UserControl, IComparable
    {

        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }

        private int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }

        private int _rowCount;
        public int RowCount
        {
            get
            {
                return _rowCount;
            }
            set
            {
                if(value > 0)
                {
                    _rowCount = value;
                    LadderCanvas.Height = _rowCount * HeightUnit;
                }
            }
        }

        private int _networkNumber;
        public int NetworkNumber
        {
            get
            {
                return _networkNumber;
            }
            set
            {
                _networkNumber = value;
                NetworkNumberLabel.Content = string.Format("Network {0}", _networkNumber);
            }
        }

        public string NetworkBrief
        {
            get
            {
                return NetworkBriefLabel.Content.ToString();
            }
            set
            {
                NetworkBriefLabel.Content = value;
            }
        }

        public string NetworkDescription
        {
            get
            {
                return NetworkDescriptionTextBlock.Text;
            }
            set
            {
                NetworkDescriptionTextBlock.Text = value;
            }
        }

        private bool _isMasked;
        public bool IsMasked
        {
            get
            {
                return _isMasked;
            }
            set
            {
                
                _isMasked = value;
                if (_isMasked)
                {
                    CommentAreaExpander.Background = Brushes.LightGray;
                    LadderCanvas.Background = Brushes.LightGray;
                    CommentAreaExpander.Opacity = 0.4;
                    LadderCanvas.Opacity = 0.4;
                }
                else
                {
                    CommentAreaExpander.Opacity = 1.0;
                    LadderCanvas.Opacity = 1.0;
                    CommentAreaExpander.Background = Brushes.LightCyan;
                    LadderCanvas.Background = Brushes.Transparent;
                }
            }
        }


        private bool _isCommendMode;
        public bool IsCommentMode
        {
            get { return _isCommendMode; }
            set
            {
                _isCommendMode = value;
                LadderCanvas.Height = _rowCount * HeightUnit;
                foreach (var ele in _ladderElements.Values)
                {
                    ele.IsCommentMode = _isCommendMode;
                }
                foreach(var vline in _ladderVerticalLines.Values)
                {
                    vline.IsCommentMode = _isCommendMode;
                }
            }
        }
        private SortedDictionary<IntPoint, BaseViewModel> _ladderElements = new SortedDictionary<IntPoint, BaseViewModel>();
        private SortedDictionary<IntPoint, VerticalLineViewModel> _ladderVerticalLines = new SortedDictionary<IntPoint, VerticalLineViewModel>();

        #region Selection relative data

        public int SelectAreaOriginX
        {
            get;set;
        }
        public int SelectAreaOriginY
        {
            get;set;
        }
        private int _selectAreaFirstX;
        private int _selectAreaFirstY;
        private int _selectAreaSecondX;
        private int _selectAreaSecondY;
        public int SelectAreaFirstX
        {
            get
            {
                return _selectAreaFirstX;
            }
            set
            {
                _selectAreaFirstX = value;
                var left = Math.Min(_selectAreaFirstX, _selectAreaSecondX) * WidthUnit;
                var width = (Math.Abs(_selectAreaFirstX - _selectAreaSecondX) + 1) * WidthUnit;
                SelectArea.Width = width;
                Canvas.SetLeft(SelectArea, left);
            }
        }
        public int SelectAreaFirstY
        {
            get
            {
                return _selectAreaFirstY;
            }
            set
            {
                _selectAreaFirstY = value;
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * HeightUnit;
                var height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
                SelectArea.Height = height;
                Canvas.SetTop(SelectArea, top);
            }
        }
        public int SelectAreaSecondX
        {
            get
            {
                return _selectAreaSecondX;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > GlobalSetting.LadderXCapacity - 1)
                {
                    value = GlobalSetting.LadderXCapacity - 1;
                }
                _selectAreaSecondX = value;
                var left = Math.Min(_selectAreaFirstX, _selectAreaSecondX) * WidthUnit;
                var width = (Math.Abs(_selectAreaFirstX - _selectAreaSecondX) + 1) * WidthUnit;
                SelectArea.Width = width;
                Canvas.SetLeft(SelectArea, left);
            }
        }
        public int SelectAreaSecondY
        {
            get
            {
                return _selectAreaSecondY;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > RowCount - 1)
                {
                    value = RowCount - 1;
                }
                _selectAreaSecondY = value;
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * HeightUnit;
                var height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
                SelectArea.Height = height;
                Canvas.SetTop(SelectArea, top);
            }
        }
        private bool _isSelectAreaMode;
        public bool IsSelectAreaMode
        {
            get
            {
                return _isSelectAreaMode;
            }
            set
            {
                if (!_isSelectAreaMode & value)
                {
                    LadderCanvas.Children.Add(SelectArea);
                }
                if (!value & _isSelectAreaMode)
                {
                    LadderCanvas.Children.Remove(SelectArea);
                }
                _isSelectAreaMode = value;
            }
        }
        private bool _isSelectAllMode;
        public bool IsSelectAllMode
        {
            get
            {
                return _isSelectAllMode;
            }
            set
            {
                _isSelectAllMode = value;
                if(_isSelectAllMode)
                {
                    IsSelectAreaMode = true;
                    SelectAreaFirstY = 0;
                    SelectAreaFirstX = 0;
                    SelectAreaSecondX = GlobalSetting.LadderXCapacity - 1;
                    SelectAreaSecondY = RowCount - 1;
                    CommentAreaGrid.Background = new SolidColorBrush(Colors.DarkBlue);
                    CommentAreaGrid.Background.Opacity = 0.3;
                }
                else
                {
                    CommentAreaGrid.Background = Brushes.Transparent;
                }
            }
        }
        public Rectangle SelectArea { get; set; } = new Rectangle();

        #endregion

        // parent ladder diagram
        private LadderDiagramViewModel _ladderDiagram;

        public LadderNetworkViewModel(LadderDiagramViewModel parent, int number)
        {
            InitializeComponent();
            _ladderDiagram = parent;
            RowCount = 1;
            NetworkNumber = number;
            SelectArea.Fill = new SolidColorBrush(Colors.DarkBlue);
            SelectArea.Opacity = 0.2;
            Canvas.SetZIndex(SelectArea, -1);
        }

        public IEnumerable<BaseViewModel> GetElements()
        {
            return _ladderElements.Values;
        }

        public BaseViewModel SearchElement(int x, int y)
        {
            var p = new IntPoint() { X = x, Y = y };
            if (_ladderElements.ContainsKey(p))
            {
                return _ladderElements[p];
            }
            return null;
        }

        public VerticalLineViewModel SearchVerticalLine(int x, int y)
        {
            var p = new IntPoint() { X = x, Y = y };
            if (_ladderVerticalLines.ContainsKey(p))
            {
                return _ladderVerticalLines[p];
            }
            return null;
        }

        public IEnumerable<VerticalLineViewModel> GetVerticalLines()
        {
            return _ladderVerticalLines.Values;
        }

        #region Ladder content modification methods
        public void ReplaceElement(BaseViewModel element)
        {
            BaseViewModel oldele = null;
            bool flag = false;
            // Remove old element before
            if (element.Type == ElementType.Output)
            {
                element.X = GlobalSetting.LadderXCapacity - 1;
                if (element.Y >= 0 && element.Y < RowCount)
                {
                    flag = true;
                }
            }
            else
            {
                if (element.X >= 0 && element.X < GlobalSetting.LadderXCapacity - 1 && element.Y >= 0 && element.Y < RowCount)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                IntPoint p = new IntPoint() { X = element.X, Y = element.Y };
                if (_ladderElements.Keys.Contains(p))
                {
                    oldele = _ladderElements[p];
                    InstructionCommentManager.Unregister(oldele);
                    oldele.ShowPropertyDialogEvent -= this.OnShowPropertyDialog;
                    _ladderElements.Remove(p);
                    LadderCanvas.Children.Remove(oldele);
                }
                element.IsCommentMode = _isCommendMode;
                _ladderElements.Add(p, element);
                LadderCanvas.Children.Add(element);
                element.ShowPropertyDialogEvent += OnShowPropertyDialog;
                InstructionCommentManager.Register(element);
            }
        }
        public void ReplaceVerticalLine(VerticalLineViewModel vline)
        {
            IntPoint p = new IntPoint() { X = vline.X, Y = vline.Y };
            if (!_ladderVerticalLines.ContainsKey(p))
            {
                vline.IsCommentMode = _isCommendMode;
                _ladderVerticalLines.Add(p, vline);
                LadderCanvas.Children.Add(vline);
            }
        }
        public void RemoveElement(IntPoint pos)
        {
            if (_ladderElements.ContainsKey(pos))
            {
                var ele = _ladderElements[pos];
                LadderCanvas.Children.Remove(ele);
                _ladderElements.Remove(pos);
                InstructionCommentManager.Unregister(ele);
                ele.ShowPropertyDialogEvent -= this.OnShowPropertyDialog;
            }
        }
        public void RemoveElement(int x, int y)
        {
            RemoveElement(new IntPoint() { X = x, Y = y });
        }
        public void RemoveElement(BaseViewModel element)
        {
            if (_ladderElements.ContainsValue(element))
            {
                RemoveElement(element.X, element.Y);
            }
        }
        public void RemoveVerticalLine(IntPoint pos)
        {
            if (_ladderVerticalLines.ContainsKey(pos))
            {
                LadderCanvas.Children.Remove(_ladderVerticalLines[pos]);
                _ladderVerticalLines.Remove(pos);
            }
        }
        public void RemoveVerticalLine(int x, int y)
        {
            RemoveVerticalLine(new IntPoint() { X = x, Y = y });
        }
        public void RemoveVerticalLine(VerticalLineViewModel vline)
        {
            if (_ladderVerticalLines.ContainsValue(vline))
            {
                RemoveVerticalLine(vline.X, vline.Y);
            }
        }
        #endregion

        #region Compile relative
        public void ClearSearchedFlag()
        {
            foreach (var model in _ladderElements.Values)
            {
                model.IsSearched = false;
            }
            foreach (var model in _ladderVerticalLines.Values)
            {
                model.IsSearched = false;
            }
        }
        private void ClearNextElements()
        {
            foreach (var ele in _ladderElements.Values)
            {
                ele.NextElemnets.Clear();
            }
        }
        public void PreCompile()
        {
            ClearNextElements();
            ClearSearchedFlag();
            var rootElements = _ladderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(rootElements);
            while (tempQueue.Count > 0)
            {
                var tempElement = tempQueue.Dequeue();
                if (tempElement.Type != ElementType.Null)
                {
                    var templist = SearchFrom(tempElement);
                    foreach (var ele in templist)
                    {
                        tempQueue.Enqueue(ele);
                    }
                }
            }
            InitializeSubElements();
        }

        public string GenerateCode()
        {
            PreCompile();
            var graph = ConvertToGraph();
            graph.Convert();
            var tree = graph.ConvertToTree();
            tree.TreeName = string.Format("network_{0}", NetworkNumber);
            return tree.GenerateCode();
        }
        public bool IsNetworkError()
        {
            PreCompile();
            if (_ladderElements.Values.Count == 0 && _ladderVerticalLines.Values.Count == 0)
            {
                return true;
            }
            return IsLadderGraphOpen() || IsLadderGraphShort();
        }
        private bool IsLadderGraphShort()
        {
            ClearSearchedFlag();
            var rootElements = _ladderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(rootElements);
            while (tempQueue.Count > 0)
            {
                var ele = tempQueue.Dequeue();
                if (!ele.IsSearched)
                {
                    ele.IsSearched = true;
                    if (ele.Type != ElementType.Null && (!CheckLadderGraphShort(ele)))
                    {
                        return true;
                    }
                }
                foreach (var item in ele.NextElemnets)
                {
                    tempQueue.Enqueue(item);
                }
            }
            return !(IsAllLinkedToRoot() && CheckSpecialModel() && CheckSelfLoop() && CheckHybridLink());
        }
        //短路检测
        private bool CheckLadderGraphShort(BaseViewModel checkmodel)
        {
            List<BaseViewModel> eles = checkmodel.NextElemnets;
            if (eles.Count == 1)
            {
                return true;
            }
            if (eles.Exists(x => { return x.Type == ElementType.Null; }) && eles.Count > 1)
            {
                return false;
            }
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(eles);
            while (tempQueue.Count > 0)
            {
                var ele = tempQueue.Dequeue();
                if (eles.Intersect(ele.NextElemnets).Count() > 0)
                {
                    return false;
                }
                else
                {
                    foreach (var item in ele.NextElemnets)
                    {
                        tempQueue.Enqueue(item);
                    }
                }
            }
            return true;
        }
        //自环检测
        private bool CheckSelfLoop()
        {
            var notHLines = _ladderElements.Values.Where(x => { return x.Type != ElementType.HLine; });
            //var needCheckElements = notHLines.Where(x => { return !(x.NextElemnets.Any(y => { return y.Type == ElementType.Null; })); });
            IEnumerable<BaseViewModel> hLinesOfNeedCheckElement;
            foreach (var needCheckElement in notHLines)
            {
                hLinesOfNeedCheckElement = GetHLines(needCheckElement);
                if (!CheckHLines(hLinesOfNeedCheckElement))
                {
                    return false;
                }
            }
            return true;
        }
        //判断集合中X坐标相同的直线，其NextElemnets集合是否完全相同
        private bool CheckHLines(IEnumerable<BaseViewModel> hLines)
        {
            for (int x = 0; x < 10; x++)
            {
                var lines = hLines.Where(l => { return l.X == x; });
                if (lines.Count() > 1)
                {
                    for (int j = 0; j < lines.Count(); j++)
                    {
                        for (int k = j + 1; k < lines.Count(); k++)
                        {
                            if (lines.ElementAt(j).NextElemnets.SequenceEqual(lines.ElementAt(k).NextElemnets))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        //得到与model直连的所有HLine
        private IEnumerable<BaseViewModel> GetHLines(BaseViewModel model)
        {
            var hLines = _ladderElements.Values.Where(x => { return (x.Type == ElementType.HLine); });
            var tempList = new List<BaseViewModel>();
            foreach (var hLine in hLines)
            {
                if (IsRelativeToModel(model, hLine))
                {
                    tempList.Add(hLine);
                }
            }
            return tempList;
        }
        private bool IsRelativeToModel(BaseViewModel model, BaseViewModel hLine)
        {
            var relativePoints = GetRightRelativePoint(hLine.X, hLine.Y);
            //判断元素hLine是否与model相连
            return CheckRelativePoints(relativePoints, model, "Right");
        }
        private bool CheckRelativePoints(IEnumerable<IntPoint> relativePoints, BaseViewModel model, string direction)
        {
            IntPoint p1 = new IntPoint();//right
            IntPoint p2 = new IntPoint();//up
            IntPoint p3 = new IntPoint();//down
            bool right = false;
            bool up = false;
            bool down = false;
            if (direction == "Right")
            {
                p1 = relativePoints.ElementAt(0);
                p2 = relativePoints.ElementAt(1);
                p3 = relativePoints.ElementAt(2);
            }
            else if (direction == "Up")
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
            if (_ladderElements.TryGetValue(p1, out element))
            {
                if (element == model)
                {
                    return true;
                }
                if (element.Type == ElementType.HLine)
                {
                    right = CheckRelativePoints(GetRightRelativePoint(p1.X, p1.Y), model, "Right");
                }
            }
            VerticalLineViewModel verticalLine;
            if ((direction == "Up") || (direction == "Right"))
            {
                if (_ladderVerticalLines.TryGetValue(p2, out verticalLine))
                {
                    up = CheckRelativePoints(GetUpRelativePoint(p2.X, p2.Y), model, "Up");
                }
            }
            if ((direction == "Down") || (direction == "Right"))
            {
                if (_ladderVerticalLines.TryGetValue(p3, out verticalLine))
                {
                    down = CheckRelativePoints(GetDownRelativePoint(p3.X, p3.Y), model, "Down");
                }
            }
            //结果必须是三个方向的并
            return right || up || down;
        }
        //混联模块检测
        private bool CheckHybridLink()
        {
            bool result1 = true;
            bool result2 = true;
            //得到有多条支路的元素集合
            var needCheckElements = _ladderElements.Values.Where(x => { return x.NextElemnets.Count > 1; });
            foreach (var ele in needCheckElements)
            {
                for (int i = 0; i < ele.NextElemnets.Count; i++)
                {
                    for (int j = i + 1; j < ele.NextElemnets.Count; j++)
                    {
                        //取其中任意两条支路并得到其交集
                        var item1 = ele.NextElemnets.ElementAt(i);
                        var item2 = ele.NextElemnets.ElementAt(j);
                        var tempPublicEle = item1.SubElements.Intersect(item2.SubElements);
                        int cnt = tempPublicEle.Count();
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
                                    result1 = false;
                                }
                            }
                        }
                        //得到除直线外，且除去ele子元素的集合。
                        var tempList = new List<BaseViewModel>(_ladderElements.Values.Where(x => { return x.Type != ElementType.HLine; }));
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
                                result2 = false;
                            }
                        }
                    }
                }
            }
            return result1 && result2;
        }
        //特殊模块检测
        private bool CheckSpecialModel()
        {
            var allElements = _ladderElements.Values.Where(x => { return x.Type != ElementType.HLine; });
            var allSpecialModels = _ladderElements.Values.Where(x => { return x.Type == ElementType.Special; });
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
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        private void InitializeSubElements()
        {
            ClearSearchedFlag();
            var rootElements = _ladderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            foreach (var rootElement in rootElements)
            {
                GetSubElements(rootElement);
            }
        }
        //得到所有子元素，包括自身和NULL元素。
        private List<BaseViewModel> GetSubElements(BaseViewModel model)
        {
            if (model.IsSearched)
            {
                return model.SubElements;
            }
            List<BaseViewModel> result = new List<BaseViewModel>();
            foreach (var ele in model.NextElemnets)
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
        private bool IsAllLinkedToRoot()
        {
            var rootElements = _ladderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            var tempList = new List<BaseViewModel>();
            tempList = GetRootLinkedEles(rootElements.ElementAt(0));
            for (int x = 1; x < rootElements.Count(); x++)
            {
                tempList = tempList.Intersect(GetRootLinkedEles(rootElements.ElementAt(x))).ToList();
                if (tempList.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }
        //得到与根元素相关的最后一个非NULL元素集合
        private List<BaseViewModel> GetRootLinkedEles(BaseViewModel rootElement)
        {
            ClearSearchedFlag();
            List<BaseViewModel> tempList = new List<BaseViewModel>();
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(rootElement.NextElemnets);
            while (tempQueue.Count > 0)
            {
                var ele = tempQueue.Dequeue();
                if (!ele.IsSearched)
                {
                    ele.IsSearched = true;
                    if (ele.NextElemnets.Exists(x => { return x.Type == ElementType.Null; }))
                    {
                        tempList.Add(ele);
                    }
                    else
                    {
                        foreach (var item in ele.NextElemnets)
                        {
                            tempQueue.Enqueue(item);
                        }
                    }
                }
            }
            return tempList;
        }
        //在PreComplie()方法后执行
        private bool IsLadderGraphOpen()
        {
            ClearSearchedFlag();
            //首先检查元素的基本性质
            if (!Assert())
            {
                return true;
            }
            var rootElements = _ladderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            //检查上并联错误
            int MinY = rootElements.First().Y;
            foreach (var ele in rootElements)
            {
                ele.IsSearched = true;
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
                        return true;
                    }
                    foreach (var ele in item.NextElemnets)
                    {
                        tempQueue.Enqueue(ele);
                    }
                }
            }
            return false;
        }
        //检测VerticalLine，不允许上或下没有元素。
        private bool CheckVerticalLines()
        {
            IntPoint pos = new IntPoint();
            IntPoint one = new IntPoint();
            IntPoint two = new IntPoint();
            var tempQueue = new Queue<VerticalLineViewModel>(_ladderVerticalLines.Values);
            while (tempQueue.Count > 0)
            {
                var verticalLine = tempQueue.Dequeue();
                pos.X = verticalLine.X;
                pos.Y = verticalLine.Y;
                one.X = pos.X + 1;
                one.Y = pos.Y;
                two.X = pos.X;
                two.Y = pos.Y - 1;
                if ((!_ladderElements.ContainsKey(pos)) && (!_ladderElements.ContainsKey(one)) && (!_ladderVerticalLines.ContainsKey(two)))
                {
                    return false;
                }
                pos.Y += 1;
                one.Y += 1;
                two.Y += 2;
                if ((!_ladderElements.ContainsKey(pos)) && (!_ladderElements.ContainsKey(one)) && (!_ladderVerticalLines.ContainsKey(two)))
                {
                    return false;
                }
            }
            return true;
        }
        private List<BaseViewModel> SearchFrom(BaseViewModel viewmodel)
        {
            if (viewmodel.IsSearched)
            {
                return viewmodel.NextElemnets;
            }
            List<BaseViewModel> result = new List<BaseViewModel>();
            if (viewmodel.X == 0)
            {
                viewmodel.IsSearched = true;
                NullViewModel nullModel = new NullViewModel();
                nullModel.X = 0;
                nullModel.Y = viewmodel.Y;
                result.Add(nullModel);
                //result.Add(BaseViewModel.Null);
                viewmodel.NextElemnets = result;
                return result;
            }
            else
            {
                int x = viewmodel.X;
                int y = viewmodel.Y;
                var relativePoints = GetRelativePoint(x - 1, y);
                foreach (var p in relativePoints)
                {
                    BaseViewModel leftmodel;
                    if (_ladderElements.TryGetValue(p, out leftmodel))
                    {
                        if (leftmodel.Type == ElementType.HLine)
                        {
                            var _leftresult = SearchFrom(leftmodel);
                            foreach (var temp in SearchFrom(leftmodel))
                            {
                                result.Add(temp);
                            }
                        }
                        else
                        {
                            result.Add(leftmodel);
                        }
                    }
                }
                viewmodel.IsSearched = true;
                viewmodel.NextElemnets = result;
                return result;
            }
        }
        private IEnumerable<IntPoint> GetRelativePoint(int x, int y)
        {
            List<IntPoint> result = new List<IntPoint>();
            Stack<IntPoint> tempStack = new Stack<IntPoint>();
            var p = new IntPoint() { X = x, Y = y };

            var q1 = new IntPoint() { X = x, Y = y };
            var q2 = new IntPoint() { X = x, Y = y };
            while (SearchUpVLine(q1))
            {
                q1 = new IntPoint() { X = q1.X, Y = q1.Y - 1 };
                tempStack.Push(q1);
            }
            while (tempStack.Count > 0)
            {
                result.Add(tempStack.Pop());
            }
            result.Add(p);
            while (SearchDownVLine(q2))
            {
                q2 = new IntPoint() { X = q2.X, Y = q2.Y + 1 };
                result.Add(q2);
            }
            return result;
        }

        public ExpGraph ConvertToGraph()
        {
            return LadderHelper.ConvertGraph(_ladderElements.Values);
        }

        private bool SearchUpVLine(IntPoint p)
        {
            return _ladderVerticalLines.Values.Any(l => { return (l.X == p.X) && (l.Y == p.Y - 1); });
        }

        private bool SearchDownVLine(IntPoint p)
        {
            return _ladderVerticalLines.Values.Any(l => { return (l.X == p.X) && (l.Y == p.Y); });
        }
        private IEnumerable<IntPoint> GetLeftRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为左,上,下与左相同
            tempList.Add(new IntPoint() { X = x - 1, Y = y });
            tempList.Add(new IntPoint() { X = x - 1, Y = y - 1 });
            return tempList;
        }
        private IEnumerable<IntPoint> GetUpRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为上,右
            tempList.Add(new IntPoint() { X = x, Y = y - 1 });
            tempList.Add(new IntPoint() { X = x + 1, Y = y });
            return tempList;
        }
        private IEnumerable<IntPoint> GetDownRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为下，右
            tempList.Add(new IntPoint() { X = x, Y = y + 1 });
            tempList.Add(new IntPoint() { X = x + 1, Y = y + 1 });
            return tempList;
        }
        private IEnumerable<IntPoint> GetRightRelativePoint(int x, int y)
        {
            List<IntPoint> tempList = new List<IntPoint>();
            //添加顺序为右，上，下
            tempList.Add(new IntPoint() { X = x + 1, Y = y });
            tempList.Add(new IntPoint() { X = x, Y = y - 1 });
            tempList.Add(new IntPoint() { X = x, Y = y });
            return tempList;
        }
        /// <summary>
        /// 判断网络是否有错误
        /// </summary>
        /// <returns>True 无错，False 有错</returns>
        public bool Assert()
        {
            return _ladderElements.Values.All(x => x.Assert()) && CheckVerticalLines();
        }
        #endregion

        #region Event handlers
        private void OnShowPropertyDialog(BaseViewModel sender, ShowPropertyDialogEventArgs e)
        {
            var dialog = e.Dialog;
            dialog.Commit += (sender1, e1) =>
            {
                try
                {
                    sender.AcceptNewValues(dialog.PropertyStrings, SamSoarII.PLCDevice.Device.DefaultDevice);
                    dialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        };
            dialog.ShowDialog();
        }
        private void OnAddNewRowBefore(object sender, RoutedEventArgs e)
        {
            if(IsSingleSelected())
            {
                _ladderDiagram.NetworkAddRow(this, _ladderDiagram.SelectionRect.Y);
            }
        }
        private void OnAddNewRowAfter(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
            {
                _ladderDiagram.NetworkAddRow(this, _ladderDiagram.SelectionRect.Y + 1);
                //AddNewRowAfter(_ladderDiagram.SelectionRect.Y);
            }
        }
        private void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
            {
                _ladderDiagram.NetworkRemoveRow(this, _ladderDiagram.SelectionRect.Y);
            }
        }
        private void OnAppendNewRow(object sender, RoutedEventArgs e)
        {
            if(IsSingleSelected())
            {
                //AppendRow();
                _ladderDiagram.NetworkAddRow(this, RowCount);
            }
        }

        private void OnAppendNewNetwork(object sender, RoutedEventArgs e)
        {
            if(IsSingleSelected())
            {
                _ladderDiagram.AppendNewNetwork();
            }
        }

        private void OnAddNewNetworkBefore(object sender, RoutedEventArgs e)
        {
            if(IsSingleSelected())
            {
                _ladderDiagram.AddNewNetworkBefore(this);
            }
        }

        private void OnAddNewNetworkAfter(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
            {
                _ladderDiagram.AddNewNetworkAfter(this);
            }
            
        }

        private void OnRemoveNetwork(object sender, RoutedEventArgs e)
        {
            if(IsSingleSelected())
            {
                _ladderDiagram.RemoveSingleNetworkCommand(this);
            }
        }

        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            SamSoarII.UserInterface.NetworkCommentEditDialog editDialog = new UserInterface.NetworkCommentEditDialog();
            editDialog.NetworkNumber = NetworkNumber;
            editDialog.NetworkBrief = NetworkBrief;
            editDialog.NetworkDescription = NetworkDescription;
            editDialog.EnsureButtonClick += (sender1, e1) =>
            {
                NetworkBrief = editDialog.NetworkBrief;
                NetworkDescription = editDialog.NetworkDescription;
                editDialog.Close();
            };
            editDialog.ShowDialog();
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(LadderCanvas);
                var intPoint = IntPoint.GetIntpointByDouble(pos.X, pos.Y, WidthUnit, HeightUnit);
                if (!IsSingleSelected())
                {
                    AcquireSelectRect();
                }
                _ladderDiagram.SelectionRect.X = intPoint.X;
                _ladderDiagram.SelectionRect.Y = intPoint.Y;
            }
        }

        #endregion

        #region Command execute
        private void ElementsCutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if((_ladderDiagram.SelectionStatus == SelectStatus.SingleSelected) || (_ladderDiagram.SelectionStatus == SelectStatus.MultiSelected))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        #endregion

        #region Command can execute
        private void OnElementsCutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {

        }
        #endregion

        #region IComparable interface implemention
        // 按照NetworkNumber大小排序
        public int CompareTo(object obj)
        {
            var net = obj as LadderNetworkViewModel;
            if (net != null)
            {
                if (net.NetworkNumber > this.NetworkNumber)
                {
                    return -1;
                }
                else
                {
                    if (net.NetworkNumber == this.NetworkNumber)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            return -1;
        }

        #endregion

        /// <summary>
        /// Be selected only if the network acquire SelecRect
        /// </summary>
        /// <returns></returns>
        public bool IsSingleSelected()
        {
            return LadderCanvas.Children.Contains(_ladderDiagram.SelectionRect);
        }

        public bool IsFirstNetwork()
        {
            return NetworkNumber == 0;
        }

        public bool IsLastNetwork()
        {
            return NetworkNumber == (_ladderDiagram.NetworkCount - 1);
        }

        public void ReleaseSelectRect()
        {
            if(IsSingleSelected())
            {
                LadderCanvas.Children.Remove(_ladderDiagram.SelectionRect);
            }
        }

        public void AcquireSelectRect()
        {
            _ladderDiagram.AcquireSelectRect(this);
            LadderCanvas.Children.Add(_ladderDiagram.SelectionRect);     
        }

        public List<BaseViewModel> GetSelectedElements()
        {
            List<BaseViewModel> result = new List<BaseViewModel>();
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            IntPoint p = new IntPoint();
            for (int i = yBegin; i <= yEnd; i++)
            {
                for(int j = xBegin; j <= xEnd; j++)
                {
                    p.X = j;
                    p.Y = i;
                    try
                    {
                        result.Add(_ladderElements[p]);
                    }
                    catch(KeyNotFoundException exception)
                    {
                    } 
                }
            }
            return result;
        }

        public List<VerticalLineViewModel> GetSelectedVerticalLines()
        {
            List<VerticalLineViewModel> result = new List<VerticalLineViewModel>();
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            IntPoint p = new IntPoint();
            //最后一行的竖线不算进选择区域内
            for (int i = yBegin; i < yEnd; i++)
            {
                for (int j = xBegin; j <= xEnd; j++)
                {
                    p.X = j;
                    p.Y = i;
                    try
                    {
                        result.Add(_ladderVerticalLines[p]);
                    }
                    catch (KeyNotFoundException exception)
                    {
                    }
                }
            }
            return result;
        }



    }
}
