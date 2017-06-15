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

using SamSoarII.Simulation.Shell;
using SamSoarII.Simulation.Shell.ViewModel;
using SamSoarII.Extend.LadderChartModel;
using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.Utility;

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// SimuViewNetworkModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewNetworkModel : UserControl
    {
        private int _rowcount;
        public int RowCount
        {
            get
            {
                return this._rowcount;
            }
            set
            {
                this._rowcount = value;
                LadderCanvas.Height = _rowcount * 300;
                this.Height = LadderCanvas.Height + 150;
            }
        }

        private int _netid;
        public int NetworkNumber
        {
            get
            {
                return this._netid;
            }
            set
            {
                this._netid = value;
                NetworkNumberLabel.Content = string.Format("Network {0:d}", _netid);
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

        private SimuViewDiagramModel _parent;
        private SimulateModel _root;

        private LadderChart _lchart;
        private LadderGraph _lgraph;
        private List<PLCInstruction> _plcinsts;

        private SortedDictionary<IntPoint, SimuViewBaseModel> _elements;
        private SortedDictionary<IntPoint, SimuViewVLineModel> _vlines;

        public LadderChart LChart
        {
            get { return this._lchart; }
        }

        public List<PLCInstruction> PLCInsts
        {
            get { return this._plcinsts; }
        }
        
        #region Initilize

        public SimuViewNetworkModel(SimuViewDiagramModel parent, int number)
        {
            InitializeComponent();
            this._parent = parent;
            this._root = parent.Root;
            NetworkNumber = number;
            this._elements = new SortedDictionary<IntPoint, SimuViewBaseModel>(new IntPointComparer());
            this._vlines = new SortedDictionary<IntPoint, SimuViewVLineModel>(new IntPointComparer());
            //Setup(lchart);
        }
        
        public void Setup(LadderChart lchart)
        {
            this._lchart = lchart;
            //RowCount = lchart.Heigh;
            /*
            IntPoint pos = null;
            SimuViewBaseModel svbmodel = null;
            SimuViewVLineModel svvmodel = null;

            foreach (LCNode lcnode in lchart.Nodes)
            {
                if (lcnode.Type == 0x00)
                {
                    if (lcnode.HAccess)
                    {
                        pos = new IntPoint(lcnode.X, lcnode.Y);
                        svbmodel = new SimuViewHLineModel(_root);
                        _elements[pos] = svbmodel;
                    }
                }
                else
                {
                    pos = new IntPoint(lcnode.X, lcnode.Y);
                    svbmodel = SimuViewBaseModel.Create(_root, lcnode.ToShowString());
                    _elements[pos] = svbmodel;
                }
                if (lcnode.VAccess)
                {
                    pos = new IntPoint(lcnode.X - 1, lcnode.Y);
                    svvmodel = new SimuViewVLineModel(_root);
                    _vlines[pos] = svvmodel;
                }
            }
            */
        }

        public void Update()
        {
            foreach (SimuViewBaseModel svbmodel in _elements.Values)
            {
                svbmodel.Dispatcher.Invoke(() => { svbmodel.Update(); });
            }
            foreach (SimuViewVLineModel svvmodel in _vlines.Values)
            {
                svvmodel.Dispatcher.Invoke(() => { svvmodel.Update(); });
            }
        }

        #endregion

        #region Selection Single Rect
        public bool IsSingleSelected()
        {
            return LadderCanvas.Children.Contains(_parent.SelectionRect);
        }

        public bool IsFirstNetwork()
        {
            return (NetworkNumber == 0);
        }

        public bool IsLastNetwork()
        {
            return (NetworkNumber == _parent.NetworkCount - 1);
        }

        public void AcquireSelectRect()
        {
            _parent.AcquireSelectRect(this);
            LadderCanvas.Children.Add(_parent.SelectionRect);
        }
        
        public void ReleaseSelectRect()
        {
            LadderCanvas.Children.Remove(_parent.SelectionRect);
        }
        #endregion

        #region Selection relative data

        public int SelectAreaOriginX
        {
            get; set;
        }
        public int SelectAreaOriginY
        {
            get; set;
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
                var left = Math.Min(_selectAreaFirstX, _selectAreaSecondX) * 300;
                var width = (Math.Abs(_selectAreaFirstX - _selectAreaSecondX) + 1) * 300;
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
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * 300;
                var height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * 300;
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
                if (value > 9)
                {
                    value = 9;
                }
                _selectAreaSecondX = value;
                var left = Math.Min(_selectAreaFirstX, _selectAreaSecondX) * 300;
                var width = (Math.Abs(_selectAreaFirstX - _selectAreaSecondX) + 1) * 300;
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
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * 300;
                var height = (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * 300;
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
                if (_isSelectAllMode)
                {
                    IsSelectAreaMode = true;
                    SelectAreaFirstY = 0;
                    SelectAreaFirstX = 0;
                    SelectAreaSecondX = 9;
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

        #region Ladder content manipulation methods
        public void UpdateRowCount()
        {
            int maxY = 0; 
            foreach (SimuViewBaseModel ele in _elements.Values)
            {
                maxY = Math.Max(maxY, ele.Y);
            }
            foreach (SimuViewVLineModel vline in _vlines.Values)
            {
                maxY = Math.Max(maxY, vline.Y);
            }
            RowCount = maxY+1;
        }

        public SimuViewBaseModel ReplaceElement(SimuViewBaseModel element)
        {
            SimuViewBaseModel oldele = null;
            bool flag = false;
            // Remove old element before
            if (element is SimuViewOutBitModel || element is SimuViewOutRecModel)
            {
                element.X = 11;
                if (element.Y >= 0 && element.Y < RowCount)
                {
                    flag = true;
                }
            }
            else
            {
                if (element.X >= 0 && element.X < 11 && element.Y >= 0 && element.Y < RowCount)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                IntPoint p = new IntPoint(element.X, element.Y);
                if (_elements.Keys.Contains(p))
                {
                    oldele = _elements[p];
                    _elements.Remove(p);
                    LadderCanvas.Children.Remove(oldele);
                }
                _elements.Add(p, element);
                LadderCanvas.Children.Add(element);
            }
            //UpdateRowCount();
            return oldele;
        }

        public SimuViewVLineModel ReplaceVerticalLine(SimuViewVLineModel vline)
        {
            IntPoint p = new IntPoint(vline.X, vline.Y);
            if (!_vlines.ContainsKey(p))
            {
                _vlines.Add(p, vline);
                LadderCanvas.Children.Add(vline);
                return null;
            }
            //UpdateRowCount();
            return vline;
        }

        public void RemoveElement(IntPoint pos)
        {
            if (_elements.ContainsKey(pos))
            {
                LadderCanvas.Children.Remove(_elements[pos]);
                _elements.Remove(pos);
            }
            //UpdateRowCount();
        }

        public void RemoveElement(int x, int y)
        {
            RemoveElement(new IntPoint(x, y));
        }

        public void RemoveElement(SimuViewBaseModel element)
        {
            if (_elements.ContainsValue(element))
            {
                RemoveElement(element.X, element.Y);
            }
        }
        public void RemoveVerticalLine(IntPoint pos)
        {
            if (_vlines.ContainsKey(pos))
            {
                LadderCanvas.Children.Remove(_vlines[pos]);
                _vlines.Remove(pos);
            }
        }
        public void RemoveVerticalLine(int x, int y)
        {
            RemoveVerticalLine(new IntPoint(x, y));
        }
        public void RemoveVerticalLine(SimuViewVLineModel vline)
        {
            if (_vlines.ContainsValue(vline))
            {
                RemoveVerticalLine(vline.X, vline.Y);
            }
        }
        #endregion

        #region Event handlers
        
        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            /*
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
            */
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
                var pos = e.GetPosition(LadderCanvas);
                var intPoint = IntPoint.GetIntpointByDouble(pos.X, pos.Y, 300);
                if (!IsSingleSelected())
                {
                    AcquireSelectRect();
                }
                _parent.SelectionRect.X = intPoint.X;
                _parent.SelectionRect.Y = intPoint.Y;
//}
        }

        #endregion

        public int CheckCircuit(TextBox report)
        {
            int ret = 0;
            _lgraph = _lchart.Generate();
            if (_lgraph.checkOpenCircuit())
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("{0:s} 的 {1:s} 出现断路错误！", _parent.Name, Name); });
                ret += 1;
            }
            if (_lgraph.checkShortCircuit())
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("{0:s} 的 {1:s} 出现短路错误！", _parent.Name, Name); });
                ret += 1;
            }
            if (ret > 0)
            {
                return ret;
            }
            if (_lgraph.CheckFusionCircuit())
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("{0:s} 的 {1:s} 出现混联错误！", _parent.Name, Name); });
                ret += 1;
            }
            return ret;
        }

        public int GenPLCInst(TextBox report)
        {
            int ret = 0;
            _plcinsts = _lgraph.GenInst();
            return ret;
        }

    }
}
