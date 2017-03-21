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
        public void PreCompile()
        {
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
                result.Add(BaseViewModel.Null);
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


        /// <summary>
        /// 判断网络是否有错误
        /// </summary>
        /// <returns>True 无错，False 有错</returns>
        public bool Assert()
        {
            return _ladderElements.Values.All(x => x.Assert());
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
