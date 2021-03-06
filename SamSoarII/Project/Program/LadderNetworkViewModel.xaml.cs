﻿using SamSoarII.AppMain.LadderCommand;
using SamSoarII.AppMain.LadderGraphModule;
using SamSoarII.AppMain.UI;
using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Communication;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.Extend.Utility;
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
using System.Threading;
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
using System.Windows.Threading;

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
    /// <summary>
    /// LadderNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderNetworkViewModel : UserControl, IComparable, INotifyPropertyChanged,IDisposable
    {
        #region Canvas System
        //private ContextMenu CM_Ladder;
        private MonitorContextMenu CM_Monitor;

        private LadderMode laddermode;
        public LadderMode LadderMode
        {
            get { return this.laddermode; }
            set
            {
                this.laddermode = value;
                _ladderBPAddrBegin = 0x3fffffff;
                _ladderBPAddrEnd = -0x3ffffffe;
                foreach (BaseViewModel bvmodel in _ladderVerticalLines.Union(_ladderElements))
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
                            if (laddermode == LadderMode.Simulate)
                            {
                                _ladderBPAddrBegin = Math.Min(
                                    _ladderBPAddrBegin, bvmodel.BPAddress);
                                _ladderBPAddrEnd = Math.Max(
                                    _ladderBPAddrEnd, bvmodel.BPAddress);
                            }
                            break;
                    }
                }
                foreach (BreakpointRect brect in _ladderBreakpoints)
                {
                    brect.Visibility = (laddermode == LadderMode.Simulate)
                        ? Visibility.Visible
                        : Visibility.Hidden;
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
                    if(value > LadderElements.Height)
                    {
                        LadderElements.Height *= 2;
                        LadderVerticalLines.Height *= 2;
                        LadderBreakpoints.Height *= 2;
                    }
                    else if (!_canHide && value < LadderElements.Height / 4 && LadderElements.Height > 8)
                    {
                        LadderElements.Height /= 2;
                        LadderVerticalLines.Height /= 2;
                        LadderBreakpoints.Height /= 2;
                    }
                }
            }
        }

        private int _networkNumber;
        public int NetworkNumber
        {
            get { return _networkNumber; }
            set
            {
                if (!IsMasked) MaskNumber = value;
                _networkNumber = value;
                NetworkNumberLabel.Content = string.Format("Network {0}", _networkNumber);
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NetworkMessage"));
                //PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NetworkNumber"));
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
        /// <summary>
        /// 屏蔽号。用于网络剪切，复制，粘贴操作时保存网络位置信息。
        /// </summary>
        public int MaskNumber { get; set; }
        private bool _isMasked;
        public bool IsInvokeByCommand = false;
        public bool IsMasked
        {
            get
            {
                return _isMasked;
            }
            set
            {
                _isMasked = value;
                if(!IsInvokeByCommand)
                    LDVModel.CommandExecute(new LadderNetworkMaskCommand(this));
                if (_isMasked)
                {
                    ReleaseSelectRect();
                    IsSelectAreaMode = false;
                    IsSelectAllMode = false;
                    CommentAreaExpander.Background = Brushes.LightGray;
                    LadderCanvas.Background = Brushes.LightGray;
                    CommentAreaExpander.Opacity = 0.4;
                    LadderCanvas.Opacity = 0.4;
                    MaskNumber = NetworkNumber;
                    ClearModelMessage();//当被屏蔽时，移除元素表中的项
                }
                else
                {
                    CommentAreaExpander.Opacity = 1.0;
                    LadderCanvas.Opacity = 1.0;
                    CommentAreaExpander.Background = Brushes.LightCyan;
                    LadderCanvas.Background = Brushes.Transparent;
                    UpdateModelMessage();//当解除屏蔽时，恢复元素表中被移除的的项
                }
                InstructionCommentManager.RaiseMappedMessageChangedEvent();//更新元素表
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsMasked"));
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
                foreach (var ele in _ladderElements)
                {
                    ele.IsCommentMode = _isCommendMode;
                }
                foreach(var vline in _ladderVerticalLines)
                {
                    vline.IsCommentMode = _isCommendMode;
                }
            }
        }
        
        private GridDictionary<BaseViewModel> _ladderElements
            = new GridDictionary<BaseViewModel>(GlobalSetting.LadderXCapacity);
        private GridDictionary<VerticalLineViewModel> _ladderVerticalLines
            = new GridDictionary<VerticalLineViewModel>(GlobalSetting.LadderXCapacity);
        private GridDictionary<BreakpointRect> _ladderBreakpoints
            = new GridDictionary<BreakpointRect>(GlobalSetting.LadderXCapacity);

        public GridDictionary<BaseViewModel> LadderElements
        {
            get
            {
                return _ladderElements;
            }
        }
        public GridDictionary<VerticalLineViewModel> LadderVerticalLines
        {
            get
            {
                return _ladderVerticalLines;
            }
        }
        public GridDictionary<BreakpointRect> LadderBreakpoints
        {
            get
            {
                return _ladderBreakpoints;
            }
        }
        public HashSet<BaseViewModel> ErrorModels { get; set; } = new HashSet<BaseViewModel>();
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

        #region Breakpoint Collection

        //private Dictionary<int, BaseViewModel> _ladderBPAddresses
        //    = new Dictionary<int, BaseViewModel>();
        private int _ladderBPAddrBegin;
        private int _ladderBPAddrEnd;

        public bool ContainBPAddr(int bpaddr)
        {
            return (bpaddr >= _ladderBPAddrBegin
                 && bpaddr <= _ladderBPAddrEnd);
        }
        
        #endregion

        public BaseViewModel GetElementByPosition(int X, int Y)
        {
            return _ladderElements[X, Y];
        }

        public VerticalLineViewModel GetVerticalLineByPosition(int X, int Y)
        {
            return _ladderVerticalLines[X, Y];
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
                if (value < 0) value = 0;
                if (value > RowCount - 1)
                    value = RowCount - 1;
                _selectAreaSecondY = value;
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * HeightUnit;
                int height;
                if (_canHide)
                    height = 0;
                else
                    height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
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
                if (!_isSelectAreaMode && value
                 && !LadderCanvas.Children.Contains(SelectArea))
                {
                    LadderCanvas.Children.Add(SelectArea);
                }
                if (!value && _isSelectAreaMode
                 && LadderCanvas.Children.Contains(SelectArea))
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
            CM_Monitor = new MonitorContextMenu(this);
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
            return _ladderElements;
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
        
        public IEnumerable<VerticalLineViewModel> GetVerticalLines()
        {
            return _ladderVerticalLines;
        }

        public BreakpointRect SearchBreakpoint(int x, int y)
        {
            return _ladderBreakpoints[x,y];
        }

        public IEnumerable<BreakpointRect> GetBreakpoint()
        {
            return _ladderBreakpoints;
        }

        #region generate LadderLogicModule
        public void InitializeLadderLogicModules()
        {
            int cnt = 0,maxY = GetMaxY();
            List<BaseViewModel> models = new List<BaseViewModel>();
            List<VerticalLineViewModel> vlines = new List<VerticalLineViewModel>();
            LadderLogicModules = new Dictionary<int, LadderLogicModule>();
            for (int i = 0; i <= maxY; i++)
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
                        if (i < maxY)
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
                oldele = _ladderElements[element.X, element.Y];
                if (oldele != null)
                {
                    if (oldele.BPRect != null)
                        RemoveBreakpoint(oldele.BPRect);
                    InstructionCommentManager.Unregister(oldele);
                    oldele.ShowPropertyDialogEvent -= this.OnShowPropertyDialog;
                    _ladderElements[oldele.X, oldele.Y] = default(BaseViewModel);
                    LadderCanvas.Children.Remove(oldele);
                }
                element.IsCommentMode = _isCommendMode;
                _ladderElements[element.X, element.Y] = element;
                LadderCanvas.Children.Add(element);
                element.ShowPropertyDialogEvent += OnShowPropertyDialog;
                InstructionCommentManager.Register(element);
                if (element.BPRect != null)
                    ReplaceBreakpoint(element.BPRect);
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = oldele;
                e.BVModel_new = element;
                ElementChanged(this, e);
            }
            return oldele;
        }
        public void ReplaceVerticalLine(VerticalLineViewModel vline)
        {
            if (_ladderVerticalLines[vline.X, vline.Y] == null)
            {
                vline.IsCommentMode = _isCommendMode;
                _ladderVerticalLines[vline.X, vline.Y] = vline;
                LadderCanvas.Children.Add(vline);
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = null;
                e.BVModel_new = vline;
                VerticalLineChanged(this, e);
            }
        }
        public void ReplaceElements(IEnumerable<BaseViewModel> elements)
        {
            foreach (var element in elements)
            {
                ReplaceElement(element);
            }
        }
        public void ReplaceVerticalLines(IEnumerable<VerticalLineViewModel> vlines)
        {
            foreach (var vline in vlines)
            {
                ReplaceVerticalLine(vline);
            }
        }
        public void ReplaceBreakpoint(BreakpointRect rect)
        {
            IntPoint p;
            if (rect.BVModel == null)
            {
                p = new IntPoint()
                {
                    X = _ladderDiagram.SelectionRect.X,
                    Y = _ladderDiagram.SelectionRect.Y
                };
                if (_ladderElements[p.X, p.Y] != null)
                    rect.BVModel = _ladderElements[p.X, p.Y];
                else
                    return;
            }
            p = new IntPoint()
            {
                X = rect.BVModel.X,
                Y = rect.BVModel.Y
            };
            BreakpointRect rect_old = _ladderBreakpoints[p.X, p.Y];
            if (rect_old != null)
            {
                LadderCanvas.Children.Remove(rect_old);
            }
            _ladderBreakpoints[p.X, p.Y] = rect;
            LadderCanvas.Children.Add(rect);
            BreakpointChanged(this, new BreakpointChangedEventArgs(
                this, rect_old, rect));
        }
        public void RemoveElement(IntPoint pos)
        {
            var ele = _ladderElements[pos.X, pos.Y];
            if (ele != null)
            {
                if (ele.BPRect != null)
                    RemoveBreakpoint(ele.BPRect);
                LadderCanvas.Children.Remove(ele);
                _ladderElements[pos.X, pos.Y] = default(BaseViewModel);
                InstructionCommentManager.Unregister(ele);
                ele.ShowPropertyDialogEvent -= this.OnShowPropertyDialog;
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = ele;
                e.BVModel_new = null;
                ElementChanged(this, e);
            }
        }
        public void RemoveElements(IEnumerable<BaseViewModel> elements)
        {
            foreach (var element in elements)
            {
                RemoveElement(element);
            }
        }
        public void RemoveVerticalLines(IEnumerable<VerticalLineViewModel> vlines)
        {
            foreach (var vline in vlines)
            {
                RemoveVerticalLine(vline);
            }
        }
        public void RemoveElement(int x, int y)
        {
            RemoveElement(new IntPoint() { X = x, Y = y });
        }
        public void RemoveElement(BaseViewModel element)
        {
            if (_ladderElements[element.X, element.Y] == element)
                RemoveElement(element.X, element.Y);
        }
        public bool RemoveVerticalLine(IntPoint pos)
        {
            var vline = _ladderVerticalLines[pos.X, pos.Y];
            if (vline != null)
            {
                LadderCanvas.Children.Remove(vline);
                _ladderVerticalLines[pos.X, pos.Y] = default(VerticalLineViewModel);
                LadderElementChangedArgs e = new LadderElementChangedArgs();
                e.BVModel_old = vline;
                e.BVModel_new = null;
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
            return _ladderVerticalLines[vline.X, vline.Y] == vline
                ? RemoveVerticalLine(vline.X, vline.Y) : false;
        }
        public bool RemoveBreakpoint(IntPoint pos)
        {
            BreakpointRect rect = _ladderBreakpoints[pos.X, pos.Y];
            if (rect != null)
            {
                _ladderBreakpoints[pos.X, pos.Y] = default(BreakpointRect);
                LadderCanvas.Children.Remove(rect);
                BreakpointChanged(this, new BreakpointChangedEventArgs(
                    this, rect, null));
                rect.BVModel = null;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool RemoveBreakpoint(int x, int y)
        {
            return RemoveBreakpoint(new IntPoint() { X = x, Y = y });
        }
        public bool RemoveBreakpoint(BreakpointRect rect)
        {
            if (rect.BVModel == null) return false;
            return RemoveBreakpoint(rect.BVModel.X, rect.BVModel.Y);
        }
        #endregion

        #region Element Manipulate when changed in same network(more efficiency)
        public void RemoveVLine(int x, int y)
        {
            var vline = _ladderVerticalLines[x, y];
            if (vline != null)
            {
                LadderCanvas.Children.Remove(vline);
                _ladderVerticalLines[x,y] = default(VerticalLineViewModel);
            }
        }
        public void ReplaceVLine(VerticalLineViewModel vline)
        {
            if (_ladderVerticalLines[vline.X, vline.Y] == null)
            {
                vline.IsCommentMode = _isCommendMode;
                _ladderVerticalLines[vline.X, vline.Y] = vline;
                LadderCanvas.Children.Add(vline);
            }
        }
        public void RemoveEle(int x, int y)
        {
            var ele = _ladderElements[x, y];
            if (ele != null)
            {
                LadderCanvas.Children.Remove(ele);
                _ladderElements[ele.X, ele.Y] = default(BaseViewModel);
            }
        }
        public BaseViewModel ReplaceEle(BaseViewModel element)
        {
            element.NetWorkNum = NetworkNumber;
            BaseViewModel oldele = _ladderElements[element.X, element.Y];
            if (oldele != null)
                LadderCanvas.Children.Remove(oldele);
            element.IsCommentMode = _isCommendMode;
            _ladderElements[element.X, element.Y] = element;
            LadderCanvas.Children.Add(element);
            return oldele;
        }
        #endregion

        #region Compile relative
        public void ClearSearchedFlag()
        {
            foreach (var model in _ladderElements)
            {
                model.IsSearched = false;
            }
            foreach (var model in _ladderVerticalLines)
            {
                model.IsSearched = false;
            }
        }
        public void ClearNextElements()
        {
            foreach (var ele in _ladderElements)
            {
                ele.NextElements.Clear();
            }
        }
        public void PreCompile()
        {
            ClearSearchedFlag();
            ClearNextElements();
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(_ladderElements.Where(x => { return x.Type == ElementType.Output; }));
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
            //graph.Convert();
            var tree = graph.ConvertToTree();
            tree.TreeName = string.Format("network_{0}", NetworkNumber);
            return tree.GenerateCode();
        }
        private void InitializeSubElements()
        {
            ClearSearchedFlag();
            ClearSubElements();
            var rootElements = _ladderElements.Where(x => { return x.Type == ElementType.Output; });
            foreach (var rootElement in rootElements)
            {
                GetSubElements(rootElement);
            }
        }
        private void ClearSubElements()
        {
            foreach (var ele in _ladderElements)
            {
                ele.SubElements.Clear();
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
            if (_ladderElements[one.X, one.Y] == null 
             && _ladderVerticalLines[two.X, two.Y] == null)
            {
                return false;
            }
            if (_ladderElements[pos.X, pos.Y] == null
             && _ladderVerticalLines[two.X, two.Y] == null)
            {
                return false;
            }
            pos.Y += 1;
            one.Y += 1;
            two.Y += 2;
            if (_ladderElements[pos.X, pos.Y] == null 
             && _ladderElements[one.X, one.Y] == null
             && _ladderVerticalLines[two.X, two.Y] == null)
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
                    BaseViewModel leftmodel = _ladderElements[p.X, p.Y];
                    if (leftmodel != null)
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
            return LadderHelper.ConvertGraph(_ladderElements);
        }

        private bool SearchUpVLine(IntPoint p)
        {
            return _ladderVerticalLines.Any(e => { return (e.X == p.X) && (e.Y == p.Y - 1); });
        }

        private bool SearchDownVLine(IntPoint p)
        {
            return _ladderVerticalLines.Any(e => { return (e.X == p.X) && (e.Y == p.Y); });
        }
        #endregion
        //返回最后一行的Y坐标
        public int GetMaxY()
        {
            return LadderElements.OrderBy(x => { return x.Y; }).Last().Y;
        }
        #region Event handlers

        public event RoutedEventHandler MaskChanged = delegate { };

        #region Relative to Element changed
        public event LadderElementChangedHandler ElementChanged = delegate { };
        public event LadderElementChangedHandler VerticalLineChanged = delegate { };
        public event BreakpointChangedEventHandler BreakpointChanged = delegate { };

        public void InvokeRemoveNetwork()
        {
            LadderElementChangedArgs e = new LadderElementChangedArgs();
            foreach (BaseViewModel ele in _ladderElements)
            {
                e.BVModel_old = ele;
                e.BVModel_new = null;
                ElementChanged(this, e);
            }
            foreach (VerticalLineViewModel vline in _ladderVerticalLines)
            {
                e.BVModel_old = vline;
                e.BVModel_new = null;
                VerticalLineChanged(this, e);
            }
            foreach (BreakpointRect bprect in _ladderBreakpoints)
            {
                BreakpointChanged(this, new BreakpointChangedEventArgs(
                    this, bprect, null));
            }
        }
        #endregion

        #region Update NetworkNum and LadderName of BaseViewModel
        public void UpdateModelMessage()
        {
            foreach (var ele in LadderElements)
            {
                ele.NetWorkNum = NetworkNumber;
                ele.RefLadderName = LDVModel.ProgramName;
            }
        }
        public void ClearModelMessage()
        {
            foreach (var ele in LadderElements)
            {
                ele.NetWorkNum = -1;
            }
        }
        #endregion

        private void OnShowPropertyDialog(BaseViewModel sender, ShowPropertyDialogEventArgs e)
        {
            var dialog = e.Dialog;
            if (dialog is ElementPropertyDialog)
            {
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
                            (FuncModel fmodel) => { return fmodel.GetMessageList(); }
                        );
                        break;
                    case ElementPropertyDialog.INST_MBUS:
                        epdialog.ModbusTables = _ladderDiagram.ProjectModel.MTVModel.Models.Select(
                            (ModbusTableModel mtmodel) => { return mtmodel.Name; }
                        );
                        break;
                }
                dialog.Commit += (sender1, e1) =>
                {
                    try
                    {
                        ElementReplaceArgumentCommand eracommand = new ElementReplaceArgumentCommand(
                            this, sender, epdialog.PropertyStrings_Old, epdialog.PropertyStrings_New);
                        _ladderDiagram.CommandExecute(eracommand);
                        dialog.Close();
                        LadderElementChangedArgs _e = new LadderElementChangedArgs();
                        _e.BVModel_old = sender;
                        _e.BVModel_new = sender;
                        ElementChanged(this, _e);
                    }
                    catch (ValueParseException ex)
                    {
                        LocalizedMessageBox.Show(ex.Message,LocalizedMessageIcon.Error);
                    }
                };
            }
            if (dialog is ElementPropertyDialog_New)
            {
                ElementPropertyDialog_New epdialog = (ElementPropertyDialog_New)dialog;
                epdialog.Details = _ladderDiagram.InstrutionNameAndToolTips[epdialog.BPModel.InstructionName];
                switch (epdialog.BPModel.InstructionName)
                {
                    case "CALL":
                    case "ATCH":
                        epdialog.SubRoutines = _ladderDiagram.ProjectModel.SubRoutines.Select(
                            (LadderDiagramViewModel ldvmodel) =>
                            {
                                return ldvmodel.ProgramName;
                            }
                        );
                        break;
                    case "CALLM":
                        epdialog.Functions = _ladderDiagram.ProjectModel.Funcs.Where(
                            (FuncModel fmodel) => { return fmodel.CanCALLM(); }
                        ).Select(
                            (FuncModel fmodel) => { return fmodel.GetMessageList(); }
                        );
                        break;
                    case "MBUS":
                        epdialog.ModbusTables = _ladderDiagram.ProjectModel.MTVModel.Models.Select(
                            (ModbusTableModel mtmodel) =>
                            {
                                return mtmodel.Name;
                            }
                        );
                        break;
                }
                IList<string> props_old = epdialog.PropertyStrings;
                epdialog.Ensure += (sender1, e1) =>
                {
                    try
                    {
                        ElementReplaceArgumentCommand eracommand = new ElementReplaceArgumentCommand(
                            this, sender, props_old, epdialog.PropertyStrings);
                        _ladderDiagram.CommandExecute(eracommand);
                        dialog.Close();
                        LadderElementChangedArgs _e = new LadderElementChangedArgs();
                        _e.BVModel_old = sender;
                        _e.BVModel_new = sender;
                        ElementChanged(this, _e);
                    }
                    catch (Exception ex)
                    {
                        LocalizedMessageBox.Show(ex.Message,LocalizedMessageIcon.Error);
                    }
                };
            }
            dialog.ShowDialog();
        }

        private void OnDeleteElement(object sender, RoutedEventArgs e)
        {
            _ladderDiagram.DeleteElementExecute();
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
            }
        }
        private void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
            {
                _ladderDiagram.NetworkRemoveRow(this, _ladderDiagram.SelectionRect.Y);
            }
            if (IsSelectAreaMode)
            {
                _ladderDiagram.NetworkRemoveRows(this, Math.Min(SelectAreaFirstY,SelectAreaSecondY),Math.Abs(SelectAreaFirstY - SelectAreaSecondY) + 1);
            }
        }
        private void OnAppendNewRow(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
            {
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
            LadderCanvas.CaptureMouse();
            AcquireSelectRect(e);
            if (e.ClickCount == 2 && LadderMode == LadderMode.Edit)
            {
                var pos = e.GetPosition(LadderCanvas);
                var intPoint = IntPoint.GetIntpointByDouble(pos.X, pos.Y, WidthUnit, HeightUnit);
                var ele = GetElementByPosition(intPoint.X,intPoint.Y);
                if (ele == null || ele.Type == ElementType.HLine || ele.Type == ElementType.Special)
                    _ladderDiagram.ShowInstructionInputDialog("");
                else
                    ele.BeginShowPropertyDialog();
            }
        }
        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            LadderCanvas.ReleaseMouseCapture();
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
                    if(ladderExpander.IsExpand && !IsMasked)
                        AcquireSelectRect(e);
                }
            }
            double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
            var point = e.GetPosition(LDVModel.MainScrollViewer);
            if (LDVModel.MainScrollViewer.ViewportHeight - point.Y < 100 * scaleY)
            {
                LDVModel.MainScrollViewer.ScrollToVerticalOffset(LDVModel.MainScrollViewer.VerticalOffset + 80 * scaleX);
            }
            else if (point.Y < 100 * scaleY)
            {
                LDVModel.MainScrollViewer.ScrollToVerticalOffset(LDVModel.MainScrollViewer.VerticalOffset - 80 * scaleY);
            }
            else if (point.X < 100 * scaleX)
            {
                LDVModel.MainScrollViewer.ScrollToHorizontalOffset(LDVModel.MainScrollViewer.HorizontalOffset - 80 * scaleY);
            }
            else if (LDVModel.MainScrollViewer.ViewportWidth - point.X < 100 * scaleX)
            {
                LDVModel.MainScrollViewer.ScrollToHorizontalOffset(LDVModel.MainScrollViewer.HorizontalOffset + 80 * scaleX);
            }
            //e.Handled = true;
        }
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (!ladderExpander.IsExpand || IsMasked) return;
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
                        LocalizedMessageBox.Show(String.Format("{0:s}{1}", fmodel.Name,Properties.Resources.Message_Can_Not_CALL),LocalizedMessageIcon.Error);
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
                    vmodel.AcceptNewValues(fmodel.Name, fmodel.Comment, avalues);
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
                        LocalizedMessageBox.Show(Properties.Resources.Message_Modbus_Table_Error,LocalizedMessageIcon.Error);
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
            if (intPoint.X < 0 || intPoint.X >= GlobalSetting.LadderXCapacity
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
        public IGridDictionarySelector<BaseViewModel> GetSelectedElements()
        {
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            return _ladderElements.SelectRange(xBegin, xEnd, yBegin, yEnd);
        }
        public List<BaseViewModel> GetSelectedHLines()
        {
            List<BaseViewModel> result = new List<BaseViewModel>();
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            return _ladderElements.SelectRange(xBegin, xEnd, yBegin, yEnd).Where(
                (ele) => { return ele is HorizontalLineViewModel; }).ToList();
        }
        public IGridDictionarySelector<VerticalLineViewModel> GetSelectedVerticalLines()
        {
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            return _ladderVerticalLines.SelectRange(xBegin, xEnd, yBegin, yEnd);
        }

        #region ladder Folding module
        private void ReloadElementsToCanvas()
        {
            if (LadderMode != LadderMode.Demo)
            {
                LadderCanvas.Children.Clear();
                RowCount = _oldRowCount;
                _canHide = false;
                foreach (var ele in _ladderElements)
                {
                    LadderCanvas.Children.Add(ele);
                }
                foreach (var ele in _ladderVerticalLines)
                {
                    LadderCanvas.Children.Add(ele);
                }
                if (IsSelectAreaMode)
                {
                    LadderCanvas.Children.Add(SelectArea);
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
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta / 10);
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
            foreach (var ele in _ladderElements)
            {
                canvas.Children.Add(ele);
            }
            foreach (var ele in _ladderVerticalLines)
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

        public void Dispose()
        {
            foreach (var element in LadderElements)
                element.Dispose();
            foreach (var vline in LadderVerticalLines)
                vline.Dispose();
            foreach (var Breakpoint in LadderBreakpoints)
                Breakpoint.Dispose();
            LadderCanvas.Children.Clear();
            InstructionCommentManager.ValueRelatedModel.Clear();
            LadderElements.Clear(0, GlobalSetting.LadderXCapacity - 1, 0, RowCount - 1);
            LadderVerticalLines.Clear(0, GlobalSetting.LadderXCapacity - 1, 0, RowCount - 1);
            LadderBreakpoints.Clear(0, GlobalSetting.LadderXCapacity - 1, 0, RowCount - 1);
            ladderExpander.MouseEnter -= OnMouseEnter;
            ladderExpander.MouseLeave -= OnMouseLeave;
            ladderExpander.expandButton.IsExpandChanged -= ExpandButton_IsExpandChanged;
            ThumbnailButton.ToolTipOpening -= ThumbnailButton_ToolTipOpening;
            ThumbnailButton.ToolTipClosing -= ThumbnailButton_ToolTipClosing;
            CM_Monitor.ValueModify -= OnMonitorValueModify;
        }
        #endregion
    }
}
