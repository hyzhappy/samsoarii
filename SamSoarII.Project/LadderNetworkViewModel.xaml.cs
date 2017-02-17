using SamSoarII.InstructionModel;
using SamSoarII.InstructionViewModel;
using SamSoarII.PLCCompiler;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
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

namespace SamSoarII.Project
{
    /// <summary>
    /// LadderNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderNetworkViewModel : UserControl
    {

        private int _rowCount;
        private int RowCount
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
                    LadderCanvas.Height = _rowCount * 300;
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
        private SortedDictionary<IntPoint, BaseViewModel> _ladderElements = new SortedDictionary<IntPoint, BaseViewModel>();
        private SortedDictionary<IntPoint, VerticalLineViewModel> _ladderVerticalLines = new SortedDictionary<IntPoint, VerticalLineViewModel>();

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
                return NetworkDescriptionTextBox.Text;
            }
            set
            {
                NetworkDescriptionTextBox.Text = value;
            }
        }

        // as parent
        private LadderDiagramViewModel _ladderDiagram;

        public LadderNetworkViewModel(LadderDiagramViewModel parent, int number)
        {
            InitializeComponent();
            _ladderDiagram = parent;
            RowCount = 1;
            NetworkNumber = number;
        }

        public IEnumerable<BaseViewModel> GetElements()
        {
            return _ladderElements.Values;
        }

        public BaseViewModel SearchElemnt(int x, int y)
        {
            return _ladderElements[new IntPoint() { X = x, Y = y }];
        }

        public VerticalLineViewModel SearchVerticalLine(int x, int y)
        {
            return _ladderVerticalLines[new IntPoint() { X = x, Y = y }];
        }

        public IEnumerable<VerticalLineViewModel> GetVerticalLines()
        {
            return _ladderVerticalLines.Values;
        }

        public void ReplaceElement(BaseViewModel element)
        {
            // Remove old element before
            if(element.Type == ElementType.Output)
            {
                element.X = 9;
            }
            IntPoint p = new IntPoint() { X = element.X, Y = element.Y };
            if(_ladderElements.Keys.Contains(p))
            {
                var oldele = _ladderElements[p];
                _ladderElements.Remove(p);
                LadderCanvas.Children.Remove(oldele);
            }
            _ladderElements.Add(p, element);
            LadderCanvas.Children.Add(element);
        }

        public void RemoveElement(IntPoint pos)
        {
            if (_ladderElements.ContainsKey(pos))
            {
                _ladderVerticalLines.Remove(pos);
                LadderCanvas.Children.Remove(_ladderElements[pos]);
            }
        }

        public void RemoveElement(int x, int y)
        {
            
        }

        public void RemoveVerticalLine(IntPoint pos)
        {
            if (_ladderVerticalLines.ContainsKey(pos))
            {
                _ladderVerticalLines.Remove(pos);
                LadderCanvas.Children.Remove(_ladderVerticalLines[pos]);
            }
        }

        public void RemoveVerticalLine(int x, int y)
        {
            IntPoint p = new IntPoint() { X = x, Y = y };
            RemoveVerticalLine(p);
        }

        public void AddVerticalLine(VerticalLineViewModel verticalLine)
        {
            IntPoint p = new IntPoint() { X = verticalLine.X, Y = verticalLine.Y };
            if (!_ladderVerticalLines.ContainsKey(p))
            {
                _ladderVerticalLines.Add(p, verticalLine);
                LadderCanvas.Children.Add(verticalLine);
            }
        }

        public void RemoveAllElements()
        {
            _ladderElements.Clear();
        }

        public void RemoveAllVerticalLines()
        {
            _ladderVerticalLines.Clear();
        }

        public void RemoveAll()
        {
            RemoveAllElements();
            RemoveAllVerticalLines();
        }

        private void AddNewRowBefore(int rowNumber)
        {
            RowCount += 1;
            var modifyKeyValuePairForElement = new List<KeyValuePair<IntPoint, BaseViewModel>>();
            var modifyKeyValuePairForVline = new List<KeyValuePair<IntPoint, VerticalLineViewModel>>();
            // step 1: 收集所有需要改变Y坐标的元素
            foreach (var pb in _ladderElements)
            {
                if (pb.Value.Y >= rowNumber)
                {
                    modifyKeyValuePairForElement.Add(pb);
                }
            }
            foreach (var pv in _ladderVerticalLines)
            {
                if (pv.Value.Y >= rowNumber)
                {
                    modifyKeyValuePairForVline.Add(pv);
                }
            }
            // step 2: 删除元素
            foreach (var pb in modifyKeyValuePairForElement)
            {
                _ladderElements.Remove(pb.Key);
            }
            foreach (var pv in modifyKeyValuePairForVline)
            {
                _ladderVerticalLines.Remove(pv.Key);
            }
            // step 3: 重新加入
            foreach (var pb in modifyKeyValuePairForElement)
            {
                pb.Value.Y += 1;
                _ladderElements.Add(new IntPoint() { X = pb.Key.X, Y = pb.Key.Y + 1 }, pb.Value);
            }
            foreach (var pv in modifyKeyValuePairForVline)
            {
                pv.Value.Y += 1;
                _ladderVerticalLines.Add(new IntPoint() { X = pv.Key.X, Y = pv.Key.Y + 1 }, pv.Value);
            }
        }

        private void AddNewRowAfter(int rowNumber)
        {
            RowCount += 1;
            var modifyKeyValuePairForElement = new List<KeyValuePair<IntPoint, BaseViewModel>>();
            var modifyKeyValuePairForVline = new List<KeyValuePair<IntPoint, VerticalLineViewModel>>();
            // step 1: 收集所有需要改变Y坐标的元素
            foreach (var pb in _ladderElements)
            {
                if (pb.Value.Y > rowNumber)
                {
                    modifyKeyValuePairForElement.Add(pb);
                }
            }
            foreach (var pv in _ladderVerticalLines)
            {
                if (pv.Value.Y > rowNumber)
                {
                    modifyKeyValuePairForVline.Add(pv);
                }
            }
            // step 2: 删除元素
            foreach (var pb in modifyKeyValuePairForElement)
            {
                _ladderElements.Remove(pb.Key);
            }
            foreach (var pv in modifyKeyValuePairForVline)
            {
                _ladderVerticalLines.Remove(pv.Key);
            }
            // step 3: 重新加入
            foreach (var pb in modifyKeyValuePairForElement)
            {
                pb.Value.Y += 1;
                _ladderElements.Add(new IntPoint() { X = pb.Key.X, Y = pb.Key.Y + 1 }, pb.Value);
            }
            foreach (var pv in modifyKeyValuePairForVline)
            {
                pv.Value.Y += 1;
                _ladderVerticalLines.Add(new IntPoint() { X = pv.Key.X, Y = pv.Key.Y + 1 }, pv.Value);
            }
        }

        private void DeleteRow(int rowNumber)
        {
            // step 1: 收集需要删除的坐标
            var deleteKeyValuePairForElement = new List<KeyValuePair<IntPoint, BaseViewModel>>();
            var deleteKeyValuePairForVline = new List<KeyValuePair<IntPoint, VerticalLineViewModel>>();
            
            foreach (var pb in _ladderElements.Where(e => e.Key.Y == rowNumber))
            {
                deleteKeyValuePairForElement.Add(pb);
            }
            foreach (var pv in _ladderVerticalLines.Where(e => e.Key.Y == rowNumber))
            {
                deleteKeyValuePairForVline.Add(pv);
            }
            // step 2: 删除
            foreach (var pb in deleteKeyValuePairForElement)
            {
                _ladderElements.Remove(pb.Key);
                LadderCanvas.Children.Remove(pb.Value);
            }
            foreach (var pv in deleteKeyValuePairForVline)
            {
                _ladderVerticalLines.Remove(pv.Key);
                LadderCanvas.Children.Remove(pv.Value);
            }

            // step 3: 收集所有需要改变Y坐标的元素
            var modifyKeyValuePairForElement = new List<KeyValuePair<IntPoint, BaseViewModel>>();
            var modifyKeyValuePairForVline = new List<KeyValuePair<IntPoint, VerticalLineViewModel>>();
        
            foreach (var pb in _ladderElements)
            {
                if (pb.Value.Y > rowNumber)
                {
                    modifyKeyValuePairForElement.Add(pb);
                }
            }
            foreach (var pv in _ladderVerticalLines)
            {
                if (pv.Value.Y > rowNumber)
                {
                    modifyKeyValuePairForVline.Add(pv);
                }
            }
            // step 4: 删除
            foreach (var pb in modifyKeyValuePairForElement)
            {
                _ladderElements.Remove(pb.Key);
            }
            foreach (var pv in modifyKeyValuePairForVline)
            {
                _ladderVerticalLines.Remove(pv.Key);
            }
            // step 5: 重新加入
            foreach (var pb in modifyKeyValuePairForElement)
            {
                pb.Value.Y -= 1;
                _ladderElements.Add(new IntPoint() { X = pb.Key.X, Y = pb.Key.Y - 1 }, pb.Value);
            }
            foreach (var pv in modifyKeyValuePairForVline)
            {
                pv.Value.Y -= 1;
                _ladderVerticalLines.Add(new IntPoint() { X = pv.Key.X, Y = pv.Key.Y - 1 }, pv.Value);
            }
            // step 6: 放置选择框
            RowCount -= 1;

        }

        private void AppendNewRow()
        {
            RowCount += 1;
        }
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
            tree.TreeName = "network1";
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

        public bool Assert()
        {
            return _ladderElements.Values.All(x => x.Assert());
        }
        #endregion

        #region Event handlers


        private void OnAddNewRowBefore(object sender, RoutedEventArgs e)
        {
            if(IsSelected())
            {
                AddNewRowBefore(_ladderDiagram.SelectionRect.Y);
            }
        }
        private void OnAddNewRowAfter(object sender, RoutedEventArgs e)
        {
            if (IsSelected())
            {
                AddNewRowAfter(_ladderDiagram.SelectionRect.Y);
            }
        }
        private void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            if (IsSelected())
            {
                DeleteRow(_ladderDiagram.SelectionRect.Y);
            }
        }

        private void OnAppendNewRow(object sender, RoutedEventArgs e)
        {
            if(IsSelected())
            {
                AppendNewRow();
            }
        }

        private void OnAppendNewNetwork(object sender, RoutedEventArgs e)
        {
            if(IsSelected())
            {
                _ladderDiagram.AppendNewNetwork();
            }
        }

        private void OnAddNewNetworkBefore(object sender, RoutedEventArgs e)
        {
            if(IsSelected())
            {
                _ladderDiagram.AddNewNetworkBefore(this);
            }
        }

        private void OnAddNewNetworkAfter(object sender, RoutedEventArgs e)
        {
            if (IsSelected())
            {
                _ladderDiagram.AddNewNetworkAfter(this);
            }
            
        }

        private void OnDeleteNetwork(object sender, RoutedEventArgs e)
        {
            if(IsSelected())
            {
                _ladderDiagram.DeleteNetwork(this);
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

        //private void OnCommentAreaDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    NetworkCommentTextBox.SelectAll();
        //}
        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(LadderCanvas);
            var intPoint = IntPoint.GetIntpointByDouble(pos.X, pos.Y, 300);
            if(!IsSelected())
            {
                AcquireSelectRect();
            }
            _ladderDiagram.SelectionRect.X = intPoint.X;
            _ladderDiagram.SelectionRect.Y = intPoint.Y;    
        }
        #endregion

        /// <summary>
        /// Be selected only if the network acquire SelecRect
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return LadderCanvas.Children.Contains(_ladderDiagram.SelectionRect);
        }

        public void ReleaseSelectRect()
        {
            if(IsSelected())
            {
                LadderCanvas.Children.Remove(_ladderDiagram.SelectionRect);
            }
        }

        private void AcquireSelectRect()
        {
            _ladderDiagram.AcquireSelectRect(this);
            LadderCanvas.Children.Add(_ladderDiagram.SelectionRect);     
        }

    }
}
