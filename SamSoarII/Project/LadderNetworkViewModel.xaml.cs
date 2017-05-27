﻿using SamSoarII.AppMain.LadderCommand;
using SamSoarII.AppMain.LadderGraphModule;
using SamSoarII.AppMain.UI;
using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Communication;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.LadderInstViewModel.Interrupt;
using SamSoarII.LadderInstViewModel.Monitor;
using SamSoarII.PLCCompiler;
using SamSoarII.PLCDevice;
using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.UserInterface;
using SamSoarII.Utility;
using SamSoarII.ValueModel;
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
    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    enum DirectionStatus
    {
        Up_Dec,
        Up_Inc,
        Down_Dec,
        Down_Inc,
        None
    }
    public enum LadderMode
    {
        Edit,
        Monitor,
        Simulate,
        Demo
    }
    /// <summary>
    /// LadderNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderNetworkViewModel : UserControl, IComparable, INotifyPropertyChanged
    {
        #region Canvas System
        //private ContextMenu CM_Ladder;
        private MonitorContextMenu CM_Monitor
            = new MonitorContextMenu();

        private LadderMode laddermode;
        public LadderMode LadderMode
        {
            get { return this.laddermode; }
            set
            {
                this.laddermode = value;
                foreach (BaseViewModel bvmodel in _ladderVerticalLines.Values.Union(_ladderElements.Values))
                {
                    switch (laddermode)
                    {
                        case LadderMode.Demo:
                            LadderCanvas.ContextMenu = null;
                            bvmodel.IsMonitorMode = false;
                            bvmodel.CanModify = false;
                            break;
                        case LadderMode.Edit:
                            LadderCanvas.ContextMenu = CM_Edit;
                            bvmodel.IsMonitorMode = false;
                            bvmodel.CanModify = true;
                            break;
                        default:
                            LadderCanvas.ContextMenu = CM_Monitor;
                            bvmodel.IsMonitorMode = true;
                            bvmodel.CanModify = false;
                            break;
                    }
                }
            }
        }
        
        #endregion

        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        private bool _isExpand = true;
        public bool IsExpand
        {
            get
            {
                return _isExpand;
            }
            set
            {
                _isExpand = value;
            }
        }
        private int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }
        private int _rowCount;
        private int _oldRowCount;
        private bool _canHide = false;
        public int RowCount
        {
            get
            {
                if (!_canHide)
                {
                    return _rowCount;
                }
                else
                {
                    return _oldRowCount;
                }
            }
            set
            {
                if(value > 0 || _canHide)
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
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NetworkMessage"));
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
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("NetworkMessage"));
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
                    ReleaseSelectRect();
                    IsSelectAreaMode = false;
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
        public SortedDictionary<IntPoint, BaseViewModel> LadderElements
        {
            get
            {
                return _ladderElements;
            }
            set
            {
                _ladderElements = value;
            }
        }
        public SortedDictionary<IntPoint, VerticalLineViewModel> LadderVerticalLines
        {
            get
            {
                return _ladderVerticalLines;
            }
            set
            {
                _ladderVerticalLines = value;
            }
        }
        public HashSet<BaseViewModel> ErrorModels { get; set; } = new HashSet<BaseViewModel>();
        private SortedDictionary<IntPoint, BaseViewModel> _ladderElements 
            = new SortedDictionary<IntPoint, BaseViewModel>();
        private SortedDictionary<IntPoint, VerticalLineViewModel> _ladderVerticalLines 
            = new SortedDictionary<IntPoint, VerticalLineViewModel>();
        public Dictionary<int, LadderLogicModule> LadderLogicModules { get; set; }
        private InstructionNetworkViewModel _invmodel;
        public InstructionNetworkViewModel INVModel
        {
            get { return this._invmodel; }
            set
            {
                this._invmodel = value;
                _invmodel.Setup(this);
            }
        }
         
        public BaseViewModel GetElementByPosition(int X, int Y)
        {
            IntPoint ip = new IntPoint();
            ip.X = X;
            ip.Y = Y;
            if (_ladderElements.ContainsKey(ip))
            {
                return _ladderElements[ip];
            }
            return null;
        }

        public VerticalLineViewModel GetVerticalLineByPosition(int X, int Y)
        {
            IntPoint ip = new IntPoint();
            ip.X = X;
            ip.Y = Y;
            if (_ladderVerticalLines.ContainsKey(ip))
            {
                return _ladderVerticalLines[ip];
            }
            return null;
        }
        
        #region Selection relative data

        public int SelectAreaOriginFX
        {
            get;set;
        }
        public int SelectAreaOriginFY
        {
            get;set;
        }
        private int _selectAreaOriginSX;
        public int SelectAreaOriginSX
        {
            get
            {
                return _selectAreaOriginSX;
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
                _selectAreaOriginSX = value;
            }
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
                int height;
                if (_canHide)
                {
                    height = 0;
                }
                else
                {
                    height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
                }
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
                int height;
                if (_canHide)
                {
                    height = 0;
                }
                else
                {
                    height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
                }
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
                    if (_canHide)
                    {
                        SelectAreaSecondY = 0;
                    }
                    else
                    {
                        SelectAreaSecondY = RowCount - 1;
                    }
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

        private bool _canScrollToolTip = false;
        // parent ladder diagram
        private LadderDiagramViewModel _ladderDiagram;

        public LadderDiagramViewModel LDVModel
        {
            get { return _ladderDiagram; }
            set { _ladderDiagram = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public LadderNetworkViewModel(LadderDiagramViewModel parent, int number)
        {
            InitializeComponent();
            LadderMode = LadderMode.Edit;
            _ladderDiagram = parent;
            RowCount = 1;
            NetworkNumber = number;
            SelectArea.Fill = new SolidColorBrush(Colors.DarkBlue);
            SelectArea.Opacity = 0.2;
            Canvas.SetZIndex(SelectArea, -1);
            Tag = false;
            Loaded += (sender, e) =>
            {
                if (!(bool)Tag)
                {
                    ladderExpander.IsExpand = IsExpand;
                }
                Tag = true;
            };
            ladderExpander.MouseEnter += OnMouseEnter;
            ladderExpander.MouseLeave += OnMouseLeave;
            ladderExpander.expandButton.IsExpandChanged += ExpandButton_IsExpandChanged;
            ThumbnailButton.ToolTipOpening += ThumbnailButton_ToolTipOpening;
            ThumbnailButton.ToolTipClosing += ThumbnailButton_ToolTipClosing;
            CM_Monitor.ValueModify += OnMonitorValueModify;
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

        public void EnterOriginSelectArea(bool isUp)
        {
            if (!isUp)
            {
                if (!_canHide && SelectAreaOriginFY == 0 && SelectAreaOriginFX == SelectAreaOriginSX)
                {
                    AcquireSelectRect();
                }
                else
                {
                    SelectAreaSecondY = 0;
                    SelectAreaFirstX = SelectAreaOriginFX;
                    SelectAreaFirstY = SelectAreaOriginFY;
                    SelectAreaSecondX = SelectAreaOriginSX;
                    IsSelectAreaMode = true;
                }
            }
            else
            {
                if (!_canHide && SelectAreaOriginFY == RowCount - 1 && SelectAreaOriginFX == SelectAreaOriginSX)
                {
                    AcquireSelectRect();
                }
                else
                {
                    SelectAreaSecondY = RowCount > 0 ? RowCount - 1 : 0 ;
                    SelectAreaFirstX = SelectAreaOriginFX;
                    SelectAreaFirstY = SelectAreaOriginFY;
                    SelectAreaSecondX = SelectAreaOriginSX;
                    IsSelectAreaMode = true;
                }
            }
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

        #region generate LadderLogicModule
        public void InitializeLadderLogicModules()
        {
            int cnt = 0;
            List<BaseViewModel> models = new List<BaseViewModel>();
            List<VerticalLineViewModel> vlines = new List<VerticalLineViewModel>();
            LadderLogicModules = new Dictionary<int, LadderLogicModule>();
            for (int i = 0; i <= GetMaxY(); i++)
            {
                var tempmodels = GetElements().Where(x => { return x.Y == i; });
                var tempvlines = GetVerticalLines().Where(x => { return x.Y == i; });
                if (tempmodels.Count() > 0)
                {
                    models.AddRange(tempmodels);
                    if (tempvlines.Count() > 0)
                    {
                        vlines.AddRange(tempvlines);
                    }
                    else
                    {
                        LadderLogicModules.Add(cnt,new LadderLogicModule(this,models,vlines));
                        if (i < GetMaxY())
                        {
                            cnt++;
                            models = new List<BaseViewModel>();
                            vlines = new List<VerticalLineViewModel>();
                        }
                    }
                }
                else if (tempvlines.Count() > 0)
                {
                    vlines.AddRange(tempvlines);
                }
            }
        }

        public int GetKeyByLadderLogicModule(LadderLogicModule ladderLogicModule)
        {
            foreach (var item in LadderLogicModules)
            {
                if (item.Value == ladderLogicModule)
                {
                    return item.Key;
                }
            }
            return -1;
        }
        public LadderLogicModule GetLadderLogicModuleByKey(int key)
        {
            return LadderLogicModules[key];
        }
        #endregion
        #region Ladder content modification methods
        public BaseViewModel ReplaceElement(BaseViewModel element)
        {
            BaseViewModel oldele = null;
            element.NetWorkNum = NetworkNumber;
            element.RefLadderName = _ladderDiagram.ProgramName;
            bool flag = false;
            // Remove old element before
            if (element.Type == ElementType.Output
             && LadderMode != LadderMode.Demo)
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
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = oldele;
                e.BVModel_new = element;
                ElementChanged(this, e);
            }
            return oldele;
        }
        public void ReplaceVerticalLine(VerticalLineViewModel vline)
        {
            IntPoint p = new IntPoint() { X = vline.X, Y = vline.Y };
            if (!_ladderVerticalLines.ContainsKey(p))
            {
                vline.IsCommentMode = _isCommendMode;
                _ladderVerticalLines.Add(p, vline);
                LadderCanvas.Children.Add(vline);
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = null;
                e.BVModel_new = vline;
                VerticalLineChanged(this, e);
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
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = ele;
                e.BVModel_new = null;
                ElementChanged(this, e);
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
        public bool RemoveVerticalLine(IntPoint pos)
        {
            if (_ladderVerticalLines.ContainsKey(pos))
            {
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = _ladderVerticalLines[pos];
                e.BVModel_new = null;
                LadderCanvas.Children.Remove(_ladderVerticalLines[pos]);
                _ladderVerticalLines.Remove(pos);
                VerticalLineChanged(this, e);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool RemoveVerticalLine(int x, int y)
        {
            return RemoveVerticalLine(new IntPoint() { X = x, Y = y });
        }
        public bool RemoveVerticalLine(VerticalLineViewModel vline)
        {
            if (_ladderVerticalLines.ContainsValue(vline))
            {
                return RemoveVerticalLine(vline.X, vline.Y);
            }
            else
            {
                return false;
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
        public void ClearNextElements()
        {
            foreach (var ele in _ladderElements.Values)
            {
                ele.NextElements.Clear();
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
        public bool CheckVerticalLine(VerticalLineViewModel VLine)
        {
            IntPoint pos = new IntPoint();
            IntPoint one = new IntPoint();
            IntPoint two = new IntPoint();
            pos.X = VLine.X;
            pos.Y = VLine.Y;
            one.X = pos.X + 1;
            one.Y = pos.Y;
            two.X = pos.X;
            two.Y = pos.Y - 1;
            if (!_ladderElements.ContainsKey(one) && !_ladderVerticalLines.ContainsKey(two))
            {
                return false;
            }
            if (!_ladderElements.ContainsKey(pos) && !_ladderVerticalLines.ContainsKey(two))
            {
                return false;
            }
            pos.Y += 1;
            one.Y += 1;
            two.Y += 2;
            if (!_ladderElements.ContainsKey(pos) && !_ladderElements.ContainsKey(one) && !_ladderVerticalLines.ContainsKey(two))
            {
                return false;
            }
            return true;
        }
        private List<BaseViewModel> SearchFrom(BaseViewModel viewmodel)
        {
            if (viewmodel.IsSearched)
            {
                return viewmodel.NextElements;
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
                viewmodel.NextElements = result;
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
                viewmodel.NextElements = result;
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
        #endregion
        //返回最后一行的Y坐标
        public int GetMaxY()
        {
            return LadderElements.Values.OrderBy(x => { return x.Y; }).Last().Y;
        }
        #region Event handlers

        #region Relative to Element changed
        public event LadderElementChangedHandler ElementChanged = delegate { };
        public event LadderElementChangedHandler VerticalLineChanged = delegate { };
        #endregion

        #region Update NetworkNum and LadderName of BaseViewModel
        public void UpdateModelMessage()
        {
            foreach (var ele in LadderElements.Values)
            {
                ele.NetWorkNum = NetworkNumber;
                ele.RefLadderName = LDVModel.ProgramName;
            }
        }
        public void ClearModelMessage()
        {
            foreach (var ele in LadderElements.Values)
            {
                ele.NetWorkNum = -1;
            }
        }
        #endregion

        private void OnShowPropertyDialog(BaseViewModel sender, ShowPropertyDialogEventArgs e)
        {
            var dialog = e.Dialog;
            ElementPropertyDialog epdialog = (ElementPropertyDialog)(dialog);
            switch (epdialog.InstMode)
            {
                case ElementPropertyDialog.INST_CALL:
                case ElementPropertyDialog.INST_ATCH:
                    epdialog.SubRoutines = _ladderDiagram.ProjectModel.SubRoutines.Select(
                        (LadderDiagramViewModel ldvmodel) =>
                        {
                            return ldvmodel.ProgramName;
                        }
                    );
                    break;
                case ElementPropertyDialog.INST_CALLM:
                    epdialog.Functions = _ladderDiagram.ProjectModel.Funcs.Where(
                        (FuncModel fmodel) => { return fmodel.CanCALLM(); }  
                    ).Select(
                        (FuncModel fmodel) =>
                        {
                            string[] result = new string[fmodel.ArgCount * 2 + 2];
                            result[0] = fmodel.ReturnType;
                            result[1] = fmodel.Name;
                            for (int i = 0; i < fmodel.ArgCount; i++)
                            {
                                result[i * 2 + 2] = fmodel.GetArgType(i);
                                result[i * 2 + 3] = fmodel.GetArgName(i);
                            }
                            return result;
                        }
                    );
                    break;
                case ElementPropertyDialog.INST_MBUS:
                    epdialog.ModbusTables = _ladderDiagram.ProjectModel.MTVModel.Models.Select(
                        (ModbusTableModel mtmodel) =>
                        {
                            return mtmodel.Name;
                        }
                    );
                    break;
            }
            dialog.Commit += (sender1, e1) =>
            {
                try
                {
                    if (dialog is ElementPropertyDialog)
                    {
                        ElementReplaceArgumentCommand eracommand = new ElementReplaceArgumentCommand(
                            this, sender, epdialog.PropertyStrings_Old, epdialog.PropertyStrings_New);
                        _ladderDiagram.CommandExecute(eracommand);
                    }
                    else
                    {
                        sender.AcceptNewValues(dialog.PropertyStrings, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    }
                    dialog.Close();
                }
                catch (ValueParseException ex)
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
            EditComment();
        }

        public void EditComment()
        {
            if (!IsMasked)
            {
                NetworkCommentEditDialog editDialog = new UserInterface.NetworkCommentEditDialog();
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
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            AcquireSelectRect(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            ProjectTreeViewItem ptvitem = new ProjectTreeViewItem(null);
            if (e.Data.GetDataPresent(ptvitem.GetType()))
            {
                ptvitem = (ProjectTreeViewItem)(e.Data.GetData(ptvitem.GetType()));
                if (ptvitem.RelativeObject is BaseViewModel
                 || ptvitem.RelativeObject is FuncModel
                 || ptvitem.RelativeObject is LadderDiagramViewModel
                 || ptvitem.RelativeObject is ModbusTableModel)
                {
                    AcquireSelectRect(e);
                }
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            ProjectTreeViewItem ptvitem = new ProjectTreeViewItem(null);
            bool isacquired = AcquireSelectRect(e);
            if (e.Data.GetDataPresent(typeof(LadderNetworkViewModel)))
            {
                isacquired = false;
                ReleaseSelectRect();
            }
            if (!isacquired) return;
            if (e.Data.GetDataPresent(ptvitem.GetType()))
            {
                ptvitem = (ProjectTreeViewItem)(e.Data.GetData(ptvitem.GetType()));
                if (ptvitem.RelativeObject is BaseViewModel)
                {
                    BaseViewModel bvmodel = (BaseViewModel)(ptvitem.RelativeObject);
                    bvmodel = bvmodel.Clone();
                    bvmodel.X = _ladderDiagram.SelectionRect.X;
                    bvmodel.Y = _ladderDiagram.SelectionRect.Y;
                    _ladderDiagram.ReplaceSingleElement(this, bvmodel);
                }
                else if (ptvitem.RelativeObject is FuncModel)
                {
                    FuncModel fmodel = (FuncModel)(ptvitem.RelativeObject);
                    if (!fmodel.CanCALLM())
                    {
                        MessageBox.Show(String.Format("{0:s}无法被CALLM指令调用，请检查参数的个数和类型。", fmodel.Name));
                        return;
                    }
                    CALLMViewModel vmodel = new CALLMViewModel();
                    ArgumentValue[] avalues = new ArgumentValue[fmodel.ArgCount];
                    for (int i = 0; i < fmodel.ArgCount; i++)
                    {
                        IValueModel ivmodel = null;
                        switch (fmodel.GetArgType(i))
                        {
                            case "BIT": ivmodel = BitValue.Null; break;
                            case "WORD": ivmodel = WordValue.Null; break;
                            case "DWORD": ivmodel = DWordValue.Null; break;
                            case "FLOAT": ivmodel = FloatValue.Null; break;
                            default: ivmodel = WordValue.Null; break;
                        }
                        avalues[i] = new ArgumentValue(
                            fmodel.GetArgName(i),
                            fmodel.GetArgType(i),
                            ivmodel);
                    }
                    vmodel.AcceptNewValues(fmodel.Name, avalues);
                    vmodel.X = _ladderDiagram.SelectionRect.X;
                    vmodel.Y = _ladderDiagram.SelectionRect.Y;
                    _ladderDiagram.ReplaceSingleElement(this, vmodel);
                }
                else if (ptvitem.RelativeObject is LadderDiagramViewModel)
                {
                    LadderDiagramViewModel ldvmodel = (LadderDiagramViewModel)(ptvitem.RelativeObject);
                    BaseViewModel bvmodel_old = GetElementByPosition(
                        _ladderDiagram.SelectionRect.X,
                        _ladderDiagram.SelectionRect.Y);
                    if (bvmodel_old is ATCHViewModel)
                    {
                        string[] paras_old = bvmodel_old.GetValueString().ToArray();
                        paras_old = new string[]
                        {
                            paras_old[0],
                            ValueCommentManager.GetComment(paras_old[0]),
                            paras_old[1]
                        };
                        string[] paras_new = paras_old.ToArray();
                        if (paras_old[0].Equals(String.Empty))
                        {
                            paras_old[0] = paras_new[0] = "K0";
                        }
                        paras_new[2] = ldvmodel.ProgramName;
                        ElementReplaceArgumentCommand command = new ElementReplaceArgumentCommand(
                            this, bvmodel_old, paras_old, paras_new);
                        _ladderDiagram.CommandExecute(command);
                    }
                    else
                    {
                        CALLViewModel bvmodel_new = new CALLViewModel();
                        bvmodel_new.AcceptNewValues(
                            new string[1] { ldvmodel.ProgramName },
                            PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                        bvmodel_new.X = _ladderDiagram.SelectionRect.X;
                        bvmodel_new.Y = _ladderDiagram.SelectionRect.Y;
                        _ladderDiagram.ReplaceSingleElement(this, bvmodel_new);
                    }
                }
                else if (ptvitem.RelativeObject is ModbusTableModel)
                {
                    ModbusTableModel mtmodel = (ModbusTableModel)(ptvitem.RelativeObject);
                    if (!mtmodel.IsVaild)
                    {
                        MessageBox.Show("Modbus表格非法，请检查表格项是否补全。");
                        return;
                    }
                    MBUSViewModel vmodel = new MBUSViewModel();
                    string[] paras = { "K232", "", mtmodel.Name, "", "D0", ""};
                    vmodel.AcceptNewValues(
                        paras,
                        PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    vmodel.X = _ladderDiagram.SelectionRect.X;
                    vmodel.Y = _ladderDiagram.SelectionRect.Y;
                    _ladderDiagram.ReplaceSingleElement(this, vmodel);
                }
            }
        }

        public bool AcquireSelectRect(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsMasked)
            {
                var pos = e.GetPosition(LadderCanvas);
                return AcquireSelectRect(pos);
            }
            return false;
        }
        
        public bool AcquireSelectRect(DragEventArgs e)
        {
            var pos = e.GetPosition(LadderCanvas);
            return AcquireSelectRect(pos);
        }

        public bool AcquireSelectRect(Point pos)
        {
            var intPoint = IntPoint.GetIntpointByDouble(pos.X, pos.Y, WidthUnit, HeightUnit);
            if (intPoint.X < 0 || intPoint.X >= 12
             || intPoint.Y < 0 || intPoint.Y >= RowCount)
            {
                return false;
            }
            _ladderDiagram.SelectionRect.X = intPoint.X;
            _ladderDiagram.SelectionRect.Y = intPoint.Y;
            if (!IsSingleSelected())
            {
                AcquireSelectRect();
            }
            return true;
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
                _ladderDiagram.SelectionRect.NetworkParent = null;
            }
        }
        public void AcquireSelectRect()
        {
            if (!LadderCanvas.Children.Contains(_ladderDiagram.SelectionRect))
            {
                _ladderDiagram.AcquireSelectRect(this);
                LadderCanvas.Children.Add(_ladderDiagram.SelectionRect);
                _ladderDiagram.SelectionRect.NetworkParent = this;
                SelectAreaOriginFX = _ladderDiagram.SelectionRect.X;
                SelectAreaOriginFY = _ladderDiagram.SelectionRect.Y;
                SelectAreaOriginSX = _ladderDiagram.SelectionRect.X;
            }
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
                    catch(KeyNotFoundException)
                    {
                    } 
                }
            }
            return result;
        }
        public List<BaseViewModel> GetSelectedHLines()
        {
            List<BaseViewModel> result = new List<BaseViewModel>();
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            IntPoint p = new IntPoint();
            for (int i = yBegin; i <= yEnd; i++)
            {
                for (int j = xBegin; j <= xEnd; j++)
                {
                    p.X = j;
                    p.Y = i;
                    try
                    {
                        if (_ladderElements.ContainsKey(p) && _ladderElements[p].Type == ElementType.HLine)
                        {
                            result.Add(_ladderElements[p]);
                        }
                    }
                    catch (KeyNotFoundException)
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
                    catch (KeyNotFoundException)
                    {
                    }
                }
            }
            return result;
        }
        #region ladder Folding module
        private void ReloadElementsToCanvas()
        {
            if (LadderMode != LadderMode.Demo)
            {
                LadderCanvas.Children.Clear();
                RowCount = _oldRowCount;
                _canHide = false;
                foreach (var ele in _ladderElements.Values)
                {
                    LadderCanvas.Children.Add(ele);
                }
                foreach (var ele in _ladderVerticalLines.Values)
                {
                    LadderCanvas.Children.Add(ele);
                }
            }
        }
        private void ClearElementsFromCanvas()
        {
            LadderCanvas.Children.Clear();
            _oldRowCount = RowCount;
            _canHide = true;
            RowCount = 0;
        }
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Rect.Fill = LadderHelper.FoldingBrush;
            Rect.Opacity = 0.08;
            ladderExpander.Rect.Fill = LadderHelper.FoldingBrush;
            ladderExpander.Rect.Opacity = 0.2;
        }
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Rect.Fill = Brushes.Transparent;
            Rect.Opacity = 1;
            ladderExpander.Rect.Fill = Brushes.Transparent;
            ladderExpander.Rect.Opacity = 1;
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ladderExpander.IsExpand = !ladderExpander.IsExpand;
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_canScrollToolTip)
            {
                ScrollViewer scroll = (ScrollViewer)((ToolTip)ThumbnailButton.ToolTip).Content;
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset + e.Delta / 20);
                e.Handled = true;
            }
        }
        private void ThumbnailButton_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = false;
        }
        private void ThumbnailButton_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = true;
        }
        private ToolTip GenerateToolTipByLadder()
        {
            ToolTip tooltip = new ToolTip();
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Canvas canvas = new Canvas();
            scroll.MaxHeight = 385;
            canvas.Background = Brushes.White;
            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            ScaleTransform transform = new ScaleTransform(GlobalSetting.LadderOriginScaleX / 1.7, GlobalSetting.LadderOriginScaleY / 1.7);
            canvas.Height = _oldRowCount * HeightUnit;
            canvas.Width = LadderCanvas.Width;
            foreach (var ele in _ladderElements.Values)
            {
                canvas.Children.Add(ele);
            }
            foreach (var ele in _ladderVerticalLines.Values)
            {
                canvas.Children.Add(ele);
            }
            canvas.LayoutTransform = transform;
            scroll.Content = canvas;
            tooltip.Content = scroll;
            return tooltip;
        }
        private void RemoveToolTipByLadder(ToolTip tooltip)
        {
            if (tooltip != null)
            {
                Canvas canvas = (Canvas)((ScrollViewer)tooltip.Content).Content;
                canvas.LayoutTransform = null;
                canvas.Children.Clear();
            }
        }
        private void ExpandButton_IsExpandChanged(object sender, RoutedEventArgs e)
        {
            ExpandLadder(ladderExpander.IsExpand);
        }
        private void ExpandLadder(bool isExpand)
        {
            if (isExpand)
            {
                RemoveToolTipByLadder((ToolTip)ThumbnailButton.ToolTip);
                ThumbnailButton.ToolTip = null;
                ReloadElementsToCanvas();
            }
            else
            {
                ClearElementsFromCanvas();
                ThumbnailButton.ToolTip = GenerateToolTipByLadder();
            }
        }
        #endregion

        #region Monitor ContextMenu

        private void LadderNetworkUserControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            IMonitorManager cmanager = null;
            if (LadderMode == LadderMode.Simulate)
            {
                cmanager = SimulateHelper.SMManager;
            }
            else if (LadderMode == LadderMode.Monitor)
            {
                cmanager = _ladderDiagram.ProjectModel.MMonitorManager;
            }
            switch (_ladderDiagram.SelectionStatus)
            {
                case SelectStatus.Idle:
                case SelectStatus.SingleSelected:
                    Point pos = Mouse.GetPosition(this);
                    if (cmanager != null 
                     && cmanager.IsRunning 
                     && AcquireSelectRect(pos))
                        CM_Monitor.BVModel = _ladderDiagram.SelectionRect.CurrentElement;
                    else
                        CM_Monitor.BVModel = null;
                    break;
                default:
                    CM_Monitor.BVModel = null;
                    break;
            }
        }
        
        private void OnMonitorValueModify(object sender, ElementValueModifyEventArgs e)
        {
            BaseViewModel bvmodel = _ladderDiagram.SelectionRect.CurrentElement;
            BaseModel bmodel = bvmodel.Model;
            if (bmodel == null) return;
            int id = 0;
            for (; id < bmodel.ParaCount; id++)
            {
                if (bmodel.GetPara(id).ValueString.Equals(e.VarName))
                    break;
            }
            if (id >= bmodel.ParaCount) return;
            IMoniValueModel mvmodel = bvmodel.GetValueModel(id);
            if (LadderMode == LadderMode.Simulate)
            {
                SimulateHelper.SMManager.Handle(mvmodel, e);
            }
            else if (LadderMode == LadderMode.Monitor)
            {
                _ladderDiagram.ProjectModel.MMonitorManager.Handle(mvmodel, e);
            }
        }

        #endregion


    }
}
